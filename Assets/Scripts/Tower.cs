using System.Collections;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    Transform towerHead;

    [SerializeField]
    Transform cannon;

    [SerializeField]
    Transform cannonBallSpawnLocation;

    [SerializeField]
    GameObject cannonBallPrefab;

    [SerializeField]
    float targetingDistance = 20f;

    enum State
    {
        Targeting,
        Firing,
    }
    State state = State.Targeting;

    Enemy target;
    const float TARGETING_RATE_LIMIT = 0.5f;
    public float TargetingDistance { get { return targetingDistance; } }

    const float SHOT_RATE = 2f;
    float timeSinceLastShot = 0f;

    const int DAMAGE = 50;

	void Start()
	{
        StartCoroutine(FindTarget());
    }
	
	void Update()
	{
        if (state == State.Firing)
        {
            Shoot();
        }
	}

    void Shoot()
    {
        if (target == null || Vector3.Distance(target.transform.position, transform.position) > targetingDistance)
        {
            target = null;
            state = State.Targeting;
            StartCoroutine(FindTarget());
            return;
        }

        // https://answers.unity.com/questions/36255/lookat-to-only-rotate-on-y-axis-how.html
        var toTarget = target.transform.position - transform.position;
        toTarget.y = 0;
        towerHead.rotation = Quaternion.LookRotation(toTarget);

        cannon.LookAt(target.transform.position);

        timeSinceLastShot += Time.deltaTime;

        if (timeSinceLastShot >= SHOT_RATE)
        {
            timeSinceLastShot -= SHOT_RATE;

            var cannonBall = Instantiate(cannonBallPrefab, cannonBallSpawnLocation.position, Quaternion.identity);
            var cannonBallScript = cannonBall.GetComponent<CannonBall>();
            cannonBallScript.Target = target;
            cannonBallScript.Damage = DAMAGE;
            
            if (target.Health - DAMAGE <= 0)
            {
                target = null;
                state = State.Targeting;
                StartCoroutine(FindTarget());
            }
        }
    }

    IEnumerator FindTarget()
    {
        while (state == State.Targeting)
        {
            var enemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < targetingDistance)
                {
                    state = State.Firing;
                    target = enemy;
                    break;
                }
            }

            yield return new WaitForSeconds(TARGETING_RATE_LIMIT);
        }
    }
}
