using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public partial class Player : MonoBehaviour
{

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private GameObject cameraObject;

    [SerializeField]
    private EnemySpawner spawner;

    [SerializeField]
    private GameObject healthUI;

    [SerializeField, SerializedDictionary("Ammo type", "Prefab")]
    private SerializedDictionary<AmmoType, AmmoProperties> weapons;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [SerializeField]
    private AmmoType selectedWeapon;

    private Order[] _healthComponents;
    private Dictionary<AmmoType, Transform[]> _weaponCannons;
    private Camera _camera;
    private Vector2 _halfSpriteSize;
    private float _timer;
    private ObjectPoolDictionary<Bullet> shotPool = new();
    private AbstractAmmoSpawner _defaultSpawner;

    private const int maxHealth = 3;
    private int _health = maxHealth;

    private bool _canBeHurt = true;

    private bool _canShoot = true;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = transform.GetComponentInChildren<Animator>();
        _spriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(r => r.gameObject != _animator.gameObject);
        _animator.gameObject.SetActive(false);

        // Get values to calculate clamping.
        _camera = cameraObject.GetComponent<Camera>();
        var spriteRenderer = transform.GetComponentInChildren<SpriteRenderer>();
        var child = spriteRenderer.gameObject.transform;
        _halfSpriteSize = new Vector2(spriteRenderer.sprite.bounds.size.x * child.lossyScale.x, spriteRenderer.sprite.bounds.size.y * child.lossyScale.y) / 2;

        // Find and save cannon spawn points.
        _weaponCannons = transform.GetComponentsInChildren<Ammo>().GroupBy(c => c.Type).ToDictionary(grp => grp.Key, grp => grp.Select(c => c.transform).ToArray());

        // Initialize ammo spawner.
        _defaultSpawner = ScriptableObject.CreateInstance<LinearAmmoSpawner>();

        _healthComponents = healthUI.transform.GetComponentsInChildren<Order>();
    }

    public void Reset()
    {
        UpdateUI(maxHealth);
    }

    private void UpdateUI(int health)
    {
        _health = health;

        foreach (var item in _healthComponents)
        {
            item.gameObject.SetActive(health > item.order);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LimitMovement();
        Fire();
    }

    private void Fire()
    {
        _timer += Time.deltaTime;
        if (Input.GetKey(KeyCode.Space) && weapons.TryGetValue(selectedWeapon, out var properties) && _canShoot)
        {
            if (properties.FireRate < _timer && (properties.InfiniteAmmo || properties.Ammo > 0))
            {
                _timer = 0;
                foreach (var cannon in _weaponCannons[selectedWeapon])
                    (properties.Spawner ?? _defaultSpawner).Spawn(cannon.transform, properties, shotPool.Get(properties.Prefab));

                if (!properties.InfiniteAmmo)
                    properties.Ammo -= 1;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && _canBeHurt)
        {
            UpdateUI(--_health);

            if (_health <= 0)
            {
                spawner.GameOver();
                _canShoot = false;
                _spriteRenderer.gameObject.SetActive(false);
                _animator.gameObject.SetActive(true);
                _animator.Play("Smoke");
            }
            else
                StartCoroutine(InmunityFrames());
        }
        else if(collision.gameObject.layer == 12)
        {
            Destroy(collision.gameObject);
            var powerUp = collision.GetComponent<PowerUp>();
            StartCoroutine(ApplyPowerUpCoroutine(4, powerUp.Spawner));
        }
    }

    private IEnumerator InmunityFrames()
    {
        _canBeHurt = false;
        for (int i = 0; i < 10; i++)
        {
            _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.2f);
            _spriteRenderer.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(0.2f);
        }
        _canBeHurt = true;
    }

    private IEnumerator ApplyPowerUpCoroutine(float seconds, AbstractAmmoSpawner spawner)
    {
        if(weapons.TryGetValue(selectedWeapon, out var properties))
        {
            var oldProps = properties;
            weapons[selectedWeapon] = new AmmoProperties
            {
                Ammo = properties.Ammo,
                FireRate = properties.FireRate,
                InfiniteAmmo = properties.InfiniteAmmo,
                Prefab = properties.Prefab,
                Spawner = spawner,
            };
            yield return new WaitForSeconds(seconds);
            weapons[selectedWeapon] = oldProps;
        }
    }

    #region Movement

    private void LimitMovement()
    {
        var vertExtent = _camera.orthographicSize;
        var horzExtent = vertExtent * _camera.aspect;
        var newX = Mathf.Clamp(transform.position.x, -horzExtent + _halfSpriteSize.x, horzExtent - _halfSpriteSize.x);
        var newY = Mathf.Clamp(transform.position.y, -vertExtent + _halfSpriteSize.y, vertExtent - _halfSpriteSize.y);

        transform.position = new Vector3(newX, newY, transform.position.z);
    }

    private void Move()
    {
        var movementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        transform.Translate(speed * Time.deltaTime * movementDirection);
    }

    #endregion
}
