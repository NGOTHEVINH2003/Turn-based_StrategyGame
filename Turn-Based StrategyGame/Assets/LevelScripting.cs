using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScripting : MonoBehaviour
{

    [SerializeField] private List<GameObject> hider1List;
    [SerializeField] private List<GameObject> hider2List;
    [SerializeField] private List<GameObject> hider3List;
    [SerializeField] private List<GameObject> hider4List;
    [SerializeField] private List<GameObject> enemy1List;
    [SerializeField] private List<GameObject> enemy2List;
    [SerializeField] private List<GameObject> enemy3List;
    [SerializeField] private List<GameObject> enemy4List;
    [SerializeField] private Door door1;
    [SerializeField] private Door door2;
    [SerializeField] private Door door3;
    [SerializeField] private Door door4;
    [SerializeField] private Door door5;



    private bool hasShownThirdHider = false;
    private void Start()
    {
        door1.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(hider2List, false);
            SetActiveGameObjectList(enemy2List, true);
        };
        door2.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(hider1List, false);
            SetActiveGameObjectList(enemy1List, true);
        };
        door3.OnDoorOpened += (object sender, EventArgs e) =>
        {
            if (!hasShownThirdHider)
            {
                SetActiveGameObjectList(hider3List, false);
                SetActiveGameObjectList(enemy3List, true);
                hasShownThirdHider = true; 
            }
           
        }; 
        door4.OnDoorOpened += (object sender, EventArgs e) =>
        {
            if (!hasShownThirdHider)
            {
                SetActiveGameObjectList(hider3List, false);
                SetActiveGameObjectList(enemy3List, true);
                hasShownThirdHider = true;
            }
            
        };
        door5.OnDoorOpened += (object sender, EventArgs e) =>
        {
            SetActiveGameObjectList(hider4List, false);
            SetActiveGameObjectList(enemy4List, true);
        };
    }

    

    private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive)
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            gameObject.SetActive(isActive);
        }
    }

}
