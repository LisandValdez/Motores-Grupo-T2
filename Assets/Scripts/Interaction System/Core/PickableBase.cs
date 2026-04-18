using UnityEngine;

public abstract class PickableBase : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Pick();
    }
    protected abstract void Pick();
}