using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Deprecated feature, no longer available</para>
	/// </summary>
	[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
	public enum ProceduralLoadingBehavior
	{
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		DoNothing,
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		Generate,
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		BakeAndKeep,
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		BakeAndDiscard,
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		Cache,
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		DoNothingAndCache
	}
}
