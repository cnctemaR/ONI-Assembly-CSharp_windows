using System;
using UnityEngine;

public class BatchAnimCamera : MonoBehaviour
{
	private void Awake()
	{
		this.cam = base.GetComponent<Camera>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.RightArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.right * BatchAnimCamera.pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.left * BatchAnimCamera.pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.up * BatchAnimCamera.pan_speed * Time.deltaTime);
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			base.transform.SetPosition(base.transform.GetPosition() + Vector3.down * BatchAnimCamera.pan_speed * Time.deltaTime);
		}
		this.ClampToBounds();
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetMouseButtonDown(0))
			{
				this.do_pan = true;
				this.last_pan = Input.mousePosition;
			}
			else if (Input.GetMouseButton(0) && this.do_pan)
			{
				Vector3 vector = this.cam.ScreenToViewportPoint(this.last_pan - Input.mousePosition);
				Vector3 vector2 = new Vector3(vector.x * BatchAnimCamera.pan_speed, vector.y * BatchAnimCamera.pan_speed, 0f);
				base.transform.Translate(vector2, Space.World);
				this.ClampToBounds();
				this.last_pan = Input.mousePosition;
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.do_pan = false;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis != 0f)
		{
			this.cam.fieldOfView = Mathf.Clamp(this.cam.fieldOfView - axis * BatchAnimCamera.zoom_speed, this.zoom_min, this.zoom_max);
		}
	}

	private void ClampToBounds()
	{
		Vector3 position = base.transform.GetPosition();
		position.x = Mathf.Clamp(base.transform.GetPosition().x, BatchAnimCamera.bounds.min.x, BatchAnimCamera.bounds.max.x);
		position.y = Mathf.Clamp(base.transform.GetPosition().y, BatchAnimCamera.bounds.min.y, BatchAnimCamera.bounds.max.y);
		position.z = Mathf.Clamp(base.transform.GetPosition().z, BatchAnimCamera.bounds.min.z, BatchAnimCamera.bounds.max.z);
		base.transform.SetPosition(position);
	}

	private void OnDrawGizmosSelected()
	{
		DebugExtension.DebugBounds(BatchAnimCamera.bounds, Color.red, 0f, true);
	}

	private static readonly float pan_speed = 5f;

	private static readonly float zoom_speed = 5f;

	public static Bounds bounds = new Bounds(new Vector3(0f, 0f, -50f), new Vector3(0f, 0f, 50f));

	private float zoom_min = 1f;

	private float zoom_max = 100f;

	private Camera cam;

	private bool do_pan;

	private Vector3 last_pan;
}
