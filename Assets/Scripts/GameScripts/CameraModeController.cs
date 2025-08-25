using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor;

public class CameraModeController : MonoBehaviour
{
    public static CameraModeController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Image cameraDisplay;
    public Sprite[] cameraSprites;
    public MapNode[] mapNodes; // must have correct order in inspector
    public TextMeshProUGUI cameraName;
    public TextMeshProUGUI presenceDisplay;
    public TextMeshProUGUI SignalInterrupted;
    public RawImage staticImage;
    public AudioSource SwitchCameraSource;
    public AudioSource BreakCameraSource;
    public AudioClip[] CameraBreakClips;
    public GameObject CharacterLayer;

    public RectTransform BackgroundRect;
    public RectTransform WorldRect;
    public float panSpeed;
    private float minPan;
    private float maxPan;
    private int direction = 1;

    public float fadeDuration = 0.5f;

    public int currentCameraIndex;
    private bool isInterrupted;

    void Start()
    {
        maxPan = (BackgroundRect.rect.width - WorldRect.rect.width) / 2;
        minPan = -maxPan;
        SignalInterrupted.gameObject.SetActive(false);
        presenceDisplay.gameObject.SetActive(false);
        SwitchCamera(0);
        StartCoroutine(PanLoop());
    }

    public void SwitchCamera(int index)
    {
        if (!isInterrupted)
            StartCoroutine(StaticFadeOut());
        SwitchCameraSource.Play();
        if (index >= 0 && index < cameraSprites.Length)
        {
            cameraDisplay.sprite = cameraSprites[index];
            currentCameraIndex = index;
            cameraName.text = mapNodes[index].ID;
            UpdatePresenceDisplay(index);
            RenderCharacters(index);
            Debug.Log($"[CameraModeController] Switched to Camera: {index} ({mapNodes[index].ID})");
        }
        else
        {
            Debug.LogWarning("[CameraModeController] Incorrect camera selected!");
        }
    }

    IEnumerator StaticFadeOut()
    {
        Material mat = staticImage.material;
        Color color = mat.color;
        color.a = 1f;
        mat.color = color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (isInterrupted)
                break;
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0.2f, elapsed / fadeDuration);
            color.a = alpha;
            mat.color = color;
            yield return null;
        }
        yield return null;
    }

    public void BreakCamera()
    {
        StartCoroutine(HandleCameraBreak());
    }

    IEnumerator HandleCameraBreak()
    {
        isInterrupted = true;
        SignalInterrupted.gameObject.SetActive(true);
        Material mat = staticImage.material;
        Color color = mat.color;
        color.a = 1f;
        mat.color = color;
        BreakCameraSource.clip = CameraBreakClips[UnityEngine.Random.Range(0, CameraBreakClips.Length)];
        BreakCameraSource.Play();
        yield return new WaitForSeconds(3f);
        BreakCameraSource.Stop();
        SignalInterrupted.gameObject.SetActive(false);
        color.a = 0.2f;
        mat.color = color;
        isInterrupted = false;
    }

    public void UpdatePresenceDisplay(int index)
    {
        presenceDisplay.text = "";
        for (int i = 0; i < IQController.Instance.nightIQ.Length; i++)
        {
            if (mapNodes[index] == IQController.Instance.nightIQ[i].CurrentNode)
            {
                if (IQController.Instance.nightIQ[i] is MazikIQ mazik)
                {
                    presenceDisplay.text += $"{mazik.CharacterData.Name} {mazik.phase}";
                }
                else
                {
                    presenceDisplay.text += $"{IQController.Instance.nightIQ[i].CharacterData.Name} ";
                }
            }
        }
    }
    public void RenderCharacters(int index)
    {
        foreach (Transform child in CharacterLayer.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < IQController.Instance.nightIQ.Length; i++)
        {
            CharacterIQ iq = IQController.Instance.nightIQ[i];
            if (mapNodes[index] == iq.CurrentNode)
            {
                GameObject CharacterRenderPrefab = Instantiate(iq.CharacterData.RenderImagePrefab, CharacterLayer.transform);
                RawImage image = CharacterRenderPrefab.GetComponent<RawImage>();
                image.enabled = false;

                RenderDataObject rd;

                if (iq is MazikIQ mazik)
                { 
                    RenderDataObject[] phaseRenders = iq.CharacterData.RenderData.Where(rd => rd.node == iq.CurrentNode).ToArray();
                    if (phaseRenders.Length <= mazik.phase)
                    {
                        continue;
                    }
                    rd = phaseRenders[mazik.phase];
                }
                else
                {   
                    rd = iq.CharacterData.RenderData.FirstOrDefault(rd => rd.node == iq.CurrentNode);
                }

                if (rd?.node == null)
                {
                    Debug.Log($"[CameraModeController] {iq.CharacterData.Name} does not have RenderData node at {mapNodes[index]} but present on camera");
                    continue;
                }
                if (rd?.sprite == null)
                {
                    Debug.Log($"[CameraModeController] {iq.CharacterData.Name} does not have sprite at {mapNodes[index]}");
                    continue;
                }

                image.texture = rd.sprite.texture;
                image.enabled = true;
            }
        }
    }
    IEnumerator PanLoop()
    {
        while (true)
        {
            Vector2 anchoredPos = WorldRect.anchoredPosition;
            anchoredPos.x += panSpeed * Time.deltaTime * direction;
            anchoredPos.x = Mathf.Clamp(anchoredPos.x, minPan, maxPan);
            WorldRect.anchoredPosition = anchoredPos;

            if (anchoredPos.x >= maxPan || anchoredPos.x <= minPan)
            { 
                direction *= -1;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}
