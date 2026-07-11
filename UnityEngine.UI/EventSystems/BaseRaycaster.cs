using System;
using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
	public abstract class BaseRaycaster : UIBehaviour
	{
		public abstract void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList);

		public abstract Camera eventCamera { get; }

		[Obsolete("Please use sortOrderPriority and renderOrderPriority", false)]
		public virtual int priority
		{
			get
			{
				return 0;
			}
		}

		public virtual int sortOrderPriority
		{
			get
			{
				return int.MinValue;
			}
		}

		public virtual int renderOrderPriority
		{
			get
			{
				return int.MinValue;
			}
		}

		public BaseRaycaster rootRaycaster
		{
			get
			{
				if (this.m_RootRaycaster == null)
				{
					BaseRaycaster[] componentsInParent = base.GetComponentsInParent<BaseRaycaster>();
					if (componentsInParent.Length != 0)
					{
						this.m_RootRaycaster = componentsInParent[componentsInParent.Length - 1];
					}
				}
				return this.m_RootRaycaster;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[] { "Name: ", base.gameObject, "\neventCamera: ", this.eventCamera, "\nsortOrderPriority: ", this.sortOrderPriority, "\nrenderOrderPriority: ", this.renderOrderPriority });
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			RaycasterManager.AddRaycaster(this);
		}

		protected override void OnDisable()
		{
			RaycasterManager.RemoveRaycasters(this);
			base.OnDisable();
		}

		protected override void OnCanvasHierarchyChanged()
		{
			base.OnCanvasHierarchyChanged();
			this.m_RootRaycaster = null;
		}

		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			this.m_RootRaycaster = null;
		}

		private BaseRaycaster m_RootRaycaster;
	}
}
