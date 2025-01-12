using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private float MoveSpeed = 4.0f;
    private float stoppingDistance = .1f;



    private void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            transform.position += moveDir * Time.deltaTime * MoveSpeed;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Move(MouseWorld.GetMousePosition());
        }

    }

}
