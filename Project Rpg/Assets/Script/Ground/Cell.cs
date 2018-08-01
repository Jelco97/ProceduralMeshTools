using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public struct CellData
    {
        public bool Walkable;
        public GroundElement GroundAtribut;
        public EventCell EventScript;
    }

    public CellData CellContaint = new CellData();
}
