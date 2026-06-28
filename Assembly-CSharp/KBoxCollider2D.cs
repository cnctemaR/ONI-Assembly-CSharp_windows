using System;
using UnityEngine;

public class KBoxCollider2D : KCollider2D
{
	public Vector2 size
	{
		get
		{
			return this._size;
		}
		set
		{
			this._size = value;
			base.MarkDirty(false);
		}
	}

	public override Extents GetExtents()
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = this.size * 0.9999f;
		Vector2 vector3 = new Vector2(vector.x - vector2.x * 0.5f, vector.y - vector2.y * 0.5f);
		Vector2 vector4 = new Vector2(vector.x + vector2.x * 0.5f, vector.y + vector2.y * 0.5f);
		Vector2I vector2I = new Vector2I((int)vector3.x, (int)vector3.y);
		Vector2I vector2I2 = new Vector2I((int)vector4.x, (int)vector4.y);
		int num = vector2I2.x - vector2I.x + 1;
		int num2 = vector2I2.y - vector2I.y + 1;
		return new Extents(vector2I.x, vector2I.y, num, num2);
	}

	public override bool Intersects(Vector2 intersect_pos)
	{
		Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
		Vector2 vector2 = new Vector2(vector.x - this.size.x * 0.5f, vector.y - this.size.y * 0.5f);
		Vector2 vector3 = new Vector2(vector.x + this.size.x * 0.5f, vector.y + this.size.y * 0.5f);
		return intersect_pos.x >= vector2.x && intersect_pos.x <= vector3.x && intersect_pos.y >= vector2.y && intersect_pos.y <= vector3.y;
	}

	public override Bounds bounds
	{
		get
		{
			Vector3 vector = base.transform.GetPosition() + new Vector3(base.offset.x, base.offset.y, 0f);
			return new Bounds(vector, new Vector3(this._size.x, this._size.y, 0f));
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(this.bounds.center, new Vector3(this._size.x, this._size.y, 0f));
	}

	[SerializeField]
	private Vector2 _size;
}
