using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FMOD;
using FMODUnity;

public class Scenarium : NetworkBehaviour
{
    public float speed;
    public Papy_Manager manager;
    public Transform fakePoint;
    public GameObject door;
    public Rigidbody body;

    public StudioEventEmitter WalkSound;
    public StudioEventEmitter Riresound;

    [Networked]public NetworkBool canGo {  get; private set; }
   void MeshGoToPoint()
    {
        Vector3 currentPosition = transform.position;

        float distance = Vector3.Distance(currentPosition, fakePoint.position);

        if (distance > 0)
        {
            Vector3 directionOfTravel = (fakePoint.position - currentPosition).normalized;
            Vector3 newPosition = currentPosition + (directionOfTravel * speed * NetworkManager.runnerInstance.DeltaTime);
            body.MovePosition(newPosition);
        }
    }

    public void Start()
    {
        StartCoroutine(CanMove());
    }

    private void Update()
    {
        if (canGo)
        {
            TryToLookAt(fakePoint.position);
            MeshGoToPoint();
        }
    }

    IEnumerator CanMove()
    {
        yield return new WaitUntil(() => door == null);
        Rpc_GO();
        
    }

    bool IsPointReach()
    {
        float distance = Vector3.Distance(transform.position, fakePoint.position);
        return distance <= 0.5f;
    }

    IEnumerator TP()
    {
        yield return new WaitUntil(() => IsPointReach());
        Rpc_TP();

    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_GO()
    {
        canGo = true;
        WalkSound.Play();
        if (HasStateAuthority) 
        {
            StartCoroutine(TP());
        }
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    void Rpc_TP()
    {
        WalkSound.Stop();
        Riresound.Play();
        manager.CanMove = true;
        if (HasStateAuthority) 
        {
            NetworkManager.runnerInstance.Despawn(this.GetComponent<NetworkObject>());
        }
    }

    public void TryToLookAt(Vector3 targetPosition)
    {
        // Ignore la hauteur pour une rotation horizontale
        targetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            body.MoveRotation(targetRotation);
        }
    }
}
