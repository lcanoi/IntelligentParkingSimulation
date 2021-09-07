using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
	public class spotTrigger : MonoBehaviour
	{
		public bool carIncoming;

		public carNavigation myCar;
		public carNavigation otherCar;

		public bool hasCar;
		public bool checkStop;

		private void Awake()
		{
			carIncoming = false;
			hasCar = false;
			checkStop = false;
		}

		// When unparking, check there is not a stopped car behind the parked car
		private IEnumerator checkingStop()
		{
			while (checkStop)
			{
				if (otherCar.stop)
				{
					myCar.stop = true;
				}
				else
				{
					myCar.stop = false;
				}
				yield return null;
			}
			
			yield return null;			
		}

		private void OnTriggerEnter(Collider other)
		{
			// Check if the car should be parked on this spot
			if (other.CompareTag("Car"))
			{
				otherCar = other.gameObject.GetComponent<carColliderScript>().thisCar;
				otherCar.triggerParking(gameObject);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			// When unparking, check there is not another car incomming that could collision if this Car went out now
			if (other.CompareTag("carIncoming"))
				carIncoming = true;

			// When unparking, check there is not a stopped car behind the parked car
			if (!checkStop)
			{
				if (hasCar)
				{
					if (other.CompareTag("Car"))
					{
						checkStop = true;
						StartCoroutine(checkingStop());
					}
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("carIncoming"))
				carIncoming = false;

			if (other.CompareTag("Car"))
			{
				checkStop = false;
			}
		}
	}
}