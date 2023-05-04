using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Variables

    // Input Manager:
    Controls control;
    Controls.JugadorActions jugador;

    // Movimiento horizontal:
    Vector2 horizontalInput;
    [Tooltip("Controlador del jugador")]
    [SerializeField] CharacterController controller;
    [Tooltip("Velocidad del jugador")]
    [SerializeField] float velocidad = 11.0f;

    // Movimiento vertical:
    [Tooltip("Gravedad del jugador")]
    [SerializeField] float gravedad = -30.0f;
    Vector3 verticalVel = Vector3.zero;
    [Tooltip("Layer que el script detecta como suelo para poder saltar")]
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    // Salto:
    [Tooltip("Altura del salto que realiza el jugador")]
    [SerializeField] float alturaSalto = 3.5f;
    bool jump;

    // Camara que sigue al jugador:
    [Tooltip("Camara asignada al jugador")]
    [SerializeField] Camera cam;

    // Salud del jugador, 2 al inicio:
    int salud = 2;

    // Vidas del jugador, no es lo mismo que salud, si la salud llega a 0, se reduce una vida y salud = 2:
    int vidas = 3;

    // Puntuación del jugador:
    int puntuacion = 0;

    [Tooltip("Posición donde se moverá el jugador al perder vida/caer")]
    [SerializeField] Transform spawn;

    [Tooltip("Posición donde el jugador será movido a spawn al caer debajo de la posicion Y de puntoCaida")]
    [SerializeField] Transform puntoCaida;

    #endregion

    //* ################

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Creamos una esfera invisible que compruebe si toca suelo con layer = Suelo, si toca suelo, isGrounded = True:
        float halfHeight = controller.height * 0.5f;
        Vector3 bottomPoint = transform.TransformPoint(controller.center - Vector3.up * halfHeight);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.1f, groundMask);
        // Si esta en el suelo, deja de ganar velocidad en y:
        if (isGrounded)
        {
            verticalVel.y = 0.0f;
        }

        // Mover el jugador horizontalmente:
        Vector3 horizontalVel = (cam.transform.right * horizontalInput.x + cam.transform.forward * horizontalInput.y) * velocidad;
        controller.Move(horizontalVel * Time.deltaTime);

        //! El jugador ha de mirar hacia donde se mueve, transform.lookat?
        //! Quizas hacer la esfera dependiendo del modelo, no del jugador entero? hijo de jugador

        // Si se pulsa saltar, si esta en el suelo, salta, sino, jump = false:
        if (jump)
        {
            if (isGrounded)
            {
                verticalVel.y = Mathf.Sqrt(-2f * alturaSalto * gravedad);
            }
            jump = false;
        }

        // Mover el jugador verticalmente:
        verticalVel.y += gravedad * Time.deltaTime;
        controller.Move(verticalVel * Time.deltaTime);

        // Salud y vidas:
        if (salud == 0 && vidas > 0)
        {
            salud = 2;
            vidas--;
            
            // Mover el jugador a la posicion de spawn
            transform.position = spawn.position; 
        }
        else if (salud == 0 && vidas == 0)
        {
            //TODO Has perdido, cargar escena de derrota
        }

        //TODO Reducir tamaño jugador si salud pasa de 2 a 1? Usar una funcion mejor, que reciba si crece o aumenta, y modificar escala Y jugador
        //TODO Cambiar modelo de fuego a normal al perder salud, usar funcion

        //TODO Flor de Fuego, solo cuando salud == 3, el jugador puede disparar bolas de fuego
        /*if (salud == 3)
        {
            // Código para comprobar si se ha pulsado keybinding para disparar
            // Disparar la bola de fuego, que se borre al cabo de x tiempo
            // Si la bola de fuego da a un enemigo, eliminarlo o hacerle daño, script en la bola que compruebe el tag
        }*/

        // Si se cae del nivel (debajo de Y de puntoCaida), mover a spawn y perder salud
        if (transform.position.y < puntoCaida.position.y)
        {
            transform.position = spawn.position;
            salud--;
        }
    }

    void Awake()
    {
        // Obtener los controles e inputs:
        control = new Controls();
        jugador = control.Jugador;

        // Jugador.[accion].performed += ctx => cosas a hacer
        // Movimiento horizontal:
        jugador.Movimiento.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        // Saltar, llama a la funcion OnJumpPressed, que asigna jump a true:
        jugador.Saltar.performed += _ => OnJumpPressed();
        //TODO Disparar bola de fuego:
    }

    void OnEnable()
    {
        control.Enable();
    }

    void OnDestroy()
    {
        control.Disable();
    }

    void OnJumpPressed()
    {
        jump = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si tocamos el lado de un enemigo, perdemos vida, si tocamos su cabeza, le matamos
        if (other.gameObject.tag == "EnemigoLado")
        {
            salud -= 1;
            puntuacion -= 50;
            Debug.Log("Has perdido salud! Salud actual: " + salud);
        }
        else if (other.gameObject.tag == "EnemigoTop")
        {
            puntuacion += 25;
            Destroy(other.transform.parent.gameObject);
            Debug.Log("Me has matado! Puntuacion: " + puntuacion);
        }
        // Codigo de los powerup:
        else if (other.gameObject.tag == "VidaExtra")
        {
            vidas++;
            Debug.Log("Has ganado una vida extra! Vidas: " + vidas);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "FlorDeFuego")
        {
            salud = 3;

            //TODO Cambiar modelo
            //TODO aumentar tamaño si salud == 1, llamar funcion para cambiar escala Y

            Debug.Log("Has cogido una flor de fuego! Salud: " + salud);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "PowerUp")
        {
            // Si tenemos 1 de salud, nos curamos a 2 de salud, sino, ganamos puntuación
            if (salud < 2)
            {
                salud = 2;
                
                //TODO Aumentar tamaño jugador? llamar funcion para cambiar escala Y
            }
            else
            {
                puntuacion += 50;
            }
            Debug.Log("Has cogido un powerup! Salud: " + salud + " Puntuacion: " + puntuacion);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Moneda")
        {
            puntuacion += 10;
            Debug.Log("Has cogido una moneda! Puntuacion: " + puntuacion);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "CambiarNivel")
        {
            puntuacion += 100;
            //TODO Cambiar nivel, scenemanagment, pedir nivel como variable publica?
        }
    }
}
