using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyShooter : MonoBehaviour
{
    [SerializeField]
    private AmmoProperties properties;

    private float _timer;
    private AbstractAmmoSpawner _defaultSpawner;



    private ObjectPoolDictionary<Bullet> _pool;
    public ObjectPoolDictionary<Bullet> Pool { get => _pool; set => _pool = value; }

    void Start()
    {
        // Initialize ammo spawner.
        _defaultSpawner = ScriptableObject.CreateInstance<LinearAmmoSpawner>();
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if(properties.FireRate > 0 && _timer > properties.FireRate)
        {
            _timer = 0f;
            Shoot();
        }
    }

    public void Shoot()
        => (properties.Spawner != null ? properties.Spawner : _defaultSpawner).Spawn(transform, properties, _pool.Get(properties.Prefab));
}
