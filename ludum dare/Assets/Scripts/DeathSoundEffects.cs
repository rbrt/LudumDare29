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
            int index = Random.Range(0,playerDeaths.Length);
            if (index >= playerDeaths.Length){
                index = 2;
            }
            sources[sourceIndex].clip = playerDeaths[index];
            sources[sourceIndex].Play();
            sourceIndex++;
        }

        if(sourceIndex >= sources.Length) {
            sourceIndex = 0;
        }
        
    }
}
