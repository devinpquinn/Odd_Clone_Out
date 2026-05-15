using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    [Header("Platform Rotation")]
    public Transform platform;
    public float rotationDuration = 0.5f;
    public Ease rotationEase = Ease.InOutQuad;

    [Header("Camera Zoom")]
    public Camera targetCamera;
    public float zoomedSize = 3f;
    public float zoomDuration = 0.2f;
    public Ease zoomEase = Ease.OutQuad;
    [Tooltip("How quickly the camera pans to follow the mouse while zoomed (higher = snappier).")]
    public float panSpeed = 8f;

    private float _currentYRotation = 0f;
    private Tween _rotateTween;

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
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _isZoomed = true;
            _zoomTween?.Kill();
            _zoomTween = targetCamera.DOOrthoSize(zoomedSize, zoomDuration).SetEase(zoomEase);
            _moveTween?.Kill();
            _moveTween = targetCamera.transform
                .DOMove(ClampedPanTarget(), zoomDuration)
                .SetEase(zoomEase);
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
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
            if (_moveTween != null && _moveTween.IsActive())
                _moveTween.Kill();
            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position,
                ClampedPanTarget(),
                Time.deltaTime * panSpeed);
        }
    }

    public void RotateClockwise()
    {
        if (_rotateTween != null && _rotateTween.IsActive() && _rotateTween.IsPlaying()) return;
        _currentYRotation += 90f;
        RotatePlatformTo(_currentYRotation);
    }

    public void RotateCounterClockwise()
    {
        if (_rotateTween != null && _rotateTween.IsActive() && _rotateTween.IsPlaying()) return;
        _currentYRotation -= 90f;
        RotatePlatformTo(_currentYRotation);
    }

    private void RotatePlatformTo(float yDegrees)
    {
        _rotateTween = platform
            .DORotate(new Vector3(0f, yDegrees, 0f), rotationDuration)
            .SetEase(rotationEase);
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

        Vector2 mousePos = Mouse.current.position.ReadValue();
        float nx = mousePos.x / Screen.width  - 0.5f;  // -0.5 .. 0.5
        float ny = mousePos.y / Screen.height - 0.5f;

        float offsetX = Mathf.Clamp(nx * 2f * maxOffsetX, -maxOffsetX, maxOffsetX);
        float offsetY = Mathf.Clamp(ny * 2f * maxOffsetY, -maxOffsetY, maxOffsetY);

        return _defaultPosition
            + targetCamera.transform.right * offsetX
            + targetCamera.transform.up    * offsetY;
    }
}
