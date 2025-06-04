using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth Instance;
    [SerializeField] private int Enemyhealth = 100;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void TakeDamage(int damage)
    {
        Enemyhealth -= damage;
        if (Enemyhealth <= 0)
            Die();
    }
    private void Die()
    {
        Destroy(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
