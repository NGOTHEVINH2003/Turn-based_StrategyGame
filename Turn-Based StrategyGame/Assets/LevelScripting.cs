using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] private Transform panel;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button restartButton;


    private bool hasShownThirdHider = false;
    private bool winCon = false;
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
            winCon = true;
        };

        UnitManager.Instance.OnDefeated += UnitManager_Instance_OnDefeated;
        UnitManager.Instance.OnWinning += Instance_OnWinning;
    }

    private void Instance_OnWinning(object sender, EventArgs e)
    {
        if (winCon)
        {
            panel.gameObject.SetActive(true);
            gameOverText.text = "YOU WIN!";
        }
    }

    private void UnitManager_Instance_OnDefeated(object sender, EventArgs e)
    {
        panel.gameObject.SetActive(true);
        gameOverText.text = "YOU LOSE!";
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetActiveGameObjectList(List<GameObject> gameObjectList, bool isActive)
    {
        foreach (GameObject gameObject in gameObjectList)
        {
            gameObject.SetActive(isActive);
        }
    }

}
