using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    
    private AudioSource audio;
    [SerializeField] private AudioClip ring;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio == null) audio = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            audio.PlayOneShot(ring);
        }
    }
}