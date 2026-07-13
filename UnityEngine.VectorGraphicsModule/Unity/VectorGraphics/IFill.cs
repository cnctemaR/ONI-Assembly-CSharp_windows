using System;

namespace Unity.VectorGraphics
{
	public interface IFill
	{
		FillMode Mode { get; set; }

		float Opacity { get; set; }
	}
}
