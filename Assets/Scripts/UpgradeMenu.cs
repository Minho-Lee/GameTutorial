using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour {

	[SerializeField]
	private Text healthText;

	[SerializeField]
	private Text speedText;

	[SerializeField]
	private float healthMultiplier = 1.2f;

	[SerializeField]
	private float speedMultiplier = 1.2f;

	[SerializeField]
	private int upgradeCost = 50;

	private PlayerStats playerStats;

	void OnEnable()
	{
		playerStats = PlayerStats.instance;
		UpdateValues ();
	}

	void UpdateValues()
	{
		healthText.text = "HEALTH: " + playerStats.maxHealth.ToString ();
		speedText.text = "SPEED: " + playerStats.movementSpeed.ToString ();
	}

	public void UpgradeHealth() 
	{
		if (GameMaster.Money < upgradeCost)
		{
			AudioManager.instance.PlaySound ("NoMoney");
			return;
		}
		playerStats.maxHealth = (int)(playerStats.maxHealth * healthMultiplier);
		GameMaster.Money -= upgradeCost;
		AudioManager.instance.PlaySound ("Money");
		UpdateValues ();
	}

	public void UpgradeSpeed() 
	{
		if (GameMaster.Money < upgradeCost)
		{
			AudioManager.instance.PlaySound ("NoMoney");
			return;
		}
		playerStats.movementSpeed = Mathf.Round (playerStats.movementSpeed * speedMultiplier);
		GameMaster.Money -= upgradeCost;
		AudioManager.instance.PlaySound ("Money");
		UpdateValues ();
	}
}
