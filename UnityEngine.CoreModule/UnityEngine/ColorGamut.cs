using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Represents a color gamut.</para>
	/// </summary>
	[UsedByNativeCode]
	public enum ColorGamut
	{
		/// <summary>
		///   <para>sRGB color gamut.</para>
		/// </summary>
		sRGB,
		/// <summary>
		///   <para>Rec. 709 color gamut.</para>
		/// </summary>
		Rec709,
		/// <summary>
		///   <para>Rec. 2020 color gamut.</para>
		/// </summary>
		Rec2020,
		/// <summary>
		///   <para>Display-P3 color gamut.</para>
		/// </summary>
		DisplayP3,
		/// <summary>
		///   <para>HDR10 high dynamic range color gamut.</para>
		/// </summary>
		HDR10,
		/// <summary>
		///   <para>DolbyHDR high dynamic range color gamut.</para>
		/// </summary>
		DolbyHDR
	}
}
