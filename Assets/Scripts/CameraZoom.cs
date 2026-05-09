using DG.Tweening;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera targetCamera;
    public float zoomedSize = 3f;
    public float zoomDuration = 0.2f;
    public Ease zoomEase = Ease.OutQuad;
    [Tooltip("How quickly the camera pans to follow the mouse while zoomed (higher = snappier).")]
    public float panSpeed = 8f;

    private float _defaultSize;
    private Vector3 _defaultPosition;
    private Tween _zoomTween;
    private Tween _moveTween;
    private bool _isZoomed;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        _defaultSize = targetCamera.orthographicSize;
        _defaultPosition = targetCamera.transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _isZoomed = true;
            _zoomTween?.Kill();
            _zoomTween = targetCamera.DOOrthoSize(zoomedSize, zoomDuration).SetEase(zoomEase);
            // Tween to the mouse target on zoom in
            _moveTween?.Kill();
            _moveTween = targetCamera.transform
                .DOMove(ClampedPanTarget(), zoomDuration)
                .SetEase(zoomEase);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            _isZoomed = false;
            _zoomTween?.Kill();
            _zoomTween = targetCamera.DOOrthoSize(_defaultSize, zoomDuration).SetEase(zoomEase);
            _moveTween?.Kill();
            _moveTween = targetCamera.transform
                .DOMove(_defaultPosition, zoomDuration)
                .SetEase(zoomEase);
        }

        if (_isZoomed && (_zoomTween == null || !_zoomTween.IsActive()))
        {
            // Zoom-in transition complete — lerp to follow the mouse
            if (_moveTween != null && _moveTween.IsActive())
                _moveTween.Kill();
            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position,
                ClampedPanTarget(),
                Time.deltaTime * panSpeed);
        }
    }

    // Returns the camera position that centers the zoomed view on the mouse cursor,
    // clamped so the zoomed viewport never exceeds the bounds of the default view.
    // Offsets are applied along the camera's local right/up axes to correctly account
    // for any camera rotation (e.g. isometric tilt/yaw).
    private Vector3 ClampedPanTarget()
    {
        float aspect = targetCamera.aspect;
        float maxOffsetX = Mathf.Max(0f, (_defaultSize - zoomedSize) * aspect);
        float maxOffsetY = Mathf.Max(0f, _defaultSize - zoomedSize);

        float nx = Input.mousePosition.x / Screen.width  - 0.5f;  // -0.5 .. 0.5
        float ny = Input.mousePosition.y / Screen.height - 0.5f;

        float offsetX = Mathf.Clamp(nx * 2f * maxOffsetX, -maxOffsetX, maxOffsetX);
        float offsetY = Mathf.Clamp(ny * 2f * maxOffsetY, -maxOffsetY, maxOffsetY);

        return _defaultPosition
            + targetCamera.transform.right * offsetX
            + targetCamera.transform.up    * offsetY;
    }
}
