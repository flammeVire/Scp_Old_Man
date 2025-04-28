using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    public PlayerMouvement mouvement;
    public Image StaminaImage;

    private void Update()
    {
        UpdateStamina();
    }


    void UpdateStamina()
    {
        StaminaImage.fillAmount = mouvement.stamina / 100;
    }
}
