using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour, IPoolable<Bullet>
{
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float lifespan = 4f;

    [SerializeField]
    private Vector2 direction = Vector2.right;

    private ObjectPool<Bullet> _pool;

    public ObjectPool<Bullet> Pool { get => _pool; set => _pool = value; }
    public Vector2 Direction { get => direction; set => direction = value; }

    private float _timer;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * direction);

        _timer += Time.deltaTime;

        if (_timer > lifespan && isActiveAndEnabled)
            _pool.Release(this);
    }

    //Just overlapped a collider 2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 11 && isActiveAndEnabled)
            _pool.Release(this);
    }

    public virtual void OnRelease()
    {
    }

    public virtual void OnGet()
    {
        _timer = 0;
    }
}
