using UnityEngine;
using System.Linq;
using System.Collections;

public class IQController : MonoBehaviour
{
    
#if UNITY_EDITOR
    public bool isDebug = true;
#endif
    public static IQController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public CharacterIQ[] nightIQ;
    private float IQTimer;

    public float IQTimerCap;
    public AIValuesObject[] AIValues;

    public MapNode Sofa; // 1A
    public MapNode TraphouseA; 
    public MapNode TraphouseB; // 1B
    public MapNode TraphouseC; // 1C
    public MapNode Kitchen; // 2A
    public MapNode Storage; // 2B
    public MapNode Balcony1; // 3
    public MapNode Hall; // 4
    public MapNode Balcony2; // 5
    public MapNode DoorNode;
    public MapNode VentNode;
    public MapNode Office;

    public MapNode[] footStepsNodesIndexes;

    public CharacterDataObject AckeData;
    public CharacterDataObject JokerData;
    public CharacterDataObject IgorData;
    public CharacterDataObject MazikData;

    public AudioSource FootStepsSource;
    public AudioClip MazikFootStepsClip;

    void Progress()
    {
        for (int i = 0; i < nightIQ.Length; i++)
        {
            int IQ = nightIQ[i].IQ;
            if (IQ == 0) continue; // skip if IQ is 0 (impossible to move anyway)

            // instant fallback to prevent door camping (perhaps can be changed to enable some door camping on higher IQ)
            if (nightIQ[i].CurrentNode == DoorNode && GameSceneController.Instance.doorState == GameSceneController.DoorState.Closed)
            {
                nightIQ[i].FallBack("door");
                continue;
            }
            else if (nightIQ[i].CurrentNode == VentNode && GameSceneController.Instance.trapDoorState == GameSceneController.DoorState.Closed)
            {
                nightIQ[i].FallBack("vent");
                continue;
            }

            int roll = Random.Range(0, 20);
            Debug.Log($"[IQController] {nightIQ[i].CharacterData.Name} rolled {roll} (req < {IQ})");

            if (roll < IQ)
            {
                nightIQ[i].MoveForward();
            }
        }
    }

    public IEnumerator PlayRunAnimation(MazikIQ mazikIQ)
    {
        yield return new WaitForSeconds(15.0f);

        if (GameSceneController.Instance.trapDoorState == GameSceneController.DoorState.Closed)
        {
            mazikIQ.FallBack("vent");
            PowerController.Instance.CurrentSanity -= 3f;
            mazikIQ.phase = 0;
        }
        else if (GameSceneController.Instance.trapDoorState == GameSceneController.DoorState.Open)
        {
            Vector3 oldPos = OfficeCameraController.Instance.officeCamera.transform.position;
            OfficeCameraController.Instance.officeCamera.transform.position = new Vector3(0, oldPos.y, oldPos.z);
            GameSceneController.Instance.SetOfficeMode();
            GameSceneController.Instance.StopGame(mazikIQ.CharacterData.Jumpscare, mazikIQ.CharacterData.JumpscareAlpha);
        }
            
        Debug.Log("[IQController] Mazik run animation complete");
    }

    void Start()
    {

        int night = GameSceneController.Instance.currentNight;
#if UNITY_EDITOR
        if (night == 666 && isDebug)
            night = 6;
        else if (night == 0 && isDebug)
            night = 7;
#endif
        AIValuesObject currentAI;
        if (GameSceneArgs.AIValues == null)
        {
            currentAI = AIValues[night - 1];
        }
        else
        {
            currentAI = GameSceneArgs.AIValues;
        }

        nightIQ = new CharacterIQ[] { new CharacterIQ(AckeData, currentAI.AckeAggression),
                                      new CharacterIQ(JokerData, currentAI.JokerAggression),
                                      new CharacterIQ(IgorData, currentAI.IgorAggression),
                                      new MazikIQ(MazikData, currentAI.MazikAggression) };
        IQTimer = 0;
        Debug.Log($"[IQController] Loaded Night {night} IQ, AI values: " + string.Join(" ", nightIQ.Select(n => n.IQ).ToList()));
    }

    void Update()
    {
        IQTimer += Time.deltaTime;
        if ( IQTimer > IQTimerCap && !GameSceneController.Instance.isGameStopped)
        {
            IQTimer -= IQTimerCap;
            Progress();
        }
    }
}
