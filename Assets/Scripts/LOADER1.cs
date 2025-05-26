using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOADER1 : MonoBehaviour
{
    public GameObject _platform;
    public GameObject _background;
    public GameObject _ball;
   

   public void PlatformLoader(){
    _platform.SetActive(true);
    _background.SetActive(false);
    _ball.SetActive(false);
   }

   public void BackgroundLoader(){
    _platform.SetActive(false);
    _background.SetActive(true);
    _ball.SetActive(false);
   }

   public void BallLoader(){
    _platform.SetActive(false);
    _background.SetActive(false);
    _ball.SetActive(true);
   }
    
    
}
