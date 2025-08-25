using UnityEngine;

public class MazikIQ : CharacterIQ
{
    public int phase;

    public MazikIQ(CharacterDataObject characterData, int iq)
    : base(characterData, iq)
    {
        phase = 0;
    }

    public override void MoveForward()
    {
        if (GameSceneController.Instance.currentMode != GameSceneController.Mode.Camera && Random.Range(0, 20) != 1)
            return;
        phase++;
        CameraModeController.Instance.RenderCharacters(CameraModeController.Instance.currentCameraIndex);
        Debug.Log($"[MazikIQ] Mazik progressed to phase {phase}");
        if (phase == 4)
        {
            StartRunAnimation();
        }
    }

    public void StartRunAnimation()
    {
        IQController.Instance.StartCoroutine(IQController.Instance.PlayRunAnimation(this));
    }
}
