using UnityEngine;

public class StaggerAnimation : MonoBehaviour
{
    public string clipName = "Idle_A";
    public int layer = 0;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(clipName, layer, Random.value);
        }
    }
}
