using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private float damage = 30.0f;
    [SerializeField] private float speed = 1.1f;
    [SerializeField] private float sightRange = 3.5f;
    [SerializeField] private float damagePause = 1.0f;

    [SerializeField] private float rotateSpeed = 1.0f;

    private float damagePauseTimer;
    private Animator animator;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        this.animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, this.transform.position);

        if (distanceToPlayer < sightRange)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);

            Vector3 direction = (player.transform.position - this.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        SetAnimationState(distanceToPlayer < sightRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damagePauseTimer == 0.0f)
            {
                var character = other.GetComponent<Character>();
                character.Damage(this.damage * Time.deltaTime);
                damagePauseTimer = damagePause;
            }
            else
            {
                damagePauseTimer -= Time.fixedDeltaTime;
            }
        }
    }

    void SetAnimationState(bool walking)
    {
        this.animator.SetBool("IsWalking", walking);
    }


}
