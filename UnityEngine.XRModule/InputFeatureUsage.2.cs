using System;

namespace UnityEngine.XR
{
	public struct InputFeatureUsage<T> : IEquatable<InputFeatureUsage<T>>
	{
		public string name { readonly get; set; }

		public InputFeatureUsage(string usageName)
		{
			this.name = usageName;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is InputFeatureUsage<T>);
			return !flag && this.Equals((InputFeatureUsage<T>)obj);
		}

		public bool Equals(InputFeatureUsage<T> other)
		{
			return this.name == other.name;
		}

		public override int GetHashCode()
		{
			return this.name.GetHashCode();
		}

		public static bool operator ==(InputFeatureUsage<T> a, InputFeatureUsage<T> b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(InputFeatureUsage<T> a, InputFeatureUsage<T> b)
		{
			return !(a == b);
		}

		private Type usageType
		{
			get
			{
				return typeof(T);
			}
		}

		public static explicit operator InputFeatureUsage(InputFeatureUsage<T> self)
		{
			InputFeatureType inputFeatureType = (InputFeatureType)4294967295U;
			Type usageType = self.usageType;
			bool flag = usageType == typeof(bool);
			if (flag)
			{
				inputFeatureType = InputFeatureType.Binary;
			}
			else
			{
				bool flag2 = usageType == typeof(uint);
				if (flag2)
				{
					inputFeatureType = InputFeatureType.DiscreteStates;
				}
				else
				{
					bool flag3 = usageType == typeof(float);
					if (flag3)
					{
						inputFeatureType = InputFeatureType.Axis1D;
					}
					else
					{
						bool flag4 = usageType == typeof(Vector2);
						if (flag4)
						{
							inputFeatureType = InputFeatureType.Axis2D;
						}
						else
						{
							bool flag5 = usageType == typeof(Vector3);
							if (flag5)
							{
								inputFeatureType = InputFeatureType.Axis3D;
							}
							else
							{
								bool flag6 = usageType == typeof(Quaternion);
								if (flag6)
								{
									inputFeatureType = InputFeatureType.Rotation;
								}
								else
								{
									bool flag7 = usageType == typeof(Hand);
									if (flag7)
									{
										inputFeatureType = InputFeatureType.Hand;
									}
									else
									{
										bool flag8 = usageType == typeof(Bone);
										if (flag8)
										{
											inputFeatureType = InputFeatureType.Bone;
										}
										else
										{
											bool flag9 = usageType == typeof(Eyes);
											if (flag9)
											{
												inputFeatureType = InputFeatureType.Eyes;
											}
											else
											{
												bool flag10 = usageType == typeof(byte[]);
												if (flag10)
												{
													inputFeatureType = InputFeatureType.Custom;
												}
												else
												{
													bool isEnum = usageType.IsEnum;
													if (isEnum)
													{
														inputFeatureType = InputFeatureType.DiscreteStates;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bool flag11 = inputFeatureType != (InputFeatureType)4294967295U;
			if (flag11)
			{
				return new InputFeatureUsage(self.name, inputFeatureType);
			}
			throw new InvalidCastException("No valid InputFeatureType for " + self.name + ".");
		}
	}
}
