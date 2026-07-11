using System;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Link type specifier.</para>
	/// </summary>
	[MovedFrom("UnityEngine")]
	public enum OffMeshLinkType
	{
		/// <summary>
		///   <para>Manually specified type of link.</para>
		/// </summary>
		LinkTypeManual,
		/// <summary>
		///   <para>Vertical drop.</para>
		/// </summary>
		LinkTypeDropDown,
		/// <summary>
		///   <para>Horizontal jump.</para>
		/// </summary>
		LinkTypeJumpAcross
	}
}
