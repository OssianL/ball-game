using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StringAudioClipPair {
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour {

    [SerializeField] private StringAudioClipPair[] soundEffects;
    [SerializeField] private StringAudioClipPair[] music;

    private Dictionary<string, AudioSource> soundEffectSources;
    private Dictionary<string, AudioSource> musicSources;

    public void Awake() {
        CreateSoundSources();
    }

    public void PlaySoundEffect(string name) {
        soundEffectSources[name].Play();
    }

    public void StartMusic(string name, bool loop = true) {
        StopMusic();
        musicSources[name].loop = loop;
        musicSources[name].Play();
    }

    public void StopMusic() {
        foreach(AudioSource source in musicSources.Values) {
            source.Stop();
        }
    }

    private void CreateSoundSources() {
        soundEffectSources = new Dictionary<string, AudioSource>(soundEffects.Length);
        foreach(StringAudioClipPair pair in soundEffects) {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = pair.clip;
            soundEffectSources.Add(pair.name, newSource);
        }
        musicSources = new Dictionary<string, AudioSource>(music.Length);
        foreach(StringAudioClipPair pair in music) {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = pair.clip;
            musicSources.Add(pair.name, newSource);
        }
    }

}
