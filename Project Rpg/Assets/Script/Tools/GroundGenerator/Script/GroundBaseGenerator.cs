using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBaseGenerator : MonoBehaviour
{
    public bool SmoothStepSlope = true;
    public HeightGround MapDefinition;
    public int Density = 1;
    public int NumberCellByLenght = 10;
    public int IndexInTheCheckboard;

    public bool TopChecker;
    public bool RightChecker;
    public bool DiagonalChecker;
    public HeightGround TopHeight;
    public HeightGround RightHeight;
    public float DiagonalHeight;

    public float RedColorByHeight;
    public float GreenColorByHeight;
    public float BlueColorByHeight;

    private bool topChecker;
    private bool rightChecker;
    private Mesh mapMesh;

    public void GenerateGroundBase()
    {
        #region Init
        if (mapMesh != null)
            DestroyImmediate(mapMesh);
        mapMesh = new Mesh();

        int numSideQuad = (Density * NumberCellByLenght) - Density;
        int numVertice = (numSideQuad + 1) * (numSideQuad + 1);
        int numTriangles = numSideQuad * numSideQuad * 2;

        Vector3[] vertices = new Vector3[numVertice];
        int[] triangle = new int[numTriangles * 3];
        Vector2[] uv = new Vector2[numVertice];
        Color[] vertexColor = new Color[numVertice];
        #endregion

        int index = 0;
        for (float z = 0f; z < numSideQuad + 1; z++)
            for (float x = 0f; x < numSideQuad + 1; x++)
            {
                Vector3 positionVertex = new Vector3();
                positionVertex.z = (z / Density);
                positionVertex.x = (x / Density);

                if(!SmoothStepSlope)
                {
                    int Xindex = Mathf.FloorToInt(x / Density);
                    if (Xindex == NumberCellByLenght - 1)
                    {
                        if (RightChecker)
                            rightChecker = true;
                        Xindex--;
                    }

                    int Zindex = Mathf.FloorToInt(z / Density);
                    if (Zindex == NumberCellByLenght - 1)
                    {
                        if (TopChecker)
                            topChecker = true;
                        Zindex--;
                    }

                    float height = 0;
                    if (!topChecker && !rightChecker)
                        height = MapDefinition.MapRowsData[Zindex].Row[Xindex];

                    else if (topChecker && !rightChecker)
                        height = TopHeight.MapRowsData[0].Row[Xindex];

                    else if (rightChecker && !topChecker)
                        height = RightHeight.MapRowsData[Zindex].Row[0];
                    else if (TopChecker && rightChecker && DiagonalChecker)
                        height = DiagonalHeight;

                    topChecker = false;
                    rightChecker = false;

                    positionVertex.y = height;
                }

                else
                {
                    float fxPos = (x + 0.5f) / Density;
                    float fzPos = (z + 0.5f) / Density;
                    int xPos = Mathf.FloorToInt(fxPos);
                    int zPos = Mathf.FloorToInt(fzPos);
                    float fracXPos = fxPos - xPos;
                    float fracZPos = fzPos - zPos;

                    float height = MapDefinition.MapRowsData[xPos].Distance[zPos];

                    float heightL = MapDefinition.MapRowsData[Mathf.Max(0, xPos - 1)].Distance[zPos];
                    float heightR = MapDefinition.MapRowsData[Mathf.Min(9, xPos + 1)].Distance[zPos];
                    float heightU = MapDefinition.MapRowsData[xPos].Distance[Mathf.Max(0, zPos - 1)];
                    float heightD = MapDefinition.MapRowsData[xPos].Distance[Mathf.Min(9, zPos + 1)];

                    float minHeightLU = Mathf.Min(Mathf.Min(Mathf.Min(heightL, heightU),
                        MapDefinition.MapRowsData[Mathf.Max(0, xPos - 1)].Distance[Mathf.Max(0, zPos - 1)]),
                        height);
                    float minHeightRU = Mathf.Min(Mathf.Min(Mathf.Min(heightR, heightU),
                        MapDefinition.MapRowsData[Mathf.Min(9, xPos + 1)].Distance[Mathf.Max(0, zPos - 1)]),
                        height);
                    float minHeightLD = Mathf.Min(Mathf.Min(Mathf.Min(heightL, heightD),
                        MapDefinition.MapRowsData[Mathf.Max(0, xPos - 1)].Distance[Mathf.Min(9, zPos + 1)]),
                        height);
                    float minHeightRD = Mathf.Min(Mathf.Min(Mathf.Min(heightR, heightD),
                        MapDefinition.MapRowsData[Mathf.Min(9, xPos + 1)].Distance[Mathf.Min(9, zPos + 1)]),
                        height);

                    float heightBlendU = Mathf.Lerp(minHeightLU, minHeightRU, fracXPos);
                    float heightBlendD = Mathf.Lerp(minHeightLD, minHeightRD, fracXPos);
                    float heightBlend = Mathf.Lerp(heightBlendU, heightBlendD, fracZPos);

                    if (height == 0)
                        positionVertex.y = 0.0f;
                    else
                        positionVertex.y = heightBlend;
                }

                vertexColor[index] = VertexColorByHeight(positionVertex.y);
                uv[index] = new Vector2(x / (numSideQuad + 1), z / (numSideQuad + 1));
                vertices[index++] = positionVertex;
            }

        index = 0;
        for (int z = 0; z < numSideQuad; z++)
            for (int x = 0; x < numSideQuad; x++)
            {
                int vertexCounterPerLine = numSideQuad + 1;

                triangle[index++] = x + z * vertexCounterPerLine;
                triangle[index++] = x + (z + 1) * vertexCounterPerLine;
                triangle[index++] = x + 1 + z * vertexCounterPerLine;

                triangle[index++] = x + (z + 1) * vertexCounterPerLine;
                triangle[index++] = x + 1 + (z + 1) * vertexCounterPerLine;
                triangle[index++] = x + 1 + z * vertexCounterPerLine;
            }

        #region Mesh
        mapMesh.vertices = vertices;
        mapMesh.triangles = triangle;
        mapMesh.uv = uv;
        mapMesh.colors = vertexColor;

        mapMesh.RecalculateNormals();
        mapMesh.RecalculateBounds();

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mapMesh;

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
            meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mapMesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        #endregion
    }

    public void CleanHeightTab()
    {
        MapDefinition.CleanHeight();
    }

    /// <summary>
    /// Return Color if height is behind Value
    /// </summary>
    /// <param name="Height"></param>
    /// <returns></returns>
    Color VertexColorByHeight(float Height)
    {
        if (Height <= RedColorByHeight)
            return Color.red;
        else if (Height <= GreenColorByHeight)
            return Color.green;
        else
            return Color.blue;
    }
}
