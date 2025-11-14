using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private int damage = 3;
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Pastikan objek yang ditabrak punya Tag "Enemy"
        if (collider.CompareTag("Enemy"))
        {
            // Coba ambil script EnemyHealth dari objek yang tertabrak
            BatHealth enemy = collider.GetComponent<BatHealth>();
            
            // Jika script ditemukan, panggil fungsi TakeDamage
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Script ini tidak melakukan apa-apa di Start()
    }

    // Update is called once per frame
    void Update()
    {
        // Script ini tidak melakukan apa-apa di Update()
    }
}