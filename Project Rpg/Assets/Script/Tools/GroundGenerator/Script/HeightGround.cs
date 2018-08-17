using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightGround
{
    [System.Serializable]
    public struct MapRowData
    {
        /// <summary>
        /// Height in the cell
        /// </summary>
        public float[] Row;//position dans la ranger
        //public float[] Distance;
        /// <summary>
        /// The cell store in this index
        /// </summary>
        public Cell[] CellsInformation;
        /// <summary>
        /// The image of the object in the custom vue
        /// </summary>
        public Texture2D[] PreviewCell;
        /// <summary>
        /// The pos of the foot of the object in this cell
        /// </summary>
        public float[] FootPos;
    }

    public MapRowData[] MapRowsData = new MapRowData[10];

    public void CleanHeight()
    {
        for (int i = 0; i < MapRowsData.Length; i++)
            for (int x = 0; x < MapRowsData.Length; x++)
            {
                MapRowsData[i].Row[x] = 0;
                if (MapRowsData[i].CellsInformation[x].CellContaint.EventScript)
                    GameObject.DestroyImmediate(MapRowsData[i].CellsInformation[x].CellContaint.EventScript.gameObject);
                MapRowsData[i].CellsInformation[x] = new Cell();
                MapRowsData[i].PreviewCell[x] = null;
                MapRowsData[i].FootPos[x] = 0;
            }
    }

    public void InitialisationRowArray(int size)
    {
        MapRowsData = new MapRowData[size];
        for (int x = 0; x < size; x++)
        {
            MapRowsData[x].Row = new float[size];
            MapRowsData[x].CellsInformation = new Cell[size];
            MapRowsData[x].PreviewCell = new Texture2D[size];
            MapRowsData[x].FootPos = new float[size];
        }

        foreach (MapRowData script in MapRowsData)
        {
            for (int i = 0; i < size; i++)
            {
                script.CellsInformation[i] = new Cell();
                script.CellsInformation[i].CellContaint = new Cell.CellData { Walkable = true, GroundAtribut = Cell.GroundElement.Earth, EventScript = null };
            }
        }
    }

    /// <summary>
    /// Add or remove ellement of array
    /// </summary>
    /// <param name="i">New size</param>
    public void NewRowArray(int i)
    {
        CleanCell();

        int maxIndex = MapRowsData.Length;

        MapRowData[] newAray = new MapRowData[i];
        for (int x = 0; x < i; x++)
        {
            newAray[x].Row = new float[i];
            newAray[x].CellsInformation = new Cell[i];
            newAray[x].PreviewCell = new Texture2D[i];
            newAray[x].FootPos = new float[i];

            for (int index = 0; index < i; index++)
            {
                newAray[x].CellsInformation[index] = new Cell();
                newAray[x].CellsInformation[index].CellContaint.Walkable = true;
                newAray[x].CellsInformation[index].CellContaint.GroundAtribut = Cell.GroundElement.Earth;
            }
        }

        for (int y = 0; y < i; y++)
        {
            if (y >= maxIndex)
                continue;

            for (int x = 0; x < i; x++)
            {
                if (x >= maxIndex)
                    continue;

                newAray[y].Row[x] = MapRowsData[y].Row[x];
                newAray[y].CellsInformation[x] = MapRowsData[y].CellsInformation[x];
            }
        }

        MapRowsData = newAray;
    }

    public void CleanCell()
    {
        for (int i = 0; i < MapRowsData.Length; i++)
        {
            for (int x = 0; x < MapRowsData.Length; x++)
            {
                if (MapRowsData[i].CellsInformation[x].CellContaint.EventScript)
                    GameObject.DestroyImmediate(MapRowsData[i].CellsInformation[x].CellContaint.EventScript.gameObject);
                if (MapRowsData[i].PreviewCell[x])
                    MapRowsData[i].PreviewCell[x] = null;
                MapRowsData[i].FootPos[x] = 0;
            }
        }
    }

    /*
    private float GetDistance(int x, int y)
    {
        if (x < 0 || x > 9 || y < 0 || y > 9)//the cell evaluate is in the checker ?
            return -1;// if not, return -1;
        return MapRowsData[x].Distance[y];// else if, return the distance
    }

    public void ComputeHeight()
    {
        for (int i = 0; i < MapRowsData.Length; i++)
        {
            MapRowsData[i].Distance = new float[MapRowsData.Length];//Initialisation of distance (size array)
            for (int j = 0; j < MapRowsData.Length; j++)// for eatch row, calculate the distance
                MapRowsData[i].Distance[j] = MapRowsData[i].Row[j] > 0 ? 0 : -1;//if row = 0 , distance = -1
        }

        for (int k = 0; k < 9; k++)//Calculate the distance betwen the cell walkable and the other
            for (int i = 0; i < MapRowsData.Length; i++)//postion of the checker on Z axis
                for (int j = 0; j < MapRowsData.Length; j++)//position in the checker on X axis
                {
                    if (GetDistance(i, j) == -1 && //if the cell in this pos = 0, stop this condition
                        (GetDistance(i - 1, j) == k ||
                        GetDistance(i + 1, j) == k ||
                        GetDistance(i, j - 1) == k ||
                        GetDistance(i, j + 1) == k ||
                        GetDistance(i - 1, j - 1) == k ||
                        GetDistance(i + 1, j - 1) == k ||
                        GetDistance(i - 1, j + 1) == k ||
                        GetDistance(i + 1, j + 1) == k))
                    {
                        MapRowsData[i].Distance[j] = k + 1;
                    }
                }
    }
    */
}
