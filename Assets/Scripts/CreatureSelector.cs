using UnityEngine;
using UnityEngine.InputSystem;

public class CreatureSelector : MonoBehaviour
{
    private string creatureLayerName = "Creature";
    private string outlineLayerName = "Outline";

    private Camera _mainCamera;
    private SkinnedMeshRenderer _currentOutlined;
    private uint _outlineMask;

    private void Start()
    {
        _mainCamera = Camera.main;
        _outlineMask = RenderingLayerMask.GetMask(outlineLayerName);
    }

    private void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        int creatureLayerMask = LayerMask.GetMask(creatureLayerName);
        int creatureLayer = LayerMask.NameToLayer(creatureLayerName);

        SkinnedMeshRenderer hoveredRenderer = null;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, creatureLayerMask))
        {
            if (!hit.collider.CompareTag(CreatureSpawner.tagCreatureReference))
            {
                hoveredRenderer = hit.collider.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        if (hoveredRenderer != _currentOutlined)
        {
            if (_currentOutlined != null)
                _currentOutlined.renderingLayerMask &= ~_outlineMask;

            _currentOutlined = hoveredRenderer;

            if (_currentOutlined != null)
                _currentOutlined.renderingLayerMask |= _outlineMask;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && _currentOutlined != null)
        {
            Collider creatureCollider = _currentOutlined.GetComponentInParent<Collider>();
            if (creatureCollider == null)
            {
                creatureCollider = _currentOutlined.GetComponentInChildren<Collider>();
            }

            if (creatureCollider != null)
            {
                if (creatureCollider.CompareTag(CreatureSpawner.tagCreatureDeviant))
                {
                    Debug.Log("Clicked creature: Deviant clone");
                }
                else if (creatureCollider.CompareTag(CreatureSpawner.tagCreatureNormal))
                {
                    Debug.Log("Clicked creature: Normal clone");
                }

                GameObject creatureRoot = GetCreatureRoot(_currentOutlined.gameObject, creatureLayer);
                _currentOutlined.renderingLayerMask &= ~_outlineMask;
                _currentOutlined = null;
                creatureRoot.SetActive(false);
            }
        }
    }

    private GameObject GetCreatureRoot(GameObject source, int creatureLayer)
    {
        Transform current = source.transform;
        while (current.parent != null && current.parent.gameObject.tag != "SpawnPoint")
        {
            current = current.parent;
        }

        return current.gameObject;
    }
}
