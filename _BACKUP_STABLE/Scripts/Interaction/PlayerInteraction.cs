using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dreamland.Interaction
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float interactionRadius = 2.0f;
        [SerializeField] private LayerMask interactableLayer = ~0;
        [SerializeField] private Vector2 detectionOffset = Vector2.zero;

        [Header("UI Feedback (Debug / OnGUI)")]
        [SerializeField] private bool showOnGUIFeedback = true;

        public event Action<IInteractable> OnCurrentTargetChanged;

        public IInteractable CurrentTarget { get; private set; }

        private readonly Collider2D[] overlapBuffer = new Collider2D[16];

        private void Update()
        {
            UpdateInteractionTarget();
            HandleInput();
        }

        private void UpdateInteractionTarget()
        {
            Vector2 origin = (Vector2)transform.position + detectionOffset;
            int count = Physics2D.OverlapCircleNonAlloc(origin, interactionRadius, overlapBuffer, interactableLayer);

            IInteractable closest = null;
            float closestDistanceSqr = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                Collider2D col = overlapBuffer[i];
                if (col.gameObject == gameObject) continue;

                IInteractable interactable = col.GetComponent<IInteractable>();
                if (interactable == null)
                {
                    interactable = col.GetComponentInParent<IInteractable>();
                }

                if (interactable != null && interactable.CanInteract(gameObject))
                {
                    float distSqr = (col.transform.position - transform.position).sqrMagnitude;
                    if (distSqr < closestDistanceSqr)
                    {
                        closestDistanceSqr = distSqr;
                        closest = interactable;
                    }
                }
            }

            if (CurrentTarget != closest)
            {
                CurrentTarget = closest;
                OnCurrentTargetChanged?.Invoke(CurrentTarget);
            }
        }

        private void HandleInput()
        {
            if (CurrentTarget == null) return;

            Keyboard kb = Keyboard.current;
            if (kb == null) return;

            if (kb.eKey.wasPressedThisFrame)
            {
                CurrentTarget.Interact(gameObject);
                UpdateInteractionTarget();
            }
        }

        private void OnGUI()
        {
            if (!showOnGUIFeedback || CurrentTarget == null || Camera.main == null) return;

            Vector3 worldPos = CurrentTarget.InteractionPosition;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            if (screenPos.z < 0) return;

            float guiY = Screen.height - screenPos.y;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.white;

            string prompt = CurrentTarget.InteractionPrompt;
            Vector2 size = style.CalcSize(new GUIContent(prompt)) + new Vector2(24, 12);
            Rect rect = new Rect(screenPos.x - size.x / 2, guiY - size.y / 2, size.x, size.y);

            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.1f, 0.12f, 0.15f, 0.9f);
            GUI.Box(rect, prompt, style);
            GUI.backgroundColor = prevColor;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere((Vector2)transform.position + detectionOffset, interactionRadius);
        }
    }
}
