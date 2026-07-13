using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct FilterParameterDeclaration
	{
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

		[CreateProperty]
		public FilterParameter interpolationDefaultValue
		{
			get
			{
				return this.m_InterpolationDefaultValue;
			}
			set
			{
				this.m_InterpolationDefaultValue = value;
			}
		}

		[SerializeField]
		[DontCreateProperty]
		private string m_Name;

		[SerializeField]
		[DontCreateProperty]
		private FilterParameter m_InterpolationDefaultValue;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal FilterParameter defaultValue;
	}
}
