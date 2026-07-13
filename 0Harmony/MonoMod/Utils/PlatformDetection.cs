using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MonoMod.Utils.Interop;

namespace MonoMod.Utils
{
	internal static class PlatformDetection
	{
		private static void EnsurePlatformInfoInitialized()
		{
			if (PlatformDetection.platInitState != 0)
			{
				return;
			}
			ValueTuple<OSKind, ArchitectureKind> valueTuple = PlatformDetection.DetectPlatformInfo();
			PlatformDetection.os = valueTuple.Item1;
			PlatformDetection.arch = valueTuple.Item2;
			Thread.MemoryBarrier();
			Interlocked.Exchange(ref PlatformDetection.platInitState, 1);
		}

		public static OSKind OS
		{
			get
			{
				PlatformDetection.EnsurePlatformInfoInitialized();
				return PlatformDetection.os;
			}
		}

		public static ArchitectureKind Architecture
		{
			get
			{
				PlatformDetection.EnsurePlatformInfoInitialized();
				return PlatformDetection.arch;
			}
		}

		[return: TupleElementNames(new string[] { "OS", "Arch" })]
		private static ValueTuple<OSKind, ArchitectureKind> DetectPlatformInfo()
		{
			OSKind oskind = OSKind.Unknown;
			ArchitectureKind architectureKind = ArchitectureKind.Unknown;
			PropertyInfo property = typeof(Environment).GetProperty("Platform", BindingFlags.Static | BindingFlags.NonPublic);
			string text;
			if (property != null)
			{
				object value = property.GetValue(null, null);
				text = ((value != null) ? value.ToString() : null);
			}
			else
			{
				text = Environment.OSVersion.Platform.ToString();
			}
			text = ((text != null) ? text.ToUpperInvariant() : null) ?? "";
			if (text.Contains("WIN", StringComparison.Ordinal))
			{
				oskind = OSKind.Windows;
			}
			else if (text.Contains("MAC", StringComparison.Ordinal) || text.Contains("OSX", StringComparison.Ordinal))
			{
				oskind = OSKind.OSX;
			}
			else if (text.Contains("LIN", StringComparison.Ordinal))
			{
				oskind = OSKind.Linux;
			}
			else if (text.Contains("BSD", StringComparison.Ordinal))
			{
				oskind = OSKind.BSD;
			}
			else if (text.Contains("UNIX", StringComparison.Ordinal))
			{
				oskind = OSKind.Posix;
			}
			if (oskind == OSKind.Windows)
			{
				PlatformDetection.DetectInfoWindows(ref oskind, ref architectureKind);
			}
			else if ((oskind & OSKind.Posix) != OSKind.Unknown)
			{
				PlatformDetection.DetectInfoPosix(ref oskind, ref architectureKind);
			}
			if (oskind != OSKind.Unknown)
			{
				if (oskind == OSKind.Linux && Directory.Exists("/data") && File.Exists("/system/build.prop"))
				{
					oskind = OSKind.Android;
				}
				else if (oskind == OSKind.Posix && Directory.Exists("/Applications") && Directory.Exists("/System") && Directory.Exists("/User") && !Directory.Exists("/Users"))
				{
					oskind = OSKind.IOS;
				}
				else if (oskind == OSKind.Windows && PlatformDetection.CheckWine())
				{
					oskind = OSKind.Wine;
				}
			}
			bool flag;
			MMDbgLog.DebugLogInfoStringHandler debugLogInfoStringHandler = new MMDbgLog.DebugLogInfoStringHandler(16, 2, out flag);
			if (flag)
			{
				debugLogInfoStringHandler.AppendLiteral("Platform info: ");
				debugLogInfoStringHandler.AppendFormatted<OSKind>(oskind);
				debugLogInfoStringHandler.AppendLiteral(" ");
				debugLogInfoStringHandler.AppendFormatted<ArchitectureKind>(architectureKind);
			}
			MMDbgLog.Info(ref debugLogInfoStringHandler);
			return new ValueTuple<OSKind, ArchitectureKind>(oskind, architectureKind);
		}

		private unsafe static int PosixUname(OSKind os, byte* buf)
		{
			if (os != OSKind.OSX)
			{
				return PlatformDetection.<PosixUname>g__Libc|9_0(buf);
			}
			return PlatformDetection.<PosixUname>g__Osx|9_1(buf);
		}

		[return: Nullable(1)]
		private unsafe static string GetCString(global::System.ReadOnlySpan<byte> buffer, out int nullByte)
		{
			fixed (byte* pinnableReference = buffer.GetPinnableReference())
			{
				return Marshal.PtrToStringAnsi((IntPtr)((void*)pinnableReference), nullByte = buffer.IndexOf(0));
			}
		}

		private unsafe static void DetectInfoPosix(ref OSKind os, ref ArchitectureKind arch)
		{
			try
			{
				global::System.Span<byte> span = new byte[3078];
				bool flag;
				try
				{
					fixed (byte* ptr = span.GetPinnableReference())
					{
						byte* ptr2 = ptr;
						if (PlatformDetection.PosixUname(os, ptr2) < 0)
						{
							string message = new Win32Exception(Marshal.GetLastWin32Error()).Message;
							MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new MMDbgLog.DebugLogErrorStringHandler(24, 1, out flag);
							if (flag)
							{
								debugLogErrorStringHandler.AppendLiteral("uname() syscall failed! ");
								debugLogErrorStringHandler.AppendFormatted(message);
							}
							MMDbgLog.Error(ref debugLogErrorStringHandler);
							return;
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				int num;
				string text = PlatformDetection.GetCString(span, out num).ToUpperInvariant();
				span = span.Slice(num);
				MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new MMDbgLog.DebugLogTraceStringHandler(22, 1, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("uname() call returned ");
					debugLogTraceStringHandler.AppendFormatted(text);
				}
				MMDbgLog.Trace(ref debugLogTraceStringHandler);
				if (text.Contains("LINUX", StringComparison.Ordinal))
				{
					os = OSKind.Linux;
				}
				else if (text.Contains("DARWIN", StringComparison.Ordinal))
				{
					os = OSKind.OSX;
				}
				else if (text.Contains("BSD", StringComparison.Ordinal))
				{
					os = OSKind.BSD;
				}
				string text2 = PlatformDetection.GetMachineNamePosix(os, span).ToUpperInvariant();
				if (text2.Contains("X86_64", StringComparison.Ordinal) || text2.Contains("AMD64", StringComparison.Ordinal))
				{
					arch = ArchitectureKind.x86_64;
				}
				else if (text2.Contains("X86", StringComparison.Ordinal) || text2.Contains("I686", StringComparison.Ordinal))
				{
					arch = ArchitectureKind.x86;
				}
				else if (text2.Contains("AARCH64", StringComparison.Ordinal) || text2.Contains("ARM64", StringComparison.Ordinal))
				{
					arch = ArchitectureKind.Arm64;
				}
				else if (text2.Contains("ARM", StringComparison.Ordinal))
				{
					arch = ArchitectureKind.Arm;
				}
				MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new MMDbgLog.DebugLogTraceStringHandler(37, 2, out flag);
				if (flag)
				{
					debugLogTraceStringHandler2.AppendLiteral("uname() detected architecture info: ");
					debugLogTraceStringHandler2.AppendFormatted<OSKind>(os);
					debugLogTraceStringHandler2.AppendLiteral(" ");
					debugLogTraceStringHandler2.AppendFormatted<ArchitectureKind>(arch);
				}
				MMDbgLog.Trace(ref debugLogTraceStringHandler2);
			}
			catch (Exception ex)
			{
				bool flag;
				MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new MMDbgLog.DebugLogErrorStringHandler(49, 1, out flag);
				if (flag)
				{
					debugLogErrorStringHandler2.AppendLiteral("Error trying to detect info on POSIX-like system ");
					debugLogErrorStringHandler2.AppendFormatted<Exception>(ex);
				}
				MMDbgLog.Error(ref debugLogErrorStringHandler2);
			}
		}

		[return: Nullable(1)]
		private unsafe static string GetMachineNamePosix(OSKind os, global::System.Span<byte> unameBuffer)
		{
			string text = null;
			if (os == OSKind.Linux)
			{
				IntPtr intPtr;
				if (DynDll.OpenLibrary("libc").TryGetExport("getauxval", out intPtr))
				{
					delegate* unmanaged[Cdecl]<IntPtr, IntPtr> system.IntPtr_u0020(System.IntPtr) = (void*)intPtr;
					IntPtr intPtr2 = calli(System.IntPtr(System.IntPtr), (IntPtr)15, system.IntPtr_u0020(System.IntPtr));
					if (intPtr2 != 0)
					{
						text = Marshal.PtrToStringAnsi(intPtr2);
						bool flag;
						MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new MMDbgLog.DebugLogTraceStringHandler(35, 1, out flag);
						if (flag)
						{
							debugLogTraceStringHandler.AppendLiteral("Got architecture from getauxval(): ");
							debugLogTraceStringHandler.AppendFormatted(text);
						}
						MMDbgLog.Trace(ref debugLogTraceStringHandler);
					}
				}
				if (text == null)
				{
					try
					{
						global::System.Span<Unix.LinuxAuxvEntry> span = global::System.Runtime.InteropServices.MemoryMarshal.Cast<byte, Unix.LinuxAuxvEntry>(Helpers.ReadAllBytes("/proc/self/auxv").AsSpan<byte>());
						text = string.Empty;
						global::System.Span<Unix.LinuxAuxvEntry> span2 = span;
						for (int i = 0; i < span2.Length; i++)
						{
							Unix.LinuxAuxvEntry linuxAuxvEntry = *span2[i];
							if (linuxAuxvEntry.Key == (IntPtr)15)
							{
								text = Marshal.PtrToStringAnsi(linuxAuxvEntry.Value) ?? string.Empty;
								break;
							}
						}
						if (text.Length == 0)
						{
							bool flag;
							MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new MMDbgLog.DebugLogWarningStringHandler(56, 1, out flag);
							if (flag)
							{
								debugLogWarningStringHandler.AppendLiteral("Auxv table did not inlcude useful AT_PLATFORM (0x");
								debugLogWarningStringHandler.AppendFormatted<int>(15, "x");
								debugLogWarningStringHandler.AppendLiteral(") entry");
							}
							MMDbgLog.Warning(ref debugLogWarningStringHandler);
							global::System.Span<Unix.LinuxAuxvEntry> span3 = span;
							for (int i = 0; i < span3.Length; i++)
							{
								Unix.LinuxAuxvEntry linuxAuxvEntry2 = *span3[i];
								MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new MMDbgLog.DebugLogTraceStringHandler(3, 2, out flag);
								if (flag)
								{
									debugLogTraceStringHandler2.AppendFormatted<IntPtr>(linuxAuxvEntry2.Key, "x16");
									debugLogTraceStringHandler2.AppendLiteral(" = ");
									debugLogTraceStringHandler2.AppendFormatted<IntPtr>(linuxAuxvEntry2.Value, "x16");
								}
								MMDbgLog.Trace(ref debugLogTraceStringHandler2);
							}
							text = null;
						}
						else
						{
							bool flag;
							MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler3 = new MMDbgLog.DebugLogTraceStringHandler(43, 1, out flag);
							if (flag)
							{
								debugLogTraceStringHandler3.AppendLiteral("Got architecture name ");
								debugLogTraceStringHandler3.AppendFormatted(text);
								debugLogTraceStringHandler3.AppendLiteral(" from /proc/self/auxv");
							}
							MMDbgLog.Trace(ref debugLogTraceStringHandler3);
						}
					}
					catch (UnauthorizedAccessException ex)
					{
						MMDbgLog.Warning("Could not read /proc/self/auxv, and libc does not have getauxval");
						MMDbgLog.Warning("Falling back to parsing out of uname() result...");
						bool flag;
						MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler2 = new MMDbgLog.DebugLogWarningStringHandler(0, 1, out flag);
						if (flag)
						{
							debugLogWarningStringHandler2.AppendFormatted<UnauthorizedAccessException>(ex);
						}
						MMDbgLog.Warning(ref debugLogWarningStringHandler2);
					}
				}
			}
			if (text == null)
			{
				for (int j = 0; j < 4; j++)
				{
					if (j != 0)
					{
						int num = unameBuffer.IndexOf(0);
						unameBuffer = unameBuffer.Slice(num);
						if (j == 1 && num < 5 && unameBuffer.Length >= 2 && *unameBuffer[1] != 0)
						{
							num = unameBuffer.Slice(1).IndexOf(0);
							unameBuffer = unameBuffer.Slice(num + 1);
						}
					}
					int num2 = 0;
					while (num2 < unameBuffer.Length && *unameBuffer[num2] == 0)
					{
						num2++;
					}
					unameBuffer = unameBuffer.Slice(num2);
				}
				int i;
				text = PlatformDetection.GetCString(unameBuffer, out i);
				bool flag;
				MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler4 = new MMDbgLog.DebugLogTraceStringHandler(35, 1, out flag);
				if (flag)
				{
					debugLogTraceStringHandler4.AppendLiteral("Got architecture name ");
					debugLogTraceStringHandler4.AppendFormatted(text);
					debugLogTraceStringHandler4.AppendLiteral(" from uname()");
				}
				MMDbgLog.Trace(ref debugLogTraceStringHandler4);
			}
			return text;
		}

		private unsafe static void DetectInfoWindows(ref OSKind os, ref ArchitectureKind arch)
		{
			Windows.SYSTEM_INFO system_INFO;
			Windows.GetSystemInfo(&system_INFO);
			ushort wProcessorArchitecture = system_INFO.Anonymous.Anonymous.wProcessorArchitecture;
			ArchitectureKind architectureKind;
			if (wProcessorArchitecture != 0)
			{
				switch (wProcessorArchitecture)
				{
				case 5:
					architectureKind = ArchitectureKind.Arm;
					goto IL_0085;
				case 6:
					throw new PlatformNotSupportedException("You're running .NET on an Itanium device!?!?");
				case 7:
				case 8:
					break;
				case 9:
					architectureKind = ArchitectureKind.x86_64;
					goto IL_0085;
				default:
					if (wProcessorArchitecture == 12)
					{
						architectureKind = ArchitectureKind.Arm64;
						goto IL_0085;
					}
					break;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Unknown Windows processor architecture ");
				defaultInterpolatedStringHandler.AppendFormatted<ushort>(wProcessorArchitecture);
				throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			architectureKind = ArchitectureKind.x86;
			IL_0085:
			arch = architectureKind;
		}

		private unsafe static bool CheckWine()
		{
			bool flag;
			if (Switches.TryGetSwitchEnabled("RunningOnWine", out flag))
			{
				return flag;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("XL_WINEONLINUX");
			string text = ((environmentVariable != null) ? environmentVariable.ToUpperInvariant() : null);
			if (text == "TRUE")
			{
				return true;
			}
			if (text == "FALSE")
			{
				return false;
			}
			fixed (char* pinnableReference = "ntdll.dll".AsSpan().GetPinnableReference())
			{
				Windows.HMODULE moduleHandleW = Windows.GetModuleHandleW((ushort*)pinnableReference);
				if (moduleHandleW != Windows.HMODULE.NULL && moduleHandleW != Windows.HMODULE.INVALID_VALUE)
				{
					fixed (byte* pinnableReference2 = new global::System.ReadOnlySpan<byte>((void*)(&<PrivateImplementationDetails>.0A3EBE02DD250439043520A24AEF10F9F051F5747BD28A93500A5C734CC975A9), 14).GetPinnableReference())
					{
						byte* ptr = pinnableReference2;
						if (Windows.GetProcAddress(moduleHandleW, (sbyte*)ptr) != IntPtr.Zero)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		[MemberNotNull("runtimeVersion")]
		private static void EnsureRuntimeInitialized()
		{
			if (PlatformDetection.runtimeInitState == 0)
			{
				ValueTuple<RuntimeKind, CorelibKind, Version> valueTuple = PlatformDetection.DetermineRuntimeInfo();
				PlatformDetection.runtime = valueTuple.Item1;
				PlatformDetection.corelib = valueTuple.Item2;
				PlatformDetection.runtimeVersion = valueTuple.Item3;
				Thread.MemoryBarrier();
				Interlocked.Exchange(ref PlatformDetection.runtimeInitState, 1);
				return;
			}
			if (PlatformDetection.runtimeVersion == null)
			{
				throw new InvalidOperationException("Despite runtimeInitState being set, runtimeVersion was somehow null");
			}
		}

		public static RuntimeKind Runtime
		{
			get
			{
				PlatformDetection.EnsureRuntimeInitialized();
				return PlatformDetection.runtime;
			}
		}

		public static CorelibKind Corelib
		{
			get
			{
				PlatformDetection.EnsureRuntimeInitialized();
				return PlatformDetection.corelib;
			}
		}

		[Nullable(1)]
		public static Version RuntimeVersion
		{
			[NullableContext(1)]
			get
			{
				PlatformDetection.EnsureRuntimeInitialized();
				return PlatformDetection.runtimeVersion;
			}
		}

		[return: TupleElementNames(new string[] { "Rt", "Cor", "Ver" })]
		[return: Nullable(new byte[] { 0, 1 })]
		private static ValueTuple<RuntimeKind, CorelibKind, Version> DetermineRuntimeInfo()
		{
			Version version = null;
			bool flag = Type.GetType("Mono.Runtime") != null || Type.GetType("Mono.RuntimeStructs") != null;
			bool flag2 = typeof(object).Assembly.GetName().Name == "System.Private.CoreLib";
			CorelibKind corelibKind = (flag2 ? CorelibKind.Core : CorelibKind.Framework);
			RuntimeKind runtimeKind;
			if (flag)
			{
				runtimeKind = RuntimeKind.Mono;
			}
			else if (flag2 && !flag)
			{
				runtimeKind = RuntimeKind.CoreCLR;
			}
			else
			{
				runtimeKind = RuntimeKind.Framework;
			}
			bool flag3;
			MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new MMDbgLog.DebugLogTraceStringHandler(21, 2, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler.AppendLiteral("IsMono: ");
				debugLogTraceStringHandler.AppendFormatted<bool>(flag);
				debugLogTraceStringHandler.AppendLiteral(", IsCoreBcl: ");
				debugLogTraceStringHandler.AppendFormatted<bool>(flag2);
			}
			MMDbgLog.Trace(ref debugLogTraceStringHandler);
			Version version2 = Environment.Version;
			MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new MMDbgLog.DebugLogTraceStringHandler(25, 1, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler2.AppendLiteral("Returned system version: ");
				debugLogTraceStringHandler2.AppendFormatted<Version>(version2);
			}
			MMDbgLog.Trace(ref debugLogTraceStringHandler2);
			Type type = Type.GetType("System.Runtime.InteropServices.RuntimeInformation");
			if (type == null)
			{
				type = Type.GetType("System.Runtime.InteropServices.RuntimeInformation, System.Runtime.InteropServices.RuntimeInformation");
			}
			object obj;
			if (type == null)
			{
				obj = null;
			}
			else
			{
				PropertyInfo property = type.GetProperty("FrameworkDescription");
				obj = ((property != null) ? property.GetValue(null, null) : null);
			}
			string text = (string)obj;
			MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler3 = new MMDbgLog.DebugLogTraceStringHandler(22, 1, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler3.AppendLiteral("FrameworkDescription: ");
				debugLogTraceStringHandler3.AppendFormatted(text ?? "(null)");
			}
			MMDbgLog.Trace(ref debugLogTraceStringHandler3);
			if (text != null)
			{
				int num;
				if (text.StartsWith("Mono ", StringComparison.Ordinal))
				{
					runtimeKind = RuntimeKind.Mono;
					num = "Mono ".Length;
				}
				else if (text.StartsWith(".NET Core ", StringComparison.Ordinal))
				{
					runtimeKind = RuntimeKind.CoreCLR;
					num = ".NET Core ".Length;
				}
				else if (text.StartsWith(".NET Framework ", StringComparison.Ordinal))
				{
					runtimeKind = RuntimeKind.Framework;
					num = ".NET Framework ".Length;
				}
				else if (text.StartsWith(".NET ", StringComparison.Ordinal))
				{
					runtimeKind = (flag ? RuntimeKind.Mono : RuntimeKind.CoreCLR);
					num = ".NET ".Length;
				}
				else
				{
					runtimeKind = RuntimeKind.Unknown;
					num = text.Length;
				}
				int num2 = text.IndexOfAny(new char[] { ' ', '-' }, num);
				if (num2 < 0)
				{
					num2 = text.Length;
				}
				string text2 = text.Substring(num, num2 - num);
				try
				{
					version = new Version(text2);
				}
				catch (Exception ex)
				{
					MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new MMDbgLog.DebugLogErrorStringHandler(61, 2, out flag3);
					if (flag3)
					{
						debugLogErrorStringHandler.AppendLiteral("Invalid version string pulled from FrameworkDescription ('");
						debugLogErrorStringHandler.AppendFormatted(text);
						debugLogErrorStringHandler.AppendLiteral("') ");
						debugLogErrorStringHandler.AppendFormatted<Exception>(ex);
					}
					MMDbgLog.Error(ref debugLogErrorStringHandler);
				}
			}
			if (runtimeKind == RuntimeKind.Framework && version == null)
			{
				version = version2;
			}
			MMDbgLog.DebugLogInfoStringHandler debugLogInfoStringHandler = new MMDbgLog.DebugLogInfoStringHandler(34, 3, out flag3);
			if (flag3)
			{
				debugLogInfoStringHandler.AppendLiteral("Detected runtime: ");
				debugLogInfoStringHandler.AppendFormatted<RuntimeKind>(runtimeKind);
				debugLogInfoStringHandler.AppendLiteral(" ");
				debugLogInfoStringHandler.AppendFormatted(((version != null) ? version.ToString() : null) ?? "(null)");
				debugLogInfoStringHandler.AppendLiteral(" using ");
				debugLogInfoStringHandler.AppendFormatted<CorelibKind>(corelibKind);
				debugLogInfoStringHandler.AppendLiteral(" corelib");
			}
			MMDbgLog.Info(ref debugLogInfoStringHandler);
			return new ValueTuple<RuntimeKind, CorelibKind, Version>(runtimeKind, corelibKind, version ?? new Version(0, 0));
		}

		[CompilerGenerated]
		internal unsafe static int <PosixUname>g__Libc|9_0(byte* buf)
		{
			return Unix.Uname(buf);
		}

		[CompilerGenerated]
		internal unsafe static int <PosixUname>g__Osx|9_1(byte* buf)
		{
			return OSX.Uname(buf);
		}

		private static int platInitState;

		private static OSKind os;

		private static ArchitectureKind arch;

		private static int runtimeInitState;

		private static RuntimeKind runtime;

		private static CorelibKind corelib;

		[Nullable(2)]
		private static Version runtimeVersion;
	}
}
