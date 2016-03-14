using UnityEngine;
using System.Collections;

public class MinimapManagement : MonoBehaviour {
	private Camera mainCam;
	private Camera minimapCam;

	public GUISkin skin;

	private Player player;
	public Vector2 lastMinimapSize;

	private float ratio;
    private bool switchOff;

    void Awake() {

	}

    public void cameraTurnOff(bool status)
    {

        switchOff = status;

        if (switchOff == true)
        {

            minimapCam.depth = -1;
        }
        else {
            minimapCam.depth = 100;
        }

    }


    // Use this for initialization
    void Start () {
		mainCam = Camera.main;
		minimapCam = this.GetComponent<Camera> ();
		ratio = minimapCam.pixelWidth / minimapCam.pixelHeight;

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		lastMinimapSize = new Vector2(minimapCam.pixelWidth, minimapCam.pixelHeight);
	}


	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0) && MouseInBoundsMinimap()) {
			Ray ray = minimapCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				mainCam.transform.position = new Vector3(hit.point.x, mainCam.transform.position.y, hit.point.z);
			}
		}

		Vector2 minimapSize = new Vector2(minimapCam.pixelWidth, minimapCam.pixelHeight);
		
		if(this.lastMinimapSize != minimapSize) {
			float screenWidth = Screen.width;
			float screenHeight = Screen.height;
			//change in width
			if(minimapSize.x != this.lastMinimapSize.x){
				float newWidth = minimapSize.x;
				float newHeight = newWidth/ratio;
				minimapCam.rect = new Rect(0,0, newWidth/screenWidth,newHeight/screenHeight);
			}

			//change in height
			if(minimapSize.y != this.lastMinimapSize.y){
				float newHeight = minimapSize.y;
				float newWidth = newHeight * ratio;
				minimapCam.rect = new Rect(0,0, newWidth/screenWidth,newHeight/screenHeight);
			}

			this.lastMinimapSize = minimapSize;
		}
	}

	void OnGUI()
	{
        if (switchOff)
        {
            return;
        }

        GUI.skin = skin;
		GUI.Box (new Rect (minimapCam.pixelRect.x, (Screen.height - minimapCam.pixelRect.yMax), minimapCam.pixelWidth, minimapCam.pixelHeight), "");
	}

	bool MouseInBoundsMinimap(){
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= lastMinimapSize.x;
		bool insideHeight = mousePos.y >= 0 && mousePos.y < lastMinimapSize.y;
		return insideWidth && insideHeight;
	}
}
