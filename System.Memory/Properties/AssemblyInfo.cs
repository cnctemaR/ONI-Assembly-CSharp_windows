using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyVersion("4.0.99.0")]
[assembly: AssemblyTitle("System.Memory")]
[assembly: AssemblyDescription("System.Memory")]
[assembly: AssemblyDefaultAlias("System.Memory")]
[assembly: AssemblyCompany("Mono development team")]
[assembly: AssemblyProduct("Mono Common Language Infrastructure")]
[assembly: AssemblyCopyright("(c) Various Mono authors")]
[assembly: AssemblyInformationalVersion("4.0.99.0")]
[assembly: AssemblyFileVersion("4.0.99.0")]
// mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
[assembly: TypeForwardedTo(typeof(Base64))]
[assembly: TypeForwardedTo(typeof(BinaryPrimitives))]
[assembly: TypeForwardedTo(typeof(BuffersExtensions))]
[assembly: TypeForwardedTo(typeof(ReadOnlySequence<>.Enumerator))]
[assembly: TypeForwardedTo(typeof(ReadOnlySpan<>.Enumerator))]
[assembly: TypeForwardedTo(typeof(Span<>.Enumerator))]
[assembly: TypeForwardedTo(typeof(IBufferWriter<>))]
[assembly: TypeForwardedTo(typeof(IMemoryOwner<>))]
[assembly: TypeForwardedTo(typeof(IPinnable))]
[assembly: TypeForwardedTo(typeof(MemoryExtensions))]
[assembly: TypeForwardedTo(typeof(MemoryHandle))]
[assembly: TypeForwardedTo(typeof(MemoryManager<>))]
[assembly: TypeForwardedTo(typeof(MemoryMarshal))]
[assembly: TypeForwardedTo(typeof(MemoryPool<>))]
[assembly: TypeForwardedTo(typeof(Memory<>))]
[assembly: TypeForwardedTo(typeof(OperationStatus))]
[assembly: TypeForwardedTo(typeof(ReadOnlyMemory<>))]
[assembly: TypeForwardedTo(typeof(ReadOnlySequenceSegment<>))]
[assembly: TypeForwardedTo(typeof(ReadOnlySequence<>))]
[assembly: TypeForwardedTo(typeof(ReadOnlySpan<>))]
[assembly: TypeForwardedTo(typeof(SequenceMarshal))]
[assembly: TypeForwardedTo(typeof(SequencePosition))]
[assembly: TypeForwardedTo(typeof(Span<>))]
[assembly: TypeForwardedTo(typeof(StandardFormat))]
[assembly: TypeForwardedTo(typeof(Utf8Formatter))]
[assembly: TypeForwardedTo(typeof(Utf8Parser))]
