using UnityEngine;
using System;

public enum DialogState
{
    PlayingAnimation,
    Hide,
    Show,
}

public class BaseDialog : MonoBehaviour
{
    protected object data;
    protected DialogState state;

    protected Action showCallBack;
    protected Action hideCallback;

    public virtual void OnShow(object data = null, Action showCallback = null, Action hideCallback = null)
    {
        this.data = data;
        state = DialogState.Show;

        this.hideCallback = hideCallback;
        showCallBack?.Invoke();
        transform.SetAsLastSibling();
    }

    public virtual void OnHide()
    {
        state = DialogState.Hide;
        hideCallback?.Invoke();
    }
}
