public interface IPointsHolder
{
    int Points { get; set; }
    bool SpendPoints(int amount);
} 