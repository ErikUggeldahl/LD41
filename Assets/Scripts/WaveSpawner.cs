using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField]
    BuildSystem buildSystem;

    [SerializeField]
    PathingNetwork pathingNetwork;

    [SerializeField]
    PathingNetwork pathingNetwork2;

    [SerializeField]
    MessageDisplay messageDisplay;

    [SerializeField]
    GameObject enemyPrefab;

    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioClip tutorialMusic;

    [SerializeField]
    float elapsedTime = 0f;

	void Start()
	{
        if (SceneManager.GetActiveScene().name == "Debug")
        {
            StartWaves();
        }
    }

    public void StartWaves()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        int largeSpawn = 0;
        int giantSpawn = 0;

        while (elapsedTime < 60f)
        {
            elapsedTime += 1.75f;
            yield return new WaitForSeconds(1.75f);
            SpawnEnemy(1, pathingNetwork);
        }

        messageDisplay.DisplayTimedMessage("Egads! There are larger ones now!", 10f);

        while (elapsedTime < 120f)
        {
            elapsedTime += 1.5f;
            yield return new WaitForSeconds(1.5f);
            if (largeSpawn == 0)
            {
                SpawnEnemy(2, pathingNetwork);
            }
            else
            {
                SpawnEnemy(1, pathingNetwork);
            }

            largeSpawn = (largeSpawn + 1) % 4;
        }

        messageDisplay.DisplayTimedMessage("Giant goblins?! Has the world gone mad?!", 10f);

        while (elapsedTime < 180f)
        {
            elapsedTime += 1.25f;
            yield return new WaitForSeconds(1.25f);
            if (giantSpawn == 0)
            {
                SpawnEnemy(3, pathingNetwork);
            }
            else if (largeSpawn == 0)
            {
                SpawnEnemy(2, pathingNetwork);
            }
            else
            {
                SpawnEnemy(1, pathingNetwork);
            }

            largeSpawn = (largeSpawn + 1) % 3;
            giantSpawn = (giantSpawn + 1) % 12;
        }

        messageDisplay.DisplayTimedMessage("Watch out Your Majesty! They now approach on the other side as well!!", 10f);

        while (elapsedTime < 240f)
        {
            elapsedTime += 1f;
            yield return new WaitForSeconds(1f);
            if (giantSpawn == 0)
            {
                SpawnEnemy(3, pathingNetwork);
            }
            else if (largeSpawn == 0)
            {
                SpawnEnemy(2, pathingNetwork);
            }
            else
            {
                SpawnEnemy(1, pathingNetwork);
            }

            SpawnEnemy(1, pathingNetwork2);

            largeSpawn = (largeSpawn + 1) % 2;
            giantSpawn = (giantSpawn + 1) % 10;
        }

        messageDisplay.DisplayTimedMessage("They're giving it all they've got!", 10f);

        while (elapsedTime < 360f)
        {
            elapsedTime += 0.8f;
            yield return new WaitForSeconds(0.8f);
            if (giantSpawn == 0)
            {
                SpawnEnemy(3, pathingNetwork);
                SpawnEnemy(3, pathingNetwork2);
            }
            else if (largeSpawn == 0)
            {
                SpawnEnemy(2, pathingNetwork);
                SpawnEnemy(2, pathingNetwork2);
            }
            else
            {
                SpawnEnemy(1, pathingNetwork);
                SpawnEnemy(1, pathingNetwork2);
            }

            largeSpawn = (largeSpawn + 1) % 2;
            giantSpawn = (giantSpawn + 1) % 8;
        }

        musicSource.clip = tutorialMusic;
        musicSource.Play();

        messageDisplay.DisplayTimedMessage("I believe we have them defeated! Hooray! Congratulations!", 10f);

        yield return new WaitForSeconds(10f);

        messageDisplay.DisplayIndefiniteMessage("(You win! Thanks for playing. Feel free to continue by pressing P.)");

        while (!(Input.GetKeyUp(KeyCode.P))) yield return null;

        messageDisplay.DisplayIndefiniteMessage("");

        while (true)
        {
            elapsedTime += 0.7f;
            yield return new WaitForSeconds(0.7f);
            if (giantSpawn == 0)
            {
                SpawnEnemy(3, pathingNetwork);
                SpawnEnemy(3, pathingNetwork2);
            }
            else if (largeSpawn == 0)
            {
                SpawnEnemy(2, pathingNetwork);
                SpawnEnemy(2, pathingNetwork2);
            }
            else
            {
                SpawnEnemy(1, pathingNetwork);
                SpawnEnemy(1, pathingNetwork2);
            }

            largeSpawn = (largeSpawn + 1) % 2;
            giantSpawn = (giantSpawn + 1) % 6;
        }
    }

    void SpawnEnemy(int size, PathingNetwork network)
    {
        var enemy = Instantiate(enemyPrefab, network.Nodes[0].position + Vector3.up, Quaternion.LookRotation(network.Nodes[1].position, Vector3.up));
        enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.VelocityChange);

        var enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.PathingNetwork = network;
        enemyScript.BuildSystem = buildSystem;

        if (size == 2)
        {
            enemyScript.MakeLarge();
        }
        else if (size == 3)
        {
            enemyScript.MakeGiant();
        }
    }
}
