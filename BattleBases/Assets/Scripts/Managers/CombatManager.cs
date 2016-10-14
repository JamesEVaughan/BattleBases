using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// This handles combat interactions
/// </summary>
public class CombatManager : MonoBehaviour
{
	// Fields
	/// <summary>
	/// A list of the current battles taking place.
	/// </summary>
	private List<Battle> curFights;

	void Start ()
	{
		// Initiatize our list
		curFights = new List<Battle>(10);

		// Subscribe to Outpost events
		OutpostSpawner[] spawners = Object.FindObjectsOfType<OutpostSpawner>();
		OutpostCombat tempCombat;

		// And listen to their spawn events
		foreach (OutpostSpawner temp in spawners)
		{
			temp.SpawnEvent += UnitSpawned;

			tempCombat = temp.GetComponent<OutpostCombat> ();

			// Each one should have one, so something bad happened if they don't
			if (tempCombat != null)
			{
				// Subscribe to their combat events
				tempCombat.CombatEvent += CombatStart;
			}
		}


	}

	// we keep time in Update
	void Update()
	{
		// Only do stuff if will have active battles
		if (curFights.Count == 0)
		{
			return;
		}

		// Add the time to each battle
		foreach (Battle temp in curFights)
		{
			temp.AddTime (Time.deltaTime);
		}
	}

	// Methods

	// Events
	/// <summary>
	/// Listens for spawn events
	/// </summary>
	public void UnitSpawned(object sender, SpawnEventArgs spawnArgs)
	{
		// We only care about GameObjects with BaseFighter components
		BaseFighter newBF = spawnArgs.UnitSpawned.GetComponent<BaseFighter>();
		if (newBF == null) 
		{
			// It doesn't have a BaseFighter, ignore
			return;
		}

		// Subscribe to it's CombatEvents
		newBF.CombatEvent += CombatStart;
	}

	/// <summary>
	/// Listens for CombatEvents
	/// </summary>
	/// <param name="sender">The object sending this argument</param>
	/// <param name="combatArgs">The event arguments</param>
	public void CombatStart(object sender, CombatEventArgs combatArgs)
	{
		// We don't care about Death events
		if (combatArgs == CombatEventArgs.Death)
		{
			return;
		}

		BaseFighter tempBF = sender as BaseFighter;
		if (tempBF == null) 
		{
			// Ignore
			return;
		}

		// It is! Add this to our battle list
		NewBattle(tempBF, combatArgs.Opponent);
	}

	/// <summary>
	/// Listens for battle completion events
	/// </summary>
	/// <param name="sender">The battle that finished</param>
	/// <param name="args">Arguments.</param>
	public void BattleCompleted(object sender, CombatEventArgs args)
	{
		// We only care about BattleComplete events
		if (args.Message != CombatEventArgs.CombatMsg.BattleComplete)
		{
			return;
		}

		Battle comp = sender as Battle;
		// Who won?
		BaseFighter victor = (args.Opponent == comp.Fighter1) ? comp.Fighter2 : comp.Fighter1;

		// Remove the Battle from curFights
		curFights.Remove(comp);

		// Are they involved in any paused battles?
		foreach (Battle temp in curFights.FindAll(x => x.Contains(victor)))
		{
			// Restart the battle
		}
	}


	// Helper Methods

	/// <summary>
	/// Adds a new Battle to the list
	/// </summary>
	/// <param name="BF1">One fighter in this Battle</param>
	/// <param name="BF2">The other fighter in this Battle</param>
	private void NewBattle(BaseFighter fight1, BaseFighter fight2)
	{
		// First, are either of this fighters currently fighting in another Battle?
		List<Battle> tempList = curFights.FindAll(x => x.ContainsCanfight(fight1) || x.ContainsCanfight(fight2));

		// Did we even find anything?
		if (tempList.Count > 0)
		{
			// See if any of those fights are interruptable
			foreach (Battle tempBatt in tempList)
			{
				// Pause the battles that can be
				if (CanBattleBePaused (tempBatt))
				{
					tempBatt.IsPaused = true;
				}

				// Sanity check, are this 2 fighters already fighting?
				if (tempBatt.Contains (fight1) && tempBatt.Contains (fight2))
				{
					// NOTE: If we add ranged units, we'll need a check to start a melee fight here

					// We don't need duplicates
					return;
				}
			}
		}

		Battle newBatt;
		// If either of these fighters are an Outpost, make the battle using the 
		//    one attacker variant
		if (fight1 is OutpostCombat)
		{
			newBatt = Battle.MakeBattleOneAttacker(fight2, fight1);
		}
		else if (fight2 is OutpostCombat)
		{
			newBatt = Battle.MakeBattleOneAttacker (fight1, fight2);
		}

		// There was no outpost, use the fighters variant
		else
		{
			newBatt = Battle.MakeBattleWithFighters (fight1, fight2);
		}

		// Add the new battle to curFights and subscribe to its event
		curFights.Add(newBatt);
		newBatt.BattleEvent += BattleCompleted;

	}

	/// <summary>
	/// Tests to see if this battle can paused.
	/// </summary>
	/// <returns><c>true</c>, if this battle can be paused, <c>false</c> otherwise.</returns>
	/// <param name="someBatt">The battle to be tested</param>
	private bool CanBattleBePaused(Battle someBatt)
	{
		// This should be an additive method. Add cases for battles that can be interrupted
		//     as they come up.

		// Outpost battles can be paused
		if (someBatt.Fighter1 is OutpostCombat || someBatt.Fighter2 is OutpostCombat)
		{
			return true;
		}

		// If we didn't catch it, it's good
		else
		{
			return false;
		}

	}
}

