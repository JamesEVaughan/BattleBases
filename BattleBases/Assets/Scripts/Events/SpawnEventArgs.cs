using System;
using UnityEngine;


/// <summary>
/// Event arguments for spawn events
/// </summary>
public class SpawnEventArgs : EventArgs
{
	// Properties
	/// <summary>
	/// This is the GameObject that was spawned
	/// </summary>
	public GameObject UnitSpawned { get; private set; }

	// Constructors
	public SpawnEventArgs(GameObject unit)
	{
		UnitSpawned = unit;
	}
}

// Event delegate
/// <summary>
/// The SpawnEventHandler signature
/// </summary>
public delegate void SpawnEventHandler(object sender, SpawnEventArgs spawnArgs);