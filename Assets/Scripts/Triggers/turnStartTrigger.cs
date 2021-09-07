using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
	// Trigger used for the turnStart waypoints
	public class turnStartTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				other.gameObject.GetComponent<carColliderScript>().thisCar.triggerTurnStart();
			}
		}
	}
}