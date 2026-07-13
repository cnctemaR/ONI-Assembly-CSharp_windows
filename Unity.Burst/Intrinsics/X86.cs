using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;

namespace Unity.Burst.Intrinsics
{
	[BurstCompile]
	public static class X86
	{
		private unsafe static v128 GenericCSharpLoad(void* ptr)
		{
			return *(v128*)ptr;
		}

		private unsafe static void GenericCSharpStore(void* ptr, v128 val)
		{
			*(v128*)ptr = val;
		}

		private static sbyte Saturate_To_Int8(int val)
		{
			if (val > 127)
			{
				return sbyte.MaxValue;
			}
			if (val < -128)
			{
				return sbyte.MinValue;
			}
			return (sbyte)val;
		}

		private static byte Saturate_To_UnsignedInt8(int val)
		{
			if (val > 255)
			{
				return byte.MaxValue;
			}
			if (val < 0)
			{
				return 0;
			}
			return (byte)val;
		}

		private static short Saturate_To_Int16(int val)
		{
			if (val > 32767)
			{
				return short.MaxValue;
			}
			if (val < -32768)
			{
				return short.MinValue;
			}
			return (short)val;
		}

		private static ushort Saturate_To_UnsignedInt16(int val)
		{
			if (val > 65535)
			{
				return ushort.MaxValue;
			}
			if (val < 0)
			{
				return 0;
			}
			return (ushort)val;
		}

		private static bool IsNaN(uint v)
		{
			return (v & 2147483647U) > 2139095040U;
		}

		private static bool IsNaN(ulong v)
		{
			return (v & 9223372036854775807UL) > 9218868437227405312UL;
		}

		private static void BurstIntrinsicSetCSRFromManaged(int _)
		{
		}

		private static int BurstIntrinsicGetCSRFromManaged()
		{
			return 0;
		}

		internal static int getcsr_raw()
		{
			return X86.DoGetCSRTrampoline();
		}

		internal static void setcsr_raw(int bits)
		{
			X86.DoSetCSRTrampoline(bits);
		}

		[BurstCompile(CompileSynchronously = true)]
		[MonoPInvokeCallback(typeof(X86.DoSetCSRTrampoline_00000129$PostfixBurstDelegate))]
		private static void DoSetCSRTrampoline(int bits)
		{
			X86.DoSetCSRTrampoline_00000129$BurstDirectCall.Invoke(bits);
		}

		[BurstCompile(CompileSynchronously = true)]
		[MonoPInvokeCallback(typeof(X86.DoGetCSRTrampoline_0000012A$PostfixBurstDelegate))]
		private static int DoGetCSRTrampoline()
		{
			return X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.Invoke();
		}

		public static X86.MXCSRBits MXCSR
		{
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			get
			{
				return (X86.MXCSRBits)X86.getcsr_raw();
			}
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			set
			{
				X86.setcsr_raw((int)value);
			}
		}

		[BurstCompile(CompileSynchronously = true)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void DoSetCSRTrampoline$BurstManaged(int bits)
		{
			if (X86.Sse.IsSseSupported)
			{
				X86.BurstIntrinsicSetCSRFromManaged(bits);
			}
		}

		[BurstCompile(CompileSynchronously = true)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int DoGetCSRTrampoline$BurstManaged()
		{
			if (X86.Sse.IsSseSupported)
			{
				return X86.BurstIntrinsicGetCSRFromManaged();
			}
			return 0;
		}

		public static class Avx
		{
			public static bool IsAvxSupported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.add_pd(a.Lo128, b.Lo128), X86.Sse2.add_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.add_ps(a.Lo128, b.Lo128), X86.Sse.add_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_addsub_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse3.addsub_pd(a.Lo128, b.Lo128), X86.Sse3.addsub_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_addsub_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse3.addsub_ps(a.Lo128, b.Lo128), X86.Sse3.addsub_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_and_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.and_pd(a.Lo128, b.Lo128), X86.Sse2.and_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_and_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.and_ps(a.Lo128, b.Lo128), X86.Sse.and_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_andnot_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.andnot_pd(a.Lo128, b.Lo128), X86.Sse2.andnot_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_andnot_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.andnot_ps(a.Lo128, b.Lo128), X86.Sse.andnot_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blend_pd(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse4_1.blend_pd(a.Lo128, b.Lo128, imm8 & 3), X86.Sse4_1.blend_pd(a.Hi128, b.Hi128, imm8 >> 2));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blend_ps(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse4_1.blend_ps(a.Lo128, b.Lo128, imm8 & 15), X86.Sse4_1.blend_ps(a.Hi128, b.Hi128, imm8 >> 4));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blendv_pd(v256 a, v256 b, v256 mask)
			{
				return new v256(X86.Sse4_1.blendv_pd(a.Lo128, b.Lo128, mask.Lo128), X86.Sse4_1.blendv_pd(a.Hi128, b.Hi128, mask.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blendv_ps(v256 a, v256 b, v256 mask)
			{
				return new v256(X86.Sse4_1.blendv_ps(a.Lo128, b.Lo128, mask.Lo128), X86.Sse4_1.blendv_ps(a.Hi128, b.Hi128, mask.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_div_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.div_pd(a.Lo128, b.Lo128), X86.Sse2.div_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_div_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.div_ps(a.Lo128, b.Lo128), X86.Sse.div_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_dp_ps(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse4_1.dp_ps(a.Lo128, b.Lo128, imm8), X86.Sse4_1.dp_ps(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hadd_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse3.hadd_pd(a.Lo128, b.Lo128), X86.Sse3.hadd_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hadd_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse3.hadd_ps(a.Lo128, b.Lo128), X86.Sse3.hadd_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hsub_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse3.hsub_pd(a.Lo128, b.Lo128), X86.Sse3.hsub_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hsub_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse3.hsub_ps(a.Lo128, b.Lo128), X86.Sse3.hsub_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.max_pd(a.Lo128, b.Lo128), X86.Sse2.max_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.max_ps(a.Lo128, b.Lo128), X86.Sse.max_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.min_pd(a.Lo128, b.Lo128), X86.Sse2.min_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.min_ps(a.Lo128, b.Lo128), X86.Sse.min_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mul_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.mul_pd(a.Lo128, b.Lo128), X86.Sse2.mul_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mul_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.mul_ps(a.Lo128, b.Lo128), X86.Sse.mul_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_or_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.or_pd(a.Lo128, b.Lo128), X86.Sse2.or_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_or_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.or_ps(a.Lo128, b.Lo128), X86.Sse.or_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shuffle_pd(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse2.shuffle_pd(a.Lo128, b.Lo128, imm8 & 3), X86.Sse2.shuffle_pd(a.Hi128, b.Hi128, imm8 >> 2));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shuffle_ps(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse.shuffle_ps(a.Lo128, b.Lo128, imm8), X86.Sse.shuffle_ps(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sub_pd(a.Lo128, b.Lo128), X86.Sse2.sub_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.sub_ps(a.Lo128, b.Lo128), X86.Sse.sub_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_xor_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.xor_pd(a.Lo128, b.Lo128), X86.Sse2.xor_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_xor_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.xor_ps(a.Lo128, b.Lo128), X86.Sse.xor_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v128 cmp_pd(v128 a, v128 b, int imm8)
			{
				switch (imm8 & 31)
				{
				case 0:
					return X86.Sse2.cmpeq_pd(a, b);
				case 1:
					return X86.Sse2.cmplt_pd(a, b);
				case 2:
					return X86.Sse2.cmple_pd(a, b);
				case 3:
					return X86.Sse2.cmpunord_pd(a, b);
				case 4:
					return X86.Sse2.cmpneq_pd(a, b);
				case 5:
					return X86.Sse2.cmpnlt_pd(a, b);
				case 6:
					return X86.Sse2.cmpnle_pd(a, b);
				case 7:
					return X86.Sse2.cmpord_pd(a, b);
				case 8:
					return X86.Sse2.or_pd(X86.Sse2.cmpeq_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 9:
					return X86.Sse2.or_pd(X86.Sse2.cmpnge_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 10:
					return X86.Sse2.or_pd(X86.Sse2.cmpngt_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 11:
					return default(v128);
				case 12:
					return X86.Sse2.and_pd(X86.Sse2.cmpneq_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 13:
					return X86.Sse2.and_pd(X86.Sse2.cmpge_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 14:
					return X86.Sse2.and_pd(X86.Sse2.cmpgt_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 15:
					return new v128(-1);
				case 16:
					return X86.Sse2.and_pd(X86.Sse2.cmpeq_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 17:
					return X86.Sse2.and_pd(X86.Sse2.cmplt_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 18:
					return X86.Sse2.and_pd(X86.Sse2.cmple_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 19:
					return X86.Sse2.cmpunord_pd(a, b);
				case 20:
					return X86.Sse2.cmpneq_pd(a, b);
				case 21:
					return X86.Sse2.or_pd(X86.Sse2.cmpnlt_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 22:
					return X86.Sse2.or_pd(X86.Sse2.cmpnle_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 23:
					return X86.Sse2.cmpord_pd(a, b);
				case 24:
					return X86.Sse2.or_pd(X86.Sse2.cmpeq_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 25:
					return X86.Sse2.or_pd(X86.Sse2.cmpnge_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 26:
					return X86.Sse2.or_pd(X86.Sse2.cmpngt_pd(a, b), X86.Sse2.cmpunord_pd(a, b));
				case 27:
					return default(v128);
				case 28:
					return X86.Sse2.and_pd(X86.Sse2.cmpneq_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 29:
					return X86.Sse2.and_pd(X86.Sse2.cmpge_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				case 30:
					return X86.Sse2.and_pd(X86.Sse2.cmpgt_pd(a, b), X86.Sse2.cmpord_pd(a, b));
				default:
					return new v128(-1);
				}
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmp_pd(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Avx.cmp_pd(a.Lo128, b.Lo128, imm8), X86.Avx.cmp_pd(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v128 cmp_ps(v128 a, v128 b, int imm8)
			{
				switch (imm8 & 31)
				{
				case 0:
					return X86.Sse.cmpeq_ps(a, b);
				case 1:
					return X86.Sse.cmplt_ps(a, b);
				case 2:
					return X86.Sse.cmple_ps(a, b);
				case 3:
					return X86.Sse.cmpunord_ps(a, b);
				case 4:
					return X86.Sse.cmpneq_ps(a, b);
				case 5:
					return X86.Sse.cmpnlt_ps(a, b);
				case 6:
					return X86.Sse.cmpnle_ps(a, b);
				case 7:
					return X86.Sse.cmpord_ps(a, b);
				case 8:
					return X86.Sse.or_ps(X86.Sse.cmpeq_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 9:
					return X86.Sse.or_ps(X86.Sse.cmpnge_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 10:
					return X86.Sse.or_ps(X86.Sse.cmpngt_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 11:
					return default(v128);
				case 12:
					return X86.Sse.and_ps(X86.Sse.cmpneq_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 13:
					return X86.Sse.and_ps(X86.Sse.cmpge_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 14:
					return X86.Sse.and_ps(X86.Sse.cmpgt_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 15:
					return new v128(-1);
				case 16:
					return X86.Sse.and_ps(X86.Sse.cmpeq_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 17:
					return X86.Sse.and_ps(X86.Sse.cmplt_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 18:
					return X86.Sse.and_ps(X86.Sse.cmple_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 19:
					return X86.Sse.cmpunord_ps(a, b);
				case 20:
					return X86.Sse.cmpneq_ps(a, b);
				case 21:
					return X86.Sse.or_ps(X86.Sse.cmpnlt_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 22:
					return X86.Sse.or_ps(X86.Sse.cmpnle_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 23:
					return X86.Sse.cmpord_ps(a, b);
				case 24:
					return X86.Sse.or_ps(X86.Sse.cmpeq_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 25:
					return X86.Sse.or_ps(X86.Sse.cmpnge_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 26:
					return X86.Sse.or_ps(X86.Sse.cmpngt_ps(a, b), X86.Sse.cmpunord_ps(a, b));
				case 27:
					return default(v128);
				case 28:
					return X86.Sse.and_ps(X86.Sse.cmpneq_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 29:
					return X86.Sse.and_ps(X86.Sse.cmpge_ps(a, b), X86.Sse.cmpord_ps(a, b));
				case 30:
					return X86.Sse.and_ps(X86.Sse.cmpgt_ps(a, b), X86.Sse.cmpord_ps(a, b));
				default:
					return new v128(-1);
				}
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmp_ps(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Avx.cmp_ps(a.Lo128, b.Lo128, imm8), X86.Avx.cmp_ps(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v128 cmp_sd(v128 a, v128 b, int imm8)
			{
				return new v128(X86.Avx.cmp_pd(a, b, imm8).ULong0, a.ULong1);
			}

			[DebuggerStepThrough]
			public static v128 cmp_ss(v128 a, v128 b, int imm8)
			{
				return new v128(X86.Avx.cmp_ps(a, b, imm8).UInt0, a.UInt1, a.UInt2, a.UInt3);
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvtepi32_pd(v128 a)
			{
				return new v256((double)a.SInt0, (double)a.SInt1, (double)a.SInt2, (double)a.SInt3);
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvtepi32_ps(v256 a)
			{
				return new v256(X86.Sse2.cvtepi32_ps(a.Lo128), X86.Sse2.cvtepi32_ps(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v128 mm256_cvtpd_ps(v256 a)
			{
				v128 v = X86.Sse2.cvtpd_ps(a.Lo128);
				v128 v2 = X86.Sse2.cvtpd_ps(a.Hi128);
				return new v128(v.Float0, v.Float1, v2.Float0, v2.Float1);
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvtps_epi32(v256 a)
			{
				return new v256(X86.Sse2.cvtps_epi32(a.Lo128), X86.Sse2.cvtps_epi32(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvtps_pd(v128 a)
			{
				return new v256((double)a.Float0, (double)a.Float1, (double)a.Float2, (double)a.Float3);
			}

			[DebuggerStepThrough]
			public static v128 mm256_cvttpd_epi32(v256 a)
			{
				return new v128((int)a.Double0, (int)a.Double1, (int)a.Double2, (int)a.Double3);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v128 mm256_cvtpd_epi32(v256 a)
			{
				v128 v = X86.Sse2.cvtpd_epi32(new v128(a.Double0, a.Double1));
				v128 v2 = X86.Sse2.cvtpd_epi32(new v128(a.Double2, a.Double3));
				return new v128(v.SInt0, v.SInt1, v2.SInt0, v2.SInt1);
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvttps_epi32(v256 a)
			{
				return new v256(X86.Sse2.cvttps_epi32(a.Lo128), X86.Sse2.cvttps_epi32(a.Hi128));
			}

			[DebuggerStepThrough]
			public static float mm256_cvtss_f32(v256 a)
			{
				return a.Float0;
			}

			[DebuggerStepThrough]
			public static v128 mm256_extractf128_ps(v256 a, int imm8)
			{
				if (imm8 == 0)
				{
					return a.Lo128;
				}
				return a.Hi128;
			}

			[DebuggerStepThrough]
			public static v128 mm256_extractf128_pd(v256 a, int imm8)
			{
				if (imm8 == 0)
				{
					return a.Lo128;
				}
				return a.Hi128;
			}

			[DebuggerStepThrough]
			public static v128 mm256_extractf128_si256(v256 a, int imm8)
			{
				if (imm8 == 0)
				{
					return a.Lo128;
				}
				return a.Hi128;
			}

			[DebuggerStepThrough]
			public static void mm256_zeroall()
			{
			}

			[DebuggerStepThrough]
			public static void mm256_zeroupper()
			{
			}

			[DebuggerStepThrough]
			public unsafe static v128 permutevar_ps(v128 a, v128 b)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i < 4; i++)
				{
					int num = ptr3[i] & 3;
					ptr[i] = ptr2[num];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_permutevar_ps(v256 a, v256 b)
			{
				return new v256(X86.Avx.permutevar_ps(a.Lo128, b.Lo128), X86.Avx.permutevar_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v128 permute_ps(v128 a, int imm8)
			{
				return X86.Sse2.shuffle_epi32(a, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute_ps(v256 a, int imm8)
			{
				return new v256(X86.Avx.permute_ps(a.Lo128, imm8), X86.Avx.permute_ps(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public unsafe static v128 permutevar_pd(v128 a, v128 b)
			{
				v128 v = default(v128);
				double* ptr = &v.Double0;
				double* ptr2 = &a.Double0;
				*ptr = ptr2[(int)(b.SLong0 & 2L) >> 1];
				ptr[1] = ptr2[(int)(b.SLong1 & 2L) >> 1];
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_permutevar_pd(v256 a, v256 b)
			{
				v256 v = default(v256);
				double* ptr = &v.Double0;
				double* ptr2 = &a.Double0;
				*ptr = ptr2[(int)(b.SLong0 & 2L) >> 1];
				ptr[1] = ptr2[(int)(b.SLong1 & 2L) >> 1];
				ptr[2] = ptr2[2 + ((int)(b.SLong2 & 2L) >> 1)];
				ptr[3] = ptr2[2 + ((int)(b.SLong3 & 2L) >> 1)];
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute_pd(v256 a, int imm8)
			{
				return new v256(X86.Avx.permute_pd(a.Lo128, imm8 & 3), X86.Avx.permute_pd(a.Hi128, imm8 >> 2));
			}

			[DebuggerStepThrough]
			public unsafe static v128 permute_pd(v128 a, int imm8)
			{
				v128 v = default(v128);
				double* ptr = &v.Double0;
				double* ptr2 = &a.Double0;
				*ptr = ptr2[imm8 & 1];
				ptr[1] = ptr2[(imm8 >> 1) & 1];
				return v;
			}

			private static v128 Select4(v256 src1, v256 src2, int control)
			{
				switch (control & 3)
				{
				case 0:
					return src1.Lo128;
				case 1:
					return src1.Hi128;
				case 2:
					return src2.Lo128;
				default:
					return src2.Hi128;
				}
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute2f128_ps(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Avx.Select4(a, b, imm8), X86.Avx.Select4(a, b, imm8 >> 4));
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute2f128_pd(v256 a, v256 b, int imm8)
			{
				return X86.Avx.mm256_permute2f128_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute2f128_si256(v256 a, v256 b, int imm8)
			{
				return X86.Avx.mm256_permute2f128_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_broadcast_ss(void* ptr)
			{
				return new v256(*(uint*)ptr);
			}

			[DebuggerStepThrough]
			public unsafe static v128 broadcast_ss(void* ptr)
			{
				return new v128(*(uint*)ptr);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_broadcast_sd(void* ptr)
			{
				return new v256(*(double*)ptr);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_broadcast_ps(void* ptr)
			{
				v128 v = X86.Sse.loadu_ps(ptr);
				return new v256(v, v);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_broadcast_pd(void* ptr)
			{
				return X86.Avx.mm256_broadcast_ps(ptr);
			}

			[DebuggerStepThrough]
			public static v256 mm256_insertf128_ps(v256 a, v128 b, int imm8)
			{
				if ((imm8 & 1) == 0)
				{
					return new v256(b, a.Hi128);
				}
				return new v256(a.Lo128, b);
			}

			[DebuggerStepThrough]
			public static v256 mm256_insertf128_pd(v256 a, v128 b, int imm8)
			{
				return X86.Avx.mm256_insertf128_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_insertf128_si256(v256 a, v128 b, int imm8)
			{
				return X86.Avx.mm256_insertf128_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_load_ps(void* ptr)
			{
				return *(v256*)ptr;
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_store_ps(void* ptr, v256 val)
			{
				*(v256*)ptr = val;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_load_pd(void* ptr)
			{
				return X86.Avx.mm256_load_ps(ptr);
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_store_pd(void* ptr, v256 a)
			{
				X86.Avx.mm256_store_ps(ptr, a);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_loadu_pd(void* ptr)
			{
				return X86.Avx.mm256_load_ps(ptr);
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_storeu_pd(void* ptr, v256 a)
			{
				X86.Avx.mm256_store_ps(ptr, a);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_loadu_ps(void* ptr)
			{
				return X86.Avx.mm256_load_ps(ptr);
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_storeu_ps(void* ptr, v256 a)
			{
				X86.Avx.mm256_store_ps(ptr, a);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_load_si256(void* ptr)
			{
				return X86.Avx.mm256_load_ps(ptr);
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_store_si256(void* ptr, v256 v)
			{
				X86.Avx.mm256_store_ps(ptr, v);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_loadu_si256(void* ptr)
			{
				return X86.Avx.mm256_load_ps(ptr);
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_storeu_si256(void* ptr, v256 v)
			{
				X86.Avx.mm256_store_ps(ptr, v);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static v256 mm256_loadu2_m128(void* hiaddr, void* loaddr)
			{
				return X86.Avx.mm256_set_m128(X86.Sse.loadu_ps(hiaddr), X86.Sse.loadu_ps(loaddr));
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static v256 mm256_loadu2_m128d(void* hiaddr, void* loaddr)
			{
				return X86.Avx.mm256_loadu2_m128(hiaddr, loaddr);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static v256 mm256_loadu2_m128i(void* hiaddr, void* loaddr)
			{
				return X86.Avx.mm256_loadu2_m128(hiaddr, loaddr);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_m128(v128 hi, v128 lo)
			{
				return new v256(lo, hi);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static void mm256_storeu2_m128(void* hiaddr, void* loaddr, v256 val)
			{
				X86.Sse.storeu_ps(hiaddr, val.Hi128);
				X86.Sse.storeu_ps(loaddr, val.Lo128);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static void mm256_storeu2_m128d(void* hiaddr, void* loaddr, v256 val)
			{
				X86.Sse.storeu_ps(hiaddr, val.Hi128);
				X86.Sse.storeu_ps(loaddr, val.Lo128);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public unsafe static void mm256_storeu2_m128i(void* hiaddr, void* loaddr, v256 val)
			{
				X86.Sse.storeu_ps(hiaddr, val.Hi128);
				X86.Sse.storeu_ps(loaddr, val.Lo128);
			}

			[DebuggerStepThrough]
			public unsafe static v128 maskload_pd(void* mem_addr, v128 mask)
			{
				v128 v = default(v128);
				if (mask.SLong0 < 0L)
				{
					v.ULong0 = (ulong)(*(long*)mem_addr);
				}
				if (mask.SLong1 < 0L)
				{
					v.ULong1 = (ulong)(*(long*)((byte*)mem_addr + 8));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_maskload_pd(void* mem_addr, v256 mask)
			{
				return new v256(X86.Avx.maskload_pd(mem_addr, mask.Lo128), X86.Avx.maskload_pd((void*)((byte*)mem_addr + 16), mask.Hi128));
			}

			[DebuggerStepThrough]
			public unsafe static void maskstore_pd(void* mem_addr, v128 mask, v128 a)
			{
				if (mask.SLong0 < 0L)
				{
					*(long*)mem_addr = (long)a.ULong0;
				}
				if (mask.SLong1 < 0L)
				{
					*(long*)((byte*)mem_addr + 8) = (long)a.ULong1;
				}
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_maskstore_pd(void* mem_addr, v256 mask, v256 a)
			{
				X86.Avx.maskstore_pd(mem_addr, mask.Lo128, a.Lo128);
				X86.Avx.maskstore_pd((void*)((byte*)mem_addr + 16), mask.Hi128, a.Hi128);
			}

			[DebuggerStepThrough]
			public unsafe static v128 maskload_ps(void* mem_addr, v128 mask)
			{
				v128 v = default(v128);
				if (mask.SInt0 < 0)
				{
					v.UInt0 = *(uint*)mem_addr;
				}
				if (mask.SInt1 < 0)
				{
					v.UInt1 = *(uint*)((byte*)mem_addr + 4);
				}
				if (mask.SInt2 < 0)
				{
					v.UInt2 = *(uint*)((byte*)mem_addr + (IntPtr)2 * 4);
				}
				if (mask.SInt3 < 0)
				{
					v.UInt3 = *(uint*)((byte*)mem_addr + (IntPtr)3 * 4);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_maskload_ps(void* mem_addr, v256 mask)
			{
				return new v256(X86.Avx.maskload_ps(mem_addr, mask.Lo128), X86.Avx.maskload_ps((void*)((byte*)mem_addr + 16), mask.Hi128));
			}

			[DebuggerStepThrough]
			public unsafe static void maskstore_ps(void* mem_addr, v128 mask, v128 a)
			{
				if (mask.SInt0 < 0)
				{
					*(int*)mem_addr = (int)a.UInt0;
				}
				if (mask.SInt1 < 0)
				{
					*(int*)((byte*)mem_addr + 4) = (int)a.UInt1;
				}
				if (mask.SInt2 < 0)
				{
					*(int*)((byte*)mem_addr + (IntPtr)2 * 4) = (int)a.UInt2;
				}
				if (mask.SInt3 < 0)
				{
					*(int*)((byte*)mem_addr + (IntPtr)3 * 4) = (int)a.UInt3;
				}
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_maskstore_ps(void* mem_addr, v256 mask, v256 a)
			{
				X86.Avx.maskstore_ps(mem_addr, mask.Lo128, a.Lo128);
				X86.Avx.maskstore_ps((void*)((byte*)mem_addr + 16), mask.Hi128, a.Hi128);
			}

			[DebuggerStepThrough]
			public static v256 mm256_movehdup_ps(v256 a)
			{
				return new v256(a.UInt1, a.UInt1, a.UInt3, a.UInt3, a.UInt5, a.UInt5, a.UInt7, a.UInt7);
			}

			[DebuggerStepThrough]
			public static v256 mm256_moveldup_ps(v256 a)
			{
				return new v256(a.UInt0, a.UInt0, a.UInt2, a.UInt2, a.UInt4, a.UInt4, a.UInt6, a.UInt6);
			}

			[DebuggerStepThrough]
			public static v256 mm256_movedup_pd(v256 a)
			{
				return new v256(a.Double0, a.Double0, a.Double2, a.Double2);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_lddqu_si256(void* mem_addr)
			{
				return *(v256*)mem_addr;
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_stream_si256(void* mem_addr, v256 a)
			{
				*(v256*)mem_addr = a;
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_stream_pd(void* mem_addr, v256 a)
			{
				*(v256*)mem_addr = a;
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_stream_ps(void* mem_addr, v256 a)
			{
				*(v256*)mem_addr = a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_rcp_ps(v256 a)
			{
				return new v256(X86.Sse.rcp_ps(a.Lo128), X86.Sse.rcp_ps(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_rsqrt_ps(v256 a)
			{
				return new v256(X86.Sse.rsqrt_ps(a.Lo128), X86.Sse.rsqrt_ps(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sqrt_pd(v256 a)
			{
				return new v256(X86.Sse2.sqrt_pd(a.Lo128), X86.Sse2.sqrt_pd(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sqrt_ps(v256 a)
			{
				return new v256(X86.Sse.sqrt_ps(a.Lo128), X86.Sse.sqrt_ps(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_round_pd(v256 a, int rounding)
			{
				return new v256(X86.Sse4_1.round_pd(a.Lo128, rounding), X86.Sse4_1.round_pd(a.Hi128, rounding));
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_ceil_pd(v256 val)
			{
				return X86.Avx.mm256_round_pd(val, 2);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_floor_pd(v256 val)
			{
				return X86.Avx.mm256_round_pd(val, 1);
			}

			[DebuggerStepThrough]
			public static v256 mm256_round_ps(v256 a, int rounding)
			{
				return new v256(X86.Sse4_1.round_ps(a.Lo128, rounding), X86.Sse4_1.round_ps(a.Hi128, rounding));
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_ceil_ps(v256 val)
			{
				return X86.Avx.mm256_round_ps(val, 2);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_floor_ps(v256 val)
			{
				return X86.Avx.mm256_round_ps(val, 1);
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpackhi_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpackhi_pd(a.Lo128, b.Lo128), X86.Sse2.unpackhi_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpacklo_pd(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpacklo_pd(a.Lo128, b.Lo128), X86.Sse2.unpacklo_pd(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_unpackhi_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.unpackhi_ps(a.Lo128, b.Lo128), X86.Sse.unpackhi_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_unpacklo_ps(v256 a, v256 b)
			{
				return new v256(X86.Sse.unpacklo_ps(a.Lo128, b.Lo128), X86.Sse.unpacklo_ps(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static int mm256_testz_si256(v256 a, v256 b)
			{
				return X86.Sse4_1.testz_si128(a.Lo128, b.Lo128) & X86.Sse4_1.testz_si128(a.Hi128, b.Hi128);
			}

			[DebuggerStepThrough]
			public static int mm256_testc_si256(v256 a, v256 b)
			{
				return X86.Sse4_1.testc_si128(a.Lo128, b.Lo128) & X86.Sse4_1.testc_si128(a.Hi128, b.Hi128);
			}

			[DebuggerStepThrough]
			public static int mm256_testnzc_si256(v256 a, v256 b)
			{
				int num = X86.Avx.mm256_testz_si256(a, b);
				int num2 = X86.Avx.mm256_testc_si256(a, b);
				return 1 - (num | num2);
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_testz_pd(v256 a, v256 b)
			{
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &b.ULong0;
				for (int i = 0; i < 4; i++)
				{
					if ((ptr[i] & ptr2[i] & 9223372036854775808UL) != 0UL)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_testc_pd(v256 a, v256 b)
			{
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &b.ULong0;
				for (int i = 0; i < 4; i++)
				{
					if ((~ptr[i] & ptr2[i] & 9223372036854775808UL) != 0UL)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int mm256_testnzc_pd(v256 a, v256 b)
			{
				return 1 - (X86.Avx.mm256_testz_pd(a, b) | X86.Avx.mm256_testc_pd(a, b));
			}

			[DebuggerStepThrough]
			public unsafe static int testz_pd(v128 a, v128 b)
			{
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &b.ULong0;
				for (int i = 0; i < 2; i++)
				{
					if ((ptr[i] & ptr2[i] & 9223372036854775808UL) != 0UL)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int testc_pd(v128 a, v128 b)
			{
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &b.ULong0;
				for (int i = 0; i < 2; i++)
				{
					if ((~ptr[i] & ptr2[i] & 9223372036854775808UL) != 0UL)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int testnzc_pd(v128 a, v128 b)
			{
				return 1 - (X86.Avx.testz_pd(a, b) | X86.Avx.testc_pd(a, b));
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_testz_ps(v256 a, v256 b)
			{
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				for (int i = 0; i < 8; i++)
				{
					if ((ptr[i] & ptr2[i] & 2147483648U) != 0U)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_testc_ps(v256 a, v256 b)
			{
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				for (int i = 0; i < 8; i++)
				{
					if ((~ptr[i] & ptr2[i] & 2147483648U) != 0U)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int mm256_testnzc_ps(v256 a, v256 b)
			{
				return 1 - (X86.Avx.mm256_testz_ps(a, b) | X86.Avx.mm256_testc_ps(a, b));
			}

			[DebuggerStepThrough]
			public unsafe static int testz_ps(v128 a, v128 b)
			{
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				for (int i = 0; i < 4; i++)
				{
					if ((ptr[i] & ptr2[i] & 2147483648U) != 0U)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int testc_ps(v128 a, v128 b)
			{
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				for (int i = 0; i < 4; i++)
				{
					if ((~ptr[i] & ptr2[i] & 2147483648U) != 0U)
					{
						return 0;
					}
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int testnzc_ps(v128 a, v128 b)
			{
				return 1 - (X86.Avx.testz_ps(a, b) | X86.Avx.testc_ps(a, b));
			}

			[DebuggerStepThrough]
			public static int mm256_movemask_pd(v256 a)
			{
				return X86.Sse2.movemask_pd(a.Lo128) | (X86.Sse2.movemask_pd(a.Hi128) << 2);
			}

			[DebuggerStepThrough]
			public static int mm256_movemask_ps(v256 a)
			{
				return X86.Sse.movemask_ps(a.Lo128) | (X86.Sse.movemask_ps(a.Hi128) << 4);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setzero_pd()
			{
				return default(v256);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setzero_ps()
			{
				return default(v256);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setzero_si256()
			{
				return default(v256);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_pd(double d, double c, double b, double a)
			{
				return new v256(a, b, c, d);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_ps(float e7, float e6, float e5, float e4, float e3, float e2, float e1, float e0)
			{
				return new v256(e0, e1, e2, e3, e4, e5, e6, e7);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_epi8(byte e31_, byte e30_, byte e29_, byte e28_, byte e27_, byte e26_, byte e25_, byte e24_, byte e23_, byte e22_, byte e21_, byte e20_, byte e19_, byte e18_, byte e17_, byte e16_, byte e15_, byte e14_, byte e13_, byte e12_, byte e11_, byte e10_, byte e9_, byte e8_, byte e7_, byte e6_, byte e5_, byte e4_, byte e3_, byte e2_, byte e1_, byte e0_)
			{
				return new v256(e0_, e1_, e2_, e3_, e4_, e5_, e6_, e7_, e8_, e9_, e10_, e11_, e12_, e13_, e14_, e15_, e16_, e17_, e18_, e19_, e20_, e21_, e22_, e23_, e24_, e25_, e26_, e27_, e28_, e29_, e30_, e31_);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_epi16(short e15_, short e14_, short e13_, short e12_, short e11_, short e10_, short e9_, short e8_, short e7_, short e6_, short e5_, short e4_, short e3_, short e2_, short e1_, short e0_)
			{
				return new v256(e0_, e1_, e2_, e3_, e4_, e5_, e6_, e7_, e8_, e9_, e10_, e11_, e12_, e13_, e14_, e15_);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_epi32(int e7, int e6, int e5, int e4, int e3, int e2, int e1, int e0)
			{
				return new v256(e0, e1, e2, e3, e4, e5, e6, e7);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_epi64x(long e3, long e2, long e1, long e0)
			{
				return new v256(e0, e1, e2, e3);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_m128d(v128 hi, v128 lo)
			{
				return new v256(lo, hi);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set_m128i(v128 hi, v128 lo)
			{
				return new v256(lo, hi);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_pd(double d, double c, double b, double a)
			{
				return new v256(d, c, b, a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_ps(float e7, float e6, float e5, float e4, float e3, float e2, float e1, float e0)
			{
				return new v256(e7, e6, e5, e4, e3, e2, e1, e0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_epi8(byte e31_, byte e30_, byte e29_, byte e28_, byte e27_, byte e26_, byte e25_, byte e24_, byte e23_, byte e22_, byte e21_, byte e20_, byte e19_, byte e18_, byte e17_, byte e16_, byte e15_, byte e14_, byte e13_, byte e12_, byte e11_, byte e10_, byte e9_, byte e8_, byte e7_, byte e6_, byte e5_, byte e4_, byte e3_, byte e2_, byte e1_, byte e0_)
			{
				return new v256(e31_, e30_, e29_, e28_, e27_, e26_, e25_, e24_, e23_, e22_, e21_, e20_, e19_, e18_, e17_, e16_, e15_, e14_, e13_, e12_, e11_, e10_, e9_, e8_, e7_, e6_, e5_, e4_, e3_, e2_, e1_, e0_);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_epi16(short e15_, short e14_, short e13_, short e12_, short e11_, short e10_, short e9_, short e8_, short e7_, short e6_, short e5_, short e4_, short e3_, short e2_, short e1_, short e0_)
			{
				return new v256(e15_, e14_, e13_, e12_, e11_, e10_, e9_, e8_, e7_, e6_, e5_, e4_, e3_, e2_, e1_, e0_);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_epi32(int e7, int e6, int e5, int e4, int e3, int e2, int e1, int e0)
			{
				return new v256(e7, e6, e5, e4, e3, e2, e1, e0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_epi64x(long e3, long e2, long e1, long e0)
			{
				return new v256(e3, e2, e1, e0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_m128(v128 hi, v128 lo)
			{
				return new v256(hi, lo);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_m128d(v128 hi, v128 lo)
			{
				return new v256(hi, lo);
			}

			[DebuggerStepThrough]
			public static v256 mm256_setr_m128i(v128 hi, v128 lo)
			{
				return new v256(hi, lo);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_pd(double a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_ps(float a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_epi8(byte a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_epi16(short a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_epi32(int a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_set1_epi64x(long a)
			{
				return new v256(a);
			}

			[DebuggerStepThrough]
			public static v256 mm256_castpd_ps(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castps_pd(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castps_si256(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castpd_si256(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castsi256_ps(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castsi256_pd(v256 a)
			{
				return a;
			}

			[DebuggerStepThrough]
			public static v128 mm256_castps256_ps128(v256 a)
			{
				return a.Lo128;
			}

			[DebuggerStepThrough]
			public static v128 mm256_castpd256_pd128(v256 a)
			{
				return a.Lo128;
			}

			[DebuggerStepThrough]
			public static v128 mm256_castsi256_si128(v256 a)
			{
				return a.Lo128;
			}

			[DebuggerStepThrough]
			public static v256 mm256_castps128_ps256(v128 a)
			{
				return new v256(a, X86.Sse.setzero_ps());
			}

			[DebuggerStepThrough]
			public static v256 mm256_castpd128_pd256(v128 a)
			{
				return new v256(a, X86.Sse.setzero_ps());
			}

			[DebuggerStepThrough]
			public static v256 mm256_castsi128_si256(v128 a)
			{
				return new v256(a, X86.Sse.setzero_ps());
			}

			[DebuggerStepThrough]
			public static v128 undefined_ps()
			{
				return default(v128);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v128 undefined_pd()
			{
				return X86.Avx.undefined_ps();
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v128 undefined_si128()
			{
				return X86.Avx.undefined_ps();
			}

			[DebuggerStepThrough]
			public static v256 mm256_undefined_ps()
			{
				return default(v256);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_undefined_pd()
			{
				return X86.Avx.mm256_undefined_ps();
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_undefined_si256()
			{
				return X86.Avx.mm256_undefined_ps();
			}

			[DebuggerStepThrough]
			public static v256 mm256_zextps128_ps256(v128 a)
			{
				return new v256(a, X86.Sse.setzero_ps());
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_zextpd128_pd256(v128 a)
			{
				return X86.Avx.mm256_zextps128_ps256(a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.AVX)]
			public static v256 mm256_zextsi128_si256(v128 a)
			{
				return X86.Avx.mm256_zextps128_ps256(a);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_insert_epi8(v256 a, int i, int index)
			{
				v256 v = a;
				(&v.Byte0)[index & 31] = (byte)i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_insert_epi16(v256 a, int i, int index)
			{
				v256 v = a;
				(&v.SShort0)[index & 15] = (short)i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_insert_epi32(v256 a, int i, int index)
			{
				v256 v = a;
				(&v.SInt0)[index & 7] = i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_insert_epi64(v256 a, long i, int index)
			{
				v256 v = a;
				(&v.SLong0)[index & 3] = i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_extract_epi32(v256 a, int index)
			{
				return (&a.SInt0)[index & 7];
			}

			[DebuggerStepThrough]
			public unsafe static long mm256_extract_epi64(v256 a, int index)
			{
				return (&a.SLong0)[index & 3];
			}

			public enum CMP
			{
				EQ_OQ,
				LT_OS,
				LE_OS,
				UNORD_Q,
				NEQ_UQ,
				NLT_US,
				NLE_US,
				ORD_Q,
				EQ_UQ,
				NGE_US,
				NGT_US,
				FALSE_OQ,
				NEQ_OQ,
				GE_OS,
				GT_OS,
				TRUE_UQ,
				EQ_OS,
				LT_OQ,
				LE_OQ,
				UNORD_S,
				NEQ_US,
				NLT_UQ,
				NLE_UQ,
				ORD_S,
				EQ_US,
				NGE_UQ,
				NGT_UQ,
				FALSE_OS,
				NEQ_OS,
				GE_OQ,
				GT_OQ,
				TRUE_US
			}
		}

		public static class Avx2
		{
			public static bool IsAvx2Supported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_movemask_epi8(v256 a)
			{
				uint num = 0U;
				byte* ptr = &a.Byte0;
				uint num2 = 1U;
				int i = 0;
				while (i < 32)
				{
					num |= ((uint)ptr[i] >> 7) * num2;
					i++;
					num2 <<= 1;
				}
				return (int)num;
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_extract_epi8(v256 a, int index)
			{
				return (int)(&a.Byte0)[index & 31];
			}

			[DebuggerStepThrough]
			public unsafe static int mm256_extract_epi16(v256 a, int index)
			{
				return (int)(&a.UShort0)[index & 15];
			}

			[DebuggerStepThrough]
			public static double mm256_cvtsd_f64(v256 a)
			{
				return a.Double0;
			}

			[DebuggerStepThrough]
			public static int mm256_cvtsi256_si32(v256 a)
			{
				return a.SInt0;
			}

			[DebuggerStepThrough]
			public static long mm256_cvtsi256_si64(v256 a)
			{
				return a.SLong0;
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpeq_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpeq_epi8(a.Lo128, b.Lo128), X86.Sse2.cmpeq_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpeq_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpeq_epi16(a.Lo128, b.Lo128), X86.Sse2.cmpeq_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpeq_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpeq_epi32(a.Lo128, b.Lo128), X86.Sse2.cmpeq_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpeq_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.cmpeq_epi64(a.Lo128, b.Lo128), X86.Sse4_1.cmpeq_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpgt_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpgt_epi8(a.Lo128, b.Lo128), X86.Sse2.cmpgt_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpgt_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpgt_epi16(a.Lo128, b.Lo128), X86.Sse2.cmpgt_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpgt_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.cmpgt_epi32(a.Lo128, b.Lo128), X86.Sse2.cmpgt_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cmpgt_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse4_2.cmpgt_epi64(a.Lo128, b.Lo128), X86.Sse4_2.cmpgt_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.max_epi8(a.Lo128, b.Lo128), X86.Sse4_1.max_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.max_epi16(a.Lo128, b.Lo128), X86.Sse2.max_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.max_epi32(a.Lo128, b.Lo128), X86.Sse4_1.max_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.max_epu8(a.Lo128, b.Lo128), X86.Sse2.max_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.max_epu16(a.Lo128, b.Lo128), X86.Sse4_1.max_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_max_epu32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.max_epu32(a.Lo128, b.Lo128), X86.Sse4_1.max_epu32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.min_epi8(a.Lo128, b.Lo128), X86.Sse4_1.min_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.min_epi16(a.Lo128, b.Lo128), X86.Sse2.min_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.min_epi32(a.Lo128, b.Lo128), X86.Sse4_1.min_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.min_epu8(a.Lo128, b.Lo128), X86.Sse2.min_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.min_epu16(a.Lo128, b.Lo128), X86.Sse4_1.min_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_min_epu32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.min_epu32(a.Lo128, b.Lo128), X86.Sse4_1.min_epu32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_and_si256(v256 a, v256 b)
			{
				return new v256(X86.Sse2.and_si128(a.Lo128, b.Lo128), X86.Sse2.and_si128(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_andnot_si256(v256 a, v256 b)
			{
				return new v256(X86.Sse2.andnot_si128(a.Lo128, b.Lo128), X86.Sse2.andnot_si128(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_or_si256(v256 a, v256 b)
			{
				return new v256(X86.Sse2.or_si128(a.Lo128, b.Lo128), X86.Sse2.or_si128(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_xor_si256(v256 a, v256 b)
			{
				return new v256(X86.Sse2.xor_si128(a.Lo128, b.Lo128), X86.Sse2.xor_si128(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_abs_epi8(v256 a)
			{
				return new v256(X86.Ssse3.abs_epi8(a.Lo128), X86.Ssse3.abs_epi8(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_abs_epi16(v256 a)
			{
				return new v256(X86.Ssse3.abs_epi16(a.Lo128), X86.Ssse3.abs_epi16(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_abs_epi32(v256 a)
			{
				return new v256(X86.Ssse3.abs_epi32(a.Lo128), X86.Ssse3.abs_epi32(a.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.add_epi8(a.Lo128, b.Lo128), X86.Sse2.add_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.add_epi16(a.Lo128, b.Lo128), X86.Sse2.add_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.add_epi32(a.Lo128, b.Lo128), X86.Sse2.add_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_add_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse2.add_epi64(a.Lo128, b.Lo128), X86.Sse2.add_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_adds_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.adds_epi8(a.Lo128, b.Lo128), X86.Sse2.adds_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_adds_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.adds_epi16(a.Lo128, b.Lo128), X86.Sse2.adds_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_adds_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.adds_epu8(a.Lo128, b.Lo128), X86.Sse2.adds_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_adds_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.adds_epu16(a.Lo128, b.Lo128), X86.Sse2.adds_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sub_epi8(a.Lo128, b.Lo128), X86.Sse2.sub_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sub_epi16(a.Lo128, b.Lo128), X86.Sse2.sub_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sub_epi32(a.Lo128, b.Lo128), X86.Sse2.sub_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sub_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sub_epi64(a.Lo128, b.Lo128), X86.Sse2.sub_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_subs_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.subs_epi8(a.Lo128, b.Lo128), X86.Sse2.subs_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_subs_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.subs_epi16(a.Lo128, b.Lo128), X86.Sse2.subs_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_subs_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.subs_epu8(a.Lo128, b.Lo128), X86.Sse2.subs_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_subs_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.subs_epu16(a.Lo128, b.Lo128), X86.Sse2.subs_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_avg_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.avg_epu8(a.Lo128, b.Lo128), X86.Sse2.avg_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_avg_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.avg_epu16(a.Lo128, b.Lo128), X86.Sse2.avg_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hadd_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hadd_epi16(a.Lo128, b.Lo128), X86.Ssse3.hadd_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hadd_epi32(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hadd_epi32(a.Lo128, b.Lo128), X86.Ssse3.hadd_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hadds_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hadds_epi16(a.Lo128, b.Lo128), X86.Ssse3.hadds_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hsub_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hsub_epi16(a.Lo128, b.Lo128), X86.Ssse3.hsub_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hsub_epi32(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hsub_epi32(a.Lo128, b.Lo128), X86.Ssse3.hsub_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_hsubs_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.hsubs_epi16(a.Lo128, b.Lo128), X86.Ssse3.hsubs_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_madd_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.madd_epi16(a.Lo128, b.Lo128), X86.Sse2.madd_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_maddubs_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.maddubs_epi16(a.Lo128, b.Lo128), X86.Ssse3.maddubs_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mulhi_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.mulhi_epi16(a.Lo128, b.Lo128), X86.Sse2.mulhi_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mulhi_epu16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.mulhi_epu16(a.Lo128, b.Lo128), X86.Sse2.mulhi_epu16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mullo_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.mullo_epi16(a.Lo128, b.Lo128), X86.Sse2.mullo_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mullo_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.mullo_epi32(a.Lo128, b.Lo128), X86.Sse4_1.mullo_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mul_epu32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.mul_epu32(a.Lo128, b.Lo128), X86.Sse2.mul_epu32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mul_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.mul_epi32(a.Lo128, b.Lo128), X86.Sse4_1.mul_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sign_epi8(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.sign_epi8(a.Lo128, b.Lo128), X86.Ssse3.sign_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sign_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.sign_epi16(a.Lo128, b.Lo128), X86.Ssse3.sign_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sign_epi32(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.sign_epi32(a.Lo128, b.Lo128), X86.Ssse3.sign_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mulhrs_epi16(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.mulhrs_epi16(a.Lo128, b.Lo128), X86.Ssse3.mulhrs_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sad_epu8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.sad_epu8(a.Lo128, b.Lo128), X86.Sse2.sad_epu8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_mpsadbw_epu8(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse4_1.mpsadbw_epu8(a.Lo128, b.Lo128, imm8 & 7), X86.Sse4_1.mpsadbw_epu8(a.Hi128, b.Hi128, (imm8 >> 3) & 7));
			}

			[DebuggerStepThrough]
			public static v256 mm256_slli_si256(v256 a, int imm8)
			{
				return new v256(X86.Sse2.slli_si128(a.Lo128, imm8), X86.Sse2.slli_si128(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_bslli_epi128(v256 a, int imm8)
			{
				return X86.Avx2.mm256_slli_si256(a, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_srli_si256(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srli_si128(a.Lo128, imm8), X86.Sse2.srli_si128(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_bsrli_epi128(v256 a, int imm8)
			{
				return X86.Avx2.mm256_srli_si256(a, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_sll_epi16(v256 a, v128 count)
			{
				return new v256(X86.Sse2.sll_epi16(a.Lo128, count), X86.Sse2.sll_epi16(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sll_epi32(v256 a, v128 count)
			{
				return new v256(X86.Sse2.sll_epi32(a.Lo128, count), X86.Sse2.sll_epi32(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sll_epi64(v256 a, v128 count)
			{
				return new v256(X86.Sse2.sll_epi64(a.Lo128, count), X86.Sse2.sll_epi64(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_slli_epi16(v256 a, int imm8)
			{
				return new v256(X86.Sse2.slli_epi16(a.Lo128, imm8), X86.Sse2.slli_epi16(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_slli_epi32(v256 a, int imm8)
			{
				return new v256(X86.Sse2.slli_epi32(a.Lo128, imm8), X86.Sse2.slli_epi32(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_slli_epi64(v256 a, int imm8)
			{
				return new v256(X86.Sse2.slli_epi64(a.Lo128, imm8), X86.Sse2.slli_epi64(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sllv_epi32(v256 a, v256 count)
			{
				return new v256(X86.Avx2.sllv_epi32(a.Lo128, count.Lo128), X86.Avx2.sllv_epi32(a.Hi128, count.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sllv_epi64(v256 a, v256 count)
			{
				return new v256(X86.Avx2.sllv_epi64(a.Lo128, count.Lo128), X86.Avx2.sllv_epi64(a.Hi128, count.Hi128));
			}

			[DebuggerStepThrough]
			public unsafe static v128 sllv_epi32(v128 a, v128 count)
			{
				v128 v = default(v128);
				uint* ptr = &a.UInt0;
				uint* ptr2 = &v.UInt0;
				int* ptr3 = &count.SInt0;
				for (int i = 0; i < 4; i++)
				{
					int num = ptr3[i];
					if (num >= 0 && num <= 31)
					{
						ptr2[i] = ptr[i] << num;
					}
					else
					{
						ptr2[i] = 0U;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sllv_epi64(v128 a, v128 count)
			{
				v128 v = default(v128);
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &v.ULong0;
				long* ptr3 = &count.SLong0;
				for (int i = 0; i < 2; i++)
				{
					int num = (int)ptr3[i];
					if (num >= 0 && num <= 63)
					{
						ptr2[i] = ptr[i] << num;
					}
					else
					{
						ptr2[i] = 0UL;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_sra_epi16(v256 a, v128 count)
			{
				return new v256(X86.Sse2.sra_epi16(a.Lo128, count), X86.Sse2.sra_epi16(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_sra_epi32(v256 a, v128 count)
			{
				return new v256(X86.Sse2.sra_epi32(a.Lo128, count), X86.Sse2.sra_epi32(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srai_epi16(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srai_epi16(a.Lo128, imm8), X86.Sse2.srai_epi16(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srai_epi32(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srai_epi32(a.Lo128, imm8), X86.Sse2.srai_epi32(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srav_epi32(v256 a, v256 count)
			{
				return new v256(X86.Avx2.srav_epi32(a.Lo128, count.Lo128), X86.Avx2.srav_epi32(a.Hi128, count.Hi128));
			}

			[DebuggerStepThrough]
			public unsafe static v128 srav_epi32(v128 a, v128 count)
			{
				v128 v = default(v128);
				int* ptr = &a.SInt0;
				int* ptr2 = &v.SInt0;
				int* ptr3 = &count.SInt0;
				for (int i = 0; i < 4; i++)
				{
					int num = Math.Min(ptr3[i] & 255, 32);
					int num2 = 0;
					if (num >= 16)
					{
						num -= 16;
						num2 += 16;
					}
					ptr2[i] = ptr[i] >> num >> num2;
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_srl_epi16(v256 a, v128 count)
			{
				return new v256(X86.Sse2.srl_epi16(a.Lo128, count), X86.Sse2.srl_epi16(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srl_epi32(v256 a, v128 count)
			{
				return new v256(X86.Sse2.srl_epi32(a.Lo128, count), X86.Sse2.srl_epi32(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srl_epi64(v256 a, v128 count)
			{
				return new v256(X86.Sse2.srl_epi64(a.Lo128, count), X86.Sse2.srl_epi64(a.Hi128, count));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srli_epi16(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srli_epi16(a.Lo128, imm8), X86.Sse2.srli_epi16(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srli_epi32(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srli_epi32(a.Lo128, imm8), X86.Sse2.srli_epi32(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srli_epi64(v256 a, int imm8)
			{
				return new v256(X86.Sse2.srli_epi64(a.Lo128, imm8), X86.Sse2.srli_epi64(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srlv_epi32(v256 a, v256 count)
			{
				return new v256(X86.Avx2.srlv_epi32(a.Lo128, count.Lo128), X86.Avx2.srlv_epi32(a.Hi128, count.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_srlv_epi64(v256 a, v256 count)
			{
				return new v256(X86.Avx2.srlv_epi64(a.Lo128, count.Lo128), X86.Avx2.srlv_epi64(a.Hi128, count.Hi128));
			}

			[DebuggerStepThrough]
			public unsafe static v128 srlv_epi32(v128 a, v128 count)
			{
				v128 v = default(v128);
				uint* ptr = &a.UInt0;
				uint* ptr2 = &v.UInt0;
				int* ptr3 = &count.SInt0;
				for (int i = 0; i < 4; i++)
				{
					int num = ptr3[i];
					if (num >= 0 && num <= 31)
					{
						ptr2[i] = ptr[i] >> num;
					}
					else
					{
						ptr2[i] = 0U;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srlv_epi64(v128 a, v128 count)
			{
				v128 v = default(v128);
				ulong* ptr = &a.ULong0;
				ulong* ptr2 = &v.ULong0;
				long* ptr3 = &count.SLong0;
				for (int i = 0; i < 2; i++)
				{
					int num = (int)ptr3[i];
					if (num >= 0 && num <= 63)
					{
						ptr2[i] = ptr[i] >> num;
					}
					else
					{
						ptr2[i] = 0UL;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 blend_epi32(v128 a, v128 b, int imm8)
			{
				return X86.Sse4_1.blend_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_blend_epi32(v256 a, v256 b, int imm8)
			{
				return X86.Avx.mm256_blend_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_alignr_epi8(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Ssse3.alignr_epi8(a.Lo128, b.Lo128, imm8), X86.Ssse3.alignr_epi8(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blendv_epi8(v256 a, v256 b, v256 mask)
			{
				return new v256(X86.Sse4_1.blendv_epi8(a.Lo128, b.Lo128, mask.Lo128), X86.Sse4_1.blendv_epi8(a.Hi128, b.Hi128, mask.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_blend_epi16(v256 a, v256 b, int imm8)
			{
				return new v256(X86.Sse4_1.blend_epi16(a.Lo128, b.Lo128, imm8), X86.Sse4_1.blend_epi16(a.Hi128, b.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_packs_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.packs_epi16(a.Lo128, b.Lo128), X86.Sse2.packs_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_packs_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.packs_epi32(a.Lo128, b.Lo128), X86.Sse2.packs_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_packus_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.packus_epi16(a.Lo128, b.Lo128), X86.Sse2.packus_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_packus_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse4_1.packus_epi32(a.Lo128, b.Lo128), X86.Sse4_1.packus_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpackhi_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpackhi_epi8(a.Lo128, b.Lo128), X86.Sse2.unpackhi_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpackhi_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpackhi_epi16(a.Lo128, b.Lo128), X86.Sse2.unpackhi_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpackhi_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpackhi_epi32(a.Lo128, b.Lo128), X86.Sse2.unpackhi_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpackhi_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpackhi_epi64(a.Lo128, b.Lo128), X86.Sse2.unpackhi_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpacklo_epi8(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpacklo_epi8(a.Lo128, b.Lo128), X86.Sse2.unpacklo_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpacklo_epi16(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpacklo_epi16(a.Lo128, b.Lo128), X86.Sse2.unpacklo_epi16(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpacklo_epi32(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpacklo_epi32(a.Lo128, b.Lo128), X86.Sse2.unpacklo_epi32(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_unpacklo_epi64(v256 a, v256 b)
			{
				return new v256(X86.Sse2.unpacklo_epi64(a.Lo128, b.Lo128), X86.Sse2.unpacklo_epi64(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shuffle_epi8(v256 a, v256 b)
			{
				return new v256(X86.Ssse3.shuffle_epi8(a.Lo128, b.Lo128), X86.Ssse3.shuffle_epi8(a.Hi128, b.Hi128));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shuffle_epi32(v256 a, int imm8)
			{
				return new v256(X86.Sse2.shuffle_epi32(a.Lo128, imm8), X86.Sse2.shuffle_epi32(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shufflehi_epi16(v256 a, int imm8)
			{
				return new v256(X86.Sse2.shufflehi_epi16(a.Lo128, imm8), X86.Sse2.shufflehi_epi16(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v256 mm256_shufflelo_epi16(v256 a, int imm8)
			{
				return new v256(X86.Sse2.shufflelo_epi16(a.Lo128, imm8), X86.Sse2.shufflelo_epi16(a.Hi128, imm8));
			}

			[DebuggerStepThrough]
			public static v128 mm256_extracti128_si256(v256 a, int imm8)
			{
				return X86.Avx.mm256_extractf128_si256(a, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_inserti128_si256(v256 a, v128 b, int imm8)
			{
				return X86.Avx.mm256_insertf128_ps(a, b, imm8);
			}

			[DebuggerStepThrough]
			public static v128 broadcastss_ps(v128 a)
			{
				return new v128(a.Float0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastss_ps(v128 a)
			{
				return new v256(a.Float0);
			}

			[DebuggerStepThrough]
			public static v128 broadcastsd_pd(v128 a)
			{
				return new v128(a.Double0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastsd_pd(v128 a)
			{
				return new v256(a.Double0);
			}

			[DebuggerStepThrough]
			public static v128 broadcastb_epi8(v128 a)
			{
				return new v128(a.Byte0);
			}

			[DebuggerStepThrough]
			public static v128 broadcastw_epi16(v128 a)
			{
				return new v128(a.SShort0);
			}

			[DebuggerStepThrough]
			public static v128 broadcastd_epi32(v128 a)
			{
				return new v128(a.SInt0);
			}

			[DebuggerStepThrough]
			public static v128 broadcastq_epi64(v128 a)
			{
				return new v128(a.SLong0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastb_epi8(v128 a)
			{
				return new v256(a.Byte0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastw_epi16(v128 a)
			{
				return new v256(a.SShort0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastd_epi32(v128 a)
			{
				return new v256(a.SInt0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastq_epi64(v128 a)
			{
				return new v256(a.SLong0);
			}

			[DebuggerStepThrough]
			public static v256 mm256_broadcastsi128_si256(v128 a)
			{
				return new v256(a, a);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi8_epi16(v128 a)
			{
				v256 v = default(v256);
				short* ptr = &v.SShort0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = (short)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi8_epi32(v128 a)
			{
				v256 v = default(v256);
				int* ptr = &v.SInt0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi8_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi16_epi32(v128 a)
			{
				v256 v = default(v256);
				int* ptr = &v.SInt0;
				short* ptr2 = &a.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi16_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				short* ptr2 = &a.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepi32_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				int* ptr2 = &a.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu8_epi16(v128 a)
			{
				v256 v = default(v256);
				short* ptr = &v.SShort0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = (short)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu8_epi32(v128 a)
			{
				v256 v = default(v256);
				int* ptr = &v.SInt0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu8_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu16_epi32(v128 a)
			{
				v256 v = default(v256);
				int* ptr = &v.SInt0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu16_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_cvtepu32_epi64(v128 a)
			{
				v256 v = default(v256);
				long* ptr = &v.SLong0;
				uint* ptr2 = &a.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 maskload_epi32(void* mem_addr, v128 mask)
			{
				v128 v = default(v128);
				int* ptr = &mask.SInt0;
				int* ptr2 = &v.SInt0;
				for (int i = 0; i < 4; i++)
				{
					if (ptr[i] < 0)
					{
						ptr2[i] = *(int*)((byte*)mem_addr + (IntPtr)i * 4);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 maskload_epi64(void* mem_addr, v128 mask)
			{
				v128 v = default(v128);
				long* ptr = &mask.SLong0;
				long* ptr2 = &v.SLong0;
				for (int i = 0; i < 2; i++)
				{
					if (ptr[i] < 0L)
					{
						ptr2[i] = *(long*)((byte*)mem_addr + (IntPtr)i * 8);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static void maskstore_epi32(void* mem_addr, v128 mask, v128 a)
			{
				int* ptr = &mask.SInt0;
				int* ptr2 = &a.SInt0;
				for (int i = 0; i < 4; i++)
				{
					if (ptr[i] < 0)
					{
						*(int*)((byte*)mem_addr + (IntPtr)i * 4) = ptr2[i];
					}
				}
			}

			[DebuggerStepThrough]
			public unsafe static void maskstore_epi64(void* mem_addr, v128 mask, v128 a)
			{
				long* ptr = &mask.SLong0;
				long* ptr2 = &a.SLong0;
				for (int i = 0; i < 2; i++)
				{
					if (ptr[i] < 0L)
					{
						*(long*)((byte*)mem_addr + (IntPtr)i * 8) = ptr2[i];
					}
				}
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_maskload_epi32(void* mem_addr, v256 mask)
			{
				v256 v = default(v256);
				int* ptr = &mask.SInt0;
				int* ptr2 = &v.SInt0;
				for (int i = 0; i < 8; i++)
				{
					if (ptr[i] < 0)
					{
						ptr2[i] = *(int*)((byte*)mem_addr + (IntPtr)i * 4);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_maskload_epi64(void* mem_addr, v256 mask)
			{
				v256 v = default(v256);
				long* ptr = &mask.SLong0;
				long* ptr2 = &v.SLong0;
				for (int i = 0; i < 4; i++)
				{
					if (ptr[i] < 0L)
					{
						ptr2[i] = *(long*)((byte*)mem_addr + (IntPtr)i * 8);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_maskstore_epi32(void* mem_addr, v256 mask, v256 a)
			{
				int* ptr = &mask.SInt0;
				int* ptr2 = &a.SInt0;
				for (int i = 0; i < 8; i++)
				{
					if (ptr[i] < 0)
					{
						*(int*)((byte*)mem_addr + (IntPtr)i * 4) = ptr2[i];
					}
				}
			}

			[DebuggerStepThrough]
			public unsafe static void mm256_maskstore_epi64(void* mem_addr, v256 mask, v256 a)
			{
				long* ptr = &mask.SLong0;
				long* ptr2 = &a.SLong0;
				for (int i = 0; i < 4; i++)
				{
					if (ptr[i] < 0L)
					{
						*(long*)((byte*)mem_addr + (IntPtr)i * 8) = ptr2[i];
					}
				}
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_permutevar8x32_epi32(v256 a, v256 idx)
			{
				v256 v = default(v256);
				int* ptr = &idx.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &v.SInt0;
				for (int i = 0; i < 8; i++)
				{
					int num = ptr[i] & 7;
					ptr3[i] = ptr2[num];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_permutevar8x32_ps(v256 a, v256 idx)
			{
				return X86.Avx2.mm256_permutevar8x32_epi32(a, idx);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_permute4x64_epi64(v256 a, int imm8)
			{
				v256 v = default(v256);
				long* ptr = &a.SLong0;
				long* ptr2 = &v.SLong0;
				int i = 0;
				while (i < 4)
				{
					ptr2[i] = ptr[imm8 & 3];
					i++;
					imm8 >>= 2;
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute4x64_pd(v256 a, int imm8)
			{
				return X86.Avx2.mm256_permute4x64_epi64(a, imm8);
			}

			[DebuggerStepThrough]
			public static v256 mm256_permute2x128_si256(v256 a, v256 b, int imm8)
			{
				return X86.Avx.mm256_permute2f128_si256(a, b, imm8);
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_stream_load_si256(void* mem_addr)
			{
				return *(v256*)mem_addr;
			}

			private unsafe static void EmulatedGather<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(T* dptr, void* base_addr, long* indexPtr, int scale, int n, U* mask) where T : struct, ValueType where U : struct, ValueType, IComparable<U>
			{
				U u = default(U);
				for (int i = 0; i < n; i++)
				{
					long num = indexPtr[i] * (long)scale;
					T* ptr = (T*)((byte*)base_addr + num);
					if (mask == null || mask[(IntPtr)i * (IntPtr)sizeof(U) / (IntPtr)sizeof(U)].CompareTo(u) < 0)
					{
						dptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = *ptr;
					}
				}
			}

			private unsafe static void EmulatedGather<[global::System.Runtime.CompilerServices.IsUnmanaged] T, [global::System.Runtime.CompilerServices.IsUnmanaged] U>(T* dptr, void* base_addr, int* indexPtr, int scale, int n, U* mask) where T : struct, ValueType where U : struct, ValueType, IComparable<U>
			{
				U u = default(U);
				for (int i = 0; i < n; i++)
				{
					long num = (long)indexPtr[i] * (long)scale;
					T* ptr = (T*)((byte*)base_addr + num);
					if (mask == null || mask[(IntPtr)i * (IntPtr)sizeof(U) / (IntPtr)sizeof(U)].CompareTo(u) < 0)
					{
						dptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = *ptr;
					}
				}
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i32gather_epi32(void* base_addr, v256 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SInt0, scale, sizeof(v256) / 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i32gather_pd(void* base_addr, v128 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SInt0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i32gather_ps(void* base_addr, v256 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SInt0, scale, 8, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i64gather_pd(void* base_addr, v256 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SLong0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mm256_i64gather_ps(void* base_addr, v256 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SLong0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i32gather_pd(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SInt0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i32gather_ps(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SInt0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i64gather_pd(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SLong0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i64gather_ps(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SLong0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i32gather_epi64(void* base_addr, v128 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SInt0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mm256_i64gather_epi32(void* base_addr, v256 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SLong0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_i64gather_epi64(void* base_addr, v256 vindex, int scale)
			{
				v256 v = default(v256);
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SLong0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i32gather_epi32(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SInt0, scale, 4, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i32gather_epi64(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SInt0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i64gather_epi32(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SLong0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 i64gather_epi64(void* base_addr, v128 vindex, int scale)
			{
				v128 v = default(v128);
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SLong0, scale, 2, null);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i32gather_pd(v256 src, void* base_addr, v128 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SInt0, scale, 4, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i32gather_ps(v256 src, void* base_addr, v256 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SInt0, scale, 8, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i64gather_pd(v256 src, void* base_addr, v256 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SLong0, scale, 4, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mm256_mask_i64gather_ps(v128 src, void* base_addr, v256 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SLong0, scale, 4, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i32gather_epi32(v256 src, void* base_addr, v256 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SInt0, scale, 8, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i32gather_epi64(v256 src, void* base_addr, v128 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SInt0, scale, 4, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v256 mm256_mask_i64gather_epi64(v256 src, void* base_addr, v256 vindex, v256 mask, int scale)
			{
				v256 v = src;
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SLong0, scale, 4, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mm256_mask_i64gather_epi32(v128 src, void* base_addr, v256 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SLong0, scale, 4, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i32gather_pd(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SInt0, scale, 2, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i32gather_ps(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SInt0, scale, 4, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i64gather_pd(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<double, long>(&v.Double0, base_addr, &vindex.SLong0, scale, 2, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i64gather_ps(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				v.UInt2 = (v.UInt3 = 0U);
				X86.Avx2.EmulatedGather<float, int>(&v.Float0, base_addr, &vindex.SLong0, scale, 2, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i32gather_epi32(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SInt0, scale, 4, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i32gather_epi64(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SInt0, scale, 2, &mask.SLong0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i64gather_epi32(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				v.UInt2 = (v.UInt3 = 0U);
				X86.Avx2.EmulatedGather<int, int>(&v.SInt0, base_addr, &vindex.SLong0, scale, 2, &mask.SInt0);
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mask_i64gather_epi64(v128 src, void* base_addr, v128 vindex, v128 mask, int scale)
			{
				v128 v = src;
				X86.Avx2.EmulatedGather<long, long>(&v.SLong0, base_addr, &vindex.SLong0, scale, 2, &mask.SLong0);
				return v;
			}
		}

		public static class Bmi1
		{
			public static bool IsBmi1Supported
			{
				get
				{
					return X86.Avx2.IsAvx2Supported;
				}
			}

			[DebuggerStepThrough]
			public static uint andn_u32(uint a, uint b)
			{
				return ~a & b;
			}

			[DebuggerStepThrough]
			public static ulong andn_u64(ulong a, ulong b)
			{
				return ~a & b;
			}

			[DebuggerStepThrough]
			public static uint bextr_u32(uint a, uint start, uint len)
			{
				start &= 255U;
				if (start >= 32U)
				{
					return 0U;
				}
				uint num = a >> (int)start;
				len &= 255U;
				if (len >= 32U)
				{
					return num;
				}
				return num & ((1U << (int)len) - 1U);
			}

			[DebuggerStepThrough]
			public static ulong bextr_u64(ulong a, uint start, uint len)
			{
				start &= 255U;
				if (start >= 64U)
				{
					return 0UL;
				}
				ulong num = a >> (int)start;
				len &= 255U;
				if (len >= 64U)
				{
					return num;
				}
				return num & ((1UL << (int)len) - 1UL);
			}

			[DebuggerStepThrough]
			public static uint bextr2_u32(uint a, uint control)
			{
				uint num = control & 255U;
				uint num2 = (control >> 8) & 255U;
				return X86.Bmi1.bextr_u32(a, num, num2);
			}

			[DebuggerStepThrough]
			public static ulong bextr2_u64(ulong a, ulong control)
			{
				uint num = (uint)(control & 255UL);
				uint num2 = (uint)((control >> 8) & 255UL);
				return X86.Bmi1.bextr_u64(a, num, num2);
			}

			[DebuggerStepThrough]
			public static uint blsi_u32(uint a)
			{
				return -a & a;
			}

			[DebuggerStepThrough]
			public static ulong blsi_u64(ulong a)
			{
				return -a & a;
			}

			[DebuggerStepThrough]
			public static uint blsmsk_u32(uint a)
			{
				return (a - 1U) ^ a;
			}

			[DebuggerStepThrough]
			public static ulong blsmsk_u64(ulong a)
			{
				return (a - 1UL) ^ a;
			}

			[DebuggerStepThrough]
			public static uint blsr_u32(uint a)
			{
				return (a - 1U) & a;
			}

			[DebuggerStepThrough]
			public static ulong blsr_u64(ulong a)
			{
				return (a - 1UL) & a;
			}

			[DebuggerStepThrough]
			public static uint tzcnt_u32(uint a)
			{
				uint num = 32U;
				a &= -a;
				if (a != 0U)
				{
					num -= 1U;
				}
				if ((a & 65535U) != 0U)
				{
					num -= 16U;
				}
				if ((a & 16711935U) != 0U)
				{
					num -= 8U;
				}
				if ((a & 252645135U) != 0U)
				{
					num -= 4U;
				}
				if ((a & 858993459U) != 0U)
				{
					num -= 2U;
				}
				if ((a & 1431655765U) != 0U)
				{
					num -= 1U;
				}
				return num;
			}

			[DebuggerStepThrough]
			public static ulong tzcnt_u64(ulong a)
			{
				ulong num = 64UL;
				a &= -a;
				if (a != 0UL)
				{
					num -= 1UL;
				}
				if ((a & (ulong)(-1)) != 0UL)
				{
					num -= 32UL;
				}
				if ((a & 281470681808895UL) != 0UL)
				{
					num -= 16UL;
				}
				if ((a & 71777214294589695UL) != 0UL)
				{
					num -= 8UL;
				}
				if ((a & 1085102592571150095UL) != 0UL)
				{
					num -= 4UL;
				}
				if ((a & 3689348814741910323UL) != 0UL)
				{
					num -= 2UL;
				}
				if ((a & 6148914691236517205UL) != 0UL)
				{
					num -= 1UL;
				}
				return num;
			}
		}

		public static class Bmi2
		{
			public static bool IsBmi2Supported
			{
				get
				{
					return X86.Avx2.IsAvx2Supported;
				}
			}

			[DebuggerStepThrough]
			public static uint bzhi_u32(uint a, uint index)
			{
				index &= 255U;
				if (index >= 32U)
				{
					return a;
				}
				return a & ((1U << (int)index) - 1U);
			}

			[DebuggerStepThrough]
			public static ulong bzhi_u64(ulong a, ulong index)
			{
				index &= 255UL;
				if (index >= 64UL)
				{
					return a;
				}
				return a & ((1UL << (int)index) - 1UL);
			}

			[DebuggerStepThrough]
			public static uint mulx_u32(uint a, uint b, out uint hi)
			{
				ulong num = (ulong)a;
				ulong num2 = (ulong)b;
				ulong num3 = num * num2;
				hi = (uint)(num3 >> 32);
				return (uint)(num3 & (ulong)(-1));
			}

			[DebuggerStepThrough]
			public static ulong mulx_u64(ulong a, ulong b, out ulong hi)
			{
				return Common.umul128(a, b, out hi);
			}

			[DebuggerStepThrough]
			public static uint pdep_u32(uint a, uint mask)
			{
				uint num = 0U;
				int num2 = 0;
				for (int i = 0; i < 32; i++)
				{
					if ((mask & (1U << i)) != 0U)
					{
						num |= ((a >> num2) & 1U) << i;
						num2++;
					}
				}
				return num;
			}

			[DebuggerStepThrough]
			public static ulong pdep_u64(ulong a, ulong mask)
			{
				ulong num = 0UL;
				int num2 = 0;
				for (int i = 0; i < 64; i++)
				{
					if ((mask & (1UL << i)) != 0UL)
					{
						num |= ((a >> num2) & 1UL) << i;
						num2++;
					}
				}
				return num;
			}

			[DebuggerStepThrough]
			public static uint pext_u32(uint a, uint mask)
			{
				uint num = 0U;
				int num2 = 0;
				for (int i = 0; i < 32; i++)
				{
					if ((mask & (1U << i)) != 0U)
					{
						num |= ((a >> i) & 1U) << num2;
						num2++;
					}
				}
				return num;
			}

			[DebuggerStepThrough]
			public static ulong pext_u64(ulong a, ulong mask)
			{
				ulong num = 0UL;
				int num2 = 0;
				for (int i = 0; i < 64; i++)
				{
					if ((mask & (1UL << i)) != 0UL)
					{
						num |= ((a >> i) & 1UL) << num2;
						num2++;
					}
				}
				return num;
			}
		}

		[Flags]
		public enum MXCSRBits
		{
			FlushToZero = 32768,
			RoundingControlMask = 24576,
			RoundToNearest = 0,
			RoundDown = 8192,
			RoundUp = 16384,
			RoundTowardZero = 24576,
			PrecisionMask = 4096,
			UnderflowMask = 2048,
			OverflowMask = 1024,
			DivideByZeroMask = 512,
			DenormalOperationMask = 256,
			InvalidOperationMask = 128,
			ExceptionMask = 8064,
			DenormalsAreZeroes = 64,
			PrecisionFlag = 32,
			UnderflowFlag = 16,
			OverflowFlag = 8,
			DivideByZeroFlag = 4,
			DenormalFlag = 2,
			InvalidOperationFlag = 1,
			FlagMask = 63
		}

		[Flags]
		public enum RoundingMode
		{
			FROUND_TO_NEAREST_INT = 0,
			FROUND_TO_NEG_INF = 1,
			FROUND_TO_POS_INF = 2,
			FROUND_TO_ZERO = 3,
			FROUND_CUR_DIRECTION = 4,
			FROUND_RAISE_EXC = 0,
			FROUND_NO_EXC = 8,
			FROUND_NINT = 0,
			FROUND_FLOOR = 1,
			FROUND_CEIL = 2,
			FROUND_TRUNC = 3,
			FROUND_RINT = 4,
			FROUND_NEARBYINT = 12,
			FROUND_NINT_NOEXC = 8,
			FROUND_FLOOR_NOEXC = 9,
			FROUND_CEIL_NOEXC = 10,
			FROUND_TRUNC_NOEXC = 11,
			FROUND_RINT_NOEXC = 12
		}

		internal struct RoundingScope : IDisposable
		{
			public RoundingScope(X86.MXCSRBits roundingMode)
			{
				this.OldBits = X86.MXCSR;
				X86.MXCSR = (this.OldBits & ~X86.MXCSRBits.RoundingControlMask) | roundingMode;
			}

			public void Dispose()
			{
				X86.MXCSR = this.OldBits;
			}

			private X86.MXCSRBits OldBits;
		}

		public static class F16C
		{
			public static bool IsF16CSupported
			{
				get
				{
					return X86.Avx2.IsAvx2Supported;
				}
			}

			[DebuggerStepThrough]
			private static uint HalfToFloat(ushort h)
			{
				bool flag = (h & 32768) > 0;
				long num = (long)(h >> 10) & 31L;
				uint num2 = (uint)(h & 1023);
				uint num3 = (flag ? 2147483648U : 0U);
				if (num != 0L || num2 != 0U)
				{
					if (num == 0L)
					{
						num = -1L;
						do
						{
							num += 1L;
							num2 <<= 1;
						}
						while ((num2 & 1024U) == 0U);
						num3 |= (uint)((uint)(112L - num) << 23);
						num3 |= (num2 & 1023U) << 13;
					}
					else
					{
						bool flag2 = num == 31L;
						num3 |= (uint)(flag2 ? 255L : ((uint)(112L + num) << 23));
						num3 |= num2 << 13;
					}
				}
				return num3;
			}

			[DebuggerStepThrough]
			public static v128 cvtph_ps(v128 a)
			{
				return new v128(X86.F16C.HalfToFloat(a.UShort0), X86.F16C.HalfToFloat(a.UShort1), X86.F16C.HalfToFloat(a.UShort2), X86.F16C.HalfToFloat(a.UShort3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_cvtph_ps(v128 a)
			{
				return new v256(X86.F16C.HalfToFloat(a.UShort0), X86.F16C.HalfToFloat(a.UShort1), X86.F16C.HalfToFloat(a.UShort2), X86.F16C.HalfToFloat(a.UShort3), X86.F16C.HalfToFloat(a.UShort4), X86.F16C.HalfToFloat(a.UShort5), X86.F16C.HalfToFloat(a.UShort6), X86.F16C.HalfToFloat(a.UShort7));
			}

			[DebuggerStepThrough]
			private static ushort FloatToHalf(uint f, int rounding)
			{
				uint num = f >> 23;
				sbyte b = X86.F16C.ShiftTable[(int)num];
				uint num2 = (uint)(X86.F16C.BaseTable[(int)num] + (ushort)((f & 8388607U) >> (int)b));
				bool flag = (num2 & 31744U) != 31744U;
				bool flag2 = (num2 & 32768U) > 0U;
				if (rounding == 8)
				{
					uint num3 = (f & 8388607U) >> (int)(b - 1);
					if ((num & 255U) == 102U)
					{
						num2 += 1U;
					}
					if (flag && (num3 & 1U) != 0U)
					{
						num2 += 1U;
					}
				}
				else if (rounding == 11)
				{
					if (!flag)
					{
						num2 -= (uint)(~b & 1);
					}
				}
				else if (rounding == 10)
				{
					if (flag && !flag2)
					{
						if (num <= 102U && num != 0U)
						{
							num2 += 1U;
						}
						else if ((f & 8388607U & ((1U << (int)b) - 1U)) != 0U)
						{
							num2 += 1U;
						}
					}
					bool flag3 = num2 == 64512U;
					bool flag4 = num != 511U;
					if (flag3 && flag4)
					{
						num2 -= 1U;
					}
				}
				else if (rounding == 9)
				{
					if (flag && flag2)
					{
						if (num <= 358U && num != 256U)
						{
							num2 += 1U;
						}
						else if ((f & 8388607U & ((1U << (int)b) - 1U)) != 0U)
						{
							num2 += 1U;
						}
					}
					bool flag5 = num2 == 31744U;
					bool flag6 = num != 255U;
					if (flag5 && flag6)
					{
						num2 -= 1U;
					}
				}
				return (ushort)num2;
			}

			[DebuggerStepThrough]
			public static v128 cvtps_ph(v128 a, int rounding)
			{
				if (rounding == 12)
				{
					X86.MXCSRBits mxcsrbits = X86.MXCSR & X86.MXCSRBits.RoundingControlMask;
					if (mxcsrbits <= X86.MXCSRBits.RoundDown)
					{
						if (mxcsrbits != X86.MXCSRBits.RoundToNearest)
						{
							if (mxcsrbits == X86.MXCSRBits.RoundDown)
							{
								rounding = 9;
							}
						}
						else
						{
							rounding = 8;
						}
					}
					else if (mxcsrbits != X86.MXCSRBits.RoundUp)
					{
						if (mxcsrbits == X86.MXCSRBits.RoundingControlMask)
						{
							rounding = 11;
						}
					}
					else
					{
						rounding = 10;
					}
				}
				return new v128(X86.F16C.FloatToHalf(a.UInt0, rounding), X86.F16C.FloatToHalf(a.UInt1, rounding), X86.F16C.FloatToHalf(a.UInt2, rounding), X86.F16C.FloatToHalf(a.UInt3, rounding), 0, 0, 0, 0);
			}

			[DebuggerStepThrough]
			public static v128 mm256_cvtps_ph(v256 a, int rounding)
			{
				if (rounding == 12)
				{
					X86.MXCSRBits mxcsrbits = X86.MXCSR & X86.MXCSRBits.RoundingControlMask;
					if (mxcsrbits <= X86.MXCSRBits.RoundDown)
					{
						if (mxcsrbits != X86.MXCSRBits.RoundToNearest)
						{
							if (mxcsrbits == X86.MXCSRBits.RoundDown)
							{
								rounding = 9;
							}
						}
						else
						{
							rounding = 8;
						}
					}
					else if (mxcsrbits != X86.MXCSRBits.RoundUp)
					{
						if (mxcsrbits == X86.MXCSRBits.RoundingControlMask)
						{
							rounding = 11;
						}
					}
					else
					{
						rounding = 10;
					}
				}
				return new v128(X86.F16C.FloatToHalf(a.UInt0, rounding), X86.F16C.FloatToHalf(a.UInt1, rounding), X86.F16C.FloatToHalf(a.UInt2, rounding), X86.F16C.FloatToHalf(a.UInt3, rounding), X86.F16C.FloatToHalf(a.UInt4, rounding), X86.F16C.FloatToHalf(a.UInt5, rounding), X86.F16C.FloatToHalf(a.UInt6, rounding), X86.F16C.FloatToHalf(a.UInt7, rounding));
			}

			private static readonly ushort[] BaseTable = new ushort[]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 1, 2, 4, 8, 16, 32, 64,
				128, 256, 512, 1024, 2048, 3072, 4096, 5120, 6144, 7168,
				8192, 9216, 10240, 11264, 12288, 13312, 14336, 15360, 16384, 17408,
				18432, 19456, 20480, 21504, 22528, 23552, 24576, 25600, 26624, 27648,
				28672, 29696, 30720, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744, 31744,
				31744, 31744, 31744, 31744, 31744, 31744, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768,
				32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32768, 32769,
				32770, 32772, 32776, 32784, 32800, 32832, 32896, 33024, 33280, 33792,
				34816, 35840, 36864, 37888, 38912, 39936, 40960, 41984, 43008, 44032,
				45056, 46080, 47104, 48128, 49152, 50176, 51200, 52224, 53248, 54272,
				55296, 56320, 57344, 58368, 59392, 60416, 61440, 62464, 63488, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512, 64512,
				64512, 64512
			};

			private static readonly sbyte[] ShiftTable = new sbyte[]
			{
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 23, 22, 21, 20, 19, 18, 17,
				16, 15, 14, 13, 13, 13, 13, 13, 13, 13,
				13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
				13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
				13, 13, 13, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 13, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 23,
				22, 21, 20, 19, 18, 17, 16, 15, 14, 13,
				13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
				13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
				13, 13, 13, 13, 13, 13, 13, 13, 13, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
				24, 13
			};
		}

		public static class Fma
		{
			public static bool IsFmaSupported
			{
				get
				{
					return X86.Avx2.IsAvx2Supported;
				}
			}

			[DebuggerStepThrough]
			private static float FmaHelper(float a, float b, float c)
			{
				return (float)((double)a * (double)b + (double)c);
			}

			[DebuggerStepThrough]
			private static float FnmaHelper(float a, float b, float c)
			{
				return X86.Fma.FmaHelper(-a, b, c);
			}

			[DebuggerStepThrough]
			public static v128 fmadd_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmadd_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmadd_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmadd_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, c.Float3), X86.Fma.FmaHelper(a.Float4, b.Float4, c.Float4), X86.Fma.FmaHelper(a.Float5, b.Float5, c.Float5), X86.Fma.FmaHelper(a.Float6, b.Float6, c.Float6), X86.Fma.FmaHelper(a.Float7, b.Float7, c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fmadd_sd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmadd_ss(v128 a, v128 b, v128 c)
			{
				v128 v = a;
				v.Float0 = X86.Fma.FmaHelper(a.Float0, b.Float0, c.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 fmaddsub_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmaddsub_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmaddsub_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmaddsub_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, c.Float3), X86.Fma.FmaHelper(a.Float4, b.Float4, -c.Float4), X86.Fma.FmaHelper(a.Float5, b.Float5, c.Float5), X86.Fma.FmaHelper(a.Float6, b.Float6, -c.Float6), X86.Fma.FmaHelper(a.Float7, b.Float7, c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fmsub_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmsub_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmsub_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, -c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmsub_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, -c.Float3), X86.Fma.FmaHelper(a.Float4, b.Float4, -c.Float4), X86.Fma.FmaHelper(a.Float5, b.Float5, -c.Float5), X86.Fma.FmaHelper(a.Float6, b.Float6, -c.Float6), X86.Fma.FmaHelper(a.Float7, b.Float7, -c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fmsub_sd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmsub_ss(v128 a, v128 b, v128 c)
			{
				v128 v = a;
				v.Float0 = X86.Fma.FmaHelper(a.Float0, b.Float0, -c.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 fmsubadd_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmsubadd_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fmsubadd_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, -c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fmsubadd_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FmaHelper(a.Float3, b.Float3, -c.Float3), X86.Fma.FmaHelper(a.Float4, b.Float4, c.Float4), X86.Fma.FmaHelper(a.Float5, b.Float5, -c.Float5), X86.Fma.FmaHelper(a.Float6, b.Float6, c.Float6), X86.Fma.FmaHelper(a.Float7, b.Float7, -c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fnmadd_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fnmadd_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fnmadd_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FnmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FnmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FnmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FnmaHelper(a.Float3, b.Float3, c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fnmadd_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FnmaHelper(a.Float0, b.Float0, c.Float0), X86.Fma.FnmaHelper(a.Float1, b.Float1, c.Float1), X86.Fma.FnmaHelper(a.Float2, b.Float2, c.Float2), X86.Fma.FnmaHelper(a.Float3, b.Float3, c.Float3), X86.Fma.FnmaHelper(a.Float4, b.Float4, c.Float4), X86.Fma.FnmaHelper(a.Float5, b.Float5, c.Float5), X86.Fma.FnmaHelper(a.Float6, b.Float6, c.Float6), X86.Fma.FnmaHelper(a.Float7, b.Float7, c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fnmadd_sd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fnmadd_ss(v128 a, v128 b, v128 c)
			{
				v128 v = a;
				v.Float0 = X86.Fma.FnmaHelper(a.Float0, b.Float0, c.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 fnmsub_pd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v256 mm256_fnmsub_pd(v256 a, v256 b, v256 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fnmsub_ps(v128 a, v128 b, v128 c)
			{
				return new v128(X86.Fma.FnmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FnmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FnmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FnmaHelper(a.Float3, b.Float3, -c.Float3));
			}

			[DebuggerStepThrough]
			public static v256 mm256_fnmsub_ps(v256 a, v256 b, v256 c)
			{
				return new v256(X86.Fma.FnmaHelper(a.Float0, b.Float0, -c.Float0), X86.Fma.FnmaHelper(a.Float1, b.Float1, -c.Float1), X86.Fma.FnmaHelper(a.Float2, b.Float2, -c.Float2), X86.Fma.FnmaHelper(a.Float3, b.Float3, -c.Float3), X86.Fma.FnmaHelper(a.Float4, b.Float4, -c.Float4), X86.Fma.FnmaHelper(a.Float5, b.Float5, -c.Float5), X86.Fma.FnmaHelper(a.Float6, b.Float6, -c.Float6), X86.Fma.FnmaHelper(a.Float7, b.Float7, -c.Float7));
			}

			[DebuggerStepThrough]
			public static v128 fnmsub_sd(v128 a, v128 b, v128 c)
			{
				throw new Exception("Double-precision FMA not emulated in C#");
			}

			[DebuggerStepThrough]
			public static v128 fnmsub_ss(v128 a, v128 b, v128 c)
			{
				v128 v = a;
				v.Float0 = X86.Fma.FnmaHelper(a.Float0, b.Float0, -c.Float0);
				return v;
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct Union
			{
				[FieldOffset(0)]
				public float f;

				[FieldOffset(0)]
				public uint u;
			}
		}

		public static class Popcnt
		{
			public static bool IsPopcntSupported
			{
				get
				{
					return X86.Sse4_2.IsSse42Supported;
				}
			}

			[DebuggerStepThrough]
			public static int popcnt_u32(uint v)
			{
				int num = 0;
				for (uint num2 = 2147483648U; num2 != 0U; num2 >>= 1)
				{
					num += (((v & num2) != 0U) ? 1 : 0);
				}
				return num;
			}

			[DebuggerStepThrough]
			public static int popcnt_u64(ulong v)
			{
				int num = 0;
				for (ulong num2 = 9223372036854775808UL; num2 != 0UL; num2 >>= 1)
				{
					num += (((v & num2) != 0UL) ? 1 : 0);
				}
				return num;
			}
		}

		public static class Sse
		{
			public static bool IsSseSupported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static v128 load_ps(void* ptr)
			{
				return X86.GenericCSharpLoad(ptr);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static v128 loadu_ps(void* ptr)
			{
				return X86.GenericCSharpLoad(ptr);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static void store_ps(void* ptr, v128 val)
			{
				X86.GenericCSharpStore(ptr, val);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static void storeu_ps(void* ptr, v128 val)
			{
				X86.GenericCSharpStore(ptr, val);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static void stream_ps(void* mem_addr, v128 a)
			{
				X86.GenericCSharpStore(mem_addr, a);
			}

			[DebuggerStepThrough]
			public static v128 cvtsi32_ss(v128 a, int b)
			{
				v128 v = a;
				v.Float0 = (float)b;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cvtsi64_ss(v128 a, long b)
			{
				v128 v = a;
				v.Float0 = (float)b;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 add_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 += b.Float0;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 add_ps(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 += b.Float0;
				v.Float1 += b.Float1;
				v.Float2 += b.Float2;
				v.Float3 += b.Float3;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 sub_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = a.Float0 - b.Float0;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 sub_ps(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 -= b.Float0;
				v.Float1 -= b.Float1;
				v.Float2 -= b.Float2;
				v.Float3 -= b.Float3;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 mul_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = a.Float0 * b.Float0;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 mul_ps(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 *= b.Float0;
				v.Float1 *= b.Float1;
				v.Float2 *= b.Float2;
				v.Float3 *= b.Float3;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 div_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = a.Float0 / b.Float0;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 div_ps(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 /= b.Float0;
				v.Float1 /= b.Float1;
				v.Float2 /= b.Float2;
				v.Float3 /= b.Float3;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 sqrt_ss(v128 a)
			{
				v128 v = a;
				v.Float0 = (float)Math.Sqrt((double)a.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 sqrt_ps(v128 a)
			{
				return new v128
				{
					Float0 = (float)Math.Sqrt((double)a.Float0),
					Float1 = (float)Math.Sqrt((double)a.Float1),
					Float2 = (float)Math.Sqrt((double)a.Float2),
					Float3 = (float)Math.Sqrt((double)a.Float3)
				};
			}

			[DebuggerStepThrough]
			public static v128 rcp_ss(v128 a)
			{
				v128 v = a;
				v.Float0 = 1f / a.Float0;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 rcp_ps(v128 a)
			{
				return new v128
				{
					Float0 = 1f / a.Float0,
					Float1 = 1f / a.Float1,
					Float2 = 1f / a.Float2,
					Float3 = 1f / a.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 rsqrt_ss(v128 a)
			{
				v128 v = a;
				v.Float0 = 1f / (float)Math.Sqrt((double)a.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 rsqrt_ps(v128 a)
			{
				return new v128
				{
					Float0 = 1f / (float)Math.Sqrt((double)a.Float0),
					Float1 = 1f / (float)Math.Sqrt((double)a.Float1),
					Float2 = 1f / (float)Math.Sqrt((double)a.Float2),
					Float3 = 1f / (float)Math.Sqrt((double)a.Float3)
				};
			}

			[DebuggerStepThrough]
			public static v128 min_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = Math.Min(a.Float0, b.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 min_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = Math.Min(a.Float0, b.Float0),
					Float1 = Math.Min(a.Float1, b.Float1),
					Float2 = Math.Min(a.Float2, b.Float2),
					Float3 = Math.Min(a.Float3, b.Float3)
				};
			}

			[DebuggerStepThrough]
			public static v128 max_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = Math.Max(a.Float0, b.Float0);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 max_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = Math.Max(a.Float0, b.Float0),
					Float1 = Math.Max(a.Float1, b.Float1),
					Float2 = Math.Max(a.Float2, b.Float2),
					Float3 = Math.Max(a.Float3, b.Float3)
				};
			}

			[DebuggerStepThrough]
			public static v128 and_ps(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 &= b.UInt0;
				v.UInt1 &= b.UInt1;
				v.UInt2 &= b.UInt2;
				v.UInt3 &= b.UInt3;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 andnot_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = (~a.UInt0 & b.UInt0),
					UInt1 = (~a.UInt1 & b.UInt1),
					UInt2 = (~a.UInt2 & b.UInt2),
					UInt3 = (~a.UInt3 & b.UInt3)
				};
			}

			[DebuggerStepThrough]
			public static v128 or_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = (a.UInt0 | b.UInt0),
					UInt1 = (a.UInt1 | b.UInt1),
					UInt2 = (a.UInt2 | b.UInt2),
					UInt3 = (a.UInt3 | b.UInt3)
				};
			}

			[DebuggerStepThrough]
			public static v128 xor_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = (a.UInt0 ^ b.UInt0),
					UInt1 = (a.UInt1 ^ b.UInt1),
					UInt2 = (a.UInt2 ^ b.UInt2),
					UInt3 = (a.UInt3 ^ b.UInt3)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpeq_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 == b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpeq_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 == b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 == b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 == b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 == b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmplt_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 < b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmplt_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 < b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 < b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 < b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 < b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmple_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 <= b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmple_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 <= b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 <= b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 <= b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 <= b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpgt_ss(v128 a, v128 b)
			{
				return X86.Sse.cmplt_ss(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpgt_ps(v128 a, v128 b)
			{
				return X86.Sse.cmplt_ps(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpge_ss(v128 a, v128 b)
			{
				return X86.Sse.cmple_ss(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpge_ps(v128 a, v128 b)
			{
				return X86.Sse.cmple_ps(b, a);
			}

			[DebuggerStepThrough]
			public static v128 cmpneq_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 != b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpneq_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 != b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 != b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 != b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 != b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnlt_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 >= b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpnlt_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 >= b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 >= b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 >= b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 >= b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnle_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((a.Float0 > b.Float0) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpnle_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((a.Float0 > b.Float0) ? uint.MaxValue : 0U),
					UInt1 = ((a.Float1 > b.Float1) ? uint.MaxValue : 0U),
					UInt2 = ((a.Float2 > b.Float2) ? uint.MaxValue : 0U),
					UInt3 = ((a.Float3 > b.Float3) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpngt_ss(v128 a, v128 b)
			{
				return X86.Sse.cmpnlt_ss(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpngt_ps(v128 a, v128 b)
			{
				return X86.Sse.cmpnlt_ps(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpnge_ss(v128 a, v128 b)
			{
				return X86.Sse.cmpnle_ss(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpnge_ps(v128 a, v128 b)
			{
				return X86.Sse.cmpnle_ps(b, a);
			}

			[DebuggerStepThrough]
			public static v128 cmpord_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((X86.IsNaN(a.UInt0) || X86.IsNaN(b.UInt0)) ? 0U : uint.MaxValue);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpord_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((X86.IsNaN(a.UInt0) || X86.IsNaN(b.UInt0)) ? 0U : uint.MaxValue),
					UInt1 = ((X86.IsNaN(a.UInt1) || X86.IsNaN(b.UInt1)) ? 0U : uint.MaxValue),
					UInt2 = ((X86.IsNaN(a.UInt2) || X86.IsNaN(b.UInt2)) ? 0U : uint.MaxValue),
					UInt3 = ((X86.IsNaN(a.UInt3) || X86.IsNaN(b.UInt3)) ? 0U : uint.MaxValue)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpunord_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.UInt0 = ((X86.IsNaN(a.UInt0) || X86.IsNaN(b.UInt0)) ? uint.MaxValue : 0U);
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cmpunord_ps(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = ((X86.IsNaN(a.UInt0) || X86.IsNaN(b.UInt0)) ? uint.MaxValue : 0U),
					UInt1 = ((X86.IsNaN(a.UInt1) || X86.IsNaN(b.UInt1)) ? uint.MaxValue : 0U),
					UInt2 = ((X86.IsNaN(a.UInt2) || X86.IsNaN(b.UInt2)) ? uint.MaxValue : 0U),
					UInt3 = ((X86.IsNaN(a.UInt3) || X86.IsNaN(b.UInt3)) ? uint.MaxValue : 0U)
				};
			}

			[DebuggerStepThrough]
			public static int comieq_ss(v128 a, v128 b)
			{
				if (a.Float0 != b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comilt_ss(v128 a, v128 b)
			{
				if (a.Float0 >= b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comile_ss(v128 a, v128 b)
			{
				if (a.Float0 > b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comigt_ss(v128 a, v128 b)
			{
				if (a.Float0 <= b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comige_ss(v128 a, v128 b)
			{
				if (a.Float0 < b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comineq_ss(v128 a, v128 b)
			{
				if (a.Float0 == b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomieq_ss(v128 a, v128 b)
			{
				if (a.Float0 != b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomilt_ss(v128 a, v128 b)
			{
				if (a.Float0 >= b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomile_ss(v128 a, v128 b)
			{
				if (a.Float0 > b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomigt_ss(v128 a, v128 b)
			{
				if (a.Float0 <= b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomige_ss(v128 a, v128 b)
			{
				if (a.Float0 < b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomineq_ss(v128 a, v128 b)
			{
				if (a.Float0 == b.Float0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static int cvtss_si32(v128 a)
			{
				return X86.Sse.cvt_ss2si(a);
			}

			[DebuggerStepThrough]
			public static int cvt_ss2si(v128 a)
			{
				return (int)Math.Round((double)a.Float0, MidpointRounding.ToEven);
			}

			[DebuggerStepThrough]
			public static long cvtss_si64(v128 a)
			{
				return (long)Math.Round((double)a.Float0, MidpointRounding.ToEven);
			}

			[DebuggerStepThrough]
			public static float cvtss_f32(v128 a)
			{
				return a.Float0;
			}

			[DebuggerStepThrough]
			public static int cvttss_si32(v128 a)
			{
				int num;
				using (new X86.RoundingScope(X86.MXCSRBits.RoundingControlMask))
				{
					num = (int)a.Float0;
				}
				return num;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static int cvtt_ss2si(v128 a)
			{
				return X86.Sse.cvttss_si32(a);
			}

			[DebuggerStepThrough]
			public static long cvttss_si64(v128 a)
			{
				long num;
				using (new X86.RoundingScope(X86.MXCSRBits.RoundingControlMask))
				{
					num = (long)a.Float0;
				}
				return num;
			}

			[DebuggerStepThrough]
			public static v128 set_ss(float a)
			{
				return new v128(a, 0f, 0f, 0f);
			}

			[DebuggerStepThrough]
			public static v128 set1_ps(float a)
			{
				return new v128(a, a, a, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 set_ps1(float a)
			{
				return X86.Sse.set1_ps(a);
			}

			[DebuggerStepThrough]
			public static v128 set_ps(float e3, float e2, float e1, float e0)
			{
				return new v128(e0, e1, e2, e3);
			}

			[DebuggerStepThrough]
			public static v128 setr_ps(float e3, float e2, float e1, float e0)
			{
				return new v128(e3, e2, e1, e0);
			}

			[DebuggerStepThrough]
			public static v128 move_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = b.Float0;
				return v;
			}

			public static int SHUFFLE(int d, int c, int b, int a)
			{
				return (a & 3) | ((b & 3) << 2) | ((c & 3) << 4) | ((d & 3) << 6);
			}

			[DebuggerStepThrough]
			public unsafe static v128 shuffle_ps(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				v.UInt0 = ptr[imm8 & 3];
				v.UInt1 = ptr[(imm8 >> 2) & 3];
				v.UInt2 = ptr2[(imm8 >> 4) & 3];
				v.UInt3 = ptr2[(imm8 >> 6) & 3];
				return v;
			}

			[DebuggerStepThrough]
			public static v128 unpackhi_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float2,
					Float1 = b.Float2,
					Float2 = a.Float3,
					Float3 = b.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 unpacklo_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float0,
					Float1 = b.Float0,
					Float2 = a.Float1,
					Float3 = b.Float1
				};
			}

			[DebuggerStepThrough]
			public static v128 movehl_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = b.Float2,
					Float1 = b.Float3,
					Float2 = a.Float2,
					Float3 = a.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 movelh_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float0,
					Float1 = a.Float1,
					Float2 = b.Float0,
					Float3 = b.Float1
				};
			}

			[DebuggerStepThrough]
			public static int movemask_ps(v128 a)
			{
				int num = 0;
				if ((a.UInt0 & 2147483648U) != 0U)
				{
					num |= 1;
				}
				if ((a.UInt1 & 2147483648U) != 0U)
				{
					num |= 2;
				}
				if ((a.UInt2 & 2147483648U) != 0U)
				{
					num |= 4;
				}
				if ((a.UInt3 & 2147483648U) != 0U)
				{
					num |= 8;
				}
				return num;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static void TRANSPOSE4_PS(ref v128 row0, ref v128 row1, ref v128 row2, ref v128 row3)
			{
				v128 v = X86.Sse.shuffle_ps(row0, row1, 68);
				v128 v2 = X86.Sse.shuffle_ps(row0, row1, 238);
				v128 v3 = X86.Sse.shuffle_ps(row2, row3, 68);
				v128 v4 = X86.Sse.shuffle_ps(row2, row3, 238);
				row0 = X86.Sse.shuffle_ps(v, v3, 136);
				row1 = X86.Sse.shuffle_ps(v, v3, 221);
				row2 = X86.Sse.shuffle_ps(v2, v4, 136);
				row3 = X86.Sse.shuffle_ps(v2, v4, 221);
			}

			[DebuggerStepThrough]
			public static v128 setzero_ps()
			{
				return default(v128);
			}

			[DebuggerStepThrough]
			public unsafe static v128 loadu_si16(void* mem_addr)
			{
				return new v128(*(short*)mem_addr, 0, 0, 0, 0, 0, 0, 0);
			}

			public unsafe static void storeu_si16(void* mem_addr, v128 a)
			{
				*(short*)mem_addr = a.SShort0;
			}

			[DebuggerStepThrough]
			public unsafe static v128 loadu_si64(void* mem_addr)
			{
				return new v128(*(long*)mem_addr, 0L);
			}

			[DebuggerStepThrough]
			public unsafe static void storeu_si64(void* mem_addr, v128 a)
			{
				*(long*)mem_addr = a.SLong0;
			}
		}

		public static class Sse2
		{
			public static bool IsSse2Supported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public static int SHUFFLE2(int x, int y)
			{
				return y | (x << 1);
			}

			[DebuggerStepThrough]
			public unsafe static void stream_si32(int* mem_addr, int a)
			{
				*mem_addr = a;
			}

			[DebuggerStepThrough]
			public unsafe static void stream_si64(long* mem_addr, long a)
			{
				*mem_addr = a;
			}

			[DebuggerStepThrough]
			public unsafe static void stream_pd(void* mem_addr, v128 a)
			{
				X86.GenericCSharpStore(mem_addr, a);
			}

			[DebuggerStepThrough]
			public unsafe static void stream_si128(void* mem_addr, v128 a)
			{
				X86.GenericCSharpStore(mem_addr, a);
			}

			[DebuggerStepThrough]
			public unsafe static v128 add_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = ptr2[i] + ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 add_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = ptr2[i] + ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 add_epi32(v128 a, v128 b)
			{
				return new v128
				{
					SInt0 = a.SInt0 + b.SInt0,
					SInt1 = a.SInt1 + b.SInt1,
					SInt2 = a.SInt2 + b.SInt2,
					SInt3 = a.SInt3 + b.SInt3
				};
			}

			[DebuggerStepThrough]
			public static v128 add_epi64(v128 a, v128 b)
			{
				return new v128
				{
					SLong0 = a.SLong0 + b.SLong0,
					SLong1 = a.SLong1 + b.SLong1
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 adds_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = X86.Saturate_To_Int8((int)(ptr2[i] + ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 adds_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = X86.Saturate_To_Int16((int)(ptr2[i] + ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 adds_epu8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = X86.Saturate_To_UnsignedInt8((int)(ptr2[i] + ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 adds_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = X86.Saturate_To_UnsignedInt16((int)(ptr2[i] + ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 avg_epu8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = (byte)(ptr2[i] + ptr3[i] + 1 >> 1);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 avg_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (ushort)(ptr2[i] + ptr3[i] + 1 >> 1);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 madd_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					int num = 2 * i;
					int num2 = (int)(ptr2[num + 1] * ptr3[num + 1]);
					int num3 = (int)(ptr2[num] * ptr3[num]);
					ptr[i] = num2 + num3;
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epu8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epu8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mulhi_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					int num = (int)(ptr2[i] * ptr3[i]);
					ptr[i] = (short)(num >> 16);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mulhi_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					uint num = (uint)(ptr2[i] * ptr3[i]);
					ptr[i] = (ushort)(num >> 16);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mullo_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					int num = (int)(ptr2[i] * ptr3[i]);
					ptr[i] = (short)num;
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 mul_epu32(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (ulong)a.UInt0 * (ulong)b.UInt0,
					ULong1 = (ulong)a.UInt2 * (ulong)b.UInt2
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 sad_epu8(v128 a, v128 b)
			{
				v128 v;
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = (byte)Math.Abs((int)(ptr2[i] - ptr3[i]));
				}
				v128 v2 = default(v128);
				ushort* ptr4 = &v2.UShort0;
				for (int j = 0; j <= 1; j++)
				{
					int num = j * 8;
					ptr4[4 * j] = (ushort)(ptr[num] + ptr[num + 1] + ptr[num + 2] + ptr[num + 3] + ptr[num + 4] + ptr[num + 5] + ptr[num + 6] + ptr[num + 7]);
				}
				return v2;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sub_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = ptr2[i] - ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sub_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = ptr2[i] - ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sub_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = ptr2[i] - ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sub_epi64(v128 a, v128 b)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				long* ptr2 = &a.SLong0;
				long* ptr3 = &b.SLong0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = ptr2[i] - ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 subs_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = X86.Saturate_To_Int8((int)(ptr2[i] - ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 subs_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = X86.Saturate_To_Int16((int)(ptr2[i] - ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 subs_epu8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = X86.Saturate_To_UnsignedInt8((int)(ptr2[i] - ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 subs_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = X86.Saturate_To_UnsignedInt16((int)(ptr2[i] - ptr3[i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 slli_si128(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 16);
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i < num; i++)
				{
					ptr[i] = 0;
				}
				for (int j = num; j < 16; j++)
				{
					ptr[j] = ptr2[j - num];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 bslli_si128(v128 a, int imm8)
			{
				return X86.Sse2.slli_si128(a, imm8);
			}

			[DebuggerStepThrough]
			public unsafe static v128 bsrli_si128(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 16);
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i < 16 - num; i++)
				{
					ptr[i] = ptr2[num + i];
				}
				for (int j = 16 - num; j < 16; j++)
				{
					ptr[j] = 0;
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 slli_epi16(v128 a, int imm8)
			{
				v128 v = default(v128);
				int num = imm8 & 255;
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					if (num > 15)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = (ushort)(ptr2[i] << num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sll_epi16(v128 a, v128 count)
			{
				v128 v = default(v128);
				int num = (int)Math.Min(count.ULong0, 16UL);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					if (num > 15)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = (ushort)(ptr2[i] << num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 slli_epi32(v128 a, int imm8)
			{
				v128 v = default(v128);
				int num = Math.Min(imm8 & 255, 32);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					if (num > 31)
					{
						ptr[i] = 0U;
					}
					else
					{
						ptr[i] = ptr2[i] << num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sll_epi32(v128 a, v128 count)
			{
				v128 v = default(v128);
				int num = (int)Math.Min(count.ULong0, 32UL);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					if (num > 31)
					{
						ptr[i] = 0U;
					}
					else
					{
						ptr[i] = ptr2[i] << num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 slli_epi64(v128 a, int imm8)
			{
				v128 v = default(v128);
				int num = Math.Min(imm8 & 255, 64);
				ulong* ptr = &v.ULong0;
				ulong* ptr2 = &a.ULong0;
				for (int i = 0; i <= 1; i++)
				{
					if (num > 63)
					{
						ptr[i] = 0UL;
					}
					else
					{
						ptr[i] = ptr2[i] << num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sll_epi64(v128 a, v128 count)
			{
				v128 v = default(v128);
				int num = (int)Math.Min(count.ULong0, 64UL);
				ulong* ptr = &v.ULong0;
				ulong* ptr2 = &a.ULong0;
				for (int i = 0; i <= 1; i++)
				{
					if (num > 63)
					{
						ptr[i] = 0UL;
					}
					else
					{
						ptr[i] = ptr2[i] << num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srai_epi16(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 16);
				v128 v = a;
				short* ptr = &v.SShort0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 7; i++)
					{
						short* ptr2 = ptr + i;
						*ptr2 = (short)(*ptr2 >> 1);
						short* ptr3 = ptr + i;
						*ptr3 = (short)(*ptr3 >> num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sra_epi16(v128 a, v128 count)
			{
				int num = (int)Math.Min(count.ULong0, 16UL);
				v128 v = a;
				short* ptr = &v.SShort0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 7; i++)
					{
						short* ptr2 = ptr + i;
						*ptr2 = (short)(*ptr2 >> 1);
						short* ptr3 = ptr + i;
						*ptr3 = (short)(*ptr3 >> num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srai_epi32(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 32);
				v128 v = a;
				int* ptr = &v.SInt0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 3; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sra_epi32(v128 a, v128 count)
			{
				int num = (int)Math.Min(count.ULong0, 32UL);
				v128 v = a;
				int* ptr = &v.SInt0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 3; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 srli_si128(v128 a, int imm8)
			{
				return X86.Sse2.bsrli_si128(a, imm8);
			}

			[DebuggerStepThrough]
			public unsafe static v128 srli_epi16(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 16);
				v128 v = a;
				ushort* ptr = &v.UShort0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 7; i++)
					{
						ushort* ptr2 = ptr + i;
						*ptr2 = (ushort)(*ptr2 >> 1);
						ushort* ptr3 = ptr + i;
						*ptr3 = (ushort)(*ptr3 >> num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srl_epi16(v128 a, v128 count)
			{
				int num = (int)Math.Min(count.ULong0, 16UL);
				v128 v = a;
				ushort* ptr = &v.UShort0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 7; i++)
					{
						ushort* ptr2 = ptr + i;
						*ptr2 = (ushort)(*ptr2 >> 1);
						ushort* ptr3 = ptr + i;
						*ptr3 = (ushort)(*ptr3 >> num);
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srli_epi32(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 32);
				v128 v = a;
				uint* ptr = &v.UInt0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 3; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srl_epi32(v128 a, v128 count)
			{
				int num = (int)Math.Min(count.ULong0, 32UL);
				v128 v = a;
				uint* ptr = &v.UInt0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 3; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srli_epi64(v128 a, int imm8)
			{
				int num = Math.Min(imm8 & 255, 64);
				v128 v = a;
				ulong* ptr = &v.ULong0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 1; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 srl_epi64(v128 a, v128 count)
			{
				int num = (int)Math.Min(count.ULong0, 64UL);
				v128 v = a;
				ulong* ptr = &v.ULong0;
				if (num > 0)
				{
					num--;
					for (int i = 0; i <= 1; i++)
					{
						ptr[i] >>= 1;
						ptr[i] >>= num;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 and_si128(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 & b.ULong0),
					ULong1 = (a.ULong1 & b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 andnot_si128(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (~a.ULong0 & b.ULong0),
					ULong1 = (~a.ULong1 & b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 or_si128(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 | b.ULong0),
					ULong1 = (a.ULong1 | b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 xor_si128(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 ^ b.ULong0),
					ULong1 = (a.ULong1 ^ b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpeq_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &a.Byte0;
				byte* ptr2 = &b.Byte0;
				byte* ptr3 = &v.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr3[i] = ((ptr[i] == ptr2[i]) ? byte.MaxValue : 0);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpeq_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &a.UShort0;
				ushort* ptr2 = &b.UShort0;
				ushort* ptr3 = &v.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr3[i] = ((ptr[i] == ptr2[i]) ? ushort.MaxValue : 0);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpeq_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				uint* ptr = &a.UInt0;
				uint* ptr2 = &b.UInt0;
				uint* ptr3 = &v.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr3[i] = ((ptr[i] == ptr2[i]) ? uint.MaxValue : 0U);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpgt_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &a.SByte0;
				sbyte* ptr2 = &b.SByte0;
				sbyte* ptr3 = &v.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr3[i] = ((ptr[i] > ptr2[i]) ? -1 : 0);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpgt_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &a.SShort0;
				short* ptr2 = &b.SShort0;
				short* ptr3 = &v.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr3[i] = ((ptr[i] > ptr2[i]) ? -1 : 0);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpgt_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &a.SInt0;
				int* ptr2 = &b.SInt0;
				int* ptr3 = &v.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr3[i] = ((ptr[i] > ptr2[i]) ? (-1) : 0);
				}
				return v;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmplt_epi8(v128 a, v128 b)
			{
				return X86.Sse2.cmpgt_epi8(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmplt_epi16(v128 a, v128 b)
			{
				return X86.Sse2.cmpgt_epi16(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmplt_epi32(v128 a, v128 b)
			{
				return X86.Sse2.cmpgt_epi32(b, a);
			}

			[DebuggerStepThrough]
			public static v128 cvtepi32_pd(v128 a)
			{
				return new v128
				{
					Double0 = (double)a.SInt0,
					Double1 = (double)a.SInt1
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtsi32_sd(v128 a, int b)
			{
				v128 v = a;
				v.Double0 = (double)b;
				return v;
			}

			[DebuggerStepThrough]
			public static v128 cvtsi64_sd(v128 a, long b)
			{
				v128 v = a;
				v.Double0 = (double)b;
				return v;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cvtsi64x_sd(v128 a, long b)
			{
				return X86.Sse2.cvtsi64_sd(a, b);
			}

			[DebuggerStepThrough]
			public static v128 cvtepi32_ps(v128 a)
			{
				return new v128
				{
					Float0 = (float)a.SInt0,
					Float1 = (float)a.SInt1,
					Float2 = (float)a.SInt2,
					Float3 = (float)a.SInt3
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtsi32_si128(int a)
			{
				return new v128
				{
					SInt0 = a
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtsi64_si128(long a)
			{
				return new v128
				{
					SLong0 = a
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtsi64x_si128(long a)
			{
				return X86.Sse2.cvtsi64_si128(a);
			}

			[DebuggerStepThrough]
			public static int cvtsi128_si32(v128 a)
			{
				return a.SInt0;
			}

			[DebuggerStepThrough]
			public static long cvtsi128_si64(v128 a)
			{
				return a.SLong0;
			}

			[DebuggerStepThrough]
			public static long cvtsi128_si64x(v128 a)
			{
				return a.SLong0;
			}

			[DebuggerStepThrough]
			public static v128 set_epi64x(long e1, long e0)
			{
				return new v128
				{
					SLong0 = e0,
					SLong1 = e1
				};
			}

			[DebuggerStepThrough]
			public static v128 set_epi32(int e3, int e2, int e1, int e0)
			{
				return new v128
				{
					SInt0 = e0,
					SInt1 = e1,
					SInt2 = e2,
					SInt3 = e3
				};
			}

			[DebuggerStepThrough]
			public static v128 set_epi16(short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
			{
				return new v128
				{
					SShort0 = e0,
					SShort1 = e1,
					SShort2 = e2,
					SShort3 = e3,
					SShort4 = e4,
					SShort5 = e5,
					SShort6 = e6,
					SShort7 = e7
				};
			}

			[DebuggerStepThrough]
			public static v128 set_epi8(sbyte e15_, sbyte e14_, sbyte e13_, sbyte e12_, sbyte e11_, sbyte e10_, sbyte e9_, sbyte e8_, sbyte e7_, sbyte e6_, sbyte e5_, sbyte e4_, sbyte e3_, sbyte e2_, sbyte e1_, sbyte e0_)
			{
				return new v128
				{
					SByte0 = e0_,
					SByte1 = e1_,
					SByte2 = e2_,
					SByte3 = e3_,
					SByte4 = e4_,
					SByte5 = e5_,
					SByte6 = e6_,
					SByte7 = e7_,
					SByte8 = e8_,
					SByte9 = e9_,
					SByte10 = e10_,
					SByte11 = e11_,
					SByte12 = e12_,
					SByte13 = e13_,
					SByte14 = e14_,
					SByte15 = e15_
				};
			}

			[DebuggerStepThrough]
			public static v128 set1_epi64x(long a)
			{
				return new v128
				{
					SLong0 = a,
					SLong1 = a
				};
			}

			[DebuggerStepThrough]
			public static v128 set1_epi32(int a)
			{
				return new v128
				{
					SInt0 = a,
					SInt1 = a,
					SInt2 = a,
					SInt3 = a
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 set1_epi16(short a)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = a;
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 set1_epi8(sbyte a)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = a;
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 setr_epi32(int e3, int e2, int e1, int e0)
			{
				return new v128
				{
					SInt0 = e3,
					SInt1 = e2,
					SInt2 = e1,
					SInt3 = e0
				};
			}

			[DebuggerStepThrough]
			public static v128 setr_epi16(short e7, short e6, short e5, short e4, short e3, short e2, short e1, short e0)
			{
				return new v128
				{
					SShort0 = e7,
					SShort1 = e6,
					SShort2 = e5,
					SShort3 = e4,
					SShort4 = e3,
					SShort5 = e2,
					SShort6 = e1,
					SShort7 = e0
				};
			}

			[DebuggerStepThrough]
			public static v128 setr_epi8(sbyte e15_, sbyte e14_, sbyte e13_, sbyte e12_, sbyte e11_, sbyte e10_, sbyte e9_, sbyte e8_, sbyte e7_, sbyte e6_, sbyte e5_, sbyte e4_, sbyte e3_, sbyte e2_, sbyte e1_, sbyte e0_)
			{
				return new v128
				{
					SByte0 = e15_,
					SByte1 = e14_,
					SByte2 = e13_,
					SByte3 = e12_,
					SByte4 = e11_,
					SByte5 = e10_,
					SByte6 = e9_,
					SByte7 = e8_,
					SByte8 = e7_,
					SByte9 = e6_,
					SByte10 = e5_,
					SByte11 = e4_,
					SByte12 = e3_,
					SByte13 = e2_,
					SByte14 = e1_,
					SByte15 = e0_
				};
			}

			[DebuggerStepThrough]
			public static v128 setzero_si128()
			{
				return default(v128);
			}

			[DebuggerStepThrough]
			public static v128 move_epi64(v128 a)
			{
				return new v128
				{
					ULong0 = a.ULong0,
					ULong1 = 0UL
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 packs_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &a.SShort0;
				short* ptr2 = &b.SShort0;
				sbyte* ptr3 = &v.SByte0;
				for (int i = 0; i < 8; i++)
				{
					ptr3[i] = X86.Saturate_To_Int8((int)ptr[i]);
				}
				for (int j = 0; j < 8; j++)
				{
					ptr3[j + 8] = X86.Saturate_To_Int8((int)ptr2[j]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 packs_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &a.SInt0;
				int* ptr2 = &b.SInt0;
				short* ptr3 = &v.SShort0;
				for (int i = 0; i < 4; i++)
				{
					ptr3[i] = X86.Saturate_To_Int16(ptr[i]);
				}
				for (int j = 0; j < 4; j++)
				{
					ptr3[j + 4] = X86.Saturate_To_Int16(ptr2[j]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 packus_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &a.SShort0;
				short* ptr2 = &b.SShort0;
				byte* ptr3 = &v.Byte0;
				for (int i = 0; i < 8; i++)
				{
					ptr3[i] = X86.Saturate_To_UnsignedInt8((int)ptr[i]);
				}
				for (int j = 0; j < 8; j++)
				{
					ptr3[j + 8] = X86.Saturate_To_UnsignedInt8((int)ptr2[j]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static ushort extract_epi16(v128 a, int imm8)
			{
				return (&a.UShort0)[imm8 & 7];
			}

			[DebuggerStepThrough]
			public unsafe static v128 insert_epi16(v128 a, int i, int imm8)
			{
				v128 v = a;
				(&v.SShort0)[imm8 & 7] = (short)i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static int movemask_epi8(v128 a)
			{
				int num = 0;
				byte* ptr = &a.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					if ((ptr[i] & 128) != 0)
					{
						num |= 1 << i;
					}
				}
				return num;
			}

			[DebuggerStepThrough]
			public unsafe static v128 shuffle_epi32(v128 a, int imm8)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				*ptr = ptr2[imm8 & 3];
				ptr[1] = ptr2[(imm8 >> 2) & 3];
				ptr[2] = ptr2[(imm8 >> 4) & 3];
				ptr[3] = ptr2[(imm8 >> 6) & 3];
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 shufflehi_epi16(v128 a, int imm8)
			{
				v128 v = a;
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				ptr[4] = ptr2[4 + (imm8 & 3)];
				ptr[5] = ptr2[4 + ((imm8 >> 2) & 3)];
				ptr[6] = ptr2[4 + ((imm8 >> 4) & 3)];
				ptr[7] = ptr2[4 + ((imm8 >> 6) & 3)];
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 shufflelo_epi16(v128 a, int imm8)
			{
				v128 v = a;
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				*ptr = ptr2[imm8 & 3];
				ptr[1] = ptr2[(imm8 >> 2) & 3];
				ptr[2] = ptr2[(imm8 >> 4) & 3];
				ptr[3] = ptr2[(imm8 >> 6) & 3];
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 unpackhi_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[2 * i] = ptr2[i + 8];
					ptr[2 * i + 1] = ptr3[i + 8];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 unpackhi_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[2 * i] = ptr2[i + 4];
					ptr[2 * i + 1] = ptr3[i + 4];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 unpackhi_epi32(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = a.UInt2,
					UInt1 = b.UInt2,
					UInt2 = a.UInt3,
					UInt3 = b.UInt3
				};
			}

			[DebuggerStepThrough]
			public static v128 unpackhi_epi64(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = a.ULong1,
					ULong1 = b.ULong1
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 unpacklo_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[2 * i] = ptr2[i];
					ptr[2 * i + 1] = ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 unpacklo_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[2 * i] = ptr2[i];
					ptr[2 * i + 1] = ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 unpacklo_epi32(v128 a, v128 b)
			{
				return new v128
				{
					UInt0 = a.UInt0,
					UInt1 = b.UInt0,
					UInt2 = a.UInt1,
					UInt3 = b.UInt1
				};
			}

			[DebuggerStepThrough]
			public static v128 unpacklo_epi64(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = a.ULong0,
					ULong1 = b.ULong0
				};
			}

			[DebuggerStepThrough]
			public static v128 add_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 + b.Double0,
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 add_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 + b.Double0,
					Double1 = a.Double1 + b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 div_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 / b.Double0,
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 div_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 / b.Double0,
					Double1 = a.Double1 / b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 max_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = Math.Max(a.Double0, b.Double0),
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 max_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = Math.Max(a.Double0, b.Double0),
					Double1 = Math.Max(a.Double1, b.Double1)
				};
			}

			[DebuggerStepThrough]
			public static v128 min_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = Math.Min(a.Double0, b.Double0),
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 min_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = Math.Min(a.Double0, b.Double0),
					Double1 = Math.Min(a.Double1, b.Double1)
				};
			}

			[DebuggerStepThrough]
			public static v128 mul_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 * b.Double0,
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 mul_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 * b.Double0,
					Double1 = a.Double1 * b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 sqrt_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = Math.Sqrt(b.Double0),
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 sqrt_pd(v128 a)
			{
				return new v128
				{
					Double0 = Math.Sqrt(a.Double0),
					Double1 = Math.Sqrt(a.Double1)
				};
			}

			[DebuggerStepThrough]
			public static v128 sub_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 - b.Double0,
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 sub_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 - b.Double0,
					Double1 = a.Double1 - b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 and_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 & b.ULong0),
					ULong1 = (a.ULong1 & b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 andnot_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (~a.ULong0 & b.ULong0),
					ULong1 = (~a.ULong1 & b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 or_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 | b.ULong0),
					ULong1 = (a.ULong1 | b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 xor_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = (a.ULong0 ^ b.ULong0),
					ULong1 = (a.ULong1 ^ b.ULong1)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpeq_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 == b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmplt_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 < b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmple_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 <= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpgt_sd(v128 a, v128 b)
			{
				return X86.Sse2.cmple_sd(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpge_sd(v128 a, v128 b)
			{
				return X86.Sse2.cmplt_sd(b, a);
			}

			[DebuggerStepThrough]
			public static v128 cmpord_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((X86.IsNaN(a.ULong0) || X86.IsNaN(b.ULong0)) ? 0UL : ulong.MaxValue),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpunord_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((X86.IsNaN(a.ULong0) || X86.IsNaN(b.ULong0)) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpneq_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 != b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnlt_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 >= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnle_sd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 > b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = a.ULong1
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpngt_sd(v128 a, v128 b)
			{
				return X86.Sse2.cmpnlt_sd(b, a);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 cmpnge_sd(v128 a, v128 b)
			{
				return X86.Sse2.cmpnle_sd(b, a);
			}

			[DebuggerStepThrough]
			public static v128 cmpeq_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 == b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 == b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmplt_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 < b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 < b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmple_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 <= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 <= b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpgt_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 > b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 > b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpge_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 >= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 >= b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpord_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((X86.IsNaN(a.ULong0) || X86.IsNaN(b.ULong0)) ? 0UL : ulong.MaxValue),
					ULong1 = ((X86.IsNaN(a.ULong1) || X86.IsNaN(b.ULong1)) ? 0UL : ulong.MaxValue)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpunord_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((X86.IsNaN(a.ULong0) || X86.IsNaN(b.ULong0)) ? ulong.MaxValue : 0UL),
					ULong1 = ((X86.IsNaN(a.ULong1) || X86.IsNaN(b.ULong1)) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpneq_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 != b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 != b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnlt_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 >= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 >= b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnle_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 > b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 > b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpngt_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 <= b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 <= b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpnge_pd(v128 a, v128 b)
			{
				return new v128
				{
					ULong0 = ((a.Double0 < b.Double0) ? ulong.MaxValue : 0UL),
					ULong1 = ((a.Double1 < b.Double1) ? ulong.MaxValue : 0UL)
				};
			}

			[DebuggerStepThrough]
			public static int comieq_sd(v128 a, v128 b)
			{
				if (a.Double0 != b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comilt_sd(v128 a, v128 b)
			{
				if (a.Double0 >= b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comile_sd(v128 a, v128 b)
			{
				if (a.Double0 > b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comigt_sd(v128 a, v128 b)
			{
				if (a.Double0 <= b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comige_sd(v128 a, v128 b)
			{
				if (a.Double0 < b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int comineq_sd(v128 a, v128 b)
			{
				if (a.Double0 == b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomieq_sd(v128 a, v128 b)
			{
				if (a.Double0 != b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomilt_sd(v128 a, v128 b)
			{
				if (a.Double0 >= b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomile_sd(v128 a, v128 b)
			{
				if (a.Double0 > b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomigt_sd(v128 a, v128 b)
			{
				if (a.Double0 <= b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomige_sd(v128 a, v128 b)
			{
				if (a.Double0 < b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int ucomineq_sd(v128 a, v128 b)
			{
				if (a.Double0 == b.Double0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static v128 cvtpd_ps(v128 a)
			{
				return new v128
				{
					Float0 = (float)a.Double0,
					Float1 = (float)a.Double1,
					Float2 = 0f,
					Float3 = 0f
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtps_pd(v128 a)
			{
				return new v128
				{
					Double0 = (double)a.Float0,
					Double1 = (double)a.Float1
				};
			}

			[DebuggerStepThrough]
			public static v128 cvtpd_epi32(v128 a)
			{
				return new v128
				{
					SInt0 = (int)Math.Round(a.Double0),
					SInt1 = (int)Math.Round(a.Double1)
				};
			}

			[DebuggerStepThrough]
			public static int cvtsd_si32(v128 a)
			{
				return (int)Math.Round(a.Double0);
			}

			[DebuggerStepThrough]
			public static long cvtsd_si64(v128 a)
			{
				return (long)Math.Round(a.Double0);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static long cvtsd_si64x(v128 a)
			{
				return X86.Sse2.cvtsd_si64(a);
			}

			[DebuggerStepThrough]
			public static v128 cvtsd_ss(v128 a, v128 b)
			{
				v128 v = a;
				v.Float0 = (float)b.Double0;
				return v;
			}

			[DebuggerStepThrough]
			public static double cvtsd_f64(v128 a)
			{
				return a.Double0;
			}

			[DebuggerStepThrough]
			public static v128 cvtss_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = (double)b.Float0,
					Double1 = (double)a.Float0
				};
			}

			[DebuggerStepThrough]
			public static v128 cvttpd_epi32(v128 a)
			{
				return new v128
				{
					SInt0 = (int)a.Double0,
					SInt1 = (int)a.Double1
				};
			}

			[DebuggerStepThrough]
			public static int cvttsd_si32(v128 a)
			{
				return (int)a.Double0;
			}

			[DebuggerStepThrough]
			public static long cvttsd_si64(v128 a)
			{
				return (long)a.Double0;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static long cvttsd_si64x(v128 a)
			{
				return X86.Sse2.cvttsd_si64(a);
			}

			[DebuggerStepThrough]
			public static v128 cvtps_epi32(v128 a)
			{
				return new v128
				{
					SInt0 = (int)Math.Round((double)a.Float0),
					SInt1 = (int)Math.Round((double)a.Float1),
					SInt2 = (int)Math.Round((double)a.Float2),
					SInt3 = (int)Math.Round((double)a.Float3)
				};
			}

			[DebuggerStepThrough]
			public static v128 cvttps_epi32(v128 a)
			{
				return new v128
				{
					SInt0 = (int)a.Float0,
					SInt1 = (int)a.Float1,
					SInt2 = (int)a.Float2,
					SInt3 = (int)a.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 set_sd(double a)
			{
				return new v128
				{
					Double0 = a,
					Double1 = 0.0
				};
			}

			[DebuggerStepThrough]
			public static v128 set1_pd(double a)
			{
				return new v128
				{
					Double1 = a,
					Double0 = a
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public static v128 set_pd1(double a)
			{
				return X86.Sse2.set1_pd(a);
			}

			[DebuggerStepThrough]
			public static v128 set_pd(double e1, double e0)
			{
				return new v128
				{
					Double0 = e0,
					Double1 = e1
				};
			}

			[DebuggerStepThrough]
			public static v128 setr_pd(double e1, double e0)
			{
				return new v128
				{
					Double0 = e1,
					Double1 = e0
				};
			}

			[DebuggerStepThrough]
			public static v128 unpackhi_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double1,
					Double1 = b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 unpacklo_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0,
					Double1 = b.Double0
				};
			}

			[DebuggerStepThrough]
			public static int movemask_pd(v128 a)
			{
				int num = 0;
				if ((a.ULong0 & 9223372036854775808UL) != 0UL)
				{
					num |= 1;
				}
				if ((a.ULong1 & 9223372036854775808UL) != 0UL)
				{
					num |= 2;
				}
				return num;
			}

			[DebuggerStepThrough]
			public unsafe static v128 shuffle_pd(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				double* ptr = &a.Double0;
				double* ptr2 = &b.Double0;
				v.Double0 = ptr[imm8 & 1];
				v.Double1 = ptr2[(imm8 >> 1) & 1];
				return v;
			}

			[DebuggerStepThrough]
			public static v128 move_sd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = b.Double0,
					Double1 = a.Double1
				};
			}

			public unsafe static v128 loadu_si32(void* mem_addr)
			{
				return new v128(*(int*)mem_addr, 0, 0, 0);
			}

			public unsafe static void storeu_si32(void* mem_addr, v128 a)
			{
				*(int*)mem_addr = a.SInt0;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static v128 load_si128(void* ptr)
			{
				return X86.GenericCSharpLoad(ptr);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static v128 loadu_si128(void* ptr)
			{
				return X86.GenericCSharpLoad(ptr);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static void store_si128(void* ptr, v128 val)
			{
				X86.GenericCSharpStore(ptr, val);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE2)]
			public unsafe static void storeu_si128(void* ptr, v128 val)
			{
				X86.GenericCSharpStore(ptr, val);
			}

			[DebuggerStepThrough]
			public unsafe static void clflush(void* ptr)
			{
			}
		}

		public static class Sse3
		{
			public static bool IsSse3Supported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public static v128 addsub_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float0 - b.Float0,
					Float1 = a.Float1 + b.Float1,
					Float2 = a.Float2 - b.Float2,
					Float3 = a.Float3 + b.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 addsub_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 - b.Double0,
					Double1 = a.Double1 + b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 hadd_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 + a.Double1,
					Double1 = b.Double0 + b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 hadd_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float0 + a.Float1,
					Float1 = a.Float2 + a.Float3,
					Float2 = b.Float0 + b.Float1,
					Float3 = b.Float2 + b.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 hsub_pd(v128 a, v128 b)
			{
				return new v128
				{
					Double0 = a.Double0 - a.Double1,
					Double1 = b.Double0 - b.Double1
				};
			}

			[DebuggerStepThrough]
			public static v128 hsub_ps(v128 a, v128 b)
			{
				return new v128
				{
					Float0 = a.Float0 - a.Float1,
					Float1 = a.Float2 - a.Float3,
					Float2 = b.Float0 - b.Float1,
					Float3 = b.Float2 - b.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 movedup_pd(v128 a)
			{
				return new v128
				{
					Double0 = a.Double0,
					Double1 = a.Double0
				};
			}

			[DebuggerStepThrough]
			public static v128 movehdup_ps(v128 a)
			{
				return new v128
				{
					Float0 = a.Float1,
					Float1 = a.Float1,
					Float2 = a.Float3,
					Float3 = a.Float3
				};
			}

			[DebuggerStepThrough]
			public static v128 moveldup_ps(v128 a)
			{
				return new v128
				{
					Float0 = a.Float0,
					Float1 = a.Float0,
					Float2 = a.Float2,
					Float3 = a.Float2
				};
			}
		}

		public static class Sse4_1
		{
			public static bool IsSse41Supported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public unsafe static v128 stream_load_si128(void* mem_addr)
			{
				return X86.GenericCSharpLoad(mem_addr);
			}

			[DebuggerStepThrough]
			public unsafe static v128 blend_pd(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				double* ptr = &v.Double0;
				double* ptr2 = &a.Double0;
				double* ptr3 = &b.Double0;
				for (int i = 0; i <= 1; i++)
				{
					if ((imm8 & (1 << i)) != 0)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 blend_ps(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				uint* ptr3 = &b.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					if ((imm8 & (1 << i)) != 0)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 blendv_pd(v128 a, v128 b, v128 mask)
			{
				v128 v = default(v128);
				double* ptr = &v.Double0;
				double* ptr2 = &a.Double0;
				double* ptr3 = &b.Double0;
				long* ptr4 = &mask.SLong0;
				for (int i = 0; i <= 1; i++)
				{
					if (ptr4[i] < 0L)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 blendv_ps(v128 a, v128 b, v128 mask)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				uint* ptr3 = &b.UInt0;
				int* ptr4 = &mask.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					if (ptr4[i] < 0)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 blendv_epi8(v128 a, v128 b, v128 mask)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				sbyte* ptr4 = &mask.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					if (ptr4[i] < 0)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 blend_epi16(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					if (((imm8 >> i) & 1) != 0)
					{
						ptr[i] = ptr3[i];
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 dp_pd(v128 a, v128 b, int imm8)
			{
				double num = (((imm8 & 16) != 0) ? (a.Double0 * b.Double0) : 0.0);
				double num2 = (((imm8 & 32) != 0) ? (a.Double1 * b.Double1) : 0.0);
				double num3 = num + num2;
				return new v128
				{
					Double0 = (((imm8 & 1) != 0) ? num3 : 0.0),
					Double1 = (((imm8 & 2) != 0) ? num3 : 0.0)
				};
			}

			[DebuggerStepThrough]
			public static v128 dp_ps(v128 a, v128 b, int imm8)
			{
				float num = (((imm8 & 16) != 0) ? (a.Float0 * b.Float0) : 0f);
				float num2 = (((imm8 & 32) != 0) ? (a.Float1 * b.Float1) : 0f);
				float num3 = (((imm8 & 64) != 0) ? (a.Float2 * b.Float2) : 0f);
				float num4 = (((imm8 & 128) != 0) ? (a.Float3 * b.Float3) : 0f);
				float num5 = num + num2 + num3 + num4;
				return new v128
				{
					Float0 = (((imm8 & 1) != 0) ? num5 : 0f),
					Float1 = (((imm8 & 2) != 0) ? num5 : 0f),
					Float2 = (((imm8 & 4) != 0) ? num5 : 0f),
					Float3 = (((imm8 & 8) != 0) ? num5 : 0f)
				};
			}

			[DebuggerStepThrough]
			public unsafe static int extract_ps(v128 a, int imm8)
			{
				return (&a.SInt0)[imm8 & 3];
			}

			[DebuggerStepThrough]
			public unsafe static float extractf_ps(v128 a, int imm8)
			{
				return (&a.Float0)[imm8 & 3];
			}

			[DebuggerStepThrough]
			public unsafe static byte extract_epi8(v128 a, int imm8)
			{
				return (&a.Byte0)[imm8 & 15];
			}

			[DebuggerStepThrough]
			public unsafe static int extract_epi32(v128 a, int imm8)
			{
				return (&a.SInt0)[imm8 & 3];
			}

			[DebuggerStepThrough]
			public unsafe static long extract_epi64(v128 a, int imm8)
			{
				return (&a.SLong0)[imm8 & 1];
			}

			[DebuggerStepThrough]
			public unsafe static v128 insert_ps(v128 a, v128 b, int imm8)
			{
				v128 v = a;
				(&v.Float0)[(imm8 >> 4) & 3] = (&b.Float0)[(imm8 >> 6) & 3];
				for (int i = 0; i < 4; i++)
				{
					if ((imm8 & (1 << i)) != 0)
					{
						(&v.Float0)[i] = 0f;
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 insert_epi8(v128 a, byte i, int imm8)
			{
				v128 v = a;
				(&v.Byte0)[imm8 & 15] = i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 insert_epi32(v128 a, int i, int imm8)
			{
				v128 v = a;
				(&v.SInt0)[imm8 & 3] = i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 insert_epi64(v128 a, long i, int imm8)
			{
				v128 v = a;
				(&v.SLong0)[imm8 & 1] = i;
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epu32(v128 a, v128 b)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				uint* ptr3 = &b.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 max_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = Math.Max(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epu32(v128 a, v128 b)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				uint* ptr2 = &a.UInt0;
				uint* ptr3 = &b.UInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 min_epu16(v128 a, v128 b)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				ushort* ptr2 = &a.UShort0;
				ushort* ptr3 = &b.UShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = Math.Min(ptr2[i], ptr3[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 packus_epi32(v128 a, v128 b)
			{
				return new v128
				{
					UShort0 = X86.Saturate_To_UnsignedInt16(a.SInt0),
					UShort1 = X86.Saturate_To_UnsignedInt16(a.SInt1),
					UShort2 = X86.Saturate_To_UnsignedInt16(a.SInt2),
					UShort3 = X86.Saturate_To_UnsignedInt16(a.SInt3),
					UShort4 = X86.Saturate_To_UnsignedInt16(b.SInt0),
					UShort5 = X86.Saturate_To_UnsignedInt16(b.SInt1),
					UShort6 = X86.Saturate_To_UnsignedInt16(b.SInt2),
					UShort7 = X86.Saturate_To_UnsignedInt16(b.SInt3)
				};
			}

			[DebuggerStepThrough]
			public static v128 cmpeq_epi64(v128 a, v128 b)
			{
				return new v128
				{
					SLong0 = ((a.SLong0 == b.SLong0) ? (-1L) : 0L),
					SLong1 = ((a.SLong1 == b.SLong1) ? (-1L) : 0L)
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi8_epi16(v128 a)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (short)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi8_epi32(v128 a)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi8_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi16_epi32(v128 a)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				short* ptr2 = &a.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi16_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				short* ptr2 = &a.SShort0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepi32_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				int* ptr2 = &a.SInt0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu8_epi16(v128 a)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (short)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu8_epi32(v128 a)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu8_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				byte* ptr2 = &a.Byte0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu16_epi32(v128 a)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (int)ptr2[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu16_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				ushort* ptr2 = &a.UShort0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cvtepu32_epi64(v128 a)
			{
				v128 v = default(v128);
				long* ptr = &v.SLong0;
				uint* ptr2 = &a.UInt0;
				for (int i = 0; i <= 1; i++)
				{
					ptr[i] = (long)((ulong)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 mul_epi32(v128 a, v128 b)
			{
				return new v128
				{
					SLong0 = (long)a.SInt0 * (long)b.SInt0,
					SLong1 = (long)a.SInt2 * (long)b.SInt2
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 mullo_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = ptr2[i] * ptr3[i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public static int testz_si128(v128 a, v128 b)
			{
				if ((a.SLong0 & b.SLong0) != 0L || (a.SLong1 & b.SLong1) != 0L)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int testc_si128(v128 a, v128 b)
			{
				if ((~(a.SLong0 != 0L) & b.SLong0) != 0L || (~(a.SLong1 != 0L) & b.SLong1) != 0L)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int testnzc_si128(v128 a, v128 b)
			{
				int num = (((a.SLong0 & b.SLong0) == 0L && (a.SLong1 & b.SLong1) == 0L) ? 1 : 0);
				int num2 = (((~(a.SLong0 != 0L) & b.SLong0) == 0L && (~(a.SLong1 != 0L) & b.SLong1) == 0L) ? 1 : 0);
				return 1 - (num | num2);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static int test_all_zeros(v128 a, v128 mask)
			{
				return X86.Sse4_1.testz_si128(a, mask);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static int test_mix_ones_zeroes(v128 a, v128 mask)
			{
				return X86.Sse4_1.testnzc_si128(a, mask);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static int test_all_ones(v128 a)
			{
				return X86.Sse4_1.testc_si128(a, X86.Sse2.cmpeq_epi32(a, a));
			}

			private static double RoundDImpl(double d, int roundingMode)
			{
				switch (roundingMode & 7)
				{
				case 0:
					return Math.Round(d);
				case 1:
					return Math.Floor(d);
				case 2:
				{
					double num = Math.Ceiling(d);
					if (num == 0.0 && d < 0.0)
					{
						return new v128(9223372036854775808UL).Double0;
					}
					return num;
				}
				case 3:
					return Math.Truncate(d);
				default:
				{
					X86.MXCSRBits mxcsrbits = X86.MXCSR & X86.MXCSRBits.RoundingControlMask;
					if (mxcsrbits == X86.MXCSRBits.RoundToNearest)
					{
						return Math.Round(d);
					}
					if (mxcsrbits == X86.MXCSRBits.RoundDown)
					{
						return Math.Floor(d);
					}
					if (mxcsrbits != X86.MXCSRBits.RoundUp)
					{
						return Math.Truncate(d);
					}
					return Math.Ceiling(d);
				}
				}
			}

			[DebuggerStepThrough]
			public static v128 round_pd(v128 a, int rounding)
			{
				return new v128
				{
					Double0 = X86.Sse4_1.RoundDImpl(a.Double0, rounding),
					Double1 = X86.Sse4_1.RoundDImpl(a.Double1, rounding)
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 floor_pd(v128 a)
			{
				return X86.Sse4_1.round_pd(a, 1);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 ceil_pd(v128 a)
			{
				return X86.Sse4_1.round_pd(a, 2);
			}

			[DebuggerStepThrough]
			public static v128 round_ps(v128 a, int rounding)
			{
				return new v128
				{
					Float0 = (float)X86.Sse4_1.RoundDImpl((double)a.Float0, rounding),
					Float1 = (float)X86.Sse4_1.RoundDImpl((double)a.Float1, rounding),
					Float2 = (float)X86.Sse4_1.RoundDImpl((double)a.Float2, rounding),
					Float3 = (float)X86.Sse4_1.RoundDImpl((double)a.Float3, rounding)
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 floor_ps(v128 a)
			{
				return X86.Sse4_1.round_ps(a, 1);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 ceil_ps(v128 a)
			{
				return X86.Sse4_1.round_ps(a, 2);
			}

			[DebuggerStepThrough]
			public static v128 round_sd(v128 a, v128 b, int rounding)
			{
				return new v128
				{
					Double0 = X86.Sse4_1.RoundDImpl(b.Double0, rounding),
					Double1 = a.Double1
				};
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 floor_sd(v128 a, v128 b)
			{
				return X86.Sse4_1.round_sd(a, b, 1);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 ceil_sd(v128 a, v128 b)
			{
				return X86.Sse4_1.round_sd(a, b, 2);
			}

			[DebuggerStepThrough]
			public static v128 round_ss(v128 a, v128 b, int rounding)
			{
				v128 v = a;
				v.Float0 = (float)X86.Sse4_1.RoundDImpl((double)b.Float0, rounding);
				return v;
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 floor_ss(v128 a, v128 b)
			{
				return X86.Sse4_1.round_ss(a, b, 1);
			}

			[DebuggerStepThrough]
			[BurstTargetCpu(BurstTargetCpu.X64_SSE4)]
			public static v128 ceil_ss(v128 a, v128 b)
			{
				return X86.Sse4_1.round_ss(a, b, 2);
			}

			[DebuggerStepThrough]
			public unsafe static v128 minpos_epu16(v128 a)
			{
				int num = 0;
				ushort num2 = a.UShort0;
				ushort* ptr = &a.UShort0;
				for (int i = 1; i <= 7; i++)
				{
					if (ptr[i] < num2)
					{
						num = i;
						num2 = ptr[i];
					}
				}
				return new v128
				{
					UShort0 = num2,
					UShort1 = (ushort)num
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 mpsadbw_epu8(v128 a, v128 b, int imm8)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				byte* ptr2 = &a.Byte0 + ((imm8 >> 2) & 1) * 4;
				byte* ptr3 = &b.Byte0 + (imm8 & 3) * 4;
				byte b2 = *ptr3;
				byte b3 = ptr3[1];
				byte b4 = ptr3[2];
				byte b5 = ptr3[3];
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (ushort)(Math.Abs((int)(ptr2[i] - b2)) + Math.Abs((int)(ptr2[i + 1] - b3)) + Math.Abs((int)(ptr2[i + 2] - b4)) + Math.Abs((int)(ptr2[i + 3] - b5)));
				}
				return v;
			}

			[DebuggerStepThrough]
			public static int MK_INSERTPS_NDX(int srcField, int dstField, int zeroMask)
			{
				return (srcField << 6) | (dstField << 4) | zeroMask;
			}
		}

		public static class Sse4_2
		{
			public static bool IsSse42Supported
			{
				get
				{
					return false;
				}
			}

			private unsafe static v128 cmpistrm_emulation<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* a, T* b, int len, int imm8, int allOnes, T allOnesT) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				int num = X86.Sse4_2.ComputeStrCmpIntRes2<T>(a, X86.Sse4_2.ComputeStringLength<T>(a, len), b, X86.Sse4_2.ComputeStringLength<T>(b, len), len, imm8, allOnes);
				return X86.Sse4_2.ComputeStrmOutput<T>(len, imm8, allOnesT, num);
			}

			private unsafe static v128 cmpestrm_emulation<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* a, int alen, T* b, int blen, int len, int imm8, int allOnes, T allOnesT) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				int num = X86.Sse4_2.ComputeStrCmpIntRes2<T>(a, alen, b, blen, len, imm8, allOnes);
				return X86.Sse4_2.ComputeStrmOutput<T>(len, imm8, allOnesT, num);
			}

			private unsafe static v128 ComputeStrmOutput<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int len, int imm8, T allOnesT, int intRes2) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				v128 v = default(v128);
				if ((imm8 & 64) != 0)
				{
					T* ptr = (T*)(&v.Byte0);
					for (int i = 0; i < len; i++)
					{
						if ((intRes2 & (1 << i)) != 0)
						{
							ptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = allOnesT;
						}
						else
						{
							ptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)] = default(T);
						}
					}
				}
				else
				{
					v.SInt0 = intRes2;
				}
				return v;
			}

			private unsafe static int cmpistri_emulation<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* a, T* b, int len, int imm8, int allOnes, T allOnesT) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				int num = X86.Sse4_2.ComputeStrCmpIntRes2<T>(a, X86.Sse4_2.ComputeStringLength<T>(a, len), b, X86.Sse4_2.ComputeStringLength<T>(b, len), len, imm8, allOnes);
				return X86.Sse4_2.ComputeStriOutput(len, imm8, num);
			}

			private unsafe static int cmpestri_emulation<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* a, int alen, T* b, int blen, int len, int imm8, int allOnes, T allOnesT) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				int num = X86.Sse4_2.ComputeStrCmpIntRes2<T>(a, alen, b, blen, len, imm8, allOnes);
				return X86.Sse4_2.ComputeStriOutput(len, imm8, num);
			}

			private static int ComputeStriOutput(int len, int imm8, int intRes2)
			{
				if ((imm8 & 64) == 0)
				{
					for (int i = 0; i < len; i++)
					{
						if ((intRes2 & (1 << i)) != 0)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = len - 1; j >= 0; j--)
					{
						if ((intRes2 & (1 << j)) != 0)
						{
							return j;
						}
					}
				}
				return len;
			}

			private unsafe static int ComputeStringLength<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* ptr, int max) where T : struct, ValueType, IEquatable<T>
			{
				for (int i = 0; i < max; i++)
				{
					if (EqualityComparer<T>.Default.Equals(ptr[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)], default(T)))
					{
						return i;
					}
				}
				return max;
			}

			private unsafe static int ComputeStrCmpIntRes2<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(T* a, int alen, T* b, int blen, int len, int imm8, int allOnes) where T : struct, ValueType, IComparable<T>, IEquatable<T>
			{
				bool flag = false;
				X86.Sse4_2.StrBoolArray strBoolArray = default(X86.Sse4_2.StrBoolArray);
				bool flag2;
				for (int i = 0; i < len; i++)
				{
					T t = a[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
					if (i == alen)
					{
						flag = true;
					}
					flag2 = false;
					for (int j = 0; j < len; j++)
					{
						T t2 = b[(IntPtr)j * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)];
						if (j == blen)
						{
							flag2 = true;
						}
						bool flag3;
						switch ((imm8 >> 2) & 3)
						{
						case 0:
							flag3 = EqualityComparer<T>.Default.Equals(t, t2);
							if (!flag && flag2)
							{
								flag3 = false;
							}
							else if (flag && !flag2)
							{
								flag3 = false;
							}
							else if (flag && flag2)
							{
								flag3 = false;
							}
							break;
						case 1:
							if ((i & 1) == 0)
							{
								flag3 = Comparer<T>.Default.Compare(t2, t) >= 0;
							}
							else
							{
								flag3 = Comparer<T>.Default.Compare(t2, t) <= 0;
							}
							if (!flag && flag2)
							{
								flag3 = false;
							}
							else if (flag && !flag2)
							{
								flag3 = false;
							}
							else if (flag && flag2)
							{
								flag3 = false;
							}
							break;
						case 2:
							flag3 = EqualityComparer<T>.Default.Equals(t, t2);
							if (!flag && flag2)
							{
								flag3 = false;
							}
							else if (flag && !flag2)
							{
								flag3 = false;
							}
							else if (flag && flag2)
							{
								flag3 = true;
							}
							break;
						default:
							flag3 = EqualityComparer<T>.Default.Equals(t, t2);
							if (!flag && flag2)
							{
								flag3 = false;
							}
							else if (flag && !flag2)
							{
								flag3 = true;
							}
							else if (flag && flag2)
							{
								flag3 = true;
							}
							break;
						}
						strBoolArray.SetBit(i, j, flag3);
					}
				}
				int num = 0;
				switch ((imm8 >> 2) & 3)
				{
				case 0:
				{
					for (int i = 0; i < len; i++)
					{
						for (int j = 0; j < len; j++)
						{
							num |= (strBoolArray.GetBit(j, i) ? 1 : 0) << i;
						}
					}
					break;
				}
				case 1:
				{
					for (int i = 0; i < len; i++)
					{
						for (int j = 0; j < len; j += 2)
						{
							num |= ((strBoolArray.GetBit(j, i) && strBoolArray.GetBit(j + 1, i)) ? 1 : 0) << i;
						}
					}
					break;
				}
				case 2:
				{
					for (int i = 0; i < len; i++)
					{
						num |= (strBoolArray.GetBit(i, i) ? 1 : 0) << i;
					}
					break;
				}
				case 3:
				{
					num = allOnes;
					for (int i = 0; i < len; i++)
					{
						int num2 = i;
						for (int j = 0; j < len - i; j++)
						{
							if (!strBoolArray.GetBit(j, num2))
							{
								num &= ~(1 << i);
							}
							num2++;
						}
					}
					break;
				}
				}
				int num3 = 0;
				flag2 = false;
				for (int i = 0; i < len; i++)
				{
					if ((imm8 & 16) != 0)
					{
						if ((imm8 & 32) != 0)
						{
							if (EqualityComparer<T>.Default.Equals(b[(IntPtr)i * (IntPtr)sizeof(T) / (IntPtr)sizeof(T)], default(T)))
							{
								flag2 = true;
							}
							if (flag2)
							{
								num3 |= num & (1 << i);
							}
							else
							{
								num3 |= ~num & (1 << i);
							}
						}
						else
						{
							num3 |= ~num & (1 << i);
						}
					}
					else
					{
						num3 |= num & (1 << i);
					}
				}
				return num3;
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpistrm(v128 a, v128 b, int imm8)
			{
				v128 v;
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						v = X86.Sse4_2.cmpistrm_emulation<byte>(&a.Byte0, &b.Byte0, 16, imm8, 65535, byte.MaxValue);
					}
					else
					{
						v = X86.Sse4_2.cmpistrm_emulation<sbyte>(&a.SByte0, &b.SByte0, 16, imm8, 65535, -1);
					}
				}
				else if ((imm8 & 2) == 0)
				{
					v = X86.Sse4_2.cmpistrm_emulation<ushort>(&a.UShort0, &b.UShort0, 8, imm8, 255, ushort.MaxValue);
				}
				else
				{
					v = X86.Sse4_2.cmpistrm_emulation<short>(&a.SShort0, &b.SShort0, 8, imm8, 255, -1);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static int cmpistri(v128 a, v128 b, int imm8)
			{
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						return X86.Sse4_2.cmpistri_emulation<byte>(&a.Byte0, &b.Byte0, 16, imm8, 65535, byte.MaxValue);
					}
					return X86.Sse4_2.cmpistri_emulation<sbyte>(&a.SByte0, &b.SByte0, 16, imm8, 65535, -1);
				}
				else
				{
					if ((imm8 & 2) == 0)
					{
						return X86.Sse4_2.cmpistri_emulation<ushort>(&a.UShort0, &b.UShort0, 8, imm8, 255, ushort.MaxValue);
					}
					return X86.Sse4_2.cmpistri_emulation<short>(&a.SShort0, &b.SShort0, 8, imm8, 255, -1);
				}
			}

			[DebuggerStepThrough]
			public unsafe static v128 cmpestrm(v128 a, int la, v128 b, int lb, int imm8)
			{
				v128 v;
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						v = X86.Sse4_2.cmpestrm_emulation<byte>(&a.Byte0, la, &b.Byte0, lb, 16, imm8, 65535, byte.MaxValue);
					}
					else
					{
						v = X86.Sse4_2.cmpestrm_emulation<sbyte>(&a.SByte0, la, &b.SByte0, lb, 16, imm8, 65535, -1);
					}
				}
				else if ((imm8 & 2) == 0)
				{
					v = X86.Sse4_2.cmpestrm_emulation<ushort>(&a.UShort0, la, &b.UShort0, lb, 8, imm8, 255, ushort.MaxValue);
				}
				else
				{
					v = X86.Sse4_2.cmpestrm_emulation<short>(&a.SShort0, la, &b.SShort0, lb, 8, imm8, 255, -1);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static int cmpestri(v128 a, int la, v128 b, int lb, int imm8)
			{
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						return X86.Sse4_2.cmpestri_emulation<byte>(&a.Byte0, la, &b.Byte0, lb, 16, imm8, 65535, byte.MaxValue);
					}
					return X86.Sse4_2.cmpestri_emulation<sbyte>(&a.SByte0, la, &b.SByte0, lb, 16, imm8, 65535, -1);
				}
				else
				{
					if ((imm8 & 2) == 0)
					{
						return X86.Sse4_2.cmpestri_emulation<ushort>(&a.UShort0, la, &b.UShort0, lb, 8, imm8, 255, ushort.MaxValue);
					}
					return X86.Sse4_2.cmpestri_emulation<short>(&a.SShort0, la, &b.SShort0, lb, 8, imm8, 255, -1);
				}
			}

			[DebuggerStepThrough]
			public unsafe static int cmpistrz(v128 a, v128 b, int imm8)
			{
				if ((imm8 & 1) == 0)
				{
					if (X86.Sse4_2.ComputeStringLength<byte>(&b.Byte0, 16) >= 16)
					{
						return 0;
					}
					return 1;
				}
				else
				{
					if (X86.Sse4_2.ComputeStringLength<ushort>(&b.UShort0, 8) >= 8)
					{
						return 0;
					}
					return 1;
				}
			}

			[DebuggerStepThrough]
			public static int cmpistrc(v128 a, v128 b, int imm8)
			{
				v128 v = X86.Sse4_2.cmpistrm(a, b, imm8);
				if (v.SInt0 != 0 || v.SInt1 != 0 || v.SInt2 != 0 || v.SInt3 != 0)
				{
					return 1;
				}
				return 0;
			}

			[DebuggerStepThrough]
			public unsafe static int cmpistrs(v128 a, v128 b, int imm8)
			{
				if ((imm8 & 1) == 0)
				{
					if (X86.Sse4_2.ComputeStringLength<byte>(&a.Byte0, 16) >= 16)
					{
						return 0;
					}
					return 1;
				}
				else
				{
					if (X86.Sse4_2.ComputeStringLength<ushort>(&a.UShort0, 8) >= 8)
					{
						return 0;
					}
					return 1;
				}
			}

			[DebuggerStepThrough]
			public unsafe static int cmpistro(v128 a, v128 b, int imm8)
			{
				int num3;
				if ((imm8 & 1) == 0)
				{
					int num = X86.Sse4_2.ComputeStringLength<byte>(&a.Byte0, 16);
					int num2 = X86.Sse4_2.ComputeStringLength<byte>(&b.Byte0, 16);
					if ((imm8 & 2) == 0)
					{
						num3 = X86.Sse4_2.ComputeStrCmpIntRes2<byte>(&a.Byte0, num, &b.Byte0, num2, 16, imm8, 65535);
					}
					else
					{
						num3 = X86.Sse4_2.ComputeStrCmpIntRes2<sbyte>(&a.SByte0, num, &b.SByte0, num2, 16, imm8, 65535);
					}
				}
				else
				{
					int num4 = X86.Sse4_2.ComputeStringLength<ushort>(&a.UShort0, 8);
					int num5 = X86.Sse4_2.ComputeStringLength<ushort>(&b.UShort0, 8);
					if ((imm8 & 2) == 0)
					{
						num3 = X86.Sse4_2.ComputeStrCmpIntRes2<ushort>(&a.UShort0, num4, &b.UShort0, num5, 8, imm8, 255);
					}
					else
					{
						num3 = X86.Sse4_2.ComputeStrCmpIntRes2<short>(&a.SShort0, num4, &b.SShort0, num5, 8, imm8, 255);
					}
				}
				return num3 & 1;
			}

			[DebuggerStepThrough]
			public static int cmpistra(v128 a, v128 b, int imm8)
			{
				return ~X86.Sse4_2.cmpistrc(a, b, imm8) & ~X86.Sse4_2.cmpistrz(a, b, imm8) & 1;
			}

			[DebuggerStepThrough]
			public static int cmpestrz(v128 a, int la, v128 b, int lb, int imm8)
			{
				int num = (((imm8 & 1) == 1) ? 16 : 8);
				int num2 = 128 / num - 1;
				if (lb > num2)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int cmpestrc(v128 a, int la, v128 b, int lb, int imm8)
			{
				int num;
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						num = X86.Sse4_2.ComputeStrCmpIntRes2<byte>(&a.Byte0, la, &b.Byte0, lb, 16, imm8, 65535);
					}
					else
					{
						num = X86.Sse4_2.ComputeStrCmpIntRes2<sbyte>(&a.SByte0, la, &b.SByte0, lb, 16, imm8, 65535);
					}
				}
				else if ((imm8 & 2) == 0)
				{
					num = X86.Sse4_2.ComputeStrCmpIntRes2<ushort>(&a.UShort0, la, &b.UShort0, lb, 8, imm8, 255);
				}
				else
				{
					num = X86.Sse4_2.ComputeStrCmpIntRes2<short>(&a.SShort0, la, &b.SShort0, lb, 8, imm8, 255);
				}
				if (num == 0)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public static int cmpestrs(v128 a, int la, v128 b, int lb, int imm8)
			{
				int num = (((imm8 & 1) == 1) ? 16 : 8);
				int num2 = 128 / num - 1;
				if (la > num2)
				{
					return 0;
				}
				return 1;
			}

			[DebuggerStepThrough]
			public unsafe static int cmpestro(v128 a, int la, v128 b, int lb, int imm8)
			{
				int num;
				if ((imm8 & 1) == 0)
				{
					if ((imm8 & 2) == 0)
					{
						num = X86.Sse4_2.ComputeStrCmpIntRes2<byte>(&a.Byte0, la, &b.Byte0, lb, 16, imm8, 65535);
					}
					else
					{
						num = X86.Sse4_2.ComputeStrCmpIntRes2<sbyte>(&a.SByte0, la, &b.SByte0, lb, 16, imm8, 65535);
					}
				}
				else if ((imm8 & 2) == 0)
				{
					num = X86.Sse4_2.ComputeStrCmpIntRes2<ushort>(&a.UShort0, la, &b.UShort0, lb, 8, imm8, 255);
				}
				else
				{
					num = X86.Sse4_2.ComputeStrCmpIntRes2<short>(&a.SShort0, la, &b.SShort0, lb, 8, imm8, 255);
				}
				return num & 1;
			}

			[DebuggerStepThrough]
			public static int cmpestra(v128 a, int la, v128 b, int lb, int imm8)
			{
				return ~X86.Sse4_2.cmpestrc(a, la, b, lb, imm8) & ~X86.Sse4_2.cmpestrz(a, la, b, lb, imm8) & 1;
			}

			[DebuggerStepThrough]
			public static v128 cmpgt_epi64(v128 val1, v128 val2)
			{
				return new v128
				{
					SLong0 = ((val1.SLong0 > val2.SLong0) ? (-1L) : 0L),
					SLong1 = ((val1.SLong1 > val2.SLong1) ? (-1L) : 0L)
				};
			}

			[DebuggerStepThrough]
			public static uint crc32_u32(uint crc, uint v)
			{
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				v >>= 8;
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				v >>= 8;
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				v >>= 8;
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				return crc;
			}

			[DebuggerStepThrough]
			public static uint crc32_u8(uint crc, byte v)
			{
				crc = (crc >> 8) ^ X86.Sse4_2.crctab[(int)((crc ^ (uint)v) & 255U)];
				return crc;
			}

			[DebuggerStepThrough]
			public static uint crc32_u16(uint crc, ushort v)
			{
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				v = (ushort)(v >> 8);
				crc = X86.Sse4_2.crc32_u8(crc, (byte)v);
				return crc;
			}

			[DebuggerStepThrough]
			[Obsolete("Use the ulong version of this intrinsic instead.")]
			public static ulong crc32_u64(ulong crc_ul, long v)
			{
				return X86.Sse4_2.crc32_u64(crc_ul, (ulong)v);
			}

			[DebuggerStepThrough]
			public static ulong crc32_u64(ulong crc_ul, ulong v)
			{
				uint num = X86.Sse4_2.crc32_u8((uint)crc_ul, (byte)v);
				v >>= 8;
				uint num2 = X86.Sse4_2.crc32_u8(num, (byte)v);
				v >>= 8;
				uint num3 = X86.Sse4_2.crc32_u8(num2, (byte)v);
				v >>= 8;
				uint num4 = X86.Sse4_2.crc32_u8(num3, (byte)v);
				v >>= 8;
				uint num5 = X86.Sse4_2.crc32_u8(num4, (byte)v);
				v >>= 8;
				uint num6 = X86.Sse4_2.crc32_u8(num5, (byte)v);
				v >>= 8;
				uint num7 = X86.Sse4_2.crc32_u8(num6, (byte)v);
				v >>= 8;
				return (ulong)X86.Sse4_2.crc32_u8(num7, (byte)v);
			}

			private static readonly uint[] crctab = new uint[]
			{
				0U, 4067132163U, 3778769143U, 324072436U, 3348797215U, 904991772U, 648144872U, 3570033899U, 2329499855U, 2024987596U,
				1809983544U, 2575936315U, 1296289744U, 3207089363U, 2893594407U, 1578318884U, 274646895U, 3795141740U, 4049975192U, 51262619U,
				3619967088U, 632279923U, 922689671U, 3298075524U, 2592579488U, 1760304291U, 2075979607U, 2312596564U, 1562183871U, 2943781820U,
				3156637768U, 1313733451U, 549293790U, 3537243613U, 3246849577U, 871202090U, 3878099393U, 357341890U, 102525238U, 4101499445U,
				2858735121U, 1477399826U, 1264559846U, 3107202533U, 1845379342U, 2677391885U, 2361733625U, 2125378298U, 820201905U, 3263744690U,
				3520608582U, 598981189U, 4151959214U, 85089709U, 373468761U, 3827903834U, 3124367742U, 1213305469U, 1526817161U, 2842354314U,
				2107672161U, 2412447074U, 2627466902U, 1861252501U, 1098587580U, 3004210879U, 2688576843U, 1378610760U, 2262928035U, 1955203488U,
				1742404180U, 2511436119U, 3416409459U, 969524848U, 714683780U, 3639785095U, 205050476U, 4266873199U, 3976438427U, 526918040U,
				1361435347U, 2739821008U, 2954799652U, 1114974503U, 2529119692U, 1691668175U, 2005155131U, 2247081528U, 3690758684U, 697762079U,
				986182379U, 3366744552U, 476452099U, 3993867776U, 4250756596U, 255256311U, 1640403810U, 2477592673U, 2164122517U, 1922457750U,
				2791048317U, 1412925310U, 1197962378U, 3037525897U, 3944729517U, 427051182U, 170179418U, 4165941337U, 746937522U, 3740196785U,
				3451792453U, 1070968646U, 1905808397U, 2213795598U, 2426610938U, 1657317369U, 3053634322U, 1147748369U, 1463399397U, 2773627110U,
				4215344322U, 153784257U, 444234805U, 3893493558U, 1021025245U, 3467647198U, 3722505002U, 797665321U, 2197175160U, 1889384571U,
				1674398607U, 2443626636U, 1164749927U, 3070701412U, 2757221520U, 1446797203U, 137323447U, 4198817972U, 3910406976U, 461344835U,
				3484808360U, 1037989803U, 781091935U, 3705997148U, 2460548119U, 1623424788U, 1939049696U, 2180517859U, 1429367560U, 2807687179U,
				3020495871U, 1180866812U, 410100952U, 3927582683U, 4182430767U, 186734380U, 3756733383U, 763408580U, 1053836080U, 3434856499U,
				2722870694U, 1344288421U, 1131464017U, 2971354706U, 1708204729U, 2545590714U, 2229949006U, 1988219213U, 680717673U, 3673779818U,
				3383336350U, 1002577565U, 4010310262U, 493091189U, 238226049U, 4233660802U, 2987750089U, 1082061258U, 1395524158U, 2705686845U,
				1972364758U, 2279892693U, 2494862625U, 1725896226U, 952904198U, 3399985413U, 3656866545U, 731699698U, 4283874585U, 222117402U,
				510512622U, 3959836397U, 3280807620U, 837199303U, 582374963U, 3504198960U, 68661723U, 4135334616U, 3844915500U, 390545967U,
				1230274059U, 3141532936U, 2825850620U, 1510247935U, 2395924756U, 2091215383U, 1878366691U, 2644384480U, 3553878443U, 565732008U,
				854102364U, 3229815391U, 340358836U, 3861050807U, 4117890627U, 119113024U, 1493875044U, 2875275879U, 3090270611U, 1247431312U,
				2660249211U, 1828433272U, 2141937292U, 2378227087U, 3811616794U, 291187481U, 34330861U, 4032846830U, 615137029U, 3603020806U,
				3314634738U, 939183345U, 1776939221U, 2609017814U, 2295496738U, 2058945313U, 2926798794U, 1545135305U, 1330124605U, 3173225534U,
				4084100981U, 17165430U, 307568514U, 3762199681U, 888469610U, 3332340585U, 3587147933U, 665062302U, 2042050490U, 2346497209U,
				2559330125U, 1793573966U, 3190661285U, 1279665062U, 1595330642U, 2910671697U
			};

			[Flags]
			public enum SIDD
			{
				UBYTE_OPS = 0,
				UWORD_OPS = 1,
				SBYTE_OPS = 2,
				SWORD_OPS = 3,
				CMP_EQUAL_ANY = 0,
				CMP_RANGES = 4,
				CMP_EQUAL_EACH = 8,
				CMP_EQUAL_ORDERED = 12,
				POSITIVE_POLARITY = 0,
				NEGATIVE_POLARITY = 16,
				MASKED_POSITIVE_POLARITY = 32,
				MASKED_NEGATIVE_POLARITY = 48,
				LEAST_SIGNIFICANT = 0,
				MOST_SIGNIFICANT = 64,
				BIT_MASK = 0,
				UNIT_MASK = 64
			}

			private struct StrBoolArray
			{
				public unsafe void SetBit(int aindex, int bindex, bool val)
				{
					fixed (ushort* ptr = &this.Bits.FixedElementField)
					{
						ushort* ptr2 = ptr;
						if (val)
						{
							ushort* ptr3 = ptr2 + aindex;
							*ptr3 |= (ushort)(1 << bindex);
						}
						else
						{
							ushort* ptr4 = ptr2 + aindex;
							*ptr4 &= (ushort)(~(ushort)(1 << bindex));
						}
					}
				}

				public unsafe bool GetBit(int aindex, int bindex)
				{
					fixed (ushort* ptr = &this.Bits.FixedElementField)
					{
						return ((int)ptr[aindex] & (1 << bindex)) != 0;
					}
				}

				[FixedBuffer(typeof(ushort), 16)]
				public X86.Sse4_2.StrBoolArray.<Bits>e__FixedBuffer Bits;

				[CompilerGenerated]
				[UnsafeValueType]
				[StructLayout(LayoutKind.Sequential, Size = 32)]
				public struct <Bits>e__FixedBuffer
				{
					public ushort FixedElementField;
				}
			}
		}

		public static class Ssse3
		{
			public static bool IsSsse3Supported
			{
				get
				{
					return false;
				}
			}

			[DebuggerStepThrough]
			public unsafe static v128 abs_epi8(v128 a)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				sbyte* ptr2 = &a.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					ptr[i] = (byte)Math.Abs((int)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 abs_epi16(v128 a)
			{
				v128 v = default(v128);
				ushort* ptr = &v.UShort0;
				short* ptr2 = &a.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					ptr[i] = (ushort)Math.Abs((int)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 abs_epi32(v128 a)
			{
				v128 v = default(v128);
				uint* ptr = &v.UInt0;
				int* ptr2 = &a.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = (uint)Math.Abs((long)ptr2[i]);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 shuffle_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0;
				byte* ptr3 = &b.Byte0;
				for (int i = 0; i <= 15; i++)
				{
					if ((ptr3[i] & 128) != 0)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = ptr2[ptr3[i] & 15];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 alignr_epi8(v128 a, v128 b, int count)
			{
				v128 v = default(v128);
				byte* ptr = &v.Byte0;
				byte* ptr2 = &a.Byte0 + count;
				byte* ptr3 = &b.Byte0;
				int i;
				for (i = 0; i < 16 - count; i++)
				{
					*(ptr++) = *(ptr2++);
				}
				while (i < 16)
				{
					*(ptr++) = *(ptr3++);
					i++;
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 hadd_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = ptr2[2 * i + 1] + ptr2[2 * i];
					ptr[i + 4] = ptr3[2 * i + 1] + ptr3[2 * i];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 hadds_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = X86.Saturate_To_Int16((int)(ptr2[2 * i + 1] + ptr2[2 * i]));
					ptr[i + 4] = X86.Saturate_To_Int16((int)(ptr3[2 * i + 1] + ptr3[2 * i]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 hadd_epi32(v128 a, v128 b)
			{
				return new v128
				{
					SInt0 = a.SInt1 + a.SInt0,
					SInt1 = a.SInt3 + a.SInt2,
					SInt2 = b.SInt1 + b.SInt0,
					SInt3 = b.SInt3 + b.SInt2
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 hsub_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = ptr2[2 * i] - ptr2[2 * i + 1];
					ptr[i + 4] = ptr3[2 * i] - ptr3[2 * i + 1];
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 hsubs_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 3; i++)
				{
					ptr[i] = X86.Saturate_To_Int16((int)(ptr2[2 * i] - ptr2[2 * i + 1]));
					ptr[i + 4] = X86.Saturate_To_Int16((int)(ptr3[2 * i] - ptr3[2 * i + 1]));
				}
				return v;
			}

			[DebuggerStepThrough]
			public static v128 hsub_epi32(v128 a, v128 b)
			{
				return new v128
				{
					SInt0 = a.SInt0 - a.SInt1,
					SInt1 = a.SInt2 - a.SInt3,
					SInt2 = b.SInt0 - b.SInt1,
					SInt3 = b.SInt2 - b.SInt3
				};
			}

			[DebuggerStepThrough]
			public unsafe static v128 maddubs_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				byte* ptr2 = &a.Byte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 7; i++)
				{
					int num = (int)(ptr2[2 * i + 1] * (byte)ptr3[2 * i + 1] + ptr2[2 * i] * (byte)ptr3[2 * i]);
					ptr[i] = X86.Saturate_To_Int16(num);
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 mulhrs_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					int num = (int)(ptr2[i] * ptr3[i]);
					num >>= 14;
					num++;
					num >>= 1;
					ptr[i] = (short)num;
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sign_epi8(v128 a, v128 b)
			{
				v128 v = default(v128);
				sbyte* ptr = &v.SByte0;
				sbyte* ptr2 = &a.SByte0;
				sbyte* ptr3 = &b.SByte0;
				for (int i = 0; i <= 15; i++)
				{
					if (ptr3[i] < 0)
					{
						ptr[i] = -ptr2[i];
					}
					else if (ptr3[i] == 0)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sign_epi16(v128 a, v128 b)
			{
				v128 v = default(v128);
				short* ptr = &v.SShort0;
				short* ptr2 = &a.SShort0;
				short* ptr3 = &b.SShort0;
				for (int i = 0; i <= 7; i++)
				{
					if (ptr3[i] < 0)
					{
						ptr[i] = -ptr2[i];
					}
					else if (ptr3[i] == 0)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}

			[DebuggerStepThrough]
			public unsafe static v128 sign_epi32(v128 a, v128 b)
			{
				v128 v = default(v128);
				int* ptr = &v.SInt0;
				int* ptr2 = &a.SInt0;
				int* ptr3 = &b.SInt0;
				for (int i = 0; i <= 3; i++)
				{
					if (ptr3[i] < 0)
					{
						ptr[i] = -ptr2[i];
					}
					else if (ptr3[i] == 0)
					{
						ptr[i] = 0;
					}
					else
					{
						ptr[i] = ptr2[i];
					}
				}
				return v;
			}
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void DoSetCSRTrampoline_00000129$PostfixBurstDelegate(int bits);

		internal static class DoSetCSRTrampoline_00000129$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (X86.DoSetCSRTrampoline_00000129$BurstDirectCall.Pointer == 0)
				{
					X86.DoSetCSRTrampoline_00000129$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<X86.DoSetCSRTrampoline_00000129$PostfixBurstDelegate>(new X86.DoSetCSRTrampoline_00000129$PostfixBurstDelegate(X86.DoSetCSRTrampoline)).Value;
				}
				A_0 = X86.DoSetCSRTrampoline_00000129$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				X86.DoSetCSRTrampoline_00000129$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public static void Invoke(int bits)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = X86.DoSetCSRTrampoline_00000129$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						calli(System.Void(System.Int32), bits, functionPointer);
						return;
					}
				}
				X86.DoSetCSRTrampoline$BurstManaged(bits);
			}

			private static IntPtr Pointer;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate int DoGetCSRTrampoline_0000012A$PostfixBurstDelegate();

		internal static class DoGetCSRTrampoline_0000012A$BurstDirectCall
		{
			[BurstDiscard]
			private static void GetFunctionPointerDiscard(ref IntPtr A_0)
			{
				if (X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.Pointer == 0)
				{
					X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<X86.DoGetCSRTrampoline_0000012A$PostfixBurstDelegate>(new X86.DoGetCSRTrampoline_0000012A$PostfixBurstDelegate(X86.DoGetCSRTrampoline)).Value;
				}
				A_0 = X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				IntPtr intPtr = (IntPtr)0;
				X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
				return intPtr;
			}

			public static int Invoke()
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = X86.DoGetCSRTrampoline_0000012A$BurstDirectCall.GetFunctionPointer();
					if (functionPointer != 0)
					{
						return calli(System.Int32(), functionPointer);
					}
				}
				return X86.DoGetCSRTrampoline$BurstManaged();
			}

			private static IntPtr Pointer;
		}
	}
}
