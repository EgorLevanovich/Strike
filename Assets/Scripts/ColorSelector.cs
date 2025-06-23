using Cysharp.Threading.Tasks;
using DefaultNamespace;
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
        if (button != null)
        {
            originalColor = buttonImage.color;
        }

        // Создаем объект для рамки
        CreateBorder();

        // Проверяем, является ли этот цвет выбранным
        if (buttonImage != null && IsThisColorSelected())
        {
            SetAsSelected();
        }
    }

    private void CreateBorder()
    {
        // Создаем новый GameObject для галочки
        borderObject = new GameObject("ColorCheckmark");
        borderObject.transform.SetParent(transform);
        borderObject.transform.SetAsFirstSibling();

        // Настраиваем RectTransform для галочки
        RectTransform checkmarkRect = borderObject.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0.5f, 0.5f);
        checkmarkRect.anchorMax = new Vector2(0.5f, 0.5f);
        checkmarkRect.anchoredPosition = Vector2.zero;
        checkmarkRect.sizeDelta = new Vector2(200, 200); // Размер галочки увеличен в 10 раз
        checkmarkRect.pivot = new Vector2(0.5f, 0.5f);

        // Добавляем компонент Image для галочки
        borderImage = borderObject.AddComponent<Image>();
        borderImage.color = new Color(0f, 0f, 0f, 1f); // Черный цвет
        borderImage.raycastTarget = false; // Отключаем взаимодействие с галочкой
        
        // Устанавливаем стандартный спрайт галочки Unity
        borderImage.sprite = Resources.Load<Sprite>("UI/Checkmark");
        
        // Если стандартный спрайт не найден, создаем простую галочку
        if (borderImage.sprite == null)
        {
            borderImage.sprite = CreateCheckmarkSprite();
        }

        // Скрываем галочку по умолчанию
        borderObject.SetActive(false);
    }

    private Sprite CreateCheckmarkSprite()
    {
        // Создаем простую текстуру галочки
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        
        // Заполняем прозрачным цветом
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.clear;
        }
        
        // Рисуем галочку (белые пиксели)
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Левая часть галочки (вертикальная линия)
                if (x >= 8 && x <= 12 && y >= 8 && y <= 24)
                {
                    pixels[y * size + x] = Color.black;
                }
                // Правая часть галочки (горизонтальная линия)
                else if (x >= 12 && x <= 24 && y >= 8 && y <= 12)
                {
                    pixels[y * size + x] = Color.black;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        // Создаем спрайт из текстуры
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
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
            
            //OnSelected(selectedColor).Forget();
            
            SetAsSelected();
        }
        ActivateAllInGroup();

        async UniTask OnSelected(Color color)
        {
            var requester = new WebRequester();
            var result = await requester.GetDataAsync(color);
            Analytics.Instance.EnemyColorSelected(result);
        }
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