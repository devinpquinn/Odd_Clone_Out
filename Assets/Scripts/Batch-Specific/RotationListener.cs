using UnityEngine;

public class RotationListener : MonoBehaviour
{
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
			Debug.Log("Rotating Clockwise");
			return;
		}

		Debug.Log("Rotating Counter-Clockwise");
	}
}
