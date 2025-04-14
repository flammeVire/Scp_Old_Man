using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Papy_Vision : MonoBehaviour
{
    public float raduis;
    [Range(0,360)] public float angle;

    public GameObject[] playerMesh;
    public GameObject Target;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    
    private void Update()
    {
            FieldOfViewCheck();
            //if bool true change the state of papy-manager
    }

    void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, raduis, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward,directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if(!Physics.Raycast(transform.position,directionToTarget, distanceToTarget, obstructionMask))
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
