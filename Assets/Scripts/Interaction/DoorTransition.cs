using UnityEngine;

namespace Dreamland.Interaction
{
    public class DoorTransition : InteractableBase
    {
        [Header("Door Settings")]
        [SerializeField] private string doorName = "\uC9C0\uD558\uC2E4 \uCD9C\uC785\uBB38"; // 지하실 출입문
        [SerializeField] private Transform destinationPoint;
        [SerializeField] private bool isLocked = false;
        [SerializeField] private string requiredKeyName = "";

        private void Reset()
        {
            promptText = $"{doorName} \uD1B5\uACFC"; // 통과
        }

        public override string InteractionPrompt
        {
            get
            {
                if (isLocked)
                    return $"[\uC7A0\uAE40] {doorName} (\uC5F4\uCDC4 \uD544\uC694: {requiredKeyName})"; // [잠김] ... (열쇠 필요: ...)
                return $"[E] {doorName} \uC5F4\uACE0 \uC774\uB3D9"; // [E] ... 열고 이동
            }
        }

        public override bool CanInteract(GameObject interactor)
        {
            if (!base.CanInteract(interactor)) return false;
            return !isLocked;
        }

        public override void Interact(GameObject interactor)
        {
            if (isLocked)
            {
                Debug.Log($"[DoorTransition] {doorName}\uC774(\uAC00) \uC7A0\uACA8 \uC788\uC2B5\uB2C8\uB2E4.");
                return;
            }

            if (destinationPoint != null && interactor != null)
            {
                Debug.Log($"[DoorTransition] {doorName}\uC744(\uB97C) \uD1B5\uD574 {destinationPoint.name} \uC704\uCE58\uB85C \uC774\uB3D9\uD569\uB2C8\uB2E4.");
                interactor.transform.position = destinationPoint.position;
            }
            else
            {
                Debug.LogWarning("[DoorTransition] destinationPoint not set");
            }
        }

        public void Unlock()
        {
            isLocked = false;
        }
    }
}
