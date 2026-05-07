using UnityEngine;

public class StaggerAnimation : MonoBehaviour
{
    public string clipName = "Idle_A";

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(clipName, 0, Random.value);
        }
    }
}
