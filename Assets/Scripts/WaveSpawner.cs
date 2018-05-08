using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour {

	// This simply defines the data structure. Need to use it as a variable.
	public enum SpawnState { SPAWNING, WAITING, COUNTING };

	[System.Serializable]
	public class Wave {
		public string name;
		public Transform enemy;
		public int count;
		public float spawnRate;

	}

	public Wave[] waves;
	private int nextWave = 0;
	public int NextWave {
		get { return nextWave + 1; }
	}

	public Transform[] spawnPoints;

	public float timeBetweenWaves = 3f;
	private float waveCountdown;
	public float WaveCountdown {
		get { return waveCountdown; }
	}

	// To not check for if enemies are alive every frame but rather longer wait period
	private float searchCountdown = 1f;

	// Setting it private makes it unavailble for other classes to modify the local state of the variable
	// In order to make it accessible while keeping 'state' private, we create a getter method for public 'State'
	private SpawnState state = SpawnState.COUNTING;
	public SpawnState State {
		get { return state; }
	}

	void Start() {
		
		waveCountdown = timeBetweenWaves;
	}

	void Update() {
		//Debug.Log (state);
		if (state == SpawnState.WAITING) {
			// Check if any enemies are still alive
			if (!IsEnemyAlive ()) {
				// Begin a new round (new wave)
				WaveCompleted();
				return;
			} else {
				return;
			}
		}

		if (waveCountdown <= 0) {
			Debug.Log ("Wave Countdown is 0!");
			if (state != SpawnState.SPAWNING) {
				// Start Spawning wave
				StartCoroutine (SpawnWave (waves [nextWave]));
			}
		}
		else {
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted () {
		Debug.Log ("Wave Completed!");

		state = SpawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if (nextWave + 1 >= waves.Length) {
			nextWave = 0;
			Debug.Log ("All Waves Complete! LOOPING");
		} else {
			nextWave++;
		}
	}

	bool IsEnemyAlive() {
		searchCountdown -= Time.deltaTime;
		if (searchCountdown <= 0f) {
			// re-initalize
			searchCountdown = 1f;
			if (GameObject.FindGameObjectWithTag ("Enemy") == null) {
				Debug.Log ("No Enemy Found!");
				return false;
			}
		}
		return true;
	}

	IEnumerator SpawnWave (Wave _wave) {
		Debug.Log ("Spawning Wave: " + _wave.name);
		state = SpawnState.SPAWNING;
		// Spawn
		for (int i = 0; i < _wave.count; i++) {
			SpawnEnemy (_wave.enemy);
			yield return new WaitForSeconds (1f / _wave.spawnRate);
		}

		state = SpawnState.WAITING;
		yield break;
	}

	void SpawnEnemy (Transform _enemy) {
		// Spawn Enemy
		Debug.Log("Spawning enemy: " + _enemy.name);

		if (spawnPoints.Length == 0) {
			Debug.LogError ("No Spawn Points referenced");
		}
		Transform _sp = spawnPoints [Random.Range (0, spawnPoints.Length)];

		Instantiate(_enemy, _sp.position, _sp.rotation);
	}

}
