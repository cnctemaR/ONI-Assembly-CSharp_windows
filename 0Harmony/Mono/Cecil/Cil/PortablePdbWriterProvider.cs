using System;
using System.IO;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil
{
	internal sealed class PortablePdbWriterProvider : ISymbolWriterProvider
	{
		public ISymbolWriter GetSymbolWriter(ModuleDefinition module, string fileName)
		{
			Mixin.CheckModule(module);
			Mixin.CheckFileName(fileName);
			FileStream fileStream = File.Open(Mixin.GetPdbFileName(fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite);
			return this.GetSymbolWriter(module, Disposable.Owned<Stream>(fileStream), Disposable.NotOwned<Stream>(null));
		}

		public ISymbolWriter GetSymbolWriter(ModuleDefinition module, Stream symbolStream)
		{
			Mixin.CheckModule(module);
			Mixin.CheckStream(symbolStream);
			return this.GetSymbolWriter(module, Disposable.Owned<Stream>(new MemoryStream()), Disposable.NotOwned<Stream>(symbolStream));
		}

		private ISymbolWriter GetSymbolWriter(ModuleDefinition module, Disposable<Stream> stream, Disposable<Stream> final_stream)
		{
			MetadataBuilder metadataBuilder = new MetadataBuilder(module, this);
			ImageWriter imageWriter = ImageWriter.CreateDebugWriter(module, metadataBuilder, stream);
			return new PortablePdbWriter(metadataBuilder, module, imageWriter, final_stream);
		}
	}
}
