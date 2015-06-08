using UnityEngine;
using System.Collections;
using RTS;

public class Building : WorldObject {
	public Building(){
		type = WorldObjectType.Building;
	}
	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();
	}
	
	protected override void Update () {
		base.Update();
	}
	
	protected override void OnGUI() {
		base.OnGUI();
	}
}
