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
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private Transform _idleAnimationTransform;
    [SerializeField] private Transform _attackAnimationTransform;
    [SerializeField] private Transform _dieAnimationTransform;
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
        _behaviourCTS.Dispose();
        _behaviourCTS = new();
    }

    public void OnSpawn(EnemyBehaviourConfig behaviourConfig, StarfishPositionConfig positionConfig)
    {
        // Set default state
        _boxCollider.enabled = true;
        _isAlive = true;
        _positionConfig = positionConfig;

        _attackAnimationTransform.localEulerAngles = Vector3.zero;
        _dieAnimationTransform.localEulerAngles = Vector3.zero;
        _dieAnimationTransform.localPosition = Vector3.zero;

        _avatar.color = UnityEngine.Color.white;
        ChangeColor(behaviourConfig.Color, EStarfishState.Full);

        // Start
        PlayIdleAnimation();
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

        // Move in
        transform.DOMove(targetPosition, behaviourConfig.MoveDuration);
        await UniTask.WaitForSeconds(behaviourConfig.MoveDuration, cancellationToken: _behaviourCTS.Token);

        // Idle or attack
        if (behaviourConfig.IsAttack)
        {
            PerformAttack(behaviourConfig.StayInterval, behaviourConfig.AttackPercent).Forget();
        }
        
        // Stay in position
        await UniTask.WaitForSeconds(behaviourConfig.StayInterval, cancellationToken: _behaviourCTS.Token);

        // Move out
        transform.DOMove(spawnPosition, behaviourConfig.MoveDuration);
        await UniTask.WaitForSeconds(behaviourConfig.MoveDuration, cancellationToken: _behaviourCTS.Token);

        // Release to pool
        if (gameObject.activeSelf) _enemyPool.Release(this);
    }

    private async UniTask PerformAttack(float stayInterval, float attackPercent)
    {
        var availableHits = (int)Math.Floor(stayInterval);

        // Every second, random attack
        for (var i = 0; i < availableHits; i++)
        {
            var isAttack = Random.value < attackPercent;
            if (isAttack) Attack();
            await UniTask.WaitForSeconds(1f, cancellationToken: _behaviourCTS.Token);
        }
    }

    private void Attack()
    {
        PlayAttackAnimation();
        var projectile = _projectilePool.Get();
        var starfishProjectile = projectile as StarfishProjectile;
        var startPosition = projectile.transform.position = transform.position;
        var targetPosition = startPosition + Vector3.down * 25f;
        starfishProjectile?.SetColor(Color);
        projectile.Fly(targetPosition, 1f);
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
        _boxCollider.enabled = false;
        _isAlive = false;
        Die?.Invoke(this);
        CancelCurrentBehaviour();
        await PlayDieAnimation();
        // Back to pool
        if (gameObject.activeSelf) _enemyPool.Release(this);
    }

    private async UniTask PlayDieAnimation()
    {
        var animationTime = 0.8f;

        // Stop move animation
        transform.DOKill();

        // Stop attack animation
        _attackAnimationTransform.DOKill();
        _attackAnimationTransform.eulerAngles = Vector3.zero;

        // Play die animation
        _dieAnimationTransform.DOKill();
        _dieAnimationTransform.eulerAngles = Vector3.zero;
        _dieAnimationTransform.DORotate(Vector3.back * 60f, animationTime);
        _dieAnimationTransform.DOLocalMoveY(_dieAnimationTransform.localPosition.y + 5f, animationTime);

        // Fade
        _avatar.DOFade(0, animationTime);

        // Wait animation complete
        await UniTask.WaitForSeconds(animationTime, cancellationToken: _behaviourCTS.Token);
    }

    private void PlayAttackAnimation()
    {
        _attackAnimationTransform.DOKill();
        _attackAnimationTransform.DORotate(Vector3.forward * 360f, 0.8f, RotateMode.FastBeyond360)
            .OnComplete(() => _attackAnimationTransform.eulerAngles = Vector3.zero);
    }

    private void PlayIdleAnimation()
    {
        _idleAnimationTransform.DOKill();
        _idleAnimationTransform.localScale = Vector3.one;
        _idleAnimationTransform.DOScale(new Vector3(0.95f, 0.9f, 1f), 0.5f)
            .SetEase(Ease.OutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }
}

public enum EStarfishState
{
    Full,
    Haft,
    Die,
}