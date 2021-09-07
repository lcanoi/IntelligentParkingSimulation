using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
	// Trigger used for the turnEnd waypoints
	public class turnEndTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				other.gameObject.GetComponent<carColliderScript>().thisCar.triggerTurnEnd(gameObject);
			}
		}
	}
}