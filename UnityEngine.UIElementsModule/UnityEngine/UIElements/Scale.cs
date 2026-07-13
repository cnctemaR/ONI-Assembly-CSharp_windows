using System;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Scale : IEquatable<Scale>
	{
		public Scale(Vector2 scale)
		{
			this.m_Scale = new Vector3(scale.x, scale.y, 1f);
			this.m_IsNone = false;
		}

		public Scale(Vector3 scale)
		{
			this.m_Scale = scale;
			this.m_IsNone = false;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static Scale Initial()
		{
			return new Scale(Vector3.one);
		}

		public static Scale None()
		{
			Scale scale = Scale.Initial();
			scale.m_IsNone = true;
			return scale;
		}

		public Vector3 value
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		internal bool IsNone()
		{
			return this.m_IsNone;
		}

		public static implicit operator Scale(Vector2 scale)
		{
			return new Scale(scale);
		}

		public static implicit operator Scale(Vector3 scale)
		{
			return new Scale(scale);
		}

		public static bool operator ==(Scale lhs, Scale rhs)
		{
			return lhs.m_Scale == rhs.m_Scale;
		}

		public static bool operator !=(Scale lhs, Scale rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Scale other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is Scale)
			{
				Scale scale = (Scale)obj;
				flag = this.Equals(scale);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_Scale.GetHashCode() * 793;
		}

		public override string ToString()
		{
			return this.m_Scale.ToString();
		}

		[SerializeField]
		private Vector3 m_Scale;

		[SerializeField]
		private bool m_IsNone;

		internal class PropertyBag : ContainerPropertyBag<Scale>
		{
			public PropertyBag()
			{
				base.AddProperty<Vector3>(new Scale.PropertyBag.ValueProperty());
			}

			private class ValueProperty : Property<Scale, Vector3>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = false;

				public override Vector3 GetValue(ref Scale container)
				{
					return container.value;
				}

				public override void SetValue(ref Scale container, Vector3 value)
				{
					container.value = value;
				}
			}
		}
	}
}
