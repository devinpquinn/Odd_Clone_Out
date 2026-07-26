using UnityEngine;
using System.Collections;

public class DizzyHandler : MonoBehaviour
{
    public Animator anim;
    public float dizzyDelay = 0.5f;
    public float dizzyDuration = 3f;
    
    private Coroutine dizzyCoroutine;
    
    public void StartDizzy()
    {
        if (dizzyCoroutine != null)
        {
            StopCoroutine(dizzyCoroutine);
        }
        
        
        dizzyCoroutine = StartCoroutine(DizzyCoroutine());
    }
    
    IEnumerator DizzyCoroutine()
    {
        yield return new WaitForSeconds(dizzyDelay);
        
        anim.Play("Eyes_Spin", 1, 0f);
        
        yield return new WaitForSeconds(dizzyDuration);
        
        anim.Play("Eyes_Blink", 1, 0f);
    }
}
