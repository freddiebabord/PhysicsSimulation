using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject puckPrefab;
    public Transform puckSpawnPoint;
    public GameObject currentSpawnedPuck;
    public Text directionText, speedText, velocityText, massText;
    public Vector3 testDirection = Vector3.forward;
    public float forceMagnitude = 5;
    private bool recentSpawn;
    public float spawnTime = 2.0f;
    List<GameObject> spawnedPucks = new List<GameObject>();
    public int maxPucks = 5;
    public Text puckVelocities;
    private bool trim = false;
    
    private int totalSpawned = 0;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    puckVelocities.text = "";

        if (currentSpawnedPuck)
	    {
	        directionText.text = "Direction: " + currentSpawnedPuck.GetComponent<CustomRigidbody>().currentVelocity.normalized.ToString();
            speedText.text = "Speed: " + currentSpawnedPuck.GetComponent<CustomRigidbody>().currentVelocity.magnitude.ToString() + "m/s";
            velocityText.text = "Velocity: " + currentSpawnedPuck.GetComponent<CustomRigidbody>().currentVelocity.ToString();
            

        }
        if (Input.GetKey(KeyCode.Space))
        {
            currentSpawnedPuck.GetComponent<CustomRigidbody>().AddForce(testDirection * forceMagnitude);
        }
        for (var i = 0; i < spawnedPucks.Count; i++)
        {
            if (spawnedPucks[i] == null)
            {
                trim = true;
                continue;
            }
            puckVelocities.text += "Velocity for " + spawnedPucks[i].name + ": " + spawnedPucks[i].GetComponent<CustomRigidbody>().currentVelocity.ToString() + "\n";
        }
	    if (trim)
	    {
	        spawnedPucks.TrimExcess();
            trim = false;
	    }

        if(Input.GetKeyDown(KeyCode.S))
            SpawnPuck();
        if(Input.GetKeyDown(KeyCode.R))
            Reset();
	    if (Input.GetKeyDown(KeyCode.B))
	        PhysicsResolver.inst.showBoundingVolumes.isOn = !PhysicsResolver.inst.showBoundingVolumes.isOn;
	}

    public void SpawnPuck()
    {
        if (!recentSpawn)
        {
            if (spawnedPucks.Count >= maxPucks)
            {
                Destroy(spawnedPucks[0]);
                spawnedPucks.TrimExcess();
            }
            
            GameObject go = Instantiate(puckPrefab, puckSpawnPoint.position, puckSpawnPoint.rotation) as GameObject;
            go.name = "Puck " + totalSpawned.ToString();
            //massText.text = go.GetComponent<CustomRigidbody>().mass.ToString();
            go.GetComponent<Renderer>().material.color = Color.red;
            if(currentSpawnedPuck)
                currentSpawnedPuck.GetComponent<Renderer>().material.color = Color.white;
            spawnedPucks.Add(go);
            currentSpawnedPuck = go;
            totalSpawned++;
            StartCoroutine(spawnTimer());
        }
    }

    public void SetMass(string newMass)
    {
        float mass = 0.0f;
        if (!float.TryParse(newMass, out mass))
            return;
        currentSpawnedPuck.GetComponent<CustomRigidbody>().mass = mass;
    }

    IEnumerator spawnTimer()
    {
        recentSpawn = true;
        yield return new WaitForSeconds(spawnTime);
        recentSpawn = false;
    }

    public void Reset()
    {
        PhysicsResolver.inst.Reset();
        for (var i = 0; i < spawnedPucks.Count; i++)
        {
            Destroy(spawnedPucks[i]);
        }
        spawnedPucks.Clear();
        totalSpawned = 0;
    }
}
