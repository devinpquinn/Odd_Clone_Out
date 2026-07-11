using UnityEngine;
using UnityEngine.InputSystem;

public class CreatureSelector : MonoBehaviour
{
    private string creatureLayerName = "Creature";
    private string outlineLayerName = "Outline";

    [SerializeField] private CreatureSpawner creatureSpawner;

    private Camera _mainCamera;
    private SkinnedMeshRenderer _currentOutlined;
    private uint _outlineMask;
    private int _creatureLayerMask;

    private void Start()
    {
        _mainCamera = Camera.main;
        _outlineMask = RenderingLayerMask.GetMask(outlineLayerName);
        _creatureLayerMask = LayerMask.GetMask(creatureLayerName);
    }

    private void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        SkinnedMeshRenderer hoveredRenderer = null;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _creatureLayerMask))
        {
            if (!hit.collider.CompareTag(CreatureSpawner.tagCreatureReference))
            {
                hoveredRenderer = hit.collider.GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }

        if (hoveredRenderer != _currentOutlined)
        {
            if (_currentOutlined != null)
            {
                _currentOutlined.renderingLayerMask &= ~_outlineMask;
                
                CreatureHoverHandler hoverHandler = _currentOutlined.GetComponent<CreatureHoverHandler>();
                if(hoverHandler)
                {
                    hoverHandler.EndHover();
                }
            } 

            _currentOutlined = hoveredRenderer;

            if (_currentOutlined != null)
            {
                _currentOutlined.renderingLayerMask |= _outlineMask;
                
                CreatureHoverHandler hoverHandler = _currentOutlined.GetComponent<CreatureHoverHandler>();
                if(hoverHandler)
                {
                    hoverHandler.StartHover();
                }
            }  
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
                bool isDeviant = creatureCollider.CompareTag(CreatureSpawner.tagCreatureDeviant);

                if (creatureCollider.CompareTag(CreatureSpawner.tagCreatureDeviant))
                {
                    Debug.Log("Clicked creature: Deviant clone");
                }
                else if (creatureCollider.CompareTag(CreatureSpawner.tagCreatureNormal))
                {
                    Debug.Log("Clicked creature: Normal clone");
                }

                _currentOutlined.renderingLayerMask &= ~_outlineMask;
                _currentOutlined = null;

                if (isDeviant && creatureSpawner != null)
                {
                    creatureSpawner.AdvanceToNextBatch();
                }
                else if (!isDeviant)
                {
                    GameObject creatureRoot = GetCreatureRoot(creatureCollider.gameObject);
                    creatureRoot.SetActive(false);
                }
            }
        }
    }

    private GameObject GetCreatureRoot(GameObject source)
    {
        Transform current = source.transform;
        while (current.parent != null && current.parent.gameObject.tag != "SpawnPoint")
        {
            current = current.parent;
        }

        return current.gameObject;
    }
}
