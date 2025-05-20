using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FMODUnity;

public class PlayerSound : NetworkBehaviour
{
    [Header("SFX")]
    public StudioEventEmitter walk;
    public StudioEventEmitter crouch;
    public StudioEventEmitter jump;
    public StudioEventEmitter run;

    // Start is called before the first frame update

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Walk(bool isWalking)
    {
        if (walk != null)
        {
            if (isWalking == true)
            {
                walk.Play();

            }
            else 
            {
                walk.Stop();

            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Run(bool isRunning)
    {
        if (run != null)
        {
            if (isRunning == true)
            {
                run.Play();

            }
            else
            {
                run.Stop();

            }
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_crouch()
    {
        if (crouch != null)
        {
            crouch.Stop();
            crouch.Play();
        }
    }
}
