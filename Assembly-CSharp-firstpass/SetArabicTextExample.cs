using System;
using ArabicSupport;
using UnityEngine;

public class SetArabicTextExample : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.GetComponent<GUIText>().text = "This sentence (wrong display):\n" + this.text + "\n\nWill appear correctly as:\n" + ArabicFixer.Fix(this.text, false, false);
	}

	private void Update()
	{
	}

	public string text;
}
