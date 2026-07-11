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
		int bouncesCompleted = 0;
		Vector3 startPos = base.gameObject.transform.position;
		yield return new WaitForEndOfFrame();
		while (bouncesCompleted < this.numBounces)
		{
			float num = 1f / Mathf.Pow(2f, (float)bouncesCompleted);
			Vector3 iterationTarget = this.bounceTarget * num;
			float num2 = 1f / (float)(bouncesCompleted + 1);
			float iterationDuration = this.durationSecs * num2;
			completion = 0f;
			while (completion < 1f)
			{
				Vector3 position = base.gameObject.transform.position;
				float num3 = Mathf.Min(Time.unscaledDeltaTime, 0.3f);
				completion = Mathf.Min(completion + num3 / iterationDuration, 1f);
				Vector3 vector = Bouncer.BounceSpline(completion) * iterationTarget;
				if (this.bounceTarget.x != 0f)
				{
					position.x = startPos.x + vector.x;
				}
				if (this.bounceTarget.y != 0f)
				{
					position.y = startPos.y + vector.y;
				}
				base.gameObject.transform.SetPosition(position);
				yield return new WaitForEndOfFrame();
			}
			int num4 = bouncesCompleted;
			bouncesCompleted = num4 + 1;
			iterationTarget = default(Vector3);
		}
		Vector3 position2 = base.gameObject.transform.position;
		if (this.bounceTarget.x != 0f)
		{
			position2.x = startPos.x;
		}
		if (this.bounceTarget.y != 0f)
		{
			position2.y = startPos.y;
		}
		base.gameObject.transform.SetPosition(position2);
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

	public Vector3 bounceTarget;

	public int numBounces = 1;
}
