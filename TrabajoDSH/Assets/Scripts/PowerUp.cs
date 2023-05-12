using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    #region Variables

    PlayerController playerController;

    [Tooltip("Velocidad de movimiento del powerup")]
    [Range(1.0f, 20.0f)]
    [SerializeField] float velocidad = 6.0f;

    [Tooltip("Poner a false si no quieres que este powerUp se mueva al crearse")]
    [SerializeField] bool puedeMoverse = true;

    [Tooltip("Poner hacia donde se quiera que se mueva el powerUp")]
    [SerializeField] Vector3 forwardDirection = Vector3.forward;

    float raycastDistance = 1.0f;

    [Tooltip("Lista de tags a ignorar al chocarse")]
    [SerializeField] string[] reflectionIgnoreTags;

    private Rigidbody rb;

    [Tooltip("Posición donde el powerup será destruido al caer debajo de la posicion Y de puntoCaida")]
    [SerializeField] Transform puntoCaida;

    #endregion

    //* ######

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        // Encontrar el jugador:
        GameObject jugador = GameObject.Find("Jugador");

        // Obtener el script del jugador:
        playerController = jugador.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //? Comprobar si el jugador esta cerca para empezar a moverse?
        if (puedeMoverse)
        {
            // Destruir el powerUp si se cae del nivel:
            if (gameObject.transform.position.y <= puntoCaida.transform.position.y)
            {
                Destroy(gameObject);
            }

            // Mover el powerUp constantemente
            transform.Translate(forwardDirection.normalized * velocidad * Time.deltaTime, Space.World);

            // Comprobar si el powerUp se choca con una pared
            RaycastHit hit;
            if (Physics.Raycast(transform.position, forwardDirection, out hit, raycastDistance))
            {
                // Check if the collider hit has a tag that should be ignored for reflection
                if (reflectionIgnoreTags == null || Array.IndexOf(reflectionIgnoreTags, hit.collider.tag) == -1)
                {
                    // cambiar la direccion del powerUp al chocarse
                    forwardDirection = Vector3.Reflect(forwardDirection, hit.normal);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //TODO sonidos
            if (gameObject.tag == "VidaExtra")
            {
                playerController.Vidas++;
                playerController.Puntuacion += 25;
                Destroy(gameObject);
            }
            else if (gameObject.tag == "FlorDeFuego")
            {
                playerController.Puntuacion += 25;

                if (playerController.Salud == 1)
                {
                    // Aumentar tamaño
                    playerController.CambiarTamaño(true);
                }
                playerController.Salud = 3;

                //TODO Cambiar modelo jugador, igual hacerlo en playercontroler if salud == 3? usar funcion publica de playercontroller?

                Destroy(gameObject);
            }
            else if (gameObject.tag == "PowerUp")
            {
                // Si tenemos 1 de salud, nos curamos a 2 de salud, sino, ganamos puntuación
                if (playerController.Salud == 1)
                {
                    playerController.Salud = 2;
                    
                    // Aumentar tamaño
                    playerController.CambiarTamaño(true);
                }
                else
                {
                    playerController.Puntuacion += 25;
                }
                Destroy(gameObject);
            }
            else if (gameObject.tag == "Moneda")
            {
                playerController.Puntuacion += 10;
                Destroy(gameObject);
            }
        }
    }
}
