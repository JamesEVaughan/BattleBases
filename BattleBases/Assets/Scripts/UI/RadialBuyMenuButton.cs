using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialBuyMenuButton : Selectable
{
	// Accessible in Unity Editor
	/// <summary>
	/// The Text object of this Button's GameObject.
	/// </summary>
	public Text currentText;

	// Fields
	/// <summary>
	/// This is the old text the Text component
	/// </summary>
	protected string oldText;

	// Use this for initialization
	void Start ()
	{
		Init ();
	}

	// Base methods
	public override void OnSelect (BaseEventData eventData)
	{
		base.OnSelect (eventData);

		// Save the old text
		oldText = currentText.text;

		// Change the current text
		currentText.text = "I've been selected!";
	}

	public override void OnDeselect (BaseEventData eventData)
	{
		base.OnDeselect (eventData);


		// Revert the old text
		currentText.text = oldText;
	}

	// Base helper methods
	/// <summary>
	/// Call at the beginning of Start(). Initializes this button's base fields.
	/// </summary>
	protected void Init()
	{
		// Grab our text object
		currentText = GetComponentInChildren<Text>();
	}
}

