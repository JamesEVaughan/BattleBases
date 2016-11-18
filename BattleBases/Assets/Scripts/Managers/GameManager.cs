using UnityEngine;
using System.Collections;

using UnityEngine.UI;

/// <summary>
/// GameManager is our global game state manager. For now it
/// focuses on when the game is over, but other functionality may be added in time.
/// It shouldn't require use of the Update method, that should be handled by other, 
/// smaller units.
/// </summary>
public class GameManager : MonoBehaviour
{
	// Public fields, accessible in Unity Editor
	/// <summary>
	/// The prefab for the GameOverText object
	/// </summary>
	public GameObject goTextPF;

	// Private fields
	private Canvas mainCanvas;

	// Use this for initialization
	void Start ()
	{
		// First, setup win/loss conditions
		MainBase[] bases = FindObjectsOfType<MainBase> ();	

		foreach (MainBase tempBase in bases)
		{
			// Subscribe to both bases CombatEvents
			tempBase.CombatEvent += BaseDefeated;
		}

		// Save a reference to the main screen canvas
		mainCanvas = FindObjectOfType<Canvas>();
	}
	
	/* Should not be needed...
	void Update ()
	{
	
	}
	*/

	// Event Methods
	/// <summary>
	/// Fired when a base is defeated.
	/// </summary>
	/// <param name="sender">This is the base that was defeated</param>
	/// <param name="args">Arguments.</param>
	public void BaseDefeated(object sender, CombatEventArgs args)
	{
		// We only care about death events
		if (args.Message != CombatEventArgs.CombatMsg.IsDefeated)
		{
			// Ignore
			return;
		}

		// The game is over, stop the game
		Time.timeScale = 0.0f;

		// So, who lost?
		string loserText = (sender as BaseFighter).tag;

		// Make the string to add to GameOverText
		loserText = "\n" + loserText + " has been defeated!";

		// Instantiate our GameOverText object
		GameObject goText = Instantiate(goTextPF, mainCanvas.transform) as GameObject;
		Text goTextComp = goText.GetComponent<Text> ();

		// Move it to the center of the screen
		goTextComp.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);

		// Add the loser text to the end and we're golden
		goTextComp.text += loserText;
	}
}

