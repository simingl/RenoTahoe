using UnityEngine;
using System.Collections;

public class TouchSound : MonoBehaviour {

	public AudioClip sample;
	
	void OnMouseDown () {
		GetComponent<AudioSource>().PlayOneShot(sample);
	}
}
