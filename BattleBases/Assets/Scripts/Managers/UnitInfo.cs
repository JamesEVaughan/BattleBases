using UnityEngine;
using System.Collections;

using UnityEditor;

/// <summary>
/// UnitInfo
/// </summary>
[System.Serializable]
public class UnitInfo
{
	// Accessible in UnityEditor
	/// <summary>
	/// The name of the unit.
	/// </summary>
	public string unitName;

	/// <summary>
	/// How much this unit costs
	/// </summary>
	public int unitCost;

	/// <summary>
	/// The unit's prefab
	/// </summary>
	public GameObject unitPreFab;
}
