using System;
using UnityEngine.UIElements;

namespace UnityEngine.EventSystems
{
	public struct RaycastResult
	{
		public GameObject gameObject
		{
			get
			{
				return this.m_GameObject;
			}
			set
			{
				this.m_GameObject = value;
			}
		}

		public bool isValid
		{
			get
			{
				return this.module != null && this.gameObject != null;
			}
		}

		public void Clear()
		{
			this.gameObject = null;
			this.module = null;
			this.distance = 0f;
			this.index = 0f;
			this.depth = 0;
			this.sortingLayer = 0;
			this.sortingOrder = 0;
			this.origin = Vector3.zero;
			this.worldNormal = Vector3.up;
			this.worldPosition = Vector3.zero;
			this.screenPosition = Vector3.zero;
			this.displayIndex = 0;
			this.document = null;
			this.element = null;
		}

		public override string ToString()
		{
			if (!this.isValid)
			{
				return "";
			}
			string[] array = new string[24];
			array[0] = "Name: ";
			int num = 1;
			GameObject gameObject = this.gameObject;
			array[num] = ((gameObject != null) ? gameObject.ToString() : null);
			array[2] = "\nmodule: ";
			int num2 = 3;
			BaseRaycaster baseRaycaster = this.module;
			array[num2] = ((baseRaycaster != null) ? baseRaycaster.ToString() : null);
			array[4] = "\ndistance: ";
			array[5] = this.distance.ToString();
			array[6] = "\nindex: ";
			array[7] = this.index.ToString();
			array[8] = "\ndepth: ";
			array[9] = this.depth.ToString();
			array[10] = "\nworldNormal: ";
			array[11] = this.worldNormal.ToString();
			array[12] = "\nworldPosition: ";
			array[13] = this.worldPosition.ToString();
			array[14] = "\nscreenPosition: ";
			array[15] = this.screenPosition.ToString();
			array[16] = "\nmodule.sortOrderPriority: ";
			array[17] = this.module.sortOrderPriority.ToString();
			array[18] = "\nmodule.renderOrderPriority: ";
			array[19] = this.module.renderOrderPriority.ToString();
			array[20] = "\nsortingLayer: ";
			array[21] = this.sortingLayer.ToString();
			array[22] = "\nsortingOrder: ";
			array[23] = this.sortingOrder.ToString();
			return string.Concat(array);
		}

		private GameObject m_GameObject;

		public BaseRaycaster module;

		public float distance;

		public float index;

		public int depth;

		public int sortingGroupID;

		public int sortingGroupOrder;

		public int sortingLayer;

		public int sortingOrder;

		public Vector3 origin;

		public Vector3 worldPosition;

		public Vector3 worldNormal;

		public Vector2 screenPosition;

		public int displayIndex;

		public UIDocument document;

		public VisualElement element;
	}
}
