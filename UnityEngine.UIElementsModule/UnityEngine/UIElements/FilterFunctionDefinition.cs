using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[HelpURL("ui-systems/custom-filters")]
	[Serializable]
	public sealed class FilterFunctionDefinition : ScriptableObject
	{
		[CreateProperty]
		public string filterName
		{
			get
			{
				return this.m_FilterName;
			}
			set
			{
				this.m_FilterName = value;
			}
		}

		[CreateProperty]
		public FilterParameterDeclaration[] parameters
		{
			get
			{
				return this.m_Parameters;
			}
			set
			{
				this.m_Parameters = value;
			}
		}

		[CreateProperty]
		public PostProcessingPass[] passes
		{
			get
			{
				return this.m_Passes;
			}
			set
			{
				this.m_Passes = value;
			}
		}

		[SerializeField]
		[DontCreateProperty]
		private string m_FilterName;

		[SerializeField]
		[DontCreateProperty]
		private FilterParameterDeclaration[] m_Parameters;

		[DontCreateProperty]
		[SerializeField]
		private PostProcessingPass[] m_Passes;
	}
}
