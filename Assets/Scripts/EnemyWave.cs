using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Enemy Wave")]
public class EnemyWave : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Префабы врагов
    public float spawnDelay = 1f;     // Задержка между спавном
    public int enemiesPerSpawn = 1;   // Сколько врагов за раз
}
