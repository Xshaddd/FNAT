using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CallController : MonoBehaviour
{
    public AudioSource CallSource;
    public AudioClip[] CallClips;

    public Button CallSkipButton;

    private bool isCallAssigned;

    void Start()
    {
        CallSkipButton.gameObject.SetActive(false);
        int night = GameSceneController.Instance.currentNight;
        if (CallClips.Length < night || night == 0)
        {
            Debug.Log($"[CallController] Current night ({night}) does not have a call clip. CallClip not assigned.");
            isCallAssigned = false;
        }
        else if (night <= 5)
        {
            CallSource.clip = CallClips[night - 1];
            isCallAssigned = true;
        }        
        if (isCallAssigned)
            StartCoroutine(HandleCall());
    }

    IEnumerator HandleCall()
    {
        yield return new WaitForSeconds(10f);
        CallSkipButton.gameObject.SetActive(true);
        CallSource.Play();
        yield return new WaitForSeconds(CallSource.clip.length);
        CallSkipButton.gameObject.SetActive(false);

    }

    public void StopCall()
    {
        CallSkipButton.gameObject.SetActive(false);
        CallSource.Stop();
    }

    void Update()
    {
        
    }
}
