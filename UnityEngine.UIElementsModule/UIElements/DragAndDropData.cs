using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	internal abstract class DragAndDropData : IDragAndDropData
	{
		public abstract object GetGenericData(string key);

		object IDragAndDropData.userData
		{
			get
			{
				return this.GetGenericData("__unity-drag-and-drop__source-view");
			}
		}

		public abstract void SetGenericData(string key, object data);

		public abstract object source { get; }

		public abstract DragVisualMode visualMode { get; }

		public abstract IEnumerable<Object> unityObjectReferences { get; }

		internal const string dragSourceKey = "__unity-drag-and-drop__source-view";
	}
}
