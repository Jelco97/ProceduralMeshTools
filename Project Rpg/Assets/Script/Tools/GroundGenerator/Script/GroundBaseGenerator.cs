using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBaseGenerator : MonoBehaviour
{
    public bool SmoothStepSlope = true;
    [Header("Mesh data")]
    public HeightGround MapDefinition;
    public int Density = 1;
    public int NumberCellByLenght = 10;
    public int IndexInTheCheckboard;

    [Header("Left Checker")]
    public bool LeftChecker;
    public float[] LeftHeight;
    //[HideInInspector]
    public float[] InterpolateLeftHeightValue;

    [Header("Right Checker")]
    public bool RightChecker;
    public float[] RightHeight;
    private bool rightChecker;
    public GroundBaseGenerator RightGroundGenrator;

    [Header("Top Checker")]
    public bool TopChecker;
    public float[] TopHeight;
    private bool topChecker;
    public GroundBaseGenerator TopGroundGenerator;

    [Header("Bot Checker")]
    public bool BotChecker;
    public float[] BotHeight;
    //[HideInInspector]
    public float[] InterpolateBotHeightValue;

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

                else
                {
                    float fxPos = (x + .5f) / Density;
                    float fzPos = (z + .5f) / Density;
                    int Xindex = Mathf.FloorToInt(fxPos);
                    int Zindex = Mathf.FloorToInt(fzPos);
                    float fracXPos = fxPos - Xindex;
                    float fracZPos = fzPos - Zindex;

                    // Debug.Log(Xindex);

                    if (MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)] != 0)
                    {
                        #region Top of the checker
                        if (Zindex >= NumberCellByLenght - 1)
                        {
                            if (Xindex == 0)//Left Cell
                            {
                                heightLT = MaxHeight(LeftHeight[NumberCellByLenght - 1],//left
                                    TopHeight[Xindex], //Top
                                    DiagonalLeftTopHeight,//diagonal lt
                                    MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(LeftHeight[NumberCellByLenght - 1],//left
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex],//bot
                                    LeftHeight[NumberCellByLenght - 2],//diagonal lb
                                    MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex + 1],//Right
                                    TopHeight[Xindex],//Top
                                    TopHeight[Xindex + 1],//Diagonal rt
                                    MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex + 1],//Right
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex],//bot
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex + 1],//diagonal rb
                                    MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                            else if (Xindex >= NumberCellByLenght - 1)//Right Cell
                            {
                                heightLT = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[NumberCellByLenght - 1],//left
                                    TopHeight[NumberCellByLenght - 1],//top
                                    TopHeight[NumberCellByLenght - 2],//diagonal lt
                                   MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[NumberCellByLenght - 1],//left
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[NumberCellByLenght - 1],//bot
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[NumberCellByLenght - 2],// diagonal lb
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(RightHeight[NumberCellByLenght - 1],//right
                                    TopHeight[NumberCellByLenght - 1],//top
                                    DiagonalRightTopHeight,//diagonal rt
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(RightHeight[NumberCellByLenght - 1],//Right
                                     MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[NumberCellByLenght - 1],//bot
                                     RightHeight[NumberCellByLenght - 2],//diagonal rb
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                            else
                            {
                                heightLT = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex - 1],//left
                                    TopHeight[Xindex],//top
                                    TopHeight[Xindex - 1],//diagonal lt
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex - 1],//left
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex],//bot
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex - 1],// diagonal lb
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex + 1],//right
                                    TopHeight[Xindex],//top
                                    TopHeight[Xindex + 1],//diagonal rt
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(MapDefinition.MapRowsData[NumberCellByLenght - 1].Row[Xindex + 1],//right
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex],//bot
                                    MapDefinition.MapRowsData[NumberCellByLenght - 2].Row[Xindex + 1],// diagonal rb
MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                        }
                        #endregion

                        #region Right  of the checker
                        else if (Xindex >= NumberCellByLenght - 1)
                        {
                            if (Zindex == 0)//botRight
                            {
                                heightLT = MaxHeight(MapDefinition.MapRowsData[0].Row[NumberCellByLenght - 2],//left
                                    MapDefinition.MapRowsData[1].Row[NumberCellByLenght - 1],//Top
                                    MapDefinition.MapRowsData[1].Row[NumberCellByLenght - 2],//lt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(MapDefinition.MapRowsData[0].Row[NumberCellByLenght - 2],//left
                                    BotHeight[NumberCellByLenght - 1],//bot
                                    BotHeight[NumberCellByLenght - 2],//lb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(RightHeight[0],//right
                                    MapDefinition.MapRowsData[1].Row[NumberCellByLenght - 1],//top
                                    RightHeight[1],//rt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(RightHeight[0],//right
                                    BotHeight[NumberCellByLenght - 1],//bot
                                    DiagonalRightBotHeight,//rb;
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                            else
                            {
                                heightLT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[NumberCellByLenght - 2],//left
                                    MapDefinition.MapRowsData[Zindex + 1].Row[NumberCellByLenght - 1],//top
                                    MapDefinition.MapRowsData[Zindex + 1].Row[NumberCellByLenght - 2],//lt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[NumberCellByLenght - 2],//left
                                    MapDefinition.MapRowsData[Zindex - 1].Row[NumberCellByLenght - 1],//bot
                                    MapDefinition.MapRowsData[Zindex - 1].Row[NumberCellByLenght - 2],//lb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(RightHeight[Zindex],//right
                                    MapDefinition.MapRowsData[Zindex + 1].Row[NumberCellByLenght - 1],//top
                                    RightHeight[Zindex + 1],//rt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(RightHeight[Zindex],//right
                                    MapDefinition.MapRowsData[Zindex - 1].Row[NumberCellByLenght - 1],//bot
                                    RightHeight[Zindex - 1],//rb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                        }
                        #endregion

                        #region Left of the checker
                        else if (Xindex == 0)
                        {
                            if (Zindex == 0)//bot left
                            {
                                heightLT = MaxHeight(LeftHeight[Zindex],//left
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                    LeftHeight[Zindex + 1],//lt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(LeftHeight[Zindex],//left
                                    BotHeight[0],//bot
                                    DiagonalLeftBotHeight,//lb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[1],//right
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex + 1],//rt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//right
                                    BotHeight[Xindex],//bot
                                    BotHeight[Xindex + 1],//rb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                            else
                            {
                                heightLT = MaxHeight(LeftHeight[Zindex],//left
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                    LeftHeight[Zindex + 1],//lt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightLB = MaxHeight(LeftHeight[Zindex],//left
                                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                                    LeftHeight[Zindex - 1],//lb
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[1],//right
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                    MapDefinition.MapRowsData[Zindex + 1].Row[Xindex + 1],//rt
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);

                                heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[1],//right
                                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex],//bot
                                    MapDefinition.MapRowsData[Zindex - 1].Row[Xindex + 1],//bot
                                    MapDefinition.MapRowsData[Zindex].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)]);
                            }
                        }
                        #endregion

                        #region Bot of the checker
                        else if (Zindex == 0)
                        {
                            heightLT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                                MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                MapDefinition.MapRowsData[Zindex + 1].Row[Xindex - 1],//lt
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightLB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex - 1],//left
                                BotHeight[Xindex],//Bot
                                BotHeight[Xindex - 1],//lb
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightRT = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//right
                                MapDefinition.MapRowsData[Zindex + 1].Row[Xindex],//top
                                MapDefinition.MapRowsData[Zindex + 1].Row[Xindex + 1],//rt
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightRB = MaxHeight(MapDefinition.MapRowsData[Zindex].Row[Xindex + 1],//right
                                BotHeight[Xindex],//bot
                                BotHeight[Xindex + 1],//rb
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);
                        }
                        #endregion
                        else
                        {
                            heightLT = MaxHeight(MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Max(0, Xindex - 1)],//Left
                                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Max(0, Xindex - 1)],//diagonal
                                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)],//up
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightRT = MaxHeight(MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//right
                                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//diagonal
                                MapDefinition.MapRowsData[Mathf.Min(NumberCellByLenght - 1, Zindex + 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)],//up
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightLB = MaxHeight(MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Max(0, Xindex - 1)],//Left
                                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Max(0, Xindex - 1)],//diagonal
                                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)],//bot
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);

                            heightRB = MaxHeight(MapDefinition.MapRowsData[Mathf.Min(Zindex, NumberCellByLenght - 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//right
                                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Min(NumberCellByLenght - 1, Xindex + 1)],//diagonal
                                MapDefinition.MapRowsData[Mathf.Max(0, Zindex - 1)].Row[Mathf.Min(Xindex, NumberCellByLenght - 1)],//bot
                                MapDefinition.MapRowsData[Zindex].Row[Xindex]);
                        }

                        float blendU = Mathf.Lerp(heightLT, heightRT, fracXPos);
                        float blendD = Mathf.Lerp(heightLB, heightRB, fracXPos);
                        float height = Mathf.Lerp(blendD, blendU, fracZPos);
                        positionVertex.y = height;
                    }
                    else
                        positionVertex.y = 0;

                    #region Extremity Right and top
                    if (z == NumberCellByLenght * Density && TopChecker)//top
                    {
                        positionVertex.y = TopGroundGenerator.InterpolateBotHeightValue[(int)x];
                    }
                    else if (x == NumberCellByLenght * Density && RightChecker)//right
                    {
                        positionVertex.y = RightGroundGenrator.InterpolateLeftHeightValue[(int)z];
                    }
                    #endregion

                    #region Extremity Bot and left
                    if (z == 0)//bot
                    {
                        InterpolateBotHeightValue[(int)x] = positionVertex.y;
                    }
                    if (Xindex == NumberCellByLenght)//right
                    {
                        InterpolateLeftHeightValue[(int)z] = positionVertex.y;
                    }
                    #endregion
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

    bool ZeroCell(int Zpos, int Xpos)
    {
        if (MapDefinition.MapRowsData[Zpos].Row[Xpos] == 0)
            return true;
        else
            return false;
    }

    float MaxHeight(float x, float y, float z, float w)
    {
        if (x == 0 || y == 0 || z == 0 || w == 0)
            return 0;

        else if (x >= y && x >= z && x >= w)
            return x;

        else if (y >= x && y >= z && z >= w)
            return y;

        else if (z >= x && z >= y && z >= w)
            return z;

        else
            return w;
    }
}
