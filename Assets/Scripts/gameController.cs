using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Min_Max_Slider;

namespace simulation
{
    public class gameController : MonoBehaviour
    {
        public static gameController Instance = null;
        public GameObject redCarPrefab;
        public GameObject blueCarPrefab;
        private GameObject carsHolder;

        public TMP_InputField redCars;
        public TMP_InputField blueCars;
        public MinMaxSlider MinMaxTime;
        public MinMaxSlider ParkedDuration;
        public Button btnRunOnce;
        public Material btnMaterial;
        public GameObject pauseBtn;
        public GameObject playBtn;

        private int redCarAmount;
        private int blueCarAmount;

        private float minSpawnInterval;
        private float maxSpawnInterval;

        private bool isRunning;

        private void Awake()
        {
            Instance = this;
            isRunning = false;
            carsHolder = GameObject.Find("Cars");
            minSpawnInterval = MinMaxTime.minValue;
            maxSpawnInterval = MinMaxTime.maxValue;
        }

        // Only used for test runs
        void Start()
        {
            // StartCoroutine(tests());
        }

        // Only used for test runs
        private IEnumerator tests()
        {
            spawnRed(1);
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            spawnBlue(1);
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            spawnBlue(1);
        }

        // Checks when the simulation has ended (when there are no more cars in the carHolder)
		private void Update()
		{
			if (carsHolder.transform.childCount <= 0)
			{
                isRunning = false;

                redCars.interactable = true;
                blueCars.interactable = true;

                btnRunOnce.image.material = btnMaterial;
                btnRunOnce.interactable = true;

                pauseBtn.SetActive(false);
                playBtn.SetActive(false);
            }
		}

        // Runs the simulation once with the given values of car amounts and spawn interval
		public void startOnce()
		{
            if (!isRunning)
            {
                isRunning = true;
                
                redCarAmount = int.Parse(redCars.text);
                redCars.interactable = false;
                blueCarAmount = int.Parse(blueCars.text);
                blueCars.interactable = false;

                StartCoroutine(runOnce());
            }
            else
                Debug.LogError("Error: simulation already running");
        }

        // when the values on the MinMaxSlider are changed, this function is called
        public void updateTimeIntervals()
		{
            minSpawnInterval = MinMaxTime.minValue;
            maxSpawnInterval = MinMaxTime.maxValue;
        }

        // Runs the simulation once with the given values of car amounts and spawn interval
        private IEnumerator runOnce()
		{
            int redLeft = redCarAmount;
            int blueLeft = blueCarAmount;
            int nextCar;

            // spawn points: 1 or 2
            int spawnPoint;

            for (int i = 0; i < (redCarAmount + blueCarAmount); i++)
			{
                if (redLeft == 0)
                    nextCar = 2;
                else if (blueLeft == 0)
                    nextCar = 1;
				else
                    nextCar = Random.Range(1, 3);

                spawnPoint = Random.Range(1, 3);

                if (nextCar == 1)
                {
                    spawnRed(spawnPoint);
                    redLeft--;
                }
				else
				{
                    spawnBlue(spawnPoint);
                    blueLeft--;
                }

                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            }
            
            yield return null;
		}

        // Instantiates a Red Car prefab at spawnPoint = point;
        void spawnRed(int point)
		{
            GameObject car_R = Instantiate(redCarPrefab);
            car_R.GetComponent<carNavigation>().spawnPoint = point;
            car_R.GetComponent<carNavigation>().color = "red";
            car_R.GetComponent<carNavigation>().parkDuration = Random.Range(ParkedDuration.minValue, ParkedDuration.maxValue);
            car_R.transform.parent = carsHolder.transform;
        }

        // Instantiates a Blue Car prefab at spawnPoint = point;
        void spawnBlue(int point)
		{
            GameObject car_B = Instantiate(blueCarPrefab);
            car_B.GetComponent<carNavigation>().spawnPoint = point;
            car_B.GetComponent<carNavigation>().color = "blue";
            car_B.GetComponent<carNavigation>().parkDuration = Random.Range(ParkedDuration.minValue, ParkedDuration.maxValue);
            car_B.transform.parent = carsHolder.transform;
        }
    }
}