using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PoolTable
{
    [SerializeField] string name;
    [SerializeField] GameObject prefab;
    [SerializeField] List<GameObject> inUse = new List<GameObject>();
    [SerializeField] List<GameObject> free = new List<GameObject>();

    public PoolTable(GameObject prefab)
    {
        this.name = prefab.name;
        this.prefab = prefab;
    }

    GameObject CreateNew(Vector3 position, Quaternion rotation) {
        return GameObject.Instantiate(prefab, position, rotation);
    }

    public void Preallocate(int count) {
        count -= inUse.Count;
        count -= free.Count;

        while (count > 0) {
            GameObject obj = CreateNew(Vector3.zero, Quaternion.identity);
            obj.SetActive(false);
            obj.transform.parent = null;
            free.Add(obj);
            --count;
        }
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation) {
        GameObject obj = null;

        if (free.Count == 0)
        {
            obj = CreateNew(position, rotation);
#if DEBUG
            Debug.LogWarning("Spawning new " + name);
#endif
        }
        else {
            obj = free[0];
            free.RemoveAt(0);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }

        inUse.Add(obj);
        return obj;
    }

    public void Despawn(GameObject obj) {
        if (!inUse.Contains(obj))
            return;
        obj.SetActive(false);
        obj.transform.parent = null;

        inUse.Remove(obj);
        free.Add(obj);
    }

    public override string ToString()
    {
        return name;
    }

}
