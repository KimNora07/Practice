using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public List<Node> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;
    public Transform player;
    public Transform gizmo;
    public GameObject saveGizmo;
    public Transform savePlayer;
    private GameObject cloneGizmo;
    [SerializeField] private float moveSpeed = 3f;

    public float detectSize = 3f;
    public float detectTime = 5f;
    public bool firstDetect = false;
    public bool firstGizmo = false;
    private bool hasLineOfSight = false;
    private bool hasLineOfGizmo = false;
    public LayerMask what;

    int sizeX, sizeY;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;
    Collider2D detectCol;

    public Vector2 FinalNodePos;

    private void Update()
    {
        //Debug.Log((int)detectTime);

        startPos = Vector2Int.RoundToInt(transform.position);
        targetPos = Vector2Int.RoundToInt(player.position);

        bottomLeft = new Vector2Int(Mathf.Min(startPos.x, targetPos.x) - 5, Mathf.Min(startPos.y, targetPos.y) - 5);
        topRight = new Vector2Int(Mathf.Max(startPos.x, targetPos.x) + 5, Mathf.Max(startPos.y, targetPos.y) + 5);

        PathFinding();

        if (FinalNodeList.Count > 1)
        {
            FinalNodePos = new Vector2(FinalNodeList[1].x, FinalNodeList[1].y);
        }

        if (FinalNodeList.Count != 0)
        {
            if (detectCol != null)
            {
                if (hasLineOfSight)
                {
                    if (!hasLineOfGizmo)
                    {
                        firstDetect = true;
                        if (FinalNodeList.Count == 1)
                        {
                            firstDetect = false;
                            return;
                        }
                        transform.position = Vector2.MoveTowards(transform.position, FinalNodePos, moveSpeed * Time.deltaTime);

                        if ((Vector2)transform.position == FinalNodePos) FinalNodeList.RemoveAt(0);
                    }
                    else
                    {
                        if (firstDetect)
                        {
                            if (firstGizmo)
                            {
                                firstGizmo = false;
                                firstDetect = false;
                                Destroy(cloneGizmo);
                                hasLineOfGizmo = false;
                                hasLineOfSight = true;
                                player = savePlayer;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (firstDetect)
                    {
                        if (!firstGizmo)
                        {
                            cloneGizmo = Instantiate(saveGizmo, player.position, Quaternion.identity);
                            player = cloneGizmo.transform;
                            hasLineOfSight = false;
                            firstGizmo = true;
                        }
                        else
                        {
                            if (FinalNodeList.Count == 1)
                            {
                                Destroy(cloneGizmo);
                                hasLineOfGizmo = false;
                                player = savePlayer;
                                firstGizmo = false;
                                firstDetect = false;
                                return;
                            }
                            transform.position = Vector2.MoveTowards(transform.position, FinalNodePos, moveSpeed * Time.deltaTime);

                            if ((Vector2)transform.position == FinalNodePos) FinalNodeList.RemoveAt(0);
                        }
                    }
                }
            }
            else
            {
                if (firstDetect)
                {
                    if (!firstGizmo)
                    {
                        cloneGizmo = Instantiate(saveGizmo, player.position, Quaternion.identity);
                        player = cloneGizmo.transform;
                        firstGizmo = true;
                    }
                    else
                    {
                        if (FinalNodeList.Count == 1)
                        {
                            Destroy(cloneGizmo);
                            hasLineOfGizmo = false;
                            player = savePlayer;
                            firstGizmo = false;
                            firstDetect = false;
                            return;
                        }

                        transform.position = Vector2.MoveTowards(transform.position, FinalNodePos, moveSpeed * Time.deltaTime);

                        if ((Vector2)transform.position == FinalNodePos) FinalNodeList.RemoveAt(0);
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (savePlayer != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, savePlayer.transform.position - transform.position);
            detectCol = Physics2D.OverlapCircle(transform.position, detectSize, what);
            if (detectCol != null)
            {
                if (hit.collider != null)
                {
                    hasLineOfSight = hit.collider.CompareTag("Player");
                    if (hasLineOfSight)
                    {
                        Debug.DrawRay(transform.position, savePlayer.transform.position - transform.position, Color.green);
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, savePlayer.transform.position - transform.position, Color.red);
                    }
                }
            }
        }
        if(cloneGizmo != null)
        {
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position, cloneGizmo.transform.position - transform.position);
            if (cloneGizmo != null)
            {
                if (hit2.collider != null)
                {
                    hasLineOfGizmo = hit2.collider.CompareTag("Gizmo");
                    if (hasLineOfGizmo)
                    {
                        Debug.DrawRay(transform.position, cloneGizmo.transform.position - transform.position, Color.green);
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, cloneGizmo.transform.position - transform.position, Color.red);
                    }
                }
            }
        }
    }

    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }


        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                //for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].y);
                return;
            }


            // ↗↖↙↘
            if (allowDiagonal)
            {
                OpenListAdd(CurNode.x + 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y + 1);
                OpenListAdd(CurNode.x - 1, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y - 1);
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkY >= bottomLeft.y && checkY < topRight.y + 1 && !NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y]))
        {
            // 대각선 허용시, 벽 사이로 통과 안됨
            if (allowDiagonal) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall && NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;

            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkY - bottomLeft.y].isWall || NodeArray[checkX - bottomLeft.x, CurNode.y - bottomLeft.y].isWall) return;


            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node NeighborNode = NodeArray[checkX - bottomLeft.x, checkY - bottomLeft.y];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectSize);
        if (FinalNodeList.Count != 0)
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                if (detectCol != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
                }
                else
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
                }
            }
        }
    }
}
