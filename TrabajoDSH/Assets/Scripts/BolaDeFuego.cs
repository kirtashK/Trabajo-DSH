using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BolaDeFuego : MonoBehaviour
{
    float tiempoInicio;
    public int daño = 1;
    float raycastDistance = 1.0f;

    [Tooltip("Lista de tags a ignorar al chocarse")]
    [SerializeField] string[] reflectionIgnoreTags;

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
        // Si da a un enemigo, matarlo y destruir bala:
        if (other.gameObject.tag == "EnemigoLado" || other.gameObject.tag == "EnemigoTop")
        {
            // Si el enemigo es Bowser, llamar a la funcion tocado
            if (other.gameObject.name == "Bowser")
            {
                other.transform.parent.SendMessage("tocado", daño);
            }
            else
            {
                Destroy(other.transform.parent.gameObject);
            }
            Destroy(gameObject);
        }
    }
}
