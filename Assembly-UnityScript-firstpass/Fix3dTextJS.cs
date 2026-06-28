using System;
using ArabicSupport;
using UnityEngine;

[Serializable]
public class Fix3dTextJS : MonoBehaviour
{
	public Fix3dTextJS()
	{
		this.text = string.Empty;
		this.tashkeel = true;
		this.hinduNumbers = true;
	}

	public virtual void Start()
	{
		((TextMesh)this.gameObject.GetComponent(typeof(TextMesh))).text = ArabicFixer.Fix(this.text, this.tashkeel, this.hinduNumbers);
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
