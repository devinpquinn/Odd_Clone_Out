using UnityEngine;
using DG.Tweening;

public class ResistRotation : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform target;

	[Header("Counter Rotation")]
	[SerializeField] private float stepDegrees = 90f;
	[SerializeField] private float duration = 0.5f;
	[SerializeField] private Ease ease = Ease.InOutQuad;

	private Tween _rotateTween;

	private void Awake()
	{
		if (target == null)
			target = transform;
	}

	// Hook this to RotationListener.onClockwiseRotation in the inspector.
	public void OnLevelRotatedClockwise()
	{
		CounterRotate(stepDegrees);
	}

	// Hook this to RotationListener.onCounterClockwiseRotation in the inspector.
	public void OnLevelRotatedCounterClockwise()
	{
		CounterRotate(-stepDegrees);
	}

	// Backward-compatible wrappers if these names are already used in existing events.
	public void CounterClockwiseLevelRotation()
	{
		OnLevelRotatedClockwise();
	}

	public void CounterCounterClockwiseLevelRotation()
	{
		OnLevelRotatedCounterClockwise();
	}

	private void CounterRotate(float deltaY)
	{
		if (target == null) return;

		_rotateTween?.Kill();

		Vector3 localEuler = target.localEulerAngles;
		float nextY = localEuler.y - deltaY;

		_rotateTween = target
			.DOLocalRotate(new Vector3(localEuler.x, nextY, localEuler.z), duration)
			.SetEase(ease);
	}

	private void OnDisable()
	{
		_rotateTween?.Kill();
	}
}
