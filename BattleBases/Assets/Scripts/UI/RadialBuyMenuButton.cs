using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RadialBuyMenuButton : Selectable
{
	// Accessible in Unity Editor
	/// <summary>
	/// The Text object of this Button's GameObject.
	/// </summary>
	public Text currentText;

	/// <summary>
	/// The unit to be bought if this is selected
	/// </summary>
	private UnitInfo unitToBuy;

	// Fields
	/// <summary>
	/// This is the old text the Text component
	/// </summary>
	protected string oldText;

	// Unity methods
	void Start ()
	{
		Init ();
	}

	// Methods
	/// <summary>
	/// Assigns the unit to be bought
	/// </summary>
	/// <param name="theUnit">The unit.</param>
	public void AssignUnit(UnitInfo theUnit)
	{
		// Sanity check:
		if (theUnit == null)
		{
			// Ignore
			return;
		}

		// The easy part
		unitToBuy = theUnit;

		// The hard part (not really that hard)
		string nameLine = unitToBuy.unitName;
		string costLine = "Cost: " + unitToBuy.unitCost;

		currentText.text = nameLine + "\n" + costLine;
	}

	public UnitInfo getUnit()
	{
		return unitToBuy;
	}

	// Helper methods
	/// <summary>
	/// Call at the beginning of Start(). Initializes this button's base fields.
	/// </summary>
	private void Init()
	{
		// Grab our text object
		currentText = GetComponentInChildren<Text>();
	}

	// Overrides of Selectable
	/// <summary>
	/// When this object is selected
	/// </summary>
	/// <param name="eventData">The event data for this operation</param>
	public override void OnSelect (BaseEventData eventData)
	{
		base.OnSelect (eventData);

	}

	/// <summary>
	/// When this object is deselected
	/// </summary>
	/// <param name="eventData">The event data for this operation.</param>
	public override void OnDeselect (BaseEventData eventData)
	{
		base.OnDeselect (eventData);

	}
}

