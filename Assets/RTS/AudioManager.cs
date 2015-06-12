using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public AudioClip selectSound;
	public AudioClip moveToSound;

	public void playUnitSelectSound(){
		AudioSource.PlayClipAtPoint(selectSound, transform.position);
	}

	public void playUnitMoveToSound(){
		AudioSource.PlayClipAtPoint(moveToSound, transform.position);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
