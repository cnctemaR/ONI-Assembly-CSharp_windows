using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UIElements
{
	public struct StartDragArgs
	{
		public StartDragArgs(string title, DragVisualMode visualMode)
		{
			this.title = title;
			this.visualMode = visualMode;
			this.genericData = null;
			this.assetPaths = null;
			this.entityIds = null;
			this.modifiers = EventModifiers.None;
		}

		internal StartDragArgs(string title, object target)
		{
			this = default(StartDragArgs);
			this.title = title;
			this.visualMode = DragVisualMode.Move;
			this.genericData = null;
			this.assetPaths = null;
			this.entityIds = null;
			this.SetGenericData("__unity-drag-and-drop__source-view", target);
		}

		internal StartDragArgs(string title, DragVisualMode visualMode, EventModifiers modifiers)
		{
			this = new StartDragArgs(title, visualMode);
			this.modifiers = modifiers;
		}

		public readonly string title { get; }

		public readonly DragVisualMode visualMode { get; }

		internal EventModifiers modifiers { readonly get; set; }

		internal Hashtable genericData { readonly get; private set; }

		internal IReadOnlyList<EntityId> entityIds { readonly get; private set; }

		internal string[] assetPaths { readonly get; private set; }

		public void SetGenericData(string key, object data)
		{
			if (this.genericData == null)
			{
				this.genericData = new Hashtable();
			}
			this.genericData[key] = data;
		}

		[Obsolete("Use SetEntityIds instead, and call Object.GetEntityId() if you really need to convert from a Unity object to an EntityId.")]
		public void SetUnityObjectReferences(IEnumerable<Object> references)
		{
			this.SetEntityIds(references.Select<Object, EntityId>((Object x) => x.GetEntityId()).ToList<EntityId>());
		}

		public void SetEntityIds(IReadOnlyList<EntityId> ids)
		{
			this.entityIds = ids;
		}

		public void SetPaths(string[] paths)
		{
			this.assetPaths = paths;
		}
	}
}
