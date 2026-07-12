using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public class UxmlEnumAttributeDescription<T> : TypedUxmlAttributeDescription<T> where T : struct, IConvertible
	{
		public UxmlEnumAttributeDescription()
		{
			bool flag = !typeof(T).IsEnum;
			if (flag)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			base.defaultValue = new T();
			UxmlEnumeration uxmlEnumeration = new UxmlEnumeration();
			List<string> list = new List<string>();
			foreach (object obj in Enum.GetValues(typeof(T)))
			{
				T t = (T)((object)obj);
				list.Add(t.ToString(CultureInfo.InvariantCulture));
			}
			uxmlEnumeration.values = list;
			base.restriction = uxmlEnumeration;
		}

		public override string defaultValueAsString
		{
			get
			{
				T defaultValue = base.defaultValue;
				return defaultValue.ToString(CultureInfo.InvariantCulture.NumberFormat);
			}
		}

		public override T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<T>(bag, cc, (string s, T convertible) => UxmlEnumAttributeDescription<T>.ConvertValueToEnum<T>(s, convertible), base.defaultValue);
		}

		public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, ref T value)
		{
			return base.TryGetValueFromBag<T>(bag, cc, (string s, T convertible) => UxmlEnumAttributeDescription<T>.ConvertValueToEnum<T>(s, convertible), base.defaultValue, ref value);
		}

		private static U ConvertValueToEnum<U>(string v, U defaultValue) where U : struct
		{
			try
			{
				bool flag = string.IsNullOrEmpty(v);
				if (flag)
				{
					return defaultValue;
				}
				return (U)((object)Enum.Parse(typeof(U), v, true));
			}
			catch (ArgumentException)
			{
				Debug.LogError(UxmlEnumAttributeDescription<T>.GetEnumNameErrorMessage(v, typeof(U)));
			}
			catch (OverflowException)
			{
				Debug.LogError(UxmlEnumAttributeDescription<T>.GetEnumRangeErrorMessage(v, typeof(U)));
			}
			return defaultValue;
		}

		private static string GetEnumNameErrorMessage(string v, Type enumType)
		{
			return string.Concat(new string[]
			{
				"The ",
				enumType.Name,
				" enum does not contain the value `",
				v,
				"`. Value must be in range [",
				string.Join(" | ", Enum.GetNames(enumType)),
				"]."
			});
		}

		private static string GetEnumRangeErrorMessage(string v, Type enumType)
		{
			return v + " is outside of the range of possible values for the " + enumType.Name + " enum.";
		}
	}
}
