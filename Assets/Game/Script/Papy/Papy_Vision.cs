using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Papy_Vision : MonoBehaviour
{
    public float raduis;
    [Range(0, 360)] public float angle;

    public GameObject Target;

    public Transform LastestTargetPos;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public LayerMask obstructionMask2;

    public bool canSeePlayer;

    private void Update()
    {
        FieldOfViewCheck();
        /*
        if (!canSeePlayer && Target != null)
        {
            LastestTargetPos = Target.transform;
            Target = null;
        }*/
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, raduis, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Combine les deux masks d'obstruction
                LayerMask combinedObstructionMask = obstructionMask | obstructionMask2;

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, combinedObstructionMask))
                {
                    canSeePlayer = true;
                    Target = rangeChecks[0].gameObject;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }
}
