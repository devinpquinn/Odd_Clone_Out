using UnityEngine;
using UnityEngine.Events;

public class ZoomListener : MonoBehaviour
{
    public UnityEvent onZoomIn;
    public UnityEvent onZoomOut;

    private void OnEnable()
    {
        LevelController.ZoomChanged += HandleZoomChanged;
    }

    private void OnDisable()
    {
        LevelController.ZoomChanged -= HandleZoomChanged;
    }

    private void HandleZoomChanged(LevelController.ZoomDirection direction)
    {
        if (direction == LevelController.ZoomDirection.ZoomIn)
        {
            onZoomIn.Invoke();
            return;
        }

        onZoomOut.Invoke();
    }
}
