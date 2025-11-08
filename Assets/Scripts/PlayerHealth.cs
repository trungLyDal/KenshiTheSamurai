using UnityEngine;
using System.Collections; // Needed for the IEnumerator/Coroutine

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Components")]
    // Added references to components needed for controlling the player on death/hit
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D mainCollider;
    private MonoBehaviour playerMovementScript; // Use a generic MonoBehaviour reference

    [Header("Damage Settings")]
    private bool isInvulnerable = false;
    public float invulnerabilityTime = 0.5f; // Time player flashes/can't take damage
    public float deathCleanupDelay = 1.5f; // Time to wait for the death animation

    void Start()
    {
        currentHealth = maxHealth;
        // Get required components
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<Collider2D>(); // Assuming the main player collider is here
        
        // IMPORTANT: Replace 'PlayerMovement' with the actual name of your player movement script!
        playerMovementScript = GetComponent<MonoBehaviour>(); // This needs to be refined.
        // Example: playerMovementScript = GetComponent<PlayerMovementScript_Name>(); 

        // Fail-safe check
        if (animator == null || rb == null || mainCollider == null)
        {
             Debug.LogError("PlayerHealth is missing a required component (Animator, Rigidbody2D, or Collider2D) on the GameObject!");
        }
    }

    public void TakeDamage(int damage)
    {
        // Check if player can take damage or is already dying
        if (isInvulnerable || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage! Remaining Health: " + currentHealth);

        // Trigger the isHurt animation
        if (animator != null)
        {
             // Ensure 'isHurt' is a Trigger parameter in your Animator!
             animator.SetTrigger("isHurt"); 
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Start invulnerability frames
            StartCoroutine(BecomeTemporarilyInvulnerable());
        }
    }

    void Die()
{
    currentHealth = 0;
    Debug.Log("Game Over! Player died.");
    
    // 1. Halt Player Functionality (Crucial Steps)
    MonoBehaviour movement = GetComponent<MonoBehaviour>(); 
    if (movement != null) movement.enabled = false; 

    // Stop all Rigidbody movement and freeze the body's position
    if (rb != null) 
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Lock physics
    }
    
    // 2. Trigger the Death Animation
    if (animator != null)
    {
        // Stop isHurt if it's a Bool that was still active (safe practice)
        // If 'isHurt' is a Trigger, this line is harmless.
        // animator.SetBool("isHurt", false); 

        animator.SetBool("isDead", true); 
    }

    // ⭐ THE CRITICAL FIX: Wait one frame for the Animator to fully transition.
    // This prevents the Coroutine from immediately checking the delay timer.
    StartCoroutine(HandleDeathCleanup());
}

// Updated Coroutine to include the single-frame wait
// Coroutine to handle waiting for the animation before scene cleanup
IEnumerator HandleDeathCleanup()
{
    // Wait for the duration of your death animation (1.067s, 1.2s is a safe value)
    yield return new WaitForSeconds(deathCleanupDelay); 
    
    // --- FINAL FIX FOR CUT-OFF ANIMATION ---
    if (animator != null)
    {
        // 1. Manually turn OFF the isDead parameter. 
        // This forces the Animator to take the Death -> Exit transition.
        animator.SetBool("isDead", false);
    }
    
    // 2. Wait one final frame for the Animator to process the exit transition.
    yield return null; 
    
    // --- Cleanup ---
    // ONLY NOW, after the animation exit is complete, disable components and the GameObject.
    if (mainCollider != null) mainCollider.enabled = false;
    if (rb != null) rb.isKinematic = false;

    // Final action: Hide player object and/or trigger game over
    gameObject.SetActive(false); 
}
    
    // Coroutine for invulnerability
    IEnumerator BecomeTemporarilyInvulnerable()
    {
        isInvulnerable = true;

        // --- Visual Flashing Example (Optional) ---
        // SpriteRenderer sr = GetComponent<SpriteRenderer>();
        // if (sr != null)
        // {
        //     for (int i = 0; i < 5; i++)
        //     {
        //         sr.enabled = !sr.enabled;
        //         yield return new WaitForSeconds(invulnerabilityTime / 10);
        //     }
        //     sr.enabled = true;
        // }
        // ----------------------------------------

        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false;
    }
}