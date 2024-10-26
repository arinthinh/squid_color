using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static event Action<EColor> Die;

    [Header("DATA")]
    [SerializeField] private EColor _currentColor;
    [SerializeField] private int _health;

    [Header("CONFIGS")]
    [SerializeField] private ColorMixFormulaConfigSO _colorMixFormula;


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
        _health--;
        ChangeColor(inkColor);
        if (_health <= 0) OnDie();
    }

    private void ChangeColor(EColor colorAdd)
    {
        var getResult = _colorMixFormula.GetResult(_currentColor, colorAdd);
        _currentColor = getResult;
    }

    private void OnDie()
    {
        Die?.Invoke(_currentColor);
    }
}