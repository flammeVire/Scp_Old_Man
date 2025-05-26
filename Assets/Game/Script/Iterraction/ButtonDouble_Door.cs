using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class ButtonDouble_Door : NetworkBehaviour
{
    public NetworkBool ButtonActive;
    public Double_Door Double_Door;
    public DoorsSound sound;

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_ActiveButton(bool state)
    {
        Debug.Log(this.gameObject.name + state);
        ButtonActive = state;
        TryToOpen();
    }
    void TryToOpen()
    {
        sound.Rpc_Good();
        Double_Door.CheckAllButton();
    }
}
