using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameSceneController : MonoBehaviour
{
    public static GameSceneController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public GameObject officePanel;
    public GameObject cameraPanel;
    public GameObject gameStopPanel;

    public GameObject OfficeButtons;
    public GameObject officeBackground;
    public GameObject DoorSprite;
    public GameObject TrapDoorSprite;
    public GameObject DoesHeKnow;

    public RawImage StopImage;
    public Texture2D WinSprite;

    public AudioSource GarbleSource;
    public AudioSource FailSource;
    public VideoPlayer JumpscarePlayer;
    public VideoPlayer JumpscareAlphaPlayer;
    public RawImage JumpscareRawImage;
    public AudioSource OverSource;
    public AudioSource DoorSource;
    public AudioSource DoorBlockedSource;
    public AudioClip DoorOpen;
    public AudioClip DoorClose;

    public TextMeshProUGUI GameStopText;
    public TextMeshProUGUI TimeDisplay;

    public int hourLength;

    public enum GameStopType { Lost = 1, Won = 2 }

    public enum Mode { Office, Camera }
    public Mode currentMode;

    public enum DoorState { Open, Closed }
    public DoorState doorState;
    public DoorState trapDoorState;
    public float doorDrain = 1f;
    public float trapDoorDrain = 1f;

    public int currentNight;
    public float currentTime = 0; //Time in seconds. 12am to 5am with 2 minutes per hour
    public int currentHour;

    public SaveData currentSave { get; private set; }
    public bool isGameStopped = false;

    public bool isGameLost = false;
    public bool isDoorBlocked = false;
    public bool isTrapDoorBlocked = false;

    public void SetCameraMode()
    {
        Instance.currentMode = Mode.Camera;
        Instance.cameraPanel.SetActive(true);
        Instance.officePanel.SetActive(false);
        Instance.OfficeButtons.SetActive(false);
        Instance.officeBackground.SetActive(false);
        CameraModeController.Instance.UpdatePresenceDisplay(CameraModeController.Instance.currentCameraIndex);
        if (isGameLost)
            OverSource.Play();
    }

    public void SetOfficeMode()
    {
        Instance.currentMode = Mode.Office;
        Instance.cameraPanel.SetActive(false);
        Instance.officePanel.SetActive(true);
        Instance.OfficeButtons.SetActive(true);
        Instance.officeBackground.SetActive(true);
        if (isGameLost)
            StopGame(JumpscarePlayer.clip, JumpscareAlphaPlayer.clip);
    }
    public void ToggleMode()
    {
        if (currentMode == Mode.Office)
            SetCameraMode();
        else if (currentMode == Mode.Camera)
            SetOfficeMode();
        GarbleSource.Play();
    }

    public void ProgressTime(float time)
    {
        currentTime += time;
        if (currentTime < hourLength)
        {
            currentHour = 12;
        }
        else
        {
            currentHour = (int)(currentTime / hourLength);
        }
        TimeDisplay.text = $"{currentHour} AM\n Night {currentNight}";
        if (currentHour == 6)
        {
            WinGame();
        }
    }

    public void OpenDoor(bool withSound)
    {
        if (withSound)
        {
            DoorSource.clip = DoorOpen;
            DoorSource.Play();
        }
        doorState = DoorState.Open;
        doorDrain = 0f;
        DoorSprite.SetActive(false);
    }

    public void CloseDoor(bool withSound)
    {
        if (isDoorBlocked)
        {
            DoorBlockedSource.Play();
            return;
        }
        if (withSound)
        {
            DoorSource.clip = DoorClose;
            DoorSource.Play();
        }
        doorState = DoorState.Closed;
        doorDrain = 1.5f;
        DoorSprite.SetActive(true);
    }
    public void ToggleDoor()
    {
        if (doorState == DoorState.Open)
            CloseDoor(true);
        else if (doorState == DoorState.Closed)
            OpenDoor(true);
    }

    public void OpenTrapDoor(bool withSound)
    {
        if (withSound)
        {
            DoorSource.clip = DoorOpen;
            DoorSource.Play();
        }
        trapDoorState = DoorState.Open;
        trapDoorDrain = 0f;
        TrapDoorSprite.SetActive(false);
    }

    public void CloseTrapDoor(bool withSound)
    {
        if (isTrapDoorBlocked)
        {
            DoorBlockedSource.Play();
            return;
        }
        if (withSound)
        {
            DoorSource.clip = DoorClose;
            DoorSource.Play();
        }
        trapDoorState = DoorState.Closed;
        trapDoorDrain = 2f;
        TrapDoorSprite.SetActive(true);
    }
    public void ToggleTrapDoor()
    {
        if (trapDoorState == DoorState.Open)
            CloseTrapDoor(true);
        else if (trapDoorState == DoorState.Closed)
            OpenTrapDoor(true);
    }

    void Start()
    {
        SetOfficeMode();
        OpenDoor(false);
        OpenTrapDoor(false);
        gameStopPanel.SetActive(false);
        DoesHeKnow.SetActive(false);
        currentSave = SaveManager.Instance.LoadGame();
#if UNITY_EDITOR
        if (!IQController.Instance.isDebug)
            currentNight = currentSave.CurrentNight;
#else
        currentNight = currentSave.CurrentNight;
#endif
        if (GameSceneArgs.Night != -1)
        {
            currentNight = GameSceneArgs.Night;
        }
        else
        {
            Debug.Log("[GameSceneController] Loaded night from savefile instead of GameSceneArgs");
        }
    }

    void Update()
    {
        if(!isGameStopped)
            ProgressTime(Time.deltaTime);
    }

    public void ItIsSoOver(VideoClip jumpscare, VideoClip alpha)
    {
        isGameLost = true;
        if (currentMode == Mode.Camera)
            OverSource.Play();
        if (jumpscare != null)
        {
            JumpscarePlayer.clip = jumpscare;
            JumpscareAlphaPlayer.clip = alpha;
        }
        DoesHeKnow.SetActive(true);
    }

    public void StopGame(VideoClip jumpscare, VideoClip alpha)
    {
        StartCoroutine(HandleStop(jumpscare, alpha));
    }

    public void WinGame()
    {
        StartCoroutine(WinGameSequence());
    }
    public void LoseToInsanity() 
    {
        StartCoroutine(LoseToInsanitySequence());
    }

    private IEnumerator WinGameSequence()
    {
        isGameStopped = true;

        OfficeButtons.GetComponent<Canvas>().sortingOrder = 0;
        gameStopPanel.SetActive(true);

        if (OverSource.isPlaying)
            OverSource.Stop();

        StopImage.texture = WinSprite;

        if (currentNight < 5 && currentNight != 0)
        {
            Instance.currentSave.CurrentNight += 1;
            SaveManager.Instance.SaveGame(currentSave);
        }
        GameStopText.text = "You stayed alive...";

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Menu");
    }
    private IEnumerator LoseToInsanitySequence()
    {
        isGameStopped = true;

        OfficeButtons.GetComponent<Canvas>().sortingOrder = 0;
        gameStopPanel.SetActive(true);
        JumpscareRawImage.gameObject.SetActive(false);
        if (OverSource.isPlaying)
            OverSource.Stop();

        StopImage.texture = PowerController.Instance.LoseToInsanityTexture;

        FailSource.clip = PowerController.Instance.LoseToInsanityClip;
        FailSource.Play();

        yield return new WaitForSeconds(FailSource.clip.length);

        SceneManager.LoadScene("Menu");
    }

    private IEnumerator HandleStop(VideoClip jumpscare = null, VideoClip alpha = null)
    {
        isGameStopped = true;

        StopImage.gameObject.SetActive(false);
        OfficeButtons.GetComponent<Canvas>().sortingOrder = 0;
        gameStopPanel.SetActive(true);

        if (OverSource.isPlaying)
            OverSource.Stop();

        if (jumpscare != null) JumpscarePlayer.clip = jumpscare;
        if (alpha != null) JumpscareAlphaPlayer.clip = alpha;
        yield return null;
        GameStopText.text = "";
        JumpscarePlayer.Play();
        JumpscareAlphaPlayer.Play();
        yield return new WaitForSeconds((float)JumpscarePlayer.clip.length);

        SceneManager.LoadScene("Menu");
    }
}
