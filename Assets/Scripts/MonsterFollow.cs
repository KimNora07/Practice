using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private GameObject player;
    public float detectSize = 10f;
    public int numGizmos = 10;

    private bool hasLineOfSight = false;

    public LayerMask what;
    Vector2 lastPlayerPosition;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (hasLineOfSight)
        {
            transform.position = Vector2.MoveTowards(transform.position, lastPlayerPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, player.transform.position - transform.position);
            Collider2D detectCol = Physics2D.OverlapCircle(transform.position, detectSize, what);
            if (detectCol != null)
            {
                if (hit.collider != null)
                {
                    hasLineOfSight = hit.collider.CompareTag("Player");
                    if (hasLineOfSight)
                    {
                        lastPlayerPosition = player.transform.position;
                        Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.green);
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectSize);
    }
}
