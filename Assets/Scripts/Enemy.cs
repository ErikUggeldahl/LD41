using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    PathingNetwork pathingNetwork;

    [SerializeField]
    Transform healthbar;

    uint nodeIndex = 0;
    const float NODE_RADIUS = 3f;

    Rigidbody rb;
    const float WALK_FORCE = 0.5f;
    const float MAX_SPEED = 2f;
    const float MAX_SPEED_SQ = MAX_SPEED * MAX_SPEED;

    const int TOTAL_HEALTH = 100;
    int health = TOTAL_HEALTH;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distanceToNode = Vector3.Distance(pathingNetwork.nodes[nodeIndex].position, transform.position);

        if (distanceToNode < NODE_RADIUS)
        {
            nodeIndex++;
            if (nodeIndex == pathingNetwork.nodes.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        transform.LookAt(pathingNetwork.nodes[nodeIndex]);
        if (rb.velocity.sqrMagnitude < MAX_SPEED_SQ)
        {
            rb.AddForce(transform.forward * WALK_FORCE, ForceMode.VelocityChange);
        }
    }

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
