using DG.Tweening;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera targetCamera;
    public float zoomedSize = 3f;
    public float zoomDuration = 0.2f;
    public Ease zoomEase = Ease.OutQuad;

    private float _defaultSize;
    private Tween _zoomTween;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        _defaultSize = targetCamera.orthographicSize;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            ZoomTo(zoomedSize);
        else if (Input.GetMouseButtonUp(1))
            ZoomTo(_defaultSize);
    }

    private void ZoomTo(float fov)
    {
        _zoomTween?.Kill();
        _zoomTween = targetCamera.DOOrthoSize(fov, zoomDuration).SetEase(zoomEase);
    }
}
