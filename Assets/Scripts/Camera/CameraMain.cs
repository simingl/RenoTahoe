using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraMain : MonoBehaviour
{
	public GUISkin skin;

	private GameObject ground;
	private Player player;           // Reference to the player's transform.
	private Transform cam;
	private Camera mycam;
	private Camera MiniMapCamera;

	private Vector2 MiniMapLastPos = Vector2.zero;
	private int MiniMapOffsetX = 0;
	private int MiniMapOffsetY = 64;
	
	private Vector3 ScreenCorner1;
	private Vector3 ScreenCorner2;
	
	private Vector3 MiniMapCorner1;
	private Vector3 MiniMapCorner2;
	
	private float MiniMapScreenOffsetX1;
	private float MiniMapScreenOffsetY1;
	
	private float MiniMapScreenOffsetX2;
	private float MiniMapScreenOffsetY2;
	private Texture2D BorderTexture; //just a small transparent image with a white border
	
	private LayerMask MapBackgroundMask;


	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		ground = GameObject.FindGameObjectWithTag ("Ground");
		cam = transform;
		mycam = Camera.main;
		MiniMapCamera = GameObject.FindGameObjectWithTag ("Camera_minimap").GetComponent<Camera>();
	}

	public void ClampCam(){
		Transform mymap = ground.transform;
		
		Vector3 tmp = cam.position;
		tmp.x = Mathf.Clamp(cam.position.x, 
		                    mymap.position.x-(mymap.localScale.x*5)+(mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth, 0.5f, cam.position.y)).x-mycam.ScreenToWorldPoint(new Vector3(0, 0.5f, cam.position.y)).x)/2, 
		                    mymap.position.x+(mymap.localScale.x*5)-(mycam.ScreenToWorldPoint(new Vector3(mycam.pixelWidth, 0.5f, cam.position.y)).x-mycam.ScreenToWorldPoint(new Vector3(0, 0.5f, cam.position.y)).x)/2 );
		tmp.z = Mathf.Clamp(cam.position.z, 
		                    mymap.position.z-(mymap.localScale.z*5)+(mycam.ScreenToWorldPoint(new Vector3(0.5f, mycam.pixelHeight, cam.position.y)).z-mycam.ScreenToWorldPoint(new Vector3(0.5f, 0, cam.position.y)).z)/2, 
		                    mymap.position.z+(mymap.localScale.z*5)-(mycam.ScreenToWorldPoint(new Vector3(0.5f, mycam.pixelHeight, cam.position.y)).z-mycam.ScreenToWorldPoint(new Vector3(0.5f, 0, cam.position.y)).z)/2 );				
		cam.position = tmp;
	}



	void OnGUI()
	{
		MiniMapOffsetX = 0;
		MiniMapOffsetY = Screen.height - MiniMapCamera.pixelHeight;

		MapBackgroundMask = 1 << 12;  //Ground layer

		//MiniMap Stuff
		Vector2 MiniMapCurrentPos = new Vector2(Screen.width, Screen.height);
		if ((MiniMapLastPos - MiniMapCurrentPos).magnitude != 2)
		{
			MiniMapLastPos = MiniMapCurrentPos;
			MiniMapCamera.pixelRect = new Rect(MiniMapOffsetX, Screen.height - MiniMapCamera.pixelHeight - MiniMapOffsetY, MiniMapCamera.pixelWidth, MiniMapCamera.pixelHeight);
		}
		
		Ray ray1 = Camera.main.ViewportPointToRay (new Vector3(0,1,0));
		RaycastHit hit1;
		if (Physics.Raycast (ray1, out hit1, Mathf.Infinity, MapBackgroundMask))
		{
			ScreenCorner1 = new Vector3(hit1.point.x, hit1.point.y, hit1.point.z);
		}
		Ray ray2 = Camera.main.ViewportPointToRay (new Vector3(1,0,0));
		RaycastHit hit2;
		if (Physics.Raycast (ray2, out hit2, Mathf.Infinity, MapBackgroundMask))
		{
			ScreenCorner2 = new Vector3(hit2.point.x, hit2.point.y, hit2.point.z);
		}
		
		MiniMapCorner1 = MiniMapCamera.WorldToViewportPoint(ScreenCorner1);
		MiniMapCorner2 = MiniMapCamera.WorldToViewportPoint(ScreenCorner2);
		
		MiniMapScreenOffsetX1 = MiniMapCamera.pixelWidth * MiniMapCorner1.x;
		MiniMapScreenOffsetY1 = MiniMapCamera.pixelHeight * MiniMapCorner1.y;
		
		MiniMapScreenOffsetX2 = MiniMapCamera.pixelWidth * MiniMapCorner2.x;
		MiniMapScreenOffsetY2 = MiniMapCamera.pixelHeight * MiniMapCorner2.y;
		
		
		Rect MiniMapWindow = new Rect(MiniMapScreenOffsetX1, Screen.height - MiniMapScreenOffsetY1, MiniMapScreenOffsetX2 - MiniMapScreenOffsetX1, MiniMapScreenOffsetY1 - MiniMapScreenOffsetY2);
		GUI.skin = skin;
		GUI.Box(MiniMapWindow,"");
	}

}