using System;
using UnityEngine;

public class UIRotator : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.rotationSpeed = global::UnityEngine.Random.Range(this.minRotationSpeed, this.maxRotationSpeed);
	}

	private void Update()
	{
		RectTransform component = base.GetComponent<RectTransform>();
		component.Rotate(0f, 0f, this.rotationSpeed * Time.unscaledDeltaTime);
	}

	public float minRotationSpeed = 1f;

	public float maxRotationSpeed = 1f;

	public float rotationSpeed = 1f;
}
