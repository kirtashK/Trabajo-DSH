using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    float tiempoInicio;

    float raycastDistance = 1.0f;

    [Tooltip("Lista de tags a ignorar al chocarse")]
    [SerializeField] string[] reflectionIgnoreTags;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        tiempoInicio = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Borrar la bola de fuego al cabo de x segundos
        if (Time.time > tiempoInicio + 7.0f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        // Borrar la bola de fuego si se choca con un obstaculo
        RaycastHit hit;
        if (Physics.Raycast(transform.position, gameObject.transform.forward, out hit, raycastDistance))
        {
            if (reflectionIgnoreTags == null || Array.IndexOf(reflectionIgnoreTags, hit.collider.tag) == -1)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Si da a un enemigo, matarlo:
        if (other.gameObject.tag == "EnemigoLado" || other.gameObject.tag == "EnemigoTop")
        {
            Destroy(other.transform.parent.gameObject);
            Destroy(gameObject);
        }
    }
}
