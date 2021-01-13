using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHurt : MonoBehaviour
{
    [SerializeField] private AudioClip audiohurt = null;

    private AudioSource Perso_AudioSource;

    private void awake () {
        Perso_AudioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other) {

        if(other.transform.tag == "Enemy") {

            Perso_AudioSource.PlayOneShot(audiohurt);
        }
    }
}
