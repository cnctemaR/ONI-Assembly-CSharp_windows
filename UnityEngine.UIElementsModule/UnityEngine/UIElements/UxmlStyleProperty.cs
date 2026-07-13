using System;
using Unity.Collections;

namespace UnityEngine.UIElements
{
	internal struct UxmlStyleProperty : IDisposable, IEquatable<UxmlStyleProperty>
	{
		public bool isInlined
		{
			get
			{
				return this.values.Length > 0;
			}
		}

		public UxmlStyleProperty(StyleValueHandle[] values, bool requireVariableResolve)
		{
			this.values = new NativeArray<StyleValueHandle>(values, StyleDiff.k_MemoryLabel);
			this.requireVariableResolve = requireVariableResolve;
		}

		public bool Equals(UxmlStyleProperty other)
		{
			bool flag = this.requireVariableResolve != other.requireVariableResolve;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.values.IsCreated != other.values.IsCreated;
				if (flag3)
				{
					flag2 = false;
				}
				else
				{
					bool flag4 = !this.values.IsCreated;
					if (flag4)
					{
						flag2 = true;
					}
					else
					{
						bool flag5 = this.values.Length != other.values.Length;
						if (flag5)
						{
							flag2 = false;
						}
						else
						{
							for (int i = 0; i < this.values.Length; i++)
							{
								bool flag6 = this.values[i] != other.values[i];
								if (flag6)
								{
									return false;
								}
							}
							flag2 = true;
						}
					}
				}
			}
			return flag2;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is UxmlStyleProperty)
			{
				UxmlStyleProperty uxmlStyleProperty = (UxmlStyleProperty)obj;
				flag = this.Equals(uxmlStyleProperty);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<NativeArray<StyleValueHandle>, bool>(this.values, this.requireVariableResolve);
		}

		public void Dispose()
		{
			this.values.Dispose();
		}

		public NativeArray<StyleValueHandle> values;

		public bool requireVariableResolve;
	}
}
