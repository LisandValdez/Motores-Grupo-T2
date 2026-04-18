using UnityEngine;

public abstract class ActivableBase : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Activate();
    }
    protected abstract void Activate();
}