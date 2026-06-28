using System;
using UnityEngine;

public class SelectMarker : KMonoBehaviour
{
	public void SetTargetTransform(Transform target_transform)
	{
		this.targetTransform = target_transform;
		this.LateUpdate();
	}

	private void LateUpdate()
	{
		if (this.targetTransform == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		Vector3 position = this.targetTransform.position;
		Collider2D component = this.targetTransform.GetComponent<Collider2D>();
		if (component != null)
		{
			position.x = component.bounds.center.x;
			position.y += component.bounds.size.y + 0.1f;
		}
		else
		{
			position.y += 2f;
		}
		Vector3 vector = new Vector3(0f, (Mathf.Sin(Time.unscaledTime * 4f) + 1f) * 0.1f, 0f);
		this.transform.SetPosition(position + vector);
	}

	private Transform targetTransform;
}
