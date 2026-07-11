using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	public interface ISerializationCallbackReceiver
	{
		/// <summary>
		///   <para>Implement this method to receive a callback before Unity serializes your object.</para>
		/// </summary>
		[RequiredByNativeCode]
		void OnBeforeSerialize();

		/// <summary>
		///   <para>Implement this method to receive a callback after Unity deserializes your object.</para>
		/// </summary>
		[RequiredByNativeCode]
		void OnAfterDeserialize();
	}
}
