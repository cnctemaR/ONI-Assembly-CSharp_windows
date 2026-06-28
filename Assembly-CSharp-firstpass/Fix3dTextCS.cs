using System;
using ArabicSupport;
using UnityEngine;

public class Fix3dTextCS : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.GetComponent<TextMesh>().text = ArabicFixer.Fix(this.text, this.tashkeel, this.hinduNumbers);
	}

	private void Update()
	{
	}

	public string text;

	public bool tashkeel = true;

	public bool hinduNumbers = true;
}
