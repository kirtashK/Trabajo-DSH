using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuVictoria : MonoBehaviour
{
    [SerializeField] Button btnMenu;



    private void Start()
    {
        btnMenu.onClick.AddListener(IrAMenu);

    }


    private void IrAMenu()
    {
        Debug.Log("cambio de escena");
        SceneManager.LoadScene(0);
    }
}
