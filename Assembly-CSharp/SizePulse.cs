using System;
using UnityEngine;

public class SizePulse : MonoBehaviour
{
	private void Start()
	{
		if (base.GetComponents<SizePulse>().Length > 1)
		{
			global::UnityEngine.Object.Destroy(this);
		}
		RectTransform rectTransform = (RectTransform)base.transform;
		this.from = rectTransform.localScale;
		this.cur = this.from;
		this.to = this.from * this.multiplier;
	}

	private void Update()
	{
		float num = (this.updateWhenPaused ? Time.unscaledDeltaTime : Time.deltaTime);
		num *= this.speed;
		SizePulse.State state = this.state;
		if (state != SizePulse.State.Up)
		{
			if (state == SizePulse.State.Down)
			{
				this.cur = Vector2.Lerp(this.cur, this.from, num);
				if ((this.from - this.cur).sqrMagnitude < 0.0001f)
				{
					this.cur = this.from;
					this.state = SizePulse.State.Finished;
					if (this.onComplete != null)
					{
						this.onComplete();
					}
				}
			}
		}
		else
		{
			this.cur = Vector2.Lerp(this.cur, this.to, num);
			if ((this.to - this.cur).sqrMagnitude < 0.0001f)
			{
				this.cur = this.to;
				this.state = SizePulse.State.Down;
			}
		}
		((RectTransform)base.transform).localScale = new Vector3(this.cur.x, this.cur.y, 1f);
	}

	public global::System.Action onComplete;

	public Vector2 from = Vector2.one;

	public Vector2 to = Vector2.one;

	public float multiplier = 1.25f;

	public float speed = 1f;

	public bool updateWhenPaused;

	private Vector2 cur;

	private SizePulse.State state;

	private enum State
	{
		Up,
		Down,
		Finished
	}
}
