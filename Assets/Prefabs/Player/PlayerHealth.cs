using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI; // Wajib: untuk mengakses komponen UI seperti Text

// Gunakan TMPro jika kamu menggunakan TextMeshPro
// using TMPro; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Life System")]
    public int maxLives = 3;
    public int currentLives;
    public float respawnDelay = 2f;

    // Di PlayerHealth.cs

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f; // Durasi kebal setelah kena hit
    public float flashRate = 0.1f; // Kecepatan berkedip (blink)
    private bool isInvincible = false;
    private SpriteRenderer sr; // Wajib: untuk mengontrol kedipan
    
    [Header("Death Effects")]
    public float deathBounceForce = 8f; // Kekuatan lompatan awal saat mati
    public float deathAnimationTime = 1.5f; // Berapa lama waktu animasi jatuh sebelum respawn

    [Header("UI Reference")]
    // Pastikan kamu menggunakan Text atau TextMeshPro
    public TextMeshProUGUI livesText; // Ubah ke public TextMeshProUGUI jika kamu pakai TMPro
    
    [Header("Respawn")]
    // Koordinat tempat Player akan muncul kembali setelah mati
    public Vector3 respawnPoint; 

    void Awake()
    {
        // Atur nyawa saat game dimulai
        currentLives = maxLives;
        
        // Panggil fungsi untuk memperbarui tampilan UI saat pertama kali
        UpdateLivesUI();

        // Atur posisi awal sebagai titik respawn default
        respawnPoint = transform.position;
        sr = GetComponent<SpriteRenderer>();
    }

    // Fungsi ini dipanggil musuh (misalnya dari WormHealth.cs) saat tabrakan samping
    public void TakeDamage()
{
    if (isInvincible) return; // Cek status kebal
    if (currentLives <= 0) return;

    currentLives--;
    UpdateLivesUI();
    
    if (currentLives <= 0)
    {
        Die();
    }
    else
    {
        // Panggil animasi mati (Lompat & Jatuh) yang diikuti dengan Respawn
        StartCoroutine(DeathAnimationAndRespawn()); 
        
        // Penting: Mulai I-Frames segera
        StartCoroutine(InvincibilityCoroutine()); 
    }
}
    IEnumerator InvincibilityCoroutine()
{
    isInvincible = true;
    float timeElapsed = 0f;

    // Loop untuk durasi kebal
    while (timeElapsed < invincibilityDuration)
    {
        // Berkedip: Mengganti SpriteRenderer antara aktif dan nonaktif
        sr.enabled = !sr.enabled;
        
        // Tunggu sesuai kecepatan flash
        yield return new WaitForSeconds(flashRate);
        
        timeElapsed += flashRate;
    }

    // Akhiri Keadaan Kebal
    isInvincible = false;
    sr.enabled = true; // Pastikan sprite terlihat penuh kembali
}
    void Die()
    {
        // Tangani Logika Game Over (nanti)
        Debug.Log("GAME OVER");
        // Misalnya: Time.timeScale = 0;
    }

    IEnumerator DeathAnimationAndRespawn()
{
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    PlayerMovement movement = GetComponent<PlayerMovement>();
    Collider2D col = GetComponent<Collider2D>();
    
    // 1. Matikan Kontrol & Collider (Biarkan GameObject Tetap Aktif)
    if (movement != null) movement.enabled = false;
    if (col != null) col.enabled = false;
    // Tambahkan: Matikan Sprite (jika kamu ingin player menghilang saat jatuh)
    if (sr != null) sr.enabled = false; 
    
    // 2. Efek Mario 'Hop'
    if (rb != null)
    {
        rb.velocity = Vector2.zero; 
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(new Vector2(0, deathBounceForce), ForceMode2D.Impulse);
    }
    
    // 3. TUNGGU ANIMASI JATUH SELESAI
    yield return new WaitForSeconds(deathAnimationTime);
    
    // --- FASE RESPAWN (Reset & Teleport) ---
    
    // 4. Reset Posisi dan Fisika
    // Lakukan Teleportasi (Objek masih aktif)
    transform.position = respawnPoint; 

    // 5. Aktifkan kembali Sprite (jika dinonaktifkan di langkah 1)
    if (sr != null) sr.enabled = true;
    
    // 6. Reset komponen & fisika
    if (movement != null) movement.enabled = true;
    if (col != null) col.enabled = true;
    
    if (rb != null)
    {
        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
    }
    
    // 7. Beri I-Frames
    StartCoroutine(InvincibilityCoroutine()); 
}

    void UpdateLivesUI()
    {
        // Tampilkan hitungan nyawa (misalnya: x 3)
        if (livesText != null)
        {
            livesText.text = "x " + currentLives.ToString();
        }
    }
    
    // Fungsi untuk memperbarui titik respawn, misalnya saat Player mencapai Checkpoint
    public void SetRespawnPoint(Vector3 newPoint)
    {
        respawnPoint = newPoint;
    }
}