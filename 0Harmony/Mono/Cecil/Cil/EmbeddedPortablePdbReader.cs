using System;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	internal sealed class EmbeddedPortablePdbReader : ISymbolReader, IDisposable
	{
		internal EmbeddedPortablePdbReader(PortablePdbReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException();
			}
			this.reader = reader;
		}

		public ISymbolWriterProvider GetWriterProvider()
		{
			return new EmbeddedPortablePdbWriterProvider();
		}

		public bool ProcessDebugHeader(ImageDebugHeader header)
		{
			return this.reader.ProcessDebugHeader(header);
		}

		public MethodDebugInformation Read(MethodDefinition method)
		{
			return this.reader.Read(method);
		}

		public Collection<CustomDebugInformation> Read(ICustomDebugInformationProvider provider)
		{
			return this.reader.Read(provider);
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

		private readonly PortablePdbReader reader;
	}
}
