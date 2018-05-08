using UnityEngine.UI;
using UnityEngine;

// Since Text is a component that will be within the same GameObject "PlayerInfo" we don't need to
// use [SerializeField] to manually add in the Text component
[RequireComponent(typeof(Text))]
public class MoneyCounterUI : MonoBehaviour {

	private Text MoneyCounterText;

	void Awake() {
		MoneyCounterText = GetComponent<Text> ();
	}
	// Update is called once per frame
	void Update () {
		MoneyCounterText.text = "MONEY: " + GameMaster.Money.ToString ();
	}
}
