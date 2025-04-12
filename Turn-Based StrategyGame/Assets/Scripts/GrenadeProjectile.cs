using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodePrefab;
    [SerializeField] private TrailRenderer grenadeTrail;
    [SerializeField] private AnimationCurve arcYanimationCurve;


    private Vector3 targetPosition;
    private float moveSpeed = 15f;
    private float reachTargetDistance = .2f;
    private Action onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;
    private int grenadeDamage = 50;

    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;
        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalize = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;
        float positionY = arcYanimationCurve.Evaluate(distanceNormalize) * maxHeight;
        
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
        if(Vector3.Distance(transform.position, targetPosition) < reachTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] collierArray = Physics.OverlapSphere(targetPosition, damageRadius );
            foreach(Collider col in collierArray)
            {
                if(col.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(grenadeDamage);
                }
                if(col.TryGetComponent<DestructableCrate>(out DestructableCrate destructable))
                {
                    destructable.Damage();
                }
                
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            grenadeTrail.transform.parent = null;
            Instantiate(grenadeExplodePrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);
             
            onGrenadeBehaviourComplete();
        }
        
    }
    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, targetPosition);
    }
}
