using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly T _prefab;
    private readonly Queue<T> _objects = new Queue<T>();
    private readonly Transform _parent;

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;
        // Pre-warm the pool
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    private T CreateNewObject()
    {
        T newObj = Object.Instantiate(_prefab, _parent);
        newObj.gameObject.SetActive(false);
        _objects.Enqueue(newObj);
        return newObj;
    }

    public T Get()
    {
        if (_objects.Count == 0)
        {
            CreateNewObject();
        }

        T instance = _objects.Dequeue();
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void ReturnToPool(T instance)
    {
        instance.gameObject.SetActive(false);
        _objects.Enqueue(instance);
    }
}
