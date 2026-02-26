using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;

    // Cambiamos el string por un int (entero) para usar el número de la escena
    public int mainMenuSceneIndex = 0;

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
            if (isPaused)
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

    // --- MÉTODO 1: Usa la variable del Inspector ---
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneIndex); // Carga la escena por su número
    }

    // --- MÉTODO 2: Permite poner el número directamente en el Botón UI ---
    public void LoadSceneByNumber(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}