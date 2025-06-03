using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private int Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = 100;
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }
    public void TakeHealth(int health)
    {
        Health += health;
        if(Health > 100)
            Health = 100;
    }
    public void Die()
    {
        //UI¨Æ¥ó
    }
}
