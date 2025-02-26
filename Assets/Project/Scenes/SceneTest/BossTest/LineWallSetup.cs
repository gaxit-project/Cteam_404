using UnityEngine;

public class LineWallSetup : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject playerHitPrefab; // PlayerHitÉvÉåÉnÉu
    [SerializeField] private float wallThickness = 0.1f; // ï«ÇÃå˙Ç≥
    [SerializeField] private float wallHeight = 5f; // ï«ÇÃçÇÇ≥
    [SerializeField] private float wallOffset = 0.5f;

    private GameObject[] wallSegments;

    void Start()
    {
        if (lineRenderer == null || playerHitPrefab == null)
        {
            return;
        }

        if (enabled)
        {
            SetupWalls();
        }
    }

    void LateUpdate()
    {
        if (enabled)
        {
            UpdateWalls();
        }
    }

    void OnEnable()
    {
        SetupWalls();
    }

    void OnDisable()
    {
        CleanupWalls();
    }

    public void SetupWalls()
    {
        CleanupWalls();

        int pointCount = lineRenderer.positionCount;

        if (pointCount < 2) return;

        wallSegments = new GameObject[2 * (pointCount - 1)];
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 point1 = lineRenderer.GetPosition(i);
            Vector3 point2 = lineRenderer.GetPosition(i + 1);

            Vector3 segmentDirection = (point2 - point1).normalized;
            float segmentLength = Vector3.Distance(point1, point2);
            Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

            for (int side = 0; side < 2; side++)
            {
                Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * (side == 0 ? -wallOffset : wallOffset));
                Quaternion wallRotation = Quaternion.LookRotation(segmentDirection, Vector3.up);

                GameObject wall = Instantiate(playerHitPrefab, wallPosition, wallRotation, transform);
                wall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);

                BoxCollider collider = wall.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.size = new Vector3(wallThickness, wallHeight, segmentLength);
                    collider.center = new Vector3(0, 0, segmentLength * 0.5f);
                }

                wallSegments[i * 2 + side] = wall;
            }
        }
    }

    void UpdateWalls()
    {
        if (lineRenderer == null || wallSegments == null)
        {
            return;
        }

        int pointCount = lineRenderer.positionCount;
        if (pointCount < 2 || wallSegments.Length != 2 * (pointCount - 1))
        {
            SetupWalls();
            return;
        }

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 point1 = lineRenderer.GetPosition(i);
            Vector3 point2 = lineRenderer.GetPosition(i + 1);

            Vector3 segmentDirection = (point2 - point1).normalized;
            float segmentLength = Vector3.Distance(point1, point2);
            Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

            for (int side = 0; side < 2; side++)
            {
                int index = i * 2 + side;
                if (index < wallSegments.Length && wallSegments[index] != null)
                {
                    GameObject wall = wallSegments[index];
                    Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * (side == 0 ? -wallOffset : wallOffset));
                    Quaternion wallRotation = Quaternion.LookRotation(segmentDirection, Vector3.up);

                    wall.transform.position = wallPosition;
                    wall.transform.rotation = wallRotation;
                    wall.transform.localScale = new Vector3(wallThickness, wallHeight, segmentLength);

                    BoxCollider collider = wall.GetComponent<BoxCollider>();
                    if (collider != null)
                    {
                        collider.size = new Vector3(wallThickness, wallHeight, segmentLength);
                        collider.center = new Vector3(0, 0, segmentLength * 0.5f);
                    }
                }
            }
        }
    }

    public void CleanupWalls()
    {
        if (wallSegments != null)
        {
            foreach (GameObject segment in wallSegments)
            {
                if (segment != null) Destroy(segment);
            }
            wallSegments = null;
        }
    }

    void OnDrawGizmos()
    {
        if (lineRenderer != null && playerHitPrefab != null && enabled)
        {
            int pointCount = lineRenderer.positionCount;
            for (int i = 0; i < pointCount - 1; i++)
            {
                Vector3 point1 = lineRenderer.GetPosition(i);
                Vector3 point2 = lineRenderer.GetPosition(i + 1);
                Vector3 segmentDirection = (point2 - point1).normalized;
                float segmentLength = Vector3.Distance(point1, point2);
                Vector3 wallDirection = Vector3.Cross(segmentDirection, Vector3.up).normalized;

                Gizmos.color = Color.green;
                for (int side = -1; side <= 1; side += 2)
                {
                    Vector3 wallPosition = ((point1 + point2) * 0.5f) + (wallDirection * side * wallOffset);
                    Vector3 wallSize = new Vector3(wallThickness, wallHeight, segmentLength);
                    Gizmos.DrawWireCube(wallPosition, wallSize);
                }
            }
        }
    }
}