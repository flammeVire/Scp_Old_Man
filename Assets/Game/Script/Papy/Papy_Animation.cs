using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Papy_Animation : MonoBehaviour
{
    public Animator animator;



    public void Idle()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Idle"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Idle");
        }
        // animator.SetTrigger("Walk");
    }

    public void Walk()
    {
        Debug.Log("Walk");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Walk"))
        {
            Debug.Log("Walk");

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Walk");
        }
        // animator.SetTrigger("Walk");
    }

    public void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Attack"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Attack");
        }
        // animator.SetTrigger("Walk");
    }

    public void Teleport()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Teleport"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Teleport");
        }
        // animator.SetTrigger("Walk");
        GoOutFloor();
    }

    public void GoOutFloor() 
    {

    }
}
