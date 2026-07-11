using System;
using UnityEngine;

public class PopIn : MonoBehaviour
{
	private void OnEnable()
	{
		this.StartPopIn(true);
	}

	private void Update()
	{
		float num = Mathf.Lerp(base.transform.localScale.x, this.targetScale, Time.unscaledDeltaTime * this.speed);
		base.transform.localScale = new Vector3(num, num, 1f);
	}

	public void StartPopIn(bool force_reset = false)
	{
		if (force_reset)
		{
			base.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
		}
		this.targetScale = 1f;
	}

	public void StartPopOut()
	{
		this.targetScale = 0f;
	}

	private float targetScale;

	public float speed;
}
