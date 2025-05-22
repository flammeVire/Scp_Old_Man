using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorteCoupeFeu : MonoBehaviour
{

    public Animation anim;

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_Close()
    {
        if (!anim.isPlaying)
        {
            anim.Play();
            anim[anim.clip.name].speed = 1;              // Joue l’animation en sens inverse
            anim[anim.clip.name].time = 0;
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void Rpc_Open()
    {
        if (!anim.isPlaying)
        {
            anim.Play();
            anim[anim.clip.name].speed = -1;              // Joue l’animation en sens inverse
            anim[anim.clip.name].time = anim[anim.clip.name].length;
        }
    }
}
