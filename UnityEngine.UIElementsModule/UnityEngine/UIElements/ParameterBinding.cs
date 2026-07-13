using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct ParameterBinding
	{
		[CreateProperty]
		public int index
		{
			get
			{
				return this.m_Index;
			}
			set
			{
				this.m_Index = value;
			}
		}

		[CreateProperty]
		public string name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				this.m_Name = value;
			}
		}

		[DontCreateProperty]
		[SerializeField]
		private int m_Index;

		[SerializeField]
		[DontCreateProperty]
		private string m_Name;
	}
}
