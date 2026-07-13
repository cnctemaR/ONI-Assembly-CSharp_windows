using System;

namespace UnityEngine.UIElements.Layout
{
	internal readonly struct LayoutHandle
	{
		public static LayoutHandle Undefined
		{
			get
			{
				return default(LayoutHandle);
			}
		}

		internal LayoutHandle(int index, int version)
		{
			this.Index = index;
			this.Version = version;
		}

		public bool IsUndefined
		{
			get
			{
				return this.Equals(LayoutHandle.Undefined);
			}
		}

		public bool Equals(LayoutHandle other)
		{
			return this.Index == other.Index && this.Version == other.Version;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is LayoutHandle)
			{
				LayoutHandle layoutHandle = (LayoutHandle)obj;
				flag = this.Equals(layoutHandle);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.Index * 397) ^ this.Version;
		}

		public readonly int Index;

		public readonly int Version;
	}
}
