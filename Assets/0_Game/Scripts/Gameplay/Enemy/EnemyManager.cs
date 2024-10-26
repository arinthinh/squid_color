using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public event Action<EColor> EnemyDie;
    
    public void OnStartPlay(List<EColor> enemyColors)
    {
        
    }

    private void SpawnEnemy()
    {
        
    }

    private void OnEnemyDie(EColor color)
    {
        EnemyDie?.Invoke(color);
    }
}