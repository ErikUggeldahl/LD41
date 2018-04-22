using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public Enemy Target { get; set; }
    public int Damage { get;  set; }

    Vector3 finalDestination;

    const float SPEED = 50f;
    const float HIT_DISTANCE = 0.5f;

    private void Start()
    {
        if (Target != null)
        {
            var targetPosition = Target.transform.position + Vector3.up;
            finalDestination = targetPosition;
            transform.LookAt(targetPosition);
        }
    }

    void Update()
	{
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += transform.forward * SPEED * Time.deltaTime;

        if (Vector3.Distance(transform.position, finalDestination) < HIT_DISTANCE)
        {
            Target.TakeDamage(Damage);
            Destroy(gameObject);
        }
	}
}
