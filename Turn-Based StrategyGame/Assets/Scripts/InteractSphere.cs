using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private MeshRenderer meshRenderer;


    private GridPosition gridPosition;
    private Action onInteractComplete;
    private bool isActive;
    private float timer;
    private bool isGreen;

    private void Start()
    {
        SetColorGreen();
    }
    private void Update()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isActive = false;
            onInteractComplete();
        }
    }

    private void SetColorGreen()
    {
        isGreen = true;
        meshRenderer.material = greenMaterial;
    }

    private void SetColorRed()
    {
        isGreen = false;
        meshRenderer.material = redMaterial;
    }

    public void Interact(Action onInteractComplete)
    {

        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f; 
        if (isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }
}
