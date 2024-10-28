using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    protected IObjectPool<Projectile> _pool;

    public void SetPool(IObjectPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void Fly(Vector3 flyTarget, float flyDuration)
    {
        transform.DOMove(flyTarget, flyDuration)
            .SetEase(Ease.Linear)
            .OnComplete(Disappear);
    }

    public void Disappear()
    {
        transform.DOKill();
        if (gameObject.activeSelf) _pool.Release(this);
    }
}