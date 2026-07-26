using UnityEngine;
using UnityEngine.Events;

public class RotationListener : MonoBehaviour
{
	public UnityEvent onClockwiseRotation;
	public UnityEvent onCounterClockwiseRotation;

	private void OnEnable()
	{
		LevelController.RotationStarted += HandleRotationStarted;
	}

	private void OnDisable()
	{
		LevelController.RotationStarted -= HandleRotationStarted;
	}

	private void HandleRotationStarted(LevelController.RotationDirection direction)
	{
		if (direction == LevelController.RotationDirection.Clockwise)
		{
			onClockwiseRotation.Invoke();
			return;
		}

		onCounterClockwiseRotation.Invoke();
	}
}
