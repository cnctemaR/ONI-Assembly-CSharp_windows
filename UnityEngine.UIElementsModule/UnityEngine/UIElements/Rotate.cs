using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Rotate : IEquatable<Rotate>
	{
		public Rotate(Angle angle, Vector3 axis)
		{
			this.m_Angle = angle;
			this.m_Axis = axis;
			this.m_IsNone = false;
		}

		public Rotate(Angle angle)
		{
			this.m_Angle = angle;
			this.m_Axis = Vector3.forward;
			this.m_IsNone = false;
		}

		public Rotate(Quaternion quaternion)
		{
			float num;
			Vector3 vector;
			quaternion.ToAngleAxis(out num, out vector);
			this.m_Angle = num;
			this.m_Axis = vector;
			this.m_IsNone = false;
		}

		internal static Rotate Initial()
		{
			return new Rotate(0f);
		}

		public static Rotate None()
		{
			Rotate rotate = Rotate.Initial();
			rotate.m_IsNone = true;
			return rotate;
		}

		public Angle angle
		{
			get
			{
				return this.m_Angle;
			}
			set
			{
				this.m_Angle = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal Vector3 axis
		{
			get
			{
				return this.m_Axis;
			}
			set
			{
				this.m_Axis = value;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal bool IsNone()
		{
			return this.m_IsNone;
		}

		public static bool operator ==(Rotate lhs, Rotate rhs)
		{
			return lhs.m_Angle == rhs.m_Angle && lhs.m_Axis == rhs.m_Axis && lhs.m_IsNone == rhs.m_IsNone;
		}

		public static bool operator !=(Rotate lhs, Rotate rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Rotate other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is Rotate)
			{
				Rotate rotate = (Rotate)obj;
				flag = this.Equals(rotate);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_Angle.GetHashCode() * 793) ^ (this.m_Axis.GetHashCode() * 791) ^ (this.m_IsNone.GetHashCode() * 197);
		}

		public override string ToString()
		{
			return this.m_Angle.ToString() + " " + this.m_Axis.ToString();
		}

		internal Quaternion ToQuaternion()
		{
			return Quaternion.AngleAxis(this.m_Angle.ToDegrees(), this.m_Axis);
		}

		public static implicit operator Rotate(Quaternion v)
		{
			return new Rotate(v);
		}

		public static implicit operator Rotate(Angle a)
		{
			return new Rotate(a);
		}

		[SerializeField]
		private Angle m_Angle;

		[SerializeField]
		private Vector3 m_Axis;

		[SerializeField]
		private bool m_IsNone;

		internal class PropertyBag : ContainerPropertyBag<Rotate>
		{
			public PropertyBag()
			{
				base.AddProperty<Angle>(new Rotate.PropertyBag.AngleProperty());
				base.AddProperty<Vector3>(new Rotate.PropertyBag.AxisProperty());
			}

			private class AngleProperty : Property<Rotate, Angle>
			{
				public override string Name { get; } = "angle";

				public override bool IsReadOnly { get; } = false;

				public override Angle GetValue(ref Rotate container)
				{
					return container.angle;
				}

				public override void SetValue(ref Rotate container, Angle value)
				{
					container.angle = value;
				}
			}

			private class AxisProperty : Property<Rotate, Vector3>
			{
				public override string Name { get; } = "axis";

				public override bool IsReadOnly { get; } = false;

				public override Vector3 GetValue(ref Rotate container)
				{
					return container.axis;
				}

				public override void SetValue(ref Rotate container, Vector3 value)
				{
					container.axis = value;
				}
			}
		}
	}
}
