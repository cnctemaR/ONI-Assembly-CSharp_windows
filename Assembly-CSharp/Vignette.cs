using System;
using UnityEngine;
using UnityEngine.UI;

public class Vignette : MonoBehaviour
{
	public static void DestroyInstance()
	{
		Vignette.Instance = null;
	}

	private void Awake()
	{
		Vignette.Instance = this;
		this.defaultColor = this.image.color;
	}

	public void SetColor(Color color)
	{
		this.image.color = color;
	}

	public void Reset()
	{
		this.SetColor(this.defaultColor);
	}

	[SerializeField]
	private Image image;

	private Color defaultColor;

	public static Vignette Instance;
}
