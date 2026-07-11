using System;

namespace UnityEngine
{
	public interface IExposedPropertyTable
	{
		/// <summary>
		///   <para>Assigns a value for an ExposedReference.</para>
		/// </summary>
		/// <param name="id">Identifier of the ExposedReference.</param>
		/// <param name="value">The value to assigned to the ExposedReference.</param>
		void SetReferenceValue(PropertyName id, Object value);

		Object GetReferenceValue(PropertyName id, out bool idValid);

		/// <summary>
		///   <para>Remove a value for the given reference.</para>
		/// </summary>
		/// <param name="id">Identifier of the ExposedReference.</param>
		void ClearReferenceValue(PropertyName id);
	}
}
