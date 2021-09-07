using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    public class sensorTrigger : MonoBehaviour
    {
        public sensorStatus sensor;

        public bool occupied;

		private void Awake()
		{
            occupied = false;
		}

		// Detects when a Car gets on top of the sensor
        // Changes the STATE of the sensor accordingly
		public void OnTriggerEnter(Collider other)
        {
            if (!occupied)
			{
                if (other.CompareTag("Car"))
                {
                    sensor.setOccupied();
                    sensor.parkTrigger.GetComponent<spotTrigger>().myCar = other.gameObject.GetComponent<carColliderScript>().thisCar;
                    sensor.parkTrigger.GetComponent<spotTrigger>().hasCar = true;
                    occupied = true;
                }
            }
        }

        // Detects when a Car leaves the sensor
        public void OnTriggerExit(Collider other)
        {
            
            if (other.CompareTag("Car"))
            {
                sensor.setReady();
                occupied = false;
            }
        }
    }
}