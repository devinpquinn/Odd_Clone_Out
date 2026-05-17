using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public GameObject creaturePrefab;
    
    private void Start()
    {
        SpawnCreatures();
    }
    
    private void SpawnCreatures()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            float yAngle = Random.Range(0, 4) * 90f;
            Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0f, yAngle, 0f);
            Instantiate(creaturePrefab, spawnPoint.position, rotation);
        }
    }
}
