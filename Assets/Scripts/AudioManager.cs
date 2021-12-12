using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StringAudioClipPair {
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour {

    [SerializeField] private StringAudioClipPair[] audioClips;

    private Dictionary<string, AudioSource> audioSources;

    public void Awake() {
        audioSources = new Dictionary<string, AudioSource>(audioClips.Length);
        foreach(StringAudioClipPair pair in audioClips) {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = pair.clip;
            audioSources.Add(pair.name, newSource);
        }
    }

    public void PlaySound(string name) {
        audioSources[name].Play();
    }

}
