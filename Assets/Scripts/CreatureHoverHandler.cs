using UnityEngine;
using UnityEngine.Events;

public class CreatureHoverHandler : MonoBehaviour
{
    public UnityEvent onHoverStart;
    public UnityEvent onHoverEnd;
    
    public void StartHover()
    {
        onHoverStart.Invoke();
    }
    
    public void EndHover()
    {
        onHoverEnd.Invoke();
    }
}
