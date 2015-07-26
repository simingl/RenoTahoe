using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarNavMeshAI : MonoBehaviour {
	
	public Transform target;
	public List<Transform> targetList = new List<Transform>();
	NavMeshAgent agent;
	public GameObject[] waypoints = null;
	public bool needsNewTarget = true;
	
	void Awake () {
		
	}
	// Use this for initialization
	void Start () {
		waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
		//int count = 1;
		foreach (GameObject waypoint in waypoints)
		{
			targetList.Add(waypoint.transform);	
		}
		agent = GetComponent<NavMeshAgent>();
		needsNewTarget = false;
		
		target = targetList[Random.Range(0,targetList.Count)];
	}
	
	// Update is called once per frame
	void Update () {
			GoToTarget();
	}
	
	public void GoToTarget ()
	{
			agent.SetDestination (target.position);
	}
	public IEnumerator GetNewTarget(Collider other)
	{
		other.enabled = false; 
		//Unity's Random.Range for ints is exclusive
		int randTarget = Random.Range(0,targetList.Count);
		while(target == targetList[randTarget])
		{
			randTarget = Random.Range(0,targetList.Count);
		}
		target = targetList[randTarget];
		needsNewTarget = false;
		yield return new WaitForSeconds(5);
		other.enabled = true;
	}
	public void OnTriggerEnter (Collider other)
	{
		needsNewTarget = true;
		if (other.tag == "Waypoint" && other.name == target.name) {
			StartCoroutine (GetNewTarget (other));
		}
	}
	
}