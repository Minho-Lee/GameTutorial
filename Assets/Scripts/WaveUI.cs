using UnityEngine.UI;
using UnityEngine;

public class WaveUI : MonoBehaviour {

	[SerializeField]
	private WaveSpawner spawner;

	[SerializeField]
	private Animator waveAnimator;

	[SerializeField]
	private Text waveCountdownText;

	[SerializeField]
	private Text waveCountText;

	private WaveSpawner.SpawnState previousState;

	// Use this for initialization
	void Start () {
		if (spawner == null) {
			Debug.LogError ("Spawner reference is null!");
			this.enabled = false;
		}
		if (waveAnimator == null) {
			Debug.LogError ("waveAnimator reference is null!");
			this.enabled = false;
		}
		if (waveCountdownText == null) {
			Debug.LogError ("waveCountdownText reference is null!");
			this.enabled = false;
		}
		if (waveCountText == null) {
			Debug.LogError ("waveCountText reference is null!");
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (spawner.State) {
			
			case WaveSpawner.SpawnState.COUNTING:
				UpdateCountingUI ();
				break;
			case WaveSpawner.SpawnState.SPAWNING:
				UpdateSpawningUI ();
				break;
		}

		previousState = spawner.State;
	}

	void UpdateCountingUI() {
		if (previousState != WaveSpawner.SpawnState.COUNTING) {
			Debug.Log ("COUNTING!");
			waveAnimator.SetBool ("WaveIncoming", false);
			waveAnimator.SetBool ("WaveCountdown", true);
		}
		waveCountdownText.text = Mathf.CeilToInt(spawner.WaveCountdown).ToString();

	}

	void UpdateSpawningUI() {
		if (previousState != WaveSpawner.SpawnState.SPAWNING) {
			Debug.Log ("SPAWNING!");
			waveAnimator.SetBool ("WaveIncoming", true);
			waveAnimator.SetBool ("WaveCountdown", false);

			waveCountText.text = spawner.NextWave.ToString ();
		}
	}
}
