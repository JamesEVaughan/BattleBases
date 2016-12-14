using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

/// <summary>
/// RadialBuyMenu controls interactions from the mouse to select the correct option 
/// from the menu. The menu has four buttons, with an attached RadialBuyMenuButton component,
/// that are child GameObjects of the main UI object this is attached to.
/// </summary>
public class RadialBuyMenu : UIBehaviour
{
	// Static fields
	/// <summary>
	/// The names of the child buttons. The GameObject MUST follow this convention
	/// </summary>
	private string[] childNames = 
	{
		"TopButton",
		"RightButton",
		"BottomButton",
		"LeftButton"
	};

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

	// Properties
	public OutpostSpawner SpawnThere { get; set;}


	// Fields
	/// <summary>
	/// What the time scale was set to before the menu was opened
	/// </summary>
	private float oldTimeScale;

	/// <summary>
	/// Old mouse position
	/// </summary>
	private Vector2 oldMousePos;

	/// <summary>
	/// The current gesture made
	/// </summary>
	private Vector2 curGesture;

	/// <summary>
	/// A unit vector. The current direction of the selector.
	/// </summary>
	private Vector2 curSelection;

	/// <summary>
	/// Our team's PurchaseList
	/// </summary>
	private PurchaseList teamBuyList;

	/// <summary>
	/// Our team's spawner
	/// </summary>
	private TeamSpawner teamTeamSpawnerSpawner;

	// The Buttons
	/// <summary>
	/// The RadialBuyMenuButton attached to the top button of the menu.
	/// </summary>
	private RadialBuyMenuButton topButton;

	/// <summary>
	/// The RadialBuyMenuButton attached to the right button of the menu.
	/// </summary>
	private RadialBuyMenuButton rightButton;

	/// <summary>
	/// The RadialBuyMenuButton attached to the bottom button of the menu.
	/// </summary>
	private RadialBuyMenuButton bottomButton;

	/// <summary>
	/// The RadialBuyMenuButton attached to the left button of the menu.
	/// </summary>
	private RadialBuyMenuButton leftButton;

	// Unity methods
	void Start()
	{
		// Start mouseDirection at (0,0)
		curGesture = new Vector2(0.0f, 0.0f);

		// Start the selection at (0,0)
		curSelection = new Vector2(0.0f, 0.0f);

		// Grab the team object components
		teamBuyList = FindObjectOfType<PurchaseList>();
		teamTeamSpawnerSpawner = FindObjectOfType<TeamSpawner> ();

		// Find the radial buttons
		GetChildButtons();

		// Hide the cursor when we open
		Cursor.visible = false;

		// Grab the current position of the mouse
		oldMousePos = Input.mousePosition;

		// Slowdown the game speed
		oldTimeScale = Time.timeScale;
		Time.timeScale = slowdownFactor;
	}

	void Update()
	{
		// Are we still holding down the mouse button?
		if (Input.GetMouseButton (0))
		{
			// What's the change in the mouse position between frames?
			Vector2 mouseDelta = (Vector2)(Input.mousePosition) - oldMousePos;
			oldMousePos = Input.mousePosition;

			// Add the change
			curGesture += mouseDelta;

			// Is the gesture big enough to make a change?
			if (curGesture.magnitude > gestureMagnitude)
			{
				// Update the selection
				UpdateSelection ();
			}

			return;
		}
		// We're not holding down a button, close up shop
		// Revert back to the old timeScale
		Time.timeScale = oldTimeScale;

		// Make the cursor visible
		Cursor.visible = true;

		// Tell the spawner what unit we're trying to spawn
		RadialBuyMenuButton selected = EventSystem.current.currentSelectedGameObject.GetComponent<RadialBuyMenuButton> ();
		// Make sure our selection is valid
		if (selected != null)
		{
			UnitInfo spawnThis = selected.getUnit();
			teamTeamSpawnerSpawner.SpawnUnit (SpawnThere, spawnThis);
		}


		// Finally, destroy the object if we're done
		Destroy(gameObject);
	}

	// Methods
	/// <summary>
	/// Call immediately after an Instantiate call!
	/// Sets the position for the buy menu.
	/// </summary>
	/// <param name="pos">The center position, in screen space.</param>
	public void SetPosition(Vector2 pos, Canvas theCanvas = null)
	{
		// First, grab a canvas if we don't have one
		if (theCanvas == null)
		{
			theCanvas = FindObjectOfType<Canvas> ();
		}

		// First things first, make it a child of theCanvas
		transform.SetParent(theCanvas.transform);

		// Grab the RectTransform of the canvas
		RectTransform canRect = theCanvas.GetComponent<RectTransform> ();
		RectTransform menuRect = GetComponent<RectTransform> ();

		// Sanity check: Make sure we found those RectTransforms
		if (canRect == null || menuRect == null)
		{
			// Tell the logger and do nothing
			Debug.Log("Could not find Canvas or MenuRect.");
			return;
		}


		// Make sure that the menu will be fully visible on the screen
		// The brCorner is the easiest one to test for not being on screen
		Vector2 brCorner = pos - canRect.sizeDelta/2;
		brCorner.x += menuRect.sizeDelta.x/2;
		brCorner.y -= menuRect.sizeDelta.y/2;
		if (!canRect.rect.Contains (brCorner))
		{
			// Test complete! Menu now doesn't go beyond the screen
			// Test: Make sure that the menu is outside the screen
			/*
			Debug.Log ("Menu is outside of the screen.\n"
						+ "brCorner corner:  " + (brCorner) + "\n"
						+ "canRect postion: " + canRect.rect.position + "\n"
						+ "canRect max: " + canRect.rect.yMax + "\n"
						+ "canRect min: " + canRect.rect.min + "\n"
						+ "Old pos.y: " + pos.y);
			*/
			// The bottom-right corner is not on screen, adjust it
			pos.y += canRect.rect.yMin - brCorner.y;

			//Debug.Log ("New pos.y: " + pos.y);
		}

		menuRect.anchoredPosition = pos - canRect.sizeDelta / 2;

	}

	// Helper methods
	/// <summary>
	/// Updates the selection based on 
	/// </summary>
	/// <param name="mouseDelta">Mouse delta.</param>
	private void UpdateSelection()
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
			rightButton.Select();
		}
		else if (newSelection.x < -cos45)
		{
			// This means we're pointing left
			leftButton.Select();
		}
		else if (newSelection.y > cos45)
		{
			// This means we're pointing up
			topButton.Select();
		}
		else
		{
			// This means we're pointing down
			bottomButton.Select();
		}
	}

	/// <summary>
	/// Finds all the RadialBuyMenuButtons attached to children and stores
	/// the reference in the approriate slot.
	/// </summary>
	private void GetChildButtons()
	{
		GameObject tempChild;


		// Loop through childNames for each name
		for (int i = 0; i < childNames.Length; i++)
		{
			if (transform.Find(childNames[i]) == null)
			{
				// Ignore this one
				continue;
			}
			tempChild = (transform.Find (childNames [i])).gameObject;

			if (tempChild != null)
			{
				// This child exists! Set the correct *Button
				RadialBuyMenuButton tempButt = tempChild.GetComponent<RadialBuyMenuButton>();
				// Sanity check
				if (tempButt == null)
				{
					// ignore
					continue;
				}
				switch (i)
				{
				case 0:
					topButton = tempButt;
					break;

				case 1:
					rightButton = tempButt;
					break;

				case 2:
					bottomButton = tempButt;
					break;

				case 3:
					leftButton = tempButt;
					break;
				}

				// Now assign the UnitInfo's
				tempButt.AssignUnit (teamBuyList [i]);

			}
		}
	}
}

