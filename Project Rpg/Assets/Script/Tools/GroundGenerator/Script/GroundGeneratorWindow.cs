using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GroundGeneratorWindow : EditorWindow
{
    #region Material
    public Material CheckerMaterial;
    #endregion

    #region GUIStyle
    GUIStyle header;
    GUIStyle subtitle;
    GUIStyle field;
    #endregion

    #region Color
    Color borderColor = new Color(.9f, .9f, .9f, 1);
    Color backgroundColor = new Color(.8f, .8f, .8f, 1);
    Color browColor = new Color(.4f, .2f, 0);
    #endregion

    #region Var
    ///Mouse
    bool mouseClicked;

    ///ToolBar
    bool groundCreat;

    ///Editor Skin
    private bool skinNewGroundVue;
    private bool skinCheckerVue;
    private bool skinCellVue;
    private bool skinUnwalkableByHeightVue;
    private bool skinCellDataVue;
    private bool skinCellEditorVue = true;
    private bool skinDataGroundVue;
    private bool skinLoadGroundVue;
    private bool skinLoadAndCreatGroundVue;
    private bool skinSaveCellVue;
    private bool skinFlattenVue;
    private bool skinFlattenAllVue;
    private bool skinVertexColorVue;
    private bool skinImportAssetVue;
    private bool skinflattenPainterVue;
    private bool skinAssetVue;
    private bool repaint;

    ///New Ground
    string nameMap = "New Map";
    int checkerOnTheLenght = 4;
    int checkerOnTheHeight = 4;
    int cellByLenghtChecker = 10;
    int cellDensity = 4;

    ///Save
    GameObject groundFolder;
    List<GameObject> checker = new List<GameObject>();
    List<HeightGround> height = new List<HeightGround>();//Only use for load and creat 
    HeightGround currentHeightGround;
    MapDataSave currentMapDataSave;
    string initNameMap;
    int initCheckerOnTheLenght;
    int initCheckerOnTheHeight;
    int initCellByLenghtChecker;
    int initCellDensity;
    string NameJSONFile;

    ///Save Cell
    EventCell eventCell;
    GameObject eventCellFolder;

    ///FlattenPainterEditor
    float flattenPainterValueAsigned;
    List<int> indexCheckerPaint = new List<int>();//Color Cell
    float[] flattenValueChecker;
    GroundBaseGenerator[] groundModifyByFlattenPainter;

    ///Cell ground 
    GameObject currentGround;
    int indexCurrentChecker;
    int indexCurrentGroundOnTheLenght;
    int indexCurrentGroundOnTheHeight;
    List<Vector2> cellPaint = new List<Vector2>();
    bool paintMode;
    bool valueChoice;
    float paintValue;
    float flattenCellValueCheckboard; // The value apply in all cell height

    ///Cell data
    Vector2 lastCellPos = Vector2.zero;
    bool walkableModeVue = true;
    bool groundEllementVue = false;
    bool walkableMode;
    bool earthMode;
    bool iceMode;
    bool waterMode;
    bool sandMode;
    int holdMode;
    Cell.GroundElement currentGroundElement = Cell.GroundElement.Earth;
    float maxHeightWalkableValue;

    ///Asset Editor
    GameObject[] assetsImport = new GameObject[6];
    float[] assetFoot = new float[6];
    Texture2D[] assetPreview = new Texture2D[6];
    GameObject currentAsset;
    int currentAssetIndex;
    Texture2D currentAssetPreview;
    bool cellEraserMode;

    ///Vertex Color
    float vertexColorRedValue = 0;
    float vertexColorGreenValue = 2;
    float vertexColorBlueValue = 4;

    #endregion

    void Awake()
    {
        header = new GUIStyle();
        header.fontStyle = FontStyle.Bold;
        header.fontSize = 18;

        subtitle = new GUIStyle();
        subtitle.fontSize = 14;

        field = new GUIStyle();
        field.fontSize = 10;
    }

    [MenuItem("CustomTools/GroundGenerator")]
    static void Init()
    {
        GroundGeneratorWindow window = (GroundGeneratorWindow)EditorWindow.GetWindow(typeof(GroundGeneratorWindow));
        window.minSize = new Vector2(500, 400);
    }

    void OnGUI()
    {
        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0 && mouseClicked)
            mouseClicked = false;
        else if (!mouseClicked && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            mouseClicked = true;

        GUILayout.BeginHorizontal(EditorStyles.toolbarButton);
        ToolBarButton();
        GUILayout.EndHorizontal();

        if (skinNewGroundVue || skinDataGroundVue)
            NewGroundEditor();

        else if (skinLoadGroundVue)
            LoadGroundEditor();

        else if (skinSaveCellVue)
            SaveCellEditor();

        else if (skinflattenPainterVue)
            FlattenPainterEditor();

        else if (skinVertexColorVue)
            VertexColorEditor();

        else if (skinFlattenVue || skinFlattenAllVue)
            FlattenEditor();

        else if (skinCheckerVue)
            CheckerEditor();

        else if (skinImportAssetVue)
            ImportAssetEditor();

        else if (skinAssetVue)
            AssetEditor();

        else if (skinUnwalkableByHeightVue)
            UnwalkableCellByHeightEditor();

        else if (skinCellVue)
            CellEditor();

        SmartKey();

        if (repaint)
        {
            repaint = false;
            Repaint();
        }
    }

    void SmartKey()
    {
        if (skinCellEditorVue && Event.current.keyCode == KeyCode.P && Event.current.type == EventType.KeyDown)
        {
            paintMode = !paintMode;
            Repaint();
        }
    }

    #region Tool Bar
    void ToolBarButton()
    {
        Rect buttonRect = new Rect(3, 0, 50, 18);
        if (GUI.Button(buttonRect, "File", EditorStyles.toolbarDropDown))
        {
            GenericMenu toolsMenu = new GenericMenu();
            toolsMenu.AddItem(new GUIContent("New"), false, NewGroundVue);
            toolsMenu.AddItem(new GUIContent("Load"), false, LoadGroundVue);
            toolsMenu.AddItem(new GUIContent("Load and creat"), false, LoadAndCreatGroundVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Save"), false, SaveGround);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("SaveCell"), false, SaveCellVue);

            Rect dropDownRect = new Rect(3, 3, 0, 16);
            toolsMenu.DropDown(dropDownRect);

            EditorGUIUtility.ExitGUI();
        }


        EditorGUI.BeginDisabledGroup(!groundCreat);
        buttonRect.x += 50;
        buttonRect.size = new Vector2(70, buttonRect.size.y);
        if (GUI.Button(buttonRect, "Checker", EditorStyles.toolbarDropDown))
        {
            GenericMenu toolsMenu = new GenericMenu();
            toolsMenu.AddItem(new GUIContent("Ground Data"), false, GroundDataVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Rebuild All"), false, RebuildAllGround);
            toolsMenu.AddItem(new GUIContent("Flatten All"), false, FlattenAllCellVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Vertex Color"), false, VertexColorVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Flatten Painter"), false, FlattenPainterVue);

            Rect dropDownRect = new Rect(53, 3, 0, 16);
            toolsMenu.DropDown(dropDownRect);

            EditorGUIUtility.ExitGUI();
        }

        EditorGUI.BeginDisabledGroup(!skinCellVue);
        buttonRect.x += 70;
        buttonRect.size = new Vector2(50, buttonRect.size.y);
        if (GUI.Button(buttonRect, "Cell", EditorStyles.toolbarDropDown))
        {
            GenericMenu toolsMenu = new GenericMenu();

            toolsMenu.AddItem(new GUIContent("Clean"), false, CleanCurrentCell);
            toolsMenu.AddItem(new GUIContent("Flatten"), false, FlattenVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Cell Editor Vue"), skinCellEditorVue, CellEditorVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Cell Data Vue"), skinCellDataVue, CellDataVue);
            toolsMenu.AddItem(new GUIContent("Flatten Cell Data"), false, FlattenWalkableCell);
            toolsMenu.AddItem(new GUIContent("Unwalkable by height"), false, UnwalkableByHeightVue);

            Rect dropDownRect = new Rect(123, 3, 0, 16);
            toolsMenu.DropDown(dropDownRect);

            EditorGUIUtility.ExitGUI();
        }

        buttonRect.x += 50;
        buttonRect.size = new Vector2(100, buttonRect.size.y);
        if (GUI.Button(buttonRect, "Custom Ground", EditorStyles.toolbarDropDown))
        {
            GenericMenu toolsMenu = new GenericMenu();

            toolsMenu.AddItem(new GUIContent("Assets Import"), false, AssetImportVue);
            toolsMenu.AddSeparator("");
            toolsMenu.AddItem(new GUIContent("Asset Vue"), skinAssetVue, SkinAssetVue);
            //toolsMenu.AddItem(new GUIContent("Clean Assets"),false,)

            Rect dropDownRect = new Rect(173, 3, 0, 16);
            toolsMenu.DropDown(dropDownRect);
        }
        EditorGUI.EndDisabledGroup();

        EditorGUI.EndDisabledGroup();
    }

    #region File
    void NewGroundVue()
    {
        skinNewGroundVue = true;
        skinDataGroundVue = false;
        repaint = true;
    }

    /// <summary>
    /// Disable
    /// </summary>
    void LoadGroundVue()
    {
        skinLoadGroundVue = true;
        repaint = true;
    }

    /// <summary>
    /// Disable
    /// </summary>
    void LoadAndCreatGroundVue()
    {
        skinLoadAndCreatGroundVue = true;
        skinLoadGroundVue = true;
        repaint = true;
    }

    void SaveGround()
    {
        if (!Directory.Exists("Assets/Script/Tools/GroundGenerator/Save/"))
        {
            Directory.CreateDirectory("Assets/Script/Tools/GroundGenerator/Save/" + nameMap);
        }

        height.Clear();
        foreach (GameObject obj in checker)
            height.Add(obj.GetComponent<GroundBaseGenerator>().MapDefinition);

        currentMapDataSave = new MapDataSave();
        currentMapDataSave.NameMap = nameMap;
        currentMapDataSave.CheckerOnTheLenght = checkerOnTheLenght;
        currentMapDataSave.CheckerOnTheHeight = checkerOnTheHeight;
        currentMapDataSave.CellByLenghtChecker = cellByLenghtChecker;
        currentMapDataSave.Density = cellDensity;
        currentMapDataSave.Height = height;
        currentMapDataSave.vertexColorRedValue = vertexColorRedValue;
        currentMapDataSave.vertexColorGreenValue = vertexColorGreenValue;
        currentMapDataSave.vertexColorBlueValue = vertexColorBlueValue;


        int index = 0;
        foreach (GameObject obj in checker)
        {
            obj.GetComponent<GroundBaseGenerator>().IndexInTheCheckboard = index;
            index++;
        }

        string json = JsonUtility.ToJson(currentMapDataSave, true);
        File.WriteAllText("Assets/Script/Tools/GroundGenerator/Save/" + nameMap, json);
        AssetDatabase.Refresh();

        Repaint();
    }

    void SaveCellVue()
    {
        skinSaveCellVue = true;
    }
    #endregion

    #region Checker
    void GroundDataVue()
    {
        initNameMap = nameMap;
        initCheckerOnTheLenght = checkerOnTheLenght;
        initCheckerOnTheHeight = checkerOnTheHeight;
        initCellByLenghtChecker = cellByLenghtChecker;
        initCellDensity = cellDensity;

        skinFlattenVue = false;
        skinFlattenAllVue = false;
        skinDataGroundVue = true;
        repaint = true;
    }

    void CancelGroundDataVue()
    {
        nameMap = initNameMap;
        checkerOnTheLenght = initCheckerOnTheLenght;
        checkerOnTheHeight = initCheckerOnTheHeight;
        cellByLenghtChecker = initCellByLenghtChecker;
        cellDensity = initCellDensity;

        skinDataGroundVue = false;
        repaint = true;
    }

    void FlattenAllCellVue()
    {
        skinFlattenVue = false;
        skinFlattenAllVue = true;
        skinflattenPainterVue = false;
        repaint = true;
    }

    void VertexColorVue()
    {
        skinflattenPainterVue = false;
        skinVertexColorVue = true;
        repaint = true;
    }

    void FlattenPainterVue()
    {
        skinCellDataVue = false;
        skinCellEditorVue = true;
        skinCellVue = false;
        skinVertexColorVue = false;
        skinflattenPainterVue = true;
        repaint = true;
    }
    #endregion

    #region cell
    void FlattenVue()
    {
        skinFlattenVue = true;
        repaint = true;
    }

    void CellDataVue()
    {
        skinCellEditorVue = false;
        skinCellDataVue = true;
        skinAssetVue = false;
        skinImportAssetVue = false;
        repaint = true;
    }

    void CellEditorVue()
    {
        skinCellEditorVue = true;
        skinCellDataVue = false;
        skinAssetVue = false;
        skinImportAssetVue = false;
        repaint = true;
    }

    void UnwalkableByHeightVue()
    {
        skinUnwalkableByHeightVue = true;
    }
    #endregion

    #region Custom ground
    void AssetImportVue()
    {
        if (skinCheckerVue)
            return;

        skinImportAssetVue = true;
        repaint = true;
    }

    void SkinAssetVue()
    {
        skinFlattenVue = false;
        skinFlattenAllVue = false;
        skinCellEditorVue = false;
        skinCellDataVue = false;
        skinAssetVue = true;
        repaint = true;
    }
    #endregion

    #endregion

    #region Skin Editor
    void NewGroundEditor()
    {
        Rect backgroundBorderRect = new Rect(MidlePos(new Vector2(456, 356)), new Vector2(456, 356));/////
        EditorGUI.DrawRect(backgroundBorderRect, borderColor);
        Rect backgroundRect = new Rect(MidlePos(new Vector2(450, 350)), new Vector2(450, 350));/////
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect headerRect = new Rect(backgroundRect.x + 10, backgroundRect.y + 10, 430, 20);/////
        if (skinDataGroundVue)
            GUI.Label(headerRect, "Ground Data", header);
        else
            GUI.Label(headerRect, "New Ground", header);

        #region Map
        Rect subtitleRect = new Rect(headerRect.x + 20, headerRect.y + 30, 410, 20);/////
        GUI.Label(subtitleRect, "Map", subtitle);

        Rect fieldBorderBackgroundRect = new Rect(subtitleRect.x + 10, subtitleRect.y + 30, 390, 20);/////
        EditorGUI.DrawRect(fieldBorderBackgroundRect, borderColor);
        Rect fieldBackgroundRect = new Rect(subtitleRect.x + 200, subtitleRect.y + 33, 197, 14);/////
        EditorGUI.DrawRect(fieldBackgroundRect, backgroundColor);

        Rect labelRec = fieldBorderBackgroundRect;/////
        labelRec.y += 3;
        labelRec.x += 3;
        GUI.Label(labelRec, "Map Name", field);
        Rect fieldRect = fieldBackgroundRect;/////
        fieldRect.x += 3;
        nameMap = EditorGUI.TextField(fieldRect, nameMap, field);
        #endregion

        #region Checker
        subtitleRect.y += 60;//20 Per Box +10 per line
        GUI.Label(subtitleRect, "Checker", subtitle);

        fieldBorderBackgroundRect.y += 60;///60 = Last box + new Laber + 2* border (10)
        EditorGUI.DrawRect(fieldBorderBackgroundRect, borderColor);
        fieldBackgroundRect.y += 60;
        EditorGUI.DrawRect(fieldBackgroundRect, backgroundColor);

        labelRec.y += 60;
        GUI.Label(labelRec, "Checker on the lenght", field);
        fieldRect.y += 60;
        //if (skinDataGround)
        //    GUI.Label(fieldRect, "" + checkerOnTheLenght, field);
        //else
        checkerOnTheLenght = EditorGUI.IntField(fieldRect, checkerOnTheLenght, field);
        checkerOnTheLenght = Mathf.Min(checkerOnTheLenght, 40);
        checkerOnTheLenght = Mathf.Max(checkerOnTheLenght, 1);

        fieldBorderBackgroundRect.y += 30;
        EditorGUI.DrawRect(fieldBorderBackgroundRect, borderColor);
        fieldBackgroundRect.y += 30;
        EditorGUI.DrawRect(fieldBackgroundRect, backgroundColor);

        labelRec.y += 30;
        GUI.Label(labelRec, "Checker on the height", field);
        fieldRect.y += 30;
        //if (skinDataGround)
        //    GUI.Label(fieldRect, "" + checkerOnTheHeight, field);
        //else
        checkerOnTheHeight = EditorGUI.IntField(fieldRect, checkerOnTheHeight, field);
        checkerOnTheHeight = Mathf.Min(checkerOnTheHeight, 40);
        checkerOnTheHeight = Mathf.Max(checkerOnTheHeight, 1);

        fieldBorderBackgroundRect.y += 30;
        EditorGUI.DrawRect(fieldBorderBackgroundRect, borderColor);
        fieldBackgroundRect.y += 30;
        EditorGUI.DrawRect(fieldBackgroundRect, backgroundColor);

        labelRec.y += 30;
        GUI.Label(labelRec, "Cell by checker on the lenght", field);
        fieldRect.y += 30;
        cellByLenghtChecker = EditorGUI.IntField(fieldRect, cellByLenghtChecker, field);
        cellByLenghtChecker = Mathf.Min(cellByLenghtChecker, 50);
        cellByLenghtChecker = Mathf.Max(cellByLenghtChecker, 5);
        #endregion

        #region Cell
        subtitleRect.y += 120;//20 Per Box +10 per line
        GUI.Label(subtitleRect, "Mesh", subtitle);

        fieldBorderBackgroundRect.y += 60;
        EditorGUI.DrawRect(fieldBorderBackgroundRect, borderColor);
        fieldBackgroundRect.y += 60;
        EditorGUI.DrawRect(fieldBackgroundRect, backgroundColor);

        labelRec.y += 60;
        GUI.Label(labelRec, "Cell Density", field);
        fieldRect.y += 60;
        cellDensity = EditorGUI.IntField(fieldRect, cellDensity, field);
        cellDensity = Mathf.Min(cellDensity, 10);
        cellDensity = Mathf.Max(cellDensity, 1);
        #endregion

        #region Validation
        Rect validationButtonRect = new Rect(backgroundRect.x + 20, backgroundRect.y + backgroundRect.size.y - 50, 120, 30);/////

        if (!skinDataGroundVue)
        {
            if (GUI.Button(validationButtonRect, "Cancel"))
                skinNewGroundVue = false;

            validationButtonRect.x += 290;
            if (GUI.Button(validationButtonRect, "Creat"))
            {
                CreatGround();
                skinNewGroundVue = false;
                skinCheckerVue = true;
            }
        }
        else
        {
            if (GUI.Button(validationButtonRect, "Cancel"))
                CancelGroundDataVue();
            validationButtonRect.x += 290;
            if (GUI.Button(validationButtonRect, "Rebuild"))
            {
                RebuildAndModifyChecker();
                skinNewGroundVue = false;
                skinDataGroundVue = false;
                skinCheckerVue = true;
            }
        }
        #endregion

    }

    void LoadGroundEditor()
    {
        Rect backgroundBorderRect = new Rect(MidlePos(new Vector2(406, 126)), new Vector2(406, 126));/////
        EditorGUI.DrawRect(backgroundBorderRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(400, 120)), new Vector2(400, 120));
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect labelRect = new Rect(backgroundRect.x + 10, backgroundRect.y + 10, 380, 20);
        EditorGUI.LabelField(labelRect, "Load ground", subtitle);

        Rect backgroundBorderFieldRect = new Rect(labelRect.x + 20, labelRect.y + 30, 340, 20);
        EditorGUI.DrawRect(backgroundBorderFieldRect, borderColor);

        Displacementfield(backgroundBorderFieldRect, true);
        if (skinLoadAndCreatGroundVue)
            GUI.Label(backgroundBorderFieldRect, "JSON Name");
        else
            GUI.Label(backgroundBorderFieldRect, "Ground folder");
        Displacementfield(backgroundBorderFieldRect, false);

        Rect backgroundFieldRect = new Rect(backgroundBorderFieldRect.x + 100, backgroundBorderFieldRect.y + 3, 237, 14);
        EditorGUI.DrawRect(backgroundFieldRect, backgroundColor);

        if (skinLoadAndCreatGroundVue)
            NameJSONFile = EditorGUI.TextField(backgroundFieldRect, NameJSONFile, field);
        else
        {
            if (!groundFolder && GameObject.Find("GroundFolder"))
            {
                groundFolder = GameObject.Find("GroundFolder");
                Selection.activeGameObject = groundFolder;
            }

            groundFolder = (GameObject)EditorGUI.ObjectField(backgroundFieldRect, groundFolder, typeof(GameObject), true);
        }


        Rect buttonRect = new Rect(backgroundRect.x + 30, backgroundRect.y + 75, 100, 30);
        if (GUI.Button(buttonRect, "Cancel"))
        {
            skinLoadGroundVue = false;
            skinLoadAndCreatGroundVue = false;
        }

        buttonRect.x += 240;
        if (GUI.Button(buttonRect, "Load"))// && currentMapDataSave
        {
            LoadGround();
        }
    }

    void SaveCellEditor()
    {
        Rect backgroundBorderRect = new Rect(MidlePos(new Vector2(406, 126)), new Vector2(406, 126));/////
        EditorGUI.DrawRect(backgroundBorderRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(400, 120)), new Vector2(400, 120));
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect labelRect = new Rect(backgroundRect.x + 10, backgroundRect.y + 10, 380, 20);
        EditorGUI.LabelField(labelRect, "Event Cell Folder", subtitle);

        Rect backgroundBorderFieldRect = new Rect(labelRect.x + 20, labelRect.y + 30, 340, 20);
        EditorGUI.DrawRect(backgroundBorderFieldRect, borderColor);

        Displacementfield(backgroundBorderFieldRect, true);
        GUI.Label(backgroundBorderFieldRect, "Folder");
        Displacementfield(backgroundBorderFieldRect, false);

        Rect backgroundFieldRect = new Rect(backgroundBorderFieldRect.x + 100, backgroundBorderFieldRect.y + 3, 237, 14);
        EditorGUI.DrawRect(backgroundFieldRect, backgroundColor);

        eventCellFolder = (GameObject)EditorGUI.ObjectField(backgroundFieldRect, eventCellFolder, typeof(GameObject), true);

        if (eventCellFolder && eventCellFolder.GetComponent<EventCell>())
            eventCell = eventCellFolder.GetComponent<EventCell>();

        Rect buttonRect = new Rect(backgroundRect.x + 30, backgroundRect.y + 75, 100, 30);
        if (GUI.Button(buttonRect, "Cancel"))
        {
            skinSaveCellVue = false;
        }

        buttonRect.x += 240;
        if (GUI.Button(buttonRect, "Save Cell") && eventCell)// && currentMapDataSave
        {
            eventCell.Ground = new Cell[(cellByLenghtChecker * cellByLenghtChecker) * (checkerOnTheHeight * checkerOnTheLenght)];

            int indexCell = 0;
            for (int indexChecker = 0; indexChecker < checker.Count; indexChecker++)
            {
                for (int y = 0; y < cellByLenghtChecker; y++)
                {
                    for (int x = 0; x < cellByLenghtChecker; x++)
                    {
                        eventCell.Ground[indexCell] = checker[indexChecker].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].CellsInformation[x];
                        eventCell.Ground[indexCell].CellContaint.Height = checker[indexChecker].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].Row[x];
                        indexCell++;
                    }
                }
            }
            skinSaveCellVue = false;
        }
    }

    void CheckerEditor()
    {
        Rect buttonRect = ButtonRect(20, checkerOnTheLenght, position.size.y - 28, checkerOnTheHeight, 20);
        buttonRect.y -= buttonRect.size.y;
        int index = 0;

        for (int y = checkerOnTheHeight; y > 0; y--)
        {
            if (y != checkerOnTheHeight)
                buttonRect.y -= buttonRect.size.y + 5;

            buttonRect.x = 20;

            for (int x = 0; x < checkerOnTheLenght; x++)
            {
                if (x != 0)
                    buttonRect.x += buttonRect.size.x + 5;
                if (GUI.Button(buttonRect, "" + index))
                {
                    skinCellVue = true;

                    indexCurrentGroundOnTheLenght = x;
                    indexCurrentGroundOnTheHeight = Mathf.Abs(y - checkerOnTheHeight);

                    currentGround = checker[index];
                    indexCurrentChecker = index;
                    Selection.activeGameObject = currentGround;
                    currentHeightGround = currentGround.GetComponent<GroundBaseGenerator>().MapDefinition;
                    skinCheckerVue = false;
                }
                index++;
            }
        }
    }

    void CellEditor()
    {
        Rect verticalToolBar = new Rect(0, 18, 53, position.size.y);//////
        EditorGUI.DrawRect(verticalToolBar, borderColor);
        verticalToolBar = new Rect(0, 18, 50, position.size.y);
        EditorGUI.DrawRect(verticalToolBar, backgroundColor);

        Rect verticalToolBarButton = new Rect(5, 23, 40, 40);//////
        if (GUI.Button(verticalToolBarButton, "Back"))
        {
            paintMode = false;
            skinCheckerVue = true;
            skinCellVue = false;
        }

        #region Cell ground
        if (skinCellEditorVue)
        {
            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Build"))
            {
                RebuildCurrentChecker();
            }

            verticalToolBarButton.y += 45;
            if (!paintMode)
            {
                if (GUI.Button(verticalToolBarButton, "Paint"))
                {
                    paintMode = true;
                }
            }
            else if (GUI.Button(verticalToolBarButton, "P"))
            {
                paintMode = false;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Data"))
            {
                skinCellEditorVue = false;
                skinCellDataVue = true;
            }
        }
        #endregion

        #region Cell Data
        else if (skinCellDataVue)
        {
            verticalToolBarButton.y += 45;
            if (walkableMode)
            {
                if (GUI.Button(verticalToolBarButton, "Unwalkable"))
                {
                    walkableMode = false;
                    groundEllementVue = false;
                }
            }
            else
            {
                if (GUI.Button(verticalToolBarButton, "walkable"))
                {
                    walkableMode = true;
                    groundEllementVue = false;
                }
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Earth"))
            {
                currentGroundElement = Cell.GroundElement.Earth;
                groundEllementVue = true;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Ice"))
            {
                currentGroundElement = Cell.GroundElement.Ice;
                groundEllementVue = true;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Fire"))
            {
                currentGroundElement = Cell.GroundElement.Fire;
                groundEllementVue = true;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Water"))
            {
                currentGroundElement = Cell.GroundElement.Water;
                groundEllementVue = true;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Sand"))
            {
                currentGroundElement = Cell.GroundElement.Sand;
                groundEllementVue = true;
            }

            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, "Editor"))
            {
                skinCellDataVue = false;
                skinCellEditorVue = true;
            }
        }
        #endregion

        #region Cell
        Rect cellButtonRect = ButtonRect(70, cellByLenghtChecker, (position.size.y - 20), cellByLenghtChecker, 5, position.size.x - 70, position.size.y - 18);
        cellButtonRect.y -= cellButtonRect.size.y;
        Rect cellFieldRect = CenterPosRect(cellButtonRect);
        float initialPosXCellFieldRect = cellFieldRect.x;

        for (int y = cellByLenghtChecker; y > 0; y--)
        {
            cellButtonRect.x = 70;
            cellFieldRect.x = initialPosXCellFieldRect;

            if (y != cellByLenghtChecker)
            {
                cellButtonRect.y -= cellButtonRect.size.y + 5;
                cellFieldRect.y -= cellButtonRect.size.y + 5;
            }

            for (int x = 0; x < cellByLenghtChecker; x++)
            {

                #region Cell Ground
                if (!skinCellDataVue)
                {
                    if (!paintMode)
                    {
                        EditorGUI.DrawRect(cellButtonRect, backgroundColor);
                        currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].Row[x] =
                            EditorGUI.FloatField(cellFieldRect, currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].Row[x], field);
                    }

                    if (paintMode)
                    {
                        if (cellPaint.Contains(new Vector2(y, x)))
                            EditorGUI.DrawRect(cellButtonRect, borderColor);
                        else
                            EditorGUI.DrawRect(cellButtonRect, backgroundColor);

                        GUI.Label(cellFieldRect, "" + currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].Row[x], field);

                        if (mouseClicked)
                        {
                            if (cellButtonRect.Contains(Event.current.mousePosition))
                            {
                                if (!valueChoice)
                                {
                                    valueChoice = true;
                                    paintValue = currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].Row[x];
                                }
                                else if (valueChoice && !cellPaint.Contains(new Vector2(y, x)))
                                {
                                    cellPaint.Add(new Vector2(y, x));
                                    currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].Row[x] = paintValue;
                                    Repaint();
                                }
                            }
                        }
                        else if (!mouseClicked && valueChoice)
                        {
                            cellPaint.Clear();
                            Repaint();
                            valueChoice = false;
                        }

                    }
                }
                #endregion

                #region Cell Data
                if (skinCellDataVue)
                {
                    if (mouseClicked)
                    {
                        if (walkableModeVue && !groundEllementVue)
                        {
                            if (cellButtonRect.Contains(Event.current.mousePosition) && cellButtonRect.position != lastCellPos)
                            {
                                lastCellPos = cellButtonRect.position;
                                currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.Walkable = walkableMode;
                                Repaint();
                            }
                        }
                        else
                        {
                            if (cellButtonRect.Contains(Event.current.mousePosition) && cellButtonRect.position != lastCellPos)
                            {
                                lastCellPos = cellButtonRect.position;
                                currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut = currentGroundElement;
                                Repaint();
                            }
                        }
                    }

                    if (!currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.Walkable)
                        EditorGUI.DrawRect(cellButtonRect, Color.black);
                    else if (currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut == Cell.GroundElement.Earth)
                        EditorGUI.DrawRect(cellButtonRect, browColor);
                    else if (currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut == Cell.GroundElement.Ice)
                        EditorGUI.DrawRect(cellButtonRect, Color.cyan);
                    else if (currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut == Cell.GroundElement.Fire)
                        EditorGUI.DrawRect(cellButtonRect, Color.red);
                    else if (currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut == Cell.GroundElement.Water)
                        EditorGUI.DrawRect(cellButtonRect, Color.blue);
                    else if (currentHeightGround.MapRowsData[Mathf.Abs(y - cellByLenghtChecker)].CellsInformation[x].CellContaint.GroundAtribut == Cell.GroundElement.Sand)
                        EditorGUI.DrawRect(cellButtonRect, Color.yellow);
                    else
                        EditorGUI.DrawRect(cellButtonRect, borderColor);
                }
                #endregion

                cellButtonRect.x += cellButtonRect.size.x + 5;
                cellFieldRect.x += cellButtonRect.size.x + 5;
            }
        }
        #endregion

    }

    /// <summary>
    /// Flatten current checker or all chacker
    /// </summary>
    void FlattenEditor()
    {
        Rect backgroundBorderRect = new Rect(MidlePos(new Vector2(306, 156)), new Vector2(306, 156));
        EditorGUI.DrawRect(backgroundBorderRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(300, 150)), new Vector2(300, 150));
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect labelRect = new Rect(backgroundRect);
        labelRect.x += 20;
        labelRect.y += 10;
        GUI.Label(labelRect, "Flatten Editor", subtitle);

        Rect borderFielRect = new Rect(labelRect.x + 20, labelRect.y + 30, labelRect.size.x - 80, 20);
        EditorGUI.DrawRect(borderFielRect, borderColor);

        Rect backgroundFielRect = new Rect(borderFielRect.x + 100, borderFielRect.y + 3, 117, 14);
        EditorGUI.DrawRect(backgroundFielRect, backgroundColor);

        Displacementfield(borderFielRect, true);
        GUI.Label(borderFielRect, "Flatten Value");

        backgroundFielRect.x += 3;
        flattenCellValueCheckboard = EditorGUI.FloatField(backgroundFielRect, flattenCellValueCheckboard, field);

        Rect buttonRect = new Rect(backgroundRect.x + 20, backgroundRect.y + 100, 100, 30);
        if (GUI.Button(buttonRect, "Cancel"))
        {
            skinFlattenVue = false;
            skinFlattenAllVue = false;
        }

        buttonRect.x += 160;
        if (GUI.Button(buttonRect, "Flatten"))
        {
            if (skinFlattenVue)
                FlattenCurrentCheckboard();
            else
                FlattenAllCheckboard();

            skinFlattenVue = false;
            skinFlattenAllVue = false;
        }
    }

    void FlattenPainterEditor()
    {
        Rect leftMenuRect = new Rect(0, 18, 53, position.size.y);/////
        EditorGUI.DrawRect(leftMenuRect, borderColor);

        leftMenuRect.size = new Vector2(50, position.size.y);
        EditorGUI.DrawRect(leftMenuRect, backgroundColor);

        Rect leftMenuButtonRect = new Rect(5, 23, 40, 40);
        if (GUI.Button(leftMenuButtonRect, "Apply"))
        {
            for (int i = 0; i < groundModifyByFlattenPainter.Length; i++)
            {
                if (!groundModifyByFlattenPainter[i])
                    continue;

                for (int w = 0; w < cellByLenghtChecker; w++)
                    for (int z = 0; z < cellByLenghtChecker; z++)
                    {
                        groundModifyByFlattenPainter[i].MapDefinition.MapRowsData[w].Row[z] = flattenValueChecker[i];
                    }
            }

            groundModifyByFlattenPainter = new GroundBaseGenerator[checker.Count];
            RebuildAllGround();
            Repaint();
        }

        leftMenuButtonRect.y += 45;
        if (GUI.Button(leftMenuButtonRect, "Back"))
        {
            groundModifyByFlattenPainter = new GroundBaseGenerator[checker.Count];
            skinflattenPainterVue = false;
            skinCheckerVue = true;
            Repaint();
        }

        leftMenuButtonRect.y += 45;
        EditorGUI.DrawRect(leftMenuButtonRect, borderColor);
        flattenPainterValueAsigned = EditorGUI.FloatField(leftMenuButtonRect, flattenPainterValueAsigned); //CenterPosRect(leftMenuButtonRect, 20, 20)

        Rect checkerRect = new Rect(ButtonRect(73, checkerOnTheLenght, position.size.y - 10, checkerOnTheHeight, 5, position.size.x - 93));
        float initialPoseX = checkerRect.x;
        checkerRect.y -= checkerRect.size.y;

        #region Checker
        int indexChecker = 0;
        for (int y = 0; y < checkerOnTheHeight; y++)
        {
            checkerRect.x = initialPoseX;
            if (y != 0)
                checkerRect.y -= checkerRect.size.y + 5;

            for (int x = 0; x < checkerOnTheLenght; x++)
            {
                #region Display Rect
                if (!indexCheckerPaint.Contains(indexChecker))
                    EditorGUI.DrawRect(checkerRect, backgroundColor);
                else
                    EditorGUI.DrawRect(checkerRect, borderColor);

                GUI.Label(CenterPosRect(checkerRect, 20, 20), "" + flattenValueChecker[indexChecker]);
                #endregion

                #region Painter
                if (mouseClicked && !indexCheckerPaint.Contains(indexChecker))//Mouse clicked and current cell is painted ?
                {
                    if (checkerRect.Contains(Event.current.mousePosition))//Mouse on the cell ?
                    {
                        if (!groundModifyByFlattenPainter[indexChecker])//The array to build the ground when all is validate
                            groundModifyByFlattenPainter[indexChecker] = checker[indexChecker].GetComponent<GroundBaseGenerator>();

                        flattenValueChecker[indexChecker] = flattenPainterValueAsigned;//The value of the flatten and use to display on the checker
                        indexCheckerPaint.Add(indexChecker);//Add the rect to display the selection color
                        Repaint();
                    }
                }
                else if (!mouseClicked && indexCheckerPaint.Count > 0)
                {
                    indexCheckerPaint.Clear();
                    Repaint();
                }
                #endregion

                checkerRect.x += checkerRect.size.x + 5;
                indexChecker++;
            }
        }
        #endregion

    }

    void VertexColorEditor()
    {
        Rect borderBackgroundRect = new Rect(MidlePos(new Vector2(356, 206)), new Vector2(356, 206));/////
        EditorGUI.DrawRect(borderBackgroundRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(350, 200)), new Vector2(350, 200));/////
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect subtitleRect = new Rect(backgroundRect.x + 10, backgroundRect.y + 10, 330, 20);/////
        GUI.Label(subtitleRect, "Vertex Color Editor", subtitle);

        #region Field
        Rect backgroundBorderFieldRect = new Rect(subtitleRect.x + 10, subtitleRect.y + 30, 310, 20);/////
        EditorGUI.DrawRect(backgroundBorderFieldRect, borderColor);

        Rect backgroundFieldRect = new Rect(backgroundBorderFieldRect.x + 150, backgroundBorderFieldRect.y + 3, 157, 14);
        EditorGUI.DrawRect(backgroundFieldRect, backgroundColor);

        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, true);
        GUI.Label(backgroundBorderFieldRect, "Red Color", field);
        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, false);

        backgroundFieldRect.x += 3;
        vertexColorRedValue = EditorGUI.FloatField(backgroundFieldRect, vertexColorRedValue, field);
        backgroundFieldRect.x -= 3;

        ///

        backgroundBorderFieldRect.y += 30;
        EditorGUI.DrawRect(backgroundBorderFieldRect, borderColor);

        backgroundFieldRect.y += 30;
        EditorGUI.DrawRect(backgroundFieldRect, backgroundColor);

        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, true);
        GUI.Label(backgroundBorderFieldRect, "Green Color", field);
        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, false);

        backgroundFieldRect.x += 3;
        vertexColorGreenValue = EditorGUI.FloatField(backgroundFieldRect, vertexColorGreenValue, field);
        backgroundFieldRect.x -= 3;

        ///

        backgroundBorderFieldRect.y += 30;
        EditorGUI.DrawRect(backgroundBorderFieldRect, borderColor);

        backgroundFieldRect.y += 30;
        EditorGUI.DrawRect(backgroundFieldRect, backgroundColor);

        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, true);
        GUI.Label(backgroundBorderFieldRect, "Blue Color", field);
        backgroundBorderFieldRect = Displacementfield(backgroundBorderFieldRect, false);

        backgroundFieldRect.x += 3;
        vertexColorBlueValue = EditorGUI.FloatField(backgroundFieldRect, vertexColorBlueValue, field);
        backgroundFieldRect.x -= 3;
        #endregion

        #region Button
        Rect buttonRect = new Rect(backgroundRect.x + 20, backgroundRect.y + backgroundRect.size.y - 50, 100, 30);
        if (GUI.Button(buttonRect, "Cancel"))
        {
            skinVertexColorVue = false;
        }

        buttonRect.x += 210;
        if (GUI.Button(buttonRect, "Apply"))
        {
            ApplyVertexColor();
            skinVertexColorVue = false;
        }
        #endregion
    }

    void ImportAssetEditor()
    {
        Rect borderRect = new Rect(MidlePos(new Vector2(406, 306)), new Vector2(406, 306));
        EditorGUI.DrawRect(borderRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(400, 300)), new Vector2(400, 300));
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect susbtitleRect = new Rect(backgroundRect.x + 10, backgroundRect.y + 10, 380, 20);
        GUI.Label(susbtitleRect, "Import Assets Editor", subtitle);

        Rect borderFieldRect = new Rect(susbtitleRect.x + 10, susbtitleRect.y + 30, 360, 20);
        Rect backgroundObjFieldRect = new Rect(susbtitleRect.x + 13, susbtitleRect.y + 33, 304, 14);
        Rect backgroundFloatFieldRect = new Rect(susbtitleRect.x + 323, susbtitleRect.y + 33, 40, 14);

        for (int i = 0; i < 6; i++)
        {
            EditorGUI.DrawRect(borderFieldRect, borderColor);
            EditorGUI.DrawRect(backgroundObjFieldRect, backgroundColor);
            EditorGUI.DrawRect(backgroundFloatFieldRect, backgroundColor);

            assetsImport[i] = (GameObject)EditorGUI.ObjectField(backgroundObjFieldRect, assetsImport[i], typeof(GameObject), true);

            if (assetsImport[i] && assetFoot[i] == 0)
            {
                assetFoot[i] = CalculateBoundingBoxSize(assetsImport[i]) / 2f;
                EditorGUI.LabelField(backgroundFloatFieldRect, "" + assetFoot[i]);
            }
            else if (assetsImport[i])
                assetFoot[i] = EditorGUI.FloatField(backgroundFloatFieldRect, assetFoot[i]);

            if (assetsImport[i] && assetsImport[i].GetComponent<EventCell>() == null)
                assetsImport[i] = null;

            if (AssetPreview.GetAssetPreview(assetsImport[i]) && !Directory.Exists("Assets/Script/Tools/GroundGenerator/PreviewGameObject/" + assetsImport[i].name + "Preview.jpg"))
            {
                File.WriteAllBytes("Assets/Script/Tools/GroundGenerator/PreviewGameObject/" + assetsImport[i].name + "Preview.jpg", AssetPreview.GetAssetPreview(assetsImport[i]).EncodeToPNG());
            }
            if (AssetPreview.GetAssetPreview(assetsImport[i]))
            {
                assetPreview[i] = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Script/Tools/GroundGenerator/PreviewGameObject/" + assetsImport[i].name + "Preview.jpg", typeof(Texture2D));
            }

            borderFieldRect.y += 30;
            backgroundObjFieldRect.y += 30;
            backgroundFloatFieldRect.y += 30;
        }

        Rect buttonRect = new Rect(backgroundRect.x + backgroundRect.size.x / 2 - 150, backgroundRect.y + backgroundRect.size.y - 50, 100, 35);
        if (GUI.Button(buttonRect, "Back"))
            skinImportAssetVue = false;

        buttonRect.x += 200;
        if (GUI.Button(buttonRect, "Assets Vue"))
        {
            skinImportAssetVue = false;
            SkinAssetVue();
        }
    }

    void AssetEditor()
    {
        Rect verticalToolBar = new Rect(0, 18, 53, position.size.y);//////
        EditorGUI.DrawRect(verticalToolBar, borderColor);
        verticalToolBar = new Rect(0, 18, 50, position.size.y);
        EditorGUI.DrawRect(verticalToolBar, backgroundColor);

        #region Left Bar
        Rect verticalToolBarButton = new Rect(5, 23, 40, 40);//////
        if (GUI.Button(verticalToolBarButton, "Back"))
        {
            skinCheckerVue = true;
            skinCellVue = false;
        }

        for (int assetsIndex = 0; assetsIndex < 6; assetsIndex++)
        {
            verticalToolBarButton.y += 45;
            if (GUI.Button(verticalToolBarButton, assetPreview[assetsIndex]))
            {
                if (assetsImport[assetsIndex] != null)//Cell containt any object ?
                {
                    currentAsset = assetsImport[assetsIndex];
                    currentAssetPreview = assetPreview[assetsIndex];
                    currentAssetIndex = assetsIndex;
                    cellEraserMode = false;
                }
                else //Load the import assets editor
                    AssetImportVue();
            }
        }

        verticalToolBarButton.y += 45;
        if (cellEraserMode)
        {
            if (GUI.Button(verticalToolBarButton, "E"))
            {
                cellEraserMode = false;
            }
        }
        else
        {
            if (GUI.Button(verticalToolBarButton, "Eraser"))
            {
                cellEraserMode = true;
            }
        }
        #endregion

        #region Cell
        Rect cellRect = ButtonRect(63, cellByLenghtChecker, position.size.y, cellByLenghtChecker, 5, position.size.x - 53);//////
        cellRect.y -= cellRect.size.y + 10;
        float initialXPos = cellRect.x;

        for (int y = 0; y < cellByLenghtChecker; y++)
        {
            if (y != 0)
                cellRect.y -= cellRect.size.y + 5;

            cellRect.x = initialXPos;

            for (int x = 0; x < cellByLenghtChecker; x++)
            {
                if (x != 0)
                    cellRect.x += cellRect.size.x;

                #region Instantiate Mode
                if (!cellEraserMode)
                {
                    if (cellRect.Contains(Event.current.mousePosition))
                    {
                        if (mouseClicked && currentAsset && currentHeightGround.MapRowsData[y].PreviewCell[x] != currentAssetPreview)
                        {
                            if (currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.EventScript)
                                DestroyImmediate(currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.EventScript.gameObject);

                            currentHeightGround.MapRowsData[y].PreviewCell[x] = currentAssetPreview;

                            Vector3 cellPos =
                                new Vector3(x + (cellByLenghtChecker * indexCurrentGroundOnTheLenght) + .5f,
                                currentHeightGround.MapRowsData[y].Row[x] + assetFoot[currentAssetIndex],
                                y + (cellByLenghtChecker * indexCurrentGroundOnTheHeight) + .5f);

                            GameObject newObject = GameObject.Instantiate<GameObject>(currentAsset, cellPos, Quaternion.identity, currentGround.transform);

                            currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.EventScript = newObject.GetComponent<EventCell>();
                            currentHeightGround.MapRowsData[y].FootPos[x] = assetFoot[currentAssetIndex];
                            Repaint();
                        }
                    }
                }
                #endregion

                #region Eraser Mode
                else
                {
                    if (mouseClicked && cellRect.Contains(Event.current.mousePosition) && currentHeightGround.MapRowsData[y].PreviewCell[x])
                    {
                        currentHeightGround.MapRowsData[y].PreviewCell[x] = null;
                        DestroyImmediate(currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.EventScript.gameObject);
                        Repaint();
                    }
                }
                #endregion

                #region rect draw
                if (!currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.EventScript)
                    EditorGUI.DrawRect(cellRect, borderColor);
                else
                    EditorGUI.DrawPreviewTexture(cellRect, currentHeightGround.MapRowsData[y].PreviewCell[x]);
                #endregion

                cellRect.x += 5;
            }
        }
        #endregion
    }

    void UnwalkableCellByHeightEditor()
    {
        Rect backgroundBorderRect = new Rect(MidlePos(new Vector2(306, 156)), new Vector2(306, 156));
        EditorGUI.DrawRect(backgroundBorderRect, borderColor);

        Rect backgroundRect = new Rect(MidlePos(new Vector2(300, 150)), new Vector2(300, 150));
        EditorGUI.DrawRect(backgroundRect, backgroundColor);

        Rect labelRect = new Rect(backgroundRect);
        labelRect.x += 20;
        labelRect.y += 10;
        GUI.Label(labelRect, "Unwalkable Editor", subtitle);

        Rect borderFielRect = new Rect(labelRect.x + 20, labelRect.y + 30, labelRect.size.x - 80, 20);
        EditorGUI.DrawRect(borderFielRect, borderColor);

        Rect backgroundFielRect = new Rect(borderFielRect.x + 100, borderFielRect.y + 3, 117, 14);
        EditorGUI.DrawRect(backgroundFielRect, backgroundColor);

        Displacementfield(borderFielRect, true);
        GUI.Label(borderFielRect, "Max Height");

        backgroundFielRect.x += 3;
        maxHeightWalkableValue = EditorGUI.FloatField(backgroundFielRect, maxHeightWalkableValue, field);

        Rect buttonRect = new Rect(backgroundRect.x + 20, backgroundRect.y + 100, 100, 30);
        if (GUI.Button(buttonRect, "Cancel"))
        {
            skinUnwalkableByHeightVue = false;
        }

        buttonRect.x += 160;
        if (GUI.Button(buttonRect, "Apply"))
        {
            UnwalkableCellByHeight(maxHeightWalkableValue);
            skinUnwalkableByHeightVue = false;
        }
    }

    #endregion

    #region Skin Constructor
    Vector2 MidlePos(Vector2 SizeRect, bool yBorder = true, bool xBorder = false)
    {
        float xPos;
        float yPos;

        if (xBorder)
            xPos = ((position.size.x + 53) / 2) - (SizeRect.x / 2);
        else
            xPos = (position.size.x / 2) - (SizeRect.x / 2);

        if (yBorder)
            yPos = ((position.size.y + 18) / 2) - (SizeRect.y / 2);
        else
            yPos = (position.size.y / 2) - (SizeRect.y / 2);

        return new Vector2(xPos, yPos);
    }

    /// <summary>
    /// Creat a Rect with correct proportion for checker
    /// </summary>
    /// <param name="XPos">X Position of the new rect</param>
    /// <param name="numberXCell"></param>
    /// <param name="YPos">Y Position of the new rect</param>
    /// <param name="numberYCell"></param>
    /// <param name="Margin">Margin in right and left</param>
    /// <param name="SizeX">Background Size X : Don't complet for all space</param>
    /// <param name="SizeY">Background Size Y : Don't complet for all space</param>
    /// <returns></returns>
    Rect ButtonRect(float XPos, int numberXCell, float YPos, int numberYCell, float Margin, float SizeX = 0, float SizeY = 0)
    {
        if (SizeX == 0)
            SizeX = position.size.x;

        if (SizeY == 0)
            SizeY = position.size.y;

        float xSize = (SizeX - (Margin * 2) - ((numberXCell + 1) * 5)) / numberXCell;
        float ySize = (SizeY - 18 - (Margin * 2) - ((numberYCell + 1) * 5)) / numberYCell;
        return new Rect(XPos, YPos, xSize, ySize);
    }

    /// <summary>
    /// Calculate and return a new rect who start in the center
    /// </summary>
    /// <param name="XPos">Current x Pos</param>
    /// <param name="YPos">Current y Pos</param>
    /// <param name="XSize">Current x Size</param>
    /// <param name="YSize">Current y Size</param>
    /// <param name="NewXSize">New x Size (10)</param>
    /// <param name="NewYSize">New y Size (10)</param>
    /// <returns></returns>
    Rect CenterPosRect(float XPos, float YPos, float XSize, float YSize, float NewXSize = 10, float NewYSize = 10)
    {
        float xPos = ((XSize / 2) + XPos) - (NewXSize / 2);
        float yPos = ((YSize / 2) + YPos) - (NewYSize / 2);

        return new Rect(xPos, yPos, NewXSize, NewYSize);
    }

    /// <summary>
    /// Calculate and return a new rect who start in the center
    /// </summary>
    /// <param name="CurrentRect"></param>
    /// <param name="NewXSize">New x Size (10)</param>
    /// <param name="NewYSize">New y Size (10)</param>
    /// <returns></returns>
    Rect CenterPosRect(Rect CurrentRect, float NewXSize = 10, float NewYSize = 10)
    {
        float xPos = ((CurrentRect.size.x / 2) + CurrentRect.position.x) - (NewXSize / 2);
        float yPos = ((CurrentRect.size.y / 2) + CurrentRect.position.y) - (NewYSize / 2);

        return new Rect(xPos, yPos, NewXSize, NewYSize);
    }

    /// <summary>
    /// Move Of 3 pixels in right or left
    /// </summary>
    /// <param name="Current">Current Rect</param>
    /// <param name="Dirrection">True = move right</param>
    /// <returns></returns>
    Rect Displacementfield(Rect Current, bool Dirrection)
    {
        if (Dirrection)
        {
            Current.x += 3;
            Current.y += 3;
        }
        else
        {
            Current.x -= 3;
            Current.y -= 3;
        }

        return Current;
    }
    #endregion

    #region Ground
    void FlattenCurrentCheckboard()
    {
        for (int y = 0; y < cellByLenghtChecker; y++)
            for (int x = 0; x < cellByLenghtChecker; x++)
            {
                currentHeightGround.MapRowsData[y].Row[x] = flattenCellValueCheckboard;
            }
    }

    void FlattenAllCheckboard()
    {
        foreach (HeightGround heightCell in height)
        {
            for (int y = 0; y < cellByLenghtChecker; y++)
                for (int x = 0; x < cellByLenghtChecker; x++)
                {
                    heightCell.MapRowsData[y].Row[x] = flattenCellValueCheckboard;
                }
        }
        RebuildAllGround();
    }

    void CreatGround()
    {
        if (!groundFolder)
        {
            groundFolder = new GameObject();
            groundFolder.transform.position = Vector3.zero;
            groundFolder.name = "GroundFolder";
            checker.Clear();
        }
        else
        {
            foreach (GameObject obj in checker)
                DestroyImmediate(obj);
            checker.Clear();

        }

        int cell = cellByLenghtChecker;
        int index = 0;
        for (int y = 0; y < checkerOnTheHeight; y++)
            for (int x = 0; x < checkerOnTheLenght; x++)
            {
                #region Creation of the flatten ground
                checker.Add(new GameObject());
                checker[index].transform.parent = groundFolder.transform;
                checker[index].name = "" + nameMap + " " + index;
                GroundBaseGenerator groundScript = checker[index].AddComponent<GroundBaseGenerator>();
                #endregion

                #region Initialisation
                groundScript.NumberCellByLenght = cell;
                groundScript.Density = cellDensity;
                if (!skinLoadGroundVue)
                {
                    groundScript.MapDefinition = new HeightGround();
                    height.Add(groundScript.MapDefinition);
                    groundScript.MapDefinition.InitialisationRowArray(cell);
                }
                else//Load ?
                {
                    groundScript.MapDefinition = height[index];
                }
                #endregion

                groundScript.GenerateGroundBase();
                groundScript.IndexInTheCheckboard = index;

                checker[index].transform.position = new Vector3(x * cellByLenghtChecker, 0, y * cellByLenghtChecker);
                checker[index].GetComponent<MeshRenderer>().material = CheckerMaterial;
                index++;
            }

        flattenValueChecker = new float[checker.Count];//Use to write the value of the checker after flatten
        groundModifyByFlattenPainter = new GroundBaseGenerator[checker.Count];

        InitialisationBorderArray();//Initialisation of the size array
        CalculateHeightBorder();//Calculate the eatch border checker
        ApplyVertexColor();//Calculate the vertex color and build after

        groundCreat = true;
    }

    /// <summary>
    /// Rebuild and modify all checker
    /// </summary>
    void RebuildAndModifyChecker()
    {
        #region Cell
        foreach (GameObject obj in checker)
        {
            GroundBaseGenerator script = obj.GetComponent<GroundBaseGenerator>();
            script.NumberCellByLenght = cellByLenghtChecker;
            script.Density = cellDensity;
            script.MapDefinition.NewRowArray(cellByLenghtChecker);
        }

        CalculateHeightBorder();

        foreach (GameObject obj in checker)
            obj.GetComponent<GroundBaseGenerator>().GenerateGroundBase();
        #endregion

        #region Checker
        int newNumberCheckerIncreaseOnTheLenght = checkerOnTheLenght - initCheckerOnTheLenght;//2-4

        if (newNumberCheckerIncreaseOnTheLenght > 0)
        {
            int indexCheckerOnTheLenghtPos = initCheckerOnTheLenght;//2

            for (int y = 0; y < initCheckerOnTheHeight; y++)
            {
                if (y != 0)
                    indexCheckerOnTheLenghtPos += initCheckerOnTheLenght + newNumberCheckerIncreaseOnTheLenght;

                for (int x = 0; x < newNumberCheckerIncreaseOnTheLenght; x++)
                {
                    checker.Insert(indexCheckerOnTheLenghtPos + x, new GameObject());
                    GameObject newChecker = checker[indexCheckerOnTheLenghtPos + x];
                    newChecker.transform.parent = groundFolder.transform;
                    newChecker.name = "Extended Checker" + (indexCheckerOnTheLenghtPos + x);

                    GroundBaseGenerator script = newChecker.AddComponent<GroundBaseGenerator>();
                    script.NumberCellByLenght = cellByLenghtChecker;
                    script.Density = cellDensity;
                    script.MapDefinition = new HeightGround();
                    script.MapDefinition.InitialisationRowArray(cellByLenghtChecker);
                    script.GenerateGroundBase();

                    newChecker.GetComponent<MeshRenderer>().material = CheckerMaterial;
                }
            }
        }

        else if (newNumberCheckerIncreaseOnTheLenght < 0)
        {
            int absNewNumberCheckerIncreaseOnTheLenght = Mathf.Abs(newNumberCheckerIncreaseOnTheLenght);
            int initialSizeArrayChecker = checker.Count;

            for (int y = 0; y < initCheckerOnTheHeight; y++)
            {
                if (y != 0)
                {
                    initialSizeArrayChecker -= checkerOnTheLenght;
                }

                for (int x = 0; x < absNewNumberCheckerIncreaseOnTheLenght; x++)
                {
                    initialSizeArrayChecker -= 1;
                    DestroyImmediate(checker[initialSizeArrayChecker]);
                    checker.RemoveAt(initialSizeArrayChecker);

                }
            }
        }

        int newNumberCheckerIncreaseOnTheHeight = checkerOnTheHeight - initCheckerOnTheHeight;//2-4

        if (newNumberCheckerIncreaseOnTheHeight > 0)
        {
            for (int y = 0; y < newNumberCheckerIncreaseOnTheHeight; y++)
            {
                for (int x = 0; x < checkerOnTheLenght; x++)
                {
                    checker.Add(new GameObject());

                    GameObject newChecker = checker[checker.Count - 1];
                    newChecker.transform.parent = groundFolder.transform;
                    newChecker.name = "Extended Checker" + checker.Count;

                    GroundBaseGenerator script = newChecker.AddComponent<GroundBaseGenerator>();
                    script.NumberCellByLenght = cellByLenghtChecker;
                    script.Density = cellDensity;
                    script.MapDefinition = new HeightGround();
                    script.MapDefinition.InitialisationRowArray(cellByLenghtChecker);
                    script.GenerateGroundBase();

                    newChecker.GetComponent<MeshRenderer>().material = CheckerMaterial;
                }
            }
        }

        else if (newNumberCheckerIncreaseOnTheHeight < 0)
        {
            int absNewNumberCheckerIncreaseOnTheHeight = Mathf.Abs(newNumberCheckerIncreaseOnTheHeight);
            for (int y = 0; y < absNewNumberCheckerIncreaseOnTheHeight; y++)
            {
                for (int x = 0; x < checkerOnTheLenght; x++)
                {
                    DestroyImmediate(checker[checker.Count - 1]);
                    checker.RemoveAt(checker.Count - 1);
                }
            }
        }

        InitialisationBorderArray();
        CalculateHeightBorder();

        RebuildAllGround();
        #endregion

        #region Placement
        int index = 0;
        for (int y = 0; y < checkerOnTheHeight; y++)
        {
            for (int x = 0; x < checkerOnTheLenght; x++)
            {
                checker[index].transform.position = new Vector3(x * (cellByLenghtChecker), 0, y * (cellByLenghtChecker));
                index++;
            }
        }
        #endregion
    }

    void InitialisationBorderArray()
    {
        for(int i = 0; i < checker.Count; i++)
        {
            GroundBaseGenerator scriptGround = checker[i].GetComponent<GroundBaseGenerator>();
            scriptGround.RightHeight = new float[cellByLenghtChecker];
            scriptGround.LeftHeight = new float[cellByLenghtChecker];
            scriptGround.TopHeight = new float[cellByLenghtChecker];
            scriptGround.BotHeight = new float[cellByLenghtChecker];
        }
    }

    /// <summary>
    /// Calculate the height of eatch border checker
    /// </summary>
    void CalculateHeightBorder()
    {
        int index = 0;
        for (int y = 0; y < checkerOnTheHeight; y++)
        {
            for (int x = 0; x < checkerOnTheLenght; x++)
            {
                GroundBaseGenerator ground = checker[index].GetComponent<GroundBaseGenerator>();

                #region Right
                if (y != 0 && index < (checkerOnTheLenght * (y + 1)) - 1)//Right after first column
                    ground.RightChecker = true;
                else if(y == 0 && index + 1 < checkerOnTheLenght)//Right in first column
                    ground.RightChecker = true;
                
                if(ground.RightChecker)
                {
                    GroundBaseGenerator scriptGround = checker[index + 1].GetComponent<GroundBaseGenerator>();//Right checker
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.RightHeight[i] = scriptGround.MapDefinition.MapRowsData[i].Row[0];
                }
                else
                {
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.RightHeight[i] = ground.MapDefinition.MapRowsData[i].Row[cellByLenghtChecker-1];
                }
                #endregion
                
                #region Left
                if (y != 0 && index - 1 >= (checkerOnTheLenght * y)) //left after first column
                    ground.LeftChecker = true;
                else if (y == 0 && index - 1 >= 0)//left in first column
                    ground.LeftChecker = true;
                
                if(ground.LeftChecker)
                {
                    GroundBaseGenerator scriptGround = checker[index - 1].GetComponent<GroundBaseGenerator>();
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.LeftHeight[i] = scriptGround.MapDefinition.MapRowsData[i].Row[cellByLenghtChecker - 1];
                }
                else
                {
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.LeftHeight[i] = ground.MapDefinition.MapRowsData[i].Row[0];
                }
                #endregion

                #region Top
                if (index + checkerOnTheLenght < checker.Count)//top
                {
                    ground.TopChecker = true;
                    GroundBaseGenerator scriptGround = checker[index + checkerOnTheLenght].GetComponent<GroundBaseGenerator>();
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.TopHeight[i] = scriptGround.MapDefinition.MapRowsData[0].Row[i];
                }
                else//if no top, use same value
                {
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.TopHeight[i] = ground.MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[i];
                }
                #endregion

                #region Bot
                if (index - checkerOnTheLenght >= 0)//bot
                {
                    ground.BotChecker = true;
                    GroundBaseGenerator scriptGround = checker[index - checkerOnTheLenght].GetComponent<GroundBaseGenerator>();
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.BotHeight[i] = scriptGround.MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[i];
                }
                else//if no bot, use same value
                {
                    for (int i = 0; i < cellByLenghtChecker; i++)
                        ground.BotHeight[i] = ground.MapDefinition.MapRowsData[0].Row[i];
                }
                #endregion

                #region Diagonal Top Right and Top Left
                if (ground.TopChecker)//diagonal Top
                {
                    if (ground.RightChecker)//Top right
                    {
                        ground.DiagonalRightTopChecker = true;
                        ground.DiagonalRightTopHeight = checker[index + checkerOnTheLenght + 1].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[0].Row[0];
                    }

                    if (ground.LeftChecker)//Top left
                    {
                        ground.DiagonalLeftTopChecker = true;
                        ground.DiagonalLeftTopHeight = checker[index + checkerOnTheLenght - 1].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[0].Row[cellByLenghtChecker - 1];
                    }
                }
                else
                {
                    ground.DiagonalRightTopHeight = ground.MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[cellByLenghtChecker - 1];//Top right
                    ground.DiagonalLeftTopHeight = ground.MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[0];//Top left
                }
                #endregion

                #region Diagonal Bot Right and Bot Left
                if (ground.BotChecker)//diagonal
                {
                    if (ground.RightChecker)//Bot Right
                    {
                        ground.DiagonalRightBotChecker = true;
                        ground.DiagonalRightBotHeight = checker[index - checkerOnTheLenght + 1].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[0];
                    }

                    if (ground.LeftChecker)//Bot Left
                    {
                        ground.DiagonalLeftBotChecker = true;
                        ground.DiagonalLeftBotHeight = checker[index - checkerOnTheLenght - 1].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[cellByLenghtChecker - 1].Row[cellByLenghtChecker - 1];
                    }
                }
                else
                {
                    ground.DiagonalRightBotHeight = ground.MapDefinition.MapRowsData[0].Row[cellByLenghtChecker - 1];//Right Bot
                    ground.DiagonalLeftBotHeight = ground.MapDefinition.MapRowsData[0].Row[0];//Left Bot
                }
                #endregion

                index++;
            }
        }
    }

    void RebuildCurrentChecker()
    {
        CalculateHeightBorder();
        RebuildSpecificChecker(indexCurrentChecker);

        if (indexCurrentChecker - 1 >= 0)
            RebuildSpecificChecker(indexCurrentChecker - 1);

        if (indexCurrentChecker - checkerOnTheLenght >= 0)
            RebuildSpecificChecker(indexCurrentChecker - checkerOnTheLenght);
    }

    void RebuildSpecificChecker(int CheckerIndex)
    {
        checker[CheckerIndex].GetComponent<GroundBaseGenerator>().GenerateGroundBase();
        HeightGround currentHeightGroundModify = checker[CheckerIndex].GetComponent<GroundBaseGenerator>().MapDefinition;

        for (int y = 0; y < cellByLenghtChecker; y++)
        {
            for (int x = 0; x < cellByLenghtChecker; x++)
            {
                if (currentHeightGroundModify.MapRowsData[y].CellsInformation[x].CellContaint.EventScript)
                {
                    GameObject obj = currentHeightGroundModify.MapRowsData[y].CellsInformation[x].CellContaint.EventScript.gameObject;

                    obj.transform.position = new Vector3(obj.transform.position.x, currentHeightGroundModify.MapRowsData[y].Row[x] + currentHeightGroundModify.MapRowsData[y].FootPos[x], obj.transform.position.z);
                }
            }
        }
    }

    /// <summary>
    /// Rebuild all checker with it height
    /// </summary>
    void RebuildAllGround()
    {
        CalculateHeightBorder();

        for (int index = 0; index < checker.Count; index++)
        {
            checker[index].GetComponent<GroundBaseGenerator>().GenerateGroundBase();

            for (int y = 0; y < cellByLenghtChecker; y++)
            {
                for (int x = 0; x < cellByLenghtChecker; x++)
                {
                    if (checker[index].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].CellsInformation[x].CellContaint.EventScript)
                    {
                        GameObject obj = checker[index].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].CellsInformation[x].CellContaint.EventScript.gameObject;

                        obj.transform.position = new Vector3(obj.transform.position.x,
                            checker[index].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].Row[x] + checker[index].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[y].FootPos[x],
                            obj.transform.position.z);
                    }
                }
            }
        }
    }

    void CleanCurrentCell()
    {
        currentHeightGround.CleanHeight();
        RebuildCurrentChecker();
        Repaint();
    }

    void LoadGround()
    {
        if (skinLoadAndCreatGroundVue)
        {
            if (!File.Exists("Assets/Script/Tools/GroundGenerator/Save/" + NameJSONFile))
            {
                Debug.Log("File didn't exist !");
                return;
            }
            currentMapDataSave = (MapDataSave)JsonUtility.FromJson(File.ReadAllText("Assets/Script/Tools/GroundGenerator/Save/" + NameJSONFile), typeof(MapDataSave));

            groundFolder = new GameObject();
            groundFolder.transform.position = Vector3.zero;
            groundFolder.name = "GroundFolder";

            vertexColorRedValue = currentMapDataSave.vertexColorRedValue;
            vertexColorGreenValue = currentMapDataSave.vertexColorGreenValue;
            vertexColorBlueValue = currentMapDataSave.vertexColorBlueValue;

            nameMap = currentMapDataSave.NameMap;
            checkerOnTheLenght = currentMapDataSave.CheckerOnTheLenght;
            checkerOnTheHeight = currentMapDataSave.CheckerOnTheHeight;
            cellByLenghtChecker = currentMapDataSave.CellByLenghtChecker;
            cellDensity = currentMapDataSave.Density;
            height = currentMapDataSave.Height;

            CreatGround();
        }

        else
        {
            int lengInitiator = 0;
            foreach (Transform trf in groundFolder.transform)
            {
                if (trf.GetComponent<GroundBaseGenerator>())
                    lengInitiator++;
            }

            GameObject[] initialisator = new GameObject[lengInitiator];
            foreach (Transform child in groundFolder.transform)
            {
                if (child.GetComponent<GroundBaseGenerator>())
                {
                    int i = child.GetComponent<GroundBaseGenerator>().IndexInTheCheckboard;
                    initialisator[i] = child.gameObject;
                }
            }
            checker = new List<GameObject>(initialisator);

            vertexColorRedValue = checker[0].GetComponent<GroundBaseGenerator>().RedColorByHeight;
            vertexColorGreenValue = checker[0].GetComponent<GroundBaseGenerator>().GreenColorByHeight;
            vertexColorBlueValue = checker[0].GetComponent<GroundBaseGenerator>().BlueColorByHeight;

            cellByLenghtChecker = checker[0].GetComponent<GroundBaseGenerator>().NumberCellByLenght;
            cellDensity = checker[0].GetComponent<GroundBaseGenerator>().Density;
            checkerOnTheLenght = 0;
            checkerOnTheHeight = 0;

            for (int index = 0; index < checker.Count; index++)
            {
                if (checker[index].transform.position.z != 0)
                {
                    checkerOnTheLenght = index;
                    break;
                }
            }

            checkerOnTheHeight = checker.Count / checkerOnTheLenght;
        }

        flattenValueChecker = new float[checker.Count];//Use to write the value of the checker after flatten
        groundModifyByFlattenPainter = new GroundBaseGenerator[checker.Count];

        height.Clear();//Recreat the height list (use for the save and for the flatten
        for (int i = 0; i < checker.Count; i++)
        {
            height.Add(checker[i].GetComponent<GroundBaseGenerator>().MapDefinition);
        }

        groundCreat = true;
        skinCheckerVue = true;
        skinLoadGroundVue = false;
        skinLoadAndCreatGroundVue = false;
    }

    void ApplyVertexColor()
    {
        foreach (GameObject obj in checker)
        {
            GroundBaseGenerator ground = obj.GetComponent<GroundBaseGenerator>();
            ground.RedColorByHeight = vertexColorRedValue;
            ground.GreenColorByHeight = vertexColorGreenValue;
            ground.BlueColorByHeight = vertexColorBlueValue;
            ground.GenerateGroundBase();
        }
    }
    #endregion

    #region Cell
    /// <summary>
    /// Put all current cell Walkable bool in true
    /// </summary>
    void FlattenWalkableCell()
    {
        for (int y = 0; y < cellByLenghtChecker; y++)
            for (int x = 0; x < cellByLenghtChecker; x++)
            {
                currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.Walkable = true;
                currentHeightGround.MapRowsData[y].CellsInformation[x].CellContaint.GroundAtribut = Cell.GroundElement.Earth;
            }
        Repaint();
    }

    void UnwalkableCellByHeight(float height)
    {

        for (int i = 0; i < checker.Count; i++)
        {
            for (int line = 0; line < cellByLenghtChecker; line++)
            {
                for (int row = 0; row < cellByLenghtChecker; row++)
                {
                    if (checker[i].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[line].Row[row] > height)
                    {
                        checker[i].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[line].CellsInformation[row].CellContaint.Walkable = false;
                    }
                    else if (checker[i].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[line].Row[row] < height)
                    {
                        checker[i].GetComponent<GroundBaseGenerator>().MapDefinition.MapRowsData[line].CellsInformation[row].CellContaint.Walkable = true;
                    }
                }
            }
        }
        Repaint();
    }
    #endregion

    #region
    float CalculateBoundingBoxSize(GameObject obj)
    {
        if (!obj.GetComponent<MeshFilter>())
            obj.AddComponent<MeshFilter>();

        return obj.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
    }
    #endregion
}
