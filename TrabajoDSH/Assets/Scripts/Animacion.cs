using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animacion : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    Vector3 horizontalVel;
    // Animacion:
    Vector2 currentMovement;
    Animator animator;
    bool pulsandoMoverse;
    bool pulsandoCorrer;
    bool pulsandoSaltar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        // Obtener Animator:
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        // Si el modelo esta activado:
        if (gameObject.activeSelf)
        {
            horizontalVel = playerController.horizontalVel;

            CambiarAnimacion();
            handRotate();
        }
    }

    void CambiarAnimacion()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");
        bool isJumping = animator.GetBool("isJumping");

        float velAndar = 0.5f;
        float velCorrer = 6.0f;

        if (Mathf.Abs(horizontalVel.x) > velAndar || Mathf.Abs(horizontalVel.z) > velAndar && !isWalking && !isRunning)
        {
            //Debug.Log("Andando...");
            animator.SetBool("isWalking",true);
        }
        if (Mathf.Abs(horizontalVel.x) <= velAndar && Mathf.Abs(horizontalVel.z) <= velAndar && isWalking && !isRunning)
        {
            //Debug.Log("Dejando de andar...");
            animator.SetBool("isWalking",false);
        }
        if (isWalking && !isRunning && (Mathf.Abs(horizontalVel.x) > velCorrer || Mathf.Abs(horizontalVel.z) > velCorrer))
        {
            //Debug.Log("Corriendo...");
            animator.SetBool("isRunning",true);
        }
        if (isRunning && isWalking && Mathf.Abs(horizontalVel.x) <= velCorrer && Mathf.Abs(horizontalVel.z) <= velCorrer)
        {
            //Debug.Log("Dejando de correr...");
            animator.SetBool("isRunning",false);
        }
        if (!isJumping && playerController.jump && playerController.isGrounded)
        {
            //Debug.Log("Saltando...");
            animator.SetBool("isJumping",true);
        }
        if (isJumping)
        {
            //Debug.Log("Dejando de saltar...");
            animator.SetBool("isJumping",false);
        }
    }
        void handRotate()
    {
        Vector3 currentposition = transform.position;
        Vector3 newPosition = new Vector3(horizontalVel.x,0,horizontalVel.z);
        Vector3 nuevaDireccion = currentposition + newPosition;
        transform.LookAt(nuevaDireccion);
    }
}
