using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    private float mobileInputX = 0f; // Variabel untuk menyimpan input sentuhan
    private float horizontal;
    public float speed = 4f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    private PlayerHealth playerHealth;
    public bool canMove = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    [Header("Jump Settings")]
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    public bool wasGrounded;

    void Awake()
{
    // ... (inisialisasi komponen lain) ...
    playerHealth = GetComponent<PlayerHealth>(); // [BARU] Ambil referensi PlayerHealth
}
    void Update()
{
    if (!canMove)
    {
        return;
    }
    
    horizontal = mobileInputX; 
    if (horizontal == 0f) // Jika tidak ada sentuhan UI, baru cek keyboard/gamepad
    {
        horizontal = Input.GetAxisRaw("Horizontal");
    }

    // [KOREKSI LOGIKA LOMPAT]
    if (Input.GetButtonDown("Jump") && IsGrounded())
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        animator.SetBool("isJumping", true);
        
        // [FIX SPAMMING] Panggil suara hanya di sini
        if (playerHealth != null)
        {
            playerHealth.PlayJumpSFX(); 
        }
    }

    // --- (Hapus semua kode PlayJumpSFX yang ada di luar blok ini) ---
    
    animator.SetFloat("speed", Mathf.Abs(horizontal));
    bool grounded = IsGrounded();

    if (grounded && !wasGrounded)
    {
        animator.SetBool("isJumping", false);
    }

    wasGrounded = grounded;

    // Tambahkan kembali logika lowJumpMultiplier jika diperlukan

    Flip();
}

    private void FixedUpdate()
{
    // A. LOGIKA UTAMA: Gerakan Normal
    if (canMove) 
    {
        // Terapkan kecepatan horizontal normal (Input aktif)
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }
    // B. LOGIKA STUCK/FREEZE KETIKA ATTACK DI DARAT
    else if (!canMove && IsGrounded()) 
    {
        // Jika kontrol terkunci DAN Player di tanah: Hentikan pergerakan X
        rb.velocity = new Vector2(0, rb.velocity.y); 
    }
    // C. JIKA !canMove DAN DI UDARA: 
    // Kita lewati kedua blok di atas, yang secara otomatis mempertahankan nilai
    // rb.velocity.x saat ini (momentum melayang) dan membiarkan gravitasi bekerja.
    
    // D. LOGIKA GRAVITASI (Lebih Baik Lompat Rendah/Tinggi)
    if (rb.velocity.y < 0)
    {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }
    
    // Catatan: Jika kamu ingin lompatan tinggi/rendah yang sudah dikomentari, kamu bisa
    // mengaktifkan kembali kodenya di sini.
}

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(GroundCheck.position, 0.2f, GroundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal <0f || !isFacingRight && horizontal >0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    public void SetMobileInput(float direction)
{
    mobileInputX = direction; // direction akan diisi 1 (kanan) atau -1 (kiri)
}

public void StopMobileInput()
{
    mobileInputX = 0f;
}
public void MobileJump()
{
    // Cek apakah Player berada di tanah sebelum melompat
    if (IsGrounded())
    {
        // Terapkan gaya lompat
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        animator.SetBool("isJumping", true);
        
        // Putar Suara Lompat
        if (playerHealth != null)
        {
            playerHealth.PlayJumpSFX(); 
        }
    }
}
}
