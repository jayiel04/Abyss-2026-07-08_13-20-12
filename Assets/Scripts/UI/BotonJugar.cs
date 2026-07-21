using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonJugar : MonoBehaviour
{
    public void IrANivel1()
    {
        SceneManager.LoadScene("TransicionCapa7");
    }

    public void RegresarMenu()
        {
        SceneManager.LoadScene("Menu");
    }
}