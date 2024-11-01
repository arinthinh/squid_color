using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class StarfishsManager : MonoBehaviour
{
    /// <summary>
    /// (EColor color, Vector3 position)
    /// </summary>
    public event Action<EColor, Vector3> EnemyDie;

    [SerializeField] private StarfishBehaviourController _starfishBehaviourPrefab;
    [SerializeField] private StarfishProjectile _starfishProjectilePrefab;
    [SerializeField] private StarfishPositionConfig _positionConfig;

    private List<EnemyWaveConfig> _waves;

    private ObjectPool<StarfishBehaviourController> _enemyPool;
    private ObjectPool<Projectile> _projectilePool;

    private readonly List<Projectile> _projectileSpawned = new();
    private readonly List<StarfishBehaviourController> _curEnemies = new();

    private bool _isPerforming;
    private CancellationTokenSource _monstersBehaviourCTS;

    private void OnEnable()
    {
        StarfishBehaviourController.Die += OnEnemyDie;
    }

    private void OnDisable()
    {
        StarfishBehaviourController.Die -= OnEnemyDie;
    }

    private void Start()
    {
        _projectilePool = new ObjectPool<Projectile>(
            () =>
            {
                var newProjectile = Instantiate(_starfishProjectilePrefab, transform);
                newProjectile.SetPool(_projectilePool);
                return newProjectile;
            },
            projectile =>
            {
                _projectileSpawned.Add(projectile);
                projectile.gameObject.SetActive(true);
            },
            projectile =>
            {
                _projectileSpawned.Remove(projectile);
                projectile.gameObject.SetActive(false);
            },
            projectile => Destroy(projectile.gameObject)
        );

        _enemyPool = new ObjectPool<StarfishBehaviourController>(
            () =>
            {
                var newEnemy = Instantiate(_starfishBehaviourPrefab, transform);
                newEnemy.SetPool(_enemyPool, _projectilePool);
                return newEnemy;
            },
            enemy =>
            {
                _curEnemies.Add(enemy);
                enemy.gameObject.SetActive(true);
            },
            enemy =>
            {
                _curEnemies.Remove(enemy);
                enemy.gameObject.SetActive(false);
            });
    }

    public void Init()
    {
    }

    public void OnStartPlay(List<EnemyWaveConfig> waves)
    {
        _waves = waves;
        StartSpawningLoop().Forget();
    }

    private async UniTaskVoid StartSpawningLoop()
    {
        _isPerforming = true;
        _monstersBehaviourCTS = new();
        while (_isPerforming)
        {
            foreach (var wave in _waves)
            {
                PerformWave(wave);
                await UniTask.WaitForSeconds(wave.Interval, cancellationToken: _monstersBehaviourCTS.Token);
            }
        }
    }

    public void OnStopPlay()
    {
        _isPerforming = false;
        _monstersBehaviourCTS.Cancel();
        _monstersBehaviourCTS.Dispose();
        _monstersBehaviourCTS = new();
    }
    
    public void OnExit()
    {
        ReleaseAllMonsters();
        ReleaseAllProjectiles();
    }

    private void PerformWave(EnemyWaveConfig wave)
    {
        foreach (var behaviourConfig in wave.EnemyBehaviours)
        {
            SpawnEnemy(behaviourConfig);
        }
    }

    private void SpawnEnemy(EnemyBehaviourConfig enemyBehaviour)
    {
        var enemy = _enemyPool.Get();
        enemy.OnSpawn(enemyBehaviour, _positionConfig);
    }

    private void OnEnemyDie(StarfishBehaviourController starfishBehaviour)
    {
        EnemyDie?.Invoke(starfishBehaviour.Color, starfishBehaviour.transform.position);
    }

    public StarfishProjectile GetProjectile()
    {
        var projectile = _projectilePool.Get() as StarfishProjectile;
        _projectileSpawned.Add(projectile);
        return projectile;
    }

    private void ReleaseAllMonsters()
    {
        foreach (var enemy in _curEnemies.ToArray())
        {
            _enemyPool.Release(enemy);
        }
        _curEnemies.Clear();
    }

    private void ReleaseAllProjectiles()
    {
        foreach (var projectile in _projectileSpawned.ToArray())
        {
            if (projectile.gameObject.activeSelf) _projectilePool.Release(projectile);
        }
        _projectileSpawned.Clear();
    }
}