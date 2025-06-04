//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public Slider HP_Slider;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] private int Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = 100;
        UpdateSlider();
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            UpdateSlider();
            Die();
        }
    }
    public void TakeHealth(int health)
    {
        Health += health;
        if (Health > 100)
            Health = 100;
        UpdateSlider();
    }
    public void Die()
    {
       Scene Currentscene = SceneManager.GetActiveScene();
       SceneManager.LoadScene(Currentscene.name);
    }
    private void Update()
    {
        UpdateSlider();
    }
    private void UpdateSlider()
    {
        if (HP_Slider != null)
        {
            HP_Slider.value = Health;
        }
    }
}
