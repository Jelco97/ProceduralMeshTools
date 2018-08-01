using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HeightGround
{
    [System.Serializable]
    public struct TileHeight
    {
        public float[] Row;//position dans la ranger
        public Cell.CellData[] CellsInformation;
        public Texture2D[] PreviewCell;
    }

    public TileHeight[] HeightGroundData = new TileHeight[10];

    public void CleanHeight()
    {
        for (int i = 0; i < HeightGroundData.Length; i++)
            for (int x = 0; x < HeightGroundData.Length; x++)
            {
                HeightGroundData[i].Row[x] = 0;
                GameObject.Destroy(HeightGroundData[i].CellsInformation[x].EventScript.gameObject);
                HeightGroundData[i].CellsInformation[x] = new Cell.CellData();
                HeightGroundData[i].PreviewCell[x] = null;
            }
    }

    public void InitialisationRowArray(int size)
    {
        //CleanCell();

        HeightGroundData = new TileHeight[size];
        for (int x = 0; x < size; x++)
        {
            HeightGroundData[x].Row = new float[size];
            HeightGroundData[x].CellsInformation = new Cell.CellData[size];
        }

        foreach (TileHeight script in HeightGroundData)
        {
            for(int i = 0; i<size;i++)
            {
                script.CellsInformation[i].Walkable = true;
                script.CellsInformation[i].GroundAtribut = Cell.GroundElement.Earth;
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

        int maxIndex = HeightGroundData.Length;

        TileHeight[] newAray = new TileHeight[i];
        for (int x = 0; x < i; x++)
        {
            newAray[x].Row = new float[i];
            newAray[x].CellsInformation = new Cell.CellData[i];
            for(int index = 0; index < i; index++)
            {
                newAray[x].CellsInformation[index].Walkable = true;
                newAray[x].CellsInformation[index].GroundAtribut = Cell.GroundElement.Earth;
            }
        }

        for(int y = 0; y < i; y++)
        {
            if (y >= maxIndex)
                continue;

            for(int x = 0; x < i; x++)
            {
                if (x >= maxIndex)
                    continue;

                newAray[y].Row[x] = HeightGroundData[y].Row[x];
                newAray[y].CellsInformation[x] = HeightGroundData[y].CellsInformation[x];
            }
        }

        HeightGroundData = newAray;
    }

    void CleanCell()
    {
        for (int i = 0; i < HeightGroundData.Length; i++)
        {
            for (int x = 0; x < HeightGroundData.Length; x++)
            {
                GameObject.Destroy(HeightGroundData[i].CellsInformation[x].EventScript.gameObject);
                HeightGroundData[i].PreviewCell[x] = null;
            }
        }
    }
}
