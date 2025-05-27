using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public PlayerMouvement mouvement;


    private void Update()
    {
        if (!mouvement.isCrouching)
        {
            if (mouvement.isMoving && mouvement.isRunning)
            {
                Run();
            }
            else if(mouvement.isMoving && !mouvement.isRunning)
            {
                Walk();
            }
            else
            {
                Idle();
            }
        }
        else if(mouvement.isCrouching) 
        {
            if (mouvement.isMoving) 
            {
                CrouchWalk();
            }
            else
            {
                Crouching();
            }
        }
        
    }
    public void Idle()
    {
        animator.SetBool("Is_Idle", true);
        animator.SetBool("Is_Walking", false);
        animator.SetBool("Is_Running", false);
        animator.SetBool("IsCrouchWalking", false);
        animator.SetBool("Is_Crouching", false);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Idle"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Idle");
        }
    }

    public void Walk()
    {
        animator.SetBool("Is_Walking", true);
        animator.SetBool("Is_Idle", false);
        animator.SetBool("Is_Running", false);
        animator.SetBool("IsCrouchWalking", false);
        animator.SetBool("Is_Crouching", false);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Walk"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Walk");
        }
       // animator.SetTrigger("Walk");
    }

    public void Run()
    {
        animator.SetBool("Is_Running", true);
        animator.SetBool("Is_Walking", false);
        animator.SetBool("Is_Idle", false);
        animator.SetBool("IsCrouchWalking", false);
        animator.SetBool("Is_Crouching", false);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Run"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Run");
        }
        //animator.SetTrigger("Run");
    }

    public void Crouching()
    {
        animator.SetBool("Is_Crouching", true);
        animator.SetBool("Is_Running", false);
        animator.SetBool("IsCrouchWalking", false);
        animator.SetBool("Is_Walking", false);
        animator.SetBool("Is_Idle", false);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Crouch"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Crouch");
        }
        //animator.SetTrigger("Crouch"); 
    }

    public void CrouchWalk()
    {
        animator.SetBool("IsCrouchWalking", true);
        animator.SetBool("Is_Crouching", false);
        animator.SetBool("Is_Running", false);
        animator.SetBool("Is_Walking", false);
        animator.SetBool("Is_Idle", false);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("CrouchWalk"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("CrouchWalk");
        }
        // animator.SetTrigger("CrouchWalk");
    }

    public void Pick()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index

        // Vérifie si le nom de l'état actuel est différent de "Idle"
        if (!stateInfo.IsName("Pick"))
        {

            //animator.ResetTrigger("Run");  // Optionnel : réinitialise les autres triggers
            animator.SetTrigger("Pick");
        }
    }
}
