using System;
using System.Reflection;

namespace System
{
	[Serializable]
	internal class __Filters
	{
		internal virtual bool FilterAttribute(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null)
			{
				throw new InvalidFilterCriteriaException(Environment.GetResourceString("An Int32 must be provided for the filter criteria."));
			}
			MemberTypes memberType = m.MemberType;
			if (memberType != MemberTypes.Constructor)
			{
				if (memberType == MemberTypes.Field)
				{
					FieldAttributes fieldAttributes = FieldAttributes.PrivateScope;
					try
					{
						fieldAttributes = (FieldAttributes)((int)filterCriteria);
					}
					catch
					{
						throw new InvalidFilterCriteriaException(Environment.GetResourceString("An Int32 must be provided for the filter criteria."));
					}
					FieldAttributes attributes = ((FieldInfo)m).Attributes;
					return ((fieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.FieldAccessMask) == (fieldAttributes & FieldAttributes.FieldAccessMask)) && ((fieldAttributes & FieldAttributes.Static) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.InitOnly) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.InitOnly) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.Literal) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.Literal) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.NotSerialized) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.NotSerialized) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.PinvokeImpl) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.PinvokeImpl) != FieldAttributes.PrivateScope);
				}
				if (memberType != MemberTypes.Method)
				{
					return false;
				}
			}
			MethodAttributes methodAttributes = MethodAttributes.PrivateScope;
			try
			{
				methodAttributes = (MethodAttributes)((int)filterCriteria);
			}
			catch
			{
				throw new InvalidFilterCriteriaException(Environment.GetResourceString("An Int32 must be provided for the filter criteria."));
			}
			MethodAttributes methodAttributes2;
			if (m.MemberType == MemberTypes.Method)
			{
				methodAttributes2 = ((MethodInfo)m).Attributes;
			}
			else
			{
				methodAttributes2 = ((ConstructorInfo)m).Attributes;
			}
			return ((methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.MemberAccessMask) == (methodAttributes & MethodAttributes.MemberAccessMask)) && ((methodAttributes & MethodAttributes.Static) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.Static) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Final) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.Final) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Virtual) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.Virtual) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Abstract) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.Abstract) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.SpecialName) == MethodAttributes.PrivateScope || (methodAttributes2 & MethodAttributes.SpecialName) != MethodAttributes.PrivateScope);
		}

		internal virtual bool FilterName(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException(Environment.GetResourceString("A String must be provided for the filter criteria."));
			}
			string text = (string)filterCriteria;
			text = text.Trim();
			string text2 = m.Name;
			if (m.MemberType == MemberTypes.NestedType)
			{
				text2 = text2.Substring(text2.LastIndexOf('+') + 1);
			}
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				return text2.StartsWith(text, StringComparison.Ordinal);
			}
			return text2.Equals(text);
		}

		internal virtual bool FilterIgnoreCase(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException(Environment.GetResourceString("A String must be provided for the filter criteria."));
			}
			string text = (string)filterCriteria;
			text = text.Trim();
			string text2 = m.Name;
			if (m.MemberType == MemberTypes.NestedType)
			{
				text2 = text2.Substring(text2.LastIndexOf('+') + 1);
			}
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				return string.Compare(text2, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return string.Compare(text, text2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static readonly __Filters Instance = new __Filters();
	}
}
