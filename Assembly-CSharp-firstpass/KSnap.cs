using System;
using UnityEngine;

[ExecuteInEditMode]
public class KSnap : MonoBehaviour
{
	public void SetTarget(GameObject newtarget)
	{
		this.target = newtarget;
		this.Update();
	}

	private void Update()
	{
		if (this.target != null)
		{
			RectTransform rectTransform = this.target.rectTransform();
			if (rectTransform != null)
			{
				rectTransform.GetWorldCorners(this.corners);
				Vector3 vector = this.corners[2];
				Vector3 vector2 = this.corners[0];
				Vector3 position = base.transform.position;
				if (this.horizontal == KSnap.LeftRight.Left)
				{
					position.x = vector2.x + this.offset.x;
				}
				else if (this.horizontal == KSnap.LeftRight.Right)
				{
					position.x = vector.x + this.offset.x;
				}
				else if (this.horizontal == KSnap.LeftRight.Middle)
				{
					position.x = vector.x + (vector2.x - vector.x) / 2f + this.offset.x;
				}
				if (this.vertical == KSnap.TopBottom.Top)
				{
					position.y = vector.y + this.offset.y;
				}
				else if (this.vertical == KSnap.TopBottom.Bottom)
				{
					position.y = vector2.y + this.offset.y;
				}
				else if (this.vertical == KSnap.TopBottom.Middle)
				{
					position.y = vector2.y + (vector.y - vector2.y) / 2f + this.offset.y;
				}
				base.transform.SetPosition(position);
				this.KeepOnScreen();
			}
		}
	}

	private void KeepOnScreen()
	{
		if (!this.keepOnScreen)
		{
			return;
		}
		RectTransform rectTransform = base.transform.rectTransform();
		rectTransform.GetWorldCorners(this.corners);
		Vector3 zero = Vector3.zero;
		foreach (Vector3 vector in this.corners)
		{
			if (vector.x < 0f)
			{
				zero.x = Mathf.Max(zero.x, -vector.x);
			}
			if (vector.x > (float)Screen.width)
			{
				zero.x = Mathf.Min(zero.x, (float)Screen.width - vector.x);
			}
			if (vector.y < 0f)
			{
				zero.y = Mathf.Max(zero.y, -vector.y);
			}
			if (vector.y > (float)Screen.height)
			{
				zero.y = Mathf.Min(zero.y, (float)Screen.height - vector.y);
			}
		}
		base.transform.position += zero;
	}

	public GameObject target;

	public KSnap.LeftRight horizontal;

	public KSnap.TopBottom vertical;

	public Vector2 offset;

	[SerializeField]
	private bool keepOnScreen;

	private Vector3[] corners = new Vector3[4];

	public enum LeftRight
	{
		None,
		Left,
		Middle,
		Right
	}

	public enum TopBottom
	{
		None,
		Top,
		Middle,
		Bottom
	}
}
