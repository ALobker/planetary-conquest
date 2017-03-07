using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Hive AI
 * Distributed AI where each camp has individual behaviour.
 * Each neighbour can be chosen at random, but likelihood varies. This makes it non-deterministic.
 */
public class HiveAI : MonoBehaviour
{
    public CampScript camp;

    public float neutralFactor = 1;
    public float enemyFactor = 0.5f;
    public float rescueFactor = 1;
    public float captureFactor = 1;
    public float unitsFactor = 1;
    public float frontlineFactor = .25f;
    public float surroundFactor = .1f;

    private float actionTimer = 0;

    // Use this for initialization
    void Start()
    {
        if (camp == null)
            camp = this.GetComponent<CampScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState != GameManager.State.Playing)
            return;
        actionTimer += Time.deltaTime;
        if (actionTimer >= 1f)
        {
            setArrow();
            actionTimer--;
        }
    }

    // determine which neighbour should be approached
    private void setArrow()
    {
        if (camp.faction == 0 || camp.faction == 1) //no ai for neutral or player
            return;
        if (camp.faction != camp.armyFaction || camp.numUnits < 5f)
        {
            camp.selectedNeighbour = null;
            camp.arrow.gameObject.SetActive(false);
        }
        else
        {
            camp.arrow.gameObject.SetActive(true);
            CampScript bestNeigh = camp.neighbours[0];

            //probability of choosing each of the neighbours
            float[] likelihoods = new float[camp.neighbours.Length];
            float totallikelihoods = 0;
            for (int i = 0; i < camp.neighbours.Length; i++)
            {
                CampScript neigh = camp.neighbours[i];
                float likelihood = 0.5f; //base likelihood

                if (camp.faction == neigh.armyFaction && neigh.numUnits > 100)
                {
                    //dont send units to a mega base
                    likelihood = 0.01f;
                }
                else
                {
                    //neighbour is neutral
                    if (neigh.faction == 0)
                        likelihood += neutralFactor;
                    //neighbour is enemy
                    if (neigh.faction != camp.faction)
                        likelihood += enemyFactor;
                    //rescue our own camps
                    if (neigh.faction == camp.faction && neigh.faction != neigh.armyFaction)
                        likelihood += rescueFactor;
                    //reinforce neighbours that we are invading
                    if (neigh.faction != camp.faction && camp.faction == neigh.armyFaction)
                        likelihood += captureFactor * (1f / (neigh.numUnits + 1));
                    //few units
                    likelihood += unitsFactor * (1f / Mathf.Sqrt(neigh.numUnits + 1));
                    //is the neighbour near the front line
                    int numEnemyCamps = 0;
                    foreach (CampScript second in neigh.neighbours)
                    {
                        if (second.faction != camp.faction)
                            numEnemyCamps++;
                    }
                    likelihood += frontlineFactor * numEnemyCamps;
                    //is the neighbour surrounded by enemies
                    float numSurroundEnemies = 0;
                    foreach (CampScript second in neigh.neighbours)
                    {
                        if (second == camp)
                            continue;
                        if (second.armyFaction != camp.faction)
                            numSurroundEnemies += second.numUnits;
                        else
                            numSurroundEnemies -= second.numUnits;
                    }
                    if(numSurroundEnemies > 0)
                        likelihood += surroundFactor * Mathf.Sqrt(numSurroundEnemies);
                }

                likelihoods[i] = likelihood;
                totallikelihoods += likelihood;
            }
            //Debug.Log("Likelihoods: " + likelihoods[0] + " | " + likelihoods[1] + " | " + likelihoods[2] + " | " + likelihoods[3]);

            //normalize likelihood
            for (int i = 0; i < likelihoods.Length; i++)
            {
                likelihoods[i] /= totallikelihoods;
            }

            //pick one of the neighbours
            float rand = Random.Range(0f, 1f);
            for (int i = 0; i < likelihoods.Length; i++)
            {
                rand -= likelihoods[i];
                if (rand <= 0)
                {
                    //set i as neighbour
                    bestNeigh = camp.neighbours[i];
                    break;
                }
            }

            camp.arrow.transform.position = Vector3.Slerp(camp.transform.position, bestNeigh.transform.position, .25f);
            float campAngle = -DraggingInteraction.AngleSigned(bestNeigh.transform.position - Vector3.Project(bestNeigh.transform.position, camp.transform.position), camp.transform.right, camp.transform.up);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, campAngle, 0));
            camp.arrow.transform.localRotation = rotation;
            camp.selectedNeighbour = bestNeigh;
        }
    }
}
