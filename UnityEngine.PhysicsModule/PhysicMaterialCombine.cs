using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Describes how physics materials of the colliding objects are combined.
	///
	/// The friction force as well as the residual bounce impulse are applied symmertrically to both of the colliders in contact, so each pair of overlapping colliders must have a single set of friction and bouciness settings. However, one can set arbitrary physics materials to any colliders. For that particular reason, there is a mechanism that allows the combination of two different sets of properties that correspond to each of the colliders in contact into one set to be used in the solver.
	///
	/// Specifying Average, Maximum, Minimum or Multiply as the physics material combine mode, you directly set the function that is used to combine the settings corresponding to the two overlapping colliders into one set of settings that can be used to apply the material effect.
	///
	/// Note that there is a special case when the two overlapping colliders have physics materials with different combine modes set. In this particular case, the function that has the highest priority is used. The priority order is as follows: Average &lt; Minimum &lt; Multiply &lt; Maximum. For example, if one material has Average set but the other one has Maximum, then the combine function to be used is Maximum, since it has higher priority.</para>
	/// </summary>
	public enum PhysicMaterialCombine
	{
		/// <summary>
		///   <para>Averages the friction/bounce of the two colliding materials.</para>
		/// </summary>
		Average,
		/// <summary>
		///   <para>Uses the smaller friction/bounce of the two colliding materials.</para>
		/// </summary>
		Minimum = 2,
		/// <summary>
		///   <para>Multiplies the friction/bounce of the two colliding materials.</para>
		/// </summary>
		Multiply = 1,
		/// <summary>
		///   <para>Uses the larger friction/bounce of the two colliding materials.</para>
		/// </summary>
		Maximum = 3
	}
}
