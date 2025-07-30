using UnityEngine;
using UnityEngine.SceneManagement;

public class Audio : MonoBehaviour
{
    private static Audio instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
        if (SceneManager.GetActiveScene().name == "juego")
        {
            Destroy(gameObject);
        }
    }
}

