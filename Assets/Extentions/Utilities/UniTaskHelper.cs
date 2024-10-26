using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UniTaskHelper : MonoBehaviour
{
    private static CancellationTokenSource _masterCTS = new();

    public static void CancelAllAsynchronous()
    {
        _masterCTS.Cancel();
        _masterCTS = new();
    }
    
    public static async UniTask Delay(float second)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(second), DelayType.DeltaTime, cancellationToken: _masterCTS.Token);
    }
}
