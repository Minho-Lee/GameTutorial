using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour {
	// SerializeField will make it show up on the editor even though it is private
	[SerializeField]
	private RectTransform healthBarRect;
	[SerializeField]
	private Text healthText;

	void Start () {
		if (healthBarRect == null) 
			Debug.LogError ("Status Indicator: NO health bar object referenced!");
		
		if (healthText == null)
			Debug.LogError ("Status Indicator: NO health text object referenced!");
	}

	public void SetHealth(int _cur, int _max) {
		float _value = (float)_cur / _max;

		// Decrease the x scale value of the healthBar in percentage
		healthBarRect.localScale = new Vector3 (_value, healthBarRect.localScale.y, healthBarRect.localScale.z);
		healthText.text = _cur + " / " + _max + " HP";

	}

}
