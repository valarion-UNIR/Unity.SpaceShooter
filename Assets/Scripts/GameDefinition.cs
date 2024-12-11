using System;
using UnityEngine;



[CreateAssetMenu(fileName = "GameDefinition", menuName = "GameDefinition")]
public class GameDefinition : ScriptableObject
{
    [Serializable]
    public struct Level
    {
        [Serializable]
        public struct Wave
        {
            [SerializeField]
            public float spawnDelay;
            [SerializeField]
            public Enemy Enemy;
            [SerializeField]
            public EnemySpawnPointType spawnPointType;
        }

        [SerializeField]
        public Wave[] waves;
    }

    [Serializable]
    public struct Spawn
    {
        [SerializeField]
        public float probability;
        [SerializeField]
        public GameObject prefab;
    }

    public Level[] levels;
    public Spawn[] spawns;
}