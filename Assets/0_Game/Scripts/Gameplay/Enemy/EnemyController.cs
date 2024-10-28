using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public static event Action<EnemyController> Die;

    [Header("DATA")]
    [SerializeField] private EColor _currentColor;
    [SerializeField] private int _health;

    [Header("CONFIGS")]
    [SerializeField] private EnemySpritesConfigSO _sprites;
    [SerializeField] private ColorMixFormulaConfigSO _colorMixFormula;

    [Header("COMPONENTS")]
    [SerializeField] private SpriteRenderer _avatar;

    private IObjectPool<EnemyController> _enemyPool;
    private IProjectileManager _projectileManager;

    private EnemyPositionConfig _positionConfig;
    private CancellationTokenSource _behaviourCTS = new();

    public EColor Color => _currentColor;

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
        transform.localEulerAngles = Vector3.zero;
        _avatar.color = UnityEngine.Color.white;
        
        
        ChangeColor(color, EEnemyState.Full);
        _positionConfig = positionConfig;
        PerformBehaviour(behaviourConfig).Forget();
    }

    private async UniTaskVoid PerformBehaviour(EnemyBehaviourConfig behaviourConfig)
    {
        var targetPosition = _positionConfig.GetEnvironmentPosition(behaviourConfig.TargetPosition);
        var offset = behaviourConfig.MoveInDirection switch
        {
            EDirection.Left => Vector2.right * 30f,
            EDirection.Right => Vector2.left * 30f,
            EDirection.Down => Vector2.up * 30f,
            _ => Vector2.zero
        };
        var spawnPosition = targetPosition + offset;

        transform.position = spawnPosition;
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
            var moveOutTargetPosition = targetPosition - offset;
            await transform.DOMove(moveOutTargetPosition, behaviourConfig.MoveDuration)
                .WithCancellation(cancellationToken: _behaviourCTS.Token);
        }
    }

    private void Attack()
    {
        var projectTile = _projectileManager.GetProjectile();
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
        ChangeColor(newColor, EEnemyState.Die);

        _health--;
        if (_health <= 0) OnDie();
    }

    private void ChangeColor(EColor color, EEnemyState state)
    {
        _currentColor = color;
        _avatar.sprite = _sprites.GetSprite(_currentColor, state);
    }

    private void OnDie()
    {
        CancelCurrentBehaviour();
        Die?.Invoke(this);
        PlayDieAnimation().Forget();
    }

    private async UniTask PlayDieAnimation()
    {
        var animationTime = 0.8f;
        _avatar.DOFade(0, animationTime);
        transform.DORotate(Vector3.back * 60f, animationTime);
        transform.DOMoveY(transform.position.y + 5f, animationTime);
        await UniTask.WaitForSeconds(animationTime);
        _enemyPool.Release(this);
    }
}

public enum EEnemyState
{
    Full,
    Haft,
    Die,
}