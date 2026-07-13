using System;
using TMPro;
using UnityEngine;

public class DropdownSample : MonoBehaviour
{
	public void OnButtonClick()
	{
		this.text.text = ((this.dropdownWithPlaceholder.value > -1) ? ("Selected values:\n" + this.dropdownWithoutPlaceholder.value.ToString() + " - " + this.dropdownWithPlaceholder.value.ToString()) : "Error: Please make a selection");
	}

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TMP_Dropdown dropdownWithoutPlaceholder;

	[SerializeField]
	private TMP_Dropdown dropdownWithPlaceholder;
}
