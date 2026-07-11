using System;
using System.Collections.Generic;

namespace UnityEngine.Animations
{
	public interface IConstraint
	{
		/// <summary>
		///   <para>The weight of the constraint component.</para>
		/// </summary>
		float weight { get; set; }

		/// <summary>
		///   <para>Activate or deactivate the constraint.</para>
		/// </summary>
		bool constraintActive { get; set; }

		/// <summary>
		///   <para>Lock or unlock the offset and position at rest.</para>
		/// </summary>
		bool locked { get; set; }

		/// <summary>
		///   <para>Gets the number of sources currently set on the component.</para>
		/// </summary>
		int sourceCount { get; }

		/// <summary>
		///   <para>Add a constraint source.</para>
		/// </summary>
		/// <param name="source">The source object and its weight.</param>
		/// <returns>
		///   <para>Returns the index of the added source.</para>
		/// </returns>
		int AddSource(ConstraintSource source);

		/// <summary>
		///   <para>Removes a source from the component.</para>
		/// </summary>
		/// <param name="index">The index of the source to remove.</param>
		void RemoveSource(int index);

		/// <summary>
		///   <para>Gets a constraint source by index.</para>
		/// </summary>
		/// <param name="index">The index of the source.</param>
		/// <returns>
		///   <para>The source object and its weight.</para>
		/// </returns>
		ConstraintSource GetSource(int index);

		/// <summary>
		///   <para>Sets a source at a specified index.</para>
		/// </summary>
		/// <param name="index">The index of the source to set.</param>
		/// <param name="source">The source object and its weight.</param>
		void SetSource(int index, ConstraintSource source);

		void GetSources(List<ConstraintSource> sources);

		void SetSources(List<ConstraintSource> sources);
	}
}
