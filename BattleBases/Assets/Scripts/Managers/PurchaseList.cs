using UnityEngine;
using System.Collections;

using System.Collections.Generic;

/// <summary>
/// PurchaseList is a listing of all units available for purchase. It's a simple container
/// MonBehaviour for use by the other components of the TeamGameObject that's editable
/// in the Unity Editor.
/// </summary>
public class PurchaseList : MonoBehaviour
{
	// Accessible in Unity Editor
	public List<UnitInfo> availableUnits;
}

