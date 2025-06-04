using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

    public Slider HP_Slider;
    [SerializeField] private int Health;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Health = 100;
        UpdateSlider();
    }

    void OnEnable()
    {
        DelayedDoubleCheckLogger.PlayerLostHP += OnPlayerLostHP;
    }

    void OnDisable()
    {
        DelayedDoubleCheckLogger.PlayerLostHP -= OnPlayerLostHP;
    }

    // This method responds to the event
    void OnPlayerLostHP()
    {
        TakeDamage(10);  // Deal 10 damage on event
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
        else
        {
            UpdateSlider();
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

    public bool IsDead()
    {
        return Health <= 0;
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
