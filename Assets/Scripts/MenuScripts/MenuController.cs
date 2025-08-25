using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Canvas MenuUI;
    public Canvas PreNightUI;
    public GameObject MenuSprites;
    public TextMeshProUGUI StartButtonText;
    public TextMeshProUGUI VersionText;
    public TextMeshProUGUI PreNightText;
    public AudioSource GarbleSource;
    public AudioSource MenuMusicSource;

    public SaveData currentSave;
    public AIValuesObject[] AIValues;

    public float fadeDuration = 3f;

    public void StartGame()
    {
        MenuSprites.SetActive(false);
        MenuUI.gameObject.SetActive(false);
        MenuMusicSource.Pause();
        PreNightUI.gameObject.SetActive(true);
        GarbleSource.Play();
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        AsyncOperation SceneLoader = SceneManager.LoadSceneAsync("MainGame");
        SceneLoader.allowSceneActivation = false;
        yield return new WaitForSeconds(2f);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            PreNightText.color = new Color(1, 1, 1, alpha);
            yield return null;
        }
        while (SceneLoader.progress < 0.9f)
        {
            yield return null;
        }
        SceneLoader.allowSceneActivation = true;
    }

    void Start()
    {
        currentSave = SaveManager.Instance.LoadGame();
        int night = currentSave.CurrentNight;
        GameSceneArgs.Night = night;
        AIValuesObject currentAI = AIValues[night - 1];
        GameSceneArgs.AIValues = currentAI;
        PreNightUI.gameObject.SetActive(false);
        VersionText.text = Application.version;
        PreNightText.text = $"12:00 AM\nNight {currentSave.CurrentNight}";
        StartButtonText.text = $"Start Game\nNight {currentSave.CurrentNight}";
    }

    public void StartCustomNight(AIValuesObject AIValues)
    {
        GameSceneArgs.AIValues = AIValues;
        GameSceneArgs.Night = 7;
        PreNightText.text = $"12:00 AM\nNight {GameSceneArgs.Night}";
        StartGame();
    }
}
