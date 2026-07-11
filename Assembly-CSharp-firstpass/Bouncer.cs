using System;
using System.Collections;
using UnityEngine;

public class Bouncer : MonoBehaviour
{
	public void Bounce()
	{
		if (base.gameObject.activeInHierarchy && !this.m_bouncing)
		{
			base.StartCoroutine(this.DoBounce());
		}
	}

	public bool IsBouncing()
	{
		return this.m_bouncing;
	}

	private IEnumerator DoBounce()
	{
		this.m_bouncing = true;
		float completion = 0f;
		Vector3 position = new Vector3(base.gameObject.transform.position.x, base.gameObject.transform.position.y, base.gameObject.transform.position.z);
		float startPos = base.gameObject.transform.position.y;
		while (completion < 1f)
		{
			completion = Mathf.Min(completion + Time.unscaledDeltaTime / this.durationSecs, 1f);
			float num = Bouncer.BounceSpline(completion) * this.height;
			position.y = startPos + num;
			base.gameObject.transform.position = position;
			yield return new WaitForEndOfFrame();
		}
		position.y = startPos;
		base.gameObject.transform.position = position;
		this.m_bouncing = false;
		yield break;
	}

	private static float BounceSpline(float k)
	{
		if (k < 0.5f)
		{
			return Bouncer.QuadOut(k * 2f);
		}
		return 1f - Bouncer.QuadIn(k * 2f - 1f);
	}

	private static float QuadOut(float k)
	{
		return k * (2f - k);
	}

	private static float QuadIn(float k)
	{
		return k * k;
	}

	private bool m_bouncing;

	public float durationSecs = 0.3f;

	public float height = 20f;
}
