using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
	[ComVisible(true)]
	[Serializable]
	public sealed class UnmanagedMarshal
	{
		private UnmanagedMarshal(UnmanagedType maint, int cnt)
		{
			this.count = cnt;
			this.t = maint;
			this.tbase = maint;
		}

		private UnmanagedMarshal(UnmanagedType maint, UnmanagedType elemt)
		{
			this.count = 0;
			this.t = maint;
			this.tbase = elemt;
		}

		public UnmanagedType BaseType
		{
			get
			{
				if (this.t == UnmanagedType.LPArray || this.t == UnmanagedType.SafeArray)
				{
					throw new ArgumentException();
				}
				return this.tbase;
			}
		}

		public int ElementCount
		{
			get
			{
				return this.count;
			}
		}

		public UnmanagedType GetUnmanagedType
		{
			get
			{
				return this.t;
			}
		}

		public Guid IIDGuid
		{
			get
			{
				return new Guid(this.guid);
			}
		}

		public static UnmanagedMarshal DefineByValArray(int elemCount)
		{
			return new UnmanagedMarshal(UnmanagedType.ByValArray, elemCount);
		}

		public static UnmanagedMarshal DefineByValTStr(int elemCount)
		{
			return new UnmanagedMarshal(UnmanagedType.ByValTStr, elemCount);
		}

		public static UnmanagedMarshal DefineLPArray(UnmanagedType elemType)
		{
			return new UnmanagedMarshal(UnmanagedType.LPArray, elemType);
		}

		public static UnmanagedMarshal DefineSafeArray(UnmanagedType elemType)
		{
			return new UnmanagedMarshal(UnmanagedType.SafeArray, elemType);
		}

		public static UnmanagedMarshal DefineUnmanagedMarshal(UnmanagedType unmanagedType)
		{
			return new UnmanagedMarshal(unmanagedType, unmanagedType);
		}

		public static UnmanagedMarshal DefineCustom(Type typeref, string cookie, string mtype, Guid id)
		{
			UnmanagedMarshal unmanagedMarshal = new UnmanagedMarshal(UnmanagedType.CustomMarshaler, UnmanagedType.CustomMarshaler);
			unmanagedMarshal.mcookie = cookie;
			unmanagedMarshal.marshaltype = mtype;
			unmanagedMarshal.marshaltyperef = typeref;
			if (id == Guid.Empty)
			{
				unmanagedMarshal.guid = string.Empty;
			}
			else
			{
				unmanagedMarshal.guid = id.ToString();
			}
			return unmanagedMarshal;
		}

		internal static UnmanagedMarshal DefineLPArrayInternal(UnmanagedType elemType, int sizeConst, int sizeParamIndex)
		{
			return new UnmanagedMarshal(UnmanagedType.LPArray, elemType)
			{
				count = sizeConst,
				param_num = sizeParamIndex,
				has_size = true
			};
		}

		internal MarshalAsAttribute ToMarshalAsAttribute()
		{
			MarshalAsAttribute marshalAsAttribute = new MarshalAsAttribute(this.t);
			marshalAsAttribute.ArraySubType = this.tbase;
			marshalAsAttribute.MarshalCookie = this.mcookie;
			marshalAsAttribute.MarshalType = this.marshaltype;
			marshalAsAttribute.MarshalTypeRef = this.marshaltyperef;
			if (this.count == -1)
			{
				marshalAsAttribute.SizeConst = 0;
			}
			else
			{
				marshalAsAttribute.SizeConst = this.count;
			}
			if (this.param_num == -1)
			{
				marshalAsAttribute.SizeParamIndex = 0;
			}
			else
			{
				marshalAsAttribute.SizeParamIndex = (short)this.param_num;
			}
			return marshalAsAttribute;
		}

		private int count;

		private UnmanagedType t;

		private UnmanagedType tbase;

		private string guid;

		private string mcookie;

		private string marshaltype;

		private Type marshaltyperef;

		private int param_num;

		private bool has_size;
	}
}
