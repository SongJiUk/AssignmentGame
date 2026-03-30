using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    Dictionary<string, Object> resoucreDic = new();

    public T Load<T>(string _key) where T : Object
    {
        if(resoucreDic.TryGetValue(_key, out Object resoure)) return resoure as T;


        T res = Resources.Load<T>(_key);
        if(res == null)
        {
            Debug.LogError($"[ResourceManager] 리소스 없음  : {_key}");
            return null;
        }

        resoucreDic[_key] = res;

        return res;
    }

    public GameObject Instantiate(string _key, Transform _parent = null, bool _pooling = false)
    {
        GameObject prefab = Load<GameObject>(_key);
        if(prefab == null)
        {
            Debug.LogError($"[ResourceManager] 프리팹 없음 : {_key}");
            return null;
        }

        if (_pooling)
            return Managers.PoolM.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, _parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject _go)
    {
        if (_go == null) return;

        if (Managers.PoolM.Push(_go)) return;

        Object.Destroy(_go);
    }
}
