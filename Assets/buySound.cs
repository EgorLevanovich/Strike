using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buySound : MonoBehaviour
{
   public AudioSource buySoundButton;
   public void PlayBuySound()
   {
    buySoundButton.Play();
   }
}
