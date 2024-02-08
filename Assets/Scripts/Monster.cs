using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public GameObject playerObj;
    public float detectSize = 10f;
    public float moveSpeed;

    private bool hasLineOfSight = false;
    
    public LayerMask what;



    private void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        Vector3 directionToTarget = playerObj.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.position =
            Vector2.MoveTowards(transform.position, playerObj.transform.position, moveSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (playerObj != null)
        {
            Collider2D detectCol = Physics2D.OverlapCircle(transform.position, detectSize, what);
            RaycastHit2D rayHit =
                Physics2D.Raycast(transform.position, playerObj.transform.position - transform.position);

            if (rayHit.collider != null)
            {
                hasLineOfSight = rayHit.collider.CompareTag("Player");
                if (hasLineOfSight)
                {
                    Debug.DrawRay(transform.position, playerObj.transform.position - transform.position, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, playerObj.transform.position - transform.position, Color.red);
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
