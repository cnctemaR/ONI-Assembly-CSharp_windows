using System;
using UnityEngine.Bindings;

namespace UnityEngine.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[Serializable]
	internal struct StyleValueHandle
	{
		internal StyleValueHandle(int valueIndex, StyleValueType valueType)
		{
			this.valueIndex = valueIndex;
			this.m_ValueType = valueType;
		}

		public StyleValueType valueType
		{
			get
			{
				return this.m_ValueType;
			}
			internal set
			{
				this.m_ValueType = value;
			}
		}

		[SerializeField]
		private StyleValueType m_ValueType;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal int valueIndex;
	}
}
