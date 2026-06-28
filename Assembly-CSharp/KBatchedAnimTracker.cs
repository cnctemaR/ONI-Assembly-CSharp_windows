using System;
using System.Collections.Generic;
using UnityEngine;

public class KBatchedAnimTracker : MonoBehaviour
{
	private void Start()
	{
		if (this.controller == null)
		{
			Transform transform = base.transform.parent;
			while (transform != null)
			{
				this.controller = transform.GetComponent<KAnimControllerBase>();
				if (this.controller != null)
				{
					break;
				}
				transform = transform.parent;
			}
		}
		if (this.controller == null)
		{
			Debug.Log("Controller Null for tracker", base.gameObject);
		}
		this.myAnim = base.GetComponent<KBatchedAnimController>();
		List<KAnimControllerBase> list = new List<KAnimControllerBase>(base.GetComponentsInChildren<KAnimControllerBase>(true));
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(false);
		}
		for (int j = list.Count - 1; j >= 0; j--)
		{
			if (!(list[j].gameObject != base.gameObject))
			{
				list.RemoveAt(j);
			}
		}
		this.anims = list.ToArray();
	}

	private void LateUpdate()
	{
		this.UpdateFrame();
	}

	private void OnDisable()
	{
		this.wasVisible = false;
	}

	private void UpdateFrame()
	{
		if (this.controller != null)
		{
			bool flag = false;
			KAnim.Anim currentAnim = this.controller.CurrentAnim;
			if (currentAnim != null)
			{
				Matrix4x4 matrix4x = this.controller.GetSymbolTransform(this.symbol, out flag);
				matrix4x *= Matrix4x4.Scale(this.matrixScale);
				if (flag)
				{
					float z = base.transform.position.z;
					if (this.controller is KBatchedAnimController)
					{
						base.transform.SetPosition(matrix4x.MultiplyPoint3x4(this.offset) + this.postOffset);
					}
					else
					{
						base.transform.localPosition = matrix4x.MultiplyPoint3x4(this.offset);
						base.transform.localPosition += this.postOffset;
					}
					if (this.trackScale)
					{
						base.transform.localScale = new Vector3(matrix4x.m00, matrix4x.m11, matrix4x.m22);
					}
					if (this.useTargetPoint)
					{
						Vector3 position = base.transform.position;
						position.z = 0f;
						Vector3 vector = this.targetPoint - position;
						float num = Vector3.Angle(vector, Vector3.right);
						if (vector.y < 0f)
						{
							num = 360f - num;
						}
						base.transform.localRotation = Quaternion.identity;
						base.transform.RotateAround(position, new Vector3(0f, 0f, 1f), num);
						float sqrMagnitude = vector.sqrMagnitude;
						this.myAnim.GetBatchInstanceData().SetClipRadius(base.transform.position.x, base.transform.position.y, sqrMagnitude, true);
					}
					else if (!this.ignoreRotation)
					{
						base.transform.up = matrix4x.MultiplyVector(Vector3.up);
						base.transform.right = matrix4x.MultiplyVector(Vector3.right);
					}
					base.transform.SetPosition(new Vector3(base.transform.position.x, base.transform.position.y, z));
				}
				float num2 = (float)currentAnim.numFrames / currentAnim.frameRate;
				float num3 = this.controller.PlayTime % num2;
				flag = flag && (!this.filterByAnim || currentAnim.name == this.anim) && (!this.useFrameRange || (this.frameRange.startTime <= num3 && num3 < this.frameRange.stopTime));
			}
			if (flag != this.wasVisible)
			{
				this.wasVisible = flag;
				if (flag)
				{
					this.myAnim.enabled = true;
				}
				else
				{
					this.myAnim.enabled = false;
				}
				if (this.matchVisibility)
				{
					this.myAnim.enabled = flag;
				}
			}
		}
	}

	public void SetTarget(Vector3 target)
	{
		this.targetPoint = target;
		this.targetPoint.z = 0f;
	}

	public void OnDrawGizmosSelected()
	{
		float num = 0.15f;
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, num);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.targetPoint, num);
	}

	[SerializeField]
	private KAnimControllerBase controller;

	[SerializeField]
	public Vector3 matrixScale = Vector3.one;

	[SerializeField]
	public Vector3 offset = Vector3.zero;

	[SerializeField]
	public Vector3 postOffset = Vector3.zero;

	[SerializeField]
	private bool trackScale;

	public HashedString symbol;

	public string anim;

	public bool filterByAnim = true;

	public bool matchVisibility;

	public Vector3 targetPoint = Vector3.zero;

	public bool useTargetPoint;

	public bool useFrameRange;

	public KBatchedAnimTracker.FrameRange frameRange;

	public bool ignoreRotation;

	public bool fadeOut = true;

	private bool wasVisible;

	private KAnimControllerBase[] anims;

	private KBatchedAnimController myAnim;

	[Serializable]
	public struct FrameRange
	{
		public float startTime;

		public float stopTime;
	}
}
