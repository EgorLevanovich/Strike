using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallCountDisplay : MonoBehaviour
{
    public Text countText;

    private void Update()
    {
        if (countText != null && NewBehaviourScript.Instance != null)
        {
            countText.text = "" + NewBehaviourScript.Instance.GetSessionCount();
        }
    }
}
