using UnityEngine;
using UnityEngine.UI;

// Since Text is a component that will be within the same GameObject "PlayerInfo" we don't need to
// use [SerializeField] to manually add in the Text component
[RequireComponent(typeof(Text))]
public class LivesCounterUI : MonoBehaviour {

	private Text LivesCounterText;

	void Awake() {
		LivesCounterText = GetComponent<Text> ();
	}
	// Update is called once per frame
	void Update () {
		LivesCounterText.text = "LIVES: " + GameMaster.RemainingLives.ToString ();
	}
}
