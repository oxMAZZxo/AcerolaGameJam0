using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Global;
    public bool isGlobal = false;
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
        if(!isGlobal) {return;}
        if(Global != this && Global != null)
        {
            Destroy(gameObject);
        }else
        {
            Global = this;
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
        if(sound.source.isPlaying && !isGlobal) 
        {
            Debug.LogWarning("Sound is already playing");
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

    public bool IsSoundPlaying()
    {
        foreach(Sound sound in sounds)
        {
            if(sound.source.isPlaying) {return true;}
        }
        return false;
    }

    public bool IsSoundPlaying(string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.name == name);
        if(sound == null)
        {
            Debug.LogWarning("Sound with name '" + name + "' does not exist");
            return false;
        }
        if(sound.source.isPlaying) {return true;}
        return false;
    }
}