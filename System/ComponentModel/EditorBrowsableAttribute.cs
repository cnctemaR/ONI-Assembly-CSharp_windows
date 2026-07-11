using System;

namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
	public sealed class EditorBrowsableAttribute : Attribute
	{
		public EditorBrowsableAttribute(EditorBrowsableState state)
		{
			this.browsableState = state;
		}

		public EditorBrowsableAttribute()
			: this(EditorBrowsableState.Always)
		{
		}

		public EditorBrowsableState State
		{
			get
			{
				return this.browsableState;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			EditorBrowsableAttribute editorBrowsableAttribute = obj as EditorBrowsableAttribute;
			return editorBrowsableAttribute != null && editorBrowsableAttribute.browsableState == this.browsableState;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private EditorBrowsableState browsableState;
	}
}
