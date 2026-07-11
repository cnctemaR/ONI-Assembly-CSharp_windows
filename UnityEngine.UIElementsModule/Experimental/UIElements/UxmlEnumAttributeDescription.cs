using System;
using System.Collections;

namespace UnityEngine.Experimental.UIElements
{
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

		[Obsolete("Pass a creation context to the method.")]
		public T GetValueFromBag(IUxmlAttributes bag)
		{
			return this.GetValueFromBag(bag, default(CreationContext));
		}

		public T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
		{
			return base.GetValueFromBag<T>(bag, cc, new Func<string, T, T>(UxmlEnumAttributeDescription<T>.ConvertValueToEnum<T>), this.defaultValue);
		}

		private static U ConvertValueToEnum<U>(string v, U defaultValue)
		{
			U u;
			if (v == null || !Enum.IsDefined(typeof(U), v))
			{
				u = defaultValue;
			}
			else
			{
				U u2 = (U)((object)Enum.Parse(typeof(U), v));
				u = u2;
			}
			return u;
		}
	}
}
