using UnityEngine;
using System.Collections;

using System;

/// <summary>
/// This is the Event Argument class for combat related events.
/// </summary>
public class CombatEventArgs : EventArgs
{
	// Properties
	/// <summary>
	/// This is the object that initiated combat with us.
	/// Must be a subclass of BaseFighter
	/// </summary>
	public BaseFighter Opponent { get; private set; }

	// Constructors
	public CombatEventArgs(BaseFighter oppo)
	{
		Opponent = oppo;
	}
}

// Event delegate
/// <summary>
/// The CombatEventHandler signature
/// </summary>
public delegate void CombatEventHandler(object sender, CombatEventArgs combatArgs);