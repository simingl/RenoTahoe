using UnityEngine;
using System.Collections;

public class NextSceneButtonScript : MonoBehaviour
{
    private int loadLvl;

    public void NextScene()
    {
        loadLvl = Application.loadedLevel;
        Application.LoadLevel(++loadLvl);
    }
}
