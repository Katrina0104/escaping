using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemyPrefab; // 敵人預製物件
    public Transform[] spawnPoints; // 生成點陣列
    public float spawnInterval = 3f; // 生成間隔時間

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval); // 每隔一段時間生成敵人
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length); // 隨機選擇生成點
        Instantiate(enemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }

}
