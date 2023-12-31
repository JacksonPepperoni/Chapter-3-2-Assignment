using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResourceManager : MonoBehaviour
{
    public bool Loaded { get; set; }
    private Dictionary<string, UnityEngine.Object> _resources = new();


    public void LoadAsync<T>(string key, Action<T> callback = null)
        where T : UnityEngine.Object
    {

        if (_resources.TryGetValue(key, out UnityEngine.Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        string loadKey = key;


        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        if (key.Contains(".sprite"))
        {
            var asyncOperation = Addressables.LoadAssetAsync<Sprite>(loadKey);


            asyncOperation.Completed += op =>
            {
                _resources.Add(key, op.Result);
                callback?.Invoke(op.Result as T);
            };
        }
        else
        {
            var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
            asyncOperation.Completed += op =>
            {
                _resources.Add(key, op.Result);
                callback?.Invoke(op.Result as T);
            };
        }
    }

    public void LoadAllAsync<T>(string label, Action<string, int, int> callback)
        where T : UnityEngine.Object
    {

        var operation = Addressables.LoadResourceLocationsAsync(label, typeof(T));

        operation.Completed += op =>
        {
            int loadCount = 0;
            int totalCount = op.Result.Count;


            foreach (IResourceLocation result in op.Result)
            {

                LoadAsync<T>(result.PrimaryKey, obj =>
                {
                    loadCount++;

                    callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                });
            }
        };

        Loaded = true;
    }

    public T Load<T>(string key) where T : UnityEngine.Object
    {
        if (!_resources.TryGetValue(key, out UnityEngine.Object resource))
            return null;

        return resource as T;
    }

    public void Unload<T>(string key) where T : UnityEngine.Object
    {
        if (_resources.TryGetValue(key, out UnityEngine.Object resource))
        {
            Addressables.Release(resource);
            _resources.Remove(key);
        }
        else
            Debug.LogError($"Resource Unload {key}");
    }

    public GameObject InstantiatePrefab(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>(key);
        if (prefab == null)
        {
            Debug.LogError($"[ResourceManager] Instantiate({key}): Failed to load prefab.");
            return null;
        }

        if (pooling) return Managers.Pool.Pop(prefab);

        GameObject obj = Instantiate(prefab, parent);
        obj.name = prefab.name;
        return obj;
    }

    public void Destroy(GameObject obj)
    {
        if (obj == null) return;

        if (Managers.Pool.Push(obj)) return;

        UnityEngine.Object.Destroy(obj);
    }



}
