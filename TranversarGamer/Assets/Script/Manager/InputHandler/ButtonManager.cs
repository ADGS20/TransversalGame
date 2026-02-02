using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject SceneOption;
    public GameObject SceneStart;

    public void StartBut()
    {
        SceneManager.LoadScene("BEta Escenario");
    }
    public void Optionbut()
    {
        SceneOption.SetActive(true);
        SceneStart.SetActive(false);
    }
    public void Quitbut()
    {
        Application.Quit();
    }
    public void Backbut()
    {
        SceneOption.SetActive(false);
        SceneStart.SetActive(true );
    }
}
