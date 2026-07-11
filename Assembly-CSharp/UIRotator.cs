using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/prefabs/UIRotator")]
public class UIRotator : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.rotationSpeed = global::UnityEngine.Random.Range(this.minRotationSpeed, this.maxRotationSpeed);
	}

	private void Update()
	{
		base.GetComponent<RectTransform>().Rotate(0f, 0f, this.rotationSpeed * Time.unscaledDeltaTime);
	}

	public float minRotationSpeed = 1f;

	public float maxRotationSpeed = 1f;

	public float rotationSpeed = 1f;
}
