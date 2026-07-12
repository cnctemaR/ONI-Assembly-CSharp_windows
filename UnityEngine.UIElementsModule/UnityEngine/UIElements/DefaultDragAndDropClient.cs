using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal class DefaultDragAndDropClient : IDragAndDrop, IDragAndDropData
	{
		public object userData
		{
			get
			{
				StartDragArgs startDragArgs = this.m_StartDragArgs;
				return (startDragArgs != null) ? startDragArgs.userData : null;
			}
		}

		public IEnumerable<Object> unityObjectReferences
		{
			get
			{
				StartDragArgs startDragArgs = this.m_StartDragArgs;
				return (startDragArgs != null) ? startDragArgs.unityObjectReferences : null;
			}
		}

		public void StartDrag(StartDragArgs args)
		{
			this.m_StartDragArgs = args;
		}

		public void AcceptDrag()
		{
			this.m_StartDragArgs = null;
		}

		public void SetVisualMode(DragVisualMode visualMode)
		{
		}

		public IDragAndDropData data
		{
			get
			{
				return this;
			}
		}

		public object GetGenericData(string key)
		{
			bool flag = this.m_StartDragArgs == null;
			object obj;
			if (flag)
			{
				obj = null;
			}
			else
			{
				obj = (this.m_StartDragArgs.genericData.ContainsKey(key) ? this.m_StartDragArgs.genericData[key] : null);
			}
			return obj;
		}

		private StartDragArgs m_StartDragArgs;
	}
}
