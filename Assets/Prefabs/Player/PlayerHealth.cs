using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Life System")]
    //public int maxLives = 3;
    //public int currentLives;
    public int maxLives = 3;
    private int currentLives;
    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;
    public float flashRate = 0.1f;
    public float deathAnimationTime = 1.5f; // Durasi total jatuh
    public float deathBounceForce = 8f;
    
    [Header("UI Reference")]
    public TextMeshProUGUI livesText;
    
    [Header("Respawn")]
    public Vector3 respawnPoint; 

    private bool isInvincible = false;
    private SpriteRenderer sr; 

    void Awake()
    {
        currentLives = GameData.Instance.currentLives;
        respawnPoint = transform.position;
        sr = GetComponent<SpriteRenderer>();
        UpdateLivesUI();
    }

    public void TakeDamage()
{
    if (isInvincible) return; // Cek status kebal
    if (GameData.Instance.currentLives <= 0) return; // Cek status di data persisten

    // 1. KURANGI NYAWA di objek PERSISTEN
    GameData.Instance.currentLives--; 

    // 2. Sinkronkan nilai lokal untuk UI & Logika Game Over
    currentLives = GameData.Instance.currentLives;
    UpdateLivesUI(); 

    // 3. Cek apakah ini Game Over Final
    if (currentLives <= 0)
    {
        // Game Over Total: Reset nyawa di GameData untuk game berikutnya
        GameData.Instance.ResetLives(); 
    }
    
    // 4. PICU SEQUENCE GAME OVER (Reload Scene)
    // Cuma dipanggil SATU KALI
    StartCoroutine(GameOverAndReload()); 
}
    
    // --- KEMATIAN LETHAL (GAME OVER) ---
    IEnumerator GameOverAndReload()
    {
        StartCoroutine(InvincibilityCoroutine());
        // 1. Matikan visual dan kontrol
        DisableAllControls();

        // 2. Efek Mario 'Hop'
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.None;
            rb.AddForce(new Vector2(0, deathBounceForce), ForceMode2D.Impulse);
        }

        // 3. Tunggu animasi jatuh selesai
        yield return new WaitForSeconds(deathAnimationTime);
        
        // 4. Reload Scene (Index 0 sudah dikonfirmasi)
        Debug.Log("GAME OVER! Memuat ulang level 1...");
        SceneManager.LoadScene(0); 
    }

    // --- KEMATIAN NON-LETHAL (RESPAWN) ---
    
    // --- UTILITIES ---
    
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        float timeElapsed = 0f;

        while (timeElapsed < invincibilityDuration)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(flashRate);
            timeElapsed += flashRate;
        }
        isInvincible = false;
        if (sr != null) sr.enabled = true;
        Debug.Log("I-Frames SELESAI."); //
    }

    public void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "x " + currentLives.ToString();
        }
    }

    private void DisableAllControls()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        Collider2D col = GetComponent<Collider2D>();

        if (movement != null) movement.enabled = false;
        if (col != null) col.enabled = false;
    }

    private void ResetControlsAndPhysics()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Collider2D col = GetComponent<Collider2D>();

        if (movement != null) movement.enabled = true;
        if (col != null) col.enabled = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; 
        }
    }
    public void InitiateGameOver()
{
    // Pastikan status nyawa disetel ke nol (agar fungsi GameOverSequence tahu ini Game Over)
    currentLives = 0;
    UpdateLivesUI(); 
    
    // Panggil Coroutine yang menangani visual jatuh dan reload
    StartCoroutine(GameOverAndReload()); 
}
}