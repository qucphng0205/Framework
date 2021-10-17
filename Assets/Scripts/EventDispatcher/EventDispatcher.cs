using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventID {
    LoadingSuccessful,
    LoadingFailed,
    LoadingProgress
}

public class EventDispatcher : MonoSingleton<EventDispatcher>
{
    Dictionary<EventID, Action<object>> _listeners = new Dictionary<EventID, Action<object>>();

    protected override void Init()
    {
        DontDestroy();
    }

    public void RegisterListener(EventID eventID, Action<object> callback)
    {
        // check if listener exist in distionary
        if (_listeners.ContainsKey(eventID))
        {
            // add callback to our collection
            _listeners[eventID] += callback;
        }
        else
        {
            // add new key-value pair
            _listeners.Add(eventID, null);
            _listeners[eventID] += callback;
        }
    }

    public void PostEvent(EventID eventID, object param = null)
    {
        if (!_listeners.ContainsKey(eventID))
        {
            Debug.Log(eventID.ToString() + ": NO LISTENER");
            //no listerner
            return;
        }

        var callbacks = _listeners[eventID];
        // if there's no listener remain, then remove
        if (callbacks != null)
        {
            callbacks(param);
        }
        else
        {
            _listeners.Remove(eventID);
        }
    }

    public void RemoveListener(EventID eventID, Action<object> callback)
    {
        if (_listeners.ContainsKey(eventID))
        {
            _listeners[eventID] -= callback;
        }
        else
            Debug.LogWarning("not found key");
    }

    /// <summary>
    /// Clears all the listener.
    /// </summary>
    public void ClearAllListener()
    {
        _listeners.Clear();
    }
}


public static class EventDispatcherExtension
{
    /// Use for registering with EventsManager
    public static void RegisterListener(this MonoBehaviour listener, EventID eventID, Action<object> callback)
    {
        EventDispatcher.Instance.RegisterListener(eventID, callback);
    }

    public static void RemoveListener(this MonoBehaviour listener, EventID eventID, Action<object> callback)
    {
        EventDispatcher.Instance.RemoveListener(eventID, callback);
    }

    /// Post event with param
    public static void PostEvent(this MonoBehaviour listener, EventID eventID, object param)
    {
        EventDispatcher.Instance.PostEvent(eventID, param);
    }

    /// Post event with no param (param = null)
    public static void PostEvent(this MonoBehaviour sender, EventID eventID)
    {
        EventDispatcher.Instance.PostEvent(eventID, null);
    }


}