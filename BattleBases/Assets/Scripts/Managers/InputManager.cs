﻿using UnityEngine;
using System.Collections;

/// <summary>
/// This manager class is the clearinghouse for all user input while the game is 
/// active.
/// </summary>
public class InputManager : MonoBehaviour
{
	// Accessible in UnityEditor
	/// <summary>
	/// This is a reference to the TeamGameObject for the player
	/// </summary>
	public GameObject playersTeamObject;

	/// <summary>
	/// The prefab of the buy menu
	/// </summary>
	public GameObject buyMenuPF;

	// Fields
	/// <summary>
	/// The main camera for this scene
	/// </summary>
	private Camera mainCam;

	/// <summary>
	/// Handy reference to the TeamSpawner for our player
	/// </summary>
	private TeamSpawner playersSpawner;

	/// <summary>
	/// This is a reference to the PurchaseList for the player
	/// </summary>
	private PurchaseList playersAvailableUnits;

	/// <summary>
	/// This is the canvas the buyMenu gets drawn to
	/// </summary>
	private Canvas theCanvas;

	// Use this for initialization
	void Start ()
	{
		// Grab the Camera for the scene, the first one is the one we want
		mainCam = Camera.main;

		// Grab the TeamSpawner component off of the TeamGameObject
		playersSpawner = playersTeamObject.GetComponent<TeamSpawner>();

		// Grab the PurchaseList off of the TeamGameObject
		playersAvailableUnits = playersTeamObject.GetComponent<PurchaseList>();

		theCanvas = FindObjectOfType<Canvas> ();
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

			// New for use with TeamGameObject implementation
			// Find out what we clicked on
			MonoBehaviour tempBehaviour;
			if ((tempBehaviour = hitParent.GetComponent<OutpostSpawner>()) != null)
			{
				// We clicked on an Outpost! 
				// Do we have a valid TeamSpawner?
				if (playersSpawner == null)
				{
					Debug.Log ("No valid TeamSpawner found for the player.");
					return;
				}

				// Ok, now to bring up buy menu
				// Do we have a buy menu?
				if (buyMenuPF == null)
				{
					// Uh oh, tell someone
					Debug.Log ("No buy menu has been assigned.");
					return;
				}

				// Make a new one and put it at the right place
				GameObject newBuyMenu = Instantiate(buyMenuPF) as GameObject;
				RadialBuyMenu newBuyMenuComp = newBuyMenu.GetComponent<RadialBuyMenu> ();
				newBuyMenuComp.SetPosition (mousePos, theCanvas);
				newBuyMenuComp.SpawnThere = tempBehaviour as OutpostSpawner;

				// That's all!
				return;
			}
		}


	}
}

