using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class MovimientoJugador : MonoBehaviour
{
    Animator animator;
    InputMario input;
    Vector2 currentMovement;
    bool pulsandoMoverse;
    bool pulsandoCorrer;
    bool pulsandoSaltar;
    // Start is called before the first frame update
    private void Awake() {
        input = new InputMario();
        input.actionMovimiento.andar.performed += ctx =>{
            currentMovement = ctx.ReadValue<Vector2>();
            pulsandoMoverse = currentMovement.x !=0 || currentMovement.y != 0;
        };

    input.actionMovimiento.corre.performed += ctx => pulsandoCorrer=ctx.ReadValueAsButton();
    input.actionMovimiento.salto.performed += ctx => pulsandoSaltar=ctx.ReadValueAsButton();

    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        handMovement();
        handRotate();
    }


    void handMovement(){
        bool isWalking = animator.GetBool("isWalking");
        bool isRunning = animator.GetBool("isRunning");
        bool isJumping = animator.GetBool("isJumping");
        Debug.Log(isWalking);
        if( pulsandoMoverse){
            animator.SetBool("isWalking",true);
        }
        if( !pulsandoMoverse){
             animator.SetBool("isWalking",false);
        }
        if(!isJumping && pulsandoSaltar){
            animator.SetBool("isJumping",true);
        }
        if(!pulsandoSaltar){
            animator.SetBool("isJumping",false);
        }

        if(isWalking && pulsandoCorrer && !isRunning){
             animator.SetBool("isRunning",true);
        }
         if(isRunning && !pulsandoCorrer){
             animator.SetBool("isRunning",false);
        }

    }

    void handRotate(){
        Vector3 currentposition = transform.position;

        Vector3 newPosition = new Vector3(currentMovement.x,0,currentMovement.y);
        Vector3 nuevaDireccion = currentposition + newPosition;
        transform.LookAt(nuevaDireccion);

    }

    void OnEnable() {
     input.actionMovimiento.Enable();   
    }
    void OnDisable() {
       input.actionMovimiento.Disable();    
    }

}
