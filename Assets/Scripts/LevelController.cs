using DG.Tweening;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform platform;
    public float rotationDuration = 0.5f;
    public Ease rotationEase = Ease.InOutQuad;

    private Tween _rotateTween;

    public void RotateClockwise()
    {
        RotatePlatformBy(90f);
    }

    public void RotateCounterClockwise()
    {
        RotatePlatformBy(-90f);
    }

    private void RotatePlatformBy(float yDelta)
    {
        _rotateTween?.Kill();
        _rotateTween = platform
            .DORotate(new Vector3(0f, yDelta, 0f), rotationDuration, RotateMode.LocalAxisAdd)
            .SetEase(rotationEase);
    }
}
