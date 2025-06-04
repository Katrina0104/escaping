using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int hp = 100;  // Starting health

    void OnEnable()
    {
        DelayedDoubleCheckLogger.PlayerLostHP += OnPlayerLoseHP;
    }

    void OnDisable()
    {
        DelayedDoubleCheckLogger.PlayerLostHP -= OnPlayerLoseHP;
    }

    void OnPlayerLoseHP()
    {
        int damageAmount = 10; // You can adjust this or pass it as a parameter later

        hp -= damageAmount;
        Debug.Log($"Player took {damageAmount} damage! HP now: {hp}");

        if (hp <= 0)
        {
            Debug.Log("Player is dead!");
            // You can add death logic here
        }
    }
}
