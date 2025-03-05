using System.Collections;
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
    [SerializeField] private GameObject mouseObject;
    [SerializeField] private float moveSpeed;

    private Vector2 edgeDistance;
    private Vector2 previousMousePos;
    private Vector2 screenMaxSize = new Vector2(960, 540);
    private Vector2 locationMaxSize = new Vector2(3.2f, 1.8f);
    private Examinable[] Examinables;
    private Examinable selectedExaminable;
    private Coroutine coroutine;

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
        {
            mouseObject.SetActive(false);
            return;
        }

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

        mouseObject.SetActive(true);
        if (previousMousePos != (Vector2)Input.mousePosition)
        {
            previousMousePos = (Vector2)Input.mousePosition;

            Vector2 screenSize = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 mousePos = (Vector2)Input.mousePosition - screenSize;

            Vector2 posPercentage = mousePos / screenSize;
            SetAimPosition(posPercentage * screenMaxSize, true);
        }
        else if (ControlManager.instance.Movement != Vector2.zero)
        {
            Vector2 newPos = (Vector2)mouseObject.transform.localPosition + (ControlManager.instance.Movement * moveSpeed);
            newPos = new Vector2(Mathf.Clamp(newPos.x, -screenMaxSize.x, screenMaxSize.x), Mathf.Clamp(newPos.y, -screenMaxSize.y, screenMaxSize.y));
            SetAimPosition(newPos, true);
        }
    }

    public void StartExamine()
    {
        coroutine = StartCoroutine(ExamineDetection());
    }

    public void StopExamine()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    private IEnumerator ExamineDetection()
    {
        while (true)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || ControlManager.instance.Progress());

            if (selectedExaminable != null)
            {
                Debug.Log(ControlManager.instance.Progress());
                selectedExaminable.Examine();
                DeselectExaminable();
            }
        }
    }

    public void GetExaminables(Transform location)
    {
        Examinables = location.GetComponentsInChildren<Examinable>();
    }

    public void ClearExaminables()
    {
        Examinables = null;
    }

    public void DeselectExaminable()
    {
        if (selectedExaminable == null)
            return;

        selectedExaminable.EndHover();
        selectedExaminable = null;
    }

    public void SetAimPosition(Vector2 position, bool select)
    {
        ControlManager.instance.DeselectButton();
        mouseObject.transform.localPosition = position;
        if (Examinables == null)
            return;

        Vector2 tempPos = position / screenMaxSize * locationMaxSize;

        if (selectedExaminable != null)
        {
            if (!selectedExaminable.IsPointInsideCollider(tempPos))
            {
                selectedExaminable.EndHover();
                selectedExaminable = null;
            }
        }

        if (selectedExaminable == null && select)
        {
            foreach (var item in Examinables)
            {
                if (item.IsPointInsideCollider(tempPos))
                {
                    selectedExaminable = item;
                    selectedExaminable.StartHover();
                    break;
                }
            }
        }
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
