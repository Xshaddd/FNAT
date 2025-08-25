using TMPro;
using UnityEngine;

public class PowerController : MonoBehaviour
{
    public TextMeshProUGUI PowerMeter;
    public AudioSource InsanitySource;

    public Texture2D LoseToInsanityTexture;
    public AudioClip LoseToInsanityClip;

    public float DrainRate = 0.06f;
    public float CurrentSanity = 101f;
    public float BaseDrainRateMultiplier = 1f;
    public float InsanityThreshold = 10f;

    private bool isInsanityThresholdReached = false;

    public static PowerController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (CurrentSanity > 0)
        {
            CurrentSanity -= DrainRate * ( BaseDrainRateMultiplier 
                                        + GameSceneController.Instance.doorDrain
                                        + GameSceneController.Instance.trapDoorDrain) 
                                        * Time.deltaTime;

            PowerMeter.text = $"SANITY: {(int)CurrentSanity}%";

            if (CurrentSanity < InsanityThreshold && !isInsanityThresholdReached)
                StartInsanitySequence();
            if (isInsanityThresholdReached)
            {
                InsanitySource.volume = Mathf.Lerp(0f, 0.9f, 1f - (CurrentSanity / InsanityThreshold));
            }
        }
        else if (!GameSceneController.Instance.isGameStopped)
        {
            InsanitySource.Stop();
            GameSceneController.Instance.LoseToInsanity();
        }
    }

    void StartInsanitySequence()
    {
        isInsanityThresholdReached = true;
        InsanitySource.Play();
    }
}
