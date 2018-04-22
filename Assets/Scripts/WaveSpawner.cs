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
    GameObject enemyPrefab;

	void Start()
	{
        if (SceneManager.GetActiveScene().name == "Debug")
        {
            StartWaves();
        }
    }
	
	void Update()
	{
	}

    public void StartWaves()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.75f);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        var enemy = Instantiate(enemyPrefab, pathingNetwork.Nodes[0].position + Vector3.up, Quaternion.LookRotation(pathingNetwork.Nodes[1].position, Vector3.up));
        enemy.GetComponent<Rigidbody>().AddForce(Vector3.up * 2f, ForceMode.VelocityChange);

        var enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.PathingNetwork = pathingNetwork;
        enemyScript.BuildSystem = buildSystem;
    }
}
