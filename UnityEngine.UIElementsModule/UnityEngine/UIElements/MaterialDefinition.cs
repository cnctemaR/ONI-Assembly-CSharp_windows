using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct MaterialDefinition : IEquatable<MaterialDefinition>
	{
		public Material material
		{
			get
			{
				return this.m_Material;
			}
			set
			{
				this.m_Material = value;
			}
		}

		public MaterialDefinition(Material m)
		{
			this.propertyValues = null;
			this.m_Material = m;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal MaterialDefinition(Material m, List<MaterialPropertyValue> propertyValues)
		{
			this.propertyValues = null;
			this.m_Material = m;
			this.propertyValues = propertyValues;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal MaterialDefinition(MaterialDefinition other)
		{
			this.propertyValues = null;
			this.m_Material = other.m_Material;
			bool flag = other.propertyValues != null;
			if (flag)
			{
				this.propertyValues = new List<MaterialPropertyValue>(other.propertyValues);
			}
			else
			{
				this.propertyValues = null;
			}
		}

		private MaterialPropertyValue GetValue(string name)
		{
			bool flag = this.propertyValues != null;
			if (flag)
			{
				int num = this.propertyValues.FindIndex((MaterialPropertyValue p) => p.name == name);
				bool flag2 = num >= 0;
				if (flag2)
				{
					return this.propertyValues[num];
				}
			}
			return default(MaterialPropertyValue);
		}

		private void SetValue(MaterialPropertyValue prop)
		{
			bool flag = this.propertyValues == null;
			if (flag)
			{
				this.propertyValues = new List<MaterialPropertyValue>();
			}
			int num = this.propertyValues.FindIndex((MaterialPropertyValue p) => p.name == prop.name);
			bool flag2 = num >= 0;
			if (flag2)
			{
				this.propertyValues[num] = prop;
			}
			else
			{
				this.propertyValues.Add(prop);
			}
		}

		public float GetFloat(string name)
		{
			return this.GetValue(name).GetFloat();
		}

		public Vector4 GetVector(string name)
		{
			return this.GetValue(name).GetVector();
		}

		public Color GetColor(string name)
		{
			return this.GetValue(name).GetColor();
		}

		public Texture GetTexture(string name)
		{
			return this.GetValue(name).textureValue;
		}

		public void SetFloat(string name, float value)
		{
			MaterialPropertyValue materialPropertyValue = new MaterialPropertyValue
			{
				name = name,
				type = MaterialPropertyValueType.Float
			};
			materialPropertyValue.SetFloat(value);
			this.SetValue(materialPropertyValue);
		}

		public void SetVector(string name, Vector4 value)
		{
			MaterialPropertyValue materialPropertyValue = new MaterialPropertyValue
			{
				name = name,
				type = MaterialPropertyValueType.Vector
			};
			materialPropertyValue.SetVector(value);
			this.SetValue(materialPropertyValue);
		}

		public void SetColor(string name, Color value)
		{
			MaterialPropertyValue materialPropertyValue = new MaterialPropertyValue
			{
				name = name,
				type = MaterialPropertyValueType.Color
			};
			materialPropertyValue.SetColor(value);
			this.SetValue(materialPropertyValue);
		}

		public void SetTexture(string name, Texture value)
		{
			this.SetValue(new MaterialPropertyValue
			{
				name = name,
				type = MaterialPropertyValueType.Texture,
				textureValue = value
			});
		}

		public static MaterialDefinition FromMaterial(Material m)
		{
			return new MaterialDefinition
			{
				material = m
			};
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static MaterialDefinition FromObject(object obj)
		{
			MaterialDefinition materialDefinition;
			bool flag;
			if (obj is MaterialDefinition)
			{
				materialDefinition = (MaterialDefinition)obj;
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			MaterialDefinition materialDefinition2;
			if (flag2)
			{
				materialDefinition2 = materialDefinition;
			}
			else
			{
				Material material = obj as Material;
				bool flag3 = material != null;
				if (flag3)
				{
					materialDefinition2 = MaterialDefinition.FromMaterial(material);
				}
				else
				{
					materialDefinition2 = default(MaterialDefinition);
				}
			}
			return materialDefinition2;
		}

		internal static IEnumerable<Type> allowedAssetTypes
		{
			get
			{
				yield return typeof(Material);
				yield return typeof(Texture2D);
				yield break;
			}
		}

		internal MaterialPropertyBlock BuildPropertyBlock()
		{
			bool flag = this.propertyValues == null || this.propertyValues.Count == 0;
			MaterialPropertyBlock materialPropertyBlock;
			if (flag)
			{
				materialPropertyBlock = null;
			}
			else
			{
				MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
				foreach (MaterialPropertyValue materialPropertyValue in this.propertyValues)
				{
					switch (materialPropertyValue.type)
					{
					case MaterialPropertyValueType.Float:
						materialPropertyBlock2.SetFloat(materialPropertyValue.name, materialPropertyValue.GetFloat());
						break;
					case MaterialPropertyValueType.Vector:
						materialPropertyBlock2.SetVector(materialPropertyValue.name, materialPropertyValue.GetVector());
						break;
					case MaterialPropertyValueType.Color:
						materialPropertyBlock2.SetColor(materialPropertyValue.name, materialPropertyValue.GetColor());
						break;
					case MaterialPropertyValueType.Texture:
					{
						bool flag2 = materialPropertyValue.textureValue != null;
						if (flag2)
						{
							materialPropertyBlock2.SetTexture(materialPropertyValue.name, materialPropertyValue.textureValue);
						}
						break;
					}
					}
				}
				materialPropertyBlock = materialPropertyBlock2;
			}
			return materialPropertyBlock;
		}

		public bool IsEmpty()
		{
			return this.material == null;
		}

		public static bool operator ==(MaterialDefinition lhs, MaterialDefinition rhs)
		{
			bool flag = lhs.material == rhs.material;
			bool flag2 = !flag;
			bool flag3;
			if (flag2)
			{
				flag3 = false;
			}
			else
			{
				bool flag4 = lhs.propertyValues != null && lhs.propertyValues.Count > 0;
				bool flag5 = rhs.propertyValues != null && rhs.propertyValues.Count > 0;
				bool flag6 = flag4 != flag5;
				if (flag6)
				{
					flag3 = false;
				}
				else
				{
					bool flag7 = !flag4;
					if (flag7)
					{
						flag3 = true;
					}
					else
					{
						bool flag8 = lhs.propertyValues.Count != rhs.propertyValues.Count;
						if (flag8)
						{
							flag3 = false;
						}
						else
						{
							for (int i = 0; i < lhs.propertyValues.Count; i++)
							{
								MaterialPropertyValue materialPropertyValue = lhs.propertyValues[i];
								MaterialPropertyValue materialPropertyValue2 = rhs.propertyValues[i];
								bool flag9 = materialPropertyValue != materialPropertyValue2;
								if (flag9)
								{
									return false;
								}
							}
							flag3 = true;
						}
					}
				}
			}
			return flag3;
		}

		public static bool operator !=(MaterialDefinition lhs, MaterialDefinition rhs)
		{
			return !(lhs == rhs);
		}

		public static implicit operator MaterialDefinition(Material m)
		{
			return MaterialDefinition.FromMaterial(m);
		}

		public bool Equals(MaterialDefinition other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is MaterialDefinition);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				MaterialDefinition materialDefinition = (MaterialDefinition)obj;
				flag2 = materialDefinition == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = 851985039;
			bool flag = this.material != null;
			if (flag)
			{
				num = num * -1521134295 + this.material.GetHashCode();
			}
			bool flag2 = this.propertyValues != null;
			if (flag2)
			{
				foreach (MaterialPropertyValue materialPropertyValue in this.propertyValues)
				{
					num = num * -1521134295 + materialPropertyValue.GetHashCode();
				}
			}
			return num;
		}

		public override string ToString()
		{
			string text = "null";
			bool flag = this.material != null;
			if (flag)
			{
				text = this.material.name;
				bool flag2 = this.propertyValues != null && this.propertyValues.Count > 0;
				if (flag2)
				{
					text += " { ";
					for (int i = 0; i < this.propertyValues.Count; i++)
					{
						text = text + this.propertyValues[i].ToString() + " ";
					}
					text += "}";
				}
			}
			return text;
		}

		[SerializeField]
		private Material m_Material;

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		[SerializeField]
		internal List<MaterialPropertyValue> propertyValues;

		internal class PropertyBag : ContainerPropertyBag<MaterialDefinition>
		{
			public PropertyBag()
			{
				base.AddProperty<Material>(new MaterialDefinition.PropertyBag.MaterialProperty());
			}

			private class MaterialProperty : Property<MaterialDefinition, Material>
			{
				public override string Name { get; } = "material";

				public override bool IsReadOnly { get; } = false;

				public override Material GetValue(ref MaterialDefinition container)
				{
					return container.material;
				}

				public override void SetValue(ref MaterialDefinition container, Material value)
				{
					container.material = value;
				}
			}
		}
	}
}
