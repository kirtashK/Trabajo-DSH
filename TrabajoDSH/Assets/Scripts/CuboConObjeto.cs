using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuboConObjeto : MonoBehaviour
{
    #region Variables

    [Tooltip("Prefab a crear cuando el jugador colisione con el trigger de este objeto")]
    [SerializeField] GameObject prefab;

    [Tooltip("Punto donde se generará el prefab")]
    [SerializeField] GameObject puntoSpawn;

    #endregion


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el jugador toca el trigger:
        if (other.gameObject.tag == "Player")
        {
            // Crear el powerUp y desactivar el trigger:
            Instantiate(prefab, puntoSpawn.transform.position, puntoSpawn.transform.rotation);
            gameObject.SetActive(false);
        }
    }
}
