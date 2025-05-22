using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;

public class Sas_Door : NetworkBehaviour
{
    public Animation anim;
    public Transform Spawn;

    [SerializeField] Transform Button1;
    [SerializeField] BoxCollider Collider1;
    [SerializeField] Transform Button2;
    [SerializeField] BoxCollider Collider2;


    public float Delay;




    [Rpc(RpcSources.All, RpcTargets.All)]
    public async void Rpc_OpeningSas(PlayerMouvement mouvement)
    {
       // if (HasStateAuthority)
        {


            if (!anim.isPlaying)
            {
                anim.Play();
                anim[anim.clip.name].speed = 1;              // Joue l’animation en sens inverse
                anim[anim.clip.name].time = 0;
            }

            await WaitForAnimationEnd();

            if (!anim.isPlaying)
            {
                anim.Play();
                anim[anim.clip.name].speed = -1;              // Joue l’animation en sens inverse
                anim[anim.clip.name].time = anim[anim.clip.name].length;
            }

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

