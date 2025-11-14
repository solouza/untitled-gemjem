using System.Collections;
using UnityEngine;

public class WormHealth : MonoBehaviour
{
    [Header("Health & Damage")]
    public int health = 1; // Disarankan 1 HP untuk mekanisme injak
    public float playerBounceForce = 8f; // Kekuatan pantulan Player saat injak
    
    [Header("Death Effects")]
    public float deathFallSpeed = 3f; // Kekuatan gravitasi saat jatuh
    public float flashDuration = 0.1f; // Durasi efek flash
    public float timeToFade = 1.5f; // Durasi menghilang

    // Komponen Internal
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private WormPatrol patrolScript;
    private Transform spriteVisual; // Biasanya sama dengan transform utama
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        patrolScript = GetComponent<WormPatrol>(); 
        anim = GetComponent<Animator>();

        // Asumsi visual sprite berada di objek root (tempat script ini berada)
        spriteVisual = transform; 
    }

    // Fungsi Stomp (Injak)
void OnCollisionEnter2D(Collision2D collision)
{
    // Hanya peduli pada tabrakan dengan Player
    if (!collision.gameObject.CompareTag("Player")) return;
    
    // Dapatkan script PlayerMovement dan Rigidbody Player
    PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
    
    // Safety check: jika komponen PlayerMovement tidak ditemukan, hentikan
    if (playerMovement == null || playerRb == null) return; 

    // --- LOGIKA UTAMA: MEMBEDAKAN STOMP VS SIDE HIT ---
    
    if (!playerMovement.wasGrounded)
    {
        // KONDISI STOMP: Player sedang di udara dan menabrak cacing
        
        // 1. Lakukan Player Bounce
        playerRb.velocity = new Vector2(playerRb.velocity.x, playerBounceForce);
        
        // 2. Matikan Cacing
        Die();
    }
    else
{
    // KONDISI SIDE HIT: Player berada di tanah, harus ambil damage
    
    // Dapatkan script PlayerHealth
    PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
    
    if (playerHealth != null)
    {
        playerHealth.TakeDamage(); // Panggil fungsi ambil damage
    }
}
}

    // --- FUNGSI KEMATIAN ---
    
    void Die()
    {
        if (anim != null)
    {
        anim.enabled = false; 
    }
        // Nonaktifkan gerakan patroli
        if (patrolScript != null)
        {
            patrolScript.enabled = false;
        }
        
        // Flip Vertikal
        Vector3 scale = spriteVisual.localScale;
        scale.y *= -1f; 
        spriteVisual.localScale = scale;

        // Atur Fisika untuk Jatuh
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.None; // Lepaskan kunci
            rb.gravityScale = deathFallSpeed; // Aktifkan gravitasi
            rb.drag = 0f;
            rb.angularDrag = 0f; // Jatuh bebas
        }
        
        // Matikan Collision Agar Player Bisa Lewat (dan tidak stuck)
        if (col != null)
        {
            col.enabled = false; 
        }

        // Mulai efek menghilang dan hancurkan
        StartCoroutine(FlashEffect()); 
        StartCoroutine(FadeOut()); 

        Destroy(gameObject, 1f + timeToFade);
    }
    
    IEnumerator FlashEffect()
    {
        if (sr == null) yield break; // Safety check
        
        Color originalColor = sr.color;
        sr.color = Color.white; 
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    IEnumerator FadeOut()
    {
        if (sr == null) yield break; // Safety check

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

        // Pastikan Alpha benar-benar 0
        Color finalColor = sr.color;
        finalColor.a = 0;
        sr.color = finalColor;
    }
}