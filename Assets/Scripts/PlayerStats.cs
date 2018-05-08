using UnityEngine;

public class PlayerStats : MonoBehaviour {
	// Since we want to control PlayerStats more freely (not attached to a player instance, which can be
	// destroyed and created from a cycle of actions) especially now where there are going to be upgrades.
	// Also making this a singleton class so only one instance of PlayerStats exist
	public static PlayerStats instance;

	public int maxHealth = 100;

	private int _currHealth = 100;
	public int currHealth {
		get { return _currHealth; }
		set { _currHealth = Mathf.Clamp (value, 0, maxHealth); }
	}

	public float movementSpeed = 10f;
	public float healthRegenRate = 2f;

	void Awake() {
		if (instance == null)
		{
			instance = this;
		}
	}
}
