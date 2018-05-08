using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

[RequireComponent(typeof(Platformer2DUserControl), typeof(Rigidbody2D))]
public class Player : MonoBehaviour {

	public int fallBoundary = -20;

	public string deathSoundName = "DeathVoice";
	public string damageSoundName = "Grunt";

	// caching
	private AudioManager audioManager;

	[SerializeField]
	private StatusIndicator statusIndicator;

	private PlayerStats playerStats;

	void Start () {
		playerStats = PlayerStats.instance;

		// When player dies and respawns, new Player clone is created but no where it says the currHP to maxHP
		// Hence we need to add this line (or else shows as 0/100 when respawned).
		playerStats.currHealth = playerStats.maxHealth;
		if (statusIndicator == null) {
			Debug.LogError ("No statusIndicator referenced on Player!");
		} else {
			statusIndicator.SetHealth (playerStats.currHealth, playerStats.maxHealth);
		}

		// caching
		audioManager = AudioManager.instance;
		if (audioManager == null) {
			Debug.LogError ("No AudioManager instance found in Main Scene!");
		}
		//Subscribing to the delegate event from GM class
		GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

		InvokeRepeating ("RegenHealth", 1f/ playerStats.healthRegenRate, 1f/ playerStats.healthRegenRate);
	}

	void RegenHealth()
	{
		playerStats.currHealth += 1;
		statusIndicator.SetHealth (playerStats.currHealth, playerStats.maxHealth);
	}

	void Update () {
		if (transform.position.y <= fallBoundary) {
			DamagePlayer (playerStats.maxHealth);
		}
	}

	void OnUpgradeMenuToggle(bool _active)
	{
		// Handle what happens when upgradeMenu is toggled
		GetComponent <Platformer2DUserControl> ().enabled = !_active;
		GetComponent <Rigidbody2D>().simulated = !_active;
		Weapon _weapon = GetComponentInChildren<Weapon> ();
		if (_weapon != null) {
			_weapon.enabled = !_active;
		}
	}

	public void DamagePlayer (int damage) {
		playerStats.currHealth -= damage;
		if (playerStats.currHealth <= 0) {
			Debug.Log ("Player DEAD!");
			GameMaster.KillPlayer (this);

			// Play Death Sound
			audioManager.PlaySound (deathSoundName);
		} else {
			// Play Grunt Sound
			audioManager.PlaySound (damageSoundName);
		}
		statusIndicator.SetHealth (playerStats.currHealth, playerStats.maxHealth);
	}

	void OnDestroy() 
	{
		GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;
	}
}
