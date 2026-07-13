using System;

namespace UnityEngine.UIElements
{
	public struct FilterPassContext
	{
		public FilterFunction filterFunction { readonly get; internal set; }

		public PostProcessingPass postProcessingPass
		{
			get
			{
				return this.filterFunction.GetDefinition().passes[this.filterPassIndex];
			}
		}

		public int filterPassIndex { readonly get; internal set; }

		public bool readsGamma { readonly get; internal set; }

		public bool writesGamma { readonly get; internal set; }
	}
}
