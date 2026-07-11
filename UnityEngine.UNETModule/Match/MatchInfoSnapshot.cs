using System;
using System.Collections.Generic;
using UnityEngine.Networking.Types;

namespace UnityEngine.Networking.Match
{
	/// <summary>
	///   <para>A class describing the match information as a snapshot at the time the request was processed on the MatchMaker.</para>
	/// </summary>
	public class MatchInfoSnapshot
	{
		public MatchInfoSnapshot()
		{
		}

		internal MatchInfoSnapshot(MatchDesc matchDesc)
		{
			this.networkId = (NetworkID)matchDesc.networkId;
			this.hostNodeId = matchDesc.hostNodeId;
			this.name = matchDesc.name;
			this.averageEloScore = matchDesc.averageEloScore;
			this.maxSize = matchDesc.maxSize;
			this.currentSize = matchDesc.currentSize;
			this.isPrivate = matchDesc.isPrivate;
			this.matchAttributes = matchDesc.matchAttributes;
			this.directConnectInfos = new List<MatchInfoSnapshot.MatchInfoDirectConnectSnapshot>();
			foreach (MatchDirectConnectInfo matchDirectConnectInfo in matchDesc.directConnectInfos)
			{
				this.directConnectInfos.Add(new MatchInfoSnapshot.MatchInfoDirectConnectSnapshot(matchDirectConnectInfo));
			}
		}

		/// <summary>
		///   <para>The network ID for this match.</para>
		/// </summary>
		public NetworkID networkId { get; private set; }

		/// <summary>
		///   <para>The NodeID of the host for this match.</para>
		/// </summary>
		public NodeID hostNodeId { get; private set; }

		/// <summary>
		///   <para>The text name for this match.</para>
		/// </summary>
		public string name { get; private set; }

		/// <summary>
		///   <para>The average Elo score of the match.</para>
		/// </summary>
		public int averageEloScore { get; private set; }

		/// <summary>
		///   <para>The maximum number of players this match can grow to.</para>
		/// </summary>
		public int maxSize { get; private set; }

		/// <summary>
		///   <para>The current number of players in the match.</para>
		/// </summary>
		public int currentSize { get; private set; }

		/// <summary>
		///   <para>Describes if the match is private. Private matches are unlisted in ListMatch results.</para>
		/// </summary>
		public bool isPrivate { get; private set; }

		/// <summary>
		///   <para>The collection of match attributes on this match.</para>
		/// </summary>
		public Dictionary<string, long> matchAttributes { get; private set; }

		/// <summary>
		///   <para>The collection of direct connect info classes describing direct connection information supplied to the MatchMaker.</para>
		/// </summary>
		public List<MatchInfoSnapshot.MatchInfoDirectConnectSnapshot> directConnectInfos { get; private set; }

		/// <summary>
		///   <para>A class describing one member of a match and what direct connect information other clients have supplied.</para>
		/// </summary>
		public class MatchInfoDirectConnectSnapshot
		{
			public MatchInfoDirectConnectSnapshot()
			{
			}

			internal MatchInfoDirectConnectSnapshot(MatchDirectConnectInfo matchDirectConnectInfo)
			{
				this.nodeId = matchDirectConnectInfo.nodeId;
				this.publicAddress = matchDirectConnectInfo.publicAddress;
				this.privateAddress = matchDirectConnectInfo.privateAddress;
				this.hostPriority = matchDirectConnectInfo.hostPriority;
			}

			/// <summary>
			///   <para>NodeID of the match member this info refers to.</para>
			/// </summary>
			public NodeID nodeId { get; private set; }

			/// <summary>
			///   <para>The public network address supplied for this direct connect info.</para>
			/// </summary>
			public string publicAddress { get; private set; }

			/// <summary>
			///   <para>The private network address supplied for this direct connect info.</para>
			/// </summary>
			public string privateAddress { get; private set; }

			/// <summary>
			///   <para>The host priority for this direct connect info. Host priority describes the order in which this match member occurs in the list of clients attached to a match.</para>
			/// </summary>
			public HostPriority hostPriority { get; private set; }
		}
	}
}
