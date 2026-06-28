using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true)]
	public struct NetworkMessageInfo
	{
		public double timestamp
		{
			get
			{
				return this.m_TimeStamp;
			}
		}

		public NetworkPlayer sender
		{
			get
			{
				return this.m_Sender;
			}
		}

		public NetworkView networkView
		{
			get
			{
				NetworkView networkView;
				if (this.m_ViewID == NetworkViewID.unassigned)
				{
					Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
					networkView = this.NullNetworkView();
				}
				else
				{
					networkView = NetworkView.Find(this.m_ViewID);
				}
				return networkView;
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern NetworkView NullNetworkView();

		private double m_TimeStamp;

		private NetworkPlayer m_Sender;

		private NetworkViewID m_ViewID;
	}
}
