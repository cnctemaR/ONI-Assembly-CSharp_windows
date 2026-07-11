using System;
using System.Collections;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Describes a XML attribute representing an enum as a string.</para>
	/// </summary>
	public class UxmlEnumAttributeDescription<T> : UxmlAttributeDescription where T : struct, IConvertible
	{
		public UxmlEnumAttributeDescription()
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}
			base.type = "string";
			base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
			this.defaultValue = new T();
			UxmlEnumeration uxmlEnumeration = new UxmlEnumeration();
			IEnumerator enumerator = Enum.GetValues(typeof(T)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					T t = (T)((object)obj);
					uxmlEnumeration.values.Add(t.ToString());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			base.restriction = uxmlEnumeration;
		}

		public T defaultValue { get; set; }

		public override string defaultValueAsString
		{
			get
			{
				T defaultValue = this.defaultValue;
				return defaultValue.ToString();
			}
		}

		public T GetValueFromBag(IUxmlAttributes bag)
		{
			return bag.GetPropertyEnum<T>(base.name, this.defaultValue);
		}
	}
}
