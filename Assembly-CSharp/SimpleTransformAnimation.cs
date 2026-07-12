using System;
using UnityEngine;

public class SimpleTransformAnimation : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		base.transform.Rotate(this.rotationSpeed * Time.unscaledDeltaTime);
		base.transform.Translate(this.translateSpeed * Time.unscaledDeltaTime);
	}

	[SerializeField]
	private Vector3 rotationSpeed;

	[SerializeField]
	private Vector3 translateSpeed;
}
