using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    private float horizontal;
    public float speed = 4f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    public bool canMove = true;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    [Header("Jump Settings")]
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;

    public bool wasGrounded;

    void Update()
    {
        if (!canMove)
        {
            //animator.SetFloat("speed", 0);
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            animator.SetBool("isJumping", true);
        }

        //if (rb.velocity.y < 0)
        //rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;

        animator.SetFloat("speed", Mathf.Abs(horizontal));
        bool grounded = IsGrounded();

        if (grounded && !wasGrounded)
        {
            animator.SetBool("isJumping", false);
        }

        wasGrounded = grounded;

       // if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        //{
            //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        //}

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
}
