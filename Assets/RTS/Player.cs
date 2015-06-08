using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public WorldObject SelectedObject { get; set; }
	public string username;
	public bool human = true;
	public HUD hud;
	
	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD >();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}