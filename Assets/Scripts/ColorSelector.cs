using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{
    private static ColorSelector currentlySelected;
    private Image buttonImage;
    private Button button;
    private Color originalColor;
    private GameObject borderObject;
    private Image borderImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        originalColor = buttonImage.color;

        // Создаем объект для рамки
        CreateBorder();

        // Проверяем, является ли этот цвет выбранным
        if (IsThisColorSelected())
        {
            SetAsSelected();
        }
    }

    private void CreateBorder()
    {
        // Создаем новый GameObject для рамки
        borderObject = new GameObject("ColorBorder");
        borderObject.transform.SetParent(transform);
        borderObject.transform.SetAsFirstSibling();

        // Настраиваем RectTransform для рамки
        RectTransform borderRect = borderObject.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-5, -5); // Отступ от краев
        borderRect.offsetMax = new Vector2(5, 5);

        // Добавляем компонент Image для рамки
        borderImage = borderObject.AddComponent<Image>();
        borderImage.color = new Color(0f, 0f, 0f, 0.8f); // Черный цвет с прозрачностью
        borderImage.raycastTarget = false; // Отключаем взаимодействие с рамкой

        // Скрываем рамку по умолчанию
        borderObject.SetActive(false);
    }

    private bool IsThisColorSelected()
    {
        float r = PlayerPrefs.GetFloat("EnemyColorR", 1f);
        float g = PlayerPrefs.GetFloat("EnemyColorG", 1f);
        float b = PlayerPrefs.GetFloat("EnemyColorB", 1f);
        float a = PlayerPrefs.GetFloat("EnemyColorA", 1f);

        Color savedColor = new Color(r, g, b, a);
        return ColorApproximatelyEquals(buttonImage.color, savedColor);
    }

    private bool ColorApproximatelyEquals(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b) &&
               Mathf.Approximately(a.a, b.a);
    }

    private void SetAsSelected()
    {
        if (currentlySelected != null)
        {
            currentlySelected.SetAsUnselected();
        }
        currentlySelected = this;
        borderObject.SetActive(true);
    }

    private void SetAsUnselected()
    {
        borderObject.SetActive(false);
    }

    // Вызывается при нажатии на кнопку цвета
    public void SelectColor()
    {
        if (buttonImage != null)
        {
            Color selectedColor = originalColor;
            PlayerPrefs.SetFloat("EnemyColorR", selectedColor.r);
            PlayerPrefs.SetFloat("EnemyColorG", selectedColor.g);
            PlayerPrefs.SetFloat("EnemyColorB", selectedColor.b);
            PlayerPrefs.SetFloat("EnemyColorA", selectedColor.a);
            PlayerPrefs.Save();
            Debug.Log($"[ColorSelector] Selected color: {selectedColor}");
            
            SetAsSelected();
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