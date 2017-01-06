using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Mana : MonoBehaviour {

	public float StartMana = 100f;
	public Image FillImage;
	public Slider Slider;

	public Color FullHealthColor = Color.blue;
	public Color ZeroHealthColor = Color.blue;

	public bool UseColor;

	[HideInInspector] public float CurrentMana;

	protected virtual void OnEnable () {

		CurrentMana = StartMana;
		Slider.maxValue = StartMana;
		SetManaUI ();

	}

	public void UseMana(float amount)
	{
		CurrentMana -= amount;

		if (CurrentMana <= 0)
			CurrentMana = 0;
		if (CurrentMana >= StartMana)
			CurrentMana = StartMana;

		SetManaUI ();
	}

	private void SetManaUI()
	{
		// Adjust the value and colour of the slider.
		Slider.value = CurrentMana;

		if (UseColor)
			FillImage.color = Color.Lerp (ZeroHealthColor, FullHealthColor, CurrentMana/StartMana);
	}

	public virtual void Reset ()
	{
		CurrentMana = StartMana;
		SetManaUI ();
	}

}
