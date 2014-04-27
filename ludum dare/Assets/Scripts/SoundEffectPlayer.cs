using UnityEngine;
using System.Collections;

public class SoundEffectPlayer : MonoBehaviour {

    public AudioClip footstep,
                     slash;

    AudioSource source;

    void Start(){
        source = GetComponent<AudioSource>();
    }
	
    public void PlayFootstep(){
        if (!source.isPlaying){
            source.clip = footstep;
            source.Play();
        }
    }

    public void PlaySlash(){
        if (!source.isPlaying){
            source.clip = slash;
            source.Play();
        }
    }

}
