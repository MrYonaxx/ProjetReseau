using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera cam = null;
    public Camera Cam
    {
        get { return cam; }
    }


    [Space]
    [SerializeField]
    float sensitivity = 5;
    [SerializeField]
    Vector2 clampY = new Vector3(-10, 20);
    [SerializeField]
    Vector2 clampZoom = new Vector3(1, 1.5f);

    [Space]
    [SerializeField]
    Curve curve;



    float rotX = 0;
    float rotY = 0;
    float zoomZ = 1;
    Transform focus = null;

    // Faut essayer d'utiliser le moins possible les singleton, ou du moins avec parcimonie
    public static CameraController Instance = null;

    // Start is called before the first frame update
    void Start()
    {
        zoomZ = transform.localScale.z;

        if (Instance == null)
            Instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (focus == null)
            return;
        // Set Focus
        transform.position = focus.transform.position;

        // Set zoom
        zoomZ -= 0.5f * Input.GetAxis("Mouse ScrollWheel");
        zoomZ = Mathf.Clamp(zoomZ, clampZoom.x, clampZoom.y);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, zoomZ);

        if (Input.GetMouseButton(1))
        {
            // Set camera rotation
            rotX += (sensitivity * Input.GetAxis("Mouse X"));
            rotY -= Input.GetAxis("Mouse Y");

            rotY = Mathf.Clamp(rotY, clampY.x, clampY.y);
            transform.eulerAngles = new Vector3(0, rotX, 0);

            // Set camera position
            float rotationYRatio = rotY;
            rotationYRatio += -clampY.x;
            cam.transform.localPosition = curve.GetPosition(rotationYRatio / (clampY.y - clampY.x));
            cam.transform.localEulerAngles = new Vector3(rotY, 0, 0);
        }
    }


    private void OnDrawGizmos()
    {
        curve.DrawGizmo(Color.yellow, transform.localToWorldMatrix);
    }

    public void SetFocus(Transform t)
    {
        focus = t;
    }
}
