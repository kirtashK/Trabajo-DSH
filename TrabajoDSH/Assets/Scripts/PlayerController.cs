using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    // Input Manager:
    Controls control;
    Controls.JugadorActions jugador;

    // Movimiento horizontal:
    Vector2 horizontalInput;
    [SerializeField] CharacterController controller;
    [SerializeField] float velocidad = 11.0f;

    // Movimiento vertical:
    [SerializeField] float gravedad = -30.0f;
    Vector3 verticalVel = Vector3.zero;

    [Tooltip("Layer que el script detecta como suelo para poder saltar")]
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    // Salto:
    [SerializeField] float alturaSalto = 3.5f;
    bool jump;

    // Camara que sigue al jugador:
    [SerializeField] Camera cam;

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
}
