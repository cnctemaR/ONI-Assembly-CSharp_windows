using System;
using UnityEngine;

public class KCircleCollider2D : KCollider2D
{
	public float radius
	{
		get
		{
			return this._radius;
		}
		set
		{
			this._radius = value;
			base.MarkDirty(false);
		}
	}

	public override Extents GetExtents()
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = new Vector2(vector.x - this.radius, vector.y - this.radius);
		Vector2 vector3 = new Vector2(vector.x + this.radius, vector.y + this.radius);
		int num = (int)vector3.x - (int)vector2.x + 1;
		int num2 = (int)vector3.y - (int)vector2.y + 1;
		return new Extents((int)(vector.x - this._radius), (int)(vector.y - this._radius), num, num2);
	}

	public override Bounds bounds
	{
		get
		{
			return new Bounds(base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f), new Vector3(this._radius, this._radius, 0f));
		}
	}

	public override bool Intersects(Vector2 pos)
	{
		Vector3 position = base.transform.GetPosition();
		Vector2 vector = new Vector2(position.x, position.y) + base.offset;
		return (pos - vector).sqrMagnitude <= this._radius * this._radius;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.bounds.center, this.radius);
	}

	[SerializeField]
	private float _radius;
}
