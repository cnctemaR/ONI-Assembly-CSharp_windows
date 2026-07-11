using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>A helper class that contains static method to inquire status of Unity Cluster.</para>
	/// </summary>
	[NativeHeader("Modules/ClusterRenderer/ClusterNetwork.h")]
	public class ClusterNetwork
	{
		/// <summary>
		///   <para>Check whether the current instance is a master node in the cluster network.</para>
		/// </summary>
		public static extern bool isMasterOfCluster
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Check whether the current instance is disconnected from the cluster network.</para>
		/// </summary>
		public static extern bool isDisconnected
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>To acquire or set the node index of the current machine from the cluster network.</para>
		/// </summary>
		public static extern int nodeIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
