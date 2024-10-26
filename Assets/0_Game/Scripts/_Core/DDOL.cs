using Cysharp.Threading.Tasks;
using UnityEngine;

public class DDOL : MonoBehaviour
{
    private void Awake() => DontDestroyOnLoad(this.gameObject);
}