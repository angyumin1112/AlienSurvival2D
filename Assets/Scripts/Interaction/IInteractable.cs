using UnityEngine;

namespace Dreamland.Interaction
{
    public interface IInteractable
    {
        string InteractionPrompt { get; }
        bool CanInteract(GameObject interactor);
        void Interact(GameObject interactor);
        Vector3 InteractionPosition { get; }
    }
}
