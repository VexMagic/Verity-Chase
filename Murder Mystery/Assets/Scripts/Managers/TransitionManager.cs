using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;

    [SerializeField] private float openSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject loadingIcon;

    private int tempScene = -1;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        loadingIcon.SetActive(false);
    }

    public void EnterScene(int scene)
    {
        if (tempScene != -1)
            return;

        tempScene = scene;
        animator.SetBool("Open", true);

        Invoke(nameof(StartLoading), openSpeed + 0.1f);
        Invoke(nameof(LoadCaseScene), openSpeed);
    }

    private void StartLoading()
    {
        ControlManager.instance.DeselectButton();
        loadingIcon.SetActive(true);
    }

    private void LoadCaseScene()
    {
        SceneManager.LoadSceneAsync(tempScene);
    }
}
