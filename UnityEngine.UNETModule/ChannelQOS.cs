using System;

namespace UnityEngine.Networking
{
	/// <summary>
	///   <para>Defines parameters of channels.</para>
	/// </summary>
	[Serializable]
	public class ChannelQOS
	{
		/// <summary>
		///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
		/// </summary>
		/// <param name="value">Requested type of quality of service (default Unreliable).</param>
		/// <param name="channel">Copy constructor.</param>
		public ChannelQOS(QosType value)
		{
			this.m_Type = value;
			this.m_BelongsSharedOrderChannel = false;
		}

		/// <summary>
		///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
		/// </summary>
		/// <param name="value">Requested type of quality of service (default Unreliable).</param>
		/// <param name="channel">Copy constructor.</param>
		public ChannelQOS()
		{
			this.m_Type = QosType.Unreliable;
			this.m_BelongsSharedOrderChannel = false;
		}

		/// <summary>
		///   <para>UnderlyingModel.MemDoc.MemDocModel.</para>
		/// </summary>
		/// <param name="value">Requested type of quality of service (default Unreliable).</param>
		/// <param name="channel">Copy constructor.</param>
		public ChannelQOS(ChannelQOS channel)
		{
			if (channel == null)
			{
				throw new NullReferenceException("channel is not defined");
			}
			this.m_Type = channel.m_Type;
			this.m_BelongsSharedOrderChannel = channel.m_BelongsSharedOrderChannel;
		}

		/// <summary>
		///   <para>Channel quality of service.</para>
		/// </summary>
		public QosType QOS
		{
			get
			{
				return this.m_Type;
			}
		}

		/// <summary>
		///   <para>Returns true if the channel belongs to a shared group.</para>
		/// </summary>
		public bool BelongsToSharedOrderChannel
		{
			get
			{
				return this.m_BelongsSharedOrderChannel;
			}
		}

		[SerializeField]
		internal QosType m_Type;

		[SerializeField]
		internal bool m_BelongsSharedOrderChannel;
	}
}
