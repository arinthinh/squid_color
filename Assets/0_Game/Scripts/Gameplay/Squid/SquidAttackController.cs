using DG.Tweening;
using Redcode.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class SquidAttackController : SquidController
{
    [Header("PROJECTILES")]
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private SquidProjectile _projectilePrefab;

    [Header("INPUT ACTIONS")]
    [SerializeField] private InputActionReference _shootAction;

    [SerializeField] private SquidAnimator _animator;

    private ObjectPool<Projectile> _projectilePool;

    private EColor _currentInkColor;
    private bool _isCooldown;
    private Tween _shootCooldownTimer;

    private void OnEnable() => _shootAction.action.started += OnShootActionPerformed;
    private void OnDisable() => _shootAction.action.started -= OnShootActionPerformed;

    public override void Init()
    {
        _projectilePool = new ObjectPool<Projectile>(
            () =>
            {
                var newProjectile = Instantiate(_projectilePrefab, _projectileSpawnPoint);
                newProjectile.SetPool(_projectilePool);
                return newProjectile;
            },
            projectile => projectile.gameObject.SetActive(true),
            projectile => projectile.gameObject.SetActive(false),
            projectile => Destroy(projectile.gameObject)
        );
        LoadNewInk();
    }

    public void Attack()
    {
        if (_data.IsOutOfInk)
        {
            return;
        }

        if (_isCooldown) return;

        // Use color 
        _animator.PlayAnimation(SquidAnimator.EAnimation.Shoot);
        ShootInkProjectile(_currentInkColor);
        StartCooldown(_config.ShootCooldown);

        var inkLeft = _data.Inks[_currentInkColor]--;
        if (inkLeft == 0) LoadNewInk();
    }

    private void ShootInkProjectile(EColor color)
    {
        var projectile = _projectilePool.Get() as SquidProjectile;
        if (projectile == null) return;

        var spawnPositionX = _data.Position * _config.MoveDistance;
        var spawnPosition = _projectileSpawnPoint.position.WithX(spawnPositionX);
        var targetPosition = spawnPosition + Vector3.up * 25f;

        projectile.SetInfo(color, spawnPosition);
        projectile.Fly(targetPosition, 0.75f);
    }


    private void StartCooldown(float cooldownTime)
    {
        _isCooldown = true;
        _shootCooldownTimer = DOVirtual.DelayedCall(cooldownTime, () => _isCooldown = false);
    }

    private void LoadNewInk()
    {
        _currentInkColor = _data.GetValidInk();
    }

    private void OnShootActionPerformed(InputAction.CallbackContext context) => Attack();
}