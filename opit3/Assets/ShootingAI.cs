using UnityEngine;

public class ShootingAi : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // You can add death effects, animations, etc. here
        Debug.Log(gameObject.name + " died.");
        Destroy(gameObject);
    }
}
