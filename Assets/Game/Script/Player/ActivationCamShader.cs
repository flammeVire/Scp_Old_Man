using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationCamShader : MonoBehaviour
{
    public PlayerUI pui;
    // Start is called before the first frame update
    public void ToggleGrain()
    {
        if (pui.mouvement.cam.GetComponent<CameraGrainEffect>().isActiveAndEnabled)
        {
            pui.mouvement.cam.GetComponent<CameraGrainEffect>().enabled = false;
        }
        else
        {
            pui.mouvement.cam.GetComponent<CameraGrainEffect>().enabled = true;
        }
    }
}
