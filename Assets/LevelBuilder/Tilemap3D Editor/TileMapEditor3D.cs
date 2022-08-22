using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Thank you for using me Level Builder!
/// This Level Builder was used to make levels for my games: Frisbee For Fun and HoneyLand
/// Both are on Steam.
/// </summary>

[System.Serializable]
public class LevelDAO
{
    public Level level;
}

[System.Serializable]
public class SerializableLevelDAO
{
    public Level serializableLevelDAO = new Level();
}

[System.Serializable]
public class Level
{
    public List<Tile> levelTiles;
}

[System.Serializable]
public class Tile
{
    public int xPos;
    public int yPos;
    public int zPos;
    public int yRot;
    public string blockName;
}

public class TileMapEditor3D : MonoBehaviour
{
    [Tooltip("All you prefabs")]
    [SerializeField] private List<Transform> tileList;
    [Tooltip("Button that selects the Prefab")]
    [SerializeField] private Transform uiBlockButton;
    [Tooltip("Layer used to detect position and height for placing or remove a object(BrushTile)")]
    [SerializeField] private LayerMask brushLayer;
    [Tooltip("Brush Cube material")]
    [SerializeField] private Material brushBottomMaterial;

    [HideInInspector] public LevelDAO levelDAO;

    private List<GameObject> allObjects;

    private string sceneNum = "";

    private Transform brush;
    private int selectedTileNum;

    private int height;
    private Text heightViwer;
    private Transform quadFloor;

    private RectTransform tilemapUI;
    private Vector3 limitBrushPos;

    private Transform camRotHorizontal;
    private Transform cam;
   
    private GameObject centerMarker;

    public LevelDAO LevelDAO {
        get {
            return levelDAO;
        }

        set {
            levelDAO = value;
        }
    }
    public List<Transform> TileList {
        get {
            return tileList;
        }

        set {
            tileList = value;
        }
    }

    private void Awake()
    {
        levelDAO = new LevelDAO();
        allObjects = new List<GameObject>();

        sceneNum = SceneManager.GetActiveScene().name;

        //When start game, all objects that has Block and Prop tag will be stored
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));
        allObjects.AddRange(GameObject.FindGameObjectsWithTag("Prop"));

        StartLevelBuilder();
    }

    void Update()
    {
       CamController();
       BrushController();
    }

    //1 - Start builder
    private void StartLevelBuilder()
    {
        #region UI
        tilemapUI = GameObject.Find("Tilemap3D UI").GetComponent<RectTransform>();
        tilemapUI.gameObject.GetComponent<CanvasGroup>().alpha = 1;

        Vector3[] v = new Vector3[4];
        tilemapUI.GetLocalCorners(v);
        limitBrushPos = v[1] * 3;


        Button saveBtn = GameObject.Find("SaveBtn").GetComponent<Button>();
        saveBtn.onClick.AddListener(() =>
        {
            this.SaveBinary("LastPlayMode", "");
        });

        Button uiRemoveBlockBtn = GameObject.Find("Tilemap3DUIBlockNull").GetComponent<Button>();
        uiRemoveBlockBtn.onClick.AddListener(() =>
        {
            this.DisableBrush();
        });

        GameObject uiBlockList = GameObject.Find("Tilemap3DUIBlocks");
        for (int j = 0; j < TileList.Count; j++)
        {
            Transform btn = Instantiate(uiBlockButton, uiBlockList.transform.position, Quaternion.identity);
            btn.SetParent(uiBlockList.transform);
            btn.name = TileList[j].name;
            btn.localScale = Vector3.one;
            btn.localRotation = Quaternion.identity;


            btn.GetChild(0).GetComponent<Text>().text = TileList[j].name;
            Button b = btn.GetComponent<Button>();
            int auxJ = j;

            b.onClick.AddListener(() =>
            {
                this.ChangeBrush(auxJ);
            });
        }

        heightViwer = GameObject.Find("HeightNum").GetComponent<Text>();

        Button increaseHeightBtn = GameObject.Find("IncreaseHeightBtn").GetComponent<Button>();
        increaseHeightBtn.onClick.AddListener(() =>
        {
            height++;
            heightViwer.text = height.ToString();
            quadFloor.position = Vector3.up * height;

        });
        Button decreaseHeightBtn = GameObject.Find("DecreaseHeightBtn").GetComponent<Button>();
        decreaseHeightBtn.onClick.AddListener(() =>
        {
            height--;
            heightViwer.text = height.ToString();
            quadFloor.position = Vector3.up * height;
        });
        #endregion

        //Camera will be stored
        camRotHorizontal = GameObject.Find("CamRotHorizontal").transform;
        cam = camRotHorizontal.GetChild(0).GetChild(0);

        //A Quad will be created and placed in the center of the scene only for orientation purposes
        centerMarker = GameObject.CreatePrimitive(PrimitiveType.Quad);
        centerMarker.transform.position = Vector3.zero;
        centerMarker.transform.localScale = Vector3.one;
        centerMarker.transform.eulerAngles = Vector3.right * 90;

        //This is important! A Quad will be created to define the height of tilemap and it will be used for Raycast to Instantiate or destroy objects
        quadFloor = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
        quadFloor.transform.position = Vector3.zero;
        quadFloor.transform.localScale = Vector3.one * 500;
        quadFloor.transform.eulerAngles = Vector3.right * 90;
        quadFloor.gameObject.layer = LayerMask.NameToLayer("BrushTile");
        quadFloor.GetComponent<MeshRenderer>().enabled = false;

        selectedTileNum = 0;

        //The height of tilemap
        height = 0;
    }

    //Change the Block/Decoration you use
    public void ChangeBrush(int num)
    {
        if (brush != null)
        {
            Destroy(brush.gameObject);
        }

        selectedTileNum = num;

        brush = new GameObject("Brush").transform;

        Transform brushItem = Instantiate(TileList[num], Vector3.zero, Quaternion.identity);
        brushItem.SetParent(brush);
        brushItem.localPosition = Vector3.zero;
        brushItem.localScale = Vector3.one / 2f;

        Transform brushBottom = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        brushBottom.GetComponent<Collider>().enabled = false;
        brushBottom.GetComponent<MeshRenderer>().material = brushBottomMaterial;
        brushBottom.SetParent(brush);
        brushBottom.localPosition = new Vector3(0, 0.52f, 0);


        brushBottom.localScale = (Vector3.one * 1);
        brushBottom.eulerAngles = Vector3.right * 90;
        brushBottom.localScale = Vector3.one;

    }
    //Disable Brush. You can disable to view your scene with peace of mind
    public void DisableBrush()
    {
        if (brush != null)
        {
            Destroy(brush.gameObject);
        }
    }
    //Using brush
    private void BrushController()
    {
        //This is a limit at the bottom of your screen to stop brush to change brushes/use bottom UI
        if (Input.mousePosition.y <= limitBrushPos.y)
        {
            if (brush != null)
                brush.gameObject.SetActive(false);
            return;
        }
        else
        {
            if (brush != null)
                brush.gameObject.SetActive(true);
        }

        //Rotate Brush horizontally
        if (brush != null)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
                brush.GetChild(0).eulerAngles += Vector3.up * 90;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                brush.GetChild(0).eulerAngles += Vector3.up * -90;
        }

        //Create and Deelte scene Blocks
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, brushLayer))
        {
            //All our world follows this!
            //1x1x1 Grid Size  (0,8f is rounded to 1 | 0.15f is rounded to 0)
            Vector3 tilePos = new Vector3(Mathf.RoundToInt(hit.point.x), height, Mathf.RoundToInt(hit.point.z));

            if (hit.collider != null)
            {
                if (brush != null)
                {
                    if (Input.GetMouseButtonDown(0))
                        AddRemoveTile(tilePos, (int)brush.GetChild(0).eulerAngles.y, true);
                    else if (Input.GetMouseButtonDown(1))
                        AddRemoveTile(tilePos, 0,  false);

                    brush.position = tilePos;
                }
            }
        }
    }

    //Camera Controller
    private void CamController()
    {
        int vertical = 0;
        vertical = Input.GetKey(KeyCode.W) ? 1 : vertical;
        vertical = Input.GetKey(KeyCode.S) ? -1 : vertical;

        int horizontal = 0;
        horizontal = Input.GetKey(KeyCode.D) ? 1 : horizontal;
        horizontal = Input.GetKey(KeyCode.A) ? -1 : horizontal;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            camRotHorizontal.position += camRotHorizontal.TransformDirection(new Vector3(horizontal, 0, vertical).normalized) * 5 * Time.deltaTime;
        }
        else
        {
            camRotHorizontal.Rotate(Vector3.up * horizontal, -70 * Time.deltaTime, Space.World);
            camRotHorizontal.GetChild(0).Rotate(Vector3.right * vertical, 30 * Time.deltaTime, Space.Self);
        }
        cam.position += cam.forward * Input.GetAxisRaw("Mouse ScrollWheel") * 1000 * Time.deltaTime;
    }
    //Func that Add or Remove a Tile
    void AddRemoveTile(Vector3 pos, int rot,  bool add)
    {
        if (!add)
        {
            for (int a = 0; a < allObjects.Count; a++)
            {
                Vector3 auxTile = allObjects[a].transform.position;

                if (auxTile == pos)
                {
                    Destroy(allObjects[a]);
                    allObjects.RemoveAt(a);
                    a = allObjects.Count;
                }
            }
            return;
        }

        Tile newTile = new Tile { xPos = (int)pos.x, yPos = (int)pos.y, zPos = (int)pos.z, yRot = rot};
        Vector3 newTilePos = new Vector3(newTile.xPos, newTile.yPos, newTile.zPos);

        for (int a = 0; a < allObjects.Count; a++)
        {
            Vector3 auxTile = allObjects[a].transform.position;

            if (auxTile == pos)
            {
                return;
            }
        }

        Transform o = Instantiate(TileList[selectedTileNum], newTilePos, Quaternion.identity);
        o.parent = transform;
        o.localEulerAngles = Vector3.up * newTile.yRot;

        if (o.CompareTag("Prop"))
        {
            o.localScale = Vector3.zero;
        }

        allObjects.Add(o.gameObject);

        StartCoroutine(TileAddEffect(o));
    }

    //That nice grow effect after you place an object in th scene.
    IEnumerator TileAddEffect(Transform tile)
    {
        float timer = 1f;
        tile.localScale = Vector3.one / 4f;

        Vector3 randScale = Vector3.one;

        if (tile.CompareTag("Prop"))
            randScale = Vector3.one * Random.Range(0.6f, 1f);


        while (tile != null && timer > 0)
        {
            tile.localScale = Vector3.Lerp(tile.localScale, randScale, 10 * Time.deltaTime);
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    //Save data (.save)
    private void SaveBinary(string bNum, string operation)
    {
        LevelDAO = new LevelDAO { level = new Level { levelTiles = new List<Tile>() } };

        foreach (GameObject g in allObjects)
        {
            string bName = g.name.Replace("(Clone)", "");

            //Position AND Y rotation stored!
            Tile newTile = new Tile { xPos = (int)g.transform.position.x, yPos = (int)g.transform.position.y, zPos = (int)g.transform.position.z, blockName = bName, yRot = (int)g.transform.eulerAngles.y };
            LevelDAO.level.levelTiles.Add(newTile);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        SerializableLevelDAO sdao;

        if (operation.Equals("Create"))
        {
            Debug.Log("Creating Level ( LastPlayMode " + sceneNum + " )... ");

            Level newLevel = new Level();

            newLevel.levelTiles = new List<Tile>();
            sdao = new SerializableLevelDAO { serializableLevelDAO = newLevel };

        }
        else
        {
            sdao = new SerializableLevelDAO { serializableLevelDAO = LevelDAO.level };
        }

        file = File.Create("Assets/Resources/Levels/LastPlayMode/" + sceneNum + ".save");

        bf.Serialize(file, sdao);
        file.Close();

        Debug.Log("Level ( LastPlayMode " + sceneNum + " ) saved with success.");

    }


    //Stop effects from coroutines on Exit.
    private void OnDestroy()
    {
        StopAllCoroutines();
        Destroy(centerMarker);
    }
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        Destroy(centerMarker);

    }
    private void OnDisable()
    {
        StopAllCoroutines();
        Destroy(centerMarker);

    }
}

