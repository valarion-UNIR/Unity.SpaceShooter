using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using static GameDefinition.Level;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameDefinition gameDefinition;
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    [SerializeField]
    private SpriteRenderer movementArea;
    [SerializeField]
    private Player player;
    [SerializeField]
    private float randomHeightRange;


    private bool _canRestart;
    private DateTime _startTime;

    private readonly List<GameObject> _enemies = new();

    private Dictionary<EnemySpawnPointType, Transform> _spawnPoints;

    private readonly ObjectPoolDictionary<Enemy> enemyPool;
    private readonly ObjectPoolDictionary<Bullet> bulletPool = new();

    public EnemySpawner()
    {
        enemyPool = new(actionOnCreate: InitializeEnemy);
    }

    // Start is called before the first frame update
    void Start()
    {
        _spawnPoints = transform.GetComponentsInChildren<EnemySpawnPoint>().ToDictionary(p => p.SpawnPointType, p => p.transform);
        StartCoroutine(SpawnLevels());
    }

    private void Update()
    {
        if (_canRestart && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }

    public void GameOver()
    {
        StopAllCoroutines();
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (!_canRestart)
        {
            var endTime = DateTime.Now;

            var time = endTime - _startTime;

            textMeshProUGUI.text = "Game over" + Environment.NewLine + "Time: " + time.ToString(@"mm\:ss") + Environment.NewLine + "Press Enter to repeat";
            _canRestart = true;
        }
    }

    private IEnumerator SpawnLevels()
    {
        _startTime = DateTime.Now;

        for(int i= 0; i < gameDefinition.levels.Length; i++)
        {
            var level = gameDefinition.levels[i];

            textMeshProUGUI.text = "Level " + (i+1);

            yield return new WaitForSeconds(2);

            textMeshProUGUI.text = string.Empty;

            foreach (var value in SpawnWaves(level))
                yield return value;

            while (_enemies.Count > 0)
                yield return new WaitForSeconds(1);
        }

        var endTime = DateTime.Now;

        var time = endTime - _startTime;

        textMeshProUGUI.text = "You win!"+Environment.NewLine+"Time: " + time.ToString(@"mm\:ss") + Environment.NewLine + "Press Enter to repeat";
        _canRestart = true;
    }

    private IEnumerable SpawnWaves(GameDefinition.Level level)
    {
        foreach(var wave in level.waves)
        {
            yield return new WaitForSeconds(wave.spawnDelay);

            foreach(EnemySpawnPointType value in Enum.GetValues(typeof(EnemySpawnPointType)))
            {
                if(wave.spawnPointType.HasFlag(value))
                {
                    var enemy = enemyPool.Get(wave.Enemy).Get();
                    var point = _spawnPoints[value].position;
                    enemy.gameObject.transform.position = new Vector3(point.x, point.y + Random.Range(-randomHeightRange, randomHeightRange), point.z);


                    _enemies.Add(enemy.gameObject);
                }
            }
        }
    }

    internal void Release(Enemy enemy)
    {
        _enemies.Remove(enemy.gameObject);
    }

    private void InitializeEnemy(Enemy enemy)
    {
        enemy.Initialize<EnemyShooter>(e => e.Pool = bulletPool);
        enemy.Initialize<Enemy>(e =>
        {
            e.Spawner = this;
            e.Spawns = gameDefinition.spawns;
        });
        enemy.Initialize<EnemyRandom>(e => e.MovementArea = movementArea);
        enemy.Initialize<EnemyRandomTeleport>(e =>
        {
            e.MovementArea = movementArea;
            e.Player = player;
        });
    }
}

public static class Extensions
{

    public static void Initialize<T>(this MonoBehaviour c, Action<T> action)
    {
        if (c.gameObject.TryGetComponent<T>(out var component))
            action(component);
    }
}