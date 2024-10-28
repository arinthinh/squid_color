using UnityEngine;
using UnityEngine.Pool;

public static class UnityObjectPoolCreator
{
    public static ObjectPool<T> CreatePool<T>(T prefab, Transform parent) where T : MonoBehaviour
    {
        return new ObjectPool<T>(
            () =>
            {
                var newObject = GameObject.Instantiate(prefab, parent);
                return newObject;
            },
            obj => obj.gameObject.SetActive(true),
            obj => obj.gameObject.SetActive(false),
            obj => GameObject.Destroy(obj.gameObject)
        );
    }
}