using UnityEngine.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Tilemap tilemap;
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private int itemsCollected = 0;
    private int totalItems;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        targetPosition = transform.position;
        InitializeItems();
    }

    // 새로 추가한 메서드
    public void InitializeItems()
    {
        itemsCollected = 0;
        totalItems = MapGenerator.Instance.GetTotalItems();
        Debug.Log($"Initialized with {totalItems} items to collect");
    }

    void Update()
    {
        // 게임오버 상태면 조작 불가
        if (GameManager.Instance.IsGameOver())
        {
            return;
        }

        if (!isMoving)
        {
            Vector3 movement = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.RightArrow))
                movement = Vector3.right;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                movement = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                movement = Vector3.up;
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                movement = Vector3.down;

            if (movement != Vector3.zero)
            {
                Vector3 newPosition = transform.position + movement;
                Vector3Int newCell = tilemap.WorldToCell(newPosition);

                if (CanMoveToTile(newCell))
                {
                    targetPosition = tilemap.GetCellCenterWorld(newCell);
                    isMoving = true;
                    GameManager.Instance.DecrementMoves();
                }
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isMoving = false;
                HandleTileAtPosition(tilemap.WorldToCell(transform.position));
            }
        }
    }

    void HandleTileAtPosition(Vector3Int cellPosition)
    {
        TileBase currentTile = tilemap.GetTile(cellPosition);

        if (currentTile == MapGenerator.Instance.itemTile)
        {
            CollectItem(cellPosition);
        }
        else if (currentTile == MapGenerator.Instance.groundTile)
        {
            // 그라운드 타일을 지울 수 있게 수정
            Debug.Log("Breaking ground tile");
            tilemap.SetTile(cellPosition, null);
        }

        // 디버그용 로그
        Debug.Log($"Current tile at {cellPosition}: {currentTile}");
    }

    bool CanMoveToTile(Vector3Int cellPosition)
    {
        TileBase tile = tilemap.GetTile(cellPosition);
        // 이동 가능한 조건 수정
        return tile == null || tile == MapGenerator.Instance.groundTile || tile == MapGenerator.Instance.itemTile;
    }
    bool IsStartPosition(Vector3Int position)
    {
        return position == tilemap.WorldToCell(targetPosition);
    }

    void CollectItem(Vector3Int cellPosition)
    {
        tilemap.SetTile(cellPosition, null);
        GameManager.Instance.AddScore(100);
        itemsCollected++;

        Debug.Log($"Collected item {itemsCollected}/{totalItems}");

        if (itemsCollected == totalItems)
        {
            Debug.Log("Stage Complete - Collected all items!");
            itemsCollected = 0;
            GameManager.Instance.OnStageComplete();
        }
    }
    public void ResetCollection(int newTotalItems)
    {
        itemsCollected = 0;
        totalItems = newTotalItems;
        Debug.Log($"Reset collection. Need to collect {totalItems} items");
    }
}