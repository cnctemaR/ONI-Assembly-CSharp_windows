using System;
using UnityEngine;

public class HideIfDisableLinks : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}
}
