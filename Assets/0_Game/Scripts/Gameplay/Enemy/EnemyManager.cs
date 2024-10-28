using System;
using System.Collections.Generic;
using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour, IProjectileManager
{
    public event Action<EColor> EnemyDie;

    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private EnemyProjectile _enemyProjectilePrefab;
    [SerializeField] private EnemyPositionConfig _positionConfig;

    private List<EColor> _enemyColors;
    private List<EnemyWaveConfigSO> _waves;

    private ObjectPool<EnemyController> _enemyPool;
    private ObjectPool<Projectile> _projectilePool;
    private List<Projectile> _projectiles = new();
    private Tween _spawningPatternTween;

    private void Start()
    {
        _projectilePool = new ObjectPool<Projectile>(
            () =>
            {
                var newProjectile = Instantiate(_enemyProjectilePrefab, transform);
                newProjectile.SetPool(_projectilePool);
                return newProjectile;
            },
            projectile => projectile.gameObject.SetActive(true),
            projectile => projectile.gameObject.SetActive(false),
            projectile => Destroy(projectile.gameObject)
        );

        _enemyPool = new ObjectPool<EnemyController>(
            () =>
            {
                var newEnemy = Instantiate(_enemyPrefab, transform);
                newEnemy.SetPool(_enemyPool, this);
                return newEnemy;
            },
            enemy => enemy.gameObject.SetActive(true),
            enemy => enemy.gameObject.SetActive(false));
    }

    public void OnStartPlay(List<EColor> enemyColors, List<EnemyWaveConfigSO> waves)
    {
        _enemyColors = enemyColors;
        _waves = waves;
    }

    private void StartSpawningLoop()
    {
        var randomWave = _waves.GetRandomElement();
        BeginWave(randomWave);
    }

    public void StopAllActions()
    {
        _spawningPatternTween.Kill();
        ReleaseAllProjectiles();
    }

    private void BeginWave(EnemyWaveConfigSO wave)
    {
        _spawningPatternTween = DOVirtual.DelayedCall(wave.Interval, StartSpawningLoop);
        foreach (var behaviourConfig in wave.EnemyBehaviours)
        {
            var randomColor = _enemyColors.GetRandomElement();
            SpawnEnemy(randomColor, behaviourConfig);
        }
    }

    private void SpawnEnemy(EColor color, EnemyBehaviourConfig enemyBehaviour)
    {
        var enemy = _enemyPool.Get();
        // enemy.OnSpawn();
        enemy.OnSpawn(color, enemyBehaviour, _positionConfig);
    }

    private void OnEnemyDie(EColor color)
    {
        EnemyDie?.Invoke(color);
    }

    public EnemyProjectile GetProjectile()
    {
        var projectile = _projectilePool.Get();
        _projectiles.Add(projectile);
        return projectile as EnemyProjectile;
    }

    public void ReturnProjectile(EnemyProjectile projectile)
    {
        _projectiles.Remove(projectile);
        _projectilePool.Release(projectile);
    }

    private void ReleaseAllProjectiles()
    {
        foreach (var projectile in _projectiles)
        {
            if (!projectile.isActiveAndEnabled) return;
            _projectilePool.Release(projectile);
        }
    }
}

public interface IProjectileManager
{
    EnemyProjectile GetProjectile();
    void ReturnProjectile(EnemyProjectile projectile);
}