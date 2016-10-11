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

	// Fields
	/// <summary>
	/// Where a new unit is spawned
	/// </summary>
	private GameObject spawnPoint;

	// Events

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
	/// Spawns a new unit, based on the assigned prefab
	/// </summary>
	/// <returns>A reference to a newly spawned unit</returns>
	public GameObject SpawnUnit()
	{
		// First, make sure that the spawn area is clear of any units
		if (!IsSpawnAreaClear ())
		{
			// Don't spawn anything
			return null;
		}

		// Spawn in the new unit
		GameObject spawnedUnit = (Instantiate (UnitPrefab, spawnPoint.transform.position, 
			                         spawnPoint.transform.rotation) as GameObject);



		// Finally, return a reference of the newly spawned unit
		return spawnedUnit;
	}

	// Helper methods
	/// <summary>
	/// Checks whether the spawn area of the outpost is clear
	/// </summary>
	/// <returns><c>true</c> if the spawn area is clear; otherwise, <c>false</c>.</returns>
	private bool IsSpawnAreaClear()
	{
		// First, calculate our reference point from our spawn point
		Vector3 refPoint = spawnPoint.transform.position;

		// Raise it 0.25 units to get it off the ground and push it back by 0.5 units
		// We push it back so that the rayCast will hit the colliders of recently spawned units
		refPoint += (Vector3.up * 0.25f) + (spawnPoint.transform.forward * -0.5f);

		// Do a raycast and record all the hits we're out of the outpost
		RaycastHit[] theHits = Physics.RaycastAll (refPoint, spawnPoint.transform.forward, 
			1f + spawnBufferDist, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

		// Cycle through and see if anything we hit prevents a spawn
		foreach (RaycastHit tempHit in theHits)
		{
			// If we hit a rigidBody inside the outpost
			if (tempHit.rigidbody != null && tempHit.distance < 1.0f)
			{
				// Spawn area is not clear
				return false;
			}
		}

		// If we didn't find anything, we're good to go!
		return true;
	}

	// IClickable implementation
	public void Clicked()
	{
		// If we've been clicked on, we should spawn a unit
		GameObject tempUnit = SpawnUnit();

		// And pass it along to someone
	}
}

