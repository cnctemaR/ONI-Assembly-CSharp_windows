using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode(Name = "ExposedReference")]
	[Serializable]
	public struct ExposedReference<T> where T : Object
	{
		public T Resolve(ExposedPropertyResolver resolver)
		{
			bool flag;
			Object @object = ExposedPropertyResolver.ResolveReferenceInternal(resolver.table, this.exposedName, out flag);
			T t;
			if (flag)
			{
				t = @object as T;
			}
			else
			{
				t = this.defaultValue as T;
			}
			return t;
		}

		[SerializeField]
		public PropertyName exposedName;

		[SerializeField]
		public Object defaultValue;
	}
}
