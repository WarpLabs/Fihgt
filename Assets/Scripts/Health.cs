using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Health : MonoBehaviour {

	public float StartHealth = 100f;
	public Image FillImage;
	public Slider Slider;

	public Color FullHealthColor = Color.green;
	public Color ZeroHealthColor = Color.red;

	public bool UseColor;

	private float CurrentHealth;

	[HideInInspector] public bool Dead;

	protected virtual void OnEnable () {

		CurrentHealth = StartHealth;
		Slider.maxValue = StartHealth;
		SetHealthUI ();

	}

	public void TakeDamage(float amount)
	{
		CurrentHealth -= amount;
		SetHealthUI ();

		if (CurrentHealth <= 0f && !Dead) 
		{
			OnDeath ();
		}
	}

	private void SetHealthUI()
	{
		// Adjust the value and colour of the slider.
		Slider.value = CurrentHealth;

		if (UseColor)
			FillImage.color = Color.Lerp (ZeroHealthColor, FullHealthColor, CurrentHealth/StartHealth);
	}

	public virtual void OnDeath ()
	{
		Dead = true;
	}

	public virtual void Reset ()
	{
		Dead = false;
		CurrentHealth = StartHealth;
		SetHealthUI ();
	}

}
