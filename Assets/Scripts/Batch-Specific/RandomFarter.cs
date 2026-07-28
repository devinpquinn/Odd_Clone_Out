using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomFarter : MonoBehaviour
{
    public GameObject fartObject;
    public float minFartWait = 5f;
    public float maxFartWait = 15f;

    private int lastFartIndex = -1;
    
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
            
            int fartIndex;
            do
            {
                fartIndex = Random.Range(1, 4);
            } 
            while (fartIndex == lastFartIndex);
            
            lastFartIndex = fartIndex;
            SoundManager.PlaySound("Fart_" + fartIndex);
        }
    }
    
}
