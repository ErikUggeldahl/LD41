using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public PathingNetwork PathingNetwork;

    [SerializeField]
    Transform healthbar;

    [SerializeField]
    public BuildSystem BuildSystem;

    [SerializeField]
    int goldGiven = 1;

    uint nodeIndex = 1;
    const float NODE_RADIUS = 3f;

    Rigidbody rb;
    const float WALK_FORCE = 0.5f;
    const float MAX_SPEED = 2f;
    const float MAX_SPEED_SQ = MAX_SPEED * MAX_SPEED;

    const int TOTAL_HEALTH = 100;
    int health = TOTAL_HEALTH;
    public int Health { get { return health; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distanceToNode = Vector3.Distance(PathingNetwork.Nodes[nodeIndex].position, transform.position);

        if (distanceToNode < NODE_RADIUS)
        {
            nodeIndex++;
            if (nodeIndex == PathingNetwork.Nodes.Length)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        transform.LookAt(PathingNetwork.Nodes[nodeIndex]);
        if (rb.velocity.sqrMagnitude < MAX_SPEED_SQ)
        {
            rb.AddForce(transform.forward * WALK_FORCE, ForceMode.VelocityChange);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.localScale = new Vector3((float)health / TOTAL_HEALTH, 1f, 1f);

        if (health <= 0)
        {
            BuildSystem.GainGold(goldGiven);

            Destroy(gameObject);
        }
    }
}
