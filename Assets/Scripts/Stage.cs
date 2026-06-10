using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Odd Clone Out/Stage")]
public class Stage : ScriptableObject
{
    public List<Batch> batches = new List<Batch>();
}