using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animacion : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    Vector3 horizontalVel;
    // Animacion:
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
        if (gameObject.activeSelf)
        {
            horizontalVel = playerController.horizontalVel;

            CambiarAnimacion();
        }
    }

    void CambiarAnimacion()
    {
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");
        bool isJumping = animator.GetBool("isJumping");

        if (Mathf.Abs(horizontalVel.x) > 0.5f || Mathf.Abs(horizontalVel.z) > 0.5f && !isWalking && !isRunning)
        {
            //Debug.Log("Andando...");
            animator.SetBool("isWalking",true);
        }
        if (Mathf.Abs(horizontalVel.x) <= 0.5f && Mathf.Abs(horizontalVel.z) <= 0.5f && isWalking && !isRunning)
        {
            Debug.Log("Dejando de andar...");
            animator.SetBool("isWalking",false);
        }
        if (!isJumping && playerController.jump && playerController.isGrounded)
        {
            Debug.Log("Saltando...");
            animator.SetBool("isJumping",true);
        }
        if (isJumping)
        {
            Debug.Log("Dejando de saltar...");
            animator.SetBool("isJumping",false);
        }
        if (isWalking && (Mathf.Abs(horizontalVel.x) > 8 || Mathf.Abs(horizontalVel.z) > 8) && !isRunning)
        {
            Debug.Log("Corriendo...");
            animator.SetBool("isRunning",true);
        }
        if (isRunning && isWalking && Mathf.Abs(horizontalVel.x) <= 8 && Mathf.Abs(horizontalVel.z) <= 8)
        {
            Debug.Log("Dejando de correr...");
            animator.SetBool("isRunning",false);
        }
    }
}
