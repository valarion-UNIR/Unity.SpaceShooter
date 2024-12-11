using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable<T> where T : class
{
    ObjectPool<T> Pool { set; }
    void OnRelease();
    void OnGet();
}

public class ObjectPoolDictionary<T> where T : MonoBehaviour, IPoolable<T>
{
    public void Clear()
    {
        foreach(var pool in _dictionary.Values)
        {
            pool.Clear();
        }
    }

    public static T DefaultCreate(T prefab) => MonoBehaviour.Instantiate(prefab);
    public static void DefaultOnGet(T obj) => obj.gameObject.SetActive(true);
    public static void DefaultOnRelease(T obj) => obj.gameObject.SetActive(false);
    public static void DefaultOnDestroy(T obj) => obj.gameObject.SetActive(false);

    public ObjectPoolDictionary(Func<T, T> createFunc = null, Action<T> actionOnCreate = null, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, Action<T> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        _createFunc = createFunc ?? DefaultCreate;
        _actionOnCreate = actionOnCreate ?? delegate { };
        _actionOnGet = actionOnGet ?? DefaultOnGet;
        _actionOnRelease = actionOnRelease ?? DefaultOnRelease;
        _actionOnDestroy = actionOnDestroy ?? DefaultOnDestroy;
        _collectionCheck = collectionCheck;
        _defaultCapacity = defaultCapacity;
        _maxSize = maxSize;
    }
        

    private readonly Dictionary<T, ObjectPool<T>> _dictionary = new();
    private readonly Func<T, T> _createFunc;
    private readonly Action<T> _actionOnCreate;
    private readonly Action<T> _actionOnGet;
    private readonly Action<T> _actionOnRelease;
    private readonly Action<T> _actionOnDestroy;
    private readonly bool _collectionCheck;
    private readonly int _defaultCapacity;
    private readonly int _maxSize;

    public ObjectPool<T> Get(T obj)
    {
        if (!_dictionary.TryGetValue(obj, out var pool))
            _dictionary[obj] = pool = new ObjectPool<T>(() => CreateFunc(obj, _dictionary[obj]), GetFunc, ReleaseFunc, _actionOnDestroy, _collectionCheck, _defaultCapacity, _maxSize);

        return pool;
    }

    private T CreateFunc(T prefab, ObjectPool<T> pool)
    {
        var result = _createFunc(prefab);
        result.Pool = pool;
        _actionOnCreate(result);
        result.OnGet();
        return result;
    }

    private void GetFunc(T obj)
    {
        _actionOnGet(obj);
        obj.OnGet();
    }

    private void ReleaseFunc(T obj)
    {
        _actionOnRelease(obj);
        obj.OnRelease();
    }
}