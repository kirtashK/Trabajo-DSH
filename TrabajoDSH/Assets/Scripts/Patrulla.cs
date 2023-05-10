using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrulla : MonoBehaviour
{
    #region Variables

    [Tooltip("Vector de posiciones que el objeto seguirá en orden, haciendo un bucle")]
    [SerializeField] Transform[] waypoints;
    int indice = 0;
    [Tooltip("Velocidad de movimiento")]
    [SerializeField] float velocidad = 2.0f;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TODO cuando el jugador se acerca, el enemigo empieza a perseguirlo, usar AI?
        Transform wp = waypoints[indice];
        if (Vector3.Distance(transform.position, wp.position) < 0.01f)
        {
            indice = (indice + 1) % waypoints.Length;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, wp.position, velocidad * Time.deltaTime);
            transform.LookAt(wp.position);
        }
    }
}
