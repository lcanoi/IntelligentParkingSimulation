using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace simulation
{
    public class sensorStatus : MonoBehaviour
    {
        public GameObject statusRed;
        public GameObject statusYellow;
        public GameObject statusGreen;

        public bool available;

        public GameObject parkTrigger;

        // A pressure sensor can have 3 states
        // Occupied: when a Car is on top of the sensor
        // Waiting: when it has been assigned to a Car and is waiting for the Car to get there
        // Ready: when it is available to be assigned
        enum STATE
        {
            NONE,
            OCCUPIED,
            WAITING,
            READY
        }

        private STATE state = STATE.NONE;

        private void Awake()
		{
            SetState(STATE.READY);
		}

        public void setOccupied()
        {
            SetState(STATE.OCCUPIED);
        }
        public void setWaiting()
        {
            SetState(STATE.WAITING);
        }

        public void setReady()
        {
            SetState(STATE.READY);
        }

        // Depending on the STATE, sets if the sensor is available or not
        // Activates/Deactivates the different colored objects in the sensor to leave only the correct one active
        void SetState(STATE state)
		{
            if (this.state == state) return;
            this.state = state;
            switch (state)
            {
                case STATE.OCCUPIED:
					{
                        statusRed.SetActive(true);
                        statusYellow.SetActive(false);
                        statusGreen.SetActive(false);
                        available = false;
                    }
                    break;
                case STATE.WAITING:
					{
                        statusRed.SetActive(false);
                        statusYellow.SetActive(true);
                        statusGreen.SetActive(false);
                        available = false;
                    }
                    break;
                case STATE.READY:
					{
                        statusRed.SetActive(false);
                        statusYellow.SetActive(false);
                        statusGreen.SetActive(true);
                        available = true;
                    }
                    break;
            }
        }
	}
}