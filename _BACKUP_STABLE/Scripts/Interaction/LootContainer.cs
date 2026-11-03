using UnityEngine;

namespace Dreamland.Interaction
{
    public class LootContainer : InteractableBase
    {
        [Header("Loot Settings")]
        [SerializeField] private string containerName = "\uBC84\uB824\uC9C4 \uBCF4\uAE09 \uC0C1\uC790"; // 버려진 보급 상자
        [SerializeField] private bool isSearched = false;
        [SerializeField] private string[] possibleItems = new string[] { 
            "\uBBFC\uAC04 \uD1B5\uC870\uB9BC", // 민간 통조림
            "\uC624\uC5FC\uB418\uC9C0 \uC54A\uC740 \uC0DD\uC218", // 오염되지 않은 생수
            "\uBD95\uB300" // 붕대
        };

        private void Reset()
        {
            promptText = $"{containerName} \uC218\uC0C9"; // 수색
        }

        public override string InteractionPrompt
        {
            get
            {
                if (isSearched)
                    return $"{containerName} (\uC774\uBBF8 \uBE44\uC5B4\uC788\uC74C)"; // (이미 비어있음)
                return $"[E] {containerName} \uC218\uC0C9"; // [E] ... 수색
            }
        }

        public override bool CanInteract(GameObject interactor)
        {
            return base.CanInteract(interactor) && !isSearched;
        }

        public override void Interact(GameObject interactor)
        {
            if (isSearched) return;

            isSearched = true;
            Debug.Log($"[LootContainer] '{containerName}'\uC744(\uB97C) \uC218\uC0C9\uD588\uC2B5\uB2C8\uB2E4!");

            if (possibleItems != null && possibleItems.Length > 0)
            {
                string acquired = possibleItems[Random.Range(0, possibleItems.Length)];
                Debug.Log($"[LootContainer] \uD68D\uB4DD \uC544\uC774\uD15C: {acquired}");
            }
        }
    }
}
