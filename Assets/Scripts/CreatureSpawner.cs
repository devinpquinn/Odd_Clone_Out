using UnityEngine;
using System.Collections.Generic;

public class CreatureSpawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public Transform referenceSpawnPoint;
    public List<Stage> stages;
    
    public static string tagCreatureReference = "CloneReference";
    public static string tagCreatureNormal = "CloneNormal";
    public static string tagCreatureDeviant = "CloneDeviant";

    private int _currentStageIndex;
    private int _currentBatchIndex;
    private bool _isComplete;
    
    private void Start()
    {
        _currentStageIndex = 0;
        _currentBatchIndex = 0;
        _isComplete = false;
        SpawnCurrentBatch();
    }
    
    public void AdvanceToNextBatch()
    {
        if (_isComplete)
        {
            return;
        }

        ClearCurrentBatch();

        if (!MoveToNextBatch())
        {
            _isComplete = true;
            Debug.Log("All stages and batches complete.");
            return;
        }

        SpawnCurrentBatch();
    }

    public bool JumpToBatch(int stageIndex, int batchIndex)
    {
        if (stages == null || stages.Count == 0)
        {
            Debug.LogWarning("Cannot jump: stages list is empty.");
            return false;
        }

        if (stageIndex < 0 || stageIndex >= stages.Count)
        {
            Debug.LogWarning($"Cannot jump: stage index {stageIndex} is out of range.");
            return false;
        }

        Stage stage = stages[stageIndex];
        if (stage == null || stage.batches == null || stage.batches.Count == 0)
        {
            Debug.LogWarning($"Cannot jump: stage at index {stageIndex} has no batches.");
            return false;
        }

        if (batchIndex < 0 || batchIndex >= stage.batches.Count)
        {
            Debug.LogWarning($"Cannot jump: batch index {batchIndex} is out of range for stage '{stage.name}'.");
            return false;
        }

        ClearCurrentBatch();

        _currentStageIndex = stageIndex;
        _currentBatchIndex = batchIndex;
        _isComplete = false;

        SpawnCurrentBatch();
        return true;
    }

    private bool MoveToNextBatch()
    {
        _currentBatchIndex++;

        while (_currentStageIndex < stages.Count)
        {
            Stage stage = stages[_currentStageIndex];
            int stageBatchCount = stage != null && stage.batches != null ? stage.batches.Count : 0;

            if (_currentBatchIndex < stageBatchCount)
            {
                return true;
            }

            _currentStageIndex++;
            _currentBatchIndex = 0;
        }

        return false;
    }

    private void SpawnCurrentBatch()
    {
        Batch currentBatch = GetCurrentBatch();
        if (currentBatch == null)
        {
            Debug.LogWarning("No valid batch available to spawn. Check stage and batch assignments.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0 || referenceSpawnPoint == null)
        {
            Debug.LogWarning("Spawner is missing spawn points or reference spawn point.");
            return;
        }

        SpawnCreatures(currentBatch);
    }

    private Batch GetCurrentBatch()
    {
        while (_currentStageIndex < stages.Count)
        {
            Stage stage = stages[_currentStageIndex];
            if (stage == null || stage.batches == null || stage.batches.Count == 0)
            {
                _currentStageIndex++;
                _currentBatchIndex = 0;
                continue;
            }

            if (_currentBatchIndex < stage.batches.Count)
            {
                return stage.batches[_currentBatchIndex];
            }

            _currentStageIndex++;
            _currentBatchIndex = 0;
        }

        return null;
    }

    private void SpawnCreatures(Batch batch)
    {
        Transform deviantSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        float referenceYAngle = Random.Range(0, 4) * 90f;
        Quaternion referenceRotation = referenceSpawnPoint.rotation * Quaternion.Euler(0f, referenceYAngle, 0f);
        GameObject reference = Instantiate(batch.normalPrefab, referenceSpawnPoint.position, referenceRotation, referenceSpawnPoint);
        reference.name = batch.normalPrefab.name;
        
        reference.GetComponentInChildren<Collider>().gameObject.tag = tagCreatureReference;

        foreach (Transform spawnPoint in spawnPoints)
        {
            float yAngle = Random.Range(0, 4) * 90f;
            Quaternion rotation = spawnPoint.rotation * Quaternion.Euler(0f, yAngle, 0f);
            GameObject prefab = (spawnPoint == deviantSpawnPoint) ? batch.deviantPrefab : batch.normalPrefab;
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

    private void ClearCurrentBatch()
    {
        ClearChildren(referenceSpawnPoint);

        foreach (Transform spawnPoint in spawnPoints)
        {
            ClearChildren(spawnPoint);
        }
    }

    private void ClearChildren(Transform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
