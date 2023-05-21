using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ComenzarJuego : MonoBehaviour
{
    [SerializeField] Button btnComenzarJuego;
    [SerializeField] Button btnNivel1;
    [SerializeField] Button btnNivel2;
    [SerializeField] Button btnNivel3;
    [SerializeField] Button btnNivel4;


    private void Start()
    {
        btnComenzarJuego.onClick.AddListener(IrANivel1);
        btnNivel1.onClick.AddListener(IrANivel1);
        btnNivel2.onClick.AddListener(IrANivel2);
        btnNivel3.onClick.AddListener(IrANivel3);
        btnNivel4.onClick.AddListener(IrANivel4);
    }

    private void IrANivel1()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(1);
    }

    private void IrANivel2()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(2);
    }

    private void IrANivel3()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(3);
    }

    private void IrANivel4()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(4);
    }
}