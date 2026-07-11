using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Deprecated feature, no longer available</para>
	/// </summary>
	[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
	[ExcludeFromPreset]
	public sealed class ProceduralTexture : Texture
	{
		private ProceduralTexture()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public ProceduralOutputType GetProceduralOutputType()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		internal ProceduralMaterial GetProceduralMaterial()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool hasAlpha
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public TextureFormat format
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="blockWidth"></param>
		/// <param name="blockHeight"></param>
		public Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}
	}
}
