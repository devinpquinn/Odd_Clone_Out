using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public Transform referenceSpawnPoint;
    public GameObject creaturePrefab;
    public GameObject deviantPrefab;
    
    private void Start()
    {
        SpawnCreatures();
    }
    
    private void SpawnCreatures()
    {
        Transform deviantSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        float referenceYAngle = Random.Range(0, 4) * 90f;
        Quaternion referenceRotation = referenceSpawnPoint.rotation * Quaternion.Euler(0f, referenceYAngle, 0f);
        GameObject reference = Instantiate(creaturePrefab, referenceSpawnPoint.position, referenceRotation, referenceSpawnPoint);
        reference.name = creaturePrefab.name;

        foreach (Transform spawnPoint in spawnPoints)
        {
            float yAngle = Random.Range(0, 4) * 90f;
            Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0f, yAngle, 0f);
            GameObject prefab = (spawnPoint == deviantSpawnPoint) ? deviantPrefab : creaturePrefab;
            GameObject creature = Instantiate(prefab, spawnPoint.position, rotation, spawnPoint);
            creature.name = prefab.name;
        }
    }
}
