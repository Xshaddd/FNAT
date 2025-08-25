using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 prevMousePosition;

    float deltaY;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Hovered over {gameObject.name}");
        if (gameObject.name == "LeftHover")
        {
            OfficeCameraController.curPosition = -1;

        }
        else if (gameObject.name == "RightHover")
        {
            OfficeCameraController.curPosition = 1;
        }
        else if (gameObject.name == "CameraHover")
        {
            if (deltaY < 0) return;
            GameSceneController.Instance.ToggleMode();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (gameObject.name == "LeftHover" || gameObject.name == "RightHover")
            OfficeCameraController.curPosition = 0;
    }

    void Update()
    {
        deltaY = Mouse.current.position.ReadValue().y - prevMousePosition.y;
    }
}