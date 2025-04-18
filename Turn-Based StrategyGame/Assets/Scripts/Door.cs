using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public static event EventHandler OnAnyDoorOpened;
    public event EventHandler OnDoorOpened;

    [SerializeField] private bool isOpen;
    private List<GridPosition> occupiedGridPositions;
    private Animator animator;
    private Action onInteractComplete;
    private bool isActive;
    private float timer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        occupiedGridPositions = new List<GridPosition>();

        Vector3 worldCenterPosition = transform.position;
        int halfX = Mathf.FloorToInt(transform.localScale.x);
        
        List<Vector3> offsets = new List<Vector3>();

        for (int x = -halfX+1; x < halfX; x++)
        {
            offsets.Add(new Vector3(x, 0, 0));
        }
        foreach (var offset in offsets)
        {
            Vector3 rotatedOffset = transform.rotation * offset;

            Vector3 worldPosition = worldCenterPosition + rotatedOffset;

            GridPosition pos = LevelGrid.Instance.GetGridPosition(worldPosition);


            occupiedGridPositions.Add(pos);

            if (LevelGrid.Instance == null)
            {
                Debug.LogError("LevelGrid.Instance is null!");
                return;
            }

            if (PathFinding.Instance == null)
            {
                Debug.LogError("PathFinding.Instance is null!");
                return;
            }

            LevelGrid.Instance.SetInteractableAtGridPosition(pos, this);
            PathFinding.Instance.SetIsWalkableGridPosition(pos, isOpen);
        }

        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            isActive = false;
            onInteractComplete();
        }
    }


    public void Interact(Action onInteractComplete)
    {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = 0.5f;

        if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        foreach (var pos in occupiedGridPositions)
        {
            PathFinding.Instance.SetIsWalkableGridPosition(pos, true);
        }
        OnAnyDoorOpened?.Invoke(this, EventArgs.Empty);
        OnDoorOpened?.Invoke(this, EventArgs.Empty);
    }
    private void CloseDoor() 
    {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        foreach (var pos in occupiedGridPositions)
        {
            PathFinding.Instance.SetIsWalkableGridPosition(pos, false);
        }
    }
}
