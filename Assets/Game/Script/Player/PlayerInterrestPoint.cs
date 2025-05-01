using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class PlayerInterrestPoint : NetworkBehaviour
{
    [SerializeField] PlayerMouvement mouvement;
    [Networked] public int CurrentPI {  get; private set; }
    [Networked] public int TotalPI {  get; private set; }
    public float Delay;

    [SerializeField] int[] Increaser;
   
    private void Start()
    {
        StartCoroutine(increaseTotalPI());

    }
    private void FixedUpdate()
    {
        IncreaseCurrentPI();
    }
    void IncreaseCurrentPI()
    {
        CurrentPI = HowManyInput();
    }
    
    IEnumerator increaseTotalPI()
    {
        yield return new WaitForSeconds(Delay);
        TotalPI += CurrentPI;
        StartCoroutine(increaseTotalPI());
    }

    int HowManyInput()
    {
        int input = 0;
        if (mouvement.isCrouching)
        {
            input += Increaser[0];
        }
        if (mouvement.isJumping) 
        {
            input += Increaser[1];
        }
        if (mouvement.isMoving) 
        {
            input += Increaser[2];
        }
        if (mouvement.isRunning) 
        {
            input += Increaser[3];
        }
        if (mouvement.isTalking)
        {
            input += Increaser[4];
        }
        input++;
        return input;
    }


}
