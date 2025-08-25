using UnityEngine;
using UnityEngine.InputSystem;

public class RayCastHandler : MonoBehaviour
{
    public bool isLMBHeldDown;

    // Update is called once per frame
    void Update()
    {

        if (Mouse.current.leftButton.IsPressed())
        {
            if (isLMBHeldDown)
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);

            foreach (RaycastHit2D hit in hits)
            {
                HandleClickHit(hit.collider.gameObject.name);
            }

            isLMBHeldDown = true;
        }
        else
            isLMBHeldDown = false;
    }

    void HandleClickHit(string name)
    {
        if (name == "DoorButton")
        {
            GameSceneController.Instance.ToggleDoor();
        }
        if (name == "TrapDoorButton")
        {
            GameSceneController.Instance.ToggleTrapDoor();
        }
    }
}
