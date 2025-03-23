using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 3;
    private const int ACTION_POINTS_ADD = 2;


    public static event EventHandler OnAnyActionPointsChanged;

    [SerializeField] private bool isEnemy;


    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private int actionPoints = 2;
    private BaseAction[] baseActionArray;


    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);

        TurnSystem.Instance.OnTurnEnd += TurnSystem_OnTurnEnd;
        healthSystem.onDead += HealthSystem_OnDead;

    }

   
    private void Update()
    {

        //update unit position
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if(newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMovePos(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }

    }
    public bool TrySpendActionPoints(BaseAction baseAction)
    {
        if (CanSpendActionPoint(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        return false;
    }
    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;

        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnEnd(object sender, EventArgs e)
    {
        if(
            (IsEnemy() && !TurnSystem.Instance.IsPlayerTurn())
            || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())
          )
        {
            actionPoints += ACTION_POINTS_ADD;
            if (actionPoints > ACTION_POINTS_MAX) actionPoints = ACTION_POINTS_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
       
    }

    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPos(gridPosition, this);
        Destroy(gameObject);
    }




    public bool CanSpendActionPoint(BaseAction baseAction)
    {
        if(actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public int GetActionPoints()
    {
        return actionPoints;
    }

    public SpinAction GetSpinAction() 
    {  
        return spinAction; 
    } 

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public Vector3 getWorldPosition()
    {
        return transform.position;
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public BaseAction[] GetBaseActionsArray()
    {
        return baseActionArray;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

}
