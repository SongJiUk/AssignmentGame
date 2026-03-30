using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class Pool
{
    GameObject prefab;
    IObjectPool<GameObject> pool;
    Transform root;
    private Transform Root
    {
        get
        {
            if (root == null)
            {
                GameObject go = new GameObject { name = $"{prefab.name}Root" };
                root = go.transform;
            }

            return root;
        }
    }

    public Pool(GameObject _prefab)
    {
        this.prefab = _prefab;
        pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }
    public GameObject Pop()
    {
        return pool.Get();
    }

    public void Push(GameObject _go)
    {
        pool.Release(_go);
    }

    private void OnDestroy(GameObject _go)
    {
        GameObject.Destroy(_go);
    }

    private void OnRelease(GameObject _go)
    {
        _go.SetActive(false);
    }

    private void OnGet(GameObject _go)
    {
        if (_go == null) return;
        _go.SetActive(true);
    }

    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(prefab);
        go.name = prefab.name;
        go.transform.SetParent(Root);
        return go;
    }

    public void DestroyPool()
    {
        if(root != null)
        {
            GameObject.Destroy(root.gameObject);
            root = null;
        }

        pool.Clear();
    }

  

}
public class PoolManager 
{
    Dictionary<string, Pool> pools = new();

    public GameObject Pop(GameObject _prefab)
    {
        if (_prefab == null) return null;
        if(!pools.TryGetValue(_prefab.name, out var pool))
        {
            pool = new Pool(_prefab);
            pools.Add(_prefab.name, pool);
        }

        return pool.Pop();
    }


    public bool Push(GameObject _go)
    {
        if (_go == null) return false;

        if (!pools.TryGetValue(_go.name, out var pool)) return false;

        pool.Push(_go);
        return true;
    }
}
