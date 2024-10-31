using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JSAM;
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

    [Header("OTHERS CONTROLLER")]
    [SerializeField] private SquidColorSelector _colorSelector;
    [SerializeField] private SquidAnimator _animator;
    [SerializeField] private SquidStunController _stunController;

    private ObjectPool<Projectile> _projectilePool;

    private bool _isCooldown;
    private Tween _shootCooldownTimer;

    private CancellationTokenSource _cts = new();

    private void OnEnable() => _shootAction.action.started += OnShootActionPerformed;

    private void OnDisable()
    {
        _cts.Cancel();
        _cts = new();
        _shootAction.action.started -= OnShootActionPerformed;
    }

    private void Start()
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
    }

    public override void OnStartPlay()
    {
        SetEnableAttack(true);
    }

    public override void OnStopPlay()
    {
        _cts.Cancel();
        _cts = new();
        SetEnableAttack(false);
    }

    public void SetEnableAttack(bool canAttack)
    {
        if (canAttack)
        {
            _shootAction.action.Enable();
        }
        else
        {
            _shootAction.action.Disable();
        }
    }

    public void Attack()
    {
        if (_colorSelector.IsSwitching) return;
        if (_stunController.IsStun) return;

        // If is out of current ink
        if (_data.Inks[_data.CurColor] == 0)
        {
            return;
        }

        if (_isCooldown) return;

        AudioManager.PlaySound(ESound.AttackSoundSO);
        // Use color 
        _animator.PlayAnimation(SquidAnimator.EAnimation.Shoot);
        ShootInkProjectile(_data.CurColor);
        StartCooldown(_config.ShootCooldown);
        _data.Inks[_data.CurColor]--;
        OnInkUsed();
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

    private void OnInkUsed()
    {
        _colorSelector.UpdateDisplayAmount();
        foreach (var inkInfo in _data.Inks.Where(inkInfo => inkInfo.Value == 0))
        {
            ReloadInk(inkInfo.Key).Forget();
        }
    }

    private async UniTaskVoid ReloadInk(EColor inkColor)
    {
        _colorSelector.PlayReloadAnimation(inkColor, _config.InkReloadTime);
        await UniTask.WaitForSeconds(_config.InkReloadTime, cancellationToken: _cts.Token);
        _data.Inks[inkColor] = _config.MaxInkAmount;
        _colorSelector.UpdateDisplayAmount();
    }

    private void OnShootActionPerformed(InputAction.CallbackContext context) => Attack();
}