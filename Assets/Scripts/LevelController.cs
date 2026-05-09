using DG.Tweening;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform platform;
    public float rotationDuration = 0.5f;
    public Ease rotationEase = Ease.InOutQuad;

    private float _currentYRotation = 0f;
    private float[] _yRotationOptions = { 0f, 90f, 180f, 270f };
    private int _currentRotationIndex = 0;
    private Tween _rotateTween;

    public void RotateClockwise()
    {
        _currentRotationIndex = (_currentRotationIndex + 1) % _yRotationOptions.Length;
        _currentYRotation = _yRotationOptions[_currentRotationIndex];
        RotatePlatformTo(_currentYRotation);
    }

    public void RotateCounterClockwise()
    {
        _currentRotationIndex = (_currentRotationIndex - 1 + _yRotationOptions.Length) % _yRotationOptions.Length;
        _currentYRotation = _yRotationOptions[_currentRotationIndex];
        RotatePlatformTo(_currentYRotation);
    }

    private void RotatePlatformTo(float yDegrees)
    {
        _rotateTween?.Kill();
        _rotateTween = platform
            .DORotate(new Vector3(0f, yDegrees, 0f), rotationDuration, RotateMode.Fast)
            .SetEase(rotationEase);
    }
}
