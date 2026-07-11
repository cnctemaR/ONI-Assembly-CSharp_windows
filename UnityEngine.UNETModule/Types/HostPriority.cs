using System;
using System.ComponentModel;

namespace UnityEngine.Networking.Types
{
	/// <summary>
	///   <para>An Enum representing the priority of a client in a match, starting at 0 and increasing.</para>
	/// </summary>
	[DefaultValue(HostPriority.Invalid)]
	public enum HostPriority
	{
		/// <summary>
		///   <para>The Invalid case for a HostPriority. An Invalid host priority is not a valid host.</para>
		/// </summary>
		Invalid = 2147483647
	}
}
