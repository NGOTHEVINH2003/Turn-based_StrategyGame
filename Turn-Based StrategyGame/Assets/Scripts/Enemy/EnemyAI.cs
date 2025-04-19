using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private enum State
    {
        Waiting,
        TakingTurn,
        Busy,
    }
    private State state;
    private float timer;
    private void Awake()
    {
        state = State.Waiting;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnEnd += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {

        if (TurnSystem.Instance.IsPlayerTurn()) return;

        timer -= Time.deltaTime;

        switch (state)
        {
            case State.Waiting:
                break;
            case State.TakingTurn:
                if (timer <= 0f)
                {
                    
                    state = State.Busy;
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                    
                }
                break;
            case State.Busy: 
                break;
        }

       
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
       
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        Debug.Log("take Enemy AI Action!");
        List<Unit> enemyList = UnitManager.Instance.GetEnemyUnitList();

        if(enemyList.Count == 0)
        {
            return false;
        }

        foreach(Unit enemy in enemyList)
        {
            if(TryTakeEnemyAIAction(enemy, onEnemyAIActionComplete))
            {
                return true;
            }
            
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Unit enemy, Action onEnemyAIActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach(BaseAction baseAction in enemy.GetBaseActionsArray())
        {
            if (!enemy.CanSpendActionPoint(baseAction))
            {
                continue;
            }

            if(bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if(testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
           
        }

        if(bestEnemyAIAction != null && enemy.TrySpendActionPoints(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }
}
