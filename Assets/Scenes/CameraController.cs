using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private Tilemap tilemap;
    public float padding = 1f;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        tilemap = FindObjectOfType<Tilemap>();
        AdjustCameraToTilemap();
    }

    public void AdjustCameraToTilemap()
    {
        if (tilemap == null || mainCamera == null) return;

        BoundsInt bounds = tilemap.cellBounds;

        // 타일맵의 실제 월드 좌표 경계 구하기
        Vector3 min = tilemap.CellToWorld(new Vector3Int(bounds.xMin, bounds.yMin, 0));
        Vector3 max = tilemap.CellToWorld(new Vector3Int(bounds.xMax, bounds.yMax, 0));

        // 맵의 중심점 계산
        Vector3 center = (min + max) * 0.5f;
        center.z = transform.position.z;
        transform.position = center;

        // 맵 전체가 보이도록 카메라 크기 조정
        float mapWidth = max.x - min.x;
        float mapHeight = max.y - min.y;

        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = mapWidth / mapHeight;

        if (screenRatio >= targetRatio)
        {
            mainCamera.orthographicSize = (mapHeight / 2) + padding;
        }
        else
        {
            mainCamera.orthographicSize = ((mapWidth / screenRatio) / 2) + padding;
        }
    }

    public void OnTilemapChanged()
    {
        AdjustCameraToTilemap();
    }
}