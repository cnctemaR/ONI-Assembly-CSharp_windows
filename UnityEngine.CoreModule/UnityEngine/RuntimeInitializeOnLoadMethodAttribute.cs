using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Allow a runtime class method to be initialized when a game is loaded at runtime
	/// without action from the user.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RuntimeInitializeOnLoadMethodAttribute : PreserveAttribute
	{
		/// <summary>
		///   <para>Creation of the runtime class used when scenes are loaded.</para>
		/// </summary>
		/// <param name="loadType">Determine whether methods are called before or after the
		///   scene is loaded.</param>
		public RuntimeInitializeOnLoadMethodAttribute()
		{
			this.loadType = RuntimeInitializeLoadType.AfterSceneLoad;
		}

		/// <summary>
		///   <para>Creation of the runtime class used when scenes are loaded.</para>
		/// </summary>
		/// <param name="loadType">Determine whether methods are called before or after the
		///   scene is loaded.</param>
		public RuntimeInitializeOnLoadMethodAttribute(RuntimeInitializeLoadType loadType)
		{
			this.loadType = loadType;
		}

		/// <summary>
		///   <para>Set RuntimeInitializeOnLoadMethod type.</para>
		/// </summary>
		public RuntimeInitializeLoadType loadType { get; private set; }
	}
}
