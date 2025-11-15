using UnityEngine;
using System.Collections; // Wajib: untuk IEnumerator

public class SpiderHealth : MonoBehaviour
{
    [Header("Health & Damage")]
    public int health = 3;
    public int playerDamage = 1; 
    
    [Header("Stomp Settings")]
    public float playerBounceForce = 10f; // Kekuatan pantulan Player saat stomp

    [Header("Death Effects")]
    public float deathFallSpeed = 3f;
    public float flashDuration = 0.1f;
    public float timeToFade = 1.5f;

    // Komponen Internal
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private SpiderPatrol patrolScript; 
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        patrolScript = GetComponent<SpiderPatrol>(); 
        anim = GetComponent<Animator>();
    }

    // Dipanggil oleh AttackArea.cs saat Player menyerang
    public void TakeDamage(int damageAmount)
    {
        if (health <= 0) return;
        
        health -= damageAmount;
        
        StartCoroutine(FlashEffect());

        if (health <= 0)
        {
            Die();
        }
    }

    // Dipanggil saat Player menabrak Laba-laba (stomp atau side hit)
    void OnCollisionEnter2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Player")) return;

    PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
    PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    // Hapus: PlayerAction playerAction (tidak diperlukan lagi)

    if (playerMovement == null || playerRb == null || playerHealth == null) return; 

    // --- LOGIKA UTAMA: STOMP VS SIDE HIT ---
    
    // Asumsi: Player.wasGrounded bernilai FALSE saat Player di udara
    if (!playerMovement.wasGrounded)
    {
        // KONDISI STOMP: Player di udara dan menabrak laba-laba
        // (Ini adalah logika yang Anda inginkan: Bunuh Musuh)
        
        // 1. Lakukan Player Bounce
        playerRb.velocity = new Vector2(playerRb.velocity.x, playerBounceForce);
        
        // 2. Laba-laba Mati
        Die();
    }
    else
    {
        // KONDISI SIDE HIT: Player berada di tanah (Player Terkena Damage pasif)
        
        // Panggil fungsi ambil damage pada Player
        playerHealth.TakeDamage(); 
    }
}
    
    // --- UTILITIES ---

    IEnumerator FlashEffect()
    {
        if (sr == null) yield break; 
        
        Color originalColor = sr.color;
        sr.color = Color.white; 
        yield return new WaitForSeconds(flashDuration); 
        sr.color = originalColor; 
    }

    // [KOREKSI STRUKTUR] FadeOut harus memiliki tipe IEnumerator
    IEnumerator FadeOut()
    {
        if (sr == null) yield break;
        
        float startAlpha = sr.color.a; 
        float timer = 0f;

        while (timer < timeToFade)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, timer / timeToFade);
            
            Color newColor = sr.color;
            newColor.a = newAlpha;
            sr.color = newColor;

            yield return null; 
        }
        // Pastikan alpha diatur ke 0
        Color finalColor = sr.color;
        finalColor.a = 0;
        sr.color = finalColor;
    }

    void Die()
    {
        // 1. Bekukan Animasi
        if (anim != null)
        {
            anim.enabled = false; 
        }
        
        // 2. Nonaktifkan kontrol dan collider
        if (patrolScript != null) patrolScript.enabled = false;
        if (col != null) col.enabled = false; 
        
        // 3. Flip Vertikal (Wajib)
        if (sr != null)
        {
            Vector3 scale = sr.transform.localScale;
            scale.y *= -1f; 
            sr.transform.localScale = scale; 
        }

        // 4. Atur Fisika untuk Jatuh
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.None; 
            rb.gravityScale = deathFallSpeed; 
            rb.drag = 0f;
            rb.angularDrag = 0f; 
        }
        
        // 5. Mulai Efek Menghilang (Fade Out)
        StartCoroutine(FadeOut()); 

        // 6. Hancurkan objek setelah durasi jatuh dan fade selesai
        Destroy(gameObject, 2f + timeToFade);
    }
}