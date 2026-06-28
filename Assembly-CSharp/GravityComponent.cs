using System;
using UnityEngine;

public struct GravityComponent
{
	public GravityComponent(Transform transform, global::System.Action on_landed)
	{
		this = new GravityComponent(transform, on_landed, Vector2.zero);
	}

	public GravityComponent(Transform transform, global::System.Action on_landed, Vector2 initial_velocity)
	{
		this.transform = transform;
		this.elapsedTime = 0f;
		this.initialVelocity = initial_velocity;
		this.onLanded = on_landed;
		this.offset = GravityComponent.GetOffset(transform);
	}

	public Vector2 Velocity
	{
		get
		{
			Vector2 vector = new Vector2(this.initialVelocity.x, this.initialVelocity.y + -9.8f * Mathf.Max(0f, this.elapsedTime));
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude > 784f)
			{
				vector *= 28f / Mathf.Sqrt(sqrMagnitude);
			}
			return vector;
		}
	}

	public static float GetOffset(Transform transform)
	{
		KCircleCollider2D component = transform.GetComponent<KCircleCollider2D>();
		if (component != null)
		{
			return component.radius;
		}
		KCollider2D component2 = transform.GetComponent<KCollider2D>();
		if (component2 != null)
		{
			return transform.GetPosition().y - component2.bounds.min.y;
		}
		return 0f;
	}

	private const float Acceleration = -9.8f;

	public Transform transform;

	public Vector2 initialVelocity;

	public float offset;

	public float elapsedTime;

	public global::System.Action onLanded;

	private const float MAX_VELOCITY = 28f;
}
