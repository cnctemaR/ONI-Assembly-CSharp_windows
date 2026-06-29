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
				this.controller = transform.GetComponent<KBatchedAnimController>();
				if (this.controller != null)
				{
					break;
				}
				transform = transform.parent;
			}
		}
		if (this.controller == null)
		{
			global::Debug.Log("Controller Null for tracker on " + base.gameObject.name, base.gameObject);
			base.enabled = false;
			return;
		}
		this.controller.onAnimEnter += this.OnAnimStart;
		this.controller.onAnimComplete += this.OnAnimStop;
		this.controller.onLayerChanged += this.OnLayerChanged;
		this.forceUpdate = true;
		this.myAnim = base.GetComponent<KBatchedAnimController>();
		List<KAnimControllerBase> list = new List<KAnimControllerBase>(base.GetComponentsInChildren<KAnimControllerBase>(true));
		if (!this.skipInitialDisable)
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				base.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		for (int j = list.Count - 1; j >= 0; j--)
		{
			if (list[j].gameObject == base.gameObject)
			{
				list.RemoveAt(j);
			}
		}
	}

	private void OnDisable()
	{
		if (this.myAnim != null)
		{
			KBatchedAnimInstanceData batchInstanceData = this.myAnim.GetBatchInstanceData();
			if (batchInstanceData != null)
			{
				batchInstanceData.ClearOverrideTransformMatrix();
			}
		}
	}

	private void OnDestroy()
	{
		if (this.controller != null)
		{
			this.controller.onAnimEnter -= this.OnAnimStart;
			this.controller.onAnimComplete -= this.OnAnimStop;
			this.controller.onLayerChanged -= this.OnLayerChanged;
			this.controller = null;
		}
		this.myAnim = null;
	}

	private void LateUpdate()
	{
		if (this.controller != null && (this.controller.IsVisible() || this.forceAlwaysVisible || this.forceUpdate))
		{
			this.UpdateFrame();
		}
		if (!this.alive)
		{
			base.enabled = false;
		}
	}

	private void UpdateFrame()
	{
		this.forceUpdate = false;
		bool flag = false;
		KAnim.Anim currentAnim = this.controller.CurrentAnim;
		if (currentAnim != null)
		{
			Matrix2x3 symbolLocalTransform = this.controller.GetSymbolLocalTransform(this.symbol, out flag);
			Vector3 position = this.controller.transform.GetPosition();
			if (flag && (this.previousMatrix != symbolLocalTransform || position != this.previousPosition || this.useTargetPoint))
			{
				this.previousMatrix = symbolLocalTransform;
				this.previousPosition = position;
				Matrix2x3 matrix2x = this.controller.GetTransformMatrix() * symbolLocalTransform;
				float z = base.transform.GetPosition().z;
				base.transform.SetPosition(matrix2x.MultiplyPoint(this.offset));
				if (this.useTargetPoint)
				{
					Vector3 position2 = base.transform.GetPosition();
					position2.z = 0f;
					Vector3 vector = this.targetPoint - position2;
					float num = Vector3.Angle(vector, Vector3.right);
					if (vector.y < 0f)
					{
						num = 360f - num;
					}
					base.transform.localRotation = Quaternion.identity;
					base.transform.RotateAround(position2, new Vector3(0f, 0f, 1f), num);
					float sqrMagnitude = vector.sqrMagnitude;
					this.myAnim.GetBatchInstanceData().SetClipRadius(base.transform.GetPosition().x, base.transform.GetPosition().y, sqrMagnitude, true);
				}
				else
				{
					Vector3 vector2 = ((!this.controller.FlipX) ? Vector3.right : Vector3.left);
					Vector3 vector3 = ((!this.controller.FlipY) ? Vector3.up : Vector3.down);
					base.transform.up = matrix2x.MultiplyVector(vector3);
					base.transform.right = matrix2x.MultiplyVector(vector2);
					if (this.myAnim != null)
					{
						KBatchedAnimInstanceData batchInstanceData = this.myAnim.GetBatchInstanceData();
						if (batchInstanceData != null)
						{
							batchInstanceData.SetOverrideTransformMatrix(matrix2x);
						}
					}
				}
				base.transform.SetPosition(new Vector3(base.transform.GetPosition().x, base.transform.GetPosition().y, z));
				this.myAnim.SetDirty();
			}
		}
		if (this.myAnim != null && flag != this.myAnim.enabled)
		{
			this.myAnim.enabled = flag;
		}
	}

	[ContextMenu("ForceAlive")]
	private void OnAnimStart(HashedString name)
	{
		this.alive = true;
		base.enabled = true;
		this.forceUpdate = true;
	}

	private void OnAnimStop(HashedString name)
	{
		this.alive = false;
	}

	private void OnLayerChanged(int layer)
	{
		this.myAnim.SetLayer(layer);
	}

	public void SetTarget(Vector3 target)
	{
		this.targetPoint = target;
		this.targetPoint.z = 0f;
	}

	[SerializeField]
	public KBatchedAnimController controller;

	[SerializeField]
	public Vector3 offset = Vector3.zero;

	public HashedString symbol;

	public Vector3 targetPoint = Vector3.zero;

	public bool useTargetPoint;

	public bool fadeOut = true;

	public bool skipInitialDisable;

	public bool forceAlwaysVisible;

	private bool alive = true;

	private bool forceUpdate;

	private Matrix2x3 previousMatrix;

	private Vector3 previousPosition;

	private KBatchedAnimController myAnim;
}
