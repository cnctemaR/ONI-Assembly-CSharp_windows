using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Asynchronous load request from the Resources bundle.</para>
	/// </summary>
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class ResourceRequest : AsyncOperation
	{
		/// <summary>
		///   <para>Asset object being loaded (Read Only).</para>
		/// </summary>
		public Object asset
		{
			get
			{
				return Resources.Load(this.m_Path, this.m_Type);
			}
		}

		internal string m_Path;

		internal Type m_Type;
	}
}
