using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var comp = gameObject.GetComponent<T>();
        if (comp == null)
            comp = gameObject.AddComponent<T>();

        return comp;
    }
}
