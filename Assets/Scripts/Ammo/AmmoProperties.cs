using System;

[Serializable]
public struct AmmoProperties
{
    public Bullet Prefab;
    public bool InfiniteAmmo;
    public int Ammo;
    public float FireRate;
    //[NonSerialized]
    public AbstractAmmoSpawner Spawner;
}

