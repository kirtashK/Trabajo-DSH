using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectarJugador : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent mover;
    public float velocidad;
    public float Rango;

    public LayerMask capaJugador;
    //Guardar la posición exacta de nuestro jugador
    public Transform player;
    //Variable que se activara en el momento que el jugador entre en el rango del enemigo
    bool alerta;
    
    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //mover.SetDestination(player.position);
        //Para crear una esfera al rededor del enemigo que se activara en el momento que nosotros nos acerquemos a esta.
        alerta = Physics.CheckSphere(transform.position, Rango, capaJugador);

        if(alerta == true)
        {
            //Para que el enemigo mire hacia nostros.
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(player.position.x, transform.position.y, player.position.z),velocidad * Time.deltaTime);
        }
    }
}
