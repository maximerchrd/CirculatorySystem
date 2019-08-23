using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Text correctionText;
	// Use this for initialization
	void Start ()
	{
		correctionText.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LevelIncorrect()
	{
		correctionText.text = "Unfortunately, there was something incorrect :-(";
		Invoke("ResetText", 5f);
	}

	private void ResetText()
	{
		correctionText.text = "";
	}
	
	public void LevelPassed()
	{
		correctionText.text = "Congratulations! You passed this Level!";
		Invoke("LoadNextLevel", 3f);
	}

	private void LoadNextLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
