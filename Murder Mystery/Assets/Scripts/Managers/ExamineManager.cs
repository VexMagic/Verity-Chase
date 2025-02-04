using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineManager : MonoBehaviour
{
    public static ExamineManager instance;

    public Material normal, unExamined, examined;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject characterCanvas;
    [SerializeField] private float speed;
    [SerializeField] private float verticalEdge;
    [SerializeField] private float horizontalEdge;
    [SerializeField] private Vector2 screenPixelSize;

    private Vector2 edgeDistance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        CalculateEdgeDistance();
    }

    private void FixedUpdate()
    {
        if (!LocationManager.instance.isExamining || 
            LogManager.instance.isCheckingLogs || 
            DialogueManager.instance.interactionActive ||
            ClueManager.instance.isOpen)
            return;

        if (Input.mousePosition.x > Screen.width - horizontalEdge && cameraObject.transform.position.x < edgeDistance.x) //move camera right
        {
            cameraObject.transform.position += Vector3.right * speed * Time.fixedDeltaTime;

            if (cameraObject.transform.position.x >= edgeDistance.x) //align with screen edge
                cameraObject.transform.position = new Vector3(edgeDistance.x, cameraObject.transform.position.y, cameraObject.transform.position.z);
        }
        else if (Input.mousePosition.x < horizontalEdge && cameraObject.transform.position.x > -edgeDistance.x) //move camera left
        {
            cameraObject.transform.position += Vector3.left * speed * Time.fixedDeltaTime;

            if (cameraObject.transform.position.x <= -edgeDistance.x) //align with screen edge
                cameraObject.transform.position = new Vector3(-edgeDistance.x, cameraObject.transform.position.y, cameraObject.transform.position.z);
        }

        if (Input.mousePosition.y > Screen.height - verticalEdge && cameraObject.transform.position.y < edgeDistance.y) //move camera up
        {
            cameraObject.transform.position += Vector3.up * speed * Time.fixedDeltaTime;

            if (cameraObject.transform.position.y >= edgeDistance.y) //align with screen edge
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, edgeDistance.y, cameraObject.transform.position.z);
        }
        else if (Input.mousePosition.y < verticalEdge && cameraObject.transform.position.y > -edgeDistance.y) //move camera down
        {
            cameraObject.transform.position += Vector3.down * speed * Time.fixedDeltaTime;

            if (cameraObject.transform.position.y <= -edgeDistance.y) //align with screen edge
                cameraObject.transform.position = new Vector3(cameraObject.transform.position.x, -edgeDistance.y, cameraObject.transform.position.z);
        }
        characterCanvas.transform.position = new Vector3(characterCanvas.transform.position.x, cameraObject.transform.position.y);
    }

    public void CalculateEdgeDistance()
    {
        Rect spriteSize = LocationManager.instance.locationSprite.sprite.rect;

        edgeDistance = new Vector2(((spriteSize.width - screenPixelSize.x) / 2) * 0.01f, ((spriteSize.height - screenPixelSize.y) / 2) * 0.01f);
    }

    public void ResetCameraPosition()
    {
        cameraObject.transform.position = new Vector3(0, 0, cameraObject.transform.position.z);
        characterCanvas.transform.position = new Vector3(characterCanvas.transform.position.x, cameraObject.transform.position.y);
    }
}
