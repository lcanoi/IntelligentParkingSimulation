using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace simulation
{
    public class carNavigation : MonoBehaviour
    {
        private PathCreator mainRoad;
        private PathCreator secondRoad;
        private PathCreator uTurn;
        private PathCreator parkRoad;
        public PathCreator currentRoad;

        private sensorSystem sensors;
        public GameObject parkSpot;
        private bool hasSpot;

        private dangerZone dangerZoneU;
        private dangerZone dangerZoneP;

        private Transform startParking;
        private Transform backToRoad;

        [HideInInspector]
        public int spawnPoint = 0;
        [HideInInspector]
        public string color;

        public bool running;
        public bool turning;
        public bool stopped;
        public bool isParking;
        public bool isUnparking;
        public bool isParked;
        public bool stop;
        public bool goingOut;

        public float speed = 15;
        float distanceTravelled;

        private void Awake()
        {
            // Find al objects in scene the car will need to reference
            mainRoad = GameObject.Find("MainRoad").GetComponent<PathCreator>();
            secondRoad = GameObject.Find("SecondRoad").GetComponent<PathCreator>();
            uTurn = GameObject.Find("UTurn").GetComponent<PathCreator>();
            parkRoad = GameObject.Find("ParkRoad").GetComponent<PathCreator>();

            sensors = GameObject.Find("SensorSystem").GetComponent<sensorSystem>();

            startParking = GameObject.Find("StartParking").GetComponent<Transform>();
            backToRoad = GameObject.Find("BackToRoad").GetComponent<Transform>();

            dangerZoneU = GameObject.Find("DangerZoneU").GetComponent<dangerZone>();
            dangerZoneP = GameObject.Find("DangerZoneP").GetComponent<dangerZone>();

            // Initial boolean values
            running = true;
            turning = false;
            stop = false;
            hasSpot = false;
            isParking = false;
            isUnparking = false;
            isParked = false;
            goingOut = false;
        }

        void Start()
        {
            // Spawn at the leftmost point of mainRoad or rightmost point of seconRoad
            if (spawnPoint == 1)
            {
                transform.position = new Vector3(-10, 0.0721005f, 25);
                currentRoad = mainRoad;
            }
            else if (spawnPoint == 2)
            {
                transform.position = new Vector3(160, 0.0721005f, 0);
                currentRoad = secondRoad;
            }
            else
                Debug.LogError("Error: invalid Spawn Point");

            Quaternion newRotation = currentRoad.path.GetRotation(currentRoad.path.GetClosestTimeOnPath(transform.position));
            newRotation.x = 0;
            newRotation.z = 0;
            transform.rotation = newRotation;

            distanceTravelled = currentRoad.path.GetClosestDistanceAlongPath(transform.position);

            getSpot();
        }

        // Call sensorSystem to get a spot near the desired Shop
        public void getSpot()
        {
            parkSpot = sensors.getClosest(color);
            if (parkSpot != null)
                hasSpot = true;
            else
                Debug.Log("No spot found");
        }

        // Called every frame, uses booleans to determine the current state of the car and move it accordingly
        void Update()
        {
            if (running)
            {
                if (!turning && !isParking && !isUnparking)
                {
                    distanceTravelled += speed * Time.deltaTime;
                    //if (running)
                    //    transform.position = currentRoad.path.GetPointAtDistance(distanceTravelled);
                    transform.position = Vector3.MoveTowards(transform.position, currentRoad.path.GetPointAtDistance(distanceTravelled), 15 * Time.deltaTime);
                    Quaternion newRotation = currentRoad.path.GetRotationAtDistance(distanceTravelled);
                    newRotation.x = 0;
                    newRotation.z = 0;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, 720f * Time.deltaTime);
                }
            }
        }

        // mainRoad and parkRoad are not connected (for mesh reasons)
        // when jumping from one another a curve movement needs to be done without using PathCreator
        private IEnumerator changeRoadTurn(Transform waypoint)
        {
            Vector3 targetPoint = waypoint.position;
            Quaternion targetRot = waypoint.rotation;
            targetRot.x = 0;
            targetRot.z = 0;

            while ((transform.position.x != targetPoint.x) || (transform.position.z != targetPoint.z))
            {
                if (!stop)
				{
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 160f * Time.deltaTime);
                }
                yield return null;
            }

            // targetPoint is not at the begining of the new currentRoad
            // distanceTravelled needs to be recalculated before going back to the Update() car movement
            yield return new WaitForSeconds(0.05f);
            distanceTravelled = currentRoad.path.GetClosestDistanceAlongPath(targetPoint);
            turning = false;

            yield return null;
        }

        // before exiting UTurn, the car stops to check if there are no incoming cars on the mainRoad
        private IEnumerator checkIncomingCarsU()
        {

            while (stopped)
            {
                if (!dangerZoneU.carInZone)
                    stopped = false;
                yield return null;
            }

            stopped = true;
            running = true;
            yield return null;
        }

        // before exiting parkRoad, the car stops to check if there are no incoming cars on the mainRoad
        private IEnumerator checkIncomingCarsP()
        {
            
            while (stopped)
            {
                if (!dangerZoneP.carInZone)
                    stopped = false;
                yield return null;
            }
            
            currentRoad = mainRoad;
            turning = true;
            running = true;
            StartCoroutine(changeRoadTurn(backToRoad));

            yield return null;
        }

        // the Car gets his park spot position and rotation, and parks accordingly
        private IEnumerator parking()
        {
            yield return new WaitForSeconds(0.05f);
            Vector3 targetPoint = parkSpot.transform.position;
            Quaternion targetRot = parkSpot.transform.rotation;
            targetRot.x = 0;
            targetRot.z = 0;

            while ((transform.position.x != targetPoint.x) || (transform.position.z != targetPoint.z))
            {
                //if (!stop)
                //{
                    transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * 0.6f * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 60f * Time.deltaTime);
                //}
                yield return null;
            }

            isParking = false;
            isParked = true;
            StartCoroutine(parked());

            yield return null;
        }

        // stays parked for a random amount of time, then starts the process of going out
        private IEnumerator parked()
        {
            float parkDuration = Random.Range(5f, 15f);
            yield return new WaitForSeconds(parkDuration);
            goingOut = true;
            StartCoroutine(checkCarsBehind());
            yield return null;
        }

        // before going out, the Car makes sure there are no Incomming cars
        private IEnumerator checkCarsBehind()
        {

            while (isParked)
            {
                if (!parkSpot.GetComponent<sensorStatus>().parkTrigger.GetComponent<spotTrigger>().carIncoming)
                    isParked = false;
                yield return null;
            }

            isUnparking = true;
            StartCoroutine(unparking());
            yield return null;
        }

        // gets the position and rotation of the place on the road where the Car should go
        // then unparks accordingly
        private IEnumerator unparking()
        {

            float angleFix;
            if (transform.position.x > 82.5f && transform.position.x < 130f && transform.position.z > 47.5f && transform.position.z < 70f)
                angleFix = -80;
            else
                angleFix = 90;

            Vector3 targetPoint = parkSpot.GetComponent<sensorStatus>().parkTrigger.transform.position;
            Quaternion targetRot = parkSpot.transform.rotation;
            
            targetRot.x = 0;
            targetRot.z = 0;
            targetRot *= Quaternion.Euler(0, angleFix, 0);

            while ((transform.position.x != targetPoint.x) || (transform.position.z != targetPoint.z))
            {
				if (!stop)
				{
					transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * 0.6f * Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 62f * Time.deltaTime);
                }
                yield return null;
            }

            distanceTravelled = currentRoad.path.GetClosestDistanceAlongPath(transform.position);
            
            yield return new WaitForSeconds(0.2f);
            isUnparking = false;
            running = true;
        }

        // The car uses multiple waypoints (Triggers) to determine where it is and know what to do:
        // Change road, turn, stop, park...
		public void triggerTurnStart()
		{
            if (hasSpot)
            {
                if (!turning)
                {
                    if (currentRoad == mainRoad)
                    {
                        currentRoad = parkRoad;
                        turning = true;
                        StartCoroutine(changeRoadTurn(startParking));
                    }
                    else if (currentRoad == secondRoad)
                    {
                        distanceTravelled = 0;
                        currentRoad = uTurn;
                    }
                }
            }
        }

        public void triggerTurnStop()
		{
            if (!stopped)
            {
                if (currentRoad == uTurn)
                {
                    running = false;
                    stopped = true;
                    StartCoroutine(checkIncomingCarsU());
                }
                else if (currentRoad == parkRoad)
                {
                    running = false;
                    stopped = true;
                    StartCoroutine(checkIncomingCarsP());
                }
            }
        }

        public void triggerTurnEnd(GameObject other)
		{
            if (currentRoad == uTurn)
            {
                stopped = false;
                currentRoad = mainRoad;
                distanceTravelled = currentRoad.path.GetClosestDistanceAlongPath(other.transform.position);
            }
        }

        public void triggerParking(GameObject other)
		{
            if (!goingOut)
            {
                if (parkSpot.GetComponent<sensorStatus>().parkTrigger == other)
                {
                    running = false;
                    isParking = true;
                    StartCoroutine(parking());
                }
            }
        }
	}
}