using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOADER1 : MonoBehaviour
{
    public GameObject platformSkinLoader;
    public GameObject _background;
    public GameObject _ball;
   

   public void PlatformLoader(){
    platformSkinLoader.SetActive(true);
    _background.SetActive(false);
    _ball.SetActive(false);
   }

   public void BackgroundLoader(){
    platformSkinLoader.SetActive(false);
    _background.SetActive(true);
    _ball.SetActive(false);
   }

   public void BallLoader(){
    platformSkinLoader.SetActive(false);
    _background.SetActive(false);
    _ball.SetActive(true);
   }
    
    
}
