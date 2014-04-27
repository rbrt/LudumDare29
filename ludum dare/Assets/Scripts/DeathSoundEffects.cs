using UnityEngine;
using System.Collections;

public class DeathSoundEffects : MonoBehaviour {

    public AudioClip[] playerDeaths;
    public AudioClip enemyDeath;

    AudioSource[] sources;
    int sourceIndex = 0;

    void Start(){
        sources = GetComponents<AudioSource>();
    }

    public void PlayEnemyDeath(){
        if (!sources[sourceIndex].isPlaying){
            sources[sourceIndex].clip = enemyDeath;
            sources[sourceIndex].Play();
            sourceIndex++;
        }

        if (sourceIndex >= sources.Length){
            sourceIndex = 0;
        }
    }

    public void PlayPlayerDeath(){
        if (!sources[sourceIndex].isPlaying){
            sources[sourceIndex].clip = playerDeaths[Random.Range(0,playerDeaths.Length-1)];
            sources[sourceIndex].Play();
            sourceIndex++;
        }

        if(sourceIndex >= sources.Length) {
            sourceIndex = 0;
        }
        
    }
}
