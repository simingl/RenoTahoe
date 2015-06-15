using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {
	public GameObject tree;
	public GameObject fire;

	private int number = 0;

	private Vector3[] treePoints;
	private Vector3[] firePoints;

	private int width = 10, height = 10;

	void Start(){
		Random.seed = 1;

		treePoints = new Vector3[number];
		firePoints = new Vector3[number];

		for (int i= 0;i<number;i++) {
			//treePoints[i] = generateRandomPosition(width, height);
			firePoints[i] = generateRandomPosition(width, height);
		}

		InitialScene ();
	}

	void InitialScene ()
	{
		// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
		for (int i = 0; i< number;i++) {
			//Instantiate (tree, treePoints[i], new Quaternion(0,0,0,0));
			GameObject newfire = (GameObject)Instantiate (fire, firePoints[i], new Quaternion(1,1,0,1));
		}
	}

	private Vector3 generateRandomPosition(int w, int h){
		float scale = 1.5f;
		float x = Random.Range(-1*width, height);
		float z = Random.Range(-1*width, height);

		return new Vector3 (x,0,z);
	}
}
