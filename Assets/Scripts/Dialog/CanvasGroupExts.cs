using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public static class CanvasGroupExts
{
    //Library: DOTween
    public static void Hiding(this CanvasGroup canvasGroup, float hidingTime = 0.2f, bool hasDeactive = true)
    {
        canvasGroup.DOKill();
        canvasGroup.blocksRaycasts = false;

        if (hasDeactive)
            canvasGroup.DOFade(0.0f, hidingTime).OnComplete(() => canvasGroup.gameObject.SetActive(false));
        else
        {
            canvasGroup.DOFade(0.0f, hidingTime);
        }
    }

    public static void Showing(this CanvasGroup canvasGroup, float showingTime = 0.3f)
    {
        if (!canvasGroup.gameObject.activeSelf)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1.0f, showingTime).OnComplete(() => canvasGroup.blocksRaycasts = true);
        }
    }

    //----------------
}
