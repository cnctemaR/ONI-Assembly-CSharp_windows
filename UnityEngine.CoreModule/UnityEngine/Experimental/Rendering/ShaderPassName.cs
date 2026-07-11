using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Shader pass name identifier.</para>
	/// </summary>
	public struct ShaderPassName
	{
		/// <summary>
		///   <para>Create shader pass name identifier.</para>
		/// </summary>
		/// <param name="name">Pass name.</param>
		public ShaderPassName(string name)
		{
			this.m_NameIndex = ShaderPassName.Init(name);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Init(string name);

		internal int nameIndex
		{
			get
			{
				return this.m_NameIndex;
			}
		}

		private int m_NameIndex;
	}
}
