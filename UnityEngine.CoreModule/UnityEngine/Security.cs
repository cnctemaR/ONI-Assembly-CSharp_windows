using System;
using System.Reflection;
using UnityEngine.Internal;

namespace UnityEngine
{
	/// <summary>
	///   <para>Webplayer security related class. Not supported from 5.4.0 onwards.</para>
	/// </summary>
	public sealed class Security
	{
		/// <summary>
		///   <para>Loads an assembly and checks that it is allowed to be used in the webplayer. (Web Player is no Longer Supported).</para>
		/// </summary>
		/// <param name="assemblyData">Assembly to verify.</param>
		/// <param name="authorizationKey">Public key used to verify assembly.</param>
		/// <returns>
		///   <para>Loaded, verified, assembly, or null if the assembly cannot be verfied.</para>
		/// </returns>
		[Obsolete("This was an internal method which is no longer used", true)]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData, string authorizationKey)
		{
			return null;
		}

		/// <summary>
		///   <para>Loads an assembly and checks that it is allowed to be used in the webplayer. (Web Player is no Longer Supported).</para>
		/// </summary>
		/// <param name="assemblyData">Assembly to verify.</param>
		/// <param name="authorizationKey">Public key used to verify assembly.</param>
		/// <returns>
		///   <para>Loaded, verified, assembly, or null if the assembly cannot be verfied.</para>
		/// </returns>
		[Obsolete("This was an internal method which is no longer used", true)]
		public static Assembly LoadAndVerifyAssembly(byte[] assemblyData)
		{
			return null;
		}

		/// <summary>
		///   <para>Prefetch the webplayer socket security policy from a non-default port number.</para>
		/// </summary>
		/// <param name="ip">IP address of server.</param>
		/// <param name="atPort">Port from where socket policy is read.</param>
		/// <param name="timeout">Time to wait for response.</param>
		[Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity.", true)]
		[ExcludeFromDocs]
		public static bool PrefetchSocketPolicy(string ip, int atPort)
		{
			int num = 3000;
			return Security.PrefetchSocketPolicy(ip, atPort, num);
		}

		/// <summary>
		///   <para>Prefetch the webplayer socket security policy from a non-default port number.</para>
		/// </summary>
		/// <param name="ip">IP address of server.</param>
		/// <param name="atPort">Port from where socket policy is read.</param>
		/// <param name="timeout">Time to wait for response.</param>
		[Obsolete("Security.PrefetchSocketPolicy is no longer supported, since the Unity Web Player is no longer supported by Unity.", true)]
		public static bool PrefetchSocketPolicy(string ip, int atPort, [DefaultValue("3000")] int timeout)
		{
			return false;
		}
	}
}
