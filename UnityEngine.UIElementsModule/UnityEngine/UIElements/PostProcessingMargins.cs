using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct PostProcessingMargins
	{
		[CreateProperty]
		public float left
		{
			get
			{
				return this.m_Left;
			}
			set
			{
				this.m_Left = value;
			}
		}

		[CreateProperty]
		public float top
		{
			get
			{
				return this.m_Top;
			}
			set
			{
				this.m_Top = value;
			}
		}

		[CreateProperty]
		public float right
		{
			get
			{
				return this.m_Right;
			}
			set
			{
				this.m_Right = value;
			}
		}

		[CreateProperty]
		public float bottom
		{
			get
			{
				return this.m_Bottom;
			}
			set
			{
				this.m_Bottom = value;
			}
		}

		[DontCreateProperty]
		[SerializeField]
		private float m_Left;

		[DontCreateProperty]
		[SerializeField]
		private float m_Top;

		[DontCreateProperty]
		[SerializeField]
		private float m_Right;

		[SerializeField]
		[DontCreateProperty]
		private float m_Bottom;
	}
}
