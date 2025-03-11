using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
   
 
    private GridPosition gridPosition;
    private MoveAction moveAction;

    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
    }


    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPos(gridPosition, this);
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

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

   

}
