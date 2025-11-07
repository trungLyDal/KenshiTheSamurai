using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float deathAnimationTime = 1f; 
    
    private int currentHealth;
    private Animator animator;
    private Rigidbody2D rb; // <-- ADD THIS LINE
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody2D>(); // <-- ADD THIS LINE
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        Debug.Log("Enemy took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("TakeHit");
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy has been defeated!");
        animator.SetTrigger("Death");

        // --- THIS IS THE FIX ---
        // This freezes the enemy in place and disables all physics, including gravity.
        rb.bodyType = RigidbodyType2D.Static;
        // --- END FIX ---
        
        // Now it's safe to disable the collider
        GetComponent<Collider2D>().enabled = false;
        
        // (Optional) Disable the patrol script so it stops thinking
        // GetComponent<EnemyPatrol>().enabled = false;
        
        Destroy(gameObject, deathAnimationTime);
    }
}