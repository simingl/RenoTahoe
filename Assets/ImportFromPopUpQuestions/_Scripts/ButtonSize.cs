using UnityEngine;
using System.Collections;
using RTS;

public class ButtonSize : MonoBehaviour
{
    private int HButtonsNum;
    private int VButtonsNum;
    private Vector2 minimapSize;
    private GameObject miniMapCamera;
    private MinimapManagement minimapManagement;
    private ConfigManager configmanager;
    public GameObject parrentButton;
    private Vector2 minimapPosition;
    private Camera minimapCam;

    // Use this for initialization
    void Start()
    {        
        miniMapCamera = GameObject.FindGameObjectWithTag("Camera_minimap");
        minimapManagement = miniMapCamera.GetComponent<MinimapManagement>();
        minimapSize = minimapManagement.lastMinimapSize;
        configmanager = ConfigManager.getInstance();
        HButtonsNum = configmanager.getSceneHorizontalButtonsNum();
        VButtonsNum = configmanager.getSceneVerticalButtonsNum();
        parrentButton.GetComponent<RectTransform>().sizeDelta = new Vector2(minimapSize.x / HButtonsNum , minimapSize.y / VButtonsNum);
        minimapCam = miniMapCamera.GetComponent<Camera>();
        //minimapPosition = new Vector2(minimapCam.pixelRect.x, (Screen.height - minimapCam.pixelRect.yMax));
    }

    void Update()
    {
    }
    private Vector2 getLastMinimapSize()
    {
        minimapSize = GameObject.FindGameObjectWithTag("Camera_minimap").GetComponent<MinimapManagement>().lastMinimapSize;
        parrentButton.GetComponent<RectTransform>().sizeDelta = minimapSize;
        return minimapSize;
    }
    public void OnButtonClick()
    {
        Debug.Log("hello");
    }
}
