using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    Transform healthbar;

    const int TOTAL_HEALTH = 100;
    int health = TOTAL_HEALTH;

    void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.localScale = new Vector3((float)health / TOTAL_HEALTH, 1f, 1f);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
