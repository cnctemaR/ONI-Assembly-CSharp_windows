using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct MaterialPropertyValue : IEquatable<MaterialPropertyValue>
	{
		public float GetFloat()
		{
			return this.packedValue.x;
		}

		public Vector4 GetVector()
		{
			return this.packedValue;
		}

		public Color GetColor()
		{
			return new Color(this.packedValue.x, this.packedValue.y, this.packedValue.z, this.packedValue.w);
		}

		public void SetFloat(float v)
		{
			this.packedValue = new Vector4(v, 0f, 0f, 0f);
		}

		public void SetVector(Vector4 v)
		{
			this.packedValue = v;
		}

		public void SetColor(Color c)
		{
			this.packedValue = new Vector4(c.r, c.g, c.b, c.a);
		}

		public override string ToString()
		{
			string text = this.name + "=";
			switch (this.type)
			{
			case MaterialPropertyValueType.Float:
				text += this.GetFloat().ToString();
				break;
			case MaterialPropertyValueType.Vector:
				text += this.GetVector().ToString();
				break;
			case MaterialPropertyValueType.Color:
				text += this.GetColor().ToString();
				break;
			case MaterialPropertyValueType.Texture:
				text += ((this.textureValue != null) ? this.textureValue.name : "null");
				break;
			}
			return text;
		}

		public static bool operator ==(MaterialPropertyValue lhs, MaterialPropertyValue rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(MaterialPropertyValue lhs, MaterialPropertyValue rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override bool Equals(object obj)
		{
			MaterialPropertyValue materialPropertyValue;
			bool flag;
			if (obj is MaterialPropertyValue)
			{
				materialPropertyValue = (MaterialPropertyValue)obj;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			return flag2 && this.Equals(materialPropertyValue);
		}

		public bool Equals(MaterialPropertyValue other)
		{
			bool flag = other.name != this.name || other.type != this.type;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				MaterialPropertyValueType materialPropertyValueType = this.type;
				MaterialPropertyValueType materialPropertyValueType2 = materialPropertyValueType;
				if (materialPropertyValueType2 > MaterialPropertyValueType.Color)
				{
					flag2 = materialPropertyValueType2 == MaterialPropertyValueType.Texture && other.textureValue == this.textureValue;
				}
				else
				{
					flag2 = other.packedValue == this.packedValue;
				}
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 1861411795;
			num = num * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.name);
			num = num * -1521134295 + this.type.GetHashCode();
			num = num * -1521134295 + this.packedValue.GetHashCode();
			bool flag = this.textureValue != null;
			if (flag)
			{
				num = num * -1521134295 + this.textureValue.GetHashCode();
			}
			return num;
		}

		public string name;

		public MaterialPropertyValueType type;

		public Vector4 packedValue;

		public Texture textureValue;
	}
}
