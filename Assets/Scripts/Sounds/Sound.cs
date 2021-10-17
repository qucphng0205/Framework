using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public SoundName name;
    [Range(0f, 1f)]
    public float volume = 1.0f;
    public bool isNormal;

    public int simultaneousSounds = 1;
    public int maxSimultaneousSounds = 1;
}
