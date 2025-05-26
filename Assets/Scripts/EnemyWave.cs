using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "Enemy Wave")]
public class EnemyWave : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // ������� ������
    public float spawnDelay = 1f;     // �������� ����� �������
    public int enemiesPerSpawn = 1;   // ������� ������ �� ���
}
