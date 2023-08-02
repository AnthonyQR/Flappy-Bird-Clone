using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button StartButton;
    public Button QuitButton;
    public GameObject FadeToBlackPanel;
    public float fadeSpeed;
    public float sceneLoadDelay;

    public AudioSource soundPlayer;
    public AudioClip clickSound;
    // Start is called before the first frame update
    void Start()
    {
        soundPlayer = this.GetComponent<AudioSource>();
        FadeToBlackPanel.SetActive(false);
        StartButton.onClick.AddListener(StartGame);
        QuitButton.onClick.AddListener(QuitGame);
    }

    void StartGame()
    {
        soundPlayer.PlayOneShot(clickSound);
        FadeToBlackPanel.SetActive(true);
        StartCoroutine(FadeToBlack());
    }

    public IEnumerator FadeToBlack()
    {
        float fadeAmount = 0;
        Color FadeToBlackColor = FadeToBlackPanel.GetComponent<Image>().color;
        while (FadeToBlackPanel.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = FadeToBlackColor.a + (fadeSpeed * Time.deltaTime);

            FadeToBlackColor = new Color(FadeToBlackColor.r, FadeToBlackColor.b, FadeToBlackColor.g, fadeAmount);
            FadeToBlackPanel.GetComponent<Image>().color = FadeToBlackColor;
            yield return null;
        }

        while (sceneLoadDelay > 0)
        {
            sceneLoadDelay -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene("GameScene");
    }
    void QuitGame()
    {
        soundPlayer.PlayOneShot(clickSound);
        FadeToBlackPanel.SetActive(true);
        StartCoroutine(FadeToQuit());
    }

    public IEnumerator FadeToQuit()
    {
        float fadeAmount = 0;
        Color FadeToBlackColor = FadeToBlackPanel.GetComponent<Image>().color;
        while (FadeToBlackPanel.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = FadeToBlackColor.a + (fadeSpeed * Time.deltaTime);

            FadeToBlackColor = new Color(FadeToBlackColor.r, FadeToBlackColor.b, FadeToBlackColor.g, fadeAmount);
            FadeToBlackPanel.GetComponent<Image>().color = FadeToBlackColor;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}

