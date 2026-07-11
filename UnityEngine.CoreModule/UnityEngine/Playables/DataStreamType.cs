using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>Describes the type of information that flows in and out of a Playable. This also specifies that this Playable is connectable to others of the same type.</para>
	/// </summary>
	[Obsolete("DataStreamType is no longer required and will be removed in a future release.", false)]
	public enum DataStreamType
	{
		/// <summary>
		///   <para>Describes that the information flowing in and out of the Playable is of Animation type.</para>
		/// </summary>
		Animation,
		/// <summary>
		///   <para>Describes that the information flowing in and out of the Playable is of Audio type.</para>
		/// </summary>
		Audio,
		/// <summary>
		///   <para>Describes that the information flowing in and out of the Playable is of type Texture.</para>
		/// </summary>
		Texture,
		/// <summary>
		///   <para>Describes that the Playable does not have any particular type. This is use for Playables that execute script code, or that create their own playable graphs, such as the Sequence.</para>
		/// </summary>
		None
	}
}
