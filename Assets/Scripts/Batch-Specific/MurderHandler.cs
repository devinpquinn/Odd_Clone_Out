using UnityEngine;
using System.Collections;

public class MurderHandler : MonoBehaviour
{
    public float waitBeforeMurder;
    public float murderFadeOutTime;
    public float murderDuration;
    public float murderFadeInTime;
    
    public SkinnedMeshRenderer murdererMesh;
    public Mesh murdererMurderMesh;

    private Coroutine murderRoutine;

    private void Start()
    {
        CreatureSelector.Instance.isLive = false;
        murderRoutine = StartCoroutine(MurderSequence());
    }

    private void OnDestroy()
    {
        if (murderRoutine != null)
        {
            StopCoroutine(murderRoutine);
            murderRoutine = null;
        }

        if (CreatureSelector.Instance != null)
        {
            CreatureSelector.Instance.isLive = true;
        }
    }

    private IEnumerator MurderSequence()
    {
        yield return new WaitForSeconds(waitBeforeMurder);

        FadeManager.FadeTo(1f, Mathf.Max(0f, murderFadeOutTime));

        yield return new WaitForSeconds(murderFadeOutTime);
        
        if (murdererMesh != null && murdererMurderMesh != null)
            murdererMesh.sharedMesh = murdererMurderMesh;

        Animator victimAnim = GameObject.Find("Crow").GetComponent<Animator>();
        victimAnim.Play("Death", 0, 0f);
        victimAnim.Play("Eyes_Dead", 1, 0f);

        yield return new WaitForSeconds(murderDuration);

        FadeManager.FadeTo(0f, Mathf.Max(0f, murderFadeInTime));

        yield return new WaitForSeconds(murderFadeInTime);

        CreatureSelector.Instance.isLive = true;
        murderRoutine = null;
    }
}
