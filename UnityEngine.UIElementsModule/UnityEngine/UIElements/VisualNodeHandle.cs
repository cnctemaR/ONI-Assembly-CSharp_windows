using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodeHandle.h")]
	internal readonly struct VisualNodeHandle : IEquatable<VisualNodeHandle>
	{
		public int Id
		{
			get
			{
				return this.m_Id;
			}
		}

		public int Version
		{
			get
			{
				return this.m_Version;
			}
		}

		public VisualNodeHandle(int id, int version)
		{
			this.m_Id = id;
			this.m_Version = version;
		}

		public static bool operator ==(in VisualNodeHandle lhs, in VisualNodeHandle rhs)
		{
			return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
		}

		public static bool operator !=(in VisualNodeHandle lhs, in VisualNodeHandle rhs)
		{
			return !((in lhs) == (in rhs));
		}

		public bool Equals(VisualNodeHandle other)
		{
			return other.Id == this.Id && other.Version == this.Version;
		}

		public override string ToString()
		{
			return "VisualNodeHandle(" + (((in this) == (in VisualNodeHandle.Null)) ? "Null" : string.Format("{0}:{1}", this.Id, this.Version)) + ")";
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is VisualNodeHandle)
			{
				VisualNodeHandle visualNodeHandle = (VisualNodeHandle)obj;
				flag = this.Equals(visualNodeHandle);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.Id, this.Version);
		}

		public static readonly VisualNodeHandle Null;

		private readonly int m_Id;

		private readonly int m_Version;
	}
}
