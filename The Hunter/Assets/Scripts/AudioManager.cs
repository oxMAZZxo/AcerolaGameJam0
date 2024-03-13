using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] sounds;

    void Awake()
    {
        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.playOnAwake = false;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = sound.spatialBlend;
        }
        
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if(sound == null) 
        {
            Debug.LogWarning("Sound with name '" + name + "' does not exist");
            return;
        }
        sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound with name '" + name + "' does not exist");
            return;
        }
        if(sound.source.isPlaying)
        {
            sound.source.Stop();
        }else
        {
            Debug.LogWarning("Sound with name '" + name + "' is not playing");
        }
    }
}