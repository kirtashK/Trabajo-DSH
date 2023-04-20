using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotar : MonoBehaviour
{
    [Tooltip("Velocidad de rotacion del objeto")]
    [SerializeField] float velocidad = 150.0f;

    [Tooltip("Distancia del objeto flotando arriba y abajo")]
    [SerializeField] float distancia = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * velocidad * Time.deltaTime);

        // Mover arriba y abajo
        if (gameObject.tag == "Moneda")
        {
            Vector3 aux = transform.position;
            float newY = Mathf.Sin(Time.time) * distancia * 0.001f + aux.y;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z) ;
        }
    }
}
