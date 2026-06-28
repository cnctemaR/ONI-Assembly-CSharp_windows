using System;
using UnityEngine;

public class ShadowBar
{
	public void toggleSelectionBorder(bool state)
	{
		this.selectionBorder.SetActive(state);
	}

	public int startLineIndex;

	public int endLineIndex;

	public GameObject gameObject;

	public Vector2 SizeBleed = new Vector2(16f, 8f);

	public float leftIndent;

	public float rightIndent;

	public GameObject selectionBorder;
}
