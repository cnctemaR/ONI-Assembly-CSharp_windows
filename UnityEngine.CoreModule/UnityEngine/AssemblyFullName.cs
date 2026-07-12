using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Mono/AssemblyFullName.h")]
	[RequiredByNativeCode(GenerateProxy = true)]
	internal struct AssemblyFullName
	{
		public override bool Equals(object other)
		{
			if (other is AssemblyFullName)
			{
				AssemblyFullName assemblyFullName = (AssemblyFullName)other;
				if (this.Name == assemblyFullName.Name && this.Version == assemblyFullName.Version && this.PublicKeyToken == assemblyFullName.PublicKeyToken)
				{
					return this.Culture == assemblyFullName.Culture;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<string, AssemblyVersion, string, string>(this.Name, this.Version, this.PublicKeyToken, this.Culture);
		}

		public override string ToString()
		{
			return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", new object[]
			{
				this.Name,
				this.Version,
				string.IsNullOrEmpty(this.Culture) ? "neutral" : this.Culture,
				this.PublicKeyToken
			});
		}

		[NativeName("name")]
		public string Name;

		[NativeName("version")]
		public AssemblyVersion Version;

		[NativeName("publicKeyToken")]
		public string PublicKeyToken;

		[NativeName("culture")]
		public string Culture;
	}
}
