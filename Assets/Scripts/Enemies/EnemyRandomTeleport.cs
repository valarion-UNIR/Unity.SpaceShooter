using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyRandomTeleport : Enemy
{
    [SerializeField]
    private SpriteRenderer movementArea;
    [SerializeField]
    private Player player;

    private Animator _animator;
    private GameObject _sprite;
    private Vector2 _targetPosition;
    private BoxCollider2D _collider;

    public SpriteRenderer MovementArea { get => movementArea; set => movementArea = value; }
    public Player Player { get => player; set => player = value; }

    public EnemyShooter _enemyShooter;

    public bool _firstUpdate = true;

    private void Start()
    {
        _enemyShooter = GetComponent<EnemyShooter>();
        _collider = GetComponent<BoxCollider2D>();
        _animator = transform.GetComponentInChildren<Animator>();
        _sprite = transform.GetComponentInChildren<EntitySprite>().gameObject;
    }

    private void Update()
    {
        if(_firstUpdate)
        {
            StartCoroutine(ShootMoveAndShoot());
            _firstUpdate = false;
        }
    }


    private IEnumerator ShootMoveAndShoot()
    {
        while (true)
        {
            RecalculateTarget();
            _animator.gameObject.SetActive(true);
            _sprite.SetActive(false);
            _collider.enabled = false;
            _animator.Play("OutAnimation");
            yield return new WaitForSeconds(0.5f);
            transform.position = _targetPosition;
            transform.right = transform.position - player.transform.position;
            _animator.Play("InAnimation");
            yield return new WaitForSeconds(0.5f);
            _sprite.SetActive(true);
            _collider.enabled = true;
            _animator.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _enemyShooter.Shoot();
            yield return new WaitForSeconds(0.5f + Random.value);
            _enemyShooter.Shoot();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RecalculateTarget()
    {
        Vector2 newTarget;
        do {
            newTarget = new Vector2(Random.Range(movementArea.bounds.min.x, movementArea.bounds.max.x), Random.Range(movementArea.bounds.min.y, movementArea.bounds.max.y));
        } while ((newTarget - (Vector2)transform.position).sqrMagnitude < 4 * 4);
        _targetPosition = newTarget;
    }

    public override void OnRelease()
    {
        StopAllCoroutines();
    }

    public override void OnGet()
    {
        _firstUpdate = true;
    }
}