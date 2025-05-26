using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SoundDoor : NetworkBehaviour
{
    [Range(0,10)]public int SoundRequired;
    public Operation operation;

    public Animation anim;
    public NetworkObject Door1;
    public NetworkObject Door2;
    bool IsOpen;
    bool SomeoneInside;

    [SerializeField] Sprite[] soundSpite = new Sprite[10];
    [SerializeField] Image soundImage;
    
    List<GameObject> PlayerInside = new List<GameObject>();

    public enum Operation
    {
        Greater,
        Lower,
        Equal,
    }

    private void OnTriggerEnter(Collider other)
    {
        SomeoneInside = true;
        PlayerInside.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInside.Remove(other.gameObject);
        if (PlayerInside.Count <= 0)
        {
            SomeoneInside = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (SomeoneInside) 
        {
            Debug.Log("SomeoneInside");
            CheckPlayerPI();
            
        }
    }


    public void CheckPlayerPI()
    {
        int globalPI = 0;
        foreach (GameObject go in PlayerInside)
        {
            globalPI += go.GetComponent<PlayerInterrestPoint>().CurrentPI;
        }
        Rpc_UpdateUI(globalPI);
        Debug.Log("global PI inDoor = " + globalPI);
        if (checkOperation(globalPI))
        {
            Debug.Log("Open the door");
            Rpc_OpenDoor();

            //MattSounds : mettre fonction RPC qui joue un son si le volume dépasse un certain seuil (PI va de 0 a 10)
        }
    }

    bool checkOperation(int globalPI)
    {

        switch (operation)
        {
            case Operation.Greater:
                if (globalPI >= SoundRequired)
                {
                    return true;
                }
                break;
            case Operation.Lower:
                if (globalPI <= SoundRequired)
                {
                    return true;
                }
                break;
            case Operation.Equal:
                if (globalPI == SoundRequired)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    async void Rpc_OpenDoor()
    {
        if (HasStateAuthority)
        {
            if (!anim.isPlaying)
            {
                anim.Play();
                anim[anim.clip.name].speed = 1;              // Joue l’animation en sens inverse
                anim[anim.clip.name].time = 0;
            }

            await WaitForAnimationEnd();

            if (Door1.HasStateAuthority)
            {

                Debug.Log("Have Authority");
                NetworkManager.runnerInstance.Despawn(Door1);

                //MattSounds : jouer son ouverture
                IsOpen = true;
            }
            if (Door2.HasStateAuthority)
            {
                NetworkManager.runnerInstance.Despawn(Door2);
                IsOpen = true;
            }

        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    private void Rpc_UpdateUI(int globalPi)
    {
        if(globalPi < 10)
        {
            soundImage.sprite = soundSpite[globalPi];
        }
        else
        {
            soundImage.sprite = soundSpite[11];
        }
    }
    private async Task WaitForAnimationEnd()
{
    // Attend que l'animation finisse
    while (anim.isPlaying)
    {
        await Task.Yield(); // Attend la frame suivante
    }
}
}
