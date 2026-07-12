using System;
using System.Data.SqlTypes;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.SqlServer.Server
{
	internal sealed class BinaryOrderedUdtNormalizer : Normalizer
	{
		private FieldInfo[] GetFields(Type t)
		{
			return t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		internal BinaryOrderedUdtNormalizer(Type t, bool isTopLevelUdt)
		{
			this._skipNormalize = false;
			if (this._skipNormalize)
			{
				this._isTopLevelUdt = true;
			}
			this._isTopLevelUdt = true;
			FieldInfo[] fields = this.GetFields(t);
			this.FieldsToNormalize = new FieldInfoEx[fields.Length];
			int num = 0;
			foreach (FieldInfo fieldInfo in fields)
			{
				int num2 = Marshal.OffsetOf(fieldInfo.DeclaringType, fieldInfo.Name).ToInt32();
				this.FieldsToNormalize[num++] = new FieldInfoEx(fieldInfo, num2, Normalizer.GetNormalizer(fieldInfo.FieldType));
			}
			Array.Sort<FieldInfoEx>(this.FieldsToNormalize);
			if (!this._isTopLevelUdt && typeof(INullable).IsAssignableFrom(t))
			{
				PropertyInfo property = t.GetProperty("Null", BindingFlags.Static | BindingFlags.Public);
				if (property == null || property.PropertyType != t)
				{
					FieldInfo field = t.GetField("Null", BindingFlags.Static | BindingFlags.Public);
					if (field == null || field.FieldType != t)
					{
						throw new Exception("could not find Null field/property in nullable type " + ((t != null) ? t.ToString() : null));
					}
					this.NullInstance = field.GetValue(null);
				}
				else
				{
					this.NullInstance = property.GetValue(null, null);
				}
				this._padBuffer = new byte[this.Size - 1];
			}
		}

		internal bool IsNullable
		{
			get
			{
				return this.NullInstance != null;
			}
		}

		internal void NormalizeTopObject(object udt, Stream s)
		{
			this.Normalize(null, udt, s);
		}

		internal object DeNormalizeTopObject(Type t, Stream s)
		{
			return this.DeNormalizeInternal(t, s);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private object DeNormalizeInternal(Type t, Stream s)
		{
			object obj = null;
			if (!this._isTopLevelUdt && typeof(INullable).IsAssignableFrom(t) && (byte)s.ReadByte() == 0)
			{
				obj = this.NullInstance;
				s.Read(this._padBuffer, 0, this._padBuffer.Length);
				return obj;
			}
			if (obj == null)
			{
				obj = Activator.CreateInstance(t);
			}
			foreach (FieldInfoEx fieldInfoEx in this.FieldsToNormalize)
			{
				fieldInfoEx.Normalizer.DeNormalize(fieldInfoEx.FieldInfo, obj, s);
			}
			return obj;
		}

		internal override void Normalize(FieldInfo fi, object obj, Stream s)
		{
			object obj2;
			if (fi == null)
			{
				obj2 = obj;
			}
			else
			{
				obj2 = base.GetValue(fi, obj);
			}
			INullable nullable = obj2 as INullable;
			if (nullable != null && !this._isTopLevelUdt)
			{
				if (nullable.IsNull)
				{
					s.WriteByte(0);
					s.Write(this._padBuffer, 0, this._padBuffer.Length);
					return;
				}
				s.WriteByte(1);
			}
			foreach (FieldInfoEx fieldInfoEx in this.FieldsToNormalize)
			{
				fieldInfoEx.Normalizer.Normalize(fieldInfoEx.FieldInfo, obj2, s);
			}
		}

		internal override void DeNormalize(FieldInfo fi, object recvr, Stream s)
		{
			base.SetValue(fi, recvr, this.DeNormalizeInternal(fi.FieldType, s));
		}

		internal override int Size
		{
			get
			{
				if (this._size != 0)
				{
					return this._size;
				}
				if (this.IsNullable && !this._isTopLevelUdt)
				{
					this._size = 1;
				}
				foreach (FieldInfoEx fieldInfoEx in this.FieldsToNormalize)
				{
					this._size += fieldInfoEx.Normalizer.Size;
				}
				return this._size;
			}
		}

		internal readonly FieldInfoEx[] FieldsToNormalize;

		private int _size;

		private byte[] _padBuffer;

		internal readonly object NullInstance;

		private bool _isTopLevelUdt;
	}
}
