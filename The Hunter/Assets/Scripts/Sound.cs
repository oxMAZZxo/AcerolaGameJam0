using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0.01f,1f)]public float volume;
    [Range(0.5f,2f)]public float pitch;
    [Range(0f,1f)]public float spatialBlend;
    [HideInInspector]public AudioSource source;
    public bool loop;
}