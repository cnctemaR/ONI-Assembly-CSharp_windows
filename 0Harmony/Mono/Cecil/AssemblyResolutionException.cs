using System;
using System.IO;
using System.Runtime.Serialization;

namespace Mono.Cecil
{
	[Serializable]
	public sealed class AssemblyResolutionException : FileNotFoundException
	{
		public AssemblyNameReference AssemblyReference
		{
			get
			{
				return this.reference;
			}
		}

		public AssemblyResolutionException(AssemblyNameReference reference)
			: this(reference, null)
		{
		}

		public AssemblyResolutionException(AssemblyNameReference reference, Exception innerException)
			: base(string.Format("Failed to resolve assembly: '{0}'", reference), innerException)
		{
			this.reference = reference;
		}

		private AssemblyResolutionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		private readonly AssemblyNameReference reference;
	}
}
