using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button StartButton;
    public Button QuitButton;
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
    }
    void QuitGame()
    {
        Application.Quit();
    }
}

