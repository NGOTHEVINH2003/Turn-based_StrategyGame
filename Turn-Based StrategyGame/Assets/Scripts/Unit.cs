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
    [SerializeField] private Animator unitAnimator;
    private float rotatingSpeed = 5f;

    private void Awake()
    {
        targetPosition = transform.position;
    }

    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            transform.position += moveDir * Time.deltaTime * MoveSpeed;

            transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * rotatingSpeed);

            unitAnimator.SetBool("IsWalking", true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking", false);
        }

    }

}
