using System;
using System.Text;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct FilterFunction : IEquatable<FilterFunction>
	{
		public FilterFunctionType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		internal FixedBuffer4<FilterParameter> parameters
		{
			get
			{
				return this.m_Parameters;
			}
			set
			{
				this.m_Parameters = value;
			}
		}

		public int parameterCount
		{
			get
			{
				return this.m_ParameterCount;
			}
		}

		public FilterFunctionDefinition customDefinition
		{
			get
			{
				return this.m_CustomDefinition;
			}
			set
			{
				this.m_CustomDefinition = value;
			}
		}

		public unsafe void AddParameter(FilterParameter p)
		{
			int parameterCount = this.m_ParameterCount;
			bool flag = parameterCount >= 4;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("FilterFunction.AddParameter only support up to {0} parameters", 4));
			}
			*this.m_Parameters[parameterCount] = p;
			this.m_ParameterCount++;
		}

		public unsafe void SetParameter(int index, FilterParameter p)
		{
			bool flag = index < 0 || index >= this.m_ParameterCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("FilterFunction.SetParameter index out of range");
			}
			*this.m_Parameters[index] = p;
		}

		public unsafe FilterParameter GetParameter(int index)
		{
			bool flag = index < 0 || index >= this.parameterCount;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return *this.m_Parameters[index];
		}

		public void ClearParameters()
		{
			this.m_ParameterCount = 0;
		}

		public FilterFunction(FilterFunctionType type)
		{
			this.m_ParameterCount = 0;
			this.m_Type = type;
			this.m_CustomDefinition = null;
			this.m_Parameters = default(FixedBuffer4<FilterParameter>);
		}

		public FilterFunction(FilterFunctionDefinition filterDef)
		{
			this.m_ParameterCount = 0;
			bool flag = filterDef == null;
			if (flag)
			{
				throw new ArgumentNullException("filterDef");
			}
			this.m_Type = FilterFunctionType.Custom;
			this.m_CustomDefinition = filterDef;
			this.m_Parameters = default(FixedBuffer4<FilterParameter>);
		}

		internal unsafe FilterFunction(FilterFunctionType type, float arg)
		{
			FixedBuffer4<FilterParameter> fixedBuffer = default(FixedBuffer4<FilterParameter>);
			*fixedBuffer[0] = new FilterParameter(arg);
			this = new FilterFunction(type, fixedBuffer, 1);
		}

		internal unsafe FilterFunction(FilterFunctionType type, Color arg)
		{
			FixedBuffer4<FilterParameter> fixedBuffer = default(FixedBuffer4<FilterParameter>);
			*fixedBuffer[0] = new FilterParameter(arg);
			this = new FilterFunction(type, fixedBuffer, 1);
		}

		internal FilterFunction(FilterFunctionType type, FixedBuffer4<FilterParameter> parameters, int paramCount)
		{
			this.m_ParameterCount = 0;
			this.m_Type = type;
			this.m_CustomDefinition = null;
			this.m_Parameters = parameters;
			this.m_ParameterCount = paramCount;
			FilterFunctionDefinition definition = this.GetDefinition();
			bool flag = definition != null;
			if (flag)
			{
				int num = this.GetDefinition().parameters.Length;
				bool flag2 = num != paramCount;
				if (flag2)
				{
					Debug.LogError(string.Format("Invalid parameter count provided with filter of type {0}: provided {1} but expected {2}", type, paramCount, num));
				}
			}
		}

		internal unsafe FilterFunction(FilterFunctionDefinition customDefinition, FixedBuffer4<FilterParameter> parameters, int paramCount)
		{
			this.m_ParameterCount = 0;
			this.m_Type = FilterFunctionType.Custom;
			this.m_CustomDefinition = customDefinition;
			this.m_Parameters = parameters;
			this.m_ParameterCount = paramCount;
			bool flag = customDefinition != null;
			if (flag)
			{
				FilterParameterDeclaration[] parameters2 = customDefinition.parameters;
				int num = ((parameters2 != null) ? parameters2.Length : 0);
				bool flag2 = paramCount < num;
				if (flag2)
				{
					Debug.LogWarning(string.Format("FilterFunction '{0}' expects {1} parameters but only {2} were provided. Missing parameters will be set to their default values.", customDefinition.filterName, num, paramCount));
					for (int i = paramCount; i < num; i++)
					{
						FilterParameterDeclaration filterParameterDeclaration = customDefinition.parameters[i];
						*this.m_Parameters[i] = filterParameterDeclaration.interpolationDefaultValue;
					}
				}
				for (int j = 0; j < num; j++)
				{
					FilterParameterDeclaration filterParameterDeclaration2 = customDefinition.parameters[j];
					FilterParameter filterParameter = *this.m_Parameters[j];
					bool flag3 = filterParameterDeclaration2.interpolationDefaultValue.type != filterParameter.type;
					if (flag3)
					{
						Debug.LogWarning(string.Format("FilterFunction '{0}' expects parameter {1} to be of type {2} but got {3}. The parameter will be reset to its default value.", new object[]
						{
							customDefinition.filterName,
							j,
							filterParameterDeclaration2.interpolationDefaultValue.type,
							filterParameter.type
						}));
						*this.m_Parameters[j] = filterParameterDeclaration2.interpolationDefaultValue;
					}
				}
				bool flag4 = paramCount > num;
				if (flag4)
				{
					Debug.LogWarning(string.Format("FilterFunction '{0}' expects {1} parameters but {2} were provided. Extra parameters will be ignored.", customDefinition.filterName, num, paramCount));
				}
				this.m_ParameterCount = num;
			}
			else
			{
				this.m_ParameterCount = 0;
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal FilterFunctionDefinition GetDefinition()
		{
			bool flag = this.m_Type == FilterFunctionType.Custom;
			FilterFunctionDefinition filterFunctionDefinition;
			if (flag)
			{
				filterFunctionDefinition = this.m_CustomDefinition;
			}
			else
			{
				filterFunctionDefinition = FilterFunctionDefinitionUtils.GetBuiltinDefinition(this.m_Type);
			}
			return filterFunctionDefinition;
		}

		public unsafe static bool operator ==(FilterFunction lhs, FilterFunction rhs)
		{
			bool flag = lhs.m_CustomDefinition != rhs.m_CustomDefinition;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				for (int i = 0; i < 4; i++)
				{
					bool flag3 = *lhs.m_Parameters[i] != *rhs.m_Parameters[i];
					if (flag3)
					{
						return false;
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		public static bool operator !=(FilterFunction lhs, FilterFunction rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(FilterFunction other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is FilterFunction)
			{
				FilterFunction filterFunction = (FilterFunction)obj;
				flag = this.Equals(filterFunction);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_Parameters.GetHashCode() * 397) ^ ((this.m_CustomDefinition != null) ? this.m_CustomDefinition.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			FilterFunctionDefinition definition = this.GetDefinition();
			bool flag = !string.IsNullOrEmpty((definition != null) ? definition.filterName : null);
			if (flag)
			{
				stringBuilder.Append(definition.filterName);
			}
			else
			{
				bool flag2 = this.type == FilterFunctionType.Custom;
				if (flag2)
				{
					stringBuilder.Append("custom");
				}
				else
				{
					bool flag3 = this.type == FilterFunctionType.None;
					if (flag3)
					{
						stringBuilder.Append("none");
					}
				}
			}
			stringBuilder.Append("(");
			for (int i = 0; i < this.parameterCount; i++)
			{
				bool flag4 = i > 0;
				if (flag4)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(this.parameters[i].ToString());
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		[SerializeField]
		private FilterFunctionType m_Type;

		[SerializeField]
		private FixedBuffer4<FilterParameter> m_Parameters;

		[SerializeField]
		private int m_ParameterCount;

		[SerializeField]
		private FilterFunctionDefinition m_CustomDefinition;

		internal class PropertyBag : ContainerPropertyBag<FilterFunction>
		{
			public PropertyBag()
			{
				base.AddProperty<FixedBuffer4<FilterParameter>>(new FilterFunction.PropertyBag.ParametersProperty());
				base.AddProperty<FilterFunctionDefinition>(new FilterFunction.PropertyBag.FilterFunctionDefinitionProperty());
			}

			private class ParametersProperty : Property<FilterFunction, FixedBuffer4<FilterParameter>>
			{
				public override string Name { get; } = "parameters";

				public override bool IsReadOnly { get; } = false;

				public override FixedBuffer4<FilterParameter> GetValue(ref FilterFunction container)
				{
					return container.parameters;
				}

				public override void SetValue(ref FilterFunction container, FixedBuffer4<FilterParameter> value)
				{
					container.parameters = value;
				}
			}

			private class FilterFunctionDefinitionProperty : Property<FilterFunction, FilterFunctionDefinition>
			{
				public override string Name { get; } = "customDefinition";

				public override bool IsReadOnly { get; } = false;

				public override FilterFunctionDefinition GetValue(ref FilterFunction container)
				{
					return container.customDefinition;
				}

				public override void SetValue(ref FilterFunction container, FilterFunctionDefinition value)
				{
					container.customDefinition = value;
				}
			}
		}
	}
}
