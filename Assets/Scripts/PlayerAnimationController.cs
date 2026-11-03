using UnityEngine;

/// <summary>
/// Drives the player's SpriteRenderer with frame-by-frame animation
/// based on PlayerController.CurrentState.
/// No Animator / AnimatorController asset required.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimationController : MonoBehaviour
{
    // ── Sprite Arrays (assigned in Inspector or via Scene YAML) ──────────
    [Header("Animation Frames")]
    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] runFrames;
    public Sprite[] crouchFrames;
    public Sprite[] jumpFrames;
    public Sprite[] hurtFrames;
    public Sprite[] dieFrames;

    [Header("Frame Rates (FPS per state)")]
    [SerializeField] private float idleFps    = 6f;
    [SerializeField] private float walkFps    = 10f;
    [SerializeField] private float runFps     = 14f;
    [SerializeField] private float crouchFps  = 6f;
    [SerializeField] private float jumpFps    = 10f;
    [SerializeField] private float hurtFps    = 8f;
    [SerializeField] private float dieFps     = 6f;

    // ── Private state ────────────────────────────────────────────────────
    private SpriteRenderer  sr;
    private PlayerController pc;

    private Sprite[]   currentFrames;
    private float      currentFps;
    private int        frameIndex;
    private float      timer;
    private bool       isDead;

    // ── Unity Lifecycle ──────────────────────────────────────────────────
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (isDead) return;

        Sprite[]  newFrames = PickFrames(pc.CurrentState);
        float     newFps    = PickFps(pc.CurrentState);

        // Switch animation set when state changes
        if (newFrames != currentFrames)
        {
            currentFrames = newFrames;
            currentFps    = newFps;
            frameIndex    = 0;
            timer         = 0f;
        }

        if (currentFrames == null || currentFrames.Length == 0) return;

        // Advance frame timer
        timer += Time.deltaTime;
        float frameInterval = 1f / Mathf.Max(currentFps, 0.1f);

        while (timer >= frameInterval)
        {
            timer -= frameInterval;
            frameIndex++;

            // Die animation: hold last frame
            if (pc.CurrentState == MovementState.Idle &&
                currentFrames == dieFrames)
            {
                frameIndex = currentFrames.Length - 1;
                isDead = true;
                break;
            }

            // Die state detected via frames
            if (currentFrames == dieFrames && frameIndex >= currentFrames.Length - 1)
            {
                frameIndex = currentFrames.Length - 1;
                isDead = true;
                break;
            }

            frameIndex %= currentFrames.Length;
        }

        sr.sprite = currentFrames[frameIndex];

        // Flip sprite based on facing direction
        sr.flipX = !pc.IsFacingRight;
    }

    // ── Helpers ──────────────────────────────────────────────────────────
    private Sprite[] PickFrames(MovementState state)
    {
        return state switch
        {
            MovementState.Idle       => (idleFrames   != null && idleFrames.Length   > 0) ? idleFrames   : null,
            MovementState.Walking    => (walkFrames   != null && walkFrames.Length   > 0) ? walkFrames   : null,
            MovementState.Sprinting  => (runFrames    != null && runFrames.Length    > 0) ? runFrames    : null,
            MovementState.Crouching  => (crouchFrames != null && crouchFrames.Length > 0) ? crouchFrames : null,
            MovementState.InAir      => (jumpFrames   != null && jumpFrames.Length   > 0) ? jumpFrames   : null,
            _                        => null
        };
    }

    private float PickFps(MovementState state)
    {
        return state switch
        {
            MovementState.Idle      => idleFps,
            MovementState.Walking   => walkFps,
            MovementState.Sprinting => runFps,
            MovementState.Crouching => crouchFps,
            MovementState.InAir     => jumpFps,
            _                       => idleFps
        };
    }

    /// <summary>
    /// Call this from a damage / health system to trigger the hurt animation once.
    /// </summary>
    public void PlayHurt()
    {
        StartCoroutine(PlayOnce(hurtFrames, hurtFps));
    }

    /// <summary>
    /// Call this from a health system when the player dies.
    /// </summary>
    public void PlayDie()
    {
        StartCoroutine(PlayOnceAndFreeze(dieFrames, dieFps));
    }

    private System.Collections.IEnumerator PlayOnce(Sprite[] frames, float fps)
    {
        if (frames == null || frames.Length == 0) yield break;
        float interval = 1f / Mathf.Max(fps, 0.1f);
        foreach (var frame in frames)
        {
            sr.sprite = frame;
            yield return new WaitForSeconds(interval);
        }
    }

    private System.Collections.IEnumerator PlayOnceAndFreeze(Sprite[] frames, float fps)
    {
        if (frames == null || frames.Length == 0) yield break;
        float interval = 1f / Mathf.Max(fps, 0.1f);
        foreach (var frame in frames)
        {
            sr.sprite = frame;
            yield return new WaitForSeconds(interval);
        }
        isDead = true;  // freeze on last frame
    }
}
