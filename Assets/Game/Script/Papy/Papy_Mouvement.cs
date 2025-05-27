using UnityEngine;
using UnityEngine.AI;
using Fusion;
using System.Collections;
using System;
using Photon.Voice;
using UnityEngine.UIElements;
using Unity.VisualScripting;
public class Papy_Mouvement : NetworkBehaviour
{
    public NavMeshAgent agent;
    public float MinimumDistance = 10f;
    public float ChaseDelay = 0;
    public float MinimumPI = 0;

    private bool canPassThroughWalls = false;

    public Rigidbody body;
    public Vector3 CurrentPointToReach;
    public NetworkBool HaveReachThePoint;

    //Header("Data")]
    [SerializeField] float Speed;

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
        if (HasStateAuthority)
        {
            if (Papy_Manager.Instance.CanMove)
            {

                if (Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.Patrol)
                {
                    ChooseTarget();
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
                    SearchingMouvement();
                    if (IsPointReach(CurrentPointToReach))
                    {
                        HaveReachThePoint = IsPointReach(CurrentPointToReach);
                        GetAPoint(CurrentPointToReach);
                    }
                }
                else if (Papy_Manager.Instance.currentState == Papy_Manager.Papy_State.chasing)
                {
                    ChaseMouvement();
                }
                SphereCheck();
                Papy_Manager.Instance.pAnim.Walk();
            }
        }
        
    }
    bool IsPointReach(Vector3 target)
    {
        float distance = Vector3.Distance(transform.position, target);
        return distance <= MinimumDistance;
    }

    public void GetAPoint(Vector3 oldPoint)
    {
        if (HasStateAuthority)
        {

            int index = UnityEngine.Random.Range(0, Papy_Manager.Instance.PointToReach.Length);

            if (Papy_Manager.Instance.PointToReach[index].position == oldPoint)
            {
                GetAPoint(oldPoint);
                return;
            }
            
            for (int i = 0; i < Papy_Manager.Instance.PointToReach.Length; i++)
            {
                if (i == index)
                {
                    CurrentPointToReach = Papy_Manager.Instance.PointToReach[i].position;
                    break;
                }
            }
            
        }
    }



    #region Patrol

    void PatrolMouvement()
    {
        Debug.Log("PatrolMenu");
        agent.enabled = true;
        agent.destination = CurrentPointToReach;
    }

    #endregion
    #region Searching

    void SearchingMouvement()
    {
        agent.enabled = true;
        agent.destination = CurrentPointToReach;

    }
    void ChooseTarget()
    {
        // check in update wich player have more of IP (interrest point)
        
        for (int i = 0; i < GameManager.Instance.PlayerMeshes.Length; i++)
        {
            if (GameManager.Instance.PlayerMeshes[i] != null)
            {
                PlayerInterrestPoint PI = GameManager.Instance.PlayerMeshes[i].GetComponent<PlayerInterrestPoint>();

                if (PI.TotalPI >= MinimumPI)
                {
                    Debug.Log("Teleport");
                    Papy_Manager.Instance.Rpc_ChangeStatus(2);
                    Papy_Manager.Instance.Rpc_TeleportToPreciseLocation(GetClosestPointFromPlayer(PI.gameObject.transform).position, GetClosestPointFromPlayer(PI.gameObject.transform).rotation);
                }
            }
        }
        
    }

    Transform GetClosestPointFromPlayer(Transform playerTransform)
    {
        // 1. Convertir la position du joueur en local dans son monde
        Vector3 localPos = GameManager.Instance.World.transform.InverseTransformPoint(playerTransform.position);

        // 3. Chercher le point de téléportation le plus proche dans le monde cible
        float minDistance = Mathf.Infinity;
        Transform closestTransform = null;

        foreach (Transform Point in Papy_Manager.Instance.PointToReach)
        {
            float dist = Vector3.Distance(localPos, Point.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestTransform = Point;
            }
        }

        return closestTransform;
    }
    #endregion
    #region chasing

    void ChaseMouvement()
    {
        Debug.Log("ChaseMenu");
        agent.enabled = false;
        Vector3 currentPosition = transform.position;

        float distance = Vector3.Distance(currentPosition, CurrentPointToReach);

        if (distance > 0)
        {
            Vector3 directionOfTravel = (CurrentPointToReach - currentPosition).normalized;
            Vector3 newPosition = currentPosition + (directionOfTravel * Speed * NetworkManager.runnerInstance.DeltaTime);
            body.MovePosition(newPosition);
        }
    }

    public void CaughtPlayer(NetworkObject obj)
    {
        NetworkManager.runnerInstance.Spawn(Papy_Manager.Instance.Corrosion,obj.transform.position+Vector3.down,obj.transform.rotation);

        Transform target = Papy_Manager.Instance.gameObject.transform;
        Vector3 direction = target.position - obj.transform.position;

        // Supprimer l'influence verticale
        direction.y = 0;

        // S'il reste une direction, appliquer la rotation
        if (direction != Vector3.zero)
        {
            obj.transform.rotation = Quaternion.LookRotation(direction);
        }



        if (obj.GetComponent<PickItem>() != null && obj.GetComponent<PickItem>().NumberOfFlashGrenade <= 0)
        {
            Debug.Log("Have not flash");
            obj.GetComponent<PlayerMouvement>().Rpc_TeleportMesh(GetClosestPocketPoint(obj.transform).position, GetClosestPocketPoint(obj.transform).rotation);
            obj.GetComponent<PlayerMouvement>().IsInPocketDim = true;
            GetAPoint(obj.transform.position);
        }
        else
        {
            Debug.Log("Have Flash");
            obj.GetComponent<PickItem>().NumberOfFlashGrenade--;
            obj.GetComponent<PlayerMouvement>().playerUI.FlashText.text = obj.GetComponent<PickItem>().NumberOfFlashGrenade.ToString();
        }
        obj.GetComponent<PlayerInterrestPoint>().Rpc_ResetPI();
        Papy_Manager.Instance.Rpc_FloorSpawnPortals(transform.position, transform.rotation);
        Papy_Manager.Instance.Rpc_TeleportPapy();
        Papy_Manager.Instance.Rpc_ChangeStatus(1);
        Papy_Manager.Instance.pVision.Target = null;
    }
    #endregion


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7) // PayerLayer
        {
            Debug.Log("Papy touch player");
            CaughtPlayer(collision.gameObject.GetComponent<NetworkObject>());
        }
        if (collision.gameObject.layer == 8) // wall
        {
            //spawn portal
            Debug.Log("Touche Wall");

           // Papy_Manager.Instance.WRpc_SpawnPortals(collision.transform.position,collision.transform.rotation);
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            GetAPoint(CurrentPointToReach);
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

    void SphereCheck()
    {
        Debug.Log("SpereCheck");
        RaycastHit[] hits = new RaycastHit[10]; // Taille fixe
        Ray ray = new Ray(transform.position, transform.forward);
        LayerMask mask = LayerMask.GetMask("Props");

        int hitCount = Physics.SphereCastNonAlloc(ray, 0.1f, hits,0.1f, mask);

        Debug.Log("HIT COUNT = " + hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            
            GameObject hitObject = hits[i].collider.gameObject;

            if (hitObject.layer == LayerMask.NameToLayer("Props"))
            {
                Debug.Log("Hit props");
                if (hitObject.GetComponent<NetworkObject>() && hitObject.GetComponent<NetworkObject>().HasStateAuthority)
                {
                    Debug.Log("Despawn props");
                    NetworkManager.runnerInstance.Despawn(hitObject.GetComponent<NetworkObject>());
                    NetworkManager.runnerInstance.Spawn(Papy_Manager.Instance.Corrosion, hitObject.transform.position, Quaternion.identity);
                }
            }
        }
    }
}


