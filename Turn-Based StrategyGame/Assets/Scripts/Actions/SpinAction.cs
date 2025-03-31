using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpinAction : BaseAction 
{


    public delegate void SpinCompleteDelegate();

    private float totalSpinAmount;

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        float spinAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAmount, 0);
        totalSpinAmount += spinAmount;
        if(totalSpinAmount >= 360f)
        {
            ActionComplete();
        }

    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPOistion = unit.GetGridPosition();

        return new List<GridPosition> { unitGridPOistion };

    }

    public override int GetActionPointsCost()
    {
        return 2;
    }
}
