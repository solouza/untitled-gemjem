using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    private GameObject attackArea = default;
    private bool attacking = false;
    private float timeToAttack = 0.22f;
    private float timer = 0f;
    private PlayerMovement movement;
    private Animator animator;

    void Start()
    {
        attackArea = transform.GetChild(0).gameObject;
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !attacking)
        {
            Attack();
            Debug.Log("Attacking");
        }

        if (attacking)
        {
            timer += Time.deltaTime;

            if (timer >= timeToAttack)
            {
                timer = 0;
                attacking = false;
                attackArea.SetActive(attacking);
            }
        }
    }

    private void Attack()
    {
        attacking = true;
        attackArea.SetActive(attacking);
        animator.SetTrigger("isAttacking");

        if (movement != null)
            movement.canMove = false;

        StartCoroutine(WaitForAttackToFinish());
    }

    private IEnumerator WaitForAttackToFinish()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength);

        attackArea.SetActive(false);
        attacking = false;

        if (movement != null)
            movement.canMove = true;
    }
}
