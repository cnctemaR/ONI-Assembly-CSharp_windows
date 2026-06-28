using System;
using UnityEngine;

public struct FallerComponent
{
	public FallerComponent(Transform transform, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.isFalling = false;
		this.initialVelocity = initial_velocity;
		CircleCollider2D component = transform.GetComponent<CircleCollider2D>();
		if (component != null)
		{
			this.offset = component.radius;
		}
		else
		{
			Collider2D component2 = transform.GetComponent<Collider2D>();
			if (component2 != null)
			{
				this.offset = transform.position.y - component2.bounds.min.y;
			}
			else
			{
				this.offset = 0f;
			}
		}
	}

	public void Destroy()
	{
		FallerComponent.RemoveGravity(this.transform);
	}

	private static void RemoveGravity(Transform transform)
	{
		if (GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Remove(transform.gameObject);
		}
	}

	private static void AddGravity(Transform transform, Vector2 initial_velocity)
	{
		if (!GameComps.Gravities.Has(transform.gameObject))
		{
			GameComps.Gravities.Add(transform.gameObject, initial_velocity, delegate
			{
				FallerComponent.OnLanded(transform);
			});
		}
	}

	private static void OnLanded(Transform transform)
	{
		FallerComponent.RemoveGravity(transform);
	}

	public void FixedUpdate(float dt)
	{
		Vector3 position = this.transform.position;
		position.y = position.y - this.offset - 0.1f;
		int num = Grid.PosToCell(position);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		bool flag = !Grid.Solid[num];
		if (flag != this.isFalling)
		{
			this.isFalling = flag;
			if (flag)
			{
				FallerComponent.AddGravity(this.transform, this.initialVelocity);
			}
			else
			{
				FallerComponent.RemoveGravity(this.transform);
			}
		}
	}

	private Transform transform;

	private bool isFalling;

	private float offset;

	private Vector2 initialVelocity;
}
