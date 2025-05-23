using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    [HideInInspector]public PlayerMouvement mouvement;
    [HideInInspector]public PlayerInterrestPoint PlayerPI;
    [Header("Sprite")]
    public Sprite[]SoundSprite;
    public Sprite TalkieOnSprite;
    public Sprite TalkieOffSprite;
    [Header("Image")]
    public Image StaminaImage;
    public Image SoundsImage;
    public Image TalkieImage;
    public Image FlashImage;
    public TextMeshProUGUI FlashText;
    public Image Card1;
    public Image Card2;
    public Image Card3;

    private void Update()
    {
        UpdateStamina();
        UpdateSound();
        UpdateTalkie();
    }


    void UpdateStamina()
    {
        // StaminaImage.fillAmount = mouvement.stamina / 100;

        float percentOfStam = mouvement.stamina / 100;
        float PercentOfCorrosion = 1 - percentOfStam;
        StaminaImage.fillAmount = PercentOfCorrosion;
    }
    void UpdateSound()
    {
        SoundsImage.sprite = SoundSprite[PlayerPI.CurrentPI];
    }
    void UpdateTalkie()
    {
        if (mouvement.isTalking) 
        {
            TalkieImage.sprite = TalkieOnSprite;
        }
        else
        {
            TalkieImage.sprite= TalkieOffSprite;
        }
    }

    

    
}
