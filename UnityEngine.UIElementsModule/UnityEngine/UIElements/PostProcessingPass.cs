using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct PostProcessingPass
	{
		[CreateProperty]
		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		[CreateProperty]
		public int passIndex
		{
			get
			{
				return this.m_PassIndex;
			}
			set
			{
				this.m_PassIndex = value;
			}
		}

		[CreateProperty]
		public ParameterBinding[] parameterBindings
		{
			get
			{
				return this.m_ParameterBindings;
			}
			set
			{
				this.m_ParameterBindings = value;
			}
		}

		internal PostProcessingMargins readMargins
		{
			get
			{
				return this.m_ReadMargins;
			}
			set
			{
				this.m_ReadMargins = value;
			}
		}

		[CreateProperty]
		public PostProcessingMargins writeMargins
		{
			get
			{
				return this.m_WriteMargins;
			}
			set
			{
				this.m_WriteMargins = value;
			}
		}

		[Obsolete("This property will be removed. Use applySettingsCallback instead, which provides additional contextual information.")]
		public PostProcessingPass.PrepareMaterialPropertyBlockDelegate prepareMaterialPropertyBlockCallback { readonly get; set; }

		public PostProcessingPass.ApplyFilterPassSettingsDelegate applySettingsCallback { readonly get; set; }

		public PostProcessingPass.ComputeRequiredMarginsDelegate computeRequiredReadMarginsCallback { readonly get; set; }

		public PostProcessingPass.ComputeRequiredMarginsDelegate computeRequiredWriteMarginsCallback { readonly get; set; }

		[DontCreateProperty]
		[SerializeField]
		private Material m_Material;

		[DontCreateProperty]
		[SerializeField]
		private int m_PassIndex;

		[DontCreateProperty]
		[SerializeField]
		private ParameterBinding[] m_ParameterBindings;

		[SerializeField]
		private PostProcessingMargins m_ReadMargins;

		[SerializeField]
		[DontCreateProperty]
		private PostProcessingMargins m_WriteMargins;

		[Obsolete("This delegate will be removed. Use ApplyFilterPassSettingsDelegate instead.")]
		public delegate void PrepareMaterialPropertyBlockDelegate(MaterialPropertyBlock mpb, FilterFunction func);

		public delegate void ApplyFilterPassSettingsDelegate(MaterialPropertyBlock mpb, FilterPassContext context);

		public delegate PostProcessingMargins ComputeRequiredMarginsDelegate(FilterFunction func);
	}
}
