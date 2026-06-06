using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public Transform referenceSpawnPoint;
    public Batch activeBatch;
    
    public static string tagCreatureReference = "CloneReference";
    public static string tagCreatureNormal = "CloneNormal";
    public static string tagCreatureDeviant = "CloneDeviant";
    
    private void Start()
    {
        SpawnCreatures();
    }
    
    private void SpawnCreatures()
    {
        Transform deviantSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        float referenceYAngle = Random.Range(0, 4) * 90f;
        Quaternion referenceRotation = referenceSpawnPoint.rotation * Quaternion.Euler(0f, referenceYAngle, 0f);
        GameObject reference = Instantiate(activeBatch.normalPrefab, referenceSpawnPoint.position, referenceRotation, referenceSpawnPoint);
        reference.name = activeBatch.normalPrefab.name;
        
        reference.GetComponentInChildren<Collider>().gameObject.tag = tagCreatureReference;

        foreach (Transform spawnPoint in spawnPoints)
        {
            float yAngle = Random.Range(0, 4) * 90f;
            Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0f, yAngle, 0f);
            GameObject prefab = (spawnPoint == deviantSpawnPoint) ? activeBatch.deviantPrefab : activeBatch.normalPrefab;
            GameObject creature = Instantiate(prefab, spawnPoint.position, rotation, spawnPoint);
            creature.name = prefab.name;
            if (spawnPoint == deviantSpawnPoint)
            {
                creature.GetComponentInChildren<Collider>().gameObject.tag = tagCreatureDeviant;
            }
            else
            {
                creature.GetComponentInChildren<Collider>().gameObject.tag = tagCreatureNormal;
            }
        }
    }
}
