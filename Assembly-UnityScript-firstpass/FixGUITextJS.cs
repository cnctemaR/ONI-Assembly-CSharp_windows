using System;
using ArabicSupport;
using UnityEngine;

[Serializable]
public class FixGUITextJS : MonoBehaviour
{
	public FixGUITextJS()
	{
		this.text = string.Empty;
		this.tashkeel = true;
		this.hinduNumbers = true;
	}

	public virtual void Start()
	{
		this.gameObject.GetComponent<GUIText>().text = ArabicFixer.Fix(this.text, this.tashkeel, this.hinduNumbers);
	}

	public virtual void Update()
	{
	}

	public virtual void Main()
	{
	}

	public string text;

	public bool tashkeel;

	public bool hinduNumbers;
}
