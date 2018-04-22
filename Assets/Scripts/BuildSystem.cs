using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BuildSystem : MonoBehaviour
{
    [SerializeField]
    GolfShot golf;

    [SerializeField]
    Transform golfBall;

    [SerializeField]
    Text goldText;

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
    }
    public TowerType BlueprintType { get; set; }
    GameObject currentBlueprint;

    int gold = 50;
    int Gold { get { return gold; } set { gold = value; goldText.text = gold.ToString(); } }

    const float MINIMUM_BUILD_RADIUS = 3f;
    const float BUILD_TIME = 2f;
    const float BUILD_RATE = 1f / BUILD_TIME;

    void Start()
    {
        Gold = gold;
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

    public bool CanBuildTower(TowerType towerType)
    {
        if (gold < CostForTower(towerType))
        {
            return false;
        }

        var towers = FindObjectsOfType<Tower>();
        foreach (var tower in towers)
        {
            var towerPosition = tower.transform.position;
            towerPosition.y = 0;
            var golfBallPosition = golfBall.position;
            golfBallPosition.y = 0;

            if (Vector3.Distance(towerPosition, golfBallPosition) < MINIMUM_BUILD_RADIUS)
            {
                return false;
            }
        }

        return true;
    }

    int CostForTower(TowerType tower)
    {
        switch (tower)
        {
            case TowerType.Regular: return 10;
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
        tower.transform.localScale = new Vector3(1f, 0.01f, 1f);

        Tower towerScript = tower.GetComponent<Tower>();
        towerScript.enabled = false;

        Transform teePoint = tower.transform.Find("TeePoint");

        while (tower.transform.localScale.y < 1f)
        {
            var newScale = Mathf.Clamp01(tower.transform.localScale.y + BUILD_RATE * Time.deltaTime);
            tower.transform.localScale = new Vector3(1f, newScale, 1f);

            golfBall.position = teePoint.position;

            yield return null;
        }

        towerScript.enabled = true;
        golf.EndBuilding();
    }
}
