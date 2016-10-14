using UnityEngine;
using System.Collections;

/// <summary>
/// This manager class is the clearinghouse for all user input while the game is 
/// active.
/// </summary>
public class InputManager : MonoBehaviour
{
	// Fields
	/// <summary>
	/// The main camera for this scene
	/// </summary>
	private Camera mainCam;

	// Use this for initialization
	void Start ()
	{
		// Grab the Camera for the scene, the first one is the one we want
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	// This is the workhorse for Input events
	void Update ()
	{
		// During the update loop, check for events we want to react to

		// Did the user click on something?
		if (Input.GetMouseButtonDown (0))
		{
			// Pass the mouse position to OnMouseClick
			MouseClick(Input.mousePosition);
		}
	}

	// Helper methods
	/// <summary>
	/// Handles MouseClick events
	/// </summary>
	/// <param name="mousePos">The screen position of the mouse cursor</param>
	private void MouseClick(Vector3 mousePos)
	{
		// First, make sure that mousePos is inside the game screen, specifically
		// inside the rectangle (0, 0) - (Screen.Width, Screen.Height)
		bool isXValid = mousePos.x > 0 && mousePos.x < Screen.width;
		bool isYValid = mousePos.y > 0 && mousePos.y < Screen.width;

		if (!isXValid || !isYValid)
		{
			// The mouse is outside the screen, ignore
			return;
		}

		// The mouse is inside the screen, do a RayTrace to see if we hit anything
		RaycastHit tempHit;
		if (Physics.Raycast (mainCam.ScreenPointToRay (mousePos), out tempHit, Mathf.Infinity, 
			LayerMask.GetMask("MouseTarget")))
		{
			// Let's see what we hit
			// RayCastTarget gameobjects are always children of the real target
			GameObject hitParent = tempHit.transform.parent.gameObject;

			// Now, find out if it has a component we work with
			IClickable tempClick = hitParent.GetComponent<IClickable>();

			// If there isn't an IClickable MonoBehaviour attached
			if (tempClick == null)
			{
				// Ignore
				return;
			}

			// Otherwise, do the click operation
			tempClick.Clicked();
		}


	}
}

