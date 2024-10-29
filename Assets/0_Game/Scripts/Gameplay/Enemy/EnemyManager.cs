using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// (EColor color, Vector3 position)
    /// </summary>
    public event Action<EColor, Vector3> EnemyDie;

    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private EnemyProjectile _enemyProjectilePrefab;
    [SerializeField] private EnemyPositionConfig _positionConfig;

    private List<EColor> _enemyColors;
    private List<EnemyWaveConfigSO> _waves;

    private ObjectPool<EnemyController> _enemyPool;
    private ObjectPool<Projectile> _projectilePool;

    private List<Projectile> _projectileSpawned = new();

    private bool _isPerforming;
    private CancellationTokenSource _monstersBehaviourCTS;

    private void OnEnable()
    {
        EnemyController.Die += OnEnemyDie;
    }

    private void OnDisable()
    {
        EnemyController.Die -= OnEnemyDie;
    }

    private void Start()
    {
        _projectilePool = new ObjectPool<Projectile>(
            () =>
            {
                var newProjectile = Instantiate(_enemyProjectilePrefab, transform);
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

        _enemyPool = new ObjectPool<EnemyController>(
            () =>
            {
                var newEnemy = Instantiate(_enemyPrefab, transform);
                newEnemy.SetPool(_enemyPool, _projectilePool);
                return newEnemy;
            },
            enemy => enemy.gameObject.SetActive(true),
            enemy => enemy.gameObject.SetActive(false));
    }

    public void OnStartPlay(List<EColor> enemyColors, List<EnemyWaveConfigSO> waves)
    {
        _enemyColors = enemyColors;
        _waves = waves;
        StartSpawningLoop().Forget();
    }

    public void OnStopPlay()
    {
        StopAllActions();
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

    public void StopAllActions()
    {
        _isPerforming = false;
        _monstersBehaviourCTS.Cancel();
        ReleaseAllProjectiles();
    }

    private void PerformWave(EnemyWaveConfigSO wave)
    {
        foreach (var behaviourConfig in wave.EnemyBehaviours)
        {
            var randomColor = _enemyColors.GetRandomElement();
            SpawnEnemy(randomColor, behaviourConfig);
        }
    }

    private void SpawnEnemy(EColor color, EnemyBehaviourConfig enemyBehaviour)
    {
        var enemy = _enemyPool.Get();
        enemy.OnSpawn(color, enemyBehaviour, _positionConfig);
    }

    private void OnEnemyDie(EnemyController enemy)
    {
        EnemyDie?.Invoke(enemy.Color, enemy.transform.position);
    }

    public EnemyProjectile GetProjectile()
    {
        var projectile = _projectilePool.Get() as EnemyProjectile;
        _projectileSpawned.Add(projectile);
        return projectile;
    }

    private void ReleaseAllProjectiles()
    {
        foreach (var projectile in _projectileSpawned)
        {
            if (projectile.gameObject.activeSelf) _projectilePool.Release(projectile);
        }
        _projectileSpawned.Clear();
    }
}