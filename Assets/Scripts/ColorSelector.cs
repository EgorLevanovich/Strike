using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    // Вызывается при нажатии на кнопку цвета
    public void SelectColor()
    {
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Color selectedColor = buttonImage.color;
            PlayerPrefs.SetFloat("EnemyColorR", selectedColor.r);
            PlayerPrefs.SetFloat("EnemyColorG", selectedColor.g);
            PlayerPrefs.SetFloat("EnemyColorB", selectedColor.b);
            PlayerPrefs.SetFloat("EnemyColorA", selectedColor.a);
            PlayerPrefs.Save();
            Debug.Log($"[ColorSelector] Selected color: {selectedColor}");
        }
        ActivateAllInGroup();
    }

    private void ActivateAllInGroup()
    {
        Transform parent = transform.parent;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                Button btn = child.GetComponent<Button>();
                if (btn != null)
                    btn.interactable = true;
            }
        }
    }
} 