//MAPNAV Navigation ToolKit v.1.3.2

using UnityEngine;
using System.Collections;

public class InOut : MonoBehaviour 
{
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			transform.Find("Status").GetComponent<GUIText>().text = "IN";
		}
	}
	void OnTriggerExit(Collider other){
		if(other.tag == "Player"){
			transform.Find("Status").GetComponent<GUIText>().text = "OUT";
		}
	}
}