using System;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	/// <summary>
	///   <para>Deprecated feature, no longer available</para>
	/// </summary>
	[Obsolete("Built-in support for Substance Designer materials has been removed from Unity. To continue using Substance Designer materials, you will need to install Allegorithmic's external importer from the Asset Store.", true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ProceduralPropertyDescription
	{
		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string name;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string label;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string group;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public ProceduralPropertyType type;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public bool hasRange;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public float minimum;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public float maximum;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public float step;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string[] enumOptions;

		/// <summary>
		///   <para>Deprecated feature, no longer available</para>
		/// </summary>
		public string[] componentLabels;
	}
}
