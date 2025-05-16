using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using FMODUnity;

public class DoorsSound : NetworkBehaviour
{
    [Header("SFX")]
    public StudioEventEmitter Good;
    public StudioEventEmitter Bad;
    public StudioEventEmitter Open;
    public StudioEventEmitter Close;

    // Start is called before the first frame update

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Good()
    {
        Good.Stop();
        Good.Play();

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Bad()
    {
        Bad.Stop();
        Bad.Play();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Open()
    {
        Open.Stop();
        Open.Play();

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Close()
    {
        Close.Stop();
        Close.Play();

    }

}
