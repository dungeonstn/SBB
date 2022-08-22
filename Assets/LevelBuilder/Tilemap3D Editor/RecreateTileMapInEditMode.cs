using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RecreateTileMapInEditMode : MonoBehaviour
{
    private TileMapEditor3D tileMapEd;
    private LevelDAO loadedDAO;

    //Load data (.save)
    //Tha Last Play Mode, after pressed Save button will me stored in a folder. This folder will be fold to aceess bin data.
    //IF YOU DIND'T CREATE ('Assets/Resources/Levels/LastPlayMode/') FOLDERS..YOU NEED TO CREATE NOW!
    public bool LoadBinary()
    {
        tileMapEd = GetComponent<TileMapEditor3D>();
        loadedDAO = GetComponent<TileMapEditor3D>().LevelDAO;

        string sceneNum = SceneManager.GetActiveScene().name;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;

        if (File.Exists("Assets/Resources/Levels/LastPlayMode/" + sceneNum + ".save"))
        {

            file = File.Open("Assets/Resources/Levels/LastPlayMode/" + sceneNum + ".save", FileMode.Open);

            SerializableLevelDAO slDAO = (SerializableLevelDAO)bf.Deserialize(file);
            file.Close();

            loadedDAO.level = new Level();
            loadedDAO.level = slDAO.serializableLevelDAO;

            Debug.Log("Level ( LastPlayMode " + sceneNum + " ) loaded with success.");

            PaintLoadedTiles();

            return true;
        }
        else
        {
            Debug.Log("Level ( LastPlayMode "+sceneNum+" ) doesn't exist. ");

            return false;
        }
    }

    void PaintLoadedTiles()
    {
        List<GameObject> aux = new List<GameObject>();
        foreach (Transform child in transform)
        {
            aux.Add(child.gameObject);
        }
        foreach (GameObject child in aux)
        {
            DestroyImmediate(child.gameObject);
        }

        foreach (Tile t in loadedDAO.level.levelTiles)
        {
            foreach (Transform tile in tileMapEd.TileList)
            {
                if (t.blockName.Equals(tile.name))
                {
                    Vector3 auxPos = new Vector3(t.xPos, t.yPos, t.zPos);

                    Transform g = PrefabUtility.InstantiatePrefab(tile, transform) as Transform;

                    g.position = auxPos;
                    g.eulerAngles = Vector3.up * t.yRot;

                    g.parent = transform;

                    if (g.CompareTag("Prop"))
                    {
                        //This is a nice feature! You cant let this line as it is to randomize the scale of object after you pressed Reload Level button.
                        //It will let your scene more atractive to have size differences between Trees, stones, flowers...
                        //You can see that it isn't present in 'Block' tag, because Blocks needs to be at the same size to fit side by side(1x1x1)
                        g.localScale = Vector3.one * Random.Range(0.7f, 1.3f);
                    }
                }
            }
        }
        //Delete .save after load in Edit Mode? 
        //I recommend you to save this binaries for backUp purposes. 
        //DeleteBinary();
    }

    //Delete data (.save)
    private void DeleteBinary()
    {
        string sceneNum = SceneManager.GetActiveScene().name;

        File.Delete("Assets/Resources/Levels/LastPlayMode/" + sceneNum + ".save");
        Debug.Log("Level ( LastPlayMode " + sceneNum + ") deleted with success.");
    }
}
