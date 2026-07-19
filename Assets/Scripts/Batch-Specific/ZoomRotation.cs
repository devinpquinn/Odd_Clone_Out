using UnityEngine;
using DG.Tweening;

public class ZoomRotation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Zoom Rotation")]
    [Tooltip("World-space Euler angles applied while zoomed in.")]
    [SerializeField] private Vector3 zoomedWorldEuler = new Vector3(0f, 45f, 0f);
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.InOutQuad;

    private Tween _rotateTween;
    private Quaternion _previousWorldRotation;
    private bool _hasStoredPreviousRotation;

    private void Awake()
    {
        if (target == null)
            target = transform;
    }

    // Hook this to ZoomListener.onZoomIn in the inspector.
    public void OnZoomIn()
    {
        if (target == null) return;

        if (!_hasStoredPreviousRotation)
        {
            _previousWorldRotation = target.rotation;
            _hasStoredPreviousRotation = true;
        }

        RotateToWorldEuler(zoomedWorldEuler);
    }

    // Hook this to ZoomListener.onZoomOut in the inspector.
    public void OnZoomOut()
    {
        if (target == null || !_hasStoredPreviousRotation) return;

        _rotateTween?.Kill();
        _rotateTween = target
            .DORotateQuaternion(_previousWorldRotation, duration)
            .SetEase(ease)
            .OnComplete(() => _hasStoredPreviousRotation = false);
    }

    // Backward-compatible wrappers if these names are already used in existing events.
    public void ZoomInRotation()
    {
        OnZoomIn();
    }

    public void ZoomOutRotation()
    {
        OnZoomOut();
    }

    private void RotateToWorldEuler(Vector3 worldEuler)
    {
        _rotateTween?.Kill();
        _rotateTween = target
            .DORotate(worldEuler, duration, RotateMode.Fast)
            .SetEase(ease);
    }

    private void OnDisable()
    {
        _rotateTween?.Kill();
    }
}
