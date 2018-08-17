using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBaseGenerator : MonoBehaviour
{
    public bool SmoothStepSlope = false;
    [Header("Mesh data")]
    public HeightGround MapDefinition;
    public int Density = 1;
    public int NumberCellByLenght = 10;
    public int IndexInTheCheckboard;

    [Header("Left Checker")]
    public bool LeftChecker;
    public float[] LeftHeight;

    [Header("Right Checker")]
    public bool RightChecker;
    public float[] RightHeight;
    private bool rightChecker;

    [Header("Top Checker")]
    public bool TopChecker;
    public float[] TopHeight;
    private bool topChecker;

    [Header("Bot Checker")]
    public bool BotChecker;
    public float[] BotHeight;

    [Header("Diagonal Checker")]
    public bool DiagonalRightTopChecker;
    public float DiagonalRightTopHeight;
    [Space(5)]
    public bool DiagonalLeftTopChecker;
    public float DiagonalLeftTopHeight;
    [Space(5)]
    public bool DiagonalLeftBotChecker;
    public float DiagonalLeftBotHeight;
    [Space(5)]
    public bool DiagonalRightBotChecker;
    public float DiagonalRightBotHeight;

    [Header("Vertex Color")]
    public float RedColorByHeight;
    public float GreenColorByHeight;
    public float BlueColorByHeight;

    private float heightLT;
    private float heightRT;
    private float heightLB;
    private float heightRB;

    private Mesh mapMesh;

    public void GenerateGroundBase()
    {
        #region Init
        if (mapMesh != null)
            DestroyImmediate(mapMesh);
        mapMesh = new Mesh();

        int numSideQuad = (Density * (NumberCellByLenght + 1)) - Density;
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

                if (!SmoothStepSlope)
                {
                    int Xindex = Mathf.FloorToInt(x / Density);
                    if (Xindex == NumberCellByLenght)
                    {
                        if (RightChecker)
                            rightChecker = true;
                        Xindex--;
                    }

                    int Zindex = Mathf.FloorToInt(z / Density);
                    if (Zindex == NumberCellByLenght)
                    {
                        if (TopChecker)
                            topChecker = true;
                        Zindex--;
                    }

                    //Debug.Log(Xindex);

                    float height = 0;
                    if (!topChecker && !rightChecker)
                        height = MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)];

                    else if (topChecker && !rightChecker)
                        height = TopHeight[Xindex];//TopHeight.MapRowsData[0].Row[Xindex];

                    else if (rightChecker && !topChecker)
                        height = RightHeight[Zindex];

                    else if (TopChecker && rightChecker && DiagonalRightTopChecker)
                        height = DiagonalRightTopHeight;

                    topChecker = false;
                    rightChecker = false;

                    positionVertex.y = height;
                }

                //else
                //{
                //    float fxPos = (x) / Density;
                //    float fzPos = (z) / Density;
                //    int Zindex = Mathf.FloorToInt(fzPos);
                //    int Xindex = Mathf.FloorToInt(fxPos);
                //    float fracXPos = fxPos - Xindex;
                //    float fracZPos = fzPos - Zindex;
                //
                //    // Debug.Log(Xindex);
                //
                //    if (MapDefinition.MapRowsData[Zindex].Row[Xindex] != 0)
                //    {
                //        if (Zindex >= NumberCellByLenght && TopChecker)
                //        {
                //            if (Xindex == 0)
                //            {
                //                heightLT = MaxHeight(LeftHeight.MapRowsData[NumberCellByLenght - 1].Row[NumberCellByLenght - 1],//left
                //                    TopHeight[Xindex], //Top
                //                    DiagonalLeftTopHeight);//diagonal lt
                //
                //                heightLB = MaxHeight(LeftHeight.MapRowsData[NumberCellByLenght - 1].Row[NumberCellByLenght - 1],//left
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                //                    LeftHeight.MapRowsData[NumberCellByLenght - 2].Row[NumberCellByLenght - 1]);//diagonal lb
                //
                //                heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//Right
                //                    TopHeight[Xindex],//Top
                //                    TopHeight[Xindex + 1]);//Diagonal rt
                //
                //                heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//Right
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex + 1]);//diagonal rb
                //            }
                //            else if(Xindex == NumberCellByLenght)
                //            {
                //                heightLT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                //                    TopHeight[Xindex],//top
                //                    TopHeight[Xindex - 1]);//diagonal lt
                //
                //                heightLB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex - 1]);// diagonal lb
                //
                //                heightRT = MaxHeight(RightHeight.MapRowsData[NumberCellByLenght - 1].Row[0],//right
                //                    TopHeight[Xindex],//top
                //                    DiagonalRightTopHeight);
                //            }
                //            else
                //            {
                //                heightLT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                //                    TopHeight[Xindex],//top
                //                    TopHeight[Xindex - 1]);//diagonal lt
                //
                //                heightLB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex - 1]);// diagonal lb
                //
                //                heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//right
                //                    TopHeight[Xindex],//top
                //                    TopHeight[Xindex + 1]);//diagonal rt
                //
                //                heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//right
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                //                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex + 1]);// diagonal rb
                //            }
                //        }
                //
                //        if (Xindex >= NumberCellByLenght && RightChecker)
                //        {
                //
                //        }
                //
                //        if (MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex)] == 0)
                //        {
                //            positionVertex.y = 0;
                //        }
                //        else
                //        {
                //            heightLT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Mathf.Max(0, Xindex - 1)],//Left
                //                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Max(0, Xindex - 1)],//diagonal
                //                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Xindex]);//up
                //
                //            heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//right
                //                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//diagonal
                //                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Xindex]);//up
                //
                //            heightLB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Mathf.Max(0, Xindex - 1)],//Left
                //                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Max(0, Xindex - 1)],//diagonal
                //                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Xindex]);//down
                //
                //            heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//right
                //                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//diagonal
                //                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Xindex]);//down
                //        }
                //
                //        float blendU = Mathf.Lerp(heightLT, heightRT, fracXPos);
                //        float blendD = Mathf.Lerp(heightLB, heightRB, fracXPos);
                //        float height = Mathf.Lerp(blendU, blendD, fracZPos);
                //        positionVertex.y = height;
                //    }
                //    else
                //        positionVertex.y = MapDefinition.MapRowsData[Zindex].Row[Xindex];
                //}

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

    bool ZeroCell(int Zpos, int Xpos)
    {
        if (MapDefinition.MapRowsData[Zpos].Row[Xpos] == 0)
            return true;
        else
            return false;
    }

    float MaxHeight(float x, float y, float z)
    {
        if (x == 0 || y == 0 || z == 0)
            return 0;

        else if (x >= y && x >= z)
            return x;

        else if (y >= x && y >= z)
            return y;

        else
            return z;
    }
}
