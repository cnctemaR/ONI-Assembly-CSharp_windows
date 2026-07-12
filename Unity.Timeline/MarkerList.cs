using System;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	[Serializable]
	internal struct MarkerList : ISerializationCallbackReceiver
	{
		public List<IMarker> markers
		{
			get
			{
				this.BuildCache();
				return this.m_Cache;
			}
		}

		public MarkerList(int capacity)
		{
			this.m_Objects = new List<ScriptableObject>(capacity);
			this.m_Cache = new List<IMarker>(capacity);
			this.m_CacheDirty = true;
			this.m_HasNotifications = false;
		}

		public void Add(ScriptableObject item)
		{
			if (item == null)
			{
				return;
			}
			this.m_Objects.Add(item);
			this.m_CacheDirty = true;
		}

		public bool Remove(IMarker item)
		{
			if (!(item is ScriptableObject))
			{
				throw new InvalidOperationException("Supplied type must be a ScriptableObject");
			}
			return this.Remove((ScriptableObject)item, item.parent.timelineAsset, item.parent);
		}

		public bool Remove(ScriptableObject item, TimelineAsset timelineAsset, PlayableAsset thingToDirty)
		{
			if (!this.m_Objects.Contains(item))
			{
				return false;
			}
			this.m_Objects.Remove(item);
			this.m_CacheDirty = true;
			TimelineUndo.PushDestroyUndo(timelineAsset, thingToDirty, item);
			return true;
		}

		public void Clear()
		{
			this.m_Objects.Clear();
			this.m_CacheDirty = true;
		}

		public bool Contains(ScriptableObject item)
		{
			return this.m_Objects.Contains(item);
		}

		public IEnumerable<IMarker> GetMarkers()
		{
			return this.markers;
		}

		public int Count
		{
			get
			{
				return this.markers.Count;
			}
		}

		public IMarker this[int idx]
		{
			get
			{
				return this.markers[idx];
			}
		}

		public List<ScriptableObject> GetRawMarkerList()
		{
			return this.m_Objects;
		}

		public IMarker CreateMarker(Type type, double time, TrackAsset owner)
		{
			if (!typeof(ScriptableObject).IsAssignableFrom(type) || !typeof(IMarker).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("The requested type needs to inherit from ScriptableObject and implement IMarker");
			}
			if (!owner.supportsNotifications && typeof(INotification).IsAssignableFrom(type))
			{
				throw new InvalidOperationException("Markers implementing the INotification interface cannot be added on tracks that do not support notifications");
			}
			ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);
			IMarker marker = (IMarker)scriptableObject;
			marker.time = time;
			TimelineCreateUtilities.SaveAssetIntoObject(scriptableObject, owner);
			this.Add(scriptableObject);
			marker.Initialize(owner);
			return marker;
		}

		public bool HasNotifications()
		{
			this.BuildCache();
			return this.m_HasNotifications;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this.m_CacheDirty = true;
		}

		private void BuildCache()
		{
			if (this.m_CacheDirty)
			{
				this.m_Cache = new List<IMarker>(this.m_Objects.Count);
				this.m_HasNotifications = false;
				foreach (ScriptableObject scriptableObject in this.m_Objects)
				{
					if (scriptableObject != null)
					{
						this.m_Cache.Add(scriptableObject as IMarker);
						if (scriptableObject is INotification)
						{
							this.m_HasNotifications = true;
						}
					}
				}
				this.m_CacheDirty = false;
			}
		}

		[SerializeField]
		[HideInInspector]
		private List<ScriptableObject> m_Objects;

		[HideInInspector]
		[NonSerialized]
		private List<IMarker> m_Cache;

		private bool m_CacheDirty;

		private bool m_HasNotifications;
	}
}
