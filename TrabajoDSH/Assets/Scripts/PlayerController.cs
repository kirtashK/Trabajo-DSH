using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Controls control;
    Controls.JugadorActions jugador;

    Vector2 horizontalInput;

    [SerializeField] CharacterController controller;
    [SerializeField] float velocidad = 11.0f;

    [SerializeField] float gravedad = -30.0f;
    Vector3 verticalVel = Vector3.zero;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    [SerializeField] float alturaSalto = 3.5f;
    bool jump;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float halfHeight = controller.height * 0.5f;
        Vector3 bottomPoint = transform.TransformPoint(controller.center - Vector3.up * halfHeight);
        isGrounded = Physics.CheckSphere(bottomPoint, 0.1f, groundMask);
        if (isGrounded)
        {
            verticalVel.y = 0.0f;
        }

        Vector3 horizontalVel = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * velocidad;
        controller.Move(horizontalVel * Time.deltaTime);

        if (jump)
        {
            if (isGrounded)
            {
                verticalVel.y = Mathf.Sqrt(-2f * alturaSalto * gravedad);
            }
            jump = false;
        }

        verticalVel.y += gravedad * Time.deltaTime;
        controller.Move(verticalVel * Time.deltaTime);
    }

    void Awake()
    {
        control = new Controls();
        jugador = control.Jugador;

        // Jugador.[accion].performed += ctx => cosas a hacer
        jugador.Movimiento.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();

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
