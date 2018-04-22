using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    WaveSpawner spawner;

    [SerializeField]
    BuildSystem buildSystem;

    [SerializeField]
    MessageDisplay messageDisplay;

    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    Camera spawnCamera;

    Coroutine tutorialCoroutine;
    bool skipped = false;
    bool fastText = false;

    void Start()
    {
        tutorialCoroutine = StartCoroutine(RunTutorial());
    }

    void Update()
    {
        if (!skipped && Input.GetKeyDown(KeyCode.S))
        {
            skipped = true;

            StopCoroutine(tutorialCoroutine);
            messageDisplay.DisplayIndefiniteMessage("");

            spawner.StartWaves();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fastText = true;
        }
    }

    IEnumerator RunTutorial()
    {
        yield return null;
        yield return DisplayTimedMessage("Your Majesty. Please allow me to welcome you to our humble gouf course.", 8f);

        yield return DisplayTimedMessage("(Press S to skip the tutorial)", 3f);

        messageDisplay.DisplayIndefiniteMessage("If it so please you, please use your Mouse Wheel to get a better lay of the land.");
        while (!(Input.GetAxis("Mouse ScrollWheel") < 0f)) yield return null;

        yield return DisplayTimedMessage("And what a splendid day it is. The royal treasury glistens brighter every moment.", 6f);

        messageDisplay.DisplayIndefiniteMessage("You of course may survey the course by holding the Left Mouse Button down and moving your Mouse left and right.");
        while (!(Input.GetMouseButton(0) && Mathf.Abs(Input.GetAxis("Mouse X")) > 0f)) yield return null;

        messageDisplay.DisplayIndefiniteMessage("Most excellent. At your leisure, you may strike the gouf ball by holding Space bar. After a regal pause, release the Space bar and let it sail!");
        while (!(Input.GetKeyUp(KeyCode.Space))) yield return null;

        yield return DisplayTimedMessage("Your eminence knows no bounds.", 4f);

        yield return DisplayTimedMessage("... Your Majesty! Troubling news! Scouts have reported roving goblins in the area. Bowing to your infinite wisdom, may I propose we fortify?", 9f);

        messageDisplay.DisplayIndefiniteMessage("When you have comfortably chosen a spot, merely press 1 and I shall draft the blueprint.");
        while (!(Input.GetKeyUp(KeyCode.Alpha1))) yield return null;

        messageDisplay.DisplayIndefiniteMessage("A masterful choice. Press Space to begin the construction, or Escape if perhaps there is an even better location.");
        while (!(Input.GetKeyUp(KeyCode.Space) && buildSystem.isActiveAndEnabled)) yield return null;

        yield return DisplayTimedMessage("May our meager construction raise you to the heavens. Do not trouble yourself about the materials! I will deduct a mere 10 gold from the coffers.", 9f);

        if (spawnCamera)
        {
            mainCamera.enabled = false;
            spawnCamera.enabled = true;
        }

        spawner.StartWaves();

        yield return DisplayTimedMessage("Good heavens! They've made their way to the course! Run for the hills!", 9f);

        if (spawnCamera)
        {
            mainCamera.enabled = true;
            spawnCamera.enabled = false;
        }
    }

    WaitForSeconds DisplayTimedMessage(string text, float duration)
    {
        if (SceneManager.GetActiveScene().name == "Debug" || fastText)
        {
            duration /= 4f;
        }

        messageDisplay.DisplayTimedMessage(text, duration);
        return new WaitForSeconds(duration);
    }
}
