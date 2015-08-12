//MAPNAV Navigation ToolKit v.1.3.4
//Attention: This script uses a custom editor inspector: MAPNAV/Editor/MapNavInspector.cs

using UnityEngine;
using System.Collections;

[AddComponentMenu("MAPNAV/MapNav")]

public class MapNav : MonoBehaviour 
{
	public Transform user;									 	//User(Player) transform
	public bool simGPS = true;								 	//True when the GPS Emulator is enabled
	public float userSpeed = 5.0f;							 	//User speed when using the GPS Emulator (keyboard input)
	public bool realSpeed = false;								//If true, the perceived player speed depends on zoom level(realistic behaviour)
	public float fixLat = 42.3627f;					   			//Latitude
	public float fixLon = -71.05686f;							//Longitude
	public float altitude;										//Current GPS altitude
	public float heading;										//Last compass sensor reading (Emulator disabled) or user's eulerAngles.y (Emulator enabled)
	public float accuracy;										//GPS location accuracy (error)
	public int maxZoom = 18;									//Maximum zoom level available. Set according to your maps provider
	public int minZoom = 1;										//Minimum zoom level available
	public int zoom = 17;										//Current zoom level
	private float multiplier; 									//1 for a size=640x640 tile, 2 for size=1280*1280 tile, etc. Automatically set when selecting tile size
	public string key = "Fmjtd%7Cluur29072d%2Cbg%3Do5-908s00";  //AppKey (API key) code obtained from your maps provider (MapQuest, Google, etc.). 
																//Default MapQuest key for demo purposes only (with limitations). Please get your own key before you start yout project.															 
	public string keyGoogle = "AIzaSyBUPUCJe4DptCMLSDvxOh427NSwyvRrzVo";
	public string[] maptype;									//Array including available map types
	public int[] mapSize;										//Array including available map sizes(pixels)
	public int index;											//maptype array index. 
	public int indexSize;										//mapsize array index. 
	public float camDist = 15.0f;								//Camera distance(3D) or height(2D) to user
	public int camAngle = 40;									//Camera angle from horizontal plane
	public int initTime = 3;									//Hold time after a successful GPS fix in order to improve location accuracy
	public int maxWait = 30;									//GPS fix timeout
	public bool buttons = true;								 	//Enables GUI sample control buttons 
	public string dmsLat;									 	//Latitude as degrees, minutes and seconds
	public string dmsLon;							 		 	//Longitude as degrees, minutes and seconds
	public float updateRate = 0.1f;								//User's position update rate
	public bool autoCenter = true;							 	//Autocenter and refresh map
	public string status;								     	//GPS and other status messages
	public bool gpsFix;								     		//True after a successful GPS fix 
	public Vector3 iniRef;							         	//First location data retrieved on Start	 
	public bool info;									     	//Used by GPS_Status.cs to enable/disable the GPS information window.
	public bool triDView = false;						     	//2D/3D modes toggle
	public bool ready;								     		//true when the map texture has been successfully loaded
	public bool freeCam = false;							 	//when true, MainCamera follows and looks at Player (3D mode only)
	public bool pinchToZoom = true;							 	//Enables Pinch to Zoom interaction on mobile devices
	public bool dragToPan = true;							 	//Enables Drag to Pan interaction on mobile devices
	public bool mapDisabled;								 	//Disables online maps
	public bool mapping = false;							 	//true while map is being downloaded
	public Transform cam;									 	//Reference to the Main Camera transform
	public float userLon;									 	//Current user position longitude
	public float userLat;									 	//Current user position latitude
	
	private float levelHeight;
	private float smooth = 1.3f;	 						    
	private float yVelocity = 0.0f;  
	private float speed;
	private Camera mycam;
	private float currentOrtoSize;
	private LocationInfo loc;
	private Vector3 currentPosition;
	private Vector3 newUserPos; 
	private Vector3 currentUserPos;
	private float download;
	private WWW www;
	private string url = ""; 
	private double longitude;
	private double latitude;
	private Rect rect;
	private float screenX;
	private float screenY;
	private Renderer maprender;
	private Transform mymap;
	private float initPointerSize;
	private double tempLat;
	private double tempLon;
	private bool touchZoom;
	private string centre;
	private bool centering;
	private Texture centerIcon;
	private Texture topIcon;
	private Texture bottomIcon;
	private Texture leftIcon;
	private Texture rightIcon;
	private GUIStyle arrowIcon;
	private float dot;
	private bool centered = true;
	private int borderTile = 0;
	private bool tileLeft;
	private bool tileRight;
	private bool tileTop;
	private bool tileBottom;
	private Rect topCursorPos;
	private Rect rightCursorPos;
	private Rect bottomCursorPos;
	private Rect leftCursorPos;

	//Touch Screen Variables
	private Vector2 prevDist;
	private float actualDist;
	private Transform target;
	private Touch touch;
	private Touch touch2;
	private Vector2 curDist;
	private float dragSpeed;
	private Rect viewArea;
	private float targetOrtoSize;
	private bool firstTime = true;
	private Vector2 focusScreenPoint;
	private Vector3 focusWorldPoint; 


	void Awake(){
		//Set the map's tag to GameController
		transform.tag = "GameController";
		
		//References to the Main Camera and Player. 
		//Please make sure your camera is tagged as "MainCamera" and your user visualization/character as "Player"
		cam = Camera.main.transform;
		mycam = Camera.main;
		user = GameObject.FindGameObjectWithTag("Player").transform.FindChild("drone").transform;
		
		//Store most used components and values into variables for faster access.
		mymap = transform;
		maprender = GetComponent<Renderer>();
		screenX = Screen.width;
		screenY = Screen.height;	
		
		//Add possible values to maptype and mapsize arrays 
		//ATENTTION: Modify if using a maps provider other than MapQuest Open Static Maps.
		maptype = new string[]{"map", "sat", "hyb"};
		mapSize = new int[]{1280, 1920, 2560, 3840}; //in pixels
		
		//Set GUI "center" button label
		if(triDView){
			centre = "refresh";
		}
		//Enable autocenter on 2D-view (default)
		else{
			autoCenter = true;
		}
		
		//Load required interface textures
		centerIcon = Resources.Load("centerIcon") as Texture2D;
		topIcon = Resources.Load("cursorTop") as Texture2D;
		bottomIcon = Resources.Load("cursorBottom") as Texture2D;
		leftIcon = Resources.Load("cursorLeft") as Texture2D;
		rightIcon = Resources.Load("cursorRight") as Texture2D;
		
		//Resize GUI according to screen size/orientation 
		if(screenY >= screenX){
			dot = screenY/800.0f;
		}else{
			dot = screenX/800.0f;
		}
	}


	IEnumerator Start () {

		//Setting variables values on Start
		gpsFix=false;
		rect = new Rect (screenX/10, screenY/10, 8*screenX/10, 8*screenY/10);
		topCursorPos = new Rect(screenX/2-25*dot, 0, 50*dot, 50*dot);
		rightCursorPos = new Rect(screenX-50*dot, screenY/2-25*dot, 50*dot, 50*dot);
		if(!buttons)
			bottomCursorPos = new Rect(screenX/2-25*dot, screenY-50*dot, 50*dot, 50*dot);
		else
			bottomCursorPos = new Rect(screenX/2-25*dot, screenY-50*dot-screenY/12, 50*dot, 50*dot);
		leftCursorPos = new Rect(0, screenY/2-25*dot, 50*dot, 50*dot);
		Vector3 tmp = mymap.eulerAngles;
		tmp.y=180;
		mymap.eulerAngles = tmp;
		initPointerSize = user.localScale.x;
		//user.position = new Vector3(0, user.position.y, 0);
		
		//Initial Camera Settings
		//3D 
		if(triDView){
			mycam.orthographic = false;
			pinchToZoom = false;
			dragToPan = false;
			//Set the camera's field of view according to Screen size so map's visible area is maximized.
			if(screenY > screenX){
				mycam.fieldOfView = 72.5f;
			}else{
				mycam.fieldOfView = 95-(28*(screenX/screenY));
			}
		}
		//2D
		else{
			mycam.orthographic = true;
			mycam.nearClipPlane = 0.1f;
			mycam.farClipPlane = cam.position.y+1;	

		}
		
		//The "ready" variable will be true when the map texture has been successfully loaded.
		ready = false; 
		
		// Start service before querying location
		Input.location.Start (3.0f, 3.0f); 
		Input.compass.enabled = true;
			
			if(!simGPS){
				//Wait in order to find enough satellites and increase GPS accuracy
				yield return new WaitForSeconds(initTime);
				//Set position
				loc  = Input.location.lastData;          
				iniRef.x = ((loc.longitude*20037508.34f/180)/100);
				iniRef.z = (float)(System.Math.Log(System.Math.Tan((90+loc.latitude)*System.Math.PI/360))/(System.Math.PI/180));
				iniRef.z = ((iniRef.z*20037508.34f/180)/100);  
				iniRef.y = 0;
				fixLon = loc.longitude;
				fixLat = loc.latitude; 
				//Successful GPS fix
				gpsFix = true;
				//Update Map for the current location
				 StartCoroutine(MapPosition());
			}  
			else{
				//Simulate initialization time
				yield return new WaitForSeconds(0);
				//Set Position
				iniRef.x = ((fixLon*20037508.34f/180)/100);
				iniRef.z = (float)(System.Math.Log(System.Math.Tan((90+fixLat)*System.Math.PI/360))/(System.Math.PI/180));
				iniRef.z = (iniRef.z*20037508.34f/180)/100;  
				iniRef.y = 0;
				//Simulated successful GPS fix
				gpsFix = true;
				//Update Map for the current location
				StartCoroutine(MapPosition());
			}    


		//Rescale map, set new camera height, and resize user pointer according to new zoom level
		ReScale(); 

		//Set player's position using new location data (every "updateRate" seconds)
		//Default value for updateRate is 0.1. Increase if necessary to improve performance
		//InvokeRepeating("MyPosition", 1, updateRate); 

		//Read incoming compass data (every 0.1s)
		//InvokeRepeating("Orientate", 1, 0.1f);
		
		//Get altitude and horizontal accuracy readings using new location data (Default: every 2s)
		//InvokeRepeating("AccuracyAltitude", 1, 2);
		
		//Auto-Center Map on 2D View Mode 
		//InvokeRepeating("Check", 1, 0.2f);
	}


	void MyPosition(){
		if(gpsFix){
			if(!simGPS){
				loc = Input.location.lastData;
				newUserPos.x = ((loc.longitude*20037508.34f/180)/100)-iniRef.x;
				newUserPos.z = (float)(System.Math.Log(System.Math.Tan((90+loc.latitude)*System.Math.PI/360))/(System.Math.PI/180));
				newUserPos.z = ((newUserPos.z*20037508.34f/180)/100)-iniRef.z;   
				dmsLat = convertdmsLat(loc.latitude);
				dmsLon = convertdmsLon(loc.longitude);
				userLon = loc.longitude;
				userLat = loc.latitude;
			}
			else{
				userLon = (18000*(user.position.x+iniRef.x))/20037508.34f;
				userLat = ((360/Mathf.PI)*Mathf.Atan(Mathf.Exp(0.00001567855943f*(user.position.z+iniRef.z))))-90;
				dmsLat = convertdmsLat(userLat);
				dmsLon = convertdmsLon(userLon);
			}
		}	
	} 
	
	void Orientate(){
		if(!simGPS && gpsFix){
			heading = Input.compass.trueHeading;
		}
		else{
			heading = user.eulerAngles.y;
		}
	}
	 
	void AccuracyAltitude(){
		if(gpsFix)
			altitude = loc.altitude;
			accuracy = loc.horizontalAccuracy;
	}
	
	void Check(){
		if(autoCenter && triDView == false){
			if(ready == true && mapping == false && gpsFix){
				if (rect.Contains(Vector2.Scale(mycam.WorldToViewportPoint (user.position), new Vector2(screenX, screenY)))){
					//DoNothing
				}
				else{
					centering=true;
					 StartCoroutine(MapPosition());
					 StartCoroutine(ReScale());	
				}
			}
		}
	}

	//Auto-Center Map on 3D View Mode when exiting map's collider
	void OnTriggerExit(Collider other){
		if(other.tag == "Player" && autoCenter && triDView){
			 StartCoroutine(MapPosition());
			 StartCoroutine(ReScale());
		}
	}

	//Update Map with the corresponding map images for the current location ============================================
	IEnumerator MapPosition(){

		//The mapping variable will only be true while the map is being updated
		mapping = true;
		
		CursorsOff();
		
		//CHECK GPS STATUS AND RESTART IF NEEDED
		
		if (Input.location.status == LocationServiceStatus.Stopped || Input.location.status == LocationServiceStatus.Failed){
			// Start service before querying location
			Input.location.Start (3.0f, 3.0f);

			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
				yield return new WaitForSeconds (0);
				maxWait--;
			}

			// Service didn't initialize in 20 seconds
			if (maxWait < 1) {
				print ("Timed out");
				//use the status string variable to print messages to your own user interface (GUIText, etc.)
				status = "Timed out";
			}

			// Connection has failed
			if (Input.location.status == LocationServiceStatus.Failed) {
				print ("Unable to determine device location");
				//use the status string variable to print messages to your own user interface (GUIText, etc.)
				status = "Unable to determine device location";
			}
		
		}
		
	   //------------------------------------------------------------------	//
	   
		www = null; 
		//Get last available location data
		loc = Input.location.lastData;
		//Make player invisible while updating map
		//user.gameObject.GetComponent<Renderer>().enabled = false;
		
		
		//Set target latitude and longitude
			
		fixLon = (18000*(iniRef.x))/20037508.34f;
		fixLat = ((360/Mathf.PI)*Mathf.Atan(Mathf.Exp(0.00001567855943f*(iniRef.z))))-90;	

		//MAPQUEST=========================================================================================
		bool isGoogleMap = true;
		//Build a valid MapQuest OpenMaps tile request for the current location
		if (!isGoogleMap) {
			multiplier = mapSize [indexSize] / 640.0f;  //Tile Size= 640*multiplier
			//ATENTTION: If you want to implement maps from a different tiles provider, modify the following url accordingly to create a valid request
			//Example code can be found at http://recursivearts.com/mapnav/faq.html

			url = "http://open.mapquestapi.com/staticmap/v4/getmap?key=" + key + "&size=" + mapSize [indexSize].ToString () + "," + mapSize [indexSize].ToString () + "&zoom=" + zoom + "&type=" + maptype [index] + "&center=" + fixLat + "," + fixLon + "&scalebar=false";
			tempLat = fixLat; 
			tempLon = fixLon;
		} else {
			//Add possible values to maptype and mapsize arrays (GOOGLE)
			maptype = new string[]{"satellite","hybrid","roadmap"};
			mapSize = new int[]{640}; //in pixels
			//GOOGLE ================================================================================
			//Build a valid Google Maps tile request for the current location 
			multiplier = 10;
			url = "http://maps.google.com/maps/api/staticmap?center=" + fixLat + "," + fixLon + "&zoom=" + 16 + "&scale=2&size=1024x1024&format=jpg&maptype=" + maptype [index] + "&sensor=false&key=" + keyGoogle;
			tempLat = fixLat;
			tempLon = fixLon;
			//=================================================================================================
		}

		//Proceed with download if a Wireless internet connection is available 
		if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork){
			StartCoroutine(Online());
		}	
		//Proceed with download if a 3G/4G internet connection is available 
		else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork){
			 StartCoroutine(Online());
		}
	}

	//ONLINE MAP DOWNLOAD
	IEnumerator Online(){
		if(!mapDisabled){
			// Start a download of the given URL
			www = new WWW(url); 
			// Wait for download to complete
			download = (www.progress);
			while(!www.isDone){
				print("Updating map "+System.Math.Round(download*100)+" %");
				//use the status string variable to print messages to your own user interface (GUIText, etc.)
				status="Updating map "+System.Math.Round(download*100)+" %";
				yield return null;
			}
			//Show download progress and apply texture
			if(www.error == null){
				print("Updating map 100 %");
				print("Map Ready!");
				//use the status string variable to print messages to your own user interface (GUIText, etc.)
				status = "Updating map 100 %\nMap Ready!";
				yield return new WaitForSeconds (0);
				maprender.material.mainTexture = null;
				Texture2D tmp;
				tmp = new Texture2D(1280, 1280, TextureFormat.RGB24, false);
				maprender.material.mainTexture = tmp;
				www.LoadImageIntoTexture(tmp); 	
			}
			//Download Error. Switching to offline mode
			else{
				print("Map Error:"+www.error);
				//use the status string variable to print messages to your own user interface (GUIText, etc.)
				status = "Map Error:"+www.error;
				yield return new WaitForSeconds (0);
				maprender.material.mainTexture = null;
			}
			maprender.enabled = true;
		}
		//ReSet();
		//user.gameObject.GetComponent<Renderer>().enabled = true;
		ready = true;
		mapping = false;
		
	}


	//Re-position map and camera using updated data
	void ReSet(){
		Vector3 tmp = transform.position;
		tmp.x = (float)((tempLon*20037508.34f/180)/100)-iniRef.x;
		tmp.z = (float)(System.Math.Log(System.Math.Tan((90+tempLat)*System.Math.PI/360))/(System.Math.PI/180));
		tmp.z = ((tmp.z*20037508.34f/180)/100)-iniRef.z;
		transform.position = tmp;
		if(!freeCam){
			cam.position = new Vector3(transform.position.x, cam.position.y, transform.position.z);
		}
		if(triDView == false && centering){
			centered = true;
			autoCenter = true;
			centering = false;
		}
	}

	//RE-SCALE =========================================================================================================
	IEnumerator ReScale(){
		while(mapping){
			yield return null;
		}

		//Rescale map according to new zoom level to maintain 1:100 scale
		float newScale = multiplier*100532.244f/(Mathf.Pow (2, zoom));
		mymap.localScale = new Vector3(newScale, mymap.localScale.y, newScale);
		
		//3D View. Free/custom camera
		if(triDView && freeCam){
			//Do Nothing
		}
		
		//3D View and Camera follows player. Set camera position
		else if(triDView && !freeCam){
			Vector3 tmp = cam.localPosition;
			tmp.z = -(65536*camDist*Mathf.Cos(camAngle*Mathf.PI/180))/Mathf.Pow(2, zoom);
			tmp.y = 65536*camDist*Mathf.Sin(camAngle*Mathf.PI/180)/Mathf.Pow(2, zoom);
			cam.localPosition = tmp;
		}
		
		//2D View. Set camera position 
		else{
			yield return null;
		}
	}

	void Update(){
		if(ready){	
			if(!simGPS){
				//Smoothly move pointer to updated position
				currentUserPos.x = user.position.x;
				currentUserPos.x = Mathf.Lerp (user.position.x, newUserPos.x, 2.0f*Time.deltaTime);
				currentUserPos.z = user.position.z;
				currentUserPos.z = Mathf.Lerp (user.position.z, newUserPos.z, 2.0f*Time.deltaTime);
				user.position = new Vector3(currentUserPos.x, user.position.y, currentUserPos.z);

				//Update rotation
				if(System.Math.Abs(user.eulerAngles.y-heading) >= 5){
					float newAngle = Mathf.SmoothDampAngle(user.eulerAngles.y, heading, ref yVelocity, smooth);
					user.eulerAngles = new Vector3(user.eulerAngles.x, newAngle, user.eulerAngles.z);
				}
			}
			else{
				//When GPS Emulator is enabled, user position is controlled by keyboard input.
				if(mapping == false){
					//Use keyboard input to move the player
					if (Input.GetKey ("up") || Input.GetKey ("w")){
						user.transform.Translate(Vector3.forward*speed*Time.deltaTime);
					}
					if (Input.GetKey ("down") || Input.GetKey ("s")){
						user.transform.Translate(-Vector3.forward*speed*Time.deltaTime);
					}
					//rotate pointer when pressing Left and Right arrow keys
					user.Rotate(Vector3.up, Input.GetAxis("Horizontal")*80*Time.deltaTime);
				}
			}	
		}
		
		if(mapping && !mapDisabled){
			//get download progress while images are still downloading
			if(www != null)
                download = www.progress;
		}	

		//DRAG TO PAN ==================================================================================================
		if(dragToPan){
			if(!mapping && ready){

				#if (UNITY_EDITOR || UNITY_STANDALONE)
				//mouse drag
				if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) {
					autoCenter = false;
					centered = false;
					if(Input.mousePosition.y > screenY/12){

						//Check if any of the tile borders has been reached
						CheckBorders();
						
						//Translate the camera
						if(Mathf.Abs (Input.GetAxis("Mouse X"))>0 || Mathf.Abs (Input.GetAxis("Mouse Y"))>0 )
							cam.Translate(-Input.GetAxis("Mouse X") * dragSpeed * 0.7f, -Input.GetAxis("Mouse Y") * dragSpeed * 0.7f, 0);
						
						//Clamp the camera position to avoid displaying any off the map areas
						ClampCam();
					}
				}
				#endif

				//Touch drag
				if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
					autoCenter = false;
					centered = false;
					if(Input.GetTouch(0).position.y > screenY/12){
						Vector2 touchDeltaPosition  = Input.GetTouch(0).deltaPosition;
						
						//Check if any of the tile borders has been reached
						CheckBorders();
						//Translate the camera
						cam.Translate(-touchDeltaPosition.x*dragSpeed*Time.deltaTime, -touchDeltaPosition.y*dragSpeed*Time.deltaTime, 0);
						
						//Clamp the camera position to avoid displaying any off the map areas
						ClampCam();
					}
				}
			}	
		}																																
	}
	
	void CheckBorders(){
		//Reached left tile border
		if(Mathf.Round((mycam.ScreenToWorldPoint(new Vector3(0, 0.5f, cam.position.y)).x)*100.0f)/100.0f <= Mathf.Round((mymap.position.x-mymap.localScale.x*5)*100.0f)/100.0f){
			//show button for borderTile=4;
			tileLeft = true;
		}else{
			//hide button
			tileLeft = false;
		}
		//Reached right tile border
		if(Mathf.Round((mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth, 0.5f, cam.position.y)).x)*100.0f)/100.0f >= Mathf.Round((mymap.position.x+mymap.localScale.x*5)*100.0f)/100.0f){
			//show button for borderTile=2;
			tileRight = true;
		}else{
			//hide button
			tileRight = false;
		}
		//Reached bottom tile border
		if(Mathf.Round((mycam.ScreenToWorldPoint(new Vector3(0.5f, 0, cam.position.y)).z)*100.0f)/100.0f <= Mathf.Round((mymap.position.z-mymap.localScale.z*5)*100.0f)/100.0f){
			//show button for borderTile=3;
			tileBottom = true;
		}else{
			//hide button
			tileBottom = false;
		}
		//Reached top tile border
		if(Mathf.Round((mycam.ScreenToWorldPoint(new Vector3(0.5f, mycam.pixelHeight, cam.position.y)).z)*100.0f)/100.0f >= Mathf.Round((mymap.position.z+mymap.localScale.z*5)*100.0f)/100.0f){
			//show button for borderTile=1;
			tileTop = true;
		}else{
			//hide button
			tileTop = false;
		}
	}

	//Disable surrounding tiles cursors
	void CursorsOff(){
		tileTop = false;
		tileBottom = false;
		tileLeft = false;
		tileRight = false;
	}

	//Clamp the camera position
	void ClampCam(){
		Vector3 tmp = cam.position;
		tmp.x = Mathf.Clamp(cam.position.x, 
		                    mymap.position.x-(mymap.localScale.x*5)+(mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth, 0.5f, cam.position.y)).x-mycam.ScreenToWorldPoint(new Vector3(0, 0.5f, cam.position.y)).x)/2, 
		                    mymap.position.x+(mymap.localScale.x*5)-(mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth, 0.5f, cam.position.y)).x-mycam.ScreenToWorldPoint(new Vector3(0, 0.5f, cam.position.y)).x)/2 );
		tmp.z = Mathf.Clamp(cam.position.z, 
		                    mymap.position.z-(mymap.localScale.z*5)+(mycam.ScreenToWorldPoint(new Vector3(0.5f, mycam.pixelHeight, cam.position.y)).z-mycam.ScreenToWorldPoint(new Vector3(0.5f, 0, cam.position.y)).z)/2, 
		                    mymap.position.z+(mymap.localScale.z*5)-(mycam.ScreenToWorldPoint(new Vector3(0.5f, mycam.pixelHeight, cam.position.y)).z-mycam.ScreenToWorldPoint(new Vector3(0.5f, 0, cam.position.y)).z)/2 );				
		cam.position = tmp;
	}

	//SAMPLE USER INTERFACE. MODIFY OR EXTEND IF NECESSARY =============================================================
	void OnGUI () {
		//Map type toggle button
		if (GUI.Button(new Rect(200, 0, 70, 30), maptype[index])){
			if(mapping == false){
				if(index < maptype.Length-1)
					index = index+1;
				else
					index = 0;	
				StartCoroutine(MapPosition());
			}    
		}
	}
  
    //Translate decimal latitude to Degrees Minutes and Seconds
    string convertdmsLat(float lat) {
        string result;
        float degrees;
        float minutes;
        float seconds; 
        degrees = Mathf.Floor(Mathf.Abs(lat)); 
        minutes = (float)((Mathf.Abs(lat)-Mathf.Floor(Mathf.Abs(lat)))*60.0);
        seconds = (float)((minutes-Mathf.Floor(minutes))*60.0);
		result = degrees+"\u00B0 "+Mathf.Floor(minutes)+"' "+seconds.ToString("F2")+"\" "+((lat > 0) ? "N" : "S");
	    return result;
    }  
 
    //Translate decimal longitude to Degrees Minutes and Seconds  
    string convertdmsLon(float lon) {
        string result;
        float degrees;
        float minutes;
        float seconds;
        degrees = Mathf.Floor(Mathf.Abs(lon));
        minutes = (float)((Mathf.Abs(lon)-Mathf.Floor(Mathf.Abs(lon)))*60.0);
        seconds = (float)((minutes-Mathf.Floor(minutes))*60.0);
        result = degrees+"\u00B0 "+Mathf.Floor(minutes)+"' "+seconds.ToString("F2")+"\" "+((lon > 0) ? "E" : "W");
        return result;
    }
}