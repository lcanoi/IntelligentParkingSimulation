using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    // With the use of triggers, detects when a Car is about to collide with another
	// stops and sets the Car running accordingly
	public class carTrigger : MonoBehaviour
    {
        public carNavigation car;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				car.running = false;
				car.stop = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Car"))
			{
				StartCoroutine(accelerate());
			}
		}

		IEnumerator accelerate()
		{
			yield return new WaitForSeconds(0.5f);
			car.running = true;
			car.stop = false;
		}
	}
}
