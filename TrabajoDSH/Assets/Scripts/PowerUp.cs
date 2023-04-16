using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("El jugador me ha tocado");

            // Codigo del powerup, estilo añadir una vida, añadir una moneda, etc, hace falta UI
            // ...

            Destroy(gameObject);
        }
    }

}
