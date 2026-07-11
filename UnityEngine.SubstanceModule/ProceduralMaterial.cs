using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Deprecated feature, no longer available</para>
	/// </summary>
	[ExcludeFromPreset]
	[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
	public sealed class ProceduralMaterial : Material
	{
		internal ProceduralMaterial()
			: base(null)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		private static void FeatureRemoved()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public ProceduralPropertyDescription[] GetProceduralPropertyDescriptions()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public bool HasProceduralProperty(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public bool GetProceduralBoolean(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public bool IsProceduralPropertyVisible(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralBoolean(string inputName, bool value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public float GetProceduralFloat(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralFloat(string inputName, float value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public Vector4 GetProceduralVector(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralVector(string inputName, Vector4 value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public Color GetProceduralColor(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralColor(string inputName, Color value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public int GetProceduralEnum(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralEnum(string inputName, int value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public Texture2D GetProceduralTexture(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralTexture(string inputName, Texture2D value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public string GetProceduralString(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void SetProceduralString(string inputName, string value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		public bool IsProceduralPropertyCached(string inputName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="inputName"></param>
		/// <param name="value"></param>
		public void CacheProceduralProperty(string inputName, bool value)
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public void ClearCache()
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public ProceduralCacheSize cacheSize
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public int animationUpdateRate
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public void RebuildTextures()
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Triggers an immediate (synchronous) rebuild of this ProceduralMaterial's dirty textures.</para>
		/// </summary>
		public void RebuildTexturesImmediately()
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool isProcessing
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public static void StopRebuilds()
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool isCachedDataAvailable
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool isLoadTimeGenerated
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public ProceduralLoadingBehavior loadingBehavior
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public static bool isSupported
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public static ProceduralProcessorUsage substanceProcessorUsage
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string preset
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public Texture[] GetGeneratedTextures()
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		/// <param name="textureName"></param>
		public ProceduralTexture GetGeneratedTexture(string textureName)
		{
			throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool isReadable
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
			set
			{
				ProceduralMaterial.FeatureRemoved();
			}
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public void FreezeAndReleaseSourceData()
		{
			ProceduralMaterial.FeatureRemoved();
		}

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool isFrozen
		{
			get
			{
				throw new Exception("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.");
			}
		}
	}
}
