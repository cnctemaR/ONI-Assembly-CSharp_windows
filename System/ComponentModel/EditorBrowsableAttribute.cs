using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
	public sealed class EditorBrowsableAttribute : Attribute
	{
		public EditorBrowsableAttribute()
		{
			this.state = EditorBrowsableState.Always;
		}

		public EditorBrowsableAttribute(EditorBrowsableState state)
		{
			this.state = state;
		}

		public EditorBrowsableState State
		{
			get
			{
				return this.state;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is EditorBrowsableAttribute && (obj == this || ((EditorBrowsableAttribute)obj).State == this.state);
		}

		public override int GetHashCode()
		{
			return this.state.GetHashCode();
		}

		private EditorBrowsableState state;
	}
}
