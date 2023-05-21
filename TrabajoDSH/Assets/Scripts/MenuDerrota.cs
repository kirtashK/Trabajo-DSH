using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuDerrota : MonoBehaviour
{
    [SerializeField] Button btnMenu;
    [SerializeField] Button btnNivel1;


    private void Start()
    {

        btnNivel1.onClick.AddListener(IrANivel1);
        btnMenu.onClick.AddListener(IrAMenu);

    }

    private void IrANivel1()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(1);
    }

    private void IrAMenu()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(0);
    }
}
