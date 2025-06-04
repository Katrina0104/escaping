using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth Instance;
    [SerializeField] private int Enemyhealth = 100;
    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void TakeDamage(int damage)
    {
        Enemyhealth -= damage;
        Debug.Log(Enemyhealth);
        if (Enemyhealth <= 0)
            Die();
    }
    private void Die()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
