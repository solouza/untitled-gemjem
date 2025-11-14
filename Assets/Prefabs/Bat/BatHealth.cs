using System.Collections;
using UnityEngine;

public class BatHealth : MonoBehaviour
{
    [Header("Health & Damage")]
    public int health = 3;
    public int playerDamage = 1; // [BARU] Damage yang diberikan ke Player
    public float deathFallSpeed = 3f; // Kekuatan gravitasi saat jatuh
    public float flashDuration = 0.1f; // Durasi efek flash putih
    
    [Header("Death Effects")]
    public float timeToFade = 1.5f; // Durasi menghilang
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Collider2D col;
    private BatPatrol patrolScript;
    private Transform spriteVisual;
    private Animator anim; // [BARU] Tambahkan referensi Animator

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        patrolScript = GetComponent<BatPatrol>();
        anim = GetComponent<Animator>(); // [BARU] Inisialisasi Animator
        spriteVisual = transform;
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;

        // Efek Visual: Flash Putih
        StartCoroutine(FlashEffect());

        if (health <= 0)
        {
            Die();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    // Cek apakah objek yang ditabrak adalah Player (Tag "Player" wajib!)
    if (collision.gameObject.CompareTag("Player"))
    {
        // Dapatkan script PlayerHealth dari objek Player
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            // Panggil fungsi TakeDamage pada Player
            playerHealth.TakeDamage(); 
            
            // [PENTING] Nonaktifkan Kelelawar Sejenak atau Tambahkan Cooldown
            // Agar Player tidak menerima damage berkali-kali dalam satu frame.
            // Paling sederhana: Hancurkan Kelelawar jika ini adalah kontak pertama.
            
            // Atau jika Kelelawar tidak boleh mati saat menyentuh:
            // Lakukan sesuatu untuk mencegah Kelelawar langsung memberikan damage lagi
            // Misalnya: col.enabled = false; StartCoroutine(ReactivateCollider(0.5f));
        }
    }
}
    IEnumerator FlashEffect()
    {
        Color originalColor = sr.color;
        // Ganti sr.color = Color.red menjadi Color.white untuk efek flash normal
        sr.color = Color.white; 

        yield return new WaitForSeconds(flashDuration);

        sr.color = originalColor;
    }

    IEnumerator FadeOut()
    {
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

        // Pastikan Alpha benar-benar 0 di akhir
        Color finalColor = sr.color;
        finalColor.a = 0;
        sr.color = finalColor;
    }

    void Die()
    {
        // [BARU] 1. Bekukan Animasi (Freeze)
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

        scale.y *= -1f; // Balik nilai Y (misalnya dari 1 menjadi -1)
        transform.localScale = scale; // Terapkan scale baru ke objek utama

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

        // [BARU] 2. Mulai Efek Menghilang (Fade Out)
        StartCoroutine(FadeOut()); 

        // [BARU] 3. Hancurkan objek setelah durasi jatuh dan fade selesai
        // Asumsi waktu jatuh/cooldown adalah 2 detik.
        Destroy(gameObject, 2f + timeToFade);
    }
}