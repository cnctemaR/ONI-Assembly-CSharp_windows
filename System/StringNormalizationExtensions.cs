using System;
using System.ComponentModel;
using System.Security;
using System.Text;
using Unity;

namespace System
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class StringNormalizationExtensions
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsNormalized(this string value)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SecurityCritical]
		public static bool IsNormalized(this string value, NormalizationForm normalizationForm)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string Normalize(this string value)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SecurityCritical]
		public static string Normalize(this string value, NormalizationForm normalizationForm)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
