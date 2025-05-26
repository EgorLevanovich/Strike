using UnityEngine;

public class SimplePointsHolder : MonoBehaviour, IPointsHolder
{
    public int points = 0;
    public int Points { get => points; set => points = value; }

    public bool SpendPoints(int amount)
    {
        if (points >= amount)
        {
            points -= amount;
            return true;
        }
        return false;
    }
} 