using UnityEngine;
using System.Collections;

public class RandomFarter : MonoBehaviour
{
    public GameObject fartObject;
    public float minFartWait = 5f;
    public float maxFartWait = 15f;
    
    private void Start()
    {
        StartCoroutine(FartRoutine());
    }
    
    IEnumerator FartRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minFartWait, maxFartWait);
            
            yield return new WaitForSeconds(waitTime);
            
            fartObject.SetActive(false);
            fartObject.SetActive(true);
        }
    }
    
}
