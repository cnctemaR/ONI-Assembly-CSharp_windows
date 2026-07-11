using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Representation of a Position, and a Rotation in 3D Space</para>
	/// </summary>
	[Serializable]
	public struct Pose
	{
		public Pose(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public override string ToString()
		{
			return string.Format("({0}, {1})", this.position.ToString(), this.rotation.ToString());
		}

		public string ToString(string format)
		{
			return string.Format("({0}, {1})", this.position.ToString(format), this.rotation.ToString(format));
		}

		/// <summary>
		///   <para>Transforms the current pose into the local space of the provided pose.</para>
		/// </summary>
		/// <param name="lhs"></param>
		public Pose GetTransformedBy(Pose lhs)
		{
			return new Pose
			{
				position = lhs.position + lhs.rotation * this.position,
				rotation = lhs.rotation * this.rotation
			};
		}

		/// <summary>
		///   <para>Transforms the current pose into the local space of the provided pose.</para>
		/// </summary>
		/// <param name="lhs"></param>
		public Pose GetTransformedBy(Transform lhs)
		{
			return new Pose
			{
				position = lhs.TransformPoint(this.position),
				rotation = lhs.rotation * this.rotation
			};
		}

		/// <summary>
		///   <para>Returns the forward vector of the pose.</para>
		/// </summary>
		public Vector3 forward
		{
			get
			{
				return this.rotation * Vector3.forward;
			}
		}

		/// <summary>
		///   <para>Returns the right vector of the pose.</para>
		/// </summary>
		public Vector3 right
		{
			get
			{
				return this.rotation * Vector3.right;
			}
		}

		/// <summary>
		///   <para>Returns the up vector of the pose.</para>
		/// </summary>
		public Vector3 up
		{
			get
			{
				return this.rotation * Vector3.up;
			}
		}

		/// <summary>
		///   <para>Shorthand for pose which represents zero position, and an identity rotation.</para>
		/// </summary>
		public static Pose identity
		{
			get
			{
				return Pose.k_Identity;
			}
		}

		/// <summary>
		///   <para>The position component of the pose.</para>
		/// </summary>
		public Vector3 position;

		/// <summary>
		///   <para>The rotation component of the pose.</para>
		/// </summary>
		public Quaternion rotation;

		private static readonly Pose k_Identity = new Pose(Vector3.zero, Quaternion.identity);
	}
}
