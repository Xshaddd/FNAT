using UnityEngine;
using System.Collections;

public class AmbienceController : MonoBehaviour
{
    public AudioSource ambienceSource;
    public AudioClip ambienceClip;

    public float minInterval = 35f;
    public float maxInterval = 60f;

    private void Start()
    {
        StartCoroutine(PlayAmbienceLoop());
    }

    IEnumerator PlayAmbienceLoop()
    {
        while (!GameSceneController.Instance.isGameStopped)
        {
            // Wait random time before playing again
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            // Play sound and wait for it to finish
            ambienceSource.clip = ambienceClip;
            ambienceSource.Play();

            yield return new WaitForSeconds(ambienceClip.length);
        }
    }
}
