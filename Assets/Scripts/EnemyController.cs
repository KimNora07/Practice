using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.5f;
    public float avoidanceDistanceMultiplier = 1f;
    public LayerMask obstacleLayer;

    private Transform player;
    private Rigidbody2D rb;
    private bool moveLeft = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            float colliderWidth = GetComponent<Collider2D>().bounds.size.x;
            float raycastDistance = colliderWidth * avoidanceDistanceMultiplier;

            // 레이캐스트의 경로를 Scene 뷰에 표시 (선)
            Debug.DrawRay(transform.position, direction * raycastDistance, Color.red);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleLayer);
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
            {
                moveLeft = !moveLeft;
                rb.velocity = (moveLeft ? Vector2.left : Vector2.right) * moveSpeed;
            }
            else
            {
                rb.velocity = direction * moveSpeed * Time.deltaTime;
            }

        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

}
