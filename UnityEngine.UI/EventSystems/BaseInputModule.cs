using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEngine.EventSystems
{
	[RequireComponent(typeof(EventSystem))]
	public abstract class BaseInputModule : UIBehaviour
	{
		protected internal bool sendPointerHoverToParent
		{
			get
			{
				return this.m_SendPointerHoverToParent;
			}
			set
			{
				this.m_SendPointerHoverToParent = value;
			}
		}

		public BaseInput input
		{
			get
			{
				if (this.m_InputOverride != null)
				{
					return this.m_InputOverride;
				}
				if (this.m_DefaultInput == null)
				{
					foreach (BaseInput baseInput in base.GetComponents<BaseInput>())
					{
						if (baseInput != null && baseInput.GetType() == typeof(BaseInput))
						{
							this.m_DefaultInput = baseInput;
							break;
						}
					}
					if (this.m_DefaultInput == null)
					{
						this.m_DefaultInput = base.gameObject.AddComponent<BaseInput>();
					}
				}
				return this.m_DefaultInput;
			}
		}

		public BaseInput inputOverride
		{
			get
			{
				return this.m_InputOverride;
			}
			set
			{
				this.m_InputOverride = value;
			}
		}

		protected EventSystem eventSystem
		{
			get
			{
				return this.m_EventSystem;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_EventSystem = base.GetComponent<EventSystem>();
			this.m_EventSystem.UpdateModules();
		}

		protected override void OnDisable()
		{
			this.m_EventSystem.UpdateModules();
			base.OnDisable();
		}

		public abstract void Process();

		protected static RaycastResult FindFirstRaycast(List<RaycastResult> candidates)
		{
			int count = candidates.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(candidates[i].gameObject == null))
				{
					return candidates[i];
				}
			}
			return default(RaycastResult);
		}

		protected static MoveDirection DetermineMoveDirection(float x, float y)
		{
			return BaseInputModule.DetermineMoveDirection(x, y, 0.6f);
		}

		protected static MoveDirection DetermineMoveDirection(float x, float y, float deadZone)
		{
			if (new Vector2(x, y).sqrMagnitude < deadZone * deadZone)
			{
				return MoveDirection.None;
			}
			if (Mathf.Abs(x) > Mathf.Abs(y))
			{
				if (x <= 0f)
				{
					return MoveDirection.Left;
				}
				return MoveDirection.Right;
			}
			else
			{
				if (y <= 0f)
				{
					return MoveDirection.Down;
				}
				return MoveDirection.Up;
			}
		}

		protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
		{
			if (g1 == null || g2 == null)
			{
				return null;
			}
			Transform transform = g1.transform;
			while (transform != null)
			{
				Transform transform2 = g2.transform;
				while (transform2 != null)
				{
					if (transform == transform2)
					{
						return transform.gameObject;
					}
					transform2 = transform2.parent;
				}
				transform = transform.parent;
			}
			return null;
		}

		protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
		{
			if (newEnterTarget == null || currentPointerData.pointerEnter == null)
			{
				int count = currentPointerData.hovered.Count;
				for (int i = 0; i < count; i++)
				{
					currentPointerData.fullyExited = true;
					ExecuteEvents.Execute<IPointerMoveHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerMoveHandler);
					ExecuteEvents.Execute<IPointerExitHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
				}
				currentPointerData.hovered.Clear();
				if (newEnterTarget == null)
				{
					currentPointerData.pointerEnter = null;
					return;
				}
			}
			if (currentPointerData.pointerEnter == newEnterTarget && newEnterTarget)
			{
				if (currentPointerData.IsPointerMoving())
				{
					int count2 = currentPointerData.hovered.Count;
					for (int j = 0; j < count2; j++)
					{
						ExecuteEvents.Execute<IPointerMoveHandler>(currentPointerData.hovered[j], currentPointerData, ExecuteEvents.pointerMoveHandler);
					}
				}
				return;
			}
			GameObject gameObject = BaseInputModule.FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
			Component component = (Component)newEnterTarget.GetComponentInParent<IPointerExitHandler>();
			GameObject gameObject2 = ((component != null) ? component.gameObject : null);
			if (currentPointerData.pointerEnter != null)
			{
				Transform transform = currentPointerData.pointerEnter.transform;
				while (transform != null && (!this.m_SendPointerHoverToParent || !(gameObject != null) || !(gameObject.transform == transform)) && (this.m_SendPointerHoverToParent || !(gameObject2 == transform.gameObject)))
				{
					currentPointerData.fullyExited = transform.gameObject != gameObject && currentPointerData.pointerEnter != newEnterTarget;
					ExecuteEvents.Execute<IPointerMoveHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerMoveHandler);
					ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
					currentPointerData.hovered.Remove(transform.gameObject);
					if (this.m_SendPointerHoverToParent)
					{
						transform = transform.parent;
					}
					if (gameObject != null && gameObject.transform == transform)
					{
						break;
					}
					if (!this.m_SendPointerHoverToParent)
					{
						transform = transform.parent;
					}
				}
			}
			GameObject pointerEnter = currentPointerData.pointerEnter;
			currentPointerData.pointerEnter = newEnterTarget;
			if (newEnterTarget != null)
			{
				Transform transform2 = newEnterTarget.transform;
				while (transform2 != null)
				{
					currentPointerData.reentered = transform2.gameObject == gameObject && transform2.gameObject != pointerEnter;
					if (this.m_SendPointerHoverToParent && currentPointerData.reentered)
					{
						break;
					}
					ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
					ExecuteEvents.Execute<IPointerMoveHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerMoveHandler);
					currentPointerData.hovered.Add(transform2.gameObject);
					if (!this.m_SendPointerHoverToParent && transform2.gameObject.GetComponent<IPointerEnterHandler>() != null)
					{
						break;
					}
					if (this.m_SendPointerHoverToParent)
					{
						transform2 = transform2.parent;
					}
					if (gameObject != null && gameObject.transform == transform2)
					{
						break;
					}
					if (!this.m_SendPointerHoverToParent)
					{
						transform2 = transform2.parent;
					}
				}
			}
		}

		protected virtual AxisEventData GetAxisEventData(float x, float y, float moveDeadZone)
		{
			if (this.m_AxisEventData == null)
			{
				this.m_AxisEventData = new AxisEventData(this.eventSystem);
			}
			this.m_AxisEventData.Reset();
			this.m_AxisEventData.moveVector = new Vector2(x, y);
			this.m_AxisEventData.moveDir = BaseInputModule.DetermineMoveDirection(x, y, moveDeadZone);
			return this.m_AxisEventData;
		}

		protected virtual BaseEventData GetBaseEventData()
		{
			if (this.m_BaseEventData == null)
			{
				this.m_BaseEventData = new BaseEventData(this.eventSystem);
			}
			this.m_BaseEventData.Reset();
			return this.m_BaseEventData;
		}

		public virtual bool IsPointerOverGameObject(int pointerId)
		{
			return false;
		}

		public virtual bool ShouldActivateModule()
		{
			return base.enabled && base.gameObject.activeInHierarchy;
		}

		public virtual void DeactivateModule()
		{
		}

		public virtual void ActivateModule()
		{
		}

		public virtual void UpdateModule()
		{
		}

		public virtual bool IsModuleSupported()
		{
			return true;
		}

		public virtual int ConvertUIToolkitPointerId(PointerEventData sourcePointerData)
		{
			if (sourcePointerData.pointerId >= 0)
			{
				return PointerId.touchPointerIdBase + sourcePointerData.pointerId;
			}
			return PointerId.mousePointerId;
		}

		public virtual Vector2 ConvertPointerEventScrollDeltaToTicks(Vector2 scrollDelta)
		{
			return scrollDelta / this.input.mouseScrollDeltaPerTick;
		}

		public virtual NavigationDeviceType GetNavigationEventDeviceType(BaseEventData eventData)
		{
			return NavigationDeviceType.Unknown;
		}

		[NonSerialized]
		protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

		[SerializeField]
		private bool m_SendPointerHoverToParent = true;

		private AxisEventData m_AxisEventData;

		private EventSystem m_EventSystem;

		private BaseEventData m_BaseEventData;

		protected BaseInput m_InputOverride;

		private BaseInput m_DefaultInput;
	}
}
