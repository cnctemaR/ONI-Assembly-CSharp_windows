using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualPanelHandle.h")]
	internal readonly struct VisualPanelHandle : IEquatable<VisualPanelHandle>
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

		public VisualPanelHandle(int id, int version)
		{
			this.m_Id = id;
			this.m_Version = version;
		}

		public static bool operator ==(in VisualPanelHandle lhs, in VisualPanelHandle rhs)
		{
			return lhs.Id == rhs.Id && lhs.Version == rhs.Version;
		}

		public static bool operator !=(in VisualPanelHandle lhs, in VisualPanelHandle rhs)
		{
			return !((in lhs) == (in rhs));
		}

		public bool Equals(VisualPanelHandle other)
		{
			return other.Id == this.Id && other.Version == this.Version;
		}

		public override string ToString()
		{
			return "VisualPanelHandle(" + (((in this) == (in VisualPanelHandle.Null)) ? "Null" : string.Format("{0}:{1}", this.Id, this.Version)) + ")";
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is VisualPanelHandle)
			{
				VisualPanelHandle visualPanelHandle = (VisualPanelHandle)obj;
				flag = this.Equals(visualPanelHandle);
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

		public static readonly VisualPanelHandle Null;

		private readonly int m_Id;

		private readonly int m_Version;
	}
}
