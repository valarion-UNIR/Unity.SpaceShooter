using System;

[Flags]
public enum EnemySpawnPointType
{
    Up = 1 << 1,
    Center = 1 << 2,
    Down = 1 << 3,
}