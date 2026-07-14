using System;

public static class RenderingConstants
{
	public static class RefNumbers
	{
		public const byte SolidNaturalTiles = 1;

		public const byte RocketPath = 2;

		public const byte Vistas_StartingREF_Value = 3;

		public const byte Vistas_RefCount = 10;

		public enum VistasREF : byte
		{
			BeachVista = 3,
			ReefVista,
			KelpVista,
			AbyssVista
		}
	}
}
