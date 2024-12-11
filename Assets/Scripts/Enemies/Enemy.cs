using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class Enemy : MonoBehaviour, IPoolable<Enemy>
{
    [SerializeField]
    protected float speed;

    private ObjectPool<Enemy> _pool;
    private EnemySpawner _spawner;
    private GameDefinition.Spawn[] _spawns;

    public ObjectPool<Enemy> Pool { get => _pool; set => _pool = value; }

    public EnemySpawner Spawner { get => _spawner; set => _spawner = value; }
    public GameDefinition.Spawn[] Spawns { get => _spawns; set => _spawns = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isActiveAndEnabled) return;
        if (collision.gameObject.layer == 11)
        {
            _pool.Release(this);
            _spawner.Release(this);
        }
        else if (collision.gameObject.layer == 8)
        {
            var bullet = collision.GetComponent<Bullet>();
            if(bullet.isActiveAndEnabled)
                bullet.Pool.Release(bullet);
            // TODO: Explosion
            _pool.Release(this);
            _spawner.Release(this);

            foreach(var spawn in _spawns)
            {
                if(Random.value < spawn.probability)
                {
                    Instantiate(spawn.prefab, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
    }

    public virtual void OnRelease()
    {
    }

    public virtual void OnGet()
    {
    }
}
