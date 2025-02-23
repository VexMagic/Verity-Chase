using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenuManager : MonoBehaviour
{
    private List<GameObject> subMenus = new List<GameObject>(); 
    
    private void Start()
    {
        foreach (Transform menu in transform) 
        {
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
    }

    public void CloseAllSubMenus()
    {
        foreach (GameObject menu in subMenus)
        {
            menu.SetActive(false);
        }
    }
}
