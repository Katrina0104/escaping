using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth Instance;
    public static float EnemyDeath = 0;

    public Slider HP_Slider;
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
        {
            Enemyhealth = 0;
            UpdateSlider();
            Die();
        }
    }
    private void Die()
    {
        Destroy(gameObject);
        EnemyDeath++;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateSlider();
    }
    private void UpdateSlider()
    {
        if (HP_Slider != null)
        {
            HP_Slider.value = Enemyhealth;
        }
    }
}
