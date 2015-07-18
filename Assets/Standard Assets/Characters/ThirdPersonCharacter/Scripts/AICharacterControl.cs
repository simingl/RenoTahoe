using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]

    public class AICharacterControl : MonoBehaviour
    {
        public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Transform target; // target to aim for

		public GameObject[] waypoints = null;

		public AudioClip callForHelp;
		public bool canCallForHelp = true;

        // Use this for initialization
        private void Start()
        {
            // get the components on the object we need ( should not be null due to require component so no need to check )
            agent = GetComponentInChildren<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;

			waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

			target = waypoints[UnityEngine.Random.Range(0,waypoints.Length)].transform;
        }

		private float timer = 0;

        // Update is called once per frame
        private void Update()
        {
			timer += Time.deltaTime;

            if (target != null)
            {
				//Vector3 dest = new Vector3(-100f,100f,0f);
				//agent.SetDestination(dest);
                agent.SetDestination(target.position);

				
				
                // use the values to move the character
                character.Move(agent.desiredVelocity, false, false);
            }
            else
            {
				if((int)timer % 30 == 0){
					Vector3 dest = this.generateRandomPosition(1000, 1000);
					agent.SetDestination(dest);
					character.Move(agent.desiredVelocity, false, false);
				}
                // We still need to call the character's move function, but we send zeroed input as the move param.
                
            }

        }


        public void SetTarget(Transform target)
        {
            this.target = target;
        }

		private Vector3 generateRandomPosition(int w, int h){
			float x = UnityEngine.Random.Range(-1*w, h);
			float z = UnityEngine.Random.Range(-1*w, h);
			
			return new Vector3 (x,1,z);
		}

		public void OnTriggerEnter (Collider other)
		{
			Debug.Log (other.gameObject.name.ToString());
			int delay = 9;
			StartCoroutine(CallForHelp(delay, other));
			
			
		}
		public void OnTriggerStay (Collider other)
		{
			int delay = 9;
			StartCoroutine(CallForHelp(delay, other));
		}
		public IEnumerator CallForHelp(int delay, Collider other)
		{
			if (other.tag == "Drone" && canCallForHelp) 
			{
				AudioSource.PlayClipAtPoint(callForHelp, transform.position, 0.3f);
				canCallForHelp = false;
				yield return new WaitForSeconds(delay);
				canCallForHelp = true;
				
			}
		}
    }
}
