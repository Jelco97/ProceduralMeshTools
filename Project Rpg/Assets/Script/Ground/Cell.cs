using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public enum GroundElement
    {
        Earth,
        Ice,
        Fire,
        Water,
        Sand
    }

    [System.Serializable]
    public struct CellData
    {
        public float Height;
        public bool Walkable;
        public GroundElement GroundAtribut;
        public EventCell EventScript;
    }

    public CellData CellContaint = new CellData();
}
