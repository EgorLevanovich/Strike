using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideOpener : MonoBehaviour
{
    public GameObject guide;
    public GameObject _Settings;
    // Start is called before the first frame update
    public void OpenGuide(){
        guide.SetActive(true);
        _Settings.SetActive(false);
    }
    public void OpenSettings(){
        guide.SetActive(false);
        _Settings.SetActive(true);
    }
}
