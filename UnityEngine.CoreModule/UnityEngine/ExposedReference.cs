using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Creates a type whos value is resolvable at runtime.</para>
	/// </summary>
	[UsedByNativeCode(Name = "ExposedReference")]
	[Serializable]
	public struct ExposedReference<T> where T : Object
	{
		public T Resolve(IExposedPropertyTable resolver)
		{
			if (resolver != null)
			{
				bool flag;
				Object referenceValue = resolver.GetReferenceValue(this.exposedName, out flag);
				if (flag)
				{
					return referenceValue as T;
				}
			}
			return this.defaultValue as T;
		}

		[SerializeField]
		public PropertyName exposedName;

		[SerializeField]
		public Object defaultValue;
	}
}
