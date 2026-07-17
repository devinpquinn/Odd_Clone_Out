using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomFarter : MonoBehaviour
{
    public GameObject fartObject;
    public float minFartWait = 5f;
    public float maxFartWait = 15f;
    public AudioSource fartAudioSource;
    public List<AudioClip> fartAudioClips;

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
                fartIndex = Random.Range(0, fartAudioClips.Count);
            } 
            while (fartAudioClips.Count > 1 && fartIndex == lastFartIndex);
            
            lastFartIndex = fartIndex;
            fartAudioSource.PlayOneShot(fartAudioClips[fartIndex]);
        }
    }
    
}
