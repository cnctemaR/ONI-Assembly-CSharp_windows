using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public sealed class WeakReference<T> : ISerializable where T : class
	{
		public WeakReference(T target)
			: this(target, false)
		{
		}

		public WeakReference(T target, bool trackResurrection)
		{
			this.trackResurrection = trackResurrection;
			GCHandleType gchandleType = (trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
			this.handle = GCHandle.Alloc(target, gchandleType);
		}

		private WeakReference(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.trackResurrection = info.GetBoolean("TrackResurrection");
			object value = info.GetValue("TrackedObject", typeof(T));
			GCHandleType gchandleType = (this.trackResurrection ? GCHandleType.WeakTrackResurrection : GCHandleType.Weak);
			this.handle = GCHandle.Alloc(value, gchandleType);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("TrackResurrection", this.trackResurrection);
			if (this.handle.IsAllocated)
			{
				info.AddValue("TrackedObject", this.handle.Target);
				return;
			}
			info.AddValue("TrackedObject", null);
		}

		public void SetTarget(T target)
		{
			this.handle.Target = target;
		}

		public bool TryGetTarget(out T target)
		{
			if (!this.handle.IsAllocated)
			{
				target = default(T);
				return false;
			}
			target = (T)((object)this.handle.Target);
			return target != null;
		}

		~WeakReference()
		{
			this.handle.Free();
		}

		private GCHandle handle;

		private bool trackResurrection;
	}
}
