using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    [SerializeField]
    GolfShot golf;

    [SerializeField]
    MessageDisplay messageDisplay;

    [SerializeField]
    Transform golfBall;

    [SerializeField]
    Text goldText;

    [SerializeField]
    Text livesText;

    [SerializeField]
    GameObject shotRadius;

    [SerializeField]
    GameObject regularTower;

    [SerializeField]
    GameObject regularTowerBlueprint;

    enum State
    {
        Placing,
        Building,
    }
    State state = State.Placing;

    public enum TowerType
    {
        Regular,
        Strong,
    }
    public TowerType BlueprintType { get; set; }
    GameObject currentBlueprint;

    int lives = 50;
    int Lives { get { return lives ; } set { lives = value; livesText.text = lives.ToString(); } }

    int gold = 50;
    int Gold { get { return gold; } set { gold = value; goldText.text = gold.ToString(); } }

    const float MINIMUM_TOWER_BUILD_RADIUS = 4f;
    const float MINIMUM_NODE_BUILD_RADIUS = 10f;
    const float BUILD_TIME = 2f;
    const float BUILD_RATE = 1f / BUILD_TIME;

    void Start()
    {
        Gold = gold;
        Lives = lives;
    }

    void OnEnable()
    {
        state = State.Placing;

        float targetingDistance = 1f;

        switch (BlueprintType)
        {
            case TowerType.Regular:
                {
                    currentBlueprint = regularTowerBlueprint;
                    targetingDistance = regularTower.GetComponent<Tower>().TargetingDistance;
                    currentBlueprint.transform.localScale = Vector3.one;
                    break;
                }
            case TowerType.Strong:
                {
                    currentBlueprint = regularTowerBlueprint;
                    targetingDistance = 50f;
                    currentBlueprint.transform.localScale = Vector3.one * 2f;
                    break;
                }
        }

        currentBlueprint.transform.position = golfBall.position;
        currentBlueprint.SetActive(true);

        shotRadius.transform.position = golfBall.position;
        shotRadius.transform.localScale = new Vector3(targetingDistance, shotRadius.transform.localScale.y, targetingDistance);
        shotRadius.SetActive(true);
    }

    void OnDisable()
    {
        currentBlueprint.SetActive(false);
        shotRadius.SetActive(false);
    }

    void Update()
    {
        if (state == State.Placing && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BuildTower());
        }
        else if (state == State.Placing && Input.GetKeyDown(KeyCode.Escape))
        {
            golf.EndBuilding();
        }
    }

    public void GainGold(int amount)
    {
        Gold += amount;
    }

    public void LoseLife()
    {
        Lives -= 1;
        if (lives == 0)
        {
            SceneManager.LoadScene("Title");
        }
    }

    public bool CanBuildTower(TowerType towerType)
    {
        if (gold < CostForTower(towerType))
        {
            messageDisplay.DisplayTimedMessage("Not enough gold!", 10f);
            return false;
        }

        var towers = FindObjectsOfType<Tower>();
        foreach (var tower in towers)
        {
            var towerPosition = tower.transform.position;
            towerPosition.y = 0;
            var golfBallPosition = golfBall.position;
            golfBallPosition.y = 0;

            if (Vector3.Distance(towerPosition, golfBallPosition) < MINIMUM_TOWER_BUILD_RADIUS)
            {
                messageDisplay.DisplayTimedMessage("Too close to another tower", 10f);
                return false;
            }
        }

        var spawnNetworks = FindObjectsOfType<PathingNetwork>();
        foreach (var spawnNetwork in spawnNetworks)
        {
            var spawnNodes = spawnNetwork.GetComponentsInChildren<Transform>();
            foreach (var node in spawnNodes)
            {
                var nodePosition = node.transform.position;
                nodePosition.y = 0;
                var golfBallPosition = golfBall.position;
                golfBallPosition.y = 0;

                if (Vector3.Distance(nodePosition, golfBallPosition) < MINIMUM_NODE_BUILD_RADIUS)
                {
                    messageDisplay.DisplayTimedMessage("Too close to an enemy pathing node!", 10f);
                    return false;
                }
            }
        }

        return true;
    }

    int CostForTower(TowerType tower)
    {
        switch (tower)
        {
            case TowerType.Regular: return 10;
            case TowerType.Strong: return 40;
            default: throw new InvalidOperationException();
        }
    }

    IEnumerator BuildTower()
    {
        state = State.Building;

        Gold -= CostForTower(BlueprintType);

        currentBlueprint.SetActive(false);
        shotRadius.SetActive(false);

        GameObject tower = Instantiate(regularTower, golfBall.position, Quaternion.identity);
        Tower towerScript = tower.GetComponent<Tower>();

        if (BlueprintType == TowerType.Strong)
        {
            towerScript.MakeStrong();
        }
        float finalScale = tower.transform.localScale.y;
        tower.transform.localScale = new Vector3(1f, 0.01f, 1f);

        towerScript.enabled = false;

        Transform teePoint = tower.transform.Find("TeePoint");

        while (tower.transform.localScale.y < finalScale)
        {
            var newScale = Mathf.Clamp(tower.transform.localScale.y + BUILD_RATE * Time.deltaTime, 0f, finalScale);
            tower.transform.localScale = new Vector3(finalScale, newScale, finalScale);

            golfBall.position = teePoint.position;

            yield return null;
        }

        towerScript.enabled = true;
        golf.EndBuilding();
    }
}
