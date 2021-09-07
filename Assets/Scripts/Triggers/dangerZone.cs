using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
	// Used for road intersections
	// Detects if there is any car in certain area
	// Can be called by other objects to know so
	public class dangerZone : MonoBehaviour
	{
		public bool carInZone;

		private void Awake()
		{
			carInZone = false;
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				carInZone = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				carInZone = false;
			}
		}
	}
}
