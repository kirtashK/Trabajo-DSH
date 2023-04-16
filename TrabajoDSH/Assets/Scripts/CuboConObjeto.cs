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
            Instantiate(prefab, puntoSpawn.transform.position, Quaternion.identity);
            Debug.Log("El jugador ha tocado el trigger.");
            gameObject.SetActive(false);
        }
    }
}
