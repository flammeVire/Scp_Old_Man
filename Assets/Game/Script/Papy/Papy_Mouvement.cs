using UnityEngine;
using UnityEngine.AI;
using Fusion;
using System.Collections;
public class RandomMovement : NetworkBehaviour
{
    public NavMeshAgent agent;
    public float range = 10f; // Portée des déplacements aléatoires
    private bool canPassThroughWalls = false;

    public Transform CurrentPointToReach;
    public NetworkBool HaveReachThePoint;
    /*
    void Start()
    {
        InvokeRepeating(nameof(SetRandomDestination), 0, 5f); // Change de destination toutes les 5s
        InvokeRepeating(nameof(ToggleWallPass), 30f, 30f); // Alterne le mode toutes les 30s
    }

    void SetRandomDestination()
    {
        Vector3 randomPos = RandomNavSphere(transform.position, range, -1);
        agent.SetDestination(randomPos);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }

    void ToggleWallPass()
    {
        canPassThroughWalls = !canPassThroughWalls;
        if (canPassThroughWalls)
        {
            agent.enabled = false; // Désactive la navigation
        }
        else
        {
            agent.enabled = true; // Réactive la navigation après 30s
            SetRandomDestination(); // Trouve une nouvelle destination
        }
    }
    */

    private void Update()
    {
        if(Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.Patrol)
        {

        }
        else if(Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.searching)
        { 

        }
        else if(Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.chasing)
        {

        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsPointReach(CurrentPointToReach)) 
        {
           HaveReachThePoint = IsPointReach(CurrentPointToReach);
            GetAPoint(CurrentPointToReach);
        }
        else
        {

        }
    }

    bool IsPointReach(Transform target, float threshold = 1.0f)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= threshold;
    }

    public void GetAPoint(Transform oldPoint)
    {
        int index = Random.Range(0, Papy_Manager.Instance.PointToReach.Length);

        if (Papy_Manager.Instance.PointToReach[index] == oldPoint)
        {
            GetAPoint(oldPoint);
            return;
        }

        for (int i = 0; i < Papy_Manager.Instance.PointToReach.Length; i++)
        {
           if(i == index)
           {
                CurrentPointToReach = Papy_Manager.Instance.PointToReach[i];
                break;
           }
        
         
        }
    }

    #region Patrol


    #endregion 
    #region Searching

    #endregion
    #region chasing

    #endregion
}
