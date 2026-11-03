using UnityEngine;

namespace Dreamland.Interaction
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] protected string promptText = "\uC0C1\uD638\uC791\uC6A9"; // 상호작용
        [SerializeField] protected bool isInteractable = true;
        [SerializeField] protected Vector3 promptOffset = new Vector3(0, 1.2f, 0);

        public virtual string InteractionPrompt => $"[E] {promptText}";
        public virtual Vector3 InteractionPosition => transform.position + promptOffset;

        public virtual bool CanInteract(GameObject interactor)
        {
            return isInteractable && gameObject.activeInHierarchy;
        }

        public abstract void Interact(GameObject interactor);

        public virtual void SetInteractable(bool state)
        {
            isInteractable = state;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + promptOffset, 0.2f);
        }
    }
}
