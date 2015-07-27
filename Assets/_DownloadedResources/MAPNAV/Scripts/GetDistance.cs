//MAPNAV Navigation ToolKit v.1.3.4
using UnityEngine;
using System;
using System.Collections;

public struct locWGS84
{
	public double Latitude, Longitude;
	
	public locWGS84(double p1, double p2)
	{
		Latitude = p1;
		Longitude = p2;
	}
}

public class GetDistance : MonoBehaviour
{

	public Transform[] waypoints;

	public bool renderPath;
	public Color pathColor = new Color (0.0f, 0.0f, 1.0f, 0.5f);
	public float pathWidth = 0.1f;
	private float initX;
	private float initZ;
	private MapNav gps;
	private bool gpsFix;

	public float updateRate = 0.5f;
	public double totalDistance;
	public double milesDist;
	public double metersDist;
	public double feetDist;

	void Awake()
	{
		//Reference to the MapNav.js script and gpsFix variable. gpsFix will be true when a valid location data has been set.
		gps = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapNav>();
		gpsFix = gps.gpsFix;
	}

	IEnumerator Start()
	{
		//Wait until the gps sensor provides a valid location.
		while (!gpsFix)
		{
			gpsFix = gps.gpsFix;
			yield return null;
		}
		//Read initial position (used as a reference system)
		initX = gps.iniRef.x;
		initZ = gps.iniRef.z;

		if (renderPath) {
			LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
			lineRenderer.enabled=true;
			lineRenderer.SetColors (pathColor, pathColor);
			lineRenderer.SetWidth (pathWidth, pathWidth);
			lineRenderer.SetVertexCount (waypoints.Length);
		}

		InvokeRepeating("MyDistance", 1, updateRate);
	}

	void MyDistance (){
		if (gpsFix) {
			totalDistance = 0.0f;
			for (int i=0; i<waypoints.Length-1; i++) {
					//Distance between waypoint[i] and waypoint[i+1]
					double leg = GeoDistance (Pos2Loc (waypoints [i].position), Pos2Loc (waypoints [i + 1].position));
					//add to totalDistance
					totalDistance += leg; 
			}
			if(renderPath){
				LineRenderer lineRenderer = GetComponent<LineRenderer>();
				for (int j=0; j<waypoints.Length; j++) {
					Vector3 flatPos = new Vector3(waypoints [j].position.x,0.1f,waypoints [j].position.z);
					lineRenderer.SetPosition(j, flatPos);
				}
			}

			metersDist = totalDistance * 1000;
			milesDist = totalDistance * 0.621371192;
			feetDist = totalDistance * 3280.83989501;

			//Debug.Log ("Total Distance: " + Math.Round(totalDistance,3).ToString() + " Km");
		}
	} 

	public double GeoDistance (locWGS84 loc1, locWGS84 loc2)
	{
		double radius = 6371;
		double dLat = this.toRadian(loc2.Latitude - loc1.Latitude);
		double dLon = this.toRadian(loc2.Longitude - loc1.Longitude);
		double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
			Math.Cos(this.toRadian(loc1.Latitude)) * Math.Cos(this.toRadian(loc2.Latitude)) *
				Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
		double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
		double d = radius * c;
		return d;
	}

	public locWGS84 Pos2Loc (Vector3 wayPoint) {

		double lat = ((360 / Mathf.PI) * Mathf.Atan(Mathf.Exp(0.00001567855943f * (wayPoint.z + initZ)))) - 90;
		double lon = (18000 * (wayPoint.x + initX)) / 20037508.34f;
		
		locWGS84 result = new locWGS84 (lat,lon);
		return result;
	
	}

	// Convert to Radians.
	private double toRadian(double val)
	{
		return (Math.PI / 180) * val;
	}
}	
