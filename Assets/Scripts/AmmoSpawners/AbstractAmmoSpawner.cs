using System;
using UnityEngine;
using UnityEngine.Pool;

public abstract class AbstractAmmoSpawner : ScriptableObject
{
    public abstract void Spawn(Transform transform, AmmoProperties properties, ObjectPool<Bullet> pool);
}