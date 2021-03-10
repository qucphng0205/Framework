using UnityEngine;
using System;

public enum DialogType
{
    Dialog, // previous dialog must have to hide for optimize
    Modal, // previous dialog don't have to hide
    Popup, // has its own logic (showing and hiding)
}

[RequireComponent(typeof(CanvasGroup))]
public class BaseDialog : MonoBehaviour
{
    [Header("Animation")]
    CanvasGroup _canvasGroup;
    protected CanvasGroup canvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }

    [SerializeField] protected float showingTime = 0.5f;
    [SerializeField] protected float hidingTime = 0.2f;

    [Header("Information")]
    protected DialogType type;
    public DialogType Type { get => type; }

    public virtual void Show()
    {
        canvasGroup.Showing(showingTime);
    }

    public virtual void Hide()
    {
        canvasGroup.Hiding(hidingTime);
    }
}
