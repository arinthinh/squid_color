using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static async UniTask LoadScene(string sceneName, Action onComplete = null)
    {
        var async = SceneManager.LoadSceneAsync(sceneName);
        if (async == null) return;

        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            await UniTask.Yield();
        }
        async.allowSceneActivation = true;
        await UniTask.Yield();
        onComplete?.Invoke();
    }
}