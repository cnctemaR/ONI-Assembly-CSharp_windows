using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
	public sealed class InterfaceImplementedInVersionAttribute : Attribute
	{
		public InterfaceImplementedInVersionAttribute(Type interfaceType, byte majorVersion, byte minorVersion, byte buildVersion, byte revisionVersion)
		{
			this.m_interfaceType = interfaceType;
			this.m_majorVersion = majorVersion;
			this.m_minorVersion = minorVersion;
			this.m_buildVersion = buildVersion;
			this.m_revisionVersion = revisionVersion;
		}

		public Type InterfaceType
		{
			get
			{
				return this.m_interfaceType;
			}
		}

		public byte MajorVersion
		{
			get
			{
				return this.m_majorVersion;
			}
		}

		public byte MinorVersion
		{
			get
			{
				return this.m_minorVersion;
			}
		}

		public byte BuildVersion
		{
			get
			{
				return this.m_buildVersion;
			}
		}

		public byte RevisionVersion
		{
			get
			{
				return this.m_revisionVersion;
			}
		}

		private Type m_interfaceType;

		private byte m_majorVersion;

		private byte m_minorVersion;

		private byte m_buildVersion;

		private byte m_revisionVersion;
	}
}
