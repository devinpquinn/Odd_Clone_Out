using DG.Tweening;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform platform;
    public float rotationDuration = 0.5f;
    public Ease rotationEase = Ease.InOutQuad;

    private float _currentYRotation = 0f;
    private Tween _rotateTween;

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
}
