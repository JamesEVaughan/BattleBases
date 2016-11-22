using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// TeamSpawner spawns handles requests to spawn in new units for this team.
/// </summary>
public class TeamSpawner : MonoBehaviour
{
	// Fields
	/// <summary>
	/// Handy reference to this teams treasury. Does not need to be explicitly 
	/// assigned.
	/// </summary>
	private Treasury ourTreasure = null;

	/// <summary>
	/// Handy reference all of this team's outposts, specifically their spawner
	/// component. Does not need to be explicitly assigned.
	/// </summary>
	private List<OutpostSpawner> ourOutposts = null;

	// Use this for initialization
	void Start ()
	{
		// Let's find our stuff!
		// NOTE: All this components should be attached to the central TeamGameObject
		ourTreasure = GetComponent<Treasury>();

		// Getting the outposts will be more challenging
		// Start with a low number so we don't overwork things
		ourOutposts = new List<OutpostSpawner>(5);

		OutpostSpawner[] allSpawners = FindObjectsOfType<OutpostSpawner>();
		foreach (OutpostSpawner tempSpawn in allSpawners)
		{
			// Gate this by matching our tag
			if (tempSpawn.tag == tag)
			{
				// Add the spawner to our list
				ourOutposts.Add (tempSpawn);

				// Listen to its CombatEvents
				(tempSpawn.GetComponent<OutpostCombat> ()).CombatEvent += SpawnerDeath;
			}
		}
	}

	// Events
	/// <summary>
	/// Listens for Death events from our Outposts
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="args">Arguments.</param>
	public void SpawnerDeath(object sender, CombatEventArgs args)
	{
		// Sanity check! Is this an OutpostCombat? Is destoryed?
		if (!(sender is OutpostCombat) || sender == null)
		{
			// Make sure this never happens again...
			(sender as OutpostCombat).CombatEvent -= SpawnerDeath;
			return;
		}
		// We only care about death events
		if (args.Message != CombatEventArgs.CombatMsg.IsDefeated)
		{
			// Ignore
			return;
		}

		OutpostCombat senderOC = sender as OutpostCombat;
		OutpostSpawner senderOS = senderOC.GetComponent<OutpostSpawner>();

		// Remove it from our list
		ourOutposts.Remove(senderOS);

		// Unsubscribe from its event
		senderOC.CombatEvent -= SpawnerDeath;
	}

	// Methods
	/// <summary>
	/// Spawns the specified unit.
	/// </summary>
	/// <param name="spawnHere">Where the new unit is to be spawned.</param>
	/// <param name="spawnUnit">The unit to be spawned.</param>
	public void SpawnUnit(OutpostSpawner spawnHere, UnitInfo spawnUnit)
	{
		// Sanity checks!
		// Is it on our team? Has it been destroyed?
		if (spawnHere.tag != tag || spawnHere == null)
		{
			// Ignore
			return;
		}

		// Ignore it if we can't afford this unit
		if (ourTreasure.Funds < spawnUnit.unitCost)
		{
			return;
		}

		// Now, attempt to spawn the unit
		if (spawnHere.SpawnUnit (spawnUnit.unitPreFab))
		{
			// Spawn was successful! Deduct it from the treasury
			ourTreasure.Funds -= spawnUnit.unitCost;
		}
	}
}

