using Cysharp.Threading.Tasks;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private async UniTaskVoid Start()
    {
        await UniTask.Yield();
        await SceneLoader.LoadScene("Game");
    }
}
