using System;

namespace UnityEngine.UIElements
{
	internal interface IPanelRenderer
	{
		bool forceGammaRendering { get; set; }

		uint vertexBudget { get; set; }

		TextureSlotCount textureSlotCount { get; set; }

		void Reset();

		void Render();
	}
}
