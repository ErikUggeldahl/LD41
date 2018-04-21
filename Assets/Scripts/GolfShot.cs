using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfShot : MonoBehaviour
{
    [SerializeField]
    RectTransform shotPowerMeter;

    [SerializeField]
    Rigidbody ball;

    [SerializeField]
    Transform cameraTransform;

    [SerializeField]
    Transform cameraTarget;

    [SerializeField]
    LineRenderer trajectoryLine;

    const float SHOT_POWER_INCREMENT = 0.8f;
    const float ONE_HUNDRED_METRES_FORCE = 31f;

    float shotPower = 0f;
    bool increasing = true;

    float forwardAngle = 0f;

    Camera cameraComponent;

    const float CAMERA_X_OFFSET = -0.05f;
    readonly Vector3 CAMERA_ZOOM_MIN = new Vector3(CAMERA_X_OFFSET, 0.5f, -1f);
    readonly Vector3 CAMERA_ZOOM_MAX = new Vector3(CAMERA_X_OFFSET, 0.5f, -1f) * 20f;
    const float CAMERA_ZOOM_VELOCITY_FACTOR = 0.25f;
    const float CAMERA_ZOOM_LERP_FACTOR = 3f;

    const float CAMERA_FOV_MIN = 60f;
    const float CAMERA_FOV_MAX = 75f;

    float zoom = 0f;

    const int TRAJECTORY_POSITION_COUNT = 180;
    Vector3[] trajectoryPositions;
    GradientAlphaKey[] trajectoryAlphas;
    const float TRAJECTORY_FADE_DURATION = 1f;

    enum State
    {
        Waiting,
        Preparing,
        Shooting,
    }
    State state = State.Waiting;

    void Start()
    {
        cameraComponent = cameraTransform.GetComponent<Camera>();

        trajectoryPositions = new Vector3[TRAJECTORY_POSITION_COUNT];
        trajectoryLine.positionCount = trajectoryPositions.Length;
        trajectoryAlphas = trajectoryLine.colorGradient.alphaKeys;
    }

    void Update()
    {
        RotateCamera();
        ZoomCamera();

        if (state == State.Waiting && Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Preparing;
            ResetTrajectory();
        }
        else if (state == State.Preparing)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                state = State.Shooting;
                Shoot();
                StartCoroutine(CheckReset());
                StartCoroutine(FadeTrajectoryLine());
            }
            else
            {
                Prepare();
                SimulateTrajectory();
            }
        }
        else if (state == State.Shooting)
        {
            TrackBall();
        }
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(0))
        {
            var rotation = Input.GetAxis("Mouse X");
            forwardAngle += rotation;
            cameraTarget.rotation = Quaternion.Euler(0f, forwardAngle, 0f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ZoomCamera()
    {
        zoom = Mathf.Clamp01(zoom + Input.GetAxis("Mouse ScrollWheel"));

        var ballVelocityRatio = ball.velocity.magnitude / ONE_HUNDRED_METRES_FORCE;

        var velocityZoom = Mathf.Lerp(0f, CAMERA_ZOOM_VELOCITY_FACTOR, ballVelocityRatio);
        var idealPosition = Vector3.Lerp(CAMERA_ZOOM_MIN, CAMERA_ZOOM_MAX, zoom + velocityZoom);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, idealPosition, Time.deltaTime * CAMERA_ZOOM_LERP_FACTOR);

        var idealFOV = Mathf.Lerp(CAMERA_FOV_MIN, CAMERA_FOV_MAX, ballVelocityRatio);
        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, idealFOV, Time.deltaTime);
    }

    void Prepare()
    {
        var increment = SHOT_POWER_INCREMENT * Time.deltaTime * (increasing ? 1f : -1f);
        shotPower = Mathf.Clamp01(shotPower + increment);

        if (Mathf.Approximately(shotPower, 1f) || Mathf.Approximately(shotPower, 0f))
        {
            increasing = !increasing;
        }

        shotPowerMeter.localScale = new Vector3(1f, shotPower, 1f);
    }
    
    void ResetTrajectory()
    {
        trajectoryLine.colorGradient = GradientWithAlpha(trajectoryLine.colorGradient.colorKeys, trajectoryAlphas, 1f);
        SimulateTrajectory();
    }

    void SimulateTrajectory()
    {
        var forward = Quaternion.AngleAxis(forwardAngle, Vector3.up) * Vector3.forward;
        var initialVector = Vector3.Slerp(forward, Vector3.up, 0.5f);
        var velocity = initialVector * ONE_HUNDRED_METRES_FORCE * shotPower;

        trajectoryPositions[0] = ball.position;

        for (var i = 1; i < trajectoryPositions.Length; i++)
        {
            velocity += Physics.gravity * Time.fixedDeltaTime;
            trajectoryPositions[i] = trajectoryPositions[i - 1] + velocity * Time.fixedDeltaTime;
        }

        trajectoryLine.SetPositions(trajectoryPositions);
    }

    void Shoot()
    {
        var forward = Quaternion.AngleAxis(forwardAngle, Vector3.up) * Vector3.forward;

        ball.isKinematic = false;
        ball.AddForce(Vector3.Slerp(forward, Vector3.up, 0.5f) * ONE_HUNDRED_METRES_FORCE * shotPower, ForceMode.VelocityChange);

        shotPower = 0;
    }

    void TrackBall()
    {
        cameraTarget.position = ball.position;
    }

    IEnumerator CheckReset()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (ball.velocity.sqrMagnitude < 0.1f)
            {
                state = State.Waiting;
                ball.isKinematic = true;
                break;
            }
        }
    }

    IEnumerator FadeTrajectoryLine()
    {
        var alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= TRAJECTORY_FADE_DURATION * Time.deltaTime;
            trajectoryLine.colorGradient = GradientWithAlpha(trajectoryLine.colorGradient.colorKeys, trajectoryAlphas, alpha);

            yield return null;
        }
    }

    Gradient GradientWithAlpha(GradientColorKey[] originalColors, GradientAlphaKey[] originalAlphas, float alpha)
    {
        var alphas = new GradientAlphaKey[originalAlphas.Length];
        for (var i = 0; i < alphas.Length; i++)
        {
            alphas[i].time = originalAlphas[i].time;
            alphas[i].alpha = originalAlphas[i].alpha * alpha;
        }
        var gradient = new Gradient();
        gradient.SetKeys(originalColors, alphas);
        return gradient;
    }
}
