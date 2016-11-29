using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// BaseRadialMenuController is a base class for controlling the behaviour of a UI
/// radial menu. A radial menu, for our purposes, is a simple menu for selecting a
/// single option by gesturing the mouse in a direction. The general use-case is 
/// for when a menu has multiple options and quick input by the user is desired.
/// </summary>
public class RadialBuyMenu : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	// Accessible in Unity
	/// <summary>
	/// A ratio between 0.0 and 1.0 that represents the rate at which the game should 
	/// be slowed down while the menu is open. 0 is full stop and 1 is normal speed.
	/// </summary>
	public float slowdownFactor = 0.25f;

	/// <summary>
	/// How big of a gesture does the user need to make before we register it
	/// </summary>
	public float gestureMagnitude = 5.0f;

	/// <summary>
	/// The buttons available to this radial menu.
	/// </summary>
	public List<RadialBuyMenuButton> theButtons;

	// Base fields
	/// <summary>
	/// What the time scale was set to before the menu was opened
	/// </summary>
	protected float oldTimeScale;

	/// <summary>
	/// The current gesture
	/// </summary>
	protected Vector2 curGesture;

	/// <summary>
	/// A unit vector. The current direction of the selector.
	/// </summary>
	protected Vector2 curSelection;

	// Unity methods
	void Start()
	{
		// Start mouseDirection at (0,0)
		curGesture = new Vector2(0.0f, 0.0f);

		// Start the selection at (0,0)
		curSelection = new Vector2(0.0f, 0.0f);
	}

	void Update()
	{
		
	}



	// Helper methods
	/// <summary>
	/// Updates the selection based on 
	/// </summary>
	/// <param name="mouseDelta">Mouse delta.</param>
	protected void UpdateSelection()
	{
		// Find the new direction by using the normalized curGesture
		Vector2 newSelection = curGesture.normalized + curSelection;
		newSelection.Normalize ();

		// This is the dividing line between the four quadrants
		float cos45 = Mathf.Cos(Mathf.PI/4);

		// Check to see where we're pointing at now

		if (newSelection.x > cos45)
		{
			// This means we're pointing right
			theButtons[1].Select();
		}
		else if (newSelection.x < -cos45)
		{
			// This means we're pointing left
			theButtons [3].Select();
		}
		else if (newSelection.y > cos45)
		{
			// This means we're pointing up
			theButtons [0].Select();
		}
		else
		{
			// This means we're pointing down
			theButtons [2].Select();
		}
	}

	// Implementation of IDrag*
	public void OnBeginDrag(PointerEventData eventData)
	{
		// Slowdown the game speed
		oldTimeScale = Time.timeScale;
		Time.timeScale = slowdownFactor;
	}

	public void OnDrag(PointerEventData eventData)
	{
		// Add the delta to curGesture
		curGesture += eventData.delta;

		// Is the gesture big enough to make a change?
		if (curGesture.magnitude > gestureMagnitude)
		{
			// Update the selection
			UpdateSelection();
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		// Revert back to the old timeScale
		Time.timeScale = oldTimeScale;

		// Test: Spit out the name of the currently selected gameObject
		Debug.Log("You selected: " + EventSystem.current.currentSelectedGameObject.name);
	}
}

