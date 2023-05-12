using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pulsar : MonoBehaviour
{
    [SerializeField] Button btn;

    private void Start()
    {
        btn.onClick.AddListener(IrANivel1);
    }

    private void IrANivel1()
    {
        Debug.Log("cambio de escena");
       // SceneManager.LoadScene("Nivel1");
        SceneManager.LoadScene(1);
    }
}