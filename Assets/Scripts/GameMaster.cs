using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {
	// Make GameMaster a Singleton Class (Only have one instance of GM running)
	public static GameMaster gm;

	// Reason for making remainingLives static is so that it is easily accessible through all
	// files, without having to instantiate a class
	[SerializeField]
	private int maxLives = 3;
	private static int _remainingLives;
	public static int RemainingLives {
		get { return _remainingLives; }
	}
	[SerializeField]
	private int startingMoney;
	public static int Money;

	void Awake () {
		if (gm == null) {
			gm = GameObject.FindGameObjectWithTag ("GM").GetComponent<GameMaster> ();
			//gm = this;
		}
	}
	// Above ^ allows us to access the instance of the GM class on top of using its methods.

	public Transform playerPrefab;
	public Transform spawnPoint;
	public float spawnDelay = 3;
	public Transform spawnPrefab;
	// public AudioClip respawnAudio;
	public string respawnCountdownSoundName = "RespawnCountdown";
	public string spawnSoundName = "Spawn";

	public CameraShake cameraShake;

	public Transform enemyPrefab;
	public Transform enemySpawnPoint;
	public float enemySpawnDelay = 1f;

	[SerializeField]
	private GameObject gameOverUI;

	public string gameOverSound = "GameOver";

	[SerializeField]
	private GameObject upgradeMenu;

	[SerializeField]
	private WaveSpawner waveSpawner;

	// To disable other components while upgradeMenu is active, we will use 'delegate'
	public delegate void UpgradeMenuCallback(bool _active);
	public UpgradeMenuCallback onToggleUpgradeMenu;

	// caching
	private AudioManager audioManager;

	void Start () {
		if (cameraShake == null) {
			Debug.LogError ("No cameraShake referenced in GM");
		}
		_remainingLives = maxLives;
		Money = startingMoney;

		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("No AudioManager instance found in this Scene!");
		}
	}

	void Update() 
	{
		if (Input.GetKeyDown (KeyCode.U))
		{
			ToggleUpgradeMenu ();
		}
	}

	private void ToggleUpgradeMenu()
	{
		upgradeMenu.SetActive (!upgradeMenu.activeSelf);
		// When upgradeMenu is active, disable other necessary parts
		// Invoke will invoke all the methods that are subscribed to this event
		onToggleUpgradeMenu.Invoke (upgradeMenu.activeSelf);
		// ^ Above only invokes methods that are subscribed, not applicable for enemies that are not yet spawned.
		// Hence we will simply disable the script that spawns enemies
		waveSpawner.enabled = !upgradeMenu.activeSelf;
	}

	public void EndGame() {
		Debug.Log("Game Over");
		audioManager.PlaySound (gameOverSound);
		gameOverUI.SetActive (true);
	}
	// NTS: creating a static method allows other scripts to use the method without having to create an object
	public static void KillPlayer (Player player) {
		Destroy (player.gameObject);
		_remainingLives -= 1;
		if (_remainingLives <= 0){
			gm.EndGame();
		} else {
			gm.StartCoroutine (gm._RespawnPlayer ());
		}
	}

	// In order to use 'yield' we make it into a IEnumerator and use Coroutine
	private IEnumerator _RespawnPlayer () {
		//AudioSource.PlayClipAtPoint (respawnAudio,
		//	new Vector3 (spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z), 1f);
		audioManager.PlaySound (respawnCountdownSoundName);

		yield return new WaitForSeconds (spawnDelay);
		audioManager.PlaySound (spawnSoundName);

		Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
		Transform spawnClone = Instantiate (spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;


		Destroy (spawnClone.gameObject, 3f);
	}

	public static void KillEnemy (Enemy enemy) {
		gm._KillEnemy (enemy); 
	}
	// Non-static method for particles (local)
	private void _KillEnemy (Enemy _enemy) {
		// Add particles
		Transform _clone = Instantiate (_enemy.deathParticles, _enemy.transform.position, Quaternion.identity) as Transform;
		Destroy (_clone.gameObject, 3f);

		// Camera Shake
		cameraShake.Shake (_enemy.shakeAmt, _enemy.shakeLength);
		Destroy (_enemy.gameObject);

		// Add Money
		Money += _enemy.moneyDrop;
		audioManager.PlaySound ("Money");

		// Play Enemy Explosion Sound
		audioManager.PlaySound (_enemy.deathSoundName);
	}
}
