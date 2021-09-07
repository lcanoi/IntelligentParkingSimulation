using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    public class carColliderScript : MonoBehaviour
    {
        // Main body collider of the Car
        // Also allows other scripts to easily access this Car's
        // carNavigation component on collision/trigger
        public carNavigation thisCar;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("EndZone"))
                Destroy(thisCar.gameObject, 0.25f);
        }
    }
}