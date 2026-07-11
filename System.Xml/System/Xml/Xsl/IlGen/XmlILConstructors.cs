using System;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace System.Xml.Xsl.IlGen
{
	internal static class XmlILConstructors
	{
		private static ConstructorInfo GetConstructor(Type className)
		{
			return className.GetConstructor(new Type[0]);
		}

		private static ConstructorInfo GetConstructor(Type className, params Type[] args)
		{
			return className.GetConstructor(args);
		}

		public static readonly ConstructorInfo DecFromParts = XmlILConstructors.GetConstructor(typeof(decimal), new Type[]
		{
			typeof(int),
			typeof(int),
			typeof(int),
			typeof(bool),
			typeof(byte)
		});

		public static readonly ConstructorInfo DecFromInt32 = XmlILConstructors.GetConstructor(typeof(decimal), new Type[] { typeof(int) });

		public static readonly ConstructorInfo DecFromInt64 = XmlILConstructors.GetConstructor(typeof(decimal), new Type[] { typeof(long) });

		public static readonly ConstructorInfo Debuggable = XmlILConstructors.GetConstructor(typeof(DebuggableAttribute), new Type[] { typeof(DebuggableAttribute.DebuggingModes) });

		public static readonly ConstructorInfo NonUserCode = XmlILConstructors.GetConstructor(typeof(DebuggerNonUserCodeAttribute));

		public static readonly ConstructorInfo QName = XmlILConstructors.GetConstructor(typeof(XmlQualifiedName), new Type[]
		{
			typeof(string),
			typeof(string)
		});

		public static readonly ConstructorInfo StepThrough = XmlILConstructors.GetConstructor(typeof(DebuggerStepThroughAttribute));

		public static readonly ConstructorInfo Transparent = XmlILConstructors.GetConstructor(typeof(SecurityTransparentAttribute));
	}
}
