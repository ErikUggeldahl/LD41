using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfShot : MonoBehaviour
{
    [SerializeField]
    RectTransform shotPowerMeter;

    [SerializeField]
    Rigidbody ball;

    float shotPower = 0.0f;
    bool increasing = true;

    const float SHOT_POWER_INCREMENT = 0.8f;
    const float ONE_HUNDRED_METRES_FORCE = 31f;

    enum State
    {
        Waiting,
        Preparing,
        Shooting,
    }
    State state = State.Waiting;

    void Start()
    {
    }

    void Update()
    {
        if (state == State.Waiting && Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Preparing;
        }
        if (state == State.Preparing)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                state = State.Shooting;
                Shoot();
            }
            else
            {
                Prepare();
            }
        }
    }

    void Prepare()
    {
        var increment = SHOT_POWER_INCREMENT * Time.deltaTime * (increasing ? 1.0f : -1.0f);
        shotPower = Mathf.Clamp01(shotPower + increment);

        if (Mathf.Approximately(shotPower, 1.0f) || Mathf.Approximately(shotPower, 0.0f))
        {
            increasing = !increasing;
        }

        shotPowerMeter.localScale = new Vector3(1.0f, shotPower, 1.0f);
    }

    void Shoot()
    {
        ball.isKinematic = false;
        ball.AddForce(Vector3.Slerp(Vector3.forward, Vector3.up, 0.5f) * ONE_HUNDRED_METRES_FORCE * shotPower, ForceMode.VelocityChange);
    }
}
