using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorsSetup : MonoBehaviour
{
    public GameObject colors;
    public GameObject _colorSetup;
    public GameObject Settings;
    

    void Start(){
        _colorSetup.SetActive(true);
    }
    public void ColorsSelect(){
        colors.SetActive(true);
        Settings.SetActive(false);
    }

    public void SettingsSelect(){
        colors.SetActive(false);
        Settings.SetActive(true);
    }
}
