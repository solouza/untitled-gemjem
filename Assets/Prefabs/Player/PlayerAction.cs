using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject attackArea = default;
    private bool attacking = false;
    // Hapus: private float timeToAttack = 0.22f;
    // Hapus: private float timer = 0f;
    private PlayerMovement movement;
    private Animator animator;
    public float attackWindupTime = 0.5f; // Durasi JEDA sebelum HITBOX aktif

    void Start() {
    // Cek apakah ada anak di Player
    if (transform.childCount > 0)
    {
        attackArea = transform.GetChild(0).gameObject;
    }
    else
    {
        Debug.LogError("Player tidak memiliki AttackArea sebagai child!");
    }
    
    animator = GetComponent<Animator>();
    if (animator == null)
    {
        Debug.LogError("Komponen Animator tidak ditemukan di Player!");
    }

        movement = GetComponent<PlayerMovement>();
    
    attackArea.SetActive(false);
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !attacking)
        {
            Attack();
            Debug.Log("Attacking");
        }

        // HAPUS SEMUA LOGIKA TIMER YANG ADA DI SINI
        /* if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        } */
    }

    // Di PlayerAction.cs

private void Attack()
{
    attacking = true;
    
    // 1. Mulai animasi visual (windup) secara instan
    animator.SetTrigger("isAttacking"); 

    // 2. Bekukan pergerakan player
    if (movement != null)
        movement.canMove = false;

    // 3. Mulai Coroutine untuk mengelola timing dan aktivasi hitbox
    StartCoroutine(WaitForAttackToFinish());
}

private IEnumerator WaitForAttackToFinish()
{
    // [LANGKAH 1: DELAY/WINDUP] 
    // Jeda di sini. Hitbox masih mati selama waktu windup ini.
    yield return new WaitForSeconds(attackWindupTime);

    // --- HITBOX AKTIF ---
    
    // 2. AKTIFKAN HITBOX (Ini akan terjadi SETELAH jeda 0.xx detik)
    if (attackArea != null)
        attackArea.SetActive(true);

    // 3. TENTUKAN WAKTU COOLDOWN (Sisa Waktu Hitbox Aktif)
    
    // Ambil total panjang animasi
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    float animationLength = stateInfo.length;
    
    // Hitung sisa waktu tunggu (Total Animasi - Waktu Windup)
    float hitDuration = animationLength - attackWindupTime;
    
    // Pastikan durasi tidak negatif (minimal 0.01s)
    if (hitDuration < 0) hitDuration = 0.01f; 

    // Tunggu sisa durasi serangan
    yield return new WaitForSeconds(hitDuration); 

    // --- RESET ---
    
    // 4. DEAKTIFKAN HITBOX
    if (attackArea != null)
        attackArea.SetActive(false);
    
    // 5. Kembalikan kontrol
    attacking = false;

    if (movement != null)
        movement.canMove = true;
}
}