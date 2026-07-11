using System;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Built-in shader modes used by Rendering.GraphicsSettings.</para>
	/// </summary>
	public enum BuiltinShaderMode
	{
		/// <summary>
		///   <para>Don't use any shader, effectively disabling the functionality.</para>
		/// </summary>
		Disabled,
		/// <summary>
		///   <para>Use built-in shader (default).</para>
		/// </summary>
		UseBuiltin,
		/// <summary>
		///   <para>Use custom shader instead of built-in one.</para>
		/// </summary>
		UseCustom
	}
}
