using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DebugMenuController : MonoBehaviour
{
    public List<Stage> stages;
    
    [SerializeField] private CreatureSpawner creatureSpawner;
    public Transform debugMenuParent;
    public GameObject debugStageObjPrefab;
    public GameObject debugLevelButtonPrefab;

    private const string StageHeaderName = "DebugStageHeaderText";
    private const string StageButtonsContainerName = "DebugLevelButtons";

    private bool _isMenuVisible;

    private void Start()
    {
        if (creatureSpawner == null)
        {
            creatureSpawner = FindObjectOfType<CreatureSpawner>();
        }

        if (creatureSpawner != null && stages != null && stages.Count > 0)
        {
            creatureSpawner.stages = stages;
        }

        PopulateDebugMenu();

        _isMenuVisible = false;
        if (debugMenuParent != null)
        {
            debugMenuParent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleDebugMenu();
        }
    }

    private void PopulateDebugMenu()
    {
        if (debugMenuParent == null)
        {
            Debug.LogWarning("Debug menu parent is not assigned.");
            return;
        }

        if (debugStageObjPrefab == null || debugLevelButtonPrefab == null)
        {
            Debug.LogWarning("Debug stage object prefab and level button prefab must be assigned.");
            return;
        }

        ClearGeneratedStages();

        if (stages == null)
        {
            return;
        }

        for (int stageIndex = 0; stageIndex < stages.Count; stageIndex++)
        {
            Stage stage = stages[stageIndex];
            if (stage == null)
            {
                continue;
            }

            GameObject stageObject = Instantiate(debugStageObjPrefab, debugMenuParent);
            stageObject.name = stage.name;

            Transform headerTransform = stageObject.transform.GetChild(0).Find(StageHeaderName);
            if (headerTransform != null)
            {
                TMP_Text headerText = headerTransform.GetComponent<TMP_Text>();
                if (headerText != null)
                {
                    headerText.text = stage.name;
                }
            }

            Transform buttonsContainer = stageObject.transform.Find(StageButtonsContainerName);
            if (buttonsContainer == null)
            {
                Debug.LogWarning($"Stage object '{stageObject.name}' is missing child '{StageButtonsContainerName}'.");
                continue;
            }

            if (stage.batches == null)
            {
                continue;
            }

            for (int batchIndex = 0; batchIndex < stage.batches.Count; batchIndex++)
            {
                Batch batch = stage.batches[batchIndex];
                if (batch == null)
                {
                    continue;
                }

                int capturedStageIndex = stageIndex;
                int capturedBatchIndex = batchIndex;

                GameObject buttonObject = Instantiate(debugLevelButtonPrefab, buttonsContainer);
                buttonObject.name = batch.name;

                TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>(true);
                if (buttonText != null)
                {
                    buttonText.text = batch.name;
                }

                Button button = buttonObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => JumpToBatch(capturedStageIndex, capturedBatchIndex));
                }
                else
                {
                    Debug.LogWarning($"Debug button prefab '{debugLevelButtonPrefab.name}' is missing a Button component.");
                }
            }
        }
    }

    private void JumpToBatch(int stageIndex, int batchIndex)
    {
        if (creatureSpawner == null)
        {
            Debug.LogWarning("CreatureSpawner reference is missing.");
            return;
        }

        bool jumped = creatureSpawner.JumpToBatch(stageIndex, batchIndex);
        if (!jumped)
        {
            return;
        }

        if (_isMenuVisible)
        {
            ToggleDebugMenu();
        }
    }

    private void ToggleDebugMenu()
    {
        if (debugMenuParent == null)
        {
            return;
        }

        _isMenuVisible = !_isMenuVisible;
        debugMenuParent.gameObject.SetActive(_isMenuVisible);
    }

    private void ClearGeneratedStages()
    {
        for (int i = debugMenuParent.childCount - 1; i >= 0; i--)
        {
            Destroy(debugMenuParent.GetChild(i).gameObject);
        }
    }
}
