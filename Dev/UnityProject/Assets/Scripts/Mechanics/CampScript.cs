using UnityEngine;
using System.Collections;

public class CampScript : MonoBehaviour
{
    public int faction = 0;
    public int armyFaction = 0;

    public GameObject cylinder;
    public Transform arrow;
    public TextMesh text;
    public GameObject progress;

    public CampScript[] neighbours;
    public CampScript selectedNeighbour;
    public float numUnits = 0;
    public float productionMultiplier = 0.5f;
    public float deploymentMultiplier = 1.5f;
    public GameObject armyPrefab;
    public Transform armyParent;

    private float campHealth = 1;
    private float deploymentTimer = 0;
    private float lastAttackTimer = 0;

    // Use this for initialization
    void Start()
    {
        armyFaction = faction;
        if (faction != 0)
            numUnits = 10;

        if (arrow == null && transform.childCount > 0)
        {
            arrow = transform.GetChild(0);
        }
        arrow.gameObject.SetActive(false);
        progress.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Clamp(campHealth, 0.05f, 1f));

        cylinder.GetComponent<Renderer>().material.color = GameManager.colors[GameManager.playerColors[faction]];
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameManager.State.Playing)
            return;
        if (faction == 0) //neutral base
        {
            //do nothing
        }
        else if (faction == armyFaction) //own army
        {
            numUnits += Time.deltaTime * productionMultiplier;
        }
        else if(numUnits < 2f && lastAttackTimer > 1f) //enemy army
        {
            numUnits -= Time.deltaTime * productionMultiplier * 0.1f;
            if(numUnits <= 0)
            {
                numUnits = 0;
                armyFaction = faction;

                selectedNeighbour = null;
                arrow.gameObject.SetActive(false);
                deploymentTimer = 0;
            }
        }

        deploymentTimer += Time.deltaTime * deploymentMultiplier;
        lastAttackTimer += Time.deltaTime;

        if (faction != armyFaction)
        {
            campHealth -= Time.deltaTime * numUnits * 0.01f;
            if (campHealth <= 0)
            {
                faction = armyFaction;
                transferBase();
            }
            progress.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Clamp(campHealth, 0.05f, 1f));
        }
        else if (campHealth < 1f) //else, because we dont want it to stack
        {
            campHealth += Time.deltaTime * numUnits * 0.01f;
            progress.GetComponent<Renderer>().material.SetFloat("_Cutoff", Mathf.Clamp(campHealth, 0.05f, 1f));
        }

        if (selectedNeighbour != null && numUnits >= 1f && deploymentTimer >= 1f)
        {
            //create unit
            GameObject army = GameObject.Instantiate<GameObject>(armyPrefab);
            army.transform.parent = armyParent;
            ArmyScript scr = army.GetComponent<ArmyScript>();
            scr.from = this;
            scr.to = selectedNeighbour;
            scr.faction = faction;

            numUnits--;
            deploymentTimer %= 1f;
        }

        text.GetComponent<Renderer>().material.color = GameManager.colors[GameManager.playerColors[armyFaction]];
        text.text = "" + Mathf.FloorToInt(numUnits);
    }

    public void addUnit(ArmyScript army)
    {
        if (armyFaction == army.faction)
        {
            numUnits++;
        }
        else if (numUnits >= 1f)
        {
            numUnits--;
            lastAttackTimer = 0;
        }
        else
        {
            numUnits = 0;
            armyFaction = army.faction;

            lastAttackTimer = 0;
        }
    }

    private void transferBase() {
        selectedNeighbour = null;
        arrow.gameObject.SetActive(false);

        cylinder.GetComponent<Renderer>().material.color = GameManager.colors[GameManager.playerColors[faction]];

        deploymentTimer = 0;
    }
}
