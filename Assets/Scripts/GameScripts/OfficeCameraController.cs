using Unity.VisualScripting;
using UnityEngine;

public class OfficeCameraController : MonoBehaviour
{
    public static OfficeCameraController Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject officeCamera;
    public static int curPosition = 0; // -1 left 0 none 1 right
    public float maxPos;
    public float minPos;

    void Update()
    {
        MoveCamera();
    }

    void MoveCamera()
    {
        Vector3 newPos = officeCamera.transform.position + new Vector3(10f * Time.deltaTime * curPosition, 0, 0);
        newPos.x = Mathf.Clamp(newPos.x, minPos, maxPos);
        officeCamera.transform.position = newPos;
    }
}
