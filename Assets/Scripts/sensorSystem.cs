using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    public class sensorSystem : MonoBehaviour
    {
        public sensorStatus[] sensors;

        public Transform[] shops;

        private int closestSpot;
        private int shopNo;
        private GameObject spotObject;
        private bool spotFound;

        // Finds the closest sensor (if any) to the objective shop, assigns it to a car
        // Shop 0 is Red, Shop 1 is blue
        public GameObject getClosest(string color)
		{
           
            float min = 999999;
            float distance;
            spotFound = false;

            if (color == "red")
                shopNo = 0;
            else if (color == "blue")
                shopNo = 1;
            else
                Debug.LogError("Error: non existing color or shop");
            
            for (int i = 0; i < sensors.Length; i++)
            {
                if (sensors[i].available)
				{
                    spotFound = true;
                    distance = Vector3.Distance(shops[shopNo].position, sensors[i].transform.position);
                    if (distance < min)
					{
                        spotObject = sensors[i].gameObject;
                        min = distance;
                        closestSpot = i;
                    }
                }
            }

            if (spotFound)
                sensors[closestSpot].setWaiting();
            else
                return null;

            return spotObject;
		}
    }
}