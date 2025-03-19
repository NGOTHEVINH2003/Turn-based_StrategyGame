using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private Button endTurnBtn;
    [SerializeField] private GameObject enemyTurnVisualObject;


    private void Start()
    {
        endTurnBtn.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnEnd += TurnSystem_OnTurnEnd;

        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnVisibility();
    }

    private void TurnSystem_OnTurnEnd(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnVisibility();
    }

    private void UpdateTurnText()
    {
        turnNumberText.text = "Turn: " + TurnSystem.Instance.GetTurnNumber();
    }
    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnVisibility()
    {
        endTurnBtn.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
