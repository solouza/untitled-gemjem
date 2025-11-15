using UnityEngine;

public class LavaRise : MonoBehaviour
{
    [Header("Movement")]
    public float riseSpeed = 0.5f; // Kecepatan Lava naik per detik (Y unit/detik)
    
    [Header("Optional Acceleration")]
    public bool enableAcceleration = false;
    public float accelerationRate = 0.01f; // Seberapa cepat Lava bertambah cepat

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("LavaRise membutuhkan Rigidbody2D di objek Tilemap Lava!");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
{
    // Cek Tag Player
    if (other.CompareTag("Player"))
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        
        if (playerHealth != null)
        {
            // [PERBAIKAN] Panggil TakeDamage() untuk mengurangi 1 nyawa
            // Fungsi ini akan secara otomatis memicu Respawn atau Game Over (jika lives = 0)
            playerHealth.TakeDamage(); 
        }
    }
}
    void FixedUpdate()
    {
        // Pastikan Rigidbody ada dan sudah disetel ke Kinematic
        if (rb != null)
        {
            // Terapkan kecepatan naik (velocity Y positif)
            rb.velocity = new Vector2(0f, riseSpeed);
            
            // Logika Akselerasi
            if (enableAcceleration)
            {
                // Tambah kecepatan naik seiring waktu
                riseSpeed += accelerationRate * Time.fixedDeltaTime; 
            }
        }
    }
}