using System; 
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainer;
    [SerializeField] private TextMeshProUGUI actionPointText;

    private List<ActionButtonUI> actionButtonUIList;
    

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnSelectedActionStarted;
        TurnSystem.Instance.OnTurnEnd += TurnSystem_OnTurnEnd;

        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        CreateUnitActionButton();
        UpdateSeletedVisual();
        UpdateActionPoints();
    }



    private void CreateUnitActionButton()
    {
        foreach(Transform buttonTransform in actionButtonContainer)
        {
            Destroy(buttonTransform.gameObject);
        }
        actionButtonUIList.Clear();

        Unit selectedunit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach(BaseAction baseAction in selectedunit.GetBaseActionsArray())
        {
           Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainer);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UpdateSeletedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void TurnSystem_OnTurnEnd(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
    //guarantee action points update correctly
    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }


    private void UnitActionSystem_OnSelectedActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }


    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButton();
        UpdateSeletedVisual();
        UpdateActionPoints();
    }
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSeletedVisual();
    }

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointText.text = "Action Points: " + selectedUnit.GetActionPoints();
    }
}
