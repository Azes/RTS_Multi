using UnityEditor;
using UnityEngine;

public class TerrainTexturePainter : MonoBehaviour
{
    public static Terrain terrain;
    public static bool isreset;
    public int textureLayerIndex = 0; // Index der Texturschicht
    public float brushSize = 5.0f;
    public float smoothness = 0.5f;
    [Range(1, 5)]
    public int DrawValue;
    [Header("Gizmo")]
    public bool DrawGizmo = true;
    public Color gColor, sColor;
    float[,,] oalpha;

    private void Awake()
    {
        if (terrain == null)
        {
            GameObject targetObject = GameObject.FindWithTag("Terrain");

            terrain = targetObject.GetComponent<Terrain>();
        }


        oalpha = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        // oalpha = terrain.terrainData.GetAlphamaps(0,0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
    }

    private void Start()
    {
        for (int i = 0;i< DrawValue;i++)
            PaintTextureAtPosition(transform.position);
        isreset = false;
    }

    private void OnApplicationQuit()
    {
        if (!isreset)//new
        {
            terrain.terrainData.SetAlphamaps(0, 0, oalpha);
            terrain.terrainData.SyncTexture(TerrainData.AlphamapTextureName);
            isreset = true;
        }
    }

    //new

   //private void OnDisable()
   //{
   //    if (!isreset)
   //    {
   //        terrain.terrainData.SetAlphamaps(0, 0, oalpha);
   //        terrain.terrainData.SyncTexture(TerrainData.AlphamapTextureName);
   //        isreset = true;
   //        Debug.Log("disabel");
   //    }
   //}
    private void OnDestroy()
    {
        if (!isreset)
        {
            terrain.terrainData.SetAlphamaps(0, 0, oalpha);
            terrain.terrainData.SyncTexture(TerrainData.AlphamapTextureName);
            isreset = true;
            Debug.Log("distroy");
        }
    }

    //

    private void PaintTextureAtPosition(Vector3 worldPosition)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 localPosition = worldPosition - terrainPosition;

        int alphamapWidth = terrainData.alphamapWidth;
        int alphamapHeight = terrainData.alphamapHeight;

        int alphamapX = (int)((localPosition.x / terrainData.size.x) * alphamapWidth);
        int alphamapY = (int)((localPosition.z / terrainData.size.z) * alphamapHeight);

        float[,,] alphamaps = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);

        for (int y = -Mathf.FloorToInt(brushSize); y <= Mathf.FloorToInt(brushSize); y++)
        {
            for (int x = -Mathf.FloorToInt(brushSize); x <= Mathf.FloorToInt(brushSize); x++)
            {
                int currentX = alphamapX + x;
                int currentY = alphamapY + y;

                if (currentX >= 0 && currentX < alphamapWidth && currentY >= 0 && currentY < alphamapHeight)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= brushSize)
                    {
                        for (int layer = 0; layer < terrainData.alphamapLayers; layer++)
                        {
                            float targetAlpha = (layer == textureLayerIndex) ? 1.0f : 0.0f;
                            float currentAlpha = alphamaps[currentY, currentX, layer];

                            float smoothnessFactor = Mathf.SmoothStep(0.0f, 1.0f, 1.0f - Mathf.Clamp01((distance - brushSize + smoothness) / smoothness));
                            float smoothedAlpha = Mathf.Lerp(currentAlpha, targetAlpha, smoothnessFactor);

                            alphamaps[currentY, currentX, layer] = smoothedAlpha;
                        }
                    }
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamaps);
        terrainData.SyncTexture(TerrainData.AlphamapTextureName);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (DrawGizmo && terrain != null)
        {
            Handles.color = gColor;
            Handles.DrawSolidDisc(transform.position, Vector3.up, brushSize * (terrain.terrainData.size.x / 500));
            Handles.color = sColor;
            Handles.DrawSolidDisc(transform.position, Vector3.up, (brushSize * (terrain.terrainData.size.x / 500)) - smoothness);
        }
    }
#endif
}
