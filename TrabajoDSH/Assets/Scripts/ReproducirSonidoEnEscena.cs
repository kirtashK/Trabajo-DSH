using UnityEngine;
using UnityEngine.SceneManagement;

public class ReproducirSonidoEnEscena : MonoBehaviour
{
    public AudioClip sonido; // El AudioClip que deseas reproducir

    private void Start()
    {
        // Verificar la escena actual
        Scene escenaActual = SceneManager.GetActiveScene();
        if (escenaActual.name == "Menu") // Reemplaza "NombreDeTuEscena" con el nombre real de la escena
        {
            // Reproducir el sonido
            AudioSource.PlayClipAtPoint(sonido, transform.position);
        }
    }
}