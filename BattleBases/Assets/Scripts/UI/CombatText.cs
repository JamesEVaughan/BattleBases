using UnityEngine;
using System.Collections;

using UnityEngine.UI;

/// <summary>
/// Displays combat related text on the UI
/// </summary>
public class CombatText : MonoBehaviour
{
	// Accessible in Unity
	/// <summary>
	/// Flag, will only display the health of this unit if this is true
	/// </summary>
	public bool DisplayHealth = true;
	/// <summary>
	/// Reference to the prefab for the HealthText prefab
	/// </summary>
	//public GameObject HTPreFab;
	/// <summary>
	/// Reference to the DamageText prefab
	/// </summary>
	public GameObject DTPreFab;
	/// <summary>
	/// Reference to the main Canvas
	/// </summary>
	public GameObject GUICanvas;


	// Properties
	/// <summary>
	/// This unit's current health
	/// </summary>
	public int CurHealth { get; private set; }

	// Fields
	/// <summary>
	/// A handy reference to the BaseFighter script we're attached to
	/// </summary>
	private BaseFighter ourBF;
	/// <summary>
	/// Last known position of our GameObject
	/// </summary>
	private Vector3 curPosition;
	/// <summary>
	/// The Text object displaying this unit's health
	/// </summary>
	private Text healthText = null;

	// Unity methods
	void Start()
	{
		// Grab the BaseFighter for the attached GameObject
		ourBF = gameObject.GetComponent<BaseFighter>();

		if (ourBF == null) 
		{
			// If this unit isn't involved in combat, we have nothing to do
			// Remove the script
			Destroy(this);
			return;
		}

		// Grab the unit's current position
		curPosition = gameObject.transform.position;
			
		// Are we displaying the health of this unit?
		if (DisplayHealth) 
		{
			CurHealth = ourBF.Health;
			// TO DO: Add HealthText functionality
		}
	}

	void Update()
	{
		// Sanity check: if we get here before we're supposed to be deleted
		if (ourBF == null) 
		{
			// Skip
			return;
		}

		// See if CurHealth needs updated
		if (DisplayHealth && CurHealth != ourBF.Health)
		{
			CurHealth = ourBF.Health;
		}


	}

	// Message methods
	void OnAttacked(object attacker)
	{
		// Sanity check: Attacker should be a GameObject
		if (!(attacker is GameObject))
		{
			return;
		}

		BaseFighter attackerBF = (attacker as GameObject).GetComponent<BaseFighter> ();
		// We only care about BaseFighters attacking
		if (attackerBF == null)
		{
			return;
		}

		// Grab the damage value
		int attack = attackerBF.AttackStr;

		// Instantiate a new DamageText with attack
		GameObject newDamText = Instantiate(DTPreFab) as GameObject;
		Canvas can = FindObjectOfType<Canvas> ();
		newDamText.transform.SetParent (can.transform, false);
		DamageText newText = newDamText.GetComponent<DamageText> ();

		newText.Damage = attack; // Set Damage
		newText.ChangePosition(gameObject);
	}
}