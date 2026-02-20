using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Primero: Comprobamos si el INVENTARIO estaba abierto
            if (InventoryManager.Instance != null && InventoryManager.Instance.EstaInventarioAbierto())
            {
                // Si estaba abierto, SOLO cerramos el inventario y no hacemos nada más.
                InventoryManager.Instance.CerrarInventario();
                return; // Cortamos la función aquí para que no se abra la pausa
            }

            // Segundo: Si el inventario no estaba abierto, hacemos la lógica normal de la Pausa
            if (isPaused) // (pon aquí tu variable booleana de pausa)
            {
                Resume(); // Llama a tu función de quitar la pausa
            }
            else
            {
                Pause();   // Llama a tu función de poner la pausa
            }
        }
    }

    public void Pause()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
