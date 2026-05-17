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
        int creatureLayer = LayerMask.GetMask(creatureLayerName);

        SkinnedMeshRenderer hoveredRenderer = null;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, creatureLayer))
        {
            hoveredRenderer = hit.collider.GetComponentInChildren<SkinnedMeshRenderer>();
        }

        if (hoveredRenderer != _currentOutlined)
        {
            if (_currentOutlined != null)
                _currentOutlined.renderingLayerMask &= ~_outlineMask;

            _currentOutlined = hoveredRenderer;

            if (_currentOutlined != null)
                _currentOutlined.renderingLayerMask |= _outlineMask;
        }
    }
}
