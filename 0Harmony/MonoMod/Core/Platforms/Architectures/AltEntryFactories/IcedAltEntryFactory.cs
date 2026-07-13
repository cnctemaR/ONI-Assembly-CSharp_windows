using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Iced.Intel;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Architectures.AltEntryFactories
{
	internal sealed class IcedAltEntryFactory : IAltEntryFactory
	{
		[NullableContext(1)]
		public IcedAltEntryFactory(ISystem system, int bitness)
		{
			this.system = system;
			this.bitness = bitness;
			this.alloc = system.MemoryAllocator;
		}

		[NullableContext(2)]
		public unsafe IntPtr CreateAlternateEntrypoint(IntPtr entrypoint, int minLength, out IDisposable handle)
		{
			IcedAltEntryFactory.PtrCodeReader ptrCodeReader = new IcedAltEntryFactory.PtrCodeReader(entrypoint);
			Decoder decoder = Decoder.Create(this.bitness, ptrCodeReader, (ulong)(long)entrypoint, DecoderOptions.NoInvalidCheck | DecoderOptions.AMD);
			InstructionList instructionList = new InstructionList();
			while (ptrCodeReader.Position < minLength)
			{
				decoder.Decode(instructionList.AllocUninitializedElement());
			}
			bool flag = false;
			using (InstructionList.Enumerator enumerator = instructionList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsIPRelativeMemoryOperand)
					{
						flag = true;
						break;
					}
				}
			}
			Instruction instruction = *instructionList[instructionList.Count - 1];
			if (instruction.Mnemonic == Mnemonic.Call)
			{
				Encoder encoder = Encoder.Create(this.bitness, new IcedAltEntryFactory.NullCodeWriter());
				Instruction instruction2 = instruction;
				Code code = instruction.Code;
				Code code2;
				if (code <= Code.Call_ptr1632)
				{
					if (code == Code.Call_ptr1616)
					{
						code2 = Code.Jmp_ptr1616;
						goto IL_01B1;
					}
					if (code == Code.Call_ptr1632)
					{
						code2 = Code.Jmp_ptr1632;
						goto IL_01B1;
					}
				}
				else
				{
					switch (code)
					{
					case Code.Call_rel16:
						code2 = Code.Jmp_rel16;
						goto IL_01B1;
					case Code.Call_rel32_32:
						code2 = Code.Jmp_rel32_32;
						goto IL_01B1;
					case Code.Call_rel32_64:
						code2 = Code.Jmp_rel32_64;
						goto IL_01B1;
					default:
						switch (code)
						{
						case Code.Call_m1616:
							code2 = Code.Jmp_m1616;
							goto IL_01B1;
						case Code.Call_m1632:
							code2 = Code.Jmp_m1632;
							goto IL_01B1;
						case Code.Call_m1664:
							code2 = Code.Jmp_m1664;
							goto IL_01B1;
						case Code.Jmp_rm16:
							code2 = Code.Jmp_rm16;
							goto IL_01B1;
						case Code.Jmp_rm32:
							code2 = Code.Jmp_rm32;
							goto IL_01B1;
						case Code.Jmp_rm64:
							code2 = Code.Jmp_rm64;
							goto IL_01B1;
						}
						break;
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Unrecognized call opcode ");
				defaultInterpolatedStringHandler.AppendFormatted<Code>(instruction.Code);
				throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
				IL_01B1:
				instruction2.Code = code2;
				instruction2.Length = (int)encoder.Encode(in instruction2, instruction2.IP);
				ulong nextIP = instruction.NextIP;
				Instruction instruction3;
				bool flag2;
				Instruction instruction4;
				if (this.bitness == 32)
				{
					instruction3 = Instruction.Create(Code.Pushd_imm32, (uint)nextIP);
					instruction3.Length = (int)encoder.Encode(in instruction3, instruction2.IP);
					instruction3.IP = instruction2.IP;
					instruction2.IP += (ulong)((long)instruction3.Length);
					flag2 = false;
					instruction4 = default(Instruction);
				}
				else
				{
					flag2 = true;
					instruction4 = Instruction.CreateDeclareQword(nextIP);
					Code code3 = Code.Push_rm64;
					MemoryOperand memoryOperand = new MemoryOperand(Register.RIP, (long)instruction2.NextIP);
					instruction3 = Instruction.Create(code3, in memoryOperand);
					instruction3.Length = (int)encoder.Encode(in instruction3, instruction2.IP);
					instruction3.IP = instruction2.IP;
					instruction2.IP += (ulong)((long)instruction3.Length);
					instruction4.IP = instruction2.NextIP;
					instruction3.MemoryDisplacement64 = instruction4.IP;
				}
				instructionList.RemoveAt(instructionList.Count - 1);
				instructionList.Add(in instruction3);
				instructionList.Add(in instruction2);
				if (flag2)
				{
					instructionList.Add(in instruction4);
				}
			}
			else
			{
				InstructionList instructionList2 = instructionList;
				Instruction instruction5 = Instruction.CreateBranch((this.bitness == 64) ? Code.Jmp_rel32_64 : Code.Jmp_rel32_32, decoder.IP);
				instructionList2.Add(in instruction5);
			}
			int num = ptrCodeReader.Position + 5;
			IntPtr baseAddress2;
			using (IcedAltEntryFactory.BufferCodeWriter bufferCodeWriter = new IcedAltEntryFactory.BufferCodeWriter())
			{
				IAllocatedMemory allocatedMemory;
				string text;
				for (;;)
				{
					bufferCodeWriter.Reset();
					if (flag)
					{
						Helpers.Assert(this.alloc.TryAllocateInRange(new PositionedAllocationRequest(entrypoint, entrypoint + (IntPtr)int.MinValue, entrypoint + (IntPtr)int.MaxValue, new AllocationRequest(num)
						{
							Executable = true
						}), out allocatedMemory), null, "alloc.TryAllocateInRange(\n                        new(entrypoint, (nint)entrypoint + int.MinValue, (nint)entrypoint + int.MaxValue,\n                        new(estTotalSize) { Executable = true }), out allocated)");
					}
					else
					{
						Helpers.Assert(this.alloc.TryAllocate(new AllocationRequest(num)
						{
							Executable = true
						}, out allocatedMemory), null, "alloc.TryAllocate(new(estTotalSize) { Executable = true }, out allocated)");
					}
					IntPtr baseAddress = allocatedMemory.BaseAddress;
					BlockEncoderResult blockEncoderResult;
					if (!BlockEncoder.TryEncode(this.bitness, new InstructionBlock(bufferCodeWriter, instructionList, (ulong)(long)baseAddress), out text, out blockEncoderResult, BlockEncoderOptions.None))
					{
						break;
					}
					if (bufferCodeWriter.Data.Length == allocatedMemory.Size)
					{
						goto IL_0445;
					}
					num = bufferCodeWriter.Data.Length;
					allocatedMemory.Dispose();
				}
				allocatedMemory.Dispose();
				bool flag3;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(44, 1, out flag3);
				if (flag3)
				{
					debugLogErrorStringHandler.AppendLiteral("BlockEncoder failed to encode instructions: ");
					debugLogErrorStringHandler.AppendFormatted(text);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				throw new InvalidOperationException("BlockEncoder failed to encode instructions: " + text);
				IL_0445:
				this.system.PatchData(PatchTargetKind.Executable, allocatedMemory.BaseAddress, bufferCodeWriter.Data.Span, default(global::System.Span<byte>));
				handle = allocatedMemory;
				baseAddress2 = allocatedMemory.BaseAddress;
			}
			return baseAddress2;
		}

		[Nullable(1)]
		private readonly ISystem system;

		[Nullable(1)]
		private readonly IMemoryAllocator alloc;

		private readonly int bitness;

		private sealed class PtrCodeReader : CodeReader
		{
			public PtrCodeReader(IntPtr basePtr)
			{
				this.Base = basePtr;
				this.Position = 0;
			}

			public IntPtr Base { get; }

			public int Position { get; private set; }

			public unsafe override int ReadByte()
			{
				IntPtr @base = this.Base;
				int position = this.Position;
				this.Position = position + 1;
				return (int)(*(@base + (IntPtr)position));
			}
		}

		private sealed class NullCodeWriter : CodeWriter
		{
			public override void WriteByte(byte value)
			{
			}
		}

		private sealed class BufferCodeWriter : CodeWriter, IDisposable
		{
			public BufferCodeWriter()
			{
				this.pool = global::System.Buffers.ArrayPool<byte>.Shared;
			}

			public global::System.ReadOnlyMemory<byte> Data
			{
				get
				{
					return this.buffer.AsMemory<byte>().Slice(0, this.pos);
				}
			}

			public override void WriteByte(byte value)
			{
				if (this.buffer == null)
				{
					this.buffer = this.pool.Rent(8);
				}
				if (this.buffer.Length <= this.pos)
				{
					byte[] array = this.pool.Rent(this.buffer.Length * 2);
					Array.Copy(this.buffer, array, this.buffer.Length);
					this.pool.Return(this.buffer, false);
					this.buffer = array;
				}
				byte[] array2 = this.buffer;
				int num = this.pos;
				this.pos = num + 1;
				array2[num] = value;
			}

			public void Reset()
			{
				this.pos = 0;
			}

			public void Dispose()
			{
				if (this.buffer != null)
				{
					byte[] array = this.buffer;
					this.buffer = null;
					this.pool.Return(array, false);
				}
			}

			[Nullable(1)]
			private readonly global::System.Buffers.ArrayPool<byte> pool;

			[Nullable(2)]
			private byte[] buffer;

			private int pos;
		}
	}
}
