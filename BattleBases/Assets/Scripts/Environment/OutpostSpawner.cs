using UnityEngine;
using System.Collections;


/// <summary>
/// An OutpostSpawner script handles the spawning of new units from the attached
/// GameObject. An OutpostSpawner requires the attached GameObject to have a child
/// GameObject tagged with "SpawnPoint." Script will destory itself if one is not
/// found.
/// </summary>
public class OutpostSpawner : MonoBehaviour, IClickable
{
	// Accessible in Unity
	/// <summary>
	/// The unit this outpost spawns
	/// </summary>
	public GameObject UnitPrefab;
	/// <summary>
	/// How many units away from the outside edge of the outpost do we need to check
	/// </summary>
	public float spawnBufferDist = 0.25f;

	/// <summary>
	/// A reference to our team's spawner.
	/// </summary>
	public TeamSpawner ourSpawner;

	// Fields
	/// <summary>
	/// Where a new unit is spawned
	/// </summary>
	private GameObject spawnPoint;

	// Events
	public event SpawnEventHandler SpawnEvent;

	// Unity methods
	// Use this for initialization
	void Start ()
	{
		// First, find our spawnpoint
		foreach (Transform child in transform)
		{
			if (child.tag == "SpawnPoint")
			{
				spawnPoint = child.gameObject;

				// There should only be one spawn point
				break;
			}
		}

		// This script can't function without:
		// A valid spawnPoint
		// A valid UnitPrefab, must be able to walk and fight
		if (spawnPoint == null ||
			UnitPrefab.GetComponent<UnitMovement>() == null ||
			UnitPrefab.GetComponent<BaseFighter>() == null)
		{
			// We can't work, delete
			Destroy(this);
			return;
		}
	}

	// Methods
	/// <summary>
	/// Spawns a new unit, based on the assigned prefab.
	/// Outdated! Use SpawnUnit(GameObject) for future versions!
	/// </summary>
	public void SpawnUnit()
	{
		// First, make sure that the spawn area is clear of any units
		if (!IsSpawnAreaClear ())
		{
			// Don't spawn anything
			return;
		}

		// Spawn in the new unit
		GameObject spawnedUnit = (Instantiate (UnitPrefab, spawnPoint.transform.position, 
			                         spawnPoint.transform.rotation) as GameObject);

		// If we've successfully spawned a unit, raise the event
		if (spawnedUnit != null)
		{
			OnSpawn (spawnedUnit);
		}
	}

	/// <summary>
	/// Spawn the specified unit, if possible
	/// </summary>
	/// <returns><c>true</c>, if the unit was spawned, <c>false</c> if not.</returns>
	/// <param name="unit">The unit to be spawned</param>
	public bool SpawnUnit(GameObject unit)
	{
		// Sanity check!
		if (unit.tag != tag)
		{
			// If this unit isn't on our team, don't spawn it
			return false;
		}

		// Can we spawn a unit?
		if (!IsSpawnAreaClear ())
		{
			// We can't do anything
			return false;
		}

		// Everything's clear, spawn in the unit
		GameObject newUnit = Instantiate (unit, spawnPoint.transform.position, 
			                     spawnPoint.transform.rotation) as GameObject;

		// Make sure this is a real unit
		if (newUnit == null)
		{
			// Welp, didn't work
			Debug.Log("Unit did not instantiate properly during SpawnUnit().");
			return false;
		}

		// Pass on the spawn event
		OnSpawn(newUnit);

		return true;
	}

	// Helper methods
	/// <summary>
	/// Checks whether the spawn area of the outpost is clear
	/// </summary>
	/// <returns><c>true</c> if the spawn area is clear; otherwise, <c>false</c>.</returns>
	private bool IsSpawnAreaClear()
	{
		// First, calculate our reference point from our spawn point and raise it
		//    0.5 units up to get it off of the ground
		Vector3 refPoint = spawnPoint.transform.position + (Vector3.up * 0.5f);

		// Second, get the half extents of the spawn area
		Vector3 refExtents = new Vector3(0.5f, 0.5f, 1.0f);

		// Do a BoxCast to see if anything is in there
		bool foundSomething = Physics.CheckBox(refPoint, refExtents, Quaternion.identity, 
			LayerMask.GetMask("UnitTarget"));


		// If we didn't find anything, the area is clear
		return !foundSomething;
	}

	/// <summary>
	/// Raises the spawn event.
	/// </summary>
	private void OnSpawn(GameObject newUnit)
	{
		SpawnEventHandler hand = SpawnEvent;
		if (hand != null)
		{
			hand (this, new SpawnEventArgs (newUnit));
		}
	}

	// IClickable implementation
	public void Clicked()
	{
		// If we've been clicked on, we should spawn a unit
		SpawnUnit();
	}
}

