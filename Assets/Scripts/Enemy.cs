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
    float walkForce = 0.5f;
    float maxSpeed = 2f;
    float maxSpeedSq;

    int totalHealth = 100;
    int health;
    public int Health { get { return health; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        maxSpeedSq = maxSpeed * maxSpeed;
        health = totalHealth;
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
        if (rb.velocity.sqrMagnitude < maxSpeedSq)
        {
            rb.AddForce(transform.forward * walkForce, ForceMode.VelocityChange);
        }
    }

    public void MakeLarge()
    {
        totalHealth = 400;
        health = totalHealth;
        transform.localScale = Vector3.one * 2f;
        goldGiven = 3;
        walkForce = 0.8f;
        maxSpeed = 1.8f;
        maxSpeedSq = maxSpeed * maxSpeed;
    }

    public void MakeGiant()
    {
        totalHealth = 800;
        health = totalHealth;
        transform.localScale = Vector3.one * 4f;
        goldGiven = 10;
        walkForce = 0.7f;
        maxSpeed = 1.5f;
        maxSpeedSq = maxSpeed * maxSpeed;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthbar.localScale = new Vector3((float)health / totalHealth, 1f, 1f);

        if (health <= 0)
        {
            BuildSystem.GainGold(goldGiven);

            Destroy(gameObject);
        }
    }
}
