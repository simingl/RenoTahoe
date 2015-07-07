//MAPNAV Navigation ToolKit v.1.3.4

using UnityEngine;
using System.Collections;

public class InitScreen : MonoBehaviour
{
    private MapNav mapnav;
    private Transform initText;
    private Transform initBackg;

    void Awake()
    {
        //Reference to the MapNav.cs script and GUI elements
        mapnav = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapNav>();
        initText = transform.Find("GUIText");
        initBackg = transform.Find("GUITexture");

        //Set GUIText font size according to our device screen size
        initText.GetComponent<GUIText>().fontSize = (int)Mathf.Round(15 * Screen.width / 320);
    }

    void Start()
    {
        //Initialization message
        initText.GetComponent<GUIText>().text = "Searching for satellites ...";

        //Enable initial screen
        initText.gameObject.SetActive(true);
        initBackg.gameObject.SetActive(true);
		initBackg.GetComponent<GUITexture>().pixelInset = new Rect (initBackg.GetComponent<GUITexture>().pixelInset.x, initBackg.GetComponent<GUITexture>().pixelInset.y, Screen.width, Screen.height);  
    }

    void Update()
    {
        if (!mapnav.ready)
        {
            //Display GPS fix and maps download progress
            initText.GetComponent<GUIText>().text = mapnav.status;
        }
        else
        {
            //Clear messages once the map is ready
            initText.GetComponent<GUIText>().text = "";

            //Disable initial screen
            initBackg.gameObject.SetActive(false);

            //Disable this script (no longer needed)
            this.enabled = false; 
        }
    }
}
