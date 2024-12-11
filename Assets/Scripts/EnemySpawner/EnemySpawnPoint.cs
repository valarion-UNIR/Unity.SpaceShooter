using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]
    private EnemySpawnPointType spawnPointType;
    public EnemySpawnPointType SpawnPointType => spawnPointType;
}
