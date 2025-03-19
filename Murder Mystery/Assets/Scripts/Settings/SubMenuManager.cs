using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SubMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject background;

    private List<GameObject> subMenus = new List<GameObject>(); 
    [SerializeField] private SettingsButton[] buttons; 
    
    private void Start()
    {
        //buttons = FindObjectsOfType<SettingsButton>();
        foreach (Transform menu in transform)
        {
            if (menu.gameObject == background)
                continue;
            subMenus.Add(menu.gameObject);
        }
        CloseAllSubMenus();
    }

    public void SelectSubMenu(string menuName)
    {
        foreach (GameObject menu in subMenus)
        {
            menu.SetActive(menu.name == menuName);
        }
        foreach (SettingsButton button in buttons)
        {
            button.Select(button.name == menuName);
        }
    }

    public void CloseAllSubMenus()
    {
        foreach (GameObject menu in subMenus)
        {
            menu.SetActive(false);
        }
    }
}
