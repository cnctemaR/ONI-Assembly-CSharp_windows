using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
	[RequireComponent(typeof(EventSystem))]
	public abstract class BaseInputModule : UIBehaviour
	{
		public BaseInput input
		{
			get
			{
				BaseInput baseInput;
				if (this.m_InputOverride != null)
				{
					baseInput = this.m_InputOverride;
				}
				else
				{
					if (this.m_DefaultInput == null)
					{
						BaseInput[] components = base.GetComponents<BaseInput>();
						foreach (BaseInput baseInput2 in components)
						{
							if (baseInput2 != null && baseInput2.GetType() == typeof(BaseInput))
							{
								this.m_DefaultInput = baseInput2;
								break;
							}
						}
						if (this.m_DefaultInput == null)
						{
							this.m_DefaultInput = base.gameObject.AddComponent<BaseInput>();
						}
					}
					baseInput = this.m_DefaultInput;
				}
				return baseInput;
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
			for (int i = 0; i < candidates.Count; i++)
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
			Vector2 vector = new Vector2(x, y);
			MoveDirection moveDirection;
			if (vector.sqrMagnitude < deadZone * deadZone)
			{
				moveDirection = MoveDirection.None;
			}
			else if (Mathf.Abs(x) > Mathf.Abs(y))
			{
				if (x > 0f)
				{
					moveDirection = MoveDirection.Right;
				}
				else
				{
					moveDirection = MoveDirection.Left;
				}
			}
			else if (y > 0f)
			{
				moveDirection = MoveDirection.Up;
			}
			else
			{
				moveDirection = MoveDirection.Down;
			}
			return moveDirection;
		}

		protected static GameObject FindCommonRoot(GameObject g1, GameObject g2)
		{
			GameObject gameObject;
			if (g1 == null || g2 == null)
			{
				gameObject = null;
			}
			else
			{
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
				gameObject = null;
			}
			return gameObject;
		}

		protected void HandlePointerExitAndEnter(PointerEventData currentPointerData, GameObject newEnterTarget)
		{
			if (newEnterTarget == null || currentPointerData.pointerEnter == null)
			{
				for (int i = 0; i < currentPointerData.hovered.Count; i++)
				{
					ExecuteEvents.Execute<IPointerExitHandler>(currentPointerData.hovered[i], currentPointerData, ExecuteEvents.pointerExitHandler);
				}
				currentPointerData.hovered.Clear();
				if (newEnterTarget == null)
				{
					currentPointerData.pointerEnter = null;
					return;
				}
			}
			if (!(currentPointerData.pointerEnter == newEnterTarget) || !newEnterTarget)
			{
				GameObject gameObject = BaseInputModule.FindCommonRoot(currentPointerData.pointerEnter, newEnterTarget);
				if (currentPointerData.pointerEnter != null)
				{
					Transform transform = currentPointerData.pointerEnter.transform;
					while (transform != null)
					{
						if (gameObject != null && gameObject.transform == transform)
						{
							break;
						}
						ExecuteEvents.Execute<IPointerExitHandler>(transform.gameObject, currentPointerData, ExecuteEvents.pointerExitHandler);
						currentPointerData.hovered.Remove(transform.gameObject);
						transform = transform.parent;
					}
				}
				currentPointerData.pointerEnter = newEnterTarget;
				if (newEnterTarget != null)
				{
					Transform transform2 = newEnterTarget.transform;
					while (transform2 != null && transform2.gameObject != gameObject)
					{
						ExecuteEvents.Execute<IPointerEnterHandler>(transform2.gameObject, currentPointerData, ExecuteEvents.pointerEnterHandler);
						currentPointerData.hovered.Add(transform2.gameObject);
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

		[NonSerialized]
		protected List<RaycastResult> m_RaycastResultCache = new List<RaycastResult>();

		private AxisEventData m_AxisEventData;

		private EventSystem m_EventSystem;

		private BaseEventData m_BaseEventData;

		protected BaseInput m_InputOverride;

		private BaseInput m_DefaultInput;
	}
}
