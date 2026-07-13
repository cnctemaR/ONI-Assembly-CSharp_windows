using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[Serializable]
	internal struct StyleValueHandle : IEquatable<StyleValueHandle>
	{
		public StyleValueType valueType
		{
			get
			{
				return this.m_ValueType;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			internal set
			{
				this.m_ValueType = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal StyleValueHandle(int valueIndex, StyleValueType valueType)
		{
			this.valueIndex = valueIndex;
			this.m_ValueType = valueType;
		}

		public bool IsVarFunction()
		{
			return this.valueType == StyleValueType.Function && this.valueIndex == 1;
		}

		public bool Equals(StyleValueHandle other)
		{
			return this.m_ValueType == other.m_ValueType && this.valueIndex == other.valueIndex;
		}

		public static bool operator ==(StyleValueHandle lhs, StyleValueHandle rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(StyleValueHandle lhs, StyleValueHandle rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is StyleValueHandle)
			{
				StyleValueHandle styleValueHandle = (StyleValueHandle)obj;
				flag = this.Equals(styleValueHandle);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>((int)this.m_ValueType, this.valueIndex);
		}

		[SerializeField]
		private StyleValueType m_ValueType;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[SerializeField]
		internal int valueIndex;
	}
}
