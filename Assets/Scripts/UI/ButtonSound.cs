using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// CAN OPTIMIZE
[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        SoundManager.PlayFx(SoundName.button);
    }
}
