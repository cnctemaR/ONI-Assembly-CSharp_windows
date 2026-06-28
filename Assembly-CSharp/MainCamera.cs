using System;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
	private void Awake()
	{
		if (Camera.main != null)
		{
			global::UnityEngine.Object.Destroy(Camera.main.gameObject);
		}
		base.gameObject.tag = "MainCamera";
	}
}
