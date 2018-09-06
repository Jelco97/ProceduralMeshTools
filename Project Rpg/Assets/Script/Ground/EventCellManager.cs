using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCellManager : MonoBehaviour {

    [Header("Checker Data")]
    public int CheckerOnTheLenght = 0;
    public int CellByChecker = 0;

    /// <summary>
    /// Modify the value of the event Cell Manager
    /// </summary>
    /// <param name="NumberCheckerOnTheLenght"></param>
    /// <param name="NumberCellByChecker"></param>
    public void InitialisationCheckerData(int NumberCheckerOnTheLenght, int NumberCellByChecker)
    {
        CheckerOnTheLenght = NumberCheckerOnTheLenght;
        CellByChecker = NumberCellByChecker;
    }
}
