using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new script zum camera bewegen
public class SimpelCameraMovement : MonoBehaviour
{
    public Camera cam, incam;
    Transform pos;
    public float minZoom,maxZoom;
    
    public float moveSpeed, scrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        move();
        Zoom();
        PanCameraWithMiddleMouse();
    }

   

    void move()
    {
        Vector3 move = cam.transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            move.z += moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move.z -= moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            move.x += moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move.x -= moveSpeed * Time.deltaTime;
        }
        cam.transform.position = new Vector3(move.x, 45, move.z);
    }

    void Zoom()
    {
        float b = Input.GetAxis("Mouse ScrollWheel");

        if (b > 0)
        {

            if (cam.orthographicSize > minZoom)
            {
                cam.orthographicSize -= Mathf.Abs(b) * scrollSpeed * Time.deltaTime;
                incam.orthographicSize -= Mathf.Abs(b) * scrollSpeed * Time.deltaTime;
            }
            else
            {
                cam.orthographicSize = minZoom;
                incam.orthographicSize = minZoom;
            }
        }
        else if (b < 0)
        {
            if (cam.orthographicSize < maxZoom)
            {
                cam.orthographicSize += Mathf.Abs(b) * scrollSpeed * Time.deltaTime;
                incam.orthographicSize += Mathf.Abs(b) * scrollSpeed * Time.deltaTime;
            }
            else
            {
                cam.orthographicSize = maxZoom;
                incam.orthographicSize = maxZoom;
            }
        }
    }

    //new 
    private Vector3 initialMousePosition;
    private Vector3 initialCameraPosition;
    private bool isPanning = false;
    public float panSpeed = 5f;
    public float smoothTime = 0.2f;
    void PanCameraWithMiddleMouse()
    {
        if (Input.GetMouseButtonDown(2)) // Wenn die mittlere Maustaste gedrückt wird
        {
            initialMousePosition = Input.mousePosition;
            initialCameraPosition = cam.transform.position;
            isPanning = true;
        }
        else if (Input.GetMouseButtonUp(2)) // Wenn die mittlere Maustaste losgelassen wird
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 mouseDelta = Input.mousePosition - initialMousePosition;
            Vector3 panOffset = new Vector3(mouseDelta.x * panSpeed, 0, mouseDelta.y * panSpeed);

            Vector3 targetCameraPosition = initialCameraPosition - panOffset;

            // Verwenden Sie eine benutzerdefinierte Interpolation
            cam.transform.position = Vector3.Lerp(cam.transform.position,
                                                 new Vector3(targetCameraPosition.x, 45, targetCameraPosition.z),
                                                 smoothTime);
        }
    }

}
