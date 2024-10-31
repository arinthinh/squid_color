using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JSAM;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class StarfishBehaviourController : MonoBehaviour
{
    public static event Action<StarfishBehaviourController> Die;

    [Header("DATA")]
    [SerializeField] private EColor _currentColor;
    [SerializeField] private bool _isAlive;

    [Header("CONFIGS")]
    [SerializeField] private StarfishSpritesConfigSO _sprites;
    [SerializeField] private ColorMixFormulaConfigSO _colorMixFormula;

    [Header("COMPONENTS")]
    [SerializeField] private SpriteRenderer _avatar;

    private IObjectPool<StarfishBehaviourController> _enemyPool;
    private IObjectPool<Projectile> _projectilePool;

    private StarfishPositionConfig _positionConfig;
    private CancellationTokenSource _behaviourCTS = new();

    public EColor Color => _currentColor;
    public bool IsAlive => _isAlive;

    private void OnDisable() => CancelCurrentBehaviour();

    public void SetPool(IObjectPool<StarfishBehaviourController> pool, IObjectPool<Projectile> projectilePool)
    {
        _enemyPool = pool;
        _projectilePool = projectilePool;
    }

    public void CancelCurrentBehaviour()
    {
        _behaviourCTS.Cancel();
        _behaviourCTS = new();
    }

    public void OnSpawn(EnemyBehaviourConfig behaviourConfig, StarfishPositionConfig positionConfig)
    {
        // Set default state
        _isAlive = true;
        _positionConfig = positionConfig;
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        _avatar.color = UnityEngine.Color.white;
        ChangeColor(behaviourConfig.Color, EStarfishState.Full);

        // Start
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
            PlayMoveAnimation(behaviourConfig.MoveInDirection, behaviourConfig.MoveDuration);
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
            PlayMoveAnimation(behaviourConfig.MoveOutDirection, behaviourConfig.MoveDuration);
            var moveOutTargetPosition = targetPosition - offset;
            await transform.DOMove(moveOutTargetPosition, behaviourConfig.MoveDuration)
                .WithCancellation(cancellationToken: _behaviourCTS.Token);
        }
    }

    private void Attack()
    {
        PlayAttackAnimation();
        var projectTile = _projectilePool.Get();
        var startPosition = projectTile.transform.position = transform.position;
        var targetPosition = startPosition + Vector3.down * 25f;
        projectTile.Fly(targetPosition, 1f);
    }

    private void ChangeColor(EColor color, EStarfishState state)
    {
        _currentColor = color;
        _avatar.sprite = _sprites.GetSprite(_currentColor, state);
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
        if (newColor == _currentColor)
        {
            AudioManager.PlaySound(ESound.HitSoundSO);
            return;
        }

        ChangeColor(newColor, EStarfishState.Die);
        AudioManager.PlaySound(ESound.HitSoundSO);
        OnDie().Forget();
    }

    private async UniTaskVoid OnDie()
    {
        _isAlive = false;
        CancelCurrentBehaviour();
        Die?.Invoke(this);
        await PlayDieAnimation();
        // Back to pool
        if (gameObject.activeSelf) _enemyPool.Release(this);
    }

    private async UniTask PlayDieAnimation()
    {
        var animationTime = 0.8f;
        transform.DOKill();
        transform.eulerAngles = Vector3.zero;
        _avatar.DOFade(0, animationTime);
        transform.DORotate(Vector3.back * 60f, animationTime);
        transform.DOMoveY(transform.position.y + 5f, animationTime);
        // Wait animation complete
        await UniTask.WaitForSeconds(animationTime, cancellationToken: _behaviourCTS.Token);
    }

    private void PlayAttackAnimation()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * 0.1f, 0.25f)
            .OnComplete(() => transform.localScale = Vector3.one);
    }

    private void PlayMoveAnimation(EDirection moveDirection, float moveDuration)
    {
        transform.DOKill();
        transform.eulerAngles = Vector3.zero;

        var rotateValue = moveDirection switch
        {
            EDirection.Left => 20f,
            EDirection.Right => -20f,
            _ => 0f
        };
        transform.DORotate(Vector3.forward * rotateValue, moveDuration / 2f)
            .OnComplete(() => transform.DORotate(Vector3.zero, moveDuration / 2f));
    }
}

public enum EStarfishState
{
    Full,
    Haft,
    Die,
}