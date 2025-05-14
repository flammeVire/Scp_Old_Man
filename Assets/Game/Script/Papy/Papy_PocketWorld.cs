using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UIElements;

public class Papy_PocketWorld : NetworkBehaviour
{
    public float durationTime;
    float timePast;
    public bool isPocket;
    private void Start()
    {
        timePast = durationTime;
        Invoke("DespawnPortal", durationTime);
        StartCoroutine(ReduceScale());
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7 && isPocket)
        {
            Debug.Log("Player in portal");
            other.GetComponent<PlayerMouvement>().Rpc_TeleportMesh(GetOtherWorldTransform(),other.transform.rotation);
        }
    }


    IEnumerator ReduceScale()
    {
        yield return new WaitForSeconds(1);
        timePast = timePast - 1;
        float percent = timePast / durationTime;
        if(percent > 0)
        {
            this.transform.localScale = new Vector3(percent, percent, percent);
        }
        StartCoroutine(ReduceScale());
    }

    public void DespawnPortal()
    {
        Debug.Log("Despawn");
        if (HasStateAuthority)
        {
            NetworkManager.runnerInstance.Despawn(gameObject.GetComponent<NetworkObject>());
        }
    }

    Vector3 GetOtherWorldTransform()
    {
        Vector3 localPos = GameManager.Instance.PocketWorld.transform.InverseTransformPoint(transform.position);
        return GameManager.Instance.World.transform.TransformPoint(localPos);
    }
}
