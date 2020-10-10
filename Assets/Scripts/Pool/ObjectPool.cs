using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoSingleton<ObjectPool>
{
    [Serializable]
    public class Preallocation
    {
        public GameObject prefab;
        public int count;
    }

    public Preallocation[] preallocations = new Preallocation[0];

    [SerializeField] List<PoolTable> items = new List<PoolTable>();
    Dictionary<GameObject, PoolTable> poolTables = new Dictionary<GameObject, PoolTable>();
    Dictionary<GameObject, PoolInstance> poolInstances = new Dictionary<GameObject, PoolInstance>();

    private void Awake()
    {
        foreach (var item in preallocations)
        {
            DoPreallocation(item);
        }
    }

    PoolTable GetOrCreateTable(GameObject prefab)
    {
        PoolTable table;
        if (!poolTables.TryGetValue(prefab, out table))
        {
            table = new PoolTable(prefab);
            poolTables[prefab] = table;
            items.Add(table);
        }
        return table;
    }

    void DoPreallocation(Preallocation item)
    {
        GetOrCreateTable(item.prefab).Preallocate(item.count);
    }

    private GameObject InternalSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        PoolTable table = GetOrCreateTable(prefab);
        GameObject obj = table.Spawn(position, rotation);
        poolInstances[obj] = new PoolInstance(obj, true, table);
        return obj;
    }

    private void InternalDespawn(GameObject obj)
    {
        PoolInstance ins;
        if (poolInstances.TryGetValue(obj, out ins))
        {
            PoolTable table = ins.Table;

            if (table != null)
            {
                ins.InUse = false;
                table.Despawn(obj);
                return;
            }
        }
#if DEBUG
        Debug.LogWarning("Cannot find any " + obj.name + " in pool to despawn");
#endif
        Destroy(obj);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instance ? Instance.InternalSpawn(prefab, position, rotation) : null;
    }

    public static GameObject Spawn(GameObject prefab)
    {
        return Instance ? Instance.InternalSpawn(prefab, Vector3.zero, Quaternion.identity) : null;
    }

    public static void Despawn(GameObject obj)
    {
        Instance?.InternalDespawn(obj);
    }
}
