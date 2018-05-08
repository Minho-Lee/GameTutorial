using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {

	private AudioManager audioManager;

	[SerializeField]
	private string MouseHoverSound = "ButtonHover";

	[SerializeField]
	private string ButtonPressSound = "ButtonPress";

	void Start() 
	{
		audioManager = AudioManager.instance;
		if (audioManager == null)
		{
			Debug.LogError ("No audioManager instance found in MenuManager");
		}
	}

	public void Quit() {
		audioManager.PlaySound (ButtonPressSound);
		Debug.Log ("Application QUIT!");
		Application.Quit ();
	}

	public void Retry() {
		audioManager.PlaySound (ButtonPressSound);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}

	public void onMouseOver() {
		audioManager.PlaySound (MouseHoverSound);
	}
}
