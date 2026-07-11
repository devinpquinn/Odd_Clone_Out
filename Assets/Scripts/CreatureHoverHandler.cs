using UnityEngine;
using UnityEngine.Events;

public class CreatureHoverHandler : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    private string creatureLayerName = "Creature";
    private string outlineLayerName = "Outline";
    public UnityEvent onHoverStart;
    public UnityEvent onHoverEnd;
    private bool isHovered = false;
    
    public void StartHover()
    {
        onHoverStart.Invoke();
    }
    
    public void EndHover()
    {
        onHoverEnd.Invoke();
    }
}
