using UnityEngine;
using UnityEngine.AI;
using Fusion;
using System.Collections;
using System;
using Photon.Voice;
using UnityEngine.UIElements;
public class Papy_Mouvement : NetworkBehaviour
{
    public NavMeshAgent agent;
    public float MinimumDistance = 10f;
    public float ChaseDelay = 0;

    private bool canPassThroughWalls = false;

    public Rigidbody body;
    public Transform CurrentPointToReach;
    public NetworkBool HaveReachThePoint;
    //Header("Data")]
    [SerializeField] float Speed;
    private void OnValidate()
    {
        Speed = GetComponent<NavMeshAgent>().speed - 1;
    }
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

        if (Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.Patrol)
        {
            Debug.Log("Patrol");
            PatrolMouvement();
            if (IsPointReach(CurrentPointToReach))
            {
                HaveReachThePoint = IsPointReach(CurrentPointToReach);
                GetAPoint(CurrentPointToReach);
            }
        }
        else if (Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.searching)
        {
            Debug.Log("Searching");
        }
        else if (Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.chasing)
        {
            Debug.Log("Chasing");
            ChaseMouvement();
        }
        Papy_Manager.Instance.LookAt(CurrentPointToReach.position);
    }

    public override void FixedUpdateNetwork()
    {
        /*
        if (Papy_Manager.Instance.currentState != Papy_Manager.Papy_State.chasing)
        {
            
        }

        */
    }

    bool IsPointReach(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= MinimumDistance;
    }

    public void GetAPoint(Transform oldPoint)
    {
        int index = UnityEngine.Random.Range(0, Papy_Manager.Instance.PointToReach.Length);

        if (Papy_Manager.Instance.PointToReach[index] == oldPoint)
        {
            GetAPoint(oldPoint);
            return;
        }

        for (int i = 0; i < Papy_Manager.Instance.PointToReach.Length; i++)
        {
            if (i == index)
            {
                CurrentPointToReach = Papy_Manager.Instance.PointToReach[i];
                break;
            }


        }
    }



    #region Patrol

    void PatrolMouvement()
    {
        Debug.Log("PatrolMenu");
        agent.enabled = true;
        agent.destination = CurrentPointToReach.position;
    }

    #endregion 
    #region Searching

    #endregion
    #region chasing

    void ChaseMouvement()
    {
        Debug.Log("ChaseMenu");
        agent.enabled = false;
        Vector3 currentPosition = transform.position;

        float distance = Vector3.Distance(currentPosition, CurrentPointToReach.position);

        if (distance > 0)
        {
            Vector3 directionOfTravel = (CurrentPointToReach.position - currentPosition).normalized;
            Vector3 newPosition = currentPosition + (directionOfTravel * Speed * NetworkManager.runnerInstance.DeltaTime);
            body.MovePosition(newPosition);
        }
    }
    public void CaughtPlayer(GameObject obj)
    {
        obj.GetComponent<PlayerMouvement>().Rpc_TeleportMesh(GetClosestPocketPoint(obj.transform).position, GetClosestPocketPoint(obj.transform).rotation);
        Papy_Manager.Instance.currentState = Papy_Manager.Papy_State.Patrol;
        GetAPoint(obj.transform);
        Papy_Manager.Instance.Rpc_SpawnPortals(transform.position, transform.rotation);

    }
    #endregion


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7) // PayerLayer
        {
            Debug.Log("Papy touch player");
            CaughtPlayer(collision.gameObject);
        }
        if (collision.gameObject.layer == 6) // wall
        {
            //spawn portal
            Papy_Manager.Instance.Rpc_SpawnPortals(collision.transform.position,collision.transform.rotation);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Debug.Log("Papy quitter joueur ??");
        }
    }


    Transform GetClosestPocketPoint(Transform playerTransform)
    {
        // get le player transform et le convertir a l'autre monde (ex: si le joueur est en 10,10 et l'autre monde en -100 -100, le tp en 10 10 de -100 -100)
        /*
        Transform PocketTransform =

        float minimDistance  = Mathf.Infinity;
        Transform minimTransform = null;
        for (int i = 0; i < GameManager.Instance.PocketDimentionSpawn.Length; i++) 
        {
            if(Vector3.Distance(playerTransform.position, GameManager.Instance.PocketDimentionSpawn[i].position) < minimDistance)
            {
                minimDistance = Vector3.Distance(playerTransform.position, GameManager.Instance.PocketDimentionSpawn[i].position);
                minimTransform = GameManager.Instance.PocketDimentionSpawn[i];
            }
        }
        if (minimDistance != Mathf.Infinity) 
        {
            return minimTransform;
        }
        else
        {
            return null;
        }
        */
        /*
         * Vector3 localPos3D = new Vector3(localPosition.x, localPosition.y, 0f);

        // Convertir la position locale en position monde pour world1
        Vector3 worldPos1 = world1.TransformPoint(localPos3D);
        GameObject obj1 = new GameObject("ObjectInWorld1");
        obj1.transform.position = worldPos1;

        // Convertir la position locale en position monde pour world2
        Vector3 worldPos2 = world2.TransformPoint(localPos3D);
        GameObject obj2 = new GameObject("ObjectInWorld2");
        obj2.transform.position = worldPos2;
         */
        // 1. Convertir la position du joueur en local dans son monde
        Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(playerTransform.position);

        // 2. Reprojeter cette position dans le monde cible
        Vector3 projectedPositionInTargetWorld = GameManager.Instance.PocketWorld.transform.TransformPoint(localPos);

        // 3. Chercher le point de téléportation le plus proche dans le monde cible
        float minDistance = Mathf.Infinity;
        Transform closestTransform = null;

        foreach (Transform spawnPoint in GameManager.Instance.PocketDimentionSpawn)
        {
            float dist = Vector3.Distance(projectedPositionInTargetWorld, spawnPoint.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestTransform = spawnPoint;
            }
        }

        return closestTransform;
    }
}


