using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeConditional("ENABLE_VR")]
	[NativeHeader("Modules/XR/Subsystems/Input/Public/XRInputDevices.h")]
	[RequiredByNativeCode]
	public struct InputFeatureUsage : IEquatable<InputFeatureUsage>
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
			internal set
			{
				this.m_Name = value;
			}
		}

		internal InputFeatureType internalType
		{
			get
			{
				return this.m_InternalType;
			}
			set
			{
				this.m_InternalType = value;
			}
		}

		public Type type
		{
			get
			{
				Type type;
				switch (this.m_InternalType)
				{
				case InputFeatureType.Custom:
					type = typeof(byte[]);
					break;
				case InputFeatureType.Binary:
					type = typeof(bool);
					break;
				case InputFeatureType.DiscreteStates:
					type = typeof(uint);
					break;
				case InputFeatureType.Axis1D:
					type = typeof(float);
					break;
				case InputFeatureType.Axis2D:
					type = typeof(Vector2);
					break;
				case InputFeatureType.Axis3D:
					type = typeof(Vector3);
					break;
				case InputFeatureType.Rotation:
					type = typeof(Quaternion);
					break;
				case InputFeatureType.Hand:
					type = typeof(Hand);
					break;
				case InputFeatureType.Bone:
					type = typeof(Bone);
					break;
				case InputFeatureType.Eyes:
					type = typeof(Eyes);
					break;
				default:
					throw new InvalidCastException("No valid managed type for unknown native type.");
				}
				return type;
			}
		}

		internal InputFeatureUsage(string name, InputFeatureType type)
		{
			this.m_Name = name;
			this.m_InternalType = type;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is InputFeatureUsage);
			return !flag && this.Equals((InputFeatureUsage)obj);
		}

		public bool Equals(InputFeatureUsage other)
		{
			return this.name == other.name && this.internalType == other.internalType;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode() ^ (this.internalType.GetHashCode() << 1);
		}

		public static bool operator ==(InputFeatureUsage a, InputFeatureUsage b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(InputFeatureUsage a, InputFeatureUsage b)
		{
			return !(a == b);
		}

		public InputFeatureUsage<T> As<T>()
		{
			bool flag = this.type != typeof(T);
			if (flag)
			{
				throw new ArgumentException("InputFeatureUsage type does not match out variable type.");
			}
			return new InputFeatureUsage<T>(this.name);
		}

		internal string m_Name;

		[NativeName("m_FeatureType")]
		internal InputFeatureType m_InternalType;
	}
}
