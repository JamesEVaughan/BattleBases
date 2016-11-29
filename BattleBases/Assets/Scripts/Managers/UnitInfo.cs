using UnityEngine;
using System.Collections;

using UnityEditor;

/// <summary>
/// UnitInfo
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "UnitInfoObject", menuName = "DataObject/UnitInfo", order = 1)]
public class UnitInfo : ScriptableObject
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

	// Overrides from Object
	public override bool Equals (object o)
	{
		// We only handle equality for objects of our type
		if (!(o is UnitInfo))
		{
			// Pass it to the base class
			return base.Equals (o);
		}

		// Make a type correct reference
		UnitInfo other = o as UnitInfo;

		// Trivial inequality
		if (this.GetHashCode () != other.GetHashCode ())
		{
			// Different hash codes means not equal
			return false;
		}

		// Test in order of least to most complex, let short circuits work
		return 	this.unitCost == other.unitCost &&
				this.unitName == other.unitName &&
				this.unitPreFab == other.unitPreFab;
	}

	public override int GetHashCode ()
	{
		// Hash the object based on unitCost and the length of unitName
		return 7 * unitName.Length + 13 * unitCost;
	}
}
