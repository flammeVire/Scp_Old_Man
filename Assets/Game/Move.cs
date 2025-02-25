using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    Rigidbody body;
    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
       body.velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * 3;
    }
}
