using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
	// Accessible in Unity
	/// <summary>
	/// Time, in seconds, that the damage text should appear on screen
	/// </summary>
	public float screenTime = 0.75f;

	// Properties
	public int Damage
	{
		get
		{
			return damage;
		}

		set
		{
			if (value != damage)
			{
				// Change damage and update the text
				damage = value;
				OnDamageChange ();
			}
		}
	}

	// Fields
	/// <summary>
	/// Field behind for the Damage property
	/// </summary>
	private int damage;
	/// <summary>
	/// Reference to the Text child of our GameObject
	/// </summary>
	private Text damText;
	/// <summary>
	/// How long we've been displayed on screen
	/// </summary>
	private float timer;
	/// <summary>
	/// Where the text starts at
	/// </summary>
	private Vector2 startPoint;
	/// <summary>
	/// Where the text will stop at
	/// </summary>
	private Vector2 endPoint;
	/// <summary>
	/// True once the text has been displayed for screenTime
	/// </summary>
	private bool isDone;

	// Unity methods
	void Awake()
	{
		// Grab the child text component
		damText = gameObject.GetComponentInChildren<Text>();

		// Clear out any text it currently has with nothing
		damText.text = "";

		// Flag timer to negative to ignore it for now
		timer = -1f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Don't do anything if timer is negative
		if (timer < 0f)
		{
			return;
		}

		timer += Time.deltaTime;
		if (timer > screenTime)
		{
			timer = screenTime;
		}
		// Movement is handled by lerping the anchor
		damText.rectTransform.anchoredPosition = Vector2.Lerp (startPoint, endPoint, timer / screenTime);

		if (timer == screenTime)
		{
			Destroy (gameObject);
		}
	}

	// Methods
	/// <summary>
	/// Moves the text to the right place
	/// </summary>
	/// <param name="target">Target.</param>
	public void ChangePosition (GameObject target)
	{
		// Get the screen position of the target
		Camera cam = FindObjectOfType<Camera>();
		Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, target.transform.position);

		// Get the  two points for our lerp action
		startPoint = screenPos - damText.rectTransform.sizeDelta/2;
		endPoint = startPoint + (Vector2.up * 10);

		damText.rectTransform.anchoredPosition = startPoint;

		// Reset our timer
		timer = 0f;
	}

	// Helper methods
	/// <summary>
	/// Updates our Text object with the correct value for
	/// </summary>
	private void OnDamageChange()
	{
		damText.text = "-" + damage.ToString();
	}
}

