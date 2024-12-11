using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "LinearAmmoSpawner", menuName = "AmmoSpawners/LinearAmmoSpawner")]
public class LinearAmmoSpawner : AbstractAmmoSpawner
{
    public override void Spawn(Transform transform, AmmoProperties properties, ObjectPool<Bullet> pool)
    {
        var shot = pool.Get();
        shot.transform.position = transform.position;
        shot.transform.right = transform.right;
    }
}
