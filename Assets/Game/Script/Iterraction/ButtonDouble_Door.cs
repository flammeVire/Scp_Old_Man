using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class ButtonDouble_Door : NetworkBehaviour
{
    public NetworkBool ButtonActive;
    public Double_Door Double_Door;
    public void ActiveButton(bool state)
    {
        ButtonActive = state;
        TryToOpen();
    }

    void TryToOpen()
    {
        Double_Door.Rpc_ManageOpening(true);
    }
}
