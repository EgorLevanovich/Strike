using UnityEngine;
using UnityEngine.UI;

public class PointsResetter : MonoBehaviour
{
    public GameObject pointsObject; // Любой объект с компонентом очков
    public string pointsComponentName = "Money"; // Имя компонента
    public string pointsFieldName = "Coins";     // Имя поля/свойства
    public Button resetPointsButton; // Кнопка для сброса очков (опционально)

    void Start()
    {
        if (resetPointsButton != null)
            resetPointsButton.onClick.AddListener(ResetPoints);
    }

    // Вызывай этот метод для аннулирования очков
    public void ResetPoints()
    {
        if (pointsObject == null) return;
        var comp = pointsObject.GetComponent(pointsComponentName);
        if (comp == null) return;
        // Сначала ищем поле
        var field = comp.GetType().GetField(pointsFieldName);
        if (field != null)
        {
            field.SetValue(comp, 0);
            // Если это Money, обновляем UI
            if (comp is Money moneyComp) moneyComp.UpdateCoinsText();
            return;
        }
        // Если поле не найдено, ищем свойство с публичным сеттером
        var prop = comp.GetType().GetProperty(pointsFieldName);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(comp, 0);
            if (comp is Money moneyComp) moneyComp.UpdateCoinsText();
        }
    }
} 