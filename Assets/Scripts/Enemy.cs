using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyAI), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour {
	// This allows the user to access the class created within a class inside Unity Inspector
	[System.Serializable]
	public class EnemyStats {
		public int maxHealth = 100;

		private int _currHealth;
		// assigning a getter a setter method for currHealth
		public int currHealth {
			get { return _currHealth; }
			set { _currHealth = Mathf.Clamp (value, 0, maxHealth); }
		}

		public int damage = 40;

		public void Init() {
			currHealth = maxHealth;
		}
	}
	public EnemyStats enemyStats = new EnemyStats ();
	public Transform deathParticles;
	public float shakeAmt = 0.1f;
	public float shakeLength = 0.1f;

	public string deathSoundName = "Explosion";

	public int moneyDrop = 10;

	[Header("Optional")]
	[SerializeField]
	private StatusIndicator statusIndicator;

	void Start() {
		enemyStats.Init();
		if (statusIndicator != null) {
			statusIndicator.SetHealth (enemyStats.currHealth, enemyStats.maxHealth); 
		}
		if (deathParticles == null) {
			Debug.LogError ("No deathParticles found for enemy");
		}

		GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;
	}

	void OnUpgradeMenuToggle(bool _active)
	{
		// Handle what happens when upgradeMenu is toggled
		GetComponent <EnemyAI> ().enabled = !_active;
		GetComponent <Rigidbody2D>().simulated = !_active;
	}

	public void DamageEnemy (int damage) {
		enemyStats.currHealth -= damage;
		if (enemyStats.currHealth <= 0) {
			Debug.Log ("Enemy DEAD!");
			GameMaster.KillEnemy (this);
		}
		if (statusIndicator != null)
			statusIndicator.SetHealth (enemyStats.currHealth, enemyStats.maxHealth);


	}

	// This method is a reserved method name similar to 'Start()' or 'Awake()'
	private void OnCollisionEnter2D(Collision2D _colInfo) {
		Player _player = _colInfo.collider.GetComponent<Player> ();
		if (_player != null) {
			_player.DamagePlayer (enemyStats.damage);
			// Whenever the enemy hits the player, suicide
			DamageEnemy (99999);
		}
	}

	void OnDestroy()
	{
		// Upon destroying an Enemy instance, it should stop subcribing to avoid null-pointer exception
		GameMaster.gm.onToggleUpgradeMenu -= OnUpgradeMenuToggle;	
	}
}
