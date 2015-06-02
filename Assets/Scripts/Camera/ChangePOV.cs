using UnityEngine;
using System.Collections;

public class ChangePOV : MonoBehaviour {

	public GameObject player;
	public GameObject camFirst;
	public GameObject camThird;
	public GameObject camMain;

	private int currentCamera;

	// Use this for initialization
	void Start () {
		camThird.gameObject.SetActive (false);
		camFirst.gameObject.SetActive (false);
		camMain.gameObject.SetActive (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F1)) {
			camFirst.gameObject.SetActive (true);
			camMain.gameObject.SetActive (false);
			camThird.gameObject.SetActive (false);
		}else if(Input.GetKeyDown(KeyCode.F2)){
			camFirst.gameObject.SetActive (false);
			camMain.gameObject.SetActive (true);
			camThird.gameObject.SetActive (false);
		}else if(Input.GetKeyDown(KeyCode.F3)){
			camFirst.gameObject.SetActive (false);
			camMain.gameObject.SetActive (false);
			camThird.gameObject.SetActive (true);
		}
	}
}
