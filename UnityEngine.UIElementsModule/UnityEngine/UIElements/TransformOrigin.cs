using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct TransformOrigin : IEquatable<TransformOrigin>
	{
		public TransformOrigin(Length x, Length y, float z)
		{
			this.m_X = x;
			this.m_Y = y;
			this.m_Z = z;
		}

		public TransformOrigin(Length x, Length y)
		{
			this = new TransformOrigin(x, y, 0f);
		}

		internal TransformOrigin(Vector3 vector)
		{
			this = new TransformOrigin(vector.x, vector.y, vector.z);
		}

		public static TransformOrigin Initial()
		{
			return new TransformOrigin(Length.Percent(50f), Length.Percent(50f), 0f);
		}

		public Length x
		{
			get
			{
				return this.m_X;
			}
			set
			{
				this.m_X = value;
			}
		}

		public Length y
		{
			get
			{
				return this.m_Y;
			}
			set
			{
				this.m_Y = value;
			}
		}

		public float z
		{
			get
			{
				return this.m_Z;
			}
			set
			{
				this.m_Z = value;
			}
		}

		public static bool operator ==(TransformOrigin lhs, TransformOrigin rhs)
		{
			return lhs.m_X == rhs.m_X && lhs.m_Y == rhs.m_Y && lhs.m_Z == rhs.m_Z;
		}

		public static bool operator !=(TransformOrigin lhs, TransformOrigin rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(TransformOrigin other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is TransformOrigin)
			{
				TransformOrigin transformOrigin = (TransformOrigin)obj;
				flag = this.Equals(transformOrigin);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_X.GetHashCode() * 793) ^ (this.m_Y.GetHashCode() * 791) ^ (this.m_Z.GetHashCode() * 571);
		}

		public override string ToString()
		{
			string text = this.m_Z.ToString(CultureInfo.InvariantCulture.NumberFormat);
			return string.Concat(new string[]
			{
				this.m_X.ToString(),
				" ",
				this.m_Y.ToString(),
				" ",
				text
			});
		}

		[SerializeField]
		private Length m_X;

		[SerializeField]
		private Length m_Y;

		[SerializeField]
		private float m_Z;

		internal class PropertyBag : ContainerPropertyBag<TransformOrigin>
		{
			public PropertyBag()
			{
				base.AddProperty<Length>(new TransformOrigin.PropertyBag.XProperty());
				base.AddProperty<Length>(new TransformOrigin.PropertyBag.YProperty());
				base.AddProperty<float>(new TransformOrigin.PropertyBag.ZProperty());
			}

			private class XProperty : Property<TransformOrigin, Length>
			{
				public override string Name { get; } = "x";

				public override bool IsReadOnly { get; } = false;

				public override Length GetValue(ref TransformOrigin container)
				{
					return container.x;
				}

				public override void SetValue(ref TransformOrigin container, Length value)
				{
					container.x = value;
				}
			}

			private class YProperty : Property<TransformOrigin, Length>
			{
				public override string Name { get; } = "y";

				public override bool IsReadOnly { get; } = false;

				public override Length GetValue(ref TransformOrigin container)
				{
					return container.y;
				}

				public override void SetValue(ref TransformOrigin container, Length value)
				{
					container.y = value;
				}
			}

			private class ZProperty : Property<TransformOrigin, float>
			{
				public override string Name { get; } = "z";

				public override bool IsReadOnly { get; } = false;

				public override float GetValue(ref TransformOrigin container)
				{
					return container.z;
				}

				public override void SetValue(ref TransformOrigin container, float value)
				{
					container.z = value;
				}
			}
		}
	}
}
