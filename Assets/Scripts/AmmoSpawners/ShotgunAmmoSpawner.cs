using System;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "ShotgunAmmoSpawner", menuName = "AmmoSpawners/ShotgunAmmoSpawner")]
public class ShotgunAmmoSpawner : AbstractAmmoSpawner
{
    [SerializeField]
    private int bulletCount;
    [SerializeField]
    private float angle;

    public int BulletCount { get => bulletCount; set => bulletCount = value; }
    public float Angle { get => angle; set => angle = value; }

    public override void Spawn(Transform transform, AmmoProperties properties, ObjectPool<Bullet> pool)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            var shot = pool.Get();
            shot.transform.position = transform.position;
            shot.transform.eulerAngles = new Vector3(0, 0, (-angle / 2) + (i * (angle / (bulletCount-1))));
        }
    }
}