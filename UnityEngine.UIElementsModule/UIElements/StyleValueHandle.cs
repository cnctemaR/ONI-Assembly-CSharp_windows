using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal struct StyleValueHandle
	{
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

		internal StyleValueHandle(int valueIndex, StyleValueType valueType)
		{
			this.valueIndex = valueIndex;
			this.m_ValueType = valueType;
		}

		[SerializeField]
		private StyleValueType m_ValueType;

		[SerializeField]
		internal int valueIndex;
	}
}
