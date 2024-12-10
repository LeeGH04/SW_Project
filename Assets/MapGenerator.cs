using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    public Tilemap tilemap;
    public TileBase wallTile;
    public TileBase itemTile;
    public TileBase groundTile;
    public Transform playerTransform;

    private int totalItems = 0;
    private int currentMapIndex = 0;

    // 쉬운 난이도 맵 (7x7)
    private readonly string[][] easyMaps = new string[][]
    {
        new string[] {  // 스테이지 1
            "WWWWWWW",
            "WGPGIGW",
            "WGWWWGW",
            "WGIGIGW",
            "WGWWWGW",
            "WGIGIGW",
            "WWWWWWW"
        },
        new string[] {  // 스테이지 2
            "WWWWWWW",
            "WPGGIGW",
            "WWWGWGW",
            "WIGIGGW",
            "WGWGWWW",
            "WGIGGEW",
            "WWWWWWW"
        },
        new string[] {  // 스테이지 3
            "WWWWWWW",
            "WGIGGPW",
            "WGWWWGW",
            "WIGGGEW",
            "WGWWWGW",
            "WGIGIGW",
            "WWWWWWW"
        },
        new string[] {  // 스테이지 4
            "WWWWWWW",
            "WPIGGIW",
            "WGWWWGW",
            "WIGGGIW",
            "WGWWWGW",
            "WIGGGIW",
            "WWWWWWW"
        },
        new string[] {  // 스테이지 5
            "WWWWWWW",
            "WIGIGPW",
            "WGWWWGW",
            "WIGIEIW",
            "WGWWWGW",
            "WIGIGIW",
            "WWWWWWW"
        }
    };

    // 중간 난이도 맵 (12x12)
    private readonly string[][] mediumMaps = new string[][]
    {
        new string[] {  // 스테이지 1
            "WWWWWWWWWWWW",
            "WGPGIGIGIGGW",
            "WGWWWWGWWWGW",
            "WGIGIGIGIGGW",
            "WGWGWWWGWWGW",
            "WGIGIIGIGGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WWWWWWWWWWWW"
        },
        new string[] {  // 스테이지 2
            "WWWWWWWWWWWW",
            "WPGGIGIGGGGW",
            "WWWWGWWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WWWWWWWWWWWW"
        },
        new string[] {  // 스테이지 2
            "WWWWWWWWWWWW",
            "WPGGIGIGGGGW",
            "WWWWGWWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WWWWWWWWWWWW"
        },
        new string[] {  // 스테이지 2
            "WWWWWWWWWWWW",
            "WPGGIGIGGGGW",
            "WWWWGWWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WWWWWWWWWWWW"
        },
        new string[] {  // 스테이지 2
            "WWWWWWWWWWWW",
            "WPGGIGIGGGGW",
            "WWWWGWWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WGIGIGIGIGGW",
            "WGWWWGWWWWGW",
            "WWWWWWWWWWWW"
        },


        // ... (스테이지 3,4,5는 비슷한 패턴으로 계속)
    };

    // 어려운 난이도 맵 (20x20)
    private readonly string[][] hardMaps = new string[][]
    {
        new string[] {  // 스테이지 1
            "WWWWWWWWWWWWWWWWWWWW",
            "WPGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWWWGWWWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WWWWWWWWWWWWWWWWWWWW"
        },
         new string[] {  // 스테이지 1
            "WWWWWWWWWWWWWWWWWWWW",
            "WPGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWWWGWWWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WWWWWWWWWWWWWWWWWWWW"
        },
          new string[] {  // 스테이지 1
            "WWWWWWWWWWWWWWWWWWWW",
            "WPGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWWWGWWWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WWWWWWWWWWWWWWWWWWWW"
        },
           new string[] {  // 스테이지 1
            "WWWWWWWWWWWWWWWWWWWW",
            "WPGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWWWGWWWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WWWWWWWWWWWWWWWWWWWW"
        },
            new string[] {  // 스테이지 1
            "WWWWWWWWWWWWWWWWWWWW",
            "WPGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWWWGWWWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WGIGIGIGIGIGIGIGIGGW",
            "WGWWWWWGWWWGWWWWWWGW",
            "WWWWWWWWWWWWWWWWWWWW"
        }
        // ... (스테이지 2,3,4,5는 비슷한 패턴으로 계속)
    };

    void Awake()
    {
        Instance = this;
    }

    public void GenerateMap(GameManager.Difficulty difficulty)
    {
        tilemap.ClearAllTiles();
        string[] mapToUse;
        totalItems = 0;

        switch (difficulty)
        {
            case GameManager.Difficulty.Easy:
                mapToUse = easyMaps[currentMapIndex];
                break;
            case GameManager.Difficulty.Medium:
                mapToUse = mediumMaps[currentMapIndex];
                break;
            case GameManager.Difficulty.Hard:
                mapToUse = hardMaps[currentMapIndex];
                break;
            default:
                mapToUse = easyMaps[currentMapIndex];
                break;
        }

        // 아이템 개수 먼저 세기
        totalItems = mapToUse.Sum(row => row.Count(c => c == 'I'));
        int newMinMoves = CalculateMinimumMoves();
        GameManager.Instance.SetMinimumMoves(newMinMoves);
        Debug.Log($"New stage - Total Items: {totalItems}, Minimum Moves: {newMinMoves}");

        int offsetX = -mapToUse[0].Length / 2;
        int offsetY = mapToUse.Length / 2;
        Vector3Int playerSpawnPos = Vector3Int.zero;  // 플레이어 스폰 위치 저장용

        for (int y = 0; y < mapToUse.Length; y++)
        {
            string row = mapToUse[y];
            for (int x = 0; x < row.Length; x++)
            {
                Vector3Int position = new Vector3Int(x + offsetX, -y + offsetY, 0);

                switch (row[x])
                {
                    case 'W':
                        tilemap.SetTile(position, wallTile);
                        break;
                    case 'G':
                        tilemap.SetTile(position, groundTile);
                        break;
                    case 'I':
                        tilemap.SetTile(position, itemTile);
                        break;
                    case 'P':
                        playerSpawnPos = position;  // 플레이어 위치 저장
                        break;
                }
            }
        }

        // 플레이어 위치 설정 (한 번만 실행)
        playerTransform.position = tilemap.GetCellCenterWorld(playerSpawnPos);
        Debug.Log($"Player spawned at: {playerSpawnPos}");

        currentMapIndex = (currentMapIndex + 1) % 5;

        var playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.InitializeItems();
        }
    }

    public int GetTotalItems()
    {
        return totalItems;
    }

    private int CalculateMinimumMoves()
    {
        List<Vector2Int> itemPositions = new List<Vector2Int>();
        Vector2Int startPos = Vector2Int.zero;

        // 맵의 실제 크기를 기반으로 좌표 계산
        int offsetX = -tilemap.size.x / 2;
        int offsetY = tilemap.size.y / 2;

        // 1. 플레이어 시작 위치와 모든 아이템 위치 찾기
        for (int y = 0; y < tilemap.size.y; y++)
        {
            for (int x = 0; x < tilemap.size.x; x++)
            {
                Vector3Int pos = new Vector3Int(x + offsetX, -y + offsetY, 0);
                TileBase tile = tilemap.GetTile(pos);

                if (tile == itemTile)
                {
                    itemPositions.Add(new Vector2Int(x + offsetX, -y + offsetY));
                    Debug.Log($"Found item at: {x + offsetX}, {-y + offsetY}");
                }
                if (tile == null) // 플레이어 시작 위치
                {
                    startPos = new Vector2Int(x + offsetX, -y + offsetY);
                    Debug.Log($"Found player start at: {x + offsetX}, {-y + offsetY}");
                }
            }
        }

        if (itemPositions.Count == 0)
        {
            Debug.LogError("No items found!");
            return 10; // 기본값
        }

        int totalDistance = 0;
        Vector2Int currentPos = startPos;
        List<Vector2Int> unvisitedItems = new List<Vector2Int>(itemPositions);

        // 2. 각 아이템까지의 최단 거리 계산
        while (unvisitedItems.Count > 0)
        {
            int minDistance = int.MaxValue;
            Vector2Int nextItem = unvisitedItems[0];

            foreach (var item in unvisitedItems)
            {
                int distance = FindShortestPath(currentPos, item);
                Debug.Log($"Distance from {currentPos} to {item}: {distance}");
                if (distance < minDistance && distance != int.MaxValue)
                {
                    minDistance = distance;
                    nextItem = item;
                }
            }

            if (minDistance == int.MaxValue)
            {
                Debug.LogError($"Can't reach item at {nextItem}!");
                continue;
            }

            totalDistance += minDistance;
            currentPos = nextItem;
            unvisitedItems.Remove(nextItem);
        }

        // 최단거리 + 아이템 개수 (먹는 동작) + 5
        int minimumMoves = totalDistance + itemPositions.Count + 5;
        Debug.Log($"Path: {totalDistance}, Items: {itemPositions.Count}, Extra: 5, Total: {minimumMoves}");
        return minimumMoves;
    }
    private int FindShortestPath(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
                return distances[current];

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                Vector3Int tilePos = new Vector3Int(next.x, next.y, 0);

                // 이미 방문했거나 벽인 경우 스킵
                if (distances.ContainsKey(next) || tilemap.GetTile(tilePos) == wallTile)
                    continue;

                queue.Enqueue(next);
                distances[next] = distances[current] + 1;
            }
        }

        return int.MaxValue; // 경로를 찾지 못한 경우
    }
}