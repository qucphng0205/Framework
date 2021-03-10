using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MODEL, VIEW
public abstract class UIContainer<T, V> : MonoBehaviour
{
    [SerializeField] protected GameObject slotPrefab;
    protected List<V> slotsUI = new List<V>();

    public void Inject(List<T> models)
    {
        while (models.Count > slotsUI.Count)
        {
            slotsUI.Add(Instantiate(slotPrefab, transform).GetComponent<V>());
        }
        int i = 0;
        for (i = 0; i < models.Count; ++i)
        {
            T model = models[i];
            Setup(slotsUI[i], model);
        }

        for (; i < slotsUI.Count; ++i)
        {
            Desetup(slotsUI[i]);
        }
    }

    protected abstract void Setup(V view, T model);
    protected abstract void Desetup(V view);
}
