using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EscenaJuego()
    {

        
        SceneManager.LoadScene("juego");
        
    }

    public void EscenaControles()
    {
        
        SceneManager.LoadScene("controles");
    }

    public void EscenaMenu()
    {
        
        SceneManager.LoadScene("menu");
    }

    public void EscenaCreditos()
    {
        
        SceneManager.LoadScene("creditos");
    }

    public void Salir()
    {

        
        UnityEditor.EditorApplication.isPlaying = false; //Para cerrar en la version del editor (osea sin exportar nada osea el juego)
        //Application.Quit(); // Para cerrar en la versión compilada (cuendo ya se exportar el juego)

    }

}

