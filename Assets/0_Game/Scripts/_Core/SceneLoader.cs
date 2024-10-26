using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    public static async UniTask LoadScene(string sceneName, float delayTime = 0f, Action onComplete = null)
    {
        await UniTask.Yield();
        var async = SceneManager.LoadSceneAsync(sceneName);
        if (async == null) return;

        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            await UniTask.Yield();
        }
        await UniTask.Yield();
        await UniTask.WaitForSeconds(delayTime);
        async.allowSceneActivation = true;
        await UniTask.Yield();
        onComplete?.Invoke();
    }
}