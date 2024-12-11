using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyRandom : Enemy
{
    [SerializeField]
    private SpriteRenderer movementArea;

    private Vector2? _targetPosition;

    public SpriteRenderer MovementArea { get => movementArea; set => movementArea = value; }

    public EnemyShooter _enemyShooter;

    private void Start()
    {
        _enemyShooter = GetComponent<EnemyShooter>();
    }

    private void Update()
    {
        if (_targetPosition.HasValue)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition.Value, speed * Time.deltaTime);

            if (transform.position == _targetPosition)
            {
                _targetPosition = null;
                StartCoroutine(WaitAndCalculateNextTarget());
            }
        }
    }

    private IEnumerator WaitAndCalculateNextTarget()
    {
        _enemyShooter.Shoot();
        yield return new WaitForSeconds(2);
        _enemyShooter.Shoot();
        yield return new WaitForSeconds(0.5f);
        RecalculateTarget();
    }

    private void RecalculateTarget()
    {
        Vector2 newTarget;
        do {
            newTarget = new Vector2(Random.Range(movementArea.bounds.min.x, movementArea.bounds.max.x), Random.Range(movementArea.bounds.min.y, movementArea.bounds.max.y));
        } while ((newTarget - (Vector2)transform.position).sqrMagnitude > 3 * 3);
        _targetPosition = new Vector2(Random.Range(movementArea.bounds.min.x, movementArea.bounds.max.x), Random.Range(movementArea.bounds.min.y, movementArea.bounds.max.y));
    }

    public override void OnGet()
    {
        RecalculateTarget();
    }
}