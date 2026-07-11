using System;
using UnityEngine.Bindings;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Struct that holds information regarding an output of a PlayableAsset.</para>
	/// </summary>
	public struct PlayableBinding
	{
		/// <summary>
		///   <para>The name of the output or input stream.</para>
		/// </summary>
		public string streamName
		{
			get
			{
				return this.m_StreamName;
			}
			set
			{
				this.m_StreamName = value;
			}
		}

		/// <summary>
		///   <para>A reference to a UnityEngine.Object that acts a key for this binding.</para>
		/// </summary>
		public Object sourceObject
		{
			get
			{
				return this.m_SourceObject;
			}
			set
			{
				this.m_SourceObject = value;
			}
		}

		/// <summary>
		///   <para>The type of target required by the PlayableOutput for this PlayableBinding.</para>
		/// </summary>
		public Type outputTargetType
		{
			get
			{
				return this.m_SourceBindingType;
			}
		}

		[Obsolete("sourceBindingType is no longer supported on PlayableBinding. Use outputBindingType instead to get the required output target type, and the appropriate binding create method (e.g. AnimationPlayableBinding.Create(name, key)) to create PlayableBindings", true)]
		public Type sourceBindingType
		{
			get
			{
				return this.m_SourceBindingType;
			}
			set
			{
			}
		}

		/// <summary>
		///   <para>The type of the output or input stream.</para>
		/// </summary>
		[Obsolete("streamType is no longer supported on PlayableBinding. Use the appropriate binding create method (e.g. AnimationPlayableBinding.Create(name, key)) instead.", true)]
		public DataStreamType streamType
		{
			get
			{
				return DataStreamType.None;
			}
			set
			{
			}
		}

		internal PlayableOutput CreateOutput(PlayableGraph graph)
		{
			PlayableOutput playableOutput;
			if (this.m_CreateOutputMethod != null)
			{
				playableOutput = this.m_CreateOutputMethod(graph, this.m_StreamName);
			}
			else
			{
				playableOutput = PlayableOutput.Null;
			}
			return playableOutput;
		}

		[VisibleToOtherModules]
		internal static PlayableBinding CreateInternal(string name, Object sourceObject, Type sourceType, PlayableBinding.CreateOutputMethod createFunction)
		{
			return new PlayableBinding
			{
				m_StreamName = name,
				m_SourceObject = sourceObject,
				m_SourceBindingType = sourceType,
				m_CreateOutputMethod = createFunction
			};
		}

		private string m_StreamName;

		private Object m_SourceObject;

		private Type m_SourceBindingType;

		private PlayableBinding.CreateOutputMethod m_CreateOutputMethod;

		/// <summary>
		///   <para>A constant to represent a PlayableAsset has no bindings.</para>
		/// </summary>
		public static readonly PlayableBinding[] None = new PlayableBinding[0];

		/// <summary>
		///   <para>The default duration used when a PlayableOutput has no fixed duration.</para>
		/// </summary>
		public static readonly double DefaultDuration = double.PositiveInfinity;

		[VisibleToOtherModules]
		internal delegate PlayableOutput CreateOutputMethod(PlayableGraph graph, string name);
	}
}
