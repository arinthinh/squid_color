using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static event Action<EColor> Die;

    [Header("DATA")]
    [SerializeField] private EColor _currentColor;
    [SerializeField] private int _health;

    [Header("CONFIGS")]
    [SerializeField] private ColorMixFormulaConfigSO _colorMixFormula;
    
    private IObjectPool<EnemyController> _enemyPool;
    private IProjectileManager _projectileManager;
    
    private EnemyPositionConfig _positionConfig;
    private CancellationTokenSource _behaviourCTS = new();

    public void SetPool(IObjectPool<EnemyController> pool, IProjectileManager projectileManager)
    {
        _enemyPool = pool;
        _projectileManager = projectileManager;
    }

    private void OnDisable() => CancelCurrentBehaviour();

    private void CancelCurrentBehaviour()
    {
        _behaviourCTS.Cancel();
        _behaviourCTS = new();
    }

    public void OnSpawn(EColor color, EnemyBehaviourConfig behaviourConfig, EnemyPositionConfig positionConfig)
    {
        ChangeColor(color);
        PerformBehaviour(behaviourConfig).Forget();
        _positionConfig = positionConfig;
    }

    private async UniTaskVoid PerformBehaviour(EnemyBehaviourConfig behaviourConfig)
    {
        var spawnPosition = behaviourConfig.TargetPosition;
        var targetPosition = behaviourConfig.TargetPosition;

        await MoveIn();
        if (behaviourConfig.IsAttack) PerformAttack(behaviourConfig.StayInterval).Forget();
        await UniTask.WaitForSeconds(behaviourConfig.StayInterval, cancellationToken: _behaviourCTS.Token);
        await MoveOut();
        _enemyPool.Release(this);

        return;

        async UniTask MoveIn()
        {
            await transform.DOMove(targetPosition, behaviourConfig.MoveDuration)
                .WithCancellation(cancellationToken: _behaviourCTS.Token);
        }

        async UniTask PerformAttack(float stayInterval)
        {
            var availableHits = (int)Math.Floor(stayInterval);

            // Every second, random attack
            for (var i = 0; i < availableHits; i++)
            {
                var isAttack = Random.value < 0.4f;
                if (isAttack) Attack();
                await UniTask.WaitForSeconds(1f, cancellationToken: _behaviourCTS.Token);
            }
        }

        async UniTask MoveOut()
        {
            await transform.DOMove(spawnPosition, behaviourConfig.MoveDuration)
                .WithCancellation(cancellationToken: _behaviourCTS.Token);
        }
    }

    private void Attack()
    {
        var projectTile = _projectilePool.Get();
        var startPosition = projectTile.transform.position = transform.position;
        var targetPosition = startPosition + Vector3.down * 25f;
        projectTile.Fly(targetPosition, 1f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ink"))
        {
            var projectileColor = other.GetComponent<SquidProjectile>().Color;
            OnHitInk(projectileColor);
        }
    }

    private void OnHitInk(EColor inkColor)
    {
        var newColor = _colorMixFormula.GetResult(_currentColor, inkColor);
        ChangeColor(newColor);
        
        _health--;
        if (_health <= 0) OnDie();
    }

    private void ChangeColor(EColor color)
    {
        _currentColor = color;
    }

    private void OnDie()
    {
        CancelCurrentBehaviour();
        Die?.Invoke(_currentColor);
        _enemyPool.Release(this);
    }
}