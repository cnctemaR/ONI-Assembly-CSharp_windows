using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[Serializable]
	public class Object
	{
		public virtual bool Equals(object obj)
		{
			return this == obj;
		}

		public static bool Equals(object objA, object objB)
		{
			return objA == objB || (objA != null && objB != null && objA.Equals(objB));
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public Object()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected virtual void Finalize()
		{
		}

		public virtual int GetHashCode()
		{
			return object.InternalGetHashCode(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern Type GetType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		protected extern object MemberwiseClone();

		public virtual string ToString()
		{
			return this.GetType().ToString();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool ReferenceEquals(object objA, object objB)
		{
			return objA == objB;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetHashCode(object o);

		private void FieldGetter(string typeName, string fieldName, ref object val)
		{
		}

		private void FieldSetter(string typeName, string fieldName, object val)
		{
		}
	}
}
