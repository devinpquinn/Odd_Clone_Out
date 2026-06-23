using UnityEngine;
using System.Collections;

public class DieAfterWait : MonoBehaviour
{
    public Animator anim;
    public float waitTime = 10f;

    private void Start()
    {
        StartCoroutine(DieAfterDelay());
    }

    private IEnumerator DieAfterDelay()
    {
        yield return new WaitForSeconds(waitTime);
        anim.SetBool("Die", true);
    }
}
