using System;
using System.Collections.Generic;
using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
    public event Action<EColor> EnemyDie;

    [SerializeField] private EnemyController _enemyPrefab;
    [SerializeField] private float _spawnPointOffset;
    [SerializeField] private List<Vector2> _spawnPoints = new List<Vector2>();

    private List<EColor> _enemyColors;
    private List<EnemyWaveConfigSO> _waves;

    private ObjectPool<EnemyController> _enemyPool;
    private Tween _spawningPatternTween;

    private void Start()
    {
        _enemyPool = new ObjectPool<EnemyController>(
            () => Instantiate(_enemyPrefab, transform),
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
        var randomPattern = _waves.GetRandomElement();
        SpawnByPattern(randomPattern);
        // Using DOTween to create a spawning loop
        _spawningPatternTween = DOVirtual.DelayedCall(randomPattern.Interval, StartSpawningLoop);
    }

    public void StopSpawningLoop()
    {
        _spawningPatternTween.Kill();
    }

    private void SpawnByPattern(EnemyWaveConfigSO pattern)
    {
        foreach (var behaviourConfig in pattern.EnemyBehaviours)
        {
            var randomColor = _enemyColors.GetRandomElement();
            SpawnEnemy(randomColor, behaviourConfig);
        }
    }

    private void SpawnEnemy(EColor color, EnemyBehaviourConfig enemyBehaviour)
    {
        var enemy = _enemyPool.Get();
        // enemy.OnSpawn();
        
    }

    private void OnEnemyDie(EColor color)
    {
        EnemyDie?.Invoke(color);
    }
}