using System;
using UnityEngine;

[ExecuteInEditMode]
public class KAlign : MonoBehaviour
{
	public void SetTarget(GameObject newtarget)
	{
		this.target = newtarget;
		this.Update();
	}

	private void OnEnable()
	{
		this.Update();
	}

	private void Update()
	{
		if (this.target != null)
		{
			RectTransform rectTransform = this.target.rectTransform();
			if (rectTransform != null)
			{
				Vector3[] array = new Vector3[4];
				rectTransform.GetWorldCorners(array);
				Vector3 vector = array[1];
				Vector3 vector2 = array[3];
				Vector3 position = base.transform.position;
				Vector3[] array2 = new Vector3[4];
				this.rectTransform().GetWorldCorners(array2);
				Vector3 vector3 = array2[1];
				Vector3 vector4 = array2[3];
				float num = position.x;
				float num2 = position.y;
				if (this.targetHorizontal != KAlign.TargetLeftRight.None)
				{
					num = this.offset.x;
					if (this.sourceHorizontal == KAlign.SourceLeftRight.Left)
					{
						num += position.x - vector3.x;
					}
					else if (this.sourceHorizontal == KAlign.SourceLeftRight.Right)
					{
						num += position.x - vector4.x;
					}
					else if (this.sourceHorizontal == KAlign.SourceLeftRight.Middle)
					{
						num += vector3.x - position.x + (vector4.x - vector3.x) / 2f;
					}
					if (this.targetHorizontal == KAlign.TargetLeftRight.Right)
					{
						num += vector2.x;
					}
					else if (this.targetHorizontal == KAlign.TargetLeftRight.Left)
					{
						num += vector.x;
					}
					else if (this.targetHorizontal == KAlign.TargetLeftRight.Middle)
					{
						num += vector.x + (vector2.x - vector.x) / 2f;
					}
				}
				if (this.targetVertical != KAlign.TargetTopBottom.None)
				{
					num2 = this.offset.y;
					if (this.sourceVertical == KAlign.SourceTopBottom.Top)
					{
						num2 += position.y - vector3.y;
					}
					else if (this.sourceVertical == KAlign.SourceTopBottom.Bottom)
					{
						num2 += position.y - vector4.y;
					}
					else if (this.sourceVertical == KAlign.SourceTopBottom.Middle)
					{
						num2 += position.y - vector3.y + (vector3.y - vector4.y) / 2f;
					}
					if (this.targetVertical == KAlign.TargetTopBottom.Top)
					{
						num2 += vector.y;
					}
					else if (this.targetVertical == KAlign.TargetTopBottom.Bottom)
					{
						num2 += vector2.y;
					}
					else if (this.targetVertical == KAlign.TargetTopBottom.Middle)
					{
						num2 += vector2.y + (vector.y - vector2.y) / 2f;
					}
				}
				position.x = num;
				position.y = num2;
				base.transform.SetPosition(position);
			}
		}
	}

	public GameObject target;

	public KAlign.SourceLeftRight sourceHorizontal;

	public KAlign.SourceTopBottom sourceVertical;

	public KAlign.TargetLeftRight targetHorizontal;

	public KAlign.TargetTopBottom targetVertical;

	public Vector2 offset;

	public enum TargetLeftRight
	{
		None,
		Left,
		Middle,
		Right
	}

	public enum TargetTopBottom
	{
		None,
		Top,
		Middle,
		Bottom
	}

	public enum SourceLeftRight
	{
		Left,
		Middle,
		Right
	}

	public enum SourceTopBottom
	{
		Top,
		Middle,
		Bottom
	}
}
