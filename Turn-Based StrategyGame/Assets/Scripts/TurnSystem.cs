using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance {get; private set;}

    public event EventHandler OnTurnEnd;

    private int turnNumber = 1;
    private bool isPlayerTurn = true;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one Turn System!" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void NextTurn()
    {
        if(UnitManager.Instance.GetEnemyUnitList().Count == 0) {
            //when no enemy.
            turnNumber++;
            isPlayerTurn = true;
            OnTurnEnd?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            turnNumber++;
            isPlayerTurn = !isPlayerTurn;
            OnTurnEnd?.Invoke(this, EventArgs.Empty);
        } 
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
