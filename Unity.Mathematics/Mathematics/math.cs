using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Unity.Mathematics
{
	[Il2CppEagerStaticClassConstruction]
	public static class math
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 bool2(bool x, bool y)
		{
			return new bool2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 bool2(bool2 xy)
		{
			return new bool2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 bool2(bool v)
		{
			return new bool2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool2 v)
		{
			return math.csum(math.select(math.uint2(2426570171U, 1561977301U), math.uint2(4205774813U, 1650214333U), v));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(bool2 v)
		{
			return math.select(math.uint2(3388112843U, 1831150513U), math.uint2(1848374953U, 3430200247U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool shuffle(bool2 left, bool2 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 shuffle(bool2 left, bool2 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.bool2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 shuffle(bool2 left, bool2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.bool3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 shuffle(bool2 left, bool2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.bool4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool select_shuffle_component(bool2 a, bool2 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 bool2x2(bool2 c0, bool2 c1)
		{
			return new bool2x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 bool2x2(bool m00, bool m01, bool m10, bool m11)
		{
			return new bool2x2(m00, m01, m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 bool2x2(bool v)
		{
			return new bool2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x2 transpose(bool2x2 v)
		{
			return math.bool2x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool2x2 v)
		{
			return math.csum(math.select(math.uint2(2062756937U, 2920485769U), math.uint2(1562056283U, 2265541847U), v.c0) + math.select(math.uint2(1283419601U, 1210229737U), math.uint2(2864955997U, 3525118277U), v.c1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(bool2x2 v)
		{
			return math.select(math.uint2(2298260269U, 1632478733U), math.uint2(1537393931U, 2353355467U), v.c0) + math.select(math.uint2(3441847433U, 4052036147U), math.uint2(2011389559U, 2252224297U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 bool2x3(bool2 c0, bool2 c1, bool2 c2)
		{
			return new bool2x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 bool2x3(bool m00, bool m01, bool m02, bool m10, bool m11, bool m12)
		{
			return new bool2x3(m00, m01, m02, m10, m11, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 bool2x3(bool v)
		{
			return new bool2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 transpose(bool2x3 v)
		{
			return math.bool3x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool2x3 v)
		{
			return math.csum(math.select(math.uint2(2078515003U, 4206465343U), math.uint2(3025146473U, 3763046909U), v.c0) + math.select(math.uint2(3678265601U, 2070747979U), math.uint2(1480171127U, 1588341193U), v.c1) + math.select(math.uint2(4234155257U, 1811310911U), math.uint2(2635799963U, 4165137857U), v.c2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(bool2x3 v)
		{
			return math.select(math.uint2(2759770933U, 2759319383U), math.uint2(3299952959U, 3121178323U), v.c0) + math.select(math.uint2(2948522579U, 1531026433U), math.uint2(1365086453U, 3969870067U), v.c1) + math.select(math.uint2(4192899797U, 3271228601U), math.uint2(1634639009U, 3318036811U), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 bool2x4(bool2 c0, bool2 c1, bool2 c2, bool2 c3)
		{
			return new bool2x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 bool2x4(bool m00, bool m01, bool m02, bool m03, bool m10, bool m11, bool m12, bool m13)
		{
			return new bool2x4(m00, m01, m02, m03, m10, m11, m12, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 bool2x4(bool v)
		{
			return new bool2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 transpose(bool2x4 v)
		{
			return math.bool4x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y, v.c3.x, v.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool2x4 v)
		{
			return math.csum(math.select(math.uint2(1168253063U, 4228926523U), math.uint2(1610574617U, 1584185147U), v.c0) + math.select(math.uint2(3041325733U, 3150930919U), math.uint2(3309258581U, 1770373673U), v.c1) + math.select(math.uint2(3778261171U, 3286279097U), math.uint2(4264629071U, 1898591447U), v.c2) + math.select(math.uint2(2641864091U, 1229113913U), math.uint2(3020867117U, 1449055807U), v.c3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(bool2x4 v)
		{
			return math.select(math.uint2(2479033387U, 3702457169U), math.uint2(1845824257U, 1963973621U), v.c0) + math.select(math.uint2(2134758553U, 1391111867U), math.uint2(1167706003U, 2209736489U), v.c1) + math.select(math.uint2(3261535807U, 1740411209U), math.uint2(2910609089U, 2183822701U), v.c2) + math.select(math.uint2(3029516053U, 3547472099U), math.uint2(2057487037U, 3781937309U), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 bool3(bool x, bool y, bool z)
		{
			return new bool3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 bool3(bool x, bool2 yz)
		{
			return new bool3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 bool3(bool2 xy, bool z)
		{
			return new bool3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 bool3(bool3 xyz)
		{
			return new bool3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 bool3(bool v)
		{
			return new bool3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool3 v)
		{
			return math.csum(math.select(math.uint3(2716413241U, 1166264321U, 2503385333U), math.uint3(2944493077U, 2599999021U, 3814721321U), v));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(bool3 v)
		{
			return math.select(math.uint3(1595355149U, 1728931849U, 2062756937U), math.uint3(2920485769U, 1562056283U, 2265541847U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool shuffle(bool3 left, bool3 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 shuffle(bool3 left, bool3 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.bool2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 shuffle(bool3 left, bool3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.bool3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 shuffle(bool3 left, bool3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.bool4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool select_shuffle_component(bool3 a, bool3 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 bool3x2(bool3 c0, bool3 c1)
		{
			return new bool3x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 bool3x2(bool m00, bool m01, bool m10, bool m11, bool m20, bool m21)
		{
			return new bool3x2(m00, m01, m10, m11, m20, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x2 bool3x2(bool v)
		{
			return new bool3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x3 transpose(bool3x2 v)
		{
			return math.bool2x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool3x2 v)
		{
			return math.csum(math.select(math.uint3(2627668003U, 1520214331U, 2949502447U), math.uint3(2827819133U, 3480140317U, 2642994593U), v.c0) + math.select(math.uint3(3940484981U, 1954192763U, 1091696537U), math.uint3(3052428017U, 4253034763U, 2338696631U), v.c1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(bool3x2 v)
		{
			return math.select(math.uint3(3757372771U, 1885959949U, 3508684087U), math.uint3(3919501043U, 1209161033U, 4007793211U), v.c0) + math.select(math.uint3(3819806693U, 3458005183U, 2078515003U), math.uint3(4206465343U, 3025146473U, 3763046909U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 bool3x3(bool3 c0, bool3 c1, bool3 c2)
		{
			return new bool3x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 bool3x3(bool m00, bool m01, bool m02, bool m10, bool m11, bool m12, bool m20, bool m21, bool m22)
		{
			return new bool3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 bool3x3(bool v)
		{
			return new bool3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x3 transpose(bool3x3 v)
		{
			return math.bool3x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool3x3 v)
		{
			return math.csum(math.select(math.uint3(3881277847U, 4017968839U, 1727237899U), math.uint3(1648514723U, 1385344481U, 3538260197U), v.c0) + math.select(math.uint3(4066109527U, 2613148903U, 3367528529U), math.uint3(1678332449U, 2918459647U, 2744611081U), v.c1) + math.select(math.uint3(1952372791U, 2631698677U, 4200781601U), math.uint3(2119021007U, 1760485621U, 3157985881U), v.c2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(bool3x3 v)
		{
			return math.select(math.uint3(2171534173U, 2723054263U, 1168253063U), math.uint3(4228926523U, 1610574617U, 1584185147U), v.c0) + math.select(math.uint3(3041325733U, 3150930919U, 3309258581U), math.uint3(1770373673U, 3778261171U, 3286279097U), v.c1) + math.select(math.uint3(4264629071U, 1898591447U, 2641864091U), math.uint3(1229113913U, 3020867117U, 1449055807U), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 bool3x4(bool3 c0, bool3 c1, bool3 c2, bool3 c3)
		{
			return new bool3x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 bool3x4(bool m00, bool m01, bool m02, bool m03, bool m10, bool m11, bool m12, bool m13, bool m20, bool m21, bool m22, bool m23)
		{
			return new bool3x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 bool3x4(bool v)
		{
			return new bool3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 transpose(bool3x4 v)
		{
			return math.bool4x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z, v.c3.x, v.c3.y, v.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool3x4 v)
		{
			return math.csum(math.select(math.uint3(2209710647U, 2201894441U, 2849577407U), math.uint3(3287031191U, 3098675399U, 1564399943U), v.c0) + math.select(math.uint3(1148435377U, 3416333663U, 1750611407U), math.uint3(3285396193U, 3110507567U, 4271396531U), v.c1) + math.select(math.uint3(4198118021U, 2908068253U, 3705492289U), math.uint3(2497566569U, 2716413241U, 1166264321U), v.c2) + math.select(math.uint3(2503385333U, 2944493077U, 2599999021U), math.uint3(3814721321U, 1595355149U, 1728931849U), v.c3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(bool3x4 v)
		{
			return math.select(math.uint3(2062756937U, 2920485769U, 1562056283U), math.uint3(2265541847U, 1283419601U, 1210229737U), v.c0) + math.select(math.uint3(2864955997U, 3525118277U, 2298260269U), math.uint3(1632478733U, 1537393931U, 2353355467U), v.c1) + math.select(math.uint3(3441847433U, 4052036147U, 2011389559U), math.uint3(2252224297U, 3784421429U, 1750626223U), v.c2) + math.select(math.uint3(3571447507U, 3412283213U, 2601761069U), math.uint3(1254033427U, 2248573027U, 3612677113U), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool x, bool y, bool z, bool w)
		{
			return new bool4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool x, bool y, bool2 zw)
		{
			return new bool4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool x, bool2 yz, bool w)
		{
			return new bool4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool x, bool3 yzw)
		{
			return new bool4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool2 xy, bool z, bool w)
		{
			return new bool4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool2 xy, bool2 zw)
		{
			return new bool4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool3 xyz, bool w)
		{
			return new bool4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool4 xyzw)
		{
			return new bool4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 bool4(bool v)
		{
			return new bool4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool4 v)
		{
			return math.csum(math.select(math.uint4(1610574617U, 1584185147U, 3041325733U, 3150930919U), math.uint4(3309258581U, 1770373673U, 3778261171U, 3286279097U), v));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(bool4 v)
		{
			return math.select(math.uint4(4264629071U, 1898591447U, 2641864091U, 1229113913U), math.uint4(3020867117U, 1449055807U, 2479033387U, 3702457169U), v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool shuffle(bool4 left, bool4 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 shuffle(bool4 left, bool4 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.bool2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 shuffle(bool4 left, bool4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.bool3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 shuffle(bool4 left, bool4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.bool4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool select_shuffle_component(bool4 a, bool4 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.LeftW:
				return a.w;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			case math.ShuffleComponent.RightW:
				return b.w;
			default:
				throw new ArgumentException("Invalid shuffle component: " + component.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 bool4x2(bool4 c0, bool4 c1)
		{
			return new bool4x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 bool4x2(bool m00, bool m01, bool m10, bool m11, bool m20, bool m21, bool m30, bool m31)
		{
			return new bool4x2(m00, m01, m10, m11, m20, m21, m30, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x2 bool4x2(bool v)
		{
			return new bool4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2x4 transpose(bool4x2 v)
		{
			return math.bool2x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool4x2 v)
		{
			return math.csum(math.select(math.uint4(3516359879U, 3050356579U, 4178586719U, 2558655391U), math.uint4(1453413133U, 2152428077U, 1938706661U, 1338588197U), v.c0) + math.select(math.uint4(3439609253U, 3535343003U, 3546061613U, 2702024231U), math.uint4(1452124841U, 1966089551U, 2668168249U, 1587512777U), v.c1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(bool4x2 v)
		{
			return math.select(math.uint4(2353831999U, 3101256173U, 2891822459U, 2837054189U), math.uint4(3016004371U, 4097481403U, 2229788699U, 2382715877U), v.c0) + math.select(math.uint4(1851936439U, 1938025801U, 3712598587U, 3956330501U), math.uint4(2437373431U, 1441286183U, 2426570171U, 1561977301U), v.c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 bool4x3(bool4 c0, bool4 c1, bool4 c2)
		{
			return new bool4x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 bool4x3(bool m00, bool m01, bool m02, bool m10, bool m11, bool m12, bool m20, bool m21, bool m22, bool m30, bool m31, bool m32)
		{
			return new bool4x3(m00, m01, m02, m10, m11, m12, m20, m21, m22, m30, m31, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x3 bool4x3(bool v)
		{
			return new bool4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3x4 transpose(bool4x3 v)
		{
			return math.bool3x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool4x3 v)
		{
			return math.csum(math.select(math.uint4(3940484981U, 1954192763U, 1091696537U, 3052428017U), math.uint4(4253034763U, 2338696631U, 3757372771U, 1885959949U), v.c0) + math.select(math.uint4(3508684087U, 3919501043U, 1209161033U, 4007793211U), math.uint4(3819806693U, 3458005183U, 2078515003U, 4206465343U), v.c1) + math.select(math.uint4(3025146473U, 3763046909U, 3678265601U, 2070747979U), math.uint4(1480171127U, 1588341193U, 4234155257U, 1811310911U), v.c2));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(bool4x3 v)
		{
			return math.select(math.uint4(2635799963U, 4165137857U, 2759770933U, 2759319383U), math.uint4(3299952959U, 3121178323U, 2948522579U, 1531026433U), v.c0) + math.select(math.uint4(1365086453U, 3969870067U, 4192899797U, 3271228601U), math.uint4(1634639009U, 3318036811U, 3404170631U, 2048213449U), v.c1) + math.select(math.uint4(4164671783U, 1780759499U, 1352369353U, 2446407751U), math.uint4(1391928079U, 3475533443U, 3777095341U, 3385463369U), v.c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 bool4x4(bool4 c0, bool4 c1, bool4 c2, bool4 c3)
		{
			return new bool4x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 bool4x4(bool m00, bool m01, bool m02, bool m03, bool m10, bool m11, bool m12, bool m13, bool m20, bool m21, bool m22, bool m23, bool m30, bool m31, bool m32, bool m33)
		{
			return new bool4x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 bool4x4(bool v)
		{
			return new bool4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4x4 transpose(bool4x4 v)
		{
			return math.bool4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(bool4x4 v)
		{
			return math.csum(math.select(math.uint4(3516359879U, 3050356579U, 4178586719U, 2558655391U), math.uint4(1453413133U, 2152428077U, 1938706661U, 1338588197U), v.c0) + math.select(math.uint4(3439609253U, 3535343003U, 3546061613U, 2702024231U), math.uint4(1452124841U, 1966089551U, 2668168249U, 1587512777U), v.c1) + math.select(math.uint4(2353831999U, 3101256173U, 2891822459U, 2837054189U), math.uint4(3016004371U, 4097481403U, 2229788699U, 2382715877U), v.c2) + math.select(math.uint4(1851936439U, 1938025801U, 3712598587U, 3956330501U), math.uint4(2437373431U, 1441286183U, 2426570171U, 1561977301U), v.c3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(bool4x4 v)
		{
			return math.select(math.uint4(4205774813U, 1650214333U, 3388112843U, 1831150513U), math.uint4(1848374953U, 3430200247U, 2209710647U, 2201894441U), v.c0) + math.select(math.uint4(2849577407U, 3287031191U, 3098675399U, 1564399943U), math.uint4(1148435377U, 3416333663U, 1750611407U, 3285396193U), v.c1) + math.select(math.uint4(3110507567U, 4271396531U, 4198118021U, 2908068253U), math.uint4(3705492289U, 2497566569U, 2716413241U, 1166264321U), v.c2) + math.select(math.uint4(2503385333U, 2944493077U, 2599999021U, 3814721321U), math.uint4(1595355149U, 1728931849U, 2062756937U, 2920485769U), v.c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(double x, double y)
		{
			return new double2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(double2 xy)
		{
			return new double2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(double v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(bool v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(bool2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(int v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(int2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(uint v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(uint2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(half v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(half2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(float v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 double2(float2 v)
		{
			return new double2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double2 v)
		{
			return math.csum(math.fold_to_uint(v) * math.uint2(2503385333U, 2944493077U)) + 2599999021U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(double2 v)
		{
			return math.fold_to_uint(v) * math.uint2(3814721321U, 1595355149U) + 1728931849U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double shuffle(double2 left, double2 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 shuffle(double2 left, double2 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.double2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 shuffle(double2 left, double2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.double3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 shuffle(double2 left, double2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.double4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double select_shuffle_component(double2 a, double2 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(double2 c0, double2 c1)
		{
			return new double2x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(double m00, double m01, double m10, double m11)
		{
			return new double2x2(m00, m01, m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(double v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(bool v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(bool2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(int v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(int2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(uint v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(uint2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(float v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 double2x2(float2x2 v)
		{
			return new double2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 transpose(double2x2 v)
		{
			return math.double2x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 inverse(double2x2 m)
		{
			double x = m.c0.x;
			double x2 = m.c1.x;
			double y = m.c0.y;
			double y2 = m.c1.y;
			double num = x * y2 - x2 * y;
			return math.double2x2(y2, -x2, -y, x) * (1.0 / num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double determinant(double2x2 m)
		{
			double x = m.c0.x;
			double x2 = m.c1.x;
			double y = m.c0.y;
			double y2 = m.c1.y;
			return x * y2 - x2 * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double2x2 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint2(4253034763U, 2338696631U) + math.fold_to_uint(v.c1) * math.uint2(3757372771U, 1885959949U)) + 3508684087U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(double2x2 v)
		{
			return math.fold_to_uint(v.c0) * math.uint2(3919501043U, 1209161033U) + math.fold_to_uint(v.c1) * math.uint2(4007793211U, 3819806693U) + 3458005183U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(double2 c0, double2 c1, double2 c2)
		{
			return new double2x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(double m00, double m01, double m02, double m10, double m11, double m12)
		{
			return new double2x3(m00, m01, m02, m10, m11, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(double v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(bool v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(bool2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(int v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(int2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(uint v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(uint2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(float v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 double2x3(float2x3 v)
		{
			return new double2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 transpose(double2x3 v)
		{
			return math.double3x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double2x3 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint2(4066109527U, 2613148903U) + math.fold_to_uint(v.c1) * math.uint2(3367528529U, 1678332449U) + math.fold_to_uint(v.c2) * math.uint2(2918459647U, 2744611081U)) + 1952372791U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(double2x3 v)
		{
			return math.fold_to_uint(v.c0) * math.uint2(2631698677U, 4200781601U) + math.fold_to_uint(v.c1) * math.uint2(2119021007U, 1760485621U) + math.fold_to_uint(v.c2) * math.uint2(3157985881U, 2171534173U) + 2723054263U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(double2 c0, double2 c1, double2 c2, double2 c3)
		{
			return new double2x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(double m00, double m01, double m02, double m03, double m10, double m11, double m12, double m13)
		{
			return new double2x4(m00, m01, m02, m03, m10, m11, m12, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(double v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(bool v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(bool2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(int v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(int2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(uint v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(uint2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(float v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 double2x4(float2x4 v)
		{
			return new double2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 transpose(double2x4 v)
		{
			return math.double4x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y, v.c3.x, v.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double2x4 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint2(2437373431U, 1441286183U) + math.fold_to_uint(v.c1) * math.uint2(2426570171U, 1561977301U) + math.fold_to_uint(v.c2) * math.uint2(4205774813U, 1650214333U) + math.fold_to_uint(v.c3) * math.uint2(3388112843U, 1831150513U)) + 1848374953U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(double2x4 v)
		{
			return math.fold_to_uint(v.c0) * math.uint2(3430200247U, 2209710647U) + math.fold_to_uint(v.c1) * math.uint2(2201894441U, 2849577407U) + math.fold_to_uint(v.c2) * math.uint2(3287031191U, 3098675399U) + math.fold_to_uint(v.c3) * math.uint2(1564399943U, 1148435377U) + 3416333663U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(double x, double y, double z)
		{
			return new double3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(double x, double2 yz)
		{
			return new double3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(double2 xy, double z)
		{
			return new double3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(double3 xyz)
		{
			return new double3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(double v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(bool v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(bool3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(int v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(int3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(uint v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(uint3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(half v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(half3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(float v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 double3(float3 v)
		{
			return new double3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double3 v)
		{
			return math.csum(math.fold_to_uint(v) * math.uint3(2937008387U, 3835713223U, 2216526373U)) + 3375971453U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(double3 v)
		{
			return math.fold_to_uint(v) * math.uint3(3559829411U, 3652178029U, 2544260129U) + 2013864031U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double shuffle(double3 left, double3 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 shuffle(double3 left, double3 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.double2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 shuffle(double3 left, double3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.double3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 shuffle(double3 left, double3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.double4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double select_shuffle_component(double3 a, double3 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(double3 c0, double3 c1)
		{
			return new double3x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(double m00, double m01, double m10, double m11, double m20, double m21)
		{
			return new double3x2(m00, m01, m10, m11, m20, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(double v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(bool v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(bool3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(int v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(int3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(uint v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(uint3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(float v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 double3x2(float3x2 v)
		{
			return new double3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 transpose(double3x2 v)
		{
			return math.double2x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double3x2 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint3(3996716183U, 2626301701U, 1306289417U) + math.fold_to_uint(v.c1) * math.uint3(2096137163U, 1548578029U, 4178800919U)) + 3898072289U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(double3x2 v)
		{
			return math.fold_to_uint(v.c0) * math.uint3(4129428421U, 2631575897U, 2854656703U) + math.fold_to_uint(v.c1) * math.uint3(3578504047U, 4245178297U, 2173281923U) + 2973357649U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(double3 c0, double3 c1, double3 c2)
		{
			return new double3x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22)
		{
			return new double3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(double v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(bool v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(bool3x3 v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(int v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(int3x3 v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(uint v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(uint3x3 v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(float v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 double3x3(float3x3 v)
		{
			return new double3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 transpose(double3x3 v)
		{
			return math.double3x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z);
		}

		public static double3x3 inverse(double3x3 m)
		{
			double3 c = m.c0;
			double3 c2 = m.c1;
			double3 c3 = m.c2;
			double3 @double = math.double3(c2.x, c3.x, c.x);
			double3 double2 = math.double3(c2.y, c3.y, c.y);
			double3 double3 = math.double3(c2.z, c3.z, c.z);
			double3 double4 = double2 * double3.yzx - double2.yzx * double3;
			double3 double5 = @double.yzx * double3 - @double * double3.yzx;
			double3 double6 = @double * double2.yzx - @double.yzx * double2;
			double num = 1.0 / math.csum(@double.zxy * double4);
			return math.double3x3(double4, double5, double6) * num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double determinant(double3x3 m)
		{
			double3 c = m.c0;
			double3 c2 = m.c1;
			double3 c3 = m.c2;
			double num = c2.y * c3.z - c2.z * c3.y;
			double num2 = c.y * c3.z - c.z * c3.y;
			double num3 = c.y * c2.z - c.z * c2.y;
			return c.x * num - c2.x * num2 + c3.x * num3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double3x3 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint3(2891822459U, 2837054189U, 3016004371U) + math.fold_to_uint(v.c1) * math.uint3(4097481403U, 2229788699U, 2382715877U) + math.fold_to_uint(v.c2) * math.uint3(1851936439U, 1938025801U, 3712598587U)) + 3956330501U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(double3x3 v)
		{
			return math.fold_to_uint(v.c0) * math.uint3(2437373431U, 1441286183U, 2426570171U) + math.fold_to_uint(v.c1) * math.uint3(1561977301U, 4205774813U, 1650214333U) + math.fold_to_uint(v.c2) * math.uint3(3388112843U, 1831150513U, 1848374953U) + 3430200247U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(double3 c0, double3 c1, double3 c2, double3 c3)
		{
			return new double3x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(double m00, double m01, double m02, double m03, double m10, double m11, double m12, double m13, double m20, double m21, double m22, double m23)
		{
			return new double3x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(double v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(bool v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(bool3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(int v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(int3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(uint v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(uint3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(float v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 double3x4(float3x4 v)
		{
			return new double3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 transpose(double3x4 v)
		{
			return math.double4x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z, v.c3.x, v.c3.y, v.c3.z);
		}

		public static double3x4 fastinverse(double3x4 m)
		{
			double3 c = m.c0;
			double3 c2 = m.c1;
			double3 c3 = m.c2;
			double3 @double = m.c3;
			double3 double2 = math.double3(c.x, c2.x, c3.x);
			double3 double3 = math.double3(c.y, c2.y, c3.y);
			double3 double4 = math.double3(c.z, c2.z, c3.z);
			@double = -(double2 * @double.x + double3 * @double.y + double4 * @double.z);
			return math.double3x4(double2, double3, double4, @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double3x4 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint3(3996716183U, 2626301701U, 1306289417U) + math.fold_to_uint(v.c1) * math.uint3(2096137163U, 1548578029U, 4178800919U) + math.fold_to_uint(v.c2) * math.uint3(3898072289U, 4129428421U, 2631575897U) + math.fold_to_uint(v.c3) * math.uint3(2854656703U, 3578504047U, 4245178297U)) + 2173281923U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(double3x4 v)
		{
			return math.fold_to_uint(v.c0) * math.uint3(2973357649U, 3881277847U, 4017968839U) + math.fold_to_uint(v.c1) * math.uint3(1727237899U, 1648514723U, 1385344481U) + math.fold_to_uint(v.c2) * math.uint3(3538260197U, 4066109527U, 2613148903U) + math.fold_to_uint(v.c3) * math.uint3(3367528529U, 1678332449U, 2918459647U) + 2744611081U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double x, double y, double z, double w)
		{
			return new double4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double x, double y, double2 zw)
		{
			return new double4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double x, double2 yz, double w)
		{
			return new double4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double x, double3 yzw)
		{
			return new double4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double2 xy, double z, double w)
		{
			return new double4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double2 xy, double2 zw)
		{
			return new double4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double3 xyz, double w)
		{
			return new double4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double4 xyzw)
		{
			return new double4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(double v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(bool v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(bool4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(int v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(int4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(uint v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(uint4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(half v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(half4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(float v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 double4(float4 v)
		{
			return new double4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double4 v)
		{
			return math.csum(math.fold_to_uint(v) * math.uint4(2669441947U, 1260114311U, 2650080659U, 4052675461U)) + 2652487619U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(double4 v)
		{
			return math.fold_to_uint(v) * math.uint4(2174136431U, 3528391193U, 2105559227U, 1899745391U) + 1966790317U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double shuffle(double4 left, double4 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 shuffle(double4 left, double4 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.double2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 shuffle(double4 left, double4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.double3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 shuffle(double4 left, double4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.double4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double select_shuffle_component(double4 a, double4 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.LeftW:
				return a.w;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			case math.ShuffleComponent.RightW:
				return b.w;
			default:
				throw new ArgumentException("Invalid shuffle component: " + component.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(double4 c0, double4 c1)
		{
			return new double4x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(double m00, double m01, double m10, double m11, double m20, double m21, double m30, double m31)
		{
			return new double4x2(m00, m01, m10, m11, m20, m21, m30, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(double v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(bool v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(bool4x2 v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(int v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(int4x2 v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(uint v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(uint4x2 v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(float v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 double4x2(float4x2 v)
		{
			return new double4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 transpose(double4x2 v)
		{
			return math.double2x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double4x2 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint4(1521739981U, 1735296007U, 3010324327U, 1875523709U) + math.fold_to_uint(v.c1) * math.uint4(2937008387U, 3835713223U, 2216526373U, 3375971453U)) + 3559829411U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(double4x2 v)
		{
			return math.fold_to_uint(v.c0) * math.uint4(3652178029U, 2544260129U, 2013864031U, 2627668003U) + math.fold_to_uint(v.c1) * math.uint4(1520214331U, 2949502447U, 2827819133U, 3480140317U) + 2642994593U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(double4 c0, double4 c1, double4 c2)
		{
			return new double4x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22, double m30, double m31, double m32)
		{
			return new double4x3(m00, m01, m02, m10, m11, m12, m20, m21, m22, m30, m31, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(double v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(bool v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(bool4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(int v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(int4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(uint v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(uint4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(float v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 double4x3(float4x3 v)
		{
			return new double4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 transpose(double4x3 v)
		{
			return math.double3x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double4x3 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint4(2057338067U, 2942577577U, 2834440507U, 2671762487U) + math.fold_to_uint(v.c1) * math.uint4(2892026051U, 2455987759U, 3868600063U, 3170963179U) + math.fold_to_uint(v.c2) * math.uint4(2632835537U, 1136528209U, 2944626401U, 2972762423U)) + 1417889653U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(double4x3 v)
		{
			return math.fold_to_uint(v.c0) * math.uint4(2080514593U, 2731544287U, 2828498809U, 2669441947U) + math.fold_to_uint(v.c1) * math.uint4(1260114311U, 2650080659U, 4052675461U, 2652487619U) + math.fold_to_uint(v.c2) * math.uint4(2174136431U, 3528391193U, 2105559227U, 1899745391U) + 1966790317U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(double4 c0, double4 c1, double4 c2, double4 c3)
		{
			return new double4x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(double m00, double m01, double m02, double m03, double m10, double m11, double m12, double m13, double m20, double m21, double m22, double m23, double m30, double m31, double m32, double m33)
		{
			return new double4x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(double v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(bool v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(bool4x4 v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(int v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(int4x4 v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(uint v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(uint4x4 v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(float v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 double4x4(float4x4 v)
		{
			return new double4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 rotate(double4x4 a, double3 b)
		{
			return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 transform(double4x4 a, double3 b)
		{
			return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3).xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 transpose(double4x4 v)
		{
			return math.double4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
		}

		public static double4x4 inverse(double4x4 m)
		{
			double4 c = m.c0;
			double4 c2 = m.c1;
			double4 c3 = m.c2;
			double4 c4 = m.c3;
			double4 @double = math.movelh(c2, c);
			double4 double2 = math.movelh(c3, c4);
			double4 double3 = math.movehl(c, c2);
			double4 double4 = math.movehl(c4, c3);
			double4 double5 = math.shuffle(c2, c, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightY, math.ShuffleComponent.RightZ);
			double4 double6 = math.shuffle(c3, c4, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightY, math.ShuffleComponent.RightZ);
			double4 double7 = math.shuffle(c2, c, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightX);
			double4 double8 = math.shuffle(c3, c4, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightX);
			double4 double9 = math.shuffle(double2, @double, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			double4 double10 = math.shuffle(double2, @double, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY, math.ShuffleComponent.RightW);
			double4 double11 = math.shuffle(double4, double3, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			double4 double12 = math.shuffle(double4, double3, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY, math.ShuffleComponent.RightW);
			double4 double13 = math.shuffle(@double, double2, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			double4 double14 = double5 * double4 - double6 * double3;
			double4 double15 = @double * double4 - double2 * double3;
			double4 double16 = double8 * @double - double7 * double2;
			double4 double17 = math.shuffle(double14, double14, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			double4 double18 = math.shuffle(double14, double14, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			double4 double19 = math.shuffle(double15, double15, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			double4 double20 = math.shuffle(double15, double15, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			double4 double21 = double12 * double17 - double11 * double20 + double10 * double18;
			double4 double22 = double13 * double21;
			double22 += math.shuffle(double22, double22, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightZ);
			double22 -= math.shuffle(double22, double22, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightX, math.ShuffleComponent.RightX);
			double4 double23 = math.double4(1.0) / double22;
			double4x4 double4x;
			double4x.c0 = double21 * double23;
			double4 double24 = math.shuffle(double16, double16, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			double4 double25 = math.shuffle(double16, double16, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			double4 double26 = double11 * double24 - double9 * double18 - double12 * double19;
			double4x.c1 = double26 * double23;
			double4 double27 = double9 * double20 - double10 * double24 - double12 * double25;
			double4x.c2 = double27 * double23;
			double4 double28 = double10 * double19 - double9 * double17 + double11 * double25;
			double4x.c3 = double28 * double23;
			return double4x;
		}

		public static double4x4 fastinverse(double4x4 m)
		{
			double4 c = m.c0;
			double4 c2 = m.c1;
			double4 c3 = m.c2;
			double4 @double = m.c3;
			double4 double2 = math.double4(0);
			double4 double3 = math.unpacklo(c, c3);
			double4 double4 = math.unpacklo(c2, double2);
			double4 double5 = math.unpackhi(c, c3);
			double4 double6 = math.unpackhi(c2, double2);
			double4 double7 = math.unpacklo(double3, double4);
			double4 double8 = math.unpackhi(double3, double4);
			double4 double9 = math.unpacklo(double5, double6);
			@double = -(double7 * @double.x + double8 * @double.y + double9 * @double.z);
			@double.w = 1.0;
			return math.double4x4(double7, double8, double9, @double);
		}

		public static double determinant(double4x4 m)
		{
			double4 c = m.c0;
			double4 c2 = m.c1;
			double4 c3 = m.c2;
			double4 c4 = m.c3;
			double num = c2.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c2.z * c4.w - c2.w * c4.z) + c4.y * (c2.z * c3.w - c2.w * c3.z);
			double num2 = c.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c3.w - c.w * c3.z);
			double num3 = c.y * (c2.z * c4.w - c2.w * c4.z) - c2.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c2.w - c.w * c2.z);
			double num4 = c.y * (c2.z * c3.w - c2.w * c3.z) - c2.y * (c.z * c3.w - c.w * c3.z) + c3.y * (c.z * c2.w - c.w * c2.z);
			return c.x * num - c2.x * num2 + c3.x * num3 - c4.x * num4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(double4x4 v)
		{
			return math.csum(math.fold_to_uint(v.c0) * math.uint4(1306289417U, 2096137163U, 1548578029U, 4178800919U) + math.fold_to_uint(v.c1) * math.uint4(3898072289U, 4129428421U, 2631575897U, 2854656703U) + math.fold_to_uint(v.c2) * math.uint4(3578504047U, 4245178297U, 2173281923U, 2973357649U) + math.fold_to_uint(v.c3) * math.uint4(3881277847U, 4017968839U, 1727237899U, 1648514723U)) + 1385344481U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(double4x4 v)
		{
			return math.fold_to_uint(v.c0) * math.uint4(3538260197U, 4066109527U, 2613148903U, 3367528529U) + math.fold_to_uint(v.c1) * math.uint4(1678332449U, 2918459647U, 2744611081U, 1952372791U) + math.fold_to_uint(v.c2) * math.uint4(2631698677U, 4200781601U, 2119021007U, 1760485621U) + math.fold_to_uint(v.c3) * math.uint4(3157985881U, 2171534173U, 2723054263U, 1168253063U) + 4228926523U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(float x, float y)
		{
			return new float2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(float2 xy)
		{
			return new float2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(float v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(bool v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(bool2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(int v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(int2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(uint v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(uint2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(half v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(half2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(double v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 float2(double2 v)
		{
			return new float2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float2 v)
		{
			return math.csum(math.asuint(v) * math.uint2(4198118021U, 2908068253U)) + 3705492289U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(float2 v)
		{
			return math.asuint(v) * math.uint2(2497566569U, 2716413241U) + 1166264321U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float shuffle(float2 left, float2 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 shuffle(float2 left, float2 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.float2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 shuffle(float2 left, float2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.float3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 shuffle(float2 left, float2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.float4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float select_shuffle_component(float2 a, float2 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(float2 c0, float2 c1)
		{
			return new float2x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(float m00, float m01, float m10, float m11)
		{
			return new float2x2(m00, m01, m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(float v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(bool v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(bool2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(int v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(int2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(uint v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(uint2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(double v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 float2x2(double2x2 v)
		{
			return new float2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 transpose(float2x2 v)
		{
			return math.float2x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 inverse(float2x2 m)
		{
			float x = m.c0.x;
			float x2 = m.c1.x;
			float y = m.c0.y;
			float y2 = m.c1.y;
			float num = x * y2 - x2 * y;
			return math.float2x2(y2, -x2, -y, x) * (1f / num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float determinant(float2x2 m)
		{
			float x = m.c0.x;
			float x2 = m.c1.x;
			float y = m.c0.y;
			float y2 = m.c1.y;
			return x * y2 - x2 * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float2x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(2627668003U, 1520214331U) + math.asuint(v.c1) * math.uint2(2949502447U, 2827819133U)) + 3480140317U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(float2x2 v)
		{
			return math.asuint(v.c0) * math.uint2(2642994593U, 3940484981U) + math.asuint(v.c1) * math.uint2(1954192763U, 1091696537U) + 3052428017U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(float2 c0, float2 c1, float2 c2)
		{
			return new float2x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(float m00, float m01, float m02, float m10, float m11, float m12)
		{
			return new float2x3(m00, m01, m02, m10, m11, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(float v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(bool v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(bool2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(int v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(int2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(uint v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(uint2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(double v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 float2x3(double2x3 v)
		{
			return new float2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 transpose(float2x3 v)
		{
			return math.float3x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float2x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(3898072289U, 4129428421U) + math.asuint(v.c1) * math.uint2(2631575897U, 2854656703U) + math.asuint(v.c2) * math.uint2(3578504047U, 4245178297U)) + 2173281923U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(float2x3 v)
		{
			return math.asuint(v.c0) * math.uint2(2973357649U, 3881277847U) + math.asuint(v.c1) * math.uint2(4017968839U, 1727237899U) + math.asuint(v.c2) * math.uint2(1648514723U, 1385344481U) + 3538260197U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(float2 c0, float2 c1, float2 c2, float2 c3)
		{
			return new float2x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13)
		{
			return new float2x4(m00, m01, m02, m03, m10, m11, m12, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(float v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(bool v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(bool2x4 v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(int v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(int2x4 v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(uint v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(uint2x4 v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(double v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 float2x4(double2x4 v)
		{
			return new float2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 transpose(float2x4 v)
		{
			return math.float4x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y, v.c3.x, v.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float2x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(3546061613U, 2702024231U) + math.asuint(v.c1) * math.uint2(1452124841U, 1966089551U) + math.asuint(v.c2) * math.uint2(2668168249U, 1587512777U) + math.asuint(v.c3) * math.uint2(2353831999U, 3101256173U)) + 2891822459U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(float2x4 v)
		{
			return math.asuint(v.c0) * math.uint2(2837054189U, 3016004371U) + math.asuint(v.c1) * math.uint2(4097481403U, 2229788699U) + math.asuint(v.c2) * math.uint2(2382715877U, 1851936439U) + math.asuint(v.c3) * math.uint2(1938025801U, 3712598587U) + 3956330501U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(float x, float y, float z)
		{
			return new float3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(float x, float2 yz)
		{
			return new float3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(float2 xy, float z)
		{
			return new float3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(float3 xyz)
		{
			return new float3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(float v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(bool v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(bool3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(int v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(int3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(uint v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(uint3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(half v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(half3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(double v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 float3(double3 v)
		{
			return new float3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float3 v)
		{
			return math.csum(math.asuint(v) * math.uint3(2601761069U, 1254033427U, 2248573027U)) + 3612677113U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(float3 v)
		{
			return math.asuint(v) * math.uint3(1521739981U, 1735296007U, 3010324327U) + 1875523709U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float shuffle(float3 left, float3 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 shuffle(float3 left, float3 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.float2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 shuffle(float3 left, float3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.float3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 shuffle(float3 left, float3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.float4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float select_shuffle_component(float3 a, float3 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(float3 c0, float3 c1)
		{
			return new float3x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(float m00, float m01, float m10, float m11, float m20, float m21)
		{
			return new float3x2(m00, m01, m10, m11, m20, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(float v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(bool v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(bool3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(int v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(int3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(uint v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(uint3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(double v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 float3x2(double3x2 v)
		{
			return new float3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 transpose(float3x2 v)
		{
			return math.float2x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float3x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(3777095341U, 3385463369U, 1773538433U) + math.asuint(v.c1) * math.uint3(3773525029U, 4131962539U, 1809525511U)) + 4016293529U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(float3x2 v)
		{
			return math.asuint(v.c0) * math.uint3(2416021567U, 2828384717U, 2636362241U) + math.asuint(v.c1) * math.uint3(1258410977U, 1952565773U, 2037535609U) + 3592785499U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(float3 c0, float3 c1, float3 c2)
		{
			return new float3x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
		{
			return new float3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(float v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(bool v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(bool3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(int v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(int3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(uint v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(uint3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(double v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(double3x3 v)
		{
			return new float3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 transpose(float3x3 v)
		{
			return math.float3x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z);
		}

		public static float3x3 inverse(float3x3 m)
		{
			float3 c = m.c0;
			float3 c2 = m.c1;
			float3 c3 = m.c2;
			float3 @float = math.float3(c2.x, c3.x, c.x);
			float3 float2 = math.float3(c2.y, c3.y, c.y);
			float3 float3 = math.float3(c2.z, c3.z, c.z);
			float3 float4 = float2 * float3.yzx - float2.yzx * float3;
			float3 float5 = @float.yzx * float3 - @float * float3.yzx;
			float3 float6 = @float * float2.yzx - @float.yzx * float2;
			float num = 1f / math.csum(@float.zxy * float4);
			return math.float3x3(float4, float5, float6) * num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float determinant(float3x3 m)
		{
			float3 c = m.c0;
			float3 c2 = m.c1;
			float3 c3 = m.c2;
			float num = c2.y * c3.z - c2.z * c3.y;
			float num2 = c.y * c3.z - c.z * c3.y;
			float num3 = c.y * c2.z - c.z * c2.y;
			return c.x * num - c2.x * num2 + c3.x * num3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float3x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(1899745391U, 1966790317U, 3516359879U) + math.asuint(v.c1) * math.uint3(3050356579U, 4178586719U, 2558655391U) + math.asuint(v.c2) * math.uint3(1453413133U, 2152428077U, 1938706661U)) + 1338588197U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(float3x3 v)
		{
			return math.asuint(v.c0) * math.uint3(3439609253U, 3535343003U, 3546061613U) + math.asuint(v.c1) * math.uint3(2702024231U, 1452124841U, 1966089551U) + math.asuint(v.c2) * math.uint3(2668168249U, 1587512777U, 2353831999U) + 3101256173U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(float3 c0, float3 c1, float3 c2, float3 c3)
		{
			return new float3x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23)
		{
			return new float3x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(float v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(bool v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(bool3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(int v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(int3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(uint v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(uint3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(double v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 float3x4(double3x4 v)
		{
			return new float3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 transpose(float3x4 v)
		{
			return math.float4x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z, v.c3.x, v.c3.y, v.c3.z);
		}

		public static float3x4 fastinverse(float3x4 m)
		{
			float3 c = m.c0;
			float3 c2 = m.c1;
			float3 c3 = m.c2;
			float3 @float = m.c3;
			float3 float2 = math.float3(c.x, c2.x, c3.x);
			float3 float3 = math.float3(c.y, c2.y, c3.y);
			float3 float4 = math.float3(c.z, c2.z, c3.z);
			@float = -(float2 * @float.x + float3 * @float.y + float4 * @float.z);
			return math.float3x4(float2, float3, float4, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float3x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(4192899797U, 3271228601U, 1634639009U) + math.asuint(v.c1) * math.uint3(3318036811U, 3404170631U, 2048213449U) + math.asuint(v.c2) * math.uint3(4164671783U, 1780759499U, 1352369353U) + math.asuint(v.c3) * math.uint3(2446407751U, 1391928079U, 3475533443U)) + 3777095341U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(float3x4 v)
		{
			return math.asuint(v.c0) * math.uint3(3385463369U, 1773538433U, 3773525029U) + math.asuint(v.c1) * math.uint3(4131962539U, 1809525511U, 4016293529U) + math.asuint(v.c2) * math.uint3(2416021567U, 2828384717U, 2636362241U) + math.asuint(v.c3) * math.uint3(1258410977U, 1952565773U, 2037535609U) + 3592785499U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float x, float y, float z, float w)
		{
			return new float4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float x, float y, float2 zw)
		{
			return new float4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float x, float2 yz, float w)
		{
			return new float4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float x, float3 yzw)
		{
			return new float4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float2 xy, float z, float w)
		{
			return new float4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float2 xy, float2 zw)
		{
			return new float4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float3 xyz, float w)
		{
			return new float4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float4 xyzw)
		{
			return new float4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(float v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(bool v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(bool4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(int v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(int4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(uint v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(uint4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(half v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(half4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(double v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 float4(double4 v)
		{
			return new float4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float4 v)
		{
			return math.csum(math.asuint(v) * math.uint4(3868600063U, 3170963179U, 2632835537U, 1136528209U)) + 2944626401U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(float4 v)
		{
			return math.asuint(v) * math.uint4(2972762423U, 1417889653U, 2080514593U, 2731544287U) + 2828498809U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float shuffle(float4 left, float4 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 shuffle(float4 left, float4 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.float2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 shuffle(float4 left, float4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.float3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 shuffle(float4 left, float4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.float4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float select_shuffle_component(float4 a, float4 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.LeftW:
				return a.w;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			case math.ShuffleComponent.RightW:
				return b.w;
			default:
				throw new ArgumentException("Invalid shuffle component: " + component.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(float4 c0, float4 c1)
		{
			return new float4x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(float m00, float m01, float m10, float m11, float m20, float m21, float m30, float m31)
		{
			return new float4x2(m00, m01, m10, m11, m20, m21, m30, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(float v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(bool v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(bool4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(int v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(int4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(uint v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(uint4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(double v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 float4x2(double4x2 v)
		{
			return new float4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 transpose(float4x2 v)
		{
			return math.float2x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float4x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(2864955997U, 3525118277U, 2298260269U, 1632478733U) + math.asuint(v.c1) * math.uint4(1537393931U, 2353355467U, 3441847433U, 4052036147U)) + 2011389559U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(float4x2 v)
		{
			return math.asuint(v.c0) * math.uint4(2252224297U, 3784421429U, 1750626223U, 3571447507U) + math.asuint(v.c1) * math.uint4(3412283213U, 2601761069U, 1254033427U, 2248573027U) + 3612677113U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(float4 c0, float4 c1, float4 c2)
		{
			return new float4x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22, float m30, float m31, float m32)
		{
			return new float4x3(m00, m01, m02, m10, m11, m12, m20, m21, m22, m30, m31, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(float v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(bool v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(bool4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(int v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(int4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(uint v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(uint4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(double v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 float4x3(double4x3 v)
		{
			return new float4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 transpose(float4x3 v)
		{
			return math.float3x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float4x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(3309258581U, 1770373673U, 3778261171U, 3286279097U) + math.asuint(v.c1) * math.uint4(4264629071U, 1898591447U, 2641864091U, 1229113913U) + math.asuint(v.c2) * math.uint4(3020867117U, 1449055807U, 2479033387U, 3702457169U)) + 1845824257U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(float4x3 v)
		{
			return math.asuint(v.c0) * math.uint4(1963973621U, 2134758553U, 1391111867U, 1167706003U) + math.asuint(v.c1) * math.uint4(2209736489U, 3261535807U, 1740411209U, 2910609089U) + math.asuint(v.c2) * math.uint4(2183822701U, 3029516053U, 3547472099U, 2057487037U) + 3781937309U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(float4 c0, float4 c1, float4 c2, float4 c3)
		{
			return new float4x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
		{
			return new float4x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(float v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(bool v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(bool4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(int v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(int4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(uint v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(uint4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(double v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(double4x4 v)
		{
			return new float4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 rotate(float4x4 a, float3 b)
		{
			return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 transform(float4x4 a, float3 b)
		{
			return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3).xyz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 transpose(float4x4 v)
		{
			return math.float4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
		}

		public static float4x4 inverse(float4x4 m)
		{
			float4 c = m.c0;
			float4 c2 = m.c1;
			float4 c3 = m.c2;
			float4 c4 = m.c3;
			float4 @float = math.movelh(c2, c);
			float4 float2 = math.movelh(c3, c4);
			float4 float3 = math.movehl(c, c2);
			float4 float4 = math.movehl(c4, c3);
			float4 float5 = math.shuffle(c2, c, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightY, math.ShuffleComponent.RightZ);
			float4 float6 = math.shuffle(c3, c4, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightY, math.ShuffleComponent.RightZ);
			float4 float7 = math.shuffle(c2, c, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightX);
			float4 float8 = math.shuffle(c3, c4, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightX);
			float4 float9 = math.shuffle(float2, @float, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			float4 float10 = math.shuffle(float2, @float, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY, math.ShuffleComponent.RightW);
			float4 float11 = math.shuffle(float4, float3, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			float4 float12 = math.shuffle(float4, float3, math.ShuffleComponent.LeftW, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY, math.ShuffleComponent.RightW);
			float4 float13 = math.shuffle(@float, float2, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.RightZ);
			float4 float14 = float5 * float4 - float6 * float3;
			float4 float15 = @float * float4 - float2 * float3;
			float4 float16 = float8 * @float - float7 * float2;
			float4 float17 = math.shuffle(float14, float14, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			float4 float18 = math.shuffle(float14, float14, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			float4 float19 = math.shuffle(float15, float15, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			float4 float20 = math.shuffle(float15, float15, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			float4 float21 = float12 * float17 - float11 * float20 + float10 * float18;
			float4 float22 = float13 * float21;
			float22 += math.shuffle(float22, float22, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightW, math.ShuffleComponent.RightZ);
			float22 -= math.shuffle(float22, float22, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightX, math.ShuffleComponent.RightX);
			float4 float23 = math.float4(1f) / float22;
			float4x4 float4x;
			float4x.c0 = float21 * float23;
			float4 float24 = math.shuffle(float16, float16, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightX);
			float4 float25 = math.shuffle(float16, float16, math.ShuffleComponent.LeftY, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW, math.ShuffleComponent.RightY);
			float4 float26 = float11 * float24 - float9 * float18 - float12 * float19;
			float4x.c1 = float26 * float23;
			float4 float27 = float9 * float20 - float10 * float24 - float12 * float25;
			float4x.c2 = float27 * float23;
			float4 float28 = float10 * float19 - float9 * float17 + float11 * float25;
			float4x.c3 = float28 * float23;
			return float4x;
		}

		public static float4x4 fastinverse(float4x4 m)
		{
			float4 c = m.c0;
			float4 c2 = m.c1;
			float4 c3 = m.c2;
			float4 @float = m.c3;
			float4 float2 = math.float4(0);
			float4 float3 = math.unpacklo(c, c3);
			float4 float4 = math.unpacklo(c2, float2);
			float4 float5 = math.unpackhi(c, c3);
			float4 float6 = math.unpackhi(c2, float2);
			float4 float7 = math.unpacklo(float3, float4);
			float4 float8 = math.unpackhi(float3, float4);
			float4 float9 = math.unpacklo(float5, float6);
			@float = -(float7 * @float.x + float8 * @float.y + float9 * @float.z);
			@float.w = 1f;
			return math.float4x4(float7, float8, float9, @float);
		}

		public static float determinant(float4x4 m)
		{
			float4 c = m.c0;
			float4 c2 = m.c1;
			float4 c3 = m.c2;
			float4 c4 = m.c3;
			float num = c2.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c2.z * c4.w - c2.w * c4.z) + c4.y * (c2.z * c3.w - c2.w * c3.z);
			float num2 = c.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c3.w - c.w * c3.z);
			float num3 = c.y * (c2.z * c4.w - c2.w * c4.z) - c2.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c2.w - c.w * c2.z);
			float num4 = c.y * (c2.z * c3.w - c2.w * c3.z) - c2.y * (c.z * c3.w - c.w * c3.z) + c3.y * (c.z * c2.w - c.w * c2.z);
			return c.x * num - c2.x * num2 + c3.x * num3 - c4.x * num4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(float4x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(3299952959U, 3121178323U, 2948522579U, 1531026433U) + math.asuint(v.c1) * math.uint4(1365086453U, 3969870067U, 4192899797U, 3271228601U) + math.asuint(v.c2) * math.uint4(1634639009U, 3318036811U, 3404170631U, 2048213449U) + math.asuint(v.c3) * math.uint4(4164671783U, 1780759499U, 1352369353U, 2446407751U)) + 1391928079U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(float4x4 v)
		{
			return math.asuint(v.c0) * math.uint4(3475533443U, 3777095341U, 3385463369U, 1773538433U) + math.asuint(v.c1) * math.uint4(3773525029U, 4131962539U, 1809525511U, 4016293529U) + math.asuint(v.c2) * math.uint4(2416021567U, 2828384717U, 2636362241U, 1258410977U) + math.asuint(v.c3) * math.uint4(1952565773U, 2037535609U, 3592785499U, 3996716183U) + 2626301701U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half half(half x)
		{
			return new half(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half half(float v)
		{
			return new half(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half half(double v)
		{
			return new half(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(half v)
		{
			return (uint)v.value * 1952372791U + 2171534173U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(half x, half y)
		{
			return new half2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(half2 xy)
		{
			return new half2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(half v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(float v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(float2 v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(double v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half2 half2(double2 v)
		{
			return new half2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(half2 v)
		{
			return math.csum(math.uint2((uint)v.x.value, (uint)v.y.value) * math.uint2(1851936439U, 1938025801U)) + 3712598587U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(half2 v)
		{
			return math.uint2((uint)v.x.value, (uint)v.y.value) * math.uint2(3956330501U, 2437373431U) + 1441286183U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(half x, half y, half z)
		{
			return new half3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(half x, half2 yz)
		{
			return new half3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(half2 xy, half z)
		{
			return new half3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(half3 xyz)
		{
			return new half3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(half v)
		{
			return new half3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(float v)
		{
			return new half3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(float3 v)
		{
			return new half3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(double v)
		{
			return new half3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half3 half3(double3 v)
		{
			return new half3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(half3 v)
		{
			return math.csum(math.uint3((uint)v.x.value, (uint)v.y.value, (uint)v.z.value) * math.uint3(1750611407U, 3285396193U, 3110507567U)) + 4271396531U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(half3 v)
		{
			return math.uint3((uint)v.x.value, (uint)v.y.value, (uint)v.z.value) * math.uint3(4198118021U, 2908068253U, 3705492289U) + 2497566569U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half x, half y, half z, half w)
		{
			return new half4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half x, half y, half2 zw)
		{
			return new half4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half x, half2 yz, half w)
		{
			return new half4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half x, half3 yzw)
		{
			return new half4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half2 xy, half z, half w)
		{
			return new half4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half2 xy, half2 zw)
		{
			return new half4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half3 xyz, half w)
		{
			return new half4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half4 xyzw)
		{
			return new half4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(half v)
		{
			return new half4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(float v)
		{
			return new half4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(float4 v)
		{
			return new half4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(double v)
		{
			return new half4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static half4 half4(double4 v)
		{
			return new half4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(half4 v)
		{
			return math.csum(math.uint4((uint)v.x.value, (uint)v.y.value, (uint)v.z.value, (uint)v.w.value) * math.uint4(1952372791U, 2631698677U, 4200781601U, 2119021007U)) + 1760485621U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(half4 v)
		{
			return math.uint4((uint)v.x.value, (uint)v.y.value, (uint)v.z.value, (uint)v.w.value) * math.uint4(3157985881U, 2171534173U, 2723054263U, 1168253063U) + 4228926523U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(int x, int y)
		{
			return new int2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(int2 xy)
		{
			return new int2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(int v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(bool v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(bool2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(uint v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(uint2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(float v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(float2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(double v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 int2(double2 v)
		{
			return new int2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int2 v)
		{
			return math.csum(math.asuint(v) * math.uint2(2209710647U, 2201894441U)) + 2849577407U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(int2 v)
		{
			return math.asuint(v) * math.uint2(3287031191U, 3098675399U) + 1564399943U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int shuffle(int2 left, int2 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 shuffle(int2 left, int2 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.int2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 shuffle(int2 left, int2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.int3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 shuffle(int2 left, int2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.int4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int select_shuffle_component(int2 a, int2 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(int2 c0, int2 c1)
		{
			return new int2x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(int m00, int m01, int m10, int m11)
		{
			return new int2x2(m00, m01, m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(int v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(bool v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(bool2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(uint v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(uint2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(float v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(float2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(double v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 int2x2(double2x2 v)
		{
			return new int2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 transpose(int2x2 v)
		{
			return math.int2x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int determinant(int2x2 m)
		{
			int x = m.c0.x;
			int x2 = m.c1.x;
			int y = m.c0.y;
			int y2 = m.c1.y;
			return x * y2 - x2 * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int2x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(3784421429U, 1750626223U) + math.asuint(v.c1) * math.uint2(3571447507U, 3412283213U)) + 2601761069U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(int2x2 v)
		{
			return math.asuint(v.c0) * math.uint2(1254033427U, 2248573027U) + math.asuint(v.c1) * math.uint2(3612677113U, 1521739981U) + 1735296007U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(int2 c0, int2 c1, int2 c2)
		{
			return new int2x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(int m00, int m01, int m02, int m10, int m11, int m12)
		{
			return new int2x3(m00, m01, m02, m10, m11, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(int v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(bool v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(bool2x3 v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(uint v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(uint2x3 v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(float v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(float2x3 v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(double v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 int2x3(double2x3 v)
		{
			return new int2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 transpose(int2x3 v)
		{
			return math.int3x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int2x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(3404170631U, 2048213449U) + math.asuint(v.c1) * math.uint2(4164671783U, 1780759499U) + math.asuint(v.c2) * math.uint2(1352369353U, 2446407751U)) + 1391928079U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(int2x3 v)
		{
			return math.asuint(v.c0) * math.uint2(3475533443U, 3777095341U) + math.asuint(v.c1) * math.uint2(3385463369U, 1773538433U) + math.asuint(v.c2) * math.uint2(3773525029U, 4131962539U) + 1809525511U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(int2 c0, int2 c1, int2 c2, int2 c3)
		{
			return new int2x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(int m00, int m01, int m02, int m03, int m10, int m11, int m12, int m13)
		{
			return new int2x4(m00, m01, m02, m03, m10, m11, m12, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(int v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(bool v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(bool2x4 v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(uint v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(uint2x4 v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(float v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(float2x4 v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(double v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 int2x4(double2x4 v)
		{
			return new int2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 transpose(int2x4 v)
		{
			return math.int4x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y, v.c3.x, v.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int2x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint2(2057338067U, 2942577577U) + math.asuint(v.c1) * math.uint2(2834440507U, 2671762487U) + math.asuint(v.c2) * math.uint2(2892026051U, 2455987759U) + math.asuint(v.c3) * math.uint2(3868600063U, 3170963179U)) + 2632835537U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(int2x4 v)
		{
			return math.asuint(v.c0) * math.uint2(1136528209U, 2944626401U) + math.asuint(v.c1) * math.uint2(2972762423U, 1417889653U) + math.asuint(v.c2) * math.uint2(2080514593U, 2731544287U) + math.asuint(v.c3) * math.uint2(2828498809U, 2669441947U) + 1260114311U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(int x, int y, int z)
		{
			return new int3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(int x, int2 yz)
		{
			return new int3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(int2 xy, int z)
		{
			return new int3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(int3 xyz)
		{
			return new int3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(int v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(bool v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(bool3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(uint v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(uint3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(float v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(float3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(double v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 int3(double3 v)
		{
			return new int3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int3 v)
		{
			return math.csum(math.asuint(v) * math.uint3(1283419601U, 1210229737U, 2864955997U)) + 3525118277U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(int3 v)
		{
			return math.asuint(v) * math.uint3(2298260269U, 1632478733U, 1537393931U) + 2353355467U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int shuffle(int3 left, int3 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 shuffle(int3 left, int3 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.int2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 shuffle(int3 left, int3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.int3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 shuffle(int3 left, int3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.int4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int select_shuffle_component(int3 a, int3 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(int3 c0, int3 c1)
		{
			return new int3x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(int m00, int m01, int m10, int m11, int m20, int m21)
		{
			return new int3x2(m00, m01, m10, m11, m20, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(int v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(bool v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(bool3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(uint v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(uint3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(float v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(float3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(double v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 int3x2(double3x2 v)
		{
			return new int3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 transpose(int3x2 v)
		{
			return math.int2x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int3x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(3678265601U, 2070747979U, 1480171127U) + math.asuint(v.c1) * math.uint3(1588341193U, 4234155257U, 1811310911U)) + 2635799963U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(int3x2 v)
		{
			return math.asuint(v.c0) * math.uint3(4165137857U, 2759770933U, 2759319383U) + math.asuint(v.c1) * math.uint3(3299952959U, 3121178323U, 2948522579U) + 1531026433U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(int3 c0, int3 c1, int3 c2)
		{
			return new int3x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(int m00, int m01, int m02, int m10, int m11, int m12, int m20, int m21, int m22)
		{
			return new int3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(int v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(bool v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(bool3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(uint v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(uint3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(float v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(float3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(double v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 int3x3(double3x3 v)
		{
			return new int3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 transpose(int3x3 v)
		{
			return math.int3x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int determinant(int3x3 m)
		{
			int3 c = m.c0;
			int3 c2 = m.c1;
			int3 c3 = m.c2;
			int num = c2.y * c3.z - c2.z * c3.y;
			int num2 = c.y * c3.z - c.z * c3.y;
			int num3 = c.y * c2.z - c.z * c2.y;
			return c.x * num - c2.x * num2 + c3.x * num3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int3x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(2479033387U, 3702457169U, 1845824257U) + math.asuint(v.c1) * math.uint3(1963973621U, 2134758553U, 1391111867U) + math.asuint(v.c2) * math.uint3(1167706003U, 2209736489U, 3261535807U)) + 1740411209U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(int3x3 v)
		{
			return math.asuint(v.c0) * math.uint3(2910609089U, 2183822701U, 3029516053U) + math.asuint(v.c1) * math.uint3(3547472099U, 2057487037U, 3781937309U) + math.asuint(v.c2) * math.uint3(2057338067U, 2942577577U, 2834440507U) + 2671762487U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(int3 c0, int3 c1, int3 c2, int3 c3)
		{
			return new int3x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(int m00, int m01, int m02, int m03, int m10, int m11, int m12, int m13, int m20, int m21, int m22, int m23)
		{
			return new int3x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(int v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(bool v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(bool3x4 v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(uint v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(uint3x4 v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(float v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(float3x4 v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(double v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 int3x4(double3x4 v)
		{
			return new int3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 transpose(int3x4 v)
		{
			return math.int4x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z, v.c3.x, v.c3.y, v.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int3x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint3(1521739981U, 1735296007U, 3010324327U) + math.asuint(v.c1) * math.uint3(1875523709U, 2937008387U, 3835713223U) + math.asuint(v.c2) * math.uint3(2216526373U, 3375971453U, 3559829411U) + math.asuint(v.c3) * math.uint3(3652178029U, 2544260129U, 2013864031U)) + 2627668003U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(int3x4 v)
		{
			return math.asuint(v.c0) * math.uint3(1520214331U, 2949502447U, 2827819133U) + math.asuint(v.c1) * math.uint3(3480140317U, 2642994593U, 3940484981U) + math.asuint(v.c2) * math.uint3(1954192763U, 1091696537U, 3052428017U) + math.asuint(v.c3) * math.uint3(4253034763U, 2338696631U, 3757372771U) + 1885959949U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int x, int y, int z, int w)
		{
			return new int4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int x, int y, int2 zw)
		{
			return new int4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int x, int2 yz, int w)
		{
			return new int4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int x, int3 yzw)
		{
			return new int4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int2 xy, int z, int w)
		{
			return new int4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int2 xy, int2 zw)
		{
			return new int4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int3 xyz, int w)
		{
			return new int4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int4 xyzw)
		{
			return new int4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(int v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(bool v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(bool4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(uint v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(uint4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(float v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(float4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(double v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 int4(double4 v)
		{
			return new int4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int4 v)
		{
			return math.csum(math.asuint(v) * math.uint4(1845824257U, 1963973621U, 2134758553U, 1391111867U)) + 1167706003U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(int4 v)
		{
			return math.asuint(v) * math.uint4(2209736489U, 3261535807U, 1740411209U, 2910609089U) + 2183822701U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int shuffle(int4 left, int4 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 shuffle(int4 left, int4 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.int2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 shuffle(int4 left, int4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.int3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 shuffle(int4 left, int4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.int4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int select_shuffle_component(int4 a, int4 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.LeftW:
				return a.w;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			case math.ShuffleComponent.RightW:
				return b.w;
			default:
				throw new ArgumentException("Invalid shuffle component: " + component.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(int4 c0, int4 c1)
		{
			return new int4x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(int m00, int m01, int m10, int m11, int m20, int m21, int m30, int m31)
		{
			return new int4x2(m00, m01, m10, m11, m20, m21, m30, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(int v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(bool v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(bool4x2 v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(uint v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(uint4x2 v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(float v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(float4x2 v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(double v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 int4x2(double4x2 v)
		{
			return new int4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 transpose(int4x2 v)
		{
			return math.int2x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int4x2 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(4205774813U, 1650214333U, 3388112843U, 1831150513U) + math.asuint(v.c1) * math.uint4(1848374953U, 3430200247U, 2209710647U, 2201894441U)) + 2849577407U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(int4x2 v)
		{
			return math.asuint(v.c0) * math.uint4(3287031191U, 3098675399U, 1564399943U, 1148435377U) + math.asuint(v.c1) * math.uint4(3416333663U, 1750611407U, 3285396193U, 3110507567U) + 4271396531U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(int4 c0, int4 c1, int4 c2)
		{
			return new int4x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(int m00, int m01, int m02, int m10, int m11, int m12, int m20, int m21, int m22, int m30, int m31, int m32)
		{
			return new int4x3(m00, m01, m02, m10, m11, m12, m20, m21, m22, m30, m31, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(int v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(bool v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(bool4x3 v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(uint v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(uint4x3 v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(float v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(float4x3 v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(double v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 int4x3(double4x3 v)
		{
			return new int4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 transpose(int4x3 v)
		{
			return math.int3x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int4x3 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(1773538433U, 3773525029U, 4131962539U, 1809525511U) + math.asuint(v.c1) * math.uint4(4016293529U, 2416021567U, 2828384717U, 2636362241U) + math.asuint(v.c2) * math.uint4(1258410977U, 1952565773U, 2037535609U, 3592785499U)) + 3996716183U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(int4x3 v)
		{
			return math.asuint(v.c0) * math.uint4(2626301701U, 1306289417U, 2096137163U, 1548578029U) + math.asuint(v.c1) * math.uint4(4178800919U, 3898072289U, 4129428421U, 2631575897U) + math.asuint(v.c2) * math.uint4(2854656703U, 3578504047U, 4245178297U, 2173281923U) + 2973357649U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(int4 c0, int4 c1, int4 c2, int4 c3)
		{
			return new int4x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(int m00, int m01, int m02, int m03, int m10, int m11, int m12, int m13, int m20, int m21, int m22, int m23, int m30, int m31, int m32, int m33)
		{
			return new int4x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(int v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(bool v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(bool4x4 v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(uint v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(uint4x4 v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(float v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(float4x4 v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(double v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 int4x4(double4x4 v)
		{
			return new int4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 transpose(int4x4 v)
		{
			return math.int4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
		}

		public static int determinant(int4x4 m)
		{
			int4 c = m.c0;
			int4 c2 = m.c1;
			int4 c3 = m.c2;
			int4 c4 = m.c3;
			int num = c2.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c2.z * c4.w - c2.w * c4.z) + c4.y * (c2.z * c3.w - c2.w * c3.z);
			int num2 = c.y * (c3.z * c4.w - c3.w * c4.z) - c3.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c3.w - c.w * c3.z);
			int num3 = c.y * (c2.z * c4.w - c2.w * c4.z) - c2.y * (c.z * c4.w - c.w * c4.z) + c4.y * (c.z * c2.w - c.w * c2.z);
			int num4 = c.y * (c2.z * c3.w - c2.w * c3.z) - c2.y * (c.z * c3.w - c.w * c3.z) + c3.y * (c.z * c2.w - c.w * c2.z);
			return c.x * num - c2.x * num2 + c3.x * num3 - c4.x * num4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(int4x4 v)
		{
			return math.csum(math.asuint(v.c0) * math.uint4(1562056283U, 2265541847U, 1283419601U, 1210229737U) + math.asuint(v.c1) * math.uint4(2864955997U, 3525118277U, 2298260269U, 1632478733U) + math.asuint(v.c2) * math.uint4(1537393931U, 2353355467U, 3441847433U, 4052036147U) + math.asuint(v.c3) * math.uint4(2011389559U, 2252224297U, 3784421429U, 1750626223U)) + 3571447507U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(int4x4 v)
		{
			return math.asuint(v.c0) * math.uint4(3412283213U, 2601761069U, 1254033427U, 2248573027U) + math.asuint(v.c1) * math.uint4(3612677113U, 1521739981U, 1735296007U, 3010324327U) + math.asuint(v.c2) * math.uint4(1875523709U, 2937008387U, 3835713223U, 2216526373U) + math.asuint(v.c3) * math.uint4(3375971453U, 3559829411U, 3652178029U, 2544260129U) + 2013864031U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int asint(uint x)
		{
			return (int)x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 asint(uint2 x)
		{
			return math.int2((int)x.x, (int)x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 asint(uint3 x)
		{
			return math.int3((int)x.x, (int)x.y, (int)x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 asint(uint4 x)
		{
			return math.int4((int)x.x, (int)x.y, (int)x.z, (int)x.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int asint(float x)
		{
			math.IntFloatUnion intFloatUnion;
			intFloatUnion.intValue = 0;
			intFloatUnion.floatValue = x;
			return intFloatUnion.intValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 asint(float2 x)
		{
			return math.int2(math.asint(x.x), math.asint(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 asint(float3 x)
		{
			return math.int3(math.asint(x.x), math.asint(x.y), math.asint(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 asint(float4 x)
		{
			return math.int4(math.asint(x.x), math.asint(x.y), math.asint(x.z), math.asint(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint asuint(int x)
		{
			return (uint)x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 asuint(int2 x)
		{
			return math.uint2((uint)x.x, (uint)x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 asuint(int3 x)
		{
			return math.uint3((uint)x.x, (uint)x.y, (uint)x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 asuint(int4 x)
		{
			return math.uint4((uint)x.x, (uint)x.y, (uint)x.z, (uint)x.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint asuint(float x)
		{
			return (uint)math.asint(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 asuint(float2 x)
		{
			return math.uint2(math.asuint(x.x), math.asuint(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 asuint(float3 x)
		{
			return math.uint3(math.asuint(x.x), math.asuint(x.y), math.asuint(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 asuint(float4 x)
		{
			return math.uint4(math.asuint(x.x), math.asuint(x.y), math.asuint(x.z), math.asuint(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long aslong(ulong x)
		{
			return (long)x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long aslong(double x)
		{
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.longValue = 0L;
			longDoubleUnion.doubleValue = x;
			return longDoubleUnion.longValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong asulong(long x)
		{
			return (ulong)x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong asulong(double x)
		{
			return (ulong)math.aslong(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float asfloat(int x)
		{
			math.IntFloatUnion intFloatUnion;
			intFloatUnion.floatValue = 0f;
			intFloatUnion.intValue = x;
			return intFloatUnion.floatValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 asfloat(int2 x)
		{
			return math.float2(math.asfloat(x.x), math.asfloat(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 asfloat(int3 x)
		{
			return math.float3(math.asfloat(x.x), math.asfloat(x.y), math.asfloat(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 asfloat(int4 x)
		{
			return math.float4(math.asfloat(x.x), math.asfloat(x.y), math.asfloat(x.z), math.asfloat(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float asfloat(uint x)
		{
			return math.asfloat((int)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 asfloat(uint2 x)
		{
			return math.float2(math.asfloat(x.x), math.asfloat(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 asfloat(uint3 x)
		{
			return math.float3(math.asfloat(x.x), math.asfloat(x.y), math.asfloat(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 asfloat(uint4 x)
		{
			return math.float4(math.asfloat(x.x), math.asfloat(x.y), math.asfloat(x.z), math.asfloat(x.w));
		}

		public static int bitmask(bool4 value)
		{
			int num = 0;
			if (value.x)
			{
				num |= 1;
			}
			if (value.y)
			{
				num |= 2;
			}
			if (value.z)
			{
				num |= 4;
			}
			if (value.w)
			{
				num |= 8;
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double asdouble(long x)
		{
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.doubleValue = 0.0;
			longDoubleUnion.longValue = x;
			return longDoubleUnion.doubleValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double asdouble(ulong x)
		{
			return math.asdouble((long)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isfinite(float x)
		{
			return math.abs(x) < float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isfinite(float2 x)
		{
			return math.abs(x) < float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isfinite(float3 x)
		{
			return math.abs(x) < float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isfinite(float4 x)
		{
			return math.abs(x) < float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isfinite(double x)
		{
			return math.abs(x) < double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isfinite(double2 x)
		{
			return math.abs(x) < double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isfinite(double3 x)
		{
			return math.abs(x) < double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isfinite(double4 x)
		{
			return math.abs(x) < double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isinf(float x)
		{
			return math.abs(x) == float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isinf(float2 x)
		{
			return math.abs(x) == float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isinf(float3 x)
		{
			return math.abs(x) == float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isinf(float4 x)
		{
			return math.abs(x) == float.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isinf(double x)
		{
			return math.abs(x) == double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isinf(double2 x)
		{
			return math.abs(x) == double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isinf(double3 x)
		{
			return math.abs(x) == double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isinf(double4 x)
		{
			return math.abs(x) == double.PositiveInfinity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isnan(float x)
		{
			return (math.asuint(x) & 2147483647U) > 2139095040U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isnan(float2 x)
		{
			return (math.asuint(x) & 2147483647U) > 2139095040U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isnan(float3 x)
		{
			return (math.asuint(x) & 2147483647U) > 2139095040U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isnan(float4 x)
		{
			return (math.asuint(x) & 2147483647U) > 2139095040U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool isnan(double x)
		{
			return (math.asulong(x) & 9223372036854775807UL) > 9218868437227405312UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 isnan(double2 x)
		{
			return math.bool2((math.asulong(x.x) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.y) & 9223372036854775807UL) > 9218868437227405312UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 isnan(double3 x)
		{
			return math.bool3((math.asulong(x.x) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.y) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.z) & 9223372036854775807UL) > 9218868437227405312UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 isnan(double4 x)
		{
			return math.bool4((math.asulong(x.x) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.y) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.z) & 9223372036854775807UL) > 9218868437227405312UL, (math.asulong(x.w) & 9223372036854775807UL) > 9218868437227405312UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ispow2(int x)
		{
			return x > 0 && (x & (x - 1)) == 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 ispow2(int2 x)
		{
			return new bool2(math.ispow2(x.x), math.ispow2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 ispow2(int3 x)
		{
			return new bool3(math.ispow2(x.x), math.ispow2(x.y), math.ispow2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 ispow2(int4 x)
		{
			return new bool4(math.ispow2(x.x), math.ispow2(x.y), math.ispow2(x.z), math.ispow2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ispow2(uint x)
		{
			return x > 0U && (x & (x - 1U)) == 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool2 ispow2(uint2 x)
		{
			return new bool2(math.ispow2(x.x), math.ispow2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool3 ispow2(uint3 x)
		{
			return new bool3(math.ispow2(x.x), math.ispow2(x.y), math.ispow2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool4 ispow2(uint4 x)
		{
			return new bool4(math.ispow2(x.x), math.ispow2(x.y), math.ispow2(x.z), math.ispow2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int min(int x, int y)
		{
			if (x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 min(int2 x, int2 y)
		{
			return new int2(math.min(x.x, y.x), math.min(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 min(int3 x, int3 y)
		{
			return new int3(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 min(int4 x, int4 y)
		{
			return new int4(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z), math.min(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint min(uint x, uint y)
		{
			if (x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 min(uint2 x, uint2 y)
		{
			return new uint2(math.min(x.x, y.x), math.min(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 min(uint3 x, uint3 y)
		{
			return new uint3(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 min(uint4 x, uint4 y)
		{
			return new uint4(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z), math.min(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long min(long x, long y)
		{
			if (x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong min(ulong x, ulong y)
		{
			if (x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float min(float x, float y)
		{
			if (!float.IsNaN(y) && x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 min(float2 x, float2 y)
		{
			return new float2(math.min(x.x, y.x), math.min(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 min(float3 x, float3 y)
		{
			return new float3(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 min(float4 x, float4 y)
		{
			return new float4(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z), math.min(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double min(double x, double y)
		{
			if (!double.IsNaN(y) && x >= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 min(double2 x, double2 y)
		{
			return new double2(math.min(x.x, y.x), math.min(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 min(double3 x, double3 y)
		{
			return new double3(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 min(double4 x, double4 y)
		{
			return new double4(math.min(x.x, y.x), math.min(x.y, y.y), math.min(x.z, y.z), math.min(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int max(int x, int y)
		{
			if (x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 max(int2 x, int2 y)
		{
			return new int2(math.max(x.x, y.x), math.max(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 max(int3 x, int3 y)
		{
			return new int3(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 max(int4 x, int4 y)
		{
			return new int4(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z), math.max(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint max(uint x, uint y)
		{
			if (x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 max(uint2 x, uint2 y)
		{
			return new uint2(math.max(x.x, y.x), math.max(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 max(uint3 x, uint3 y)
		{
			return new uint3(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 max(uint4 x, uint4 y)
		{
			return new uint4(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z), math.max(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long max(long x, long y)
		{
			if (x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong max(ulong x, ulong y)
		{
			if (x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float max(float x, float y)
		{
			if (!float.IsNaN(y) && x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 max(float2 x, float2 y)
		{
			return new float2(math.max(x.x, y.x), math.max(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 max(float3 x, float3 y)
		{
			return new float3(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 max(float4 x, float4 y)
		{
			return new float4(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z), math.max(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double max(double x, double y)
		{
			if (!double.IsNaN(y) && x <= y)
			{
				return y;
			}
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 max(double2 x, double2 y)
		{
			return new double2(math.max(x.x, y.x), math.max(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 max(double3 x, double3 y)
		{
			return new double3(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 max(double4 x, double4 y)
		{
			return new double4(math.max(x.x, y.x), math.max(x.y, y.y), math.max(x.z, y.z), math.max(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lerp(float x, float y, float s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 lerp(float2 x, float2 y, float s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 lerp(float3 x, float3 y, float s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 lerp(float4 x, float4 y, float s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 lerp(float2 x, float2 y, float2 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 lerp(float3 x, float3 y, float3 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 lerp(float4 x, float4 y, float4 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double lerp(double x, double y, double s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 lerp(double2 x, double2 y, double s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 lerp(double3 x, double3 y, double s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 lerp(double4 x, double4 y, double s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 lerp(double2 x, double2 y, double2 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 lerp(double3 x, double3 y, double3 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 lerp(double4 x, double4 y, double4 s)
		{
			return x + s * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float unlerp(float a, float b, float x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 unlerp(float2 a, float2 b, float2 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 unlerp(float3 a, float3 b, float3 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 unlerp(float4 a, float4 b, float4 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double unlerp(double a, double b, double x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 unlerp(double2 a, double2 b, double2 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 unlerp(double3 a, double3 b, double3 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 unlerp(double4 a, double4 b, double4 x)
		{
			return (x - a) / (b - a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float remap(float a, float b, float c, float d, float x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 remap(float2 a, float2 b, float2 c, float2 d, float2 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 remap(float3 a, float3 b, float3 c, float3 d, float3 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 remap(float4 a, float4 b, float4 c, float4 d, float4 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double remap(double a, double b, double c, double d, double x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 remap(double2 a, double2 b, double2 c, double2 d, double2 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 remap(double3 a, double3 b, double3 c, double3 d, double3 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 remap(double4 a, double4 b, double4 c, double4 d, double4 x)
		{
			return math.lerp(c, d, math.unlerp(a, b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int mad(int a, int b, int c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mad(int2 a, int2 b, int2 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mad(int3 a, int3 b, int3 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mad(int4 a, int4 b, int4 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint mad(uint a, uint b, uint c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mad(uint2 a, uint2 b, uint2 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mad(uint3 a, uint3 b, uint3 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mad(uint4 a, uint4 b, uint4 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long mad(long a, long b, long c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong mad(ulong a, ulong b, ulong c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float mad(float a, float b, float c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mad(float2 a, float2 b, float2 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mad(float3 a, float3 b, float3 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mad(float4 a, float4 b, float4 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double mad(double a, double b, double c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mad(double2 a, double2 b, double2 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mad(double3 a, double3 b, double3 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mad(double4 a, double4 b, double4 c)
		{
			return a * b + c;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int clamp(int x, int a, int b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 clamp(int2 x, int2 a, int2 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 clamp(int3 x, int3 a, int3 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 clamp(int4 x, int4 a, int4 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint clamp(uint x, uint a, uint b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 clamp(uint2 x, uint2 a, uint2 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 clamp(uint3 x, uint3 a, uint3 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 clamp(uint4 x, uint4 a, uint4 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long clamp(long x, long a, long b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong clamp(ulong x, ulong a, ulong b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float clamp(float x, float a, float b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 clamp(float2 x, float2 a, float2 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 clamp(float3 x, float3 a, float3 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 clamp(float4 x, float4 a, float4 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double clamp(double x, double a, double b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 clamp(double2 x, double2 a, double2 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 clamp(double3 x, double3 a, double3 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 clamp(double4 x, double4 a, double4 b)
		{
			return math.max(a, math.min(b, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float saturate(float x)
		{
			return math.clamp(x, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 saturate(float2 x)
		{
			return math.clamp(x, new float2(0f), new float2(1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 saturate(float3 x)
		{
			return math.clamp(x, new float3(0f), new float3(1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 saturate(float4 x)
		{
			return math.clamp(x, new float4(0f), new float4(1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double saturate(double x)
		{
			return math.clamp(x, 0.0, 1.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 saturate(double2 x)
		{
			return math.clamp(x, new double2(0.0), new double2(1.0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 saturate(double3 x)
		{
			return math.clamp(x, new double3(0.0), new double3(1.0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 saturate(double4 x)
		{
			return math.clamp(x, new double4(0.0), new double4(1.0));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int abs(int x)
		{
			return math.max(-x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 abs(int2 x)
		{
			return math.max(-x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 abs(int3 x)
		{
			return math.max(-x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 abs(int4 x)
		{
			return math.max(-x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long abs(long x)
		{
			return math.max(-x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float abs(float x)
		{
			return math.asfloat(math.asuint(x) & 2147483647U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 abs(float2 x)
		{
			return math.asfloat(math.asuint(x) & 2147483647U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 abs(float3 x)
		{
			return math.asfloat(math.asuint(x) & 2147483647U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 abs(float4 x)
		{
			return math.asfloat(math.asuint(x) & 2147483647U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double abs(double x)
		{
			return math.asdouble(math.asulong(x) & 9223372036854775807UL);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 abs(double2 x)
		{
			return math.double2(math.asdouble(math.asulong(x.x) & 9223372036854775807UL), math.asdouble(math.asulong(x.y) & 9223372036854775807UL));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 abs(double3 x)
		{
			return math.double3(math.asdouble(math.asulong(x.x) & 9223372036854775807UL), math.asdouble(math.asulong(x.y) & 9223372036854775807UL), math.asdouble(math.asulong(x.z) & 9223372036854775807UL));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 abs(double4 x)
		{
			return math.double4(math.asdouble(math.asulong(x.x) & 9223372036854775807UL), math.asdouble(math.asulong(x.y) & 9223372036854775807UL), math.asdouble(math.asulong(x.z) & 9223372036854775807UL), math.asdouble(math.asulong(x.w) & 9223372036854775807UL));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int dot(int x, int y)
		{
			return x * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int dot(int2 x, int2 y)
		{
			return x.x * y.x + x.y * y.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int dot(int3 x, int3 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int dot(int4 x, int4 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint dot(uint x, uint y)
		{
			return x * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint dot(uint2 x, uint2 y)
		{
			return x.x * y.x + x.y * y.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint dot(uint3 x, uint3 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint dot(uint4 x, uint4 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float dot(float x, float y)
		{
			return x * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float dot(float2 x, float2 y)
		{
			return x.x * y.x + x.y * y.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float dot(float3 x, float3 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float dot(float4 x, float4 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double dot(double x, double y)
		{
			return x * y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double dot(double2 x, double2 y)
		{
			return x.x * y.x + x.y * y.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double dot(double3 x, double3 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double dot(double4 x, double4 y)
		{
			return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float tan(float x)
		{
			return (float)Math.Tan((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 tan(float2 x)
		{
			return new float2(math.tan(x.x), math.tan(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 tan(float3 x)
		{
			return new float3(math.tan(x.x), math.tan(x.y), math.tan(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 tan(float4 x)
		{
			return new float4(math.tan(x.x), math.tan(x.y), math.tan(x.z), math.tan(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double tan(double x)
		{
			return Math.Tan(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 tan(double2 x)
		{
			return new double2(math.tan(x.x), math.tan(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 tan(double3 x)
		{
			return new double3(math.tan(x.x), math.tan(x.y), math.tan(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 tan(double4 x)
		{
			return new double4(math.tan(x.x), math.tan(x.y), math.tan(x.z), math.tan(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float tanh(float x)
		{
			return (float)Math.Tanh((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 tanh(float2 x)
		{
			return new float2(math.tanh(x.x), math.tanh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 tanh(float3 x)
		{
			return new float3(math.tanh(x.x), math.tanh(x.y), math.tanh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 tanh(float4 x)
		{
			return new float4(math.tanh(x.x), math.tanh(x.y), math.tanh(x.z), math.tanh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double tanh(double x)
		{
			return Math.Tanh(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 tanh(double2 x)
		{
			return new double2(math.tanh(x.x), math.tanh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 tanh(double3 x)
		{
			return new double3(math.tanh(x.x), math.tanh(x.y), math.tanh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 tanh(double4 x)
		{
			return new double4(math.tanh(x.x), math.tanh(x.y), math.tanh(x.z), math.tanh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float atan(float x)
		{
			return (float)Math.Atan((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 atan(float2 x)
		{
			return new float2(math.atan(x.x), math.atan(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 atan(float3 x)
		{
			return new float3(math.atan(x.x), math.atan(x.y), math.atan(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 atan(float4 x)
		{
			return new float4(math.atan(x.x), math.atan(x.y), math.atan(x.z), math.atan(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double atan(double x)
		{
			return Math.Atan(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 atan(double2 x)
		{
			return new double2(math.atan(x.x), math.atan(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 atan(double3 x)
		{
			return new double3(math.atan(x.x), math.atan(x.y), math.atan(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 atan(double4 x)
		{
			return new double4(math.atan(x.x), math.atan(x.y), math.atan(x.z), math.atan(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float atan2(float y, float x)
		{
			return (float)Math.Atan2((double)y, (double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 atan2(float2 y, float2 x)
		{
			return new float2(math.atan2(y.x, x.x), math.atan2(y.y, x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 atan2(float3 y, float3 x)
		{
			return new float3(math.atan2(y.x, x.x), math.atan2(y.y, x.y), math.atan2(y.z, x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 atan2(float4 y, float4 x)
		{
			return new float4(math.atan2(y.x, x.x), math.atan2(y.y, x.y), math.atan2(y.z, x.z), math.atan2(y.w, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double atan2(double y, double x)
		{
			return Math.Atan2(y, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 atan2(double2 y, double2 x)
		{
			return new double2(math.atan2(y.x, x.x), math.atan2(y.y, x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 atan2(double3 y, double3 x)
		{
			return new double3(math.atan2(y.x, x.x), math.atan2(y.y, x.y), math.atan2(y.z, x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 atan2(double4 y, double4 x)
		{
			return new double4(math.atan2(y.x, x.x), math.atan2(y.y, x.y), math.atan2(y.z, x.z), math.atan2(y.w, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 cos(float2 x)
		{
			return new float2(math.cos(x.x), math.cos(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 cos(float3 x)
		{
			return new float3(math.cos(x.x), math.cos(x.y), math.cos(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 cos(float4 x)
		{
			return new float4(math.cos(x.x), math.cos(x.y), math.cos(x.z), math.cos(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cos(double x)
		{
			return Math.Cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 cos(double2 x)
		{
			return new double2(math.cos(x.x), math.cos(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 cos(double3 x)
		{
			return new double3(math.cos(x.x), math.cos(x.y), math.cos(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 cos(double4 x)
		{
			return new double4(math.cos(x.x), math.cos(x.y), math.cos(x.z), math.cos(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cosh(float x)
		{
			return (float)Math.Cosh((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 cosh(float2 x)
		{
			return new float2(math.cosh(x.x), math.cosh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 cosh(float3 x)
		{
			return new float3(math.cosh(x.x), math.cosh(x.y), math.cosh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 cosh(float4 x)
		{
			return new float4(math.cosh(x.x), math.cosh(x.y), math.cosh(x.z), math.cosh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cosh(double x)
		{
			return Math.Cosh(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 cosh(double2 x)
		{
			return new double2(math.cosh(x.x), math.cosh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 cosh(double3 x)
		{
			return new double3(math.cosh(x.x), math.cosh(x.y), math.cosh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 cosh(double4 x)
		{
			return new double4(math.cosh(x.x), math.cosh(x.y), math.cosh(x.z), math.cosh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float acos(float x)
		{
			return (float)Math.Acos((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 acos(float2 x)
		{
			return new float2(math.acos(x.x), math.acos(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 acos(float3 x)
		{
			return new float3(math.acos(x.x), math.acos(x.y), math.acos(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 acos(float4 x)
		{
			return new float4(math.acos(x.x), math.acos(x.y), math.acos(x.z), math.acos(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double acos(double x)
		{
			return Math.Acos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 acos(double2 x)
		{
			return new double2(math.acos(x.x), math.acos(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 acos(double3 x)
		{
			return new double3(math.acos(x.x), math.acos(x.y), math.acos(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 acos(double4 x)
		{
			return new double4(math.acos(x.x), math.acos(x.y), math.acos(x.z), math.acos(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 sin(float2 x)
		{
			return new float2(math.sin(x.x), math.sin(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 sin(float3 x)
		{
			return new float3(math.sin(x.x), math.sin(x.y), math.sin(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 sin(float4 x)
		{
			return new float4(math.sin(x.x), math.sin(x.y), math.sin(x.z), math.sin(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double sin(double x)
		{
			return Math.Sin(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 sin(double2 x)
		{
			return new double2(math.sin(x.x), math.sin(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 sin(double3 x)
		{
			return new double3(math.sin(x.x), math.sin(x.y), math.sin(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 sin(double4 x)
		{
			return new double4(math.sin(x.x), math.sin(x.y), math.sin(x.z), math.sin(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sinh(float x)
		{
			return (float)Math.Sinh((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 sinh(float2 x)
		{
			return new float2(math.sinh(x.x), math.sinh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 sinh(float3 x)
		{
			return new float3(math.sinh(x.x), math.sinh(x.y), math.sinh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 sinh(float4 x)
		{
			return new float4(math.sinh(x.x), math.sinh(x.y), math.sinh(x.z), math.sinh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double sinh(double x)
		{
			return Math.Sinh(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 sinh(double2 x)
		{
			return new double2(math.sinh(x.x), math.sinh(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 sinh(double3 x)
		{
			return new double3(math.sinh(x.x), math.sinh(x.y), math.sinh(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 sinh(double4 x)
		{
			return new double4(math.sinh(x.x), math.sinh(x.y), math.sinh(x.z), math.sinh(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float asin(float x)
		{
			return (float)Math.Asin((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 asin(float2 x)
		{
			return new float2(math.asin(x.x), math.asin(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 asin(float3 x)
		{
			return new float3(math.asin(x.x), math.asin(x.y), math.asin(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 asin(float4 x)
		{
			return new float4(math.asin(x.x), math.asin(x.y), math.asin(x.z), math.asin(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double asin(double x)
		{
			return Math.Asin(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 asin(double2 x)
		{
			return new double2(math.asin(x.x), math.asin(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 asin(double3 x)
		{
			return new double3(math.asin(x.x), math.asin(x.y), math.asin(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 asin(double4 x)
		{
			return new double4(math.asin(x.x), math.asin(x.y), math.asin(x.z), math.asin(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float floor(float x)
		{
			return (float)Math.Floor((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 floor(float2 x)
		{
			return new float2(math.floor(x.x), math.floor(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 floor(float3 x)
		{
			return new float3(math.floor(x.x), math.floor(x.y), math.floor(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 floor(float4 x)
		{
			return new float4(math.floor(x.x), math.floor(x.y), math.floor(x.z), math.floor(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double floor(double x)
		{
			return Math.Floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 floor(double2 x)
		{
			return new double2(math.floor(x.x), math.floor(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 floor(double3 x)
		{
			return new double3(math.floor(x.x), math.floor(x.y), math.floor(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 floor(double4 x)
		{
			return new double4(math.floor(x.x), math.floor(x.y), math.floor(x.z), math.floor(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float ceil(float x)
		{
			return (float)Math.Ceiling((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 ceil(float2 x)
		{
			return new float2(math.ceil(x.x), math.ceil(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 ceil(float3 x)
		{
			return new float3(math.ceil(x.x), math.ceil(x.y), math.ceil(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 ceil(float4 x)
		{
			return new float4(math.ceil(x.x), math.ceil(x.y), math.ceil(x.z), math.ceil(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ceil(double x)
		{
			return Math.Ceiling(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 ceil(double2 x)
		{
			return new double2(math.ceil(x.x), math.ceil(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 ceil(double3 x)
		{
			return new double3(math.ceil(x.x), math.ceil(x.y), math.ceil(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 ceil(double4 x)
		{
			return new double4(math.ceil(x.x), math.ceil(x.y), math.ceil(x.z), math.ceil(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float round(float x)
		{
			return (float)Math.Round((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 round(float2 x)
		{
			return new float2(math.round(x.x), math.round(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 round(float3 x)
		{
			return new float3(math.round(x.x), math.round(x.y), math.round(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 round(float4 x)
		{
			return new float4(math.round(x.x), math.round(x.y), math.round(x.z), math.round(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double round(double x)
		{
			return Math.Round(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 round(double2 x)
		{
			return new double2(math.round(x.x), math.round(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 round(double3 x)
		{
			return new double3(math.round(x.x), math.round(x.y), math.round(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 round(double4 x)
		{
			return new double4(math.round(x.x), math.round(x.y), math.round(x.z), math.round(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float trunc(float x)
		{
			return (float)Math.Truncate((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 trunc(float2 x)
		{
			return new float2(math.trunc(x.x), math.trunc(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 trunc(float3 x)
		{
			return new float3(math.trunc(x.x), math.trunc(x.y), math.trunc(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 trunc(float4 x)
		{
			return new float4(math.trunc(x.x), math.trunc(x.y), math.trunc(x.z), math.trunc(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double trunc(double x)
		{
			return Math.Truncate(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 trunc(double2 x)
		{
			return new double2(math.trunc(x.x), math.trunc(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 trunc(double3 x)
		{
			return new double3(math.trunc(x.x), math.trunc(x.y), math.trunc(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 trunc(double4 x)
		{
			return new double4(math.trunc(x.x), math.trunc(x.y), math.trunc(x.z), math.trunc(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float frac(float x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 frac(float2 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 frac(float3 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 frac(float4 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double frac(double x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 frac(double2 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 frac(double3 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 frac(double4 x)
		{
			return x - math.floor(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float rcp(float x)
		{
			return 1f / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 rcp(float2 x)
		{
			return 1f / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 rcp(float3 x)
		{
			return 1f / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 rcp(float4 x)
		{
			return 1f / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double rcp(double x)
		{
			return 1.0 / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 rcp(double2 x)
		{
			return 1.0 / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 rcp(double3 x)
		{
			return 1.0 / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 rcp(double4 x)
		{
			return 1.0 / x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sign(float x)
		{
			return ((x > 0f) ? 1f : 0f) - ((x < 0f) ? 1f : 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 sign(float2 x)
		{
			return new float2(math.sign(x.x), math.sign(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 sign(float3 x)
		{
			return new float3(math.sign(x.x), math.sign(x.y), math.sign(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 sign(float4 x)
		{
			return new float4(math.sign(x.x), math.sign(x.y), math.sign(x.z), math.sign(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double sign(double x)
		{
			if (x != 0.0)
			{
				return ((x > 0.0) ? 1.0 : 0.0) - ((x < 0.0) ? 1.0 : 0.0);
			}
			return 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 sign(double2 x)
		{
			return new double2(math.sign(x.x), math.sign(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 sign(double3 x)
		{
			return new double3(math.sign(x.x), math.sign(x.y), math.sign(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 sign(double4 x)
		{
			return new double4(math.sign(x.x), math.sign(x.y), math.sign(x.z), math.sign(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 pow(float2 x, float2 y)
		{
			return new float2(math.pow(x.x, y.x), math.pow(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 pow(float3 x, float3 y)
		{
			return new float3(math.pow(x.x, y.x), math.pow(x.y, y.y), math.pow(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 pow(float4 x, float4 y)
		{
			return new float4(math.pow(x.x, y.x), math.pow(x.y, y.y), math.pow(x.z, y.z), math.pow(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double pow(double x, double y)
		{
			return Math.Pow(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 pow(double2 x, double2 y)
		{
			return new double2(math.pow(x.x, y.x), math.pow(x.y, y.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 pow(double3 x, double3 y)
		{
			return new double3(math.pow(x.x, y.x), math.pow(x.y, y.y), math.pow(x.z, y.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 pow(double4 x, double4 y)
		{
			return new double4(math.pow(x.x, y.x), math.pow(x.y, y.y), math.pow(x.z, y.z), math.pow(x.w, y.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float exp(float x)
		{
			return (float)Math.Exp((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 exp(float2 x)
		{
			return new float2(math.exp(x.x), math.exp(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 exp(float3 x)
		{
			return new float3(math.exp(x.x), math.exp(x.y), math.exp(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 exp(float4 x)
		{
			return new float4(math.exp(x.x), math.exp(x.y), math.exp(x.z), math.exp(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double exp(double x)
		{
			return Math.Exp(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 exp(double2 x)
		{
			return new double2(math.exp(x.x), math.exp(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 exp(double3 x)
		{
			return new double3(math.exp(x.x), math.exp(x.y), math.exp(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 exp(double4 x)
		{
			return new double4(math.exp(x.x), math.exp(x.y), math.exp(x.z), math.exp(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float exp2(float x)
		{
			return (float)Math.Exp((double)(x * 0.6931472f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 exp2(float2 x)
		{
			return new float2(math.exp2(x.x), math.exp2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 exp2(float3 x)
		{
			return new float3(math.exp2(x.x), math.exp2(x.y), math.exp2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 exp2(float4 x)
		{
			return new float4(math.exp2(x.x), math.exp2(x.y), math.exp2(x.z), math.exp2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double exp2(double x)
		{
			return Math.Exp(x * 0.6931471805599453);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 exp2(double2 x)
		{
			return new double2(math.exp2(x.x), math.exp2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 exp2(double3 x)
		{
			return new double3(math.exp2(x.x), math.exp2(x.y), math.exp2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 exp2(double4 x)
		{
			return new double4(math.exp2(x.x), math.exp2(x.y), math.exp2(x.z), math.exp2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float exp10(float x)
		{
			return (float)Math.Exp((double)(x * 2.3025851f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 exp10(float2 x)
		{
			return new float2(math.exp10(x.x), math.exp10(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 exp10(float3 x)
		{
			return new float3(math.exp10(x.x), math.exp10(x.y), math.exp10(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 exp10(float4 x)
		{
			return new float4(math.exp10(x.x), math.exp10(x.y), math.exp10(x.z), math.exp10(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double exp10(double x)
		{
			return Math.Exp(x * 2.302585092994046);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 exp10(double2 x)
		{
			return new double2(math.exp10(x.x), math.exp10(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 exp10(double3 x)
		{
			return new double3(math.exp10(x.x), math.exp10(x.y), math.exp10(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 exp10(double4 x)
		{
			return new double4(math.exp10(x.x), math.exp10(x.y), math.exp10(x.z), math.exp10(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float log(float x)
		{
			return (float)Math.Log((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 log(float2 x)
		{
			return new float2(math.log(x.x), math.log(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 log(float3 x)
		{
			return new float3(math.log(x.x), math.log(x.y), math.log(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 log(float4 x)
		{
			return new float4(math.log(x.x), math.log(x.y), math.log(x.z), math.log(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double log(double x)
		{
			return Math.Log(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 log(double2 x)
		{
			return new double2(math.log(x.x), math.log(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 log(double3 x)
		{
			return new double3(math.log(x.x), math.log(x.y), math.log(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 log(double4 x)
		{
			return new double4(math.log(x.x), math.log(x.y), math.log(x.z), math.log(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float log2(float x)
		{
			return (float)Math.Log((double)x, 2.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 log2(float2 x)
		{
			return new float2(math.log2(x.x), math.log2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 log2(float3 x)
		{
			return new float3(math.log2(x.x), math.log2(x.y), math.log2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 log2(float4 x)
		{
			return new float4(math.log2(x.x), math.log2(x.y), math.log2(x.z), math.log2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double log2(double x)
		{
			return Math.Log(x, 2.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 log2(double2 x)
		{
			return new double2(math.log2(x.x), math.log2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 log2(double3 x)
		{
			return new double3(math.log2(x.x), math.log2(x.y), math.log2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 log2(double4 x)
		{
			return new double4(math.log2(x.x), math.log2(x.y), math.log2(x.z), math.log2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float log10(float x)
		{
			return (float)Math.Log10((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 log10(float2 x)
		{
			return new float2(math.log10(x.x), math.log10(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 log10(float3 x)
		{
			return new float3(math.log10(x.x), math.log10(x.y), math.log10(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 log10(float4 x)
		{
			return new float4(math.log10(x.x), math.log10(x.y), math.log10(x.z), math.log10(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double log10(double x)
		{
			return Math.Log10(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 log10(double2 x)
		{
			return new double2(math.log10(x.x), math.log10(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 log10(double3 x)
		{
			return new double3(math.log10(x.x), math.log10(x.y), math.log10(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 log10(double4 x)
		{
			return new double4(math.log10(x.x), math.log10(x.y), math.log10(x.z), math.log10(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float fmod(float x, float y)
		{
			return x % y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 fmod(float2 x, float2 y)
		{
			return new float2(x.x % y.x, x.y % y.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 fmod(float3 x, float3 y)
		{
			return new float3(x.x % y.x, x.y % y.y, x.z % y.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 fmod(float4 x, float4 y)
		{
			return new float4(x.x % y.x, x.y % y.y, x.z % y.z, x.w % y.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double fmod(double x, double y)
		{
			return x % y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 fmod(double2 x, double2 y)
		{
			return new double2(x.x % y.x, x.y % y.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 fmod(double3 x, double3 y)
		{
			return new double3(x.x % y.x, x.y % y.y, x.z % y.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 fmod(double4 x, double4 y)
		{
			return new double4(x.x % y.x, x.y % y.y, x.z % y.z, x.w % y.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float modf(float x, out float i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 modf(float2 x, out float2 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 modf(float3 x, out float3 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 modf(float4 x, out float4 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double modf(double x, out double i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 modf(double2 x, out double2 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 modf(double3 x, out double3 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 modf(double4 x, out double4 i)
		{
			i = math.trunc(x);
			return x - i;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 sqrt(float2 x)
		{
			return new float2(math.sqrt(x.x), math.sqrt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 sqrt(float3 x)
		{
			return new float3(math.sqrt(x.x), math.sqrt(x.y), math.sqrt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 sqrt(float4 x)
		{
			return new float4(math.sqrt(x.x), math.sqrt(x.y), math.sqrt(x.z), math.sqrt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double sqrt(double x)
		{
			return Math.Sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 sqrt(double2 x)
		{
			return new double2(math.sqrt(x.x), math.sqrt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 sqrt(double3 x)
		{
			return new double3(math.sqrt(x.x), math.sqrt(x.y), math.sqrt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 sqrt(double4 x)
		{
			return new double4(math.sqrt(x.x), math.sqrt(x.y), math.sqrt(x.z), math.sqrt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float rsqrt(float x)
		{
			return 1f / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 rsqrt(float2 x)
		{
			return 1f / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 rsqrt(float3 x)
		{
			return 1f / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 rsqrt(float4 x)
		{
			return 1f / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double rsqrt(double x)
		{
			return 1.0 / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 rsqrt(double2 x)
		{
			return 1.0 / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 rsqrt(double3 x)
		{
			return 1.0 / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 rsqrt(double4 x)
		{
			return 1.0 / math.sqrt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 normalize(float2 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 normalize(float3 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 normalize(float4 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 normalize(double2 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 normalize(double3 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 normalize(double4 x)
		{
			return math.rsqrt(math.dot(x, x)) * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 normalizesafe(float2 x, float2 defaultvalue = default(float2))
		{
			float num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754944E-38f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 normalizesafe(float3 x, float3 defaultvalue = default(float3))
		{
			float num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754944E-38f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 normalizesafe(float4 x, float4 defaultvalue = default(float4))
		{
			float num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754944E-38f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 normalizesafe(double2 x, double2 defaultvalue = default(double2))
		{
			double num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754943508222875E-38);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 normalizesafe(double3 x, double3 defaultvalue = default(double3))
		{
			double num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754943508222875E-38);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 normalizesafe(double4 x, double4 defaultvalue = default(double4))
		{
			double num = math.dot(x, x);
			return math.select(defaultvalue, x * math.rsqrt(num), num > 1.1754943508222875E-38);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float length(float x)
		{
			return math.abs(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float length(float2 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float length(float3 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float length(float4 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double length(double x)
		{
			return math.abs(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double length(double2 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double length(double3 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double length(double4 x)
		{
			return math.sqrt(math.dot(x, x));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lengthsq(float x)
		{
			return x * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lengthsq(float2 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lengthsq(float3 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lengthsq(float4 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double lengthsq(double x)
		{
			return x * x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double lengthsq(double2 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double lengthsq(double3 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double lengthsq(double4 x)
		{
			return math.dot(x, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distance(float x, float y)
		{
			return math.abs(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distance(float2 x, float2 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distance(float3 x, float3 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distance(float4 x, float4 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distance(double x, double y)
		{
			return math.abs(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distance(double2 x, double2 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distance(double3 x, double3 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distance(double4 x, double4 y)
		{
			return math.length(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distancesq(float x, float y)
		{
			return (y - x) * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distancesq(float2 x, float2 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distancesq(float3 x, float3 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float distancesq(float4 x, float4 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distancesq(double x, double y)
		{
			return (y - x) * (y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distancesq(double2 x, double2 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distancesq(double3 x, double3 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double distancesq(double4 x, double4 y)
		{
			return math.lengthsq(y - x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 cross(float3 x, float3 y)
		{
			return (x * y.yzx - x.yzx * y).yzx;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 cross(double3 x, double3 y)
		{
			return (x * y.yzx - x.yzx * y).yzx;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float smoothstep(float a, float b, float x)
		{
			float num = math.saturate((x - a) / (b - a));
			return num * num * (3f - 2f * num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 smoothstep(float2 a, float2 b, float2 x)
		{
			float2 @float = math.saturate((x - a) / (b - a));
			return @float * @float * (3f - 2f * @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 smoothstep(float3 a, float3 b, float3 x)
		{
			float3 @float = math.saturate((x - a) / (b - a));
			return @float * @float * (3f - 2f * @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 smoothstep(float4 a, float4 b, float4 x)
		{
			float4 @float = math.saturate((x - a) / (b - a));
			return @float * @float * (3f - 2f * @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double smoothstep(double a, double b, double x)
		{
			double num = math.saturate((x - a) / (b - a));
			return num * num * (3.0 - 2.0 * num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 smoothstep(double2 a, double2 b, double2 x)
		{
			double2 @double = math.saturate((x - a) / (b - a));
			return @double * @double * (3.0 - 2.0 * @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 smoothstep(double3 a, double3 b, double3 x)
		{
			double3 @double = math.saturate((x - a) / (b - a));
			return @double * @double * (3.0 - 2.0 * @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 smoothstep(double4 a, double4 b, double4 x)
		{
			double4 @double = math.saturate((x - a) / (b - a));
			return @double * @double * (3.0 - 2.0 * @double);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(bool2 x)
		{
			return x.x || x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(bool3 x)
		{
			return x.x || x.y || x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(bool4 x)
		{
			return x.x || x.y || x.z || x.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(int2 x)
		{
			return x.x != 0 || x.y != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(int3 x)
		{
			return x.x != 0 || x.y != 0 || x.z != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(int4 x)
		{
			return x.x != 0 || x.y != 0 || x.z != 0 || x.w != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(uint2 x)
		{
			return x.x != 0U || x.y > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(uint3 x)
		{
			return x.x != 0U || x.y != 0U || x.z > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(uint4 x)
		{
			return x.x != 0U || x.y != 0U || x.z != 0U || x.w > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(float2 x)
		{
			return x.x != 0f || x.y != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(float3 x)
		{
			return x.x != 0f || x.y != 0f || x.z != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(float4 x)
		{
			return x.x != 0f || x.y != 0f || x.z != 0f || x.w != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(double2 x)
		{
			return x.x != 0.0 || x.y != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(double3 x)
		{
			return x.x != 0.0 || x.y != 0.0 || x.z != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool any(double4 x)
		{
			return x.x != 0.0 || x.y != 0.0 || x.z != 0.0 || x.w != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(bool2 x)
		{
			return x.x && x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(bool3 x)
		{
			return x.x && x.y && x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(bool4 x)
		{
			return x.x && x.y && x.z && x.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(int2 x)
		{
			return x.x != 0 && x.y != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(int3 x)
		{
			return x.x != 0 && x.y != 0 && x.z != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(int4 x)
		{
			return x.x != 0 && x.y != 0 && x.z != 0 && x.w != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(uint2 x)
		{
			return x.x != 0U && x.y > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(uint3 x)
		{
			return x.x != 0U && x.y != 0U && x.z > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(uint4 x)
		{
			return x.x != 0U && x.y != 0U && x.z != 0U && x.w > 0U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(float2 x)
		{
			return x.x != 0f && x.y != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(float3 x)
		{
			return x.x != 0f && x.y != 0f && x.z != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(float4 x)
		{
			return x.x != 0f && x.y != 0f && x.z != 0f && x.w != 0f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(double2 x)
		{
			return x.x != 0.0 && x.y != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(double3 x)
		{
			return x.x != 0.0 && x.y != 0.0 && x.z != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool all(double4 x)
		{
			return x.x != 0.0 && x.y != 0.0 && x.z != 0.0 && x.w != 0.0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int select(int a, int b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 select(int2 a, int2 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 select(int3 a, int3 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 select(int4 a, int4 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 select(int2 a, int2 b, bool2 c)
		{
			return new int2(c.x ? b.x : a.x, c.y ? b.y : a.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 select(int3 a, int3 b, bool3 c)
		{
			return new int3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 select(int4 a, int4 b, bool4 c)
		{
			return new int4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint select(uint a, uint b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 select(uint2 a, uint2 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 select(uint3 a, uint3 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 select(uint4 a, uint4 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 select(uint2 a, uint2 b, bool2 c)
		{
			return new uint2(c.x ? b.x : a.x, c.y ? b.y : a.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 select(uint3 a, uint3 b, bool3 c)
		{
			return new uint3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 select(uint4 a, uint4 b, bool4 c)
		{
			return new uint4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long select(long a, long b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong select(ulong a, ulong b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float select(float a, float b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 select(float2 a, float2 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 select(float3 a, float3 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 select(float4 a, float4 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 select(float2 a, float2 b, bool2 c)
		{
			return new float2(c.x ? b.x : a.x, c.y ? b.y : a.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 select(float3 a, float3 b, bool3 c)
		{
			return new float3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 select(float4 a, float4 b, bool4 c)
		{
			return new float4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double select(double a, double b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 select(double2 a, double2 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 select(double3 a, double3 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 select(double4 a, double4 b, bool c)
		{
			if (!c)
			{
				return a;
			}
			return b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 select(double2 a, double2 b, bool2 c)
		{
			return new double2(c.x ? b.x : a.x, c.y ? b.y : a.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 select(double3 a, double3 b, bool3 c)
		{
			return new double3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 select(double4 a, double4 b, bool4 c)
		{
			return new double4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float step(float y, float x)
		{
			return math.select(0f, 1f, x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 step(float2 y, float2 x)
		{
			return math.select(math.float2(0f), math.float2(1f), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 step(float3 y, float3 x)
		{
			return math.select(math.float3(0f), math.float3(1f), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 step(float4 y, float4 x)
		{
			return math.select(math.float4(0f), math.float4(1f), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double step(double y, double x)
		{
			return math.select(0.0, 1.0, x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 step(double2 y, double2 x)
		{
			return math.select(math.double2(0.0), math.double2(1.0), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 step(double3 y, double3 x)
		{
			return math.select(math.double3(0.0), math.double3(1.0), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 step(double4 y, double4 x)
		{
			return math.select(math.double4(0.0), math.double4(1.0), x >= y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 reflect(float2 i, float2 n)
		{
			return i - 2f * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 reflect(float3 i, float3 n)
		{
			return i - 2f * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 reflect(float4 i, float4 n)
		{
			return i - 2f * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 reflect(double2 i, double2 n)
		{
			return i - 2.0 * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 reflect(double3 i, double3 n)
		{
			return i - 2.0 * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 reflect(double4 i, double4 n)
		{
			return i - 2.0 * n * math.dot(i, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 refract(float2 i, float2 n, float eta)
		{
			float num = math.dot(n, i);
			float num2 = 1f - eta * eta * (1f - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 refract(float3 i, float3 n, float eta)
		{
			float num = math.dot(n, i);
			float num2 = 1f - eta * eta * (1f - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 refract(float4 i, float4 n, float eta)
		{
			float num = math.dot(n, i);
			float num2 = 1f - eta * eta * (1f - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 refract(double2 i, double2 n, double eta)
		{
			double num = math.dot(n, i);
			double num2 = 1.0 - eta * eta * (1.0 - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 refract(double3 i, double3 n, double eta)
		{
			double num = math.dot(n, i);
			double num2 = 1.0 - eta * eta * (1.0 - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 refract(double4 i, double4 n, double eta)
		{
			double num = math.dot(n, i);
			double num2 = 1.0 - eta * eta * (1.0 - num * num);
			return math.select(0f, eta * i - (eta * num + math.sqrt(num2)) * n, num2 >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 project(float2 a, float2 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 project(float3 a, float3 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 project(float4 a, float4 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 projectsafe(float2 a, float2 b, float2 defaultValue = default(float2))
		{
			float2 @float = math.project(a, b);
			return math.select(defaultValue, @float, math.all(math.isfinite(@float)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 projectsafe(float3 a, float3 b, float3 defaultValue = default(float3))
		{
			float3 @float = math.project(a, b);
			return math.select(defaultValue, @float, math.all(math.isfinite(@float)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 projectsafe(float4 a, float4 b, float4 defaultValue = default(float4))
		{
			float4 @float = math.project(a, b);
			return math.select(defaultValue, @float, math.all(math.isfinite(@float)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 project(double2 a, double2 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 project(double3 a, double3 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 project(double4 a, double4 b)
		{
			return math.dot(a, b) / math.dot(b, b) * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 projectsafe(double2 a, double2 b, double2 defaultValue = default(double2))
		{
			double2 @double = math.project(a, b);
			return math.select(defaultValue, @double, math.all(math.isfinite(@double)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 projectsafe(double3 a, double3 b, double3 defaultValue = default(double3))
		{
			double3 @double = math.project(a, b);
			return math.select(defaultValue, @double, math.all(math.isfinite(@double)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 projectsafe(double4 a, double4 b, double4 defaultValue = default(double4))
		{
			double4 @double = math.project(a, b);
			return math.select(defaultValue, @double, math.all(math.isfinite(@double)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 faceforward(float2 n, float2 i, float2 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 faceforward(float3 n, float3 i, float3 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 faceforward(float4 n, float4 i, float4 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 faceforward(double2 n, double2 i, double2 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 faceforward(double3 n, double3 i, double3 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 faceforward(double4 n, double4 i, double4 ng)
		{
			return math.select(n, -n, math.dot(ng, i) >= 0.0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(float x, out float s, out float c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(float2 x, out float2 s, out float2 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(float3 x, out float3 s, out float3 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(float4 x, out float4 s, out float4 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(double x, out double s, out double c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(double2 x, out double2 s, out double2 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(double3 x, out double3 s, out double3 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void sincos(double4 x, out double4 s, out double4 c)
		{
			s = math.sin(x);
			c = math.cos(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int countbits(int x)
		{
			return math.countbits((uint)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 countbits(int2 x)
		{
			return math.countbits((uint2)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 countbits(int3 x)
		{
			return math.countbits((uint3)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 countbits(int4 x)
		{
			return math.countbits((uint4)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int countbits(uint x)
		{
			x -= (x >> 1) & 1431655765U;
			x = (x & 858993459U) + ((x >> 2) & 858993459U);
			return (int)(((x + (x >> 4)) & 252645135U) * 16843009U >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 countbits(uint2 x)
		{
			x -= (x >> 1) & 1431655765U;
			x = (x & 858993459U) + ((x >> 2) & 858993459U);
			return math.int2(((x + (x >> 4)) & 252645135U) * 16843009U >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 countbits(uint3 x)
		{
			x -= (x >> 1) & 1431655765U;
			x = (x & 858993459U) + ((x >> 2) & 858993459U);
			return math.int3(((x + (x >> 4)) & 252645135U) * 16843009U >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 countbits(uint4 x)
		{
			x -= (x >> 1) & 1431655765U;
			x = (x & 858993459U) + ((x >> 2) & 858993459U);
			return math.int4(((x + (x >> 4)) & 252645135U) * 16843009U >> 24);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int countbits(ulong x)
		{
			x -= (x >> 1) & 6148914691236517205UL;
			x = (x & 3689348814741910323UL) + ((x >> 2) & 3689348814741910323UL);
			return (int)(((x + (x >> 4)) & 1085102592571150095UL) * 72340172838076673UL >> 56);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int countbits(long x)
		{
			return math.countbits((ulong)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int lzcnt(int x)
		{
			return math.lzcnt((uint)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 lzcnt(int2 x)
		{
			return math.int2(math.lzcnt(x.x), math.lzcnt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 lzcnt(int3 x)
		{
			return math.int3(math.lzcnt(x.x), math.lzcnt(x.y), math.lzcnt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 lzcnt(int4 x)
		{
			return math.int4(math.lzcnt(x.x), math.lzcnt(x.y), math.lzcnt(x.z), math.lzcnt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int lzcnt(uint x)
		{
			if (x == 0U)
			{
				return 32;
			}
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.doubleValue = 0.0;
			longDoubleUnion.longValue = (long)(4841369599423283200UL + (ulong)x);
			longDoubleUnion.doubleValue -= 4503599627370496.0;
			return 1054 - (int)(longDoubleUnion.longValue >> 52);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 lzcnt(uint2 x)
		{
			return math.int2(math.lzcnt(x.x), math.lzcnt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 lzcnt(uint3 x)
		{
			return math.int3(math.lzcnt(x.x), math.lzcnt(x.y), math.lzcnt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 lzcnt(uint4 x)
		{
			return math.int4(math.lzcnt(x.x), math.lzcnt(x.y), math.lzcnt(x.z), math.lzcnt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int lzcnt(long x)
		{
			return math.lzcnt((ulong)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int lzcnt(ulong x)
		{
			if (x == 0UL)
			{
				return 64;
			}
			uint num = (uint)(x >> 32);
			uint num2 = ((num != 0U) ? num : ((uint)x));
			int num3 = ((num != 0U) ? 1054 : 1086);
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.doubleValue = 0.0;
			longDoubleUnion.longValue = (long)(4841369599423283200UL + (ulong)num2);
			longDoubleUnion.doubleValue -= 4503599627370496.0;
			return num3 - (int)(longDoubleUnion.longValue >> 52);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int tzcnt(int x)
		{
			return math.tzcnt((uint)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 tzcnt(int2 x)
		{
			return math.int2(math.tzcnt(x.x), math.tzcnt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 tzcnt(int3 x)
		{
			return math.int3(math.tzcnt(x.x), math.tzcnt(x.y), math.tzcnt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 tzcnt(int4 x)
		{
			return math.int4(math.tzcnt(x.x), math.tzcnt(x.y), math.tzcnt(x.z), math.tzcnt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int tzcnt(uint x)
		{
			if (x == 0U)
			{
				return 32;
			}
			x &= (uint)(-(uint)((ulong)x));
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.doubleValue = 0.0;
			longDoubleUnion.longValue = (long)(4841369599423283200UL + (ulong)x);
			longDoubleUnion.doubleValue -= 4503599627370496.0;
			return (int)(longDoubleUnion.longValue >> 52) - 1023;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 tzcnt(uint2 x)
		{
			return math.int2(math.tzcnt(x.x), math.tzcnt(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 tzcnt(uint3 x)
		{
			return math.int3(math.tzcnt(x.x), math.tzcnt(x.y), math.tzcnt(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 tzcnt(uint4 x)
		{
			return math.int4(math.tzcnt(x.x), math.tzcnt(x.y), math.tzcnt(x.z), math.tzcnt(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int tzcnt(long x)
		{
			return math.tzcnt((ulong)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int tzcnt(ulong x)
		{
			if (x == 0UL)
			{
				return 64;
			}
			x &= -x;
			uint num = (uint)x;
			uint num2 = ((num != 0U) ? num : ((uint)(x >> 32)));
			int num3 = ((num != 0U) ? 1023 : 991);
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.doubleValue = 0.0;
			longDoubleUnion.longValue = (long)(4841369599423283200UL + (ulong)num2);
			longDoubleUnion.doubleValue -= 4503599627370496.0;
			return (int)(longDoubleUnion.longValue >> 52) - num3;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int reversebits(int x)
		{
			return (int)math.reversebits((uint)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 reversebits(int2 x)
		{
			return (int2)math.reversebits((uint2)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 reversebits(int3 x)
		{
			return (int3)math.reversebits((uint3)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 reversebits(int4 x)
		{
			return (int4)math.reversebits((uint4)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint reversebits(uint x)
		{
			x = ((x >> 1) & 1431655765U) | ((x & 1431655765U) << 1);
			x = ((x >> 2) & 858993459U) | ((x & 858993459U) << 2);
			x = ((x >> 4) & 252645135U) | ((x & 252645135U) << 4);
			x = ((x >> 8) & 16711935U) | ((x & 16711935U) << 8);
			return (x >> 16) | (x << 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 reversebits(uint2 x)
		{
			x = ((x >> 1) & 1431655765U) | ((x & 1431655765U) << 1);
			x = ((x >> 2) & 858993459U) | ((x & 858993459U) << 2);
			x = ((x >> 4) & 252645135U) | ((x & 252645135U) << 4);
			x = ((x >> 8) & 16711935U) | ((x & 16711935U) << 8);
			return (x >> 16) | (x << 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 reversebits(uint3 x)
		{
			x = ((x >> 1) & 1431655765U) | ((x & 1431655765U) << 1);
			x = ((x >> 2) & 858993459U) | ((x & 858993459U) << 2);
			x = ((x >> 4) & 252645135U) | ((x & 252645135U) << 4);
			x = ((x >> 8) & 16711935U) | ((x & 16711935U) << 8);
			return (x >> 16) | (x << 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 reversebits(uint4 x)
		{
			x = ((x >> 1) & 1431655765U) | ((x & 1431655765U) << 1);
			x = ((x >> 2) & 858993459U) | ((x & 858993459U) << 2);
			x = ((x >> 4) & 252645135U) | ((x & 252645135U) << 4);
			x = ((x >> 8) & 16711935U) | ((x & 16711935U) << 8);
			return (x >> 16) | (x << 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long reversebits(long x)
		{
			return (long)math.reversebits((ulong)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong reversebits(ulong x)
		{
			x = ((x >> 1) & 6148914691236517205UL) | ((x & 6148914691236517205UL) << 1);
			x = ((x >> 2) & 3689348814741910323UL) | ((x & 3689348814741910323UL) << 2);
			x = ((x >> 4) & 1085102592571150095UL) | ((x & 1085102592571150095UL) << 4);
			x = ((x >> 8) & 71777214294589695UL) | ((x & 71777214294589695UL) << 8);
			x = ((x >> 16) & 281470681808895UL) | ((x & 281470681808895UL) << 16);
			return (x >> 32) | (x << 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int rol(int x, int n)
		{
			return (int)math.rol((uint)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 rol(int2 x, int n)
		{
			return (int2)math.rol((uint2)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 rol(int3 x, int n)
		{
			return (int3)math.rol((uint3)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 rol(int4 x, int n)
		{
			return (int4)math.rol((uint4)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint rol(uint x, int n)
		{
			return (x << n) | (x >> 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 rol(uint2 x, int n)
		{
			return (x << n) | (x >> 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 rol(uint3 x, int n)
		{
			return (x << n) | (x >> 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 rol(uint4 x, int n)
		{
			return (x << n) | (x >> 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long rol(long x, int n)
		{
			return (long)math.rol((ulong)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong rol(ulong x, int n)
		{
			return (x << n) | (x >> 64 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ror(int x, int n)
		{
			return (int)math.ror((uint)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 ror(int2 x, int n)
		{
			return (int2)math.ror((uint2)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ror(int3 x, int n)
		{
			return (int3)math.ror((uint3)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 ror(int4 x, int n)
		{
			return (int4)math.ror((uint4)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ror(uint x, int n)
		{
			return (x >> n) | (x << 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 ror(uint2 x, int n)
		{
			return (x >> n) | (x << 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 ror(uint3 x, int n)
		{
			return (x >> n) | (x << 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 ror(uint4 x, int n)
		{
			return (x >> n) | (x << 32 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ror(long x, int n)
		{
			return (long)math.ror((ulong)x, n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ror(ulong x, int n)
		{
			return (x >> n) | (x << 64 - n);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ceilpow2(int x)
		{
			x--;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 ceilpow2(int2 x)
		{
			x -= 1;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ceilpow2(int3 x)
		{
			x -= 1;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 ceilpow2(int4 x)
		{
			x -= 1;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint ceilpow2(uint x)
		{
			x -= 1U;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 ceilpow2(uint2 x)
		{
			x -= 1U;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 ceilpow2(uint3 x)
		{
			x -= 1U;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 ceilpow2(uint4 x)
		{
			x -= 1U;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			return x + 1U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ceilpow2(long x)
		{
			x -= 1L;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x |= x >> 32;
			return x + 1L;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong ceilpow2(ulong x)
		{
			x -= 1UL;
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x |= x >> 32;
			return x + 1UL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ceillog2(int x)
		{
			return 32 - math.lzcnt((uint)(x - 1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 ceillog2(int2 x)
		{
			return new int2(math.ceillog2(x.x), math.ceillog2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ceillog2(int3 x)
		{
			return new int3(math.ceillog2(x.x), math.ceillog2(x.y), math.ceillog2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 ceillog2(int4 x)
		{
			return new int4(math.ceillog2(x.x), math.ceillog2(x.y), math.ceillog2(x.z), math.ceillog2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ceillog2(uint x)
		{
			return 32 - math.lzcnt(x - 1U);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 ceillog2(uint2 x)
		{
			return new int2(math.ceillog2(x.x), math.ceillog2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 ceillog2(uint3 x)
		{
			return new int3(math.ceillog2(x.x), math.ceillog2(x.y), math.ceillog2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 ceillog2(uint4 x)
		{
			return new int4(math.ceillog2(x.x), math.ceillog2(x.y), math.ceillog2(x.z), math.ceillog2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int floorlog2(int x)
		{
			return 31 - math.lzcnt((uint)x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 floorlog2(int2 x)
		{
			return new int2(math.floorlog2(x.x), math.floorlog2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 floorlog2(int3 x)
		{
			return new int3(math.floorlog2(x.x), math.floorlog2(x.y), math.floorlog2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 floorlog2(int4 x)
		{
			return new int4(math.floorlog2(x.x), math.floorlog2(x.y), math.floorlog2(x.z), math.floorlog2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int floorlog2(uint x)
		{
			return 31 - math.lzcnt(x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 floorlog2(uint2 x)
		{
			return new int2(math.floorlog2(x.x), math.floorlog2(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 floorlog2(uint3 x)
		{
			return new int3(math.floorlog2(x.x), math.floorlog2(x.y), math.floorlog2(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 floorlog2(uint4 x)
		{
			return new int4(math.floorlog2(x.x), math.floorlog2(x.y), math.floorlog2(x.z), math.floorlog2(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float radians(float x)
		{
			return x * 0.017453292f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 radians(float2 x)
		{
			return x * 0.017453292f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 radians(float3 x)
		{
			return x * 0.017453292f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 radians(float4 x)
		{
			return x * 0.017453292f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double radians(double x)
		{
			return x * 0.017453292519943295;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 radians(double2 x)
		{
			return x * 0.017453292519943295;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 radians(double3 x)
		{
			return x * 0.017453292519943295;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 radians(double4 x)
		{
			return x * 0.017453292519943295;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float degrees(float x)
		{
			return x * 57.29578f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 degrees(float2 x)
		{
			return x * 57.29578f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 degrees(float3 x)
		{
			return x * 57.29578f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 degrees(float4 x)
		{
			return x * 57.29578f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double degrees(double x)
		{
			return x * 57.29577951308232;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 degrees(double2 x)
		{
			return x * 57.29577951308232;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 degrees(double3 x)
		{
			return x * 57.29577951308232;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 degrees(double4 x)
		{
			return x * 57.29577951308232;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmin(int2 x)
		{
			return math.min(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmin(int3 x)
		{
			return math.min(math.min(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmin(int4 x)
		{
			return math.min(math.min(x.x, x.y), math.min(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmin(uint2 x)
		{
			return math.min(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmin(uint3 x)
		{
			return math.min(math.min(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmin(uint4 x)
		{
			return math.min(math.min(x.x, x.y), math.min(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmin(float2 x)
		{
			return math.min(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmin(float3 x)
		{
			return math.min(math.min(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmin(float4 x)
		{
			return math.min(math.min(x.x, x.y), math.min(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmin(double2 x)
		{
			return math.min(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmin(double3 x)
		{
			return math.min(math.min(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmin(double4 x)
		{
			return math.min(math.min(x.x, x.y), math.min(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmax(int2 x)
		{
			return math.max(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmax(int3 x)
		{
			return math.max(math.max(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int cmax(int4 x)
		{
			return math.max(math.max(x.x, x.y), math.max(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmax(uint2 x)
		{
			return math.max(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmax(uint3 x)
		{
			return math.max(math.max(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint cmax(uint4 x)
		{
			return math.max(math.max(x.x, x.y), math.max(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmax(float2 x)
		{
			return math.max(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmax(float3 x)
		{
			return math.max(math.max(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float cmax(float4 x)
		{
			return math.max(math.max(x.x, x.y), math.max(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmax(double2 x)
		{
			return math.max(x.x, x.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmax(double3 x)
		{
			return math.max(math.max(x.x, x.y), x.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double cmax(double4 x)
		{
			return math.max(math.max(x.x, x.y), math.max(x.z, x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int csum(int2 x)
		{
			return x.x + x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int csum(int3 x)
		{
			return x.x + x.y + x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int csum(int4 x)
		{
			return x.x + x.y + x.z + x.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint csum(uint2 x)
		{
			return x.x + x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint csum(uint3 x)
		{
			return x.x + x.y + x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint csum(uint4 x)
		{
			return x.x + x.y + x.z + x.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float csum(float2 x)
		{
			return x.x + x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float csum(float3 x)
		{
			return x.x + x.y + x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float csum(float4 x)
		{
			return x.x + x.y + (x.z + x.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double csum(double2 x)
		{
			return x.x + x.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double csum(double3 x)
		{
			return x.x + x.y + x.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double csum(double4 x)
		{
			return x.x + x.y + (x.z + x.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int compress(int* output, int index, int4 val, bool4 mask)
		{
			if (mask.x)
			{
				output[index++] = val.x;
			}
			if (mask.y)
			{
				output[index++] = val.y;
			}
			if (mask.z)
			{
				output[index++] = val.z;
			}
			if (mask.w)
			{
				output[index++] = val.w;
			}
			return index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int compress(uint* output, int index, uint4 val, bool4 mask)
		{
			return math.compress((int*)output, index, *(int4*)(&val), mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static int compress(float* output, int index, float4 val, bool4 mask)
		{
			return math.compress((int*)output, index, *(int4*)(&val), mask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float f16tof32(uint x)
		{
			uint num = (x & 32767U) << 13;
			uint num2 = num & 260046848U;
			uint num3 = num + 939524096U + math.select(0U, 939524096U, num2 == 260046848U);
			return math.asfloat(math.select(num3, math.asuint(math.asfloat(num3 + 8388608U) - 6.1035156E-05f), num2 == 0U) | ((x & 32768U) << 16));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 f16tof32(uint2 x)
		{
			uint2 @uint = (x & 32767U) << 13;
			uint2 uint2 = @uint & 260046848U;
			uint2 uint3 = @uint + 939524096U + math.select(0U, 939524096U, uint2 == 260046848U);
			return math.asfloat(math.select(uint3, math.asuint(math.asfloat(uint3 + 8388608U) - 6.1035156E-05f), uint2 == 0U) | ((x & 32768U) << 16));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 f16tof32(uint3 x)
		{
			uint3 @uint = (x & 32767U) << 13;
			uint3 uint2 = @uint & 260046848U;
			uint3 uint3 = @uint + 939524096U + math.select(0U, 939524096U, uint2 == 260046848U);
			return math.asfloat(math.select(uint3, math.asuint(math.asfloat(uint3 + 8388608U) - 6.1035156E-05f), uint2 == 0U) | ((x & 32768U) << 16));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 f16tof32(uint4 x)
		{
			uint4 @uint = (x & 32767U) << 13;
			uint4 uint2 = @uint & 260046848U;
			uint4 uint3 = @uint + 939524096U + math.select(0U, 939524096U, uint2 == 260046848U);
			return math.asfloat(math.select(uint3, math.asuint(math.asfloat(uint3 + 8388608U) - 6.1035156E-05f), uint2 == 0U) | ((x & 32768U) << 16));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint f32tof16(float x)
		{
			uint num = math.asuint(x);
			uint num2 = num & 2147479552U;
			return math.select(math.asuint(math.min(math.asfloat(num2) * 1.92593E-34f, 260042750f)) + 4096U >> 13, math.select(31744U, 32256U, num2 > 2139095040U), num2 >= 2139095040U) | ((num & 2147487743U) >> 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 f32tof16(float2 x)
		{
			uint2 @uint = math.asuint(x);
			uint2 uint2 = @uint & 2147479552U;
			return math.select((uint2)(math.asint(math.min(math.asfloat(uint2) * 1.92593E-34f, 260042750f)) + 4096) >> 13, math.select(31744U, 32256U, (int2)uint2 > 2139095040), (int2)uint2 >= 2139095040) | ((@uint & 2147487743U) >> 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 f32tof16(float3 x)
		{
			uint3 @uint = math.asuint(x);
			uint3 uint2 = @uint & 2147479552U;
			return math.select((uint3)(math.asint(math.min(math.asfloat(uint2) * 1.92593E-34f, 260042750f)) + 4096) >> 13, math.select(31744U, 32256U, (int3)uint2 > 2139095040), (int3)uint2 >= 2139095040) | ((@uint & 2147487743U) >> 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 f32tof16(float4 x)
		{
			uint4 @uint = math.asuint(x);
			uint4 uint2 = @uint & 2147479552U;
			return math.select((uint4)(math.asint(math.min(math.asfloat(uint2) * 1.92593E-34f, 260042750f)) + 4096) >> 13, math.select(31744U, 32256U, (int4)uint2 > 2139095040), (int4)uint2 >= 2139095040) | ((@uint & 2147487743U) >> 16);
		}

		public unsafe static uint hash(void* pBuffer, int numBytes, uint seed = 0U)
		{
			uint4* ptr = (uint4*)pBuffer;
			uint num = seed + 374761393U;
			if (numBytes >= 16)
			{
				uint4 @uint = new uint4(606290984U, 2246822519U, 0U, 1640531535U) + seed;
				int num2 = numBytes >> 4;
				for (int i = 0; i < num2; i++)
				{
					@uint += *(ptr++) * 2246822519U;
					@uint = (@uint << 13) | (@uint >> 19);
					@uint *= 2654435761U;
				}
				num = math.rol(@uint.x, 1) + math.rol(@uint.y, 7) + math.rol(@uint.z, 12) + math.rol(@uint.w, 18);
			}
			num += (uint)numBytes;
			uint* ptr2 = (uint*)ptr;
			for (int j = 0; j < ((numBytes >> 2) & 3); j++)
			{
				num += *(ptr2++) * 3266489917U;
				num = math.rol(num, 17) * 668265263U;
			}
			byte* ptr3 = (byte*)ptr2;
			for (int k = 0; k < (numBytes & 3); k++)
			{
				num += (uint)(*(ptr3++)) * 374761393U;
				num = math.rol(num, 11) * 2654435761U;
			}
			num ^= num >> 15;
			num *= 2246822519U;
			num ^= num >> 13;
			num *= 3266489917U;
			return num ^ (num >> 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 up()
		{
			return new float3(0f, 1f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 down()
		{
			return new float3(0f, -1f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 forward()
		{
			return new float3(0f, 0f, 1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 back()
		{
			return new float3(0f, 0f, -1f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 left()
		{
			return new float3(-1f, 0f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 right()
		{
			return new float3(1f, 0f, 0f);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float4 unpacklo(float4 a, float4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double4 unpacklo(double4 a, double4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftX, math.ShuffleComponent.RightX, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightY);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float4 unpackhi(float4 a, float4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double4 unpackhi(double4 a, double4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftZ, math.ShuffleComponent.RightZ, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightW);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float4 movelh(float4 a, float4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightX, math.ShuffleComponent.RightY);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double4 movelh(double4 a, double4 b)
		{
			return math.shuffle(a, b, math.ShuffleComponent.LeftX, math.ShuffleComponent.LeftY, math.ShuffleComponent.RightX, math.ShuffleComponent.RightY);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static float4 movehl(float4 a, float4 b)
		{
			return math.shuffle(b, a, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightW);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static double4 movehl(double4 a, double4 b)
		{
			return math.shuffle(b, a, math.ShuffleComponent.LeftZ, math.ShuffleComponent.LeftW, math.ShuffleComponent.RightZ, math.ShuffleComponent.RightW);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint fold_to_uint(double x)
		{
			math.LongDoubleUnion longDoubleUnion;
			longDoubleUnion.longValue = 0L;
			longDoubleUnion.doubleValue = x;
			return (uint)(longDoubleUnion.longValue >> 32) ^ (uint)longDoubleUnion.longValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint2 fold_to_uint(double2 x)
		{
			return math.uint2(math.fold_to_uint(x.x), math.fold_to_uint(x.y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint3 fold_to_uint(double3 x)
		{
			return math.uint3(math.fold_to_uint(x.x), math.fold_to_uint(x.y), math.fold_to_uint(x.z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint4 fold_to_uint(double4 x)
		{
			return math.uint4(math.fold_to_uint(x.x), math.fold_to_uint(x.y), math.fold_to_uint(x.z), math.fold_to_uint(x.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(float4x4 f4x4)
		{
			return new float3x3(f4x4);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 float3x3(quaternion rotation)
		{
			return new float3x3(rotation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(float3x3 rotation, float3 translation)
		{
			return new float4x4(rotation, translation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(quaternion rotation, float3 translation)
		{
			return new float4x4(rotation, translation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 float4x4(RigidTransform transform)
		{
			return new float4x4(transform);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 orthonormalize(float3x3 i)
		{
			float3 c = i.c0;
			float3 @float = i.c1 - i.c0 * math.dot(i.c1, i.c0);
			float num = math.length(c);
			float num2 = math.length(@float);
			bool flag = num > 1E-30f && num2 > 1E-30f;
			float3x3 float3x;
			float3x.c0 = math.select(math.float3(1f, 0f, 0f), c / num, flag);
			float3x.c1 = math.select(math.float3(0f, 1f, 0f), @float / num2, flag);
			float3x.c2 = math.cross(float3x.c0, float3x.c1);
			return float3x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float mul(float a, float b)
		{
			return a * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float mul(float2 a, float2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float2 a, float2x2 b)
		{
			return math.float2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float2 a, float2x3 b)
		{
			return math.float3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float2 a, float2x4 b)
		{
			return math.float4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float mul(float3 a, float3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float3 a, float3x2 b)
		{
			return math.float2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float3 a, float3x3 b)
		{
			return math.float3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float3 a, float3x4 b)
		{
			return math.float4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float mul(float4 a, float4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float4 a, float4x2 b)
		{
			return math.float2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float4 a, float4x3 b)
		{
			return math.float3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float4 a, float4x4 b)
		{
			return math.float4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float2x2 a, float2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 mul(float2x2 a, float2x2 b)
		{
			return math.float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 mul(float2x2 a, float2x3 b)
		{
			return math.float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 mul(float2x2 a, float2x4 b)
		{
			return math.float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float2x3 a, float3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 mul(float2x3 a, float3x2 b)
		{
			return math.float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 mul(float2x3 a, float3x3 b)
		{
			return math.float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 mul(float2x3 a, float3x4 b)
		{
			return math.float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2 mul(float2x4 a, float4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x2 mul(float2x4 a, float4x2 b)
		{
			return math.float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x3 mul(float2x4 a, float4x3 b)
		{
			return math.float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float2x4 mul(float2x4 a, float4x4 b)
		{
			return math.float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float3x2 a, float2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 mul(float3x2 a, float2x2 b)
		{
			return math.float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 mul(float3x2 a, float2x3 b)
		{
			return math.float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 mul(float3x2 a, float2x4 b)
		{
			return math.float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float3x3 a, float3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 mul(float3x3 a, float3x2 b)
		{
			return math.float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 mul(float3x3 a, float3x3 b)
		{
			return math.float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 mul(float3x3 a, float3x4 b)
		{
			return math.float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(float3x4 a, float4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x2 mul(float3x4 a, float4x2 b)
		{
			return math.float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x3 mul(float3x4 a, float4x3 b)
		{
			return math.float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3x4 mul(float3x4 a, float4x4 b)
		{
			return math.float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float4x2 a, float2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 mul(float4x2 a, float2x2 b)
		{
			return math.float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 mul(float4x2 a, float2x3 b)
		{
			return math.float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 mul(float4x2 a, float2x4 b)
		{
			return math.float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float4x3 a, float3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 mul(float4x3 a, float3x2 b)
		{
			return math.float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 mul(float4x3 a, float3x3 b)
		{
			return math.float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 mul(float4x3 a, float3x4 b)
		{
			return math.float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(float4x4 a, float4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x2 mul(float4x4 a, float4x2 b)
		{
			return math.float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x3 mul(float4x4 a, float4x3 b)
		{
			return math.float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4x4 mul(float4x4 a, float4x4 b)
		{
			return math.float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double mul(double a, double b)
		{
			return a * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double mul(double2 a, double2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double2 a, double2x2 b)
		{
			return math.double2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double2 a, double2x3 b)
		{
			return math.double3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double2 a, double2x4 b)
		{
			return math.double4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double mul(double3 a, double3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double3 a, double3x2 b)
		{
			return math.double2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double3 a, double3x3 b)
		{
			return math.double3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double3 a, double3x4 b)
		{
			return math.double4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double mul(double4 a, double4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double4 a, double4x2 b)
		{
			return math.double2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double4 a, double4x3 b)
		{
			return math.double3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double4 a, double4x4 b)
		{
			return math.double4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double2x2 a, double2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 mul(double2x2 a, double2x2 b)
		{
			return math.double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 mul(double2x2 a, double2x3 b)
		{
			return math.double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 mul(double2x2 a, double2x4 b)
		{
			return math.double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double2x3 a, double3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 mul(double2x3 a, double3x2 b)
		{
			return math.double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 mul(double2x3 a, double3x3 b)
		{
			return math.double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 mul(double2x3 a, double3x4 b)
		{
			return math.double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2 mul(double2x4 a, double4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x2 mul(double2x4 a, double4x2 b)
		{
			return math.double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x3 mul(double2x4 a, double4x3 b)
		{
			return math.double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double2x4 mul(double2x4 a, double4x4 b)
		{
			return math.double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double3x2 a, double2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 mul(double3x2 a, double2x2 b)
		{
			return math.double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 mul(double3x2 a, double2x3 b)
		{
			return math.double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 mul(double3x2 a, double2x4 b)
		{
			return math.double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double3x3 a, double3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 mul(double3x3 a, double3x2 b)
		{
			return math.double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 mul(double3x3 a, double3x3 b)
		{
			return math.double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 mul(double3x3 a, double3x4 b)
		{
			return math.double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3 mul(double3x4 a, double4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x2 mul(double3x4 a, double4x2 b)
		{
			return math.double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x3 mul(double3x4 a, double4x3 b)
		{
			return math.double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double3x4 mul(double3x4 a, double4x4 b)
		{
			return math.double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double4x2 a, double2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 mul(double4x2 a, double2x2 b)
		{
			return math.double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 mul(double4x2 a, double2x3 b)
		{
			return math.double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 mul(double4x2 a, double2x4 b)
		{
			return math.double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double4x3 a, double3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 mul(double4x3 a, double3x2 b)
		{
			return math.double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 mul(double4x3 a, double3x3 b)
		{
			return math.double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 mul(double4x3 a, double3x4 b)
		{
			return math.double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4 mul(double4x4 a, double4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x2 mul(double4x4 a, double4x2 b)
		{
			return math.double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x3 mul(double4x4 a, double4x3 b)
		{
			return math.double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double4x4 mul(double4x4 a, double4x4 b)
		{
			return math.double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int mul(int a, int b)
		{
			return a * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int mul(int2 a, int2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int2 a, int2x2 b)
		{
			return math.int2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int2 a, int2x3 b)
		{
			return math.int3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int2 a, int2x4 b)
		{
			return math.int4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int mul(int3 a, int3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int3 a, int3x2 b)
		{
			return math.int2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int3 a, int3x3 b)
		{
			return math.int3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int3 a, int3x4 b)
		{
			return math.int4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int mul(int4 a, int4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int4 a, int4x2 b)
		{
			return math.int2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int4 a, int4x3 b)
		{
			return math.int3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int4 a, int4x4 b)
		{
			return math.int4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int2x2 a, int2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 mul(int2x2 a, int2x2 b)
		{
			return math.int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 mul(int2x2 a, int2x3 b)
		{
			return math.int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 mul(int2x2 a, int2x4 b)
		{
			return math.int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int2x3 a, int3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 mul(int2x3 a, int3x2 b)
		{
			return math.int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 mul(int2x3 a, int3x3 b)
		{
			return math.int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 mul(int2x3 a, int3x4 b)
		{
			return math.int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2 mul(int2x4 a, int4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x2 mul(int2x4 a, int4x2 b)
		{
			return math.int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x3 mul(int2x4 a, int4x3 b)
		{
			return math.int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int2x4 mul(int2x4 a, int4x4 b)
		{
			return math.int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int3x2 a, int2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 mul(int3x2 a, int2x2 b)
		{
			return math.int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 mul(int3x2 a, int2x3 b)
		{
			return math.int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 mul(int3x2 a, int2x4 b)
		{
			return math.int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int3x3 a, int3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 mul(int3x3 a, int3x2 b)
		{
			return math.int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 mul(int3x3 a, int3x3 b)
		{
			return math.int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 mul(int3x3 a, int3x4 b)
		{
			return math.int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3 mul(int3x4 a, int4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x2 mul(int3x4 a, int4x2 b)
		{
			return math.int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x3 mul(int3x4 a, int4x3 b)
		{
			return math.int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int3x4 mul(int3x4 a, int4x4 b)
		{
			return math.int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int4x2 a, int2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 mul(int4x2 a, int2x2 b)
		{
			return math.int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 mul(int4x2 a, int2x3 b)
		{
			return math.int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 mul(int4x2 a, int2x4 b)
		{
			return math.int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int4x3 a, int3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 mul(int4x3 a, int3x2 b)
		{
			return math.int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 mul(int4x3 a, int3x3 b)
		{
			return math.int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 mul(int4x3 a, int3x4 b)
		{
			return math.int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4 mul(int4x4 a, int4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x2 mul(int4x4 a, int4x2 b)
		{
			return math.int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x3 mul(int4x4 a, int4x3 b)
		{
			return math.int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int4x4 mul(int4x4 a, int4x4 b)
		{
			return math.int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint mul(uint a, uint b)
		{
			return a * b;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint mul(uint2 a, uint2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint2 a, uint2x2 b)
		{
			return math.uint2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint2 a, uint2x3 b)
		{
			return math.uint3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint2 a, uint2x4 b)
		{
			return math.uint4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint mul(uint3 a, uint3 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint3 a, uint3x2 b)
		{
			return math.uint2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint3 a, uint3x3 b)
		{
			return math.uint3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint3 a, uint3x4 b)
		{
			return math.uint4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint mul(uint4 a, uint4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint4 a, uint4x2 b)
		{
			return math.uint2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint4 a, uint4x3 b)
		{
			return math.uint3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint4 a, uint4x4 b)
		{
			return math.uint4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint2x2 a, uint2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 mul(uint2x2 a, uint2x2 b)
		{
			return math.uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 mul(uint2x2 a, uint2x3 b)
		{
			return math.uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 mul(uint2x2 a, uint2x4 b)
		{
			return math.uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint2x3 a, uint3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 mul(uint2x3 a, uint3x2 b)
		{
			return math.uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 mul(uint2x3 a, uint3x3 b)
		{
			return math.uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 mul(uint2x3 a, uint3x4 b)
		{
			return math.uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 mul(uint2x4 a, uint4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 mul(uint2x4 a, uint4x2 b)
		{
			return math.uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 mul(uint2x4 a, uint4x3 b)
		{
			return math.uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 mul(uint2x4 a, uint4x4 b)
		{
			return math.uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint3x2 a, uint2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 mul(uint3x2 a, uint2x2 b)
		{
			return math.uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 mul(uint3x2 a, uint2x3 b)
		{
			return math.uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 mul(uint3x2 a, uint2x4 b)
		{
			return math.uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint3x3 a, uint3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 mul(uint3x3 a, uint3x2 b)
		{
			return math.uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 mul(uint3x3 a, uint3x3 b)
		{
			return math.uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 mul(uint3x3 a, uint3x4 b)
		{
			return math.uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 mul(uint3x4 a, uint4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 mul(uint3x4 a, uint4x2 b)
		{
			return math.uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 mul(uint3x4 a, uint4x3 b)
		{
			return math.uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 mul(uint3x4 a, uint4x4 b)
		{
			return math.uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint4x2 a, uint2 b)
		{
			return a.c0 * b.x + a.c1 * b.y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 mul(uint4x2 a, uint2x2 b)
		{
			return math.uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 mul(uint4x2 a, uint2x3 b)
		{
			return math.uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 mul(uint4x2 a, uint2x4 b)
		{
			return math.uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint4x3 a, uint3 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 mul(uint4x3 a, uint3x2 b)
		{
			return math.uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 mul(uint4x3 a, uint3x3 b)
		{
			return math.uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 mul(uint4x3 a, uint3x4 b)
		{
			return math.uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 mul(uint4x4 a, uint4 b)
		{
			return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 mul(uint4x4 a, uint4x2 b)
		{
			return math.uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 mul(uint4x4 a, uint4x3 b)
		{
			return math.uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 mul(uint4x4 a, uint4x4 b)
		{
			return math.uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion quaternion(float x, float y, float z, float w)
		{
			return new quaternion(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion quaternion(float4 value)
		{
			return new quaternion(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion quaternion(float3x3 m)
		{
			return new quaternion(m);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion quaternion(float4x4 m)
		{
			return new quaternion(m);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion conjugate(quaternion q)
		{
			return math.quaternion(q.value * math.float4(-1f, -1f, -1f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion inverse(quaternion q)
		{
			float4 value = q.value;
			return math.quaternion(math.rcp(math.dot(value, value)) * value * math.float4(-1f, -1f, -1f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float dot(quaternion a, quaternion b)
		{
			return math.dot(a.value, b.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float length(quaternion q)
		{
			return math.sqrt(math.dot(q.value, q.value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float lengthsq(quaternion q)
		{
			return math.dot(q.value, q.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion normalize(quaternion q)
		{
			float4 value = q.value;
			return math.quaternion(math.rsqrt(math.dot(value, value)) * value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion normalizesafe(quaternion q)
		{
			float4 value = q.value;
			float num = math.dot(value, value);
			return math.quaternion(math.select(Unity.Mathematics.quaternion.identity.value, value * math.rsqrt(num), num > 1.1754944E-38f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion normalizesafe(quaternion q, quaternion defaultvalue)
		{
			float4 value = q.value;
			float num = math.dot(value, value);
			return math.quaternion(math.select(defaultvalue.value, value * math.rsqrt(num), num > 1.1754944E-38f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion unitexp(quaternion q)
		{
			float num = math.rsqrt(math.dot(q.value.xyz, q.value.xyz));
			float num2;
			float num3;
			math.sincos(math.rcp(num), out num2, out num3);
			return math.quaternion(math.float4(q.value.xyz * num * num2, num3));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion exp(quaternion q)
		{
			float num = math.rsqrt(math.dot(q.value.xyz, q.value.xyz));
			float num2;
			float num3;
			math.sincos(math.rcp(num), out num2, out num3);
			return math.quaternion(math.float4(q.value.xyz * num * num2, num3) * math.exp(q.value.w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion unitlog(quaternion q)
		{
			float num = math.clamp(q.value.w, -1f, 1f);
			float num2 = math.acos(num) * math.rsqrt(1f - num * num);
			return math.quaternion(math.float4(q.value.xyz * num2, 0f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion log(quaternion q)
		{
			float num = math.dot(q.value.xyz, q.value.xyz);
			float num2 = num + q.value.w * q.value.w;
			float num3 = math.acos(math.clamp(q.value.w * math.rsqrt(num2), -1f, 1f)) * math.rsqrt(num);
			return math.quaternion(math.float4(q.value.xyz * num3, 0.5f * math.log(num2)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion mul(quaternion a, quaternion b)
		{
			return math.quaternion(a.value.wwww * b.value + (a.value.xyzx * b.value.wwwx + a.value.yzxy * b.value.zxyy) * math.float4(1f, 1f, 1f, -1f) - a.value.zxyz * b.value.yzxz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 mul(quaternion q, float3 v)
		{
			float3 @float = 2f * math.cross(q.value.xyz, v);
			return v + q.value.w * @float + math.cross(q.value.xyz, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 rotate(quaternion q, float3 v)
		{
			float3 @float = 2f * math.cross(q.value.xyz, v);
			return v + q.value.w * @float + math.cross(q.value.xyz, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion nlerp(quaternion q1, quaternion q2, float t)
		{
			if (math.dot(q1, q2) < 0f)
			{
				q2.value = -q2.value;
			}
			return math.normalize(math.quaternion(math.lerp(q1.value, q2.value, t)));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static quaternion slerp(quaternion q1, quaternion q2, float t)
		{
			float num = math.dot(q1, q2);
			if (num < 0f)
			{
				num = -num;
				q2.value = -q2.value;
			}
			if (num < 0.9995f)
			{
				float num2 = math.acos(num);
				float num3 = math.rsqrt(1f - num * num);
				float num4 = math.sin(num2 * (1f - t)) * num3;
				float num5 = math.sin(num2 * t) * num3;
				return math.quaternion(q1.value * num4 + q2.value * num5);
			}
			return math.nlerp(q1, q2, t);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(quaternion q)
		{
			return math.hash(q.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(quaternion q)
		{
			return math.hashwide(q.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 forward(quaternion q)
		{
			return math.mul(q, math.float3(0f, 0f, 1f));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RigidTransform(quaternion rot, float3 pos)
		{
			return new RigidTransform(rot, pos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RigidTransform(float3x3 rotation, float3 translation)
		{
			return new RigidTransform(rotation, translation);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform RigidTransform(float4x4 transform)
		{
			return new RigidTransform(transform);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform inverse(RigidTransform t)
		{
			quaternion quaternion = math.inverse(t.rot);
			float3 @float = math.mul(quaternion, -t.pos);
			return new RigidTransform(quaternion, @float);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static RigidTransform mul(RigidTransform a, RigidTransform b)
		{
			return new RigidTransform(math.mul(a.rot, b.rot), math.mul(a.rot, b.pos) + a.pos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float4 mul(RigidTransform a, float4 pos)
		{
			return math.float4(math.mul(a.rot, pos.xyz) + a.pos * pos.w, pos.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 rotate(RigidTransform a, float3 dir)
		{
			return math.mul(a.rot, dir);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static float3 transform(RigidTransform a, float3 pos)
		{
			return math.mul(a.rot, pos) + a.pos;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(RigidTransform t)
		{
			return math.hash(t.rot) + 3318036811U * math.hash(t.pos);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(RigidTransform t)
		{
			return math.hashwide(t.rot) + 3318036811U * math.hashwide(t.pos).xyzz;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(uint x, uint y)
		{
			return new uint2(x, y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(uint2 xy)
		{
			return new uint2(xy);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(uint v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(bool v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(bool2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(int v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(int2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(float v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(float2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(double v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 uint2(double2 v)
		{
			return new uint2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint2 v)
		{
			return math.csum(v * math.uint2(1148435377U, 3416333663U)) + 1750611407U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(uint2 v)
		{
			return v * math.uint2(3285396193U, 3110507567U) + 4271396531U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint shuffle(uint2 left, uint2 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 shuffle(uint2 left, uint2 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.uint2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 shuffle(uint2 left, uint2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.uint3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 shuffle(uint2 left, uint2 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.uint4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint select_shuffle_component(uint2 a, uint2 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(uint2 c0, uint2 c1)
		{
			return new uint2x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(uint m00, uint m01, uint m10, uint m11)
		{
			return new uint2x2(m00, m01, m10, m11);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(uint v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(bool v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(bool2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(int v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(int2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(float v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(float2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(double v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 uint2x2(double2x2 v)
		{
			return new uint2x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x2 transpose(uint2x2 v)
		{
			return math.uint2x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint2x2 v)
		{
			return math.csum(v.c0 * math.uint2(3010324327U, 1875523709U) + v.c1 * math.uint2(2937008387U, 3835713223U)) + 2216526373U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(uint2x2 v)
		{
			return v.c0 * math.uint2(3375971453U, 3559829411U) + v.c1 * math.uint2(3652178029U, 2544260129U) + 2013864031U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(uint2 c0, uint2 c1, uint2 c2)
		{
			return new uint2x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(uint m00, uint m01, uint m02, uint m10, uint m11, uint m12)
		{
			return new uint2x3(m00, m01, m02, m10, m11, m12);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(uint v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(bool v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(bool2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(int v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(int2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(float v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(float2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(double v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 uint2x3(double2x3 v)
		{
			return new uint2x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 transpose(uint2x3 v)
		{
			return math.uint3x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint2x3 v)
		{
			return math.csum(v.c0 * math.uint2(4016293529U, 2416021567U) + v.c1 * math.uint2(2828384717U, 2636362241U) + v.c2 * math.uint2(1258410977U, 1952565773U)) + 2037535609U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(uint2x3 v)
		{
			return v.c0 * math.uint2(3592785499U, 3996716183U) + v.c1 * math.uint2(2626301701U, 1306289417U) + v.c2 * math.uint2(2096137163U, 1548578029U) + 4178800919U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(uint2 c0, uint2 c1, uint2 c2, uint2 c3)
		{
			return new uint2x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(uint m00, uint m01, uint m02, uint m03, uint m10, uint m11, uint m12, uint m13)
		{
			return new uint2x4(m00, m01, m02, m03, m10, m11, m12, m13);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(uint v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(bool v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(bool2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(int v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(int2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(float v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(float2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(double v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 uint2x4(double2x4 v)
		{
			return new uint2x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 transpose(uint2x4 v)
		{
			return math.uint4x2(v.c0.x, v.c0.y, v.c1.x, v.c1.y, v.c2.x, v.c2.y, v.c3.x, v.c3.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint2x4 v)
		{
			return math.csum(v.c0 * math.uint2(2650080659U, 4052675461U) + v.c1 * math.uint2(2652487619U, 2174136431U) + v.c2 * math.uint2(3528391193U, 2105559227U) + v.c3 * math.uint2(1899745391U, 1966790317U)) + 3516359879U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 hashwide(uint2x4 v)
		{
			return v.c0 * math.uint2(3050356579U, 4178586719U) + v.c1 * math.uint2(2558655391U, 1453413133U) + v.c2 * math.uint2(2152428077U, 1938706661U) + v.c3 * math.uint2(1338588197U, 3439609253U) + 3535343003U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(uint x, uint y, uint z)
		{
			return new uint3(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(uint x, uint2 yz)
		{
			return new uint3(x, yz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(uint2 xy, uint z)
		{
			return new uint3(xy, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(uint3 xyz)
		{
			return new uint3(xyz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(uint v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(bool v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(bool3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(int v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(int3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(float v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(float3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(double v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 uint3(double3 v)
		{
			return new uint3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint3 v)
		{
			return math.csum(v * math.uint3(3441847433U, 4052036147U, 2011389559U)) + 2252224297U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(uint3 v)
		{
			return v * math.uint3(3784421429U, 1750626223U, 3571447507U) + 3412283213U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint shuffle(uint3 left, uint3 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 shuffle(uint3 left, uint3 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.uint2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 shuffle(uint3 left, uint3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.uint3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 shuffle(uint3 left, uint3 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.uint4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint select_shuffle_component(uint3 a, uint3 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			}
			throw new ArgumentException("Invalid shuffle component: " + component.ToString());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(uint3 c0, uint3 c1)
		{
			return new uint3x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(uint m00, uint m01, uint m10, uint m11, uint m20, uint m21)
		{
			return new uint3x2(m00, m01, m10, m11, m20, m21);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(uint v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(bool v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(bool3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(int v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(int3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(float v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(float3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(double v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x2 uint3x2(double3x2 v)
		{
			return new uint3x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x3 transpose(uint3x2 v)
		{
			return math.uint2x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint3x2 v)
		{
			return math.csum(v.c0 * math.uint3(1365086453U, 3969870067U, 4192899797U) + v.c1 * math.uint3(3271228601U, 1634639009U, 3318036811U)) + 3404170631U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(uint3x2 v)
		{
			return v.c0 * math.uint3(2048213449U, 4164671783U, 1780759499U) + v.c1 * math.uint3(1352369353U, 2446407751U, 1391928079U) + 3475533443U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(uint3 c0, uint3 c1, uint3 c2)
		{
			return new uint3x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(uint m00, uint m01, uint m02, uint m10, uint m11, uint m12, uint m20, uint m21, uint m22)
		{
			return new uint3x3(m00, m01, m02, m10, m11, m12, m20, m21, m22);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(uint v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(bool v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(bool3x3 v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(int v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(int3x3 v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(float v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(float3x3 v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(double v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 uint3x3(double3x3 v)
		{
			return new uint3x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x3 transpose(uint3x3 v)
		{
			return math.uint3x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint3x3 v)
		{
			return math.csum(v.c0 * math.uint3(2892026051U, 2455987759U, 3868600063U) + v.c1 * math.uint3(3170963179U, 2632835537U, 1136528209U) + v.c2 * math.uint3(2944626401U, 2972762423U, 1417889653U)) + 2080514593U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(uint3x3 v)
		{
			return v.c0 * math.uint3(2731544287U, 2828498809U, 2669441947U) + v.c1 * math.uint3(1260114311U, 2650080659U, 4052675461U) + v.c2 * math.uint3(2652487619U, 2174136431U, 3528391193U) + 2105559227U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(uint3 c0, uint3 c1, uint3 c2, uint3 c3)
		{
			return new uint3x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(uint m00, uint m01, uint m02, uint m03, uint m10, uint m11, uint m12, uint m13, uint m20, uint m21, uint m22, uint m23)
		{
			return new uint3x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(uint v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(bool v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(bool3x4 v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(int v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(int3x4 v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(float v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(float3x4 v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(double v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 uint3x4(double3x4 v)
		{
			return new uint3x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 transpose(uint3x4 v)
		{
			return math.uint4x3(v.c0.x, v.c0.y, v.c0.z, v.c1.x, v.c1.y, v.c1.z, v.c2.x, v.c2.y, v.c2.z, v.c3.x, v.c3.y, v.c3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint3x4 v)
		{
			return math.csum(v.c0 * math.uint3(3508684087U, 3919501043U, 1209161033U) + v.c1 * math.uint3(4007793211U, 3819806693U, 3458005183U) + v.c2 * math.uint3(2078515003U, 4206465343U, 3025146473U) + v.c3 * math.uint3(3763046909U, 3678265601U, 2070747979U)) + 1480171127U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 hashwide(uint3x4 v)
		{
			return v.c0 * math.uint3(1588341193U, 4234155257U, 1811310911U) + v.c1 * math.uint3(2635799963U, 4165137857U, 2759770933U) + v.c2 * math.uint3(2759319383U, 3299952959U, 3121178323U) + v.c3 * math.uint3(2948522579U, 1531026433U, 1365086453U) + 3969870067U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint x, uint y, uint z, uint w)
		{
			return new uint4(x, y, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint x, uint y, uint2 zw)
		{
			return new uint4(x, y, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint x, uint2 yz, uint w)
		{
			return new uint4(x, yz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint x, uint3 yzw)
		{
			return new uint4(x, yzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint2 xy, uint z, uint w)
		{
			return new uint4(xy, z, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint2 xy, uint2 zw)
		{
			return new uint4(xy, zw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint3 xyz, uint w)
		{
			return new uint4(xyz, w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint4 xyzw)
		{
			return new uint4(xyzw);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(uint v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(bool v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(bool4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(int v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(int4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(float v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(float4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(double v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 uint4(double4 v)
		{
			return new uint4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint4 v)
		{
			return math.csum(v * math.uint4(3029516053U, 3547472099U, 2057487037U, 3781937309U)) + 2057338067U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(uint4 v)
		{
			return v * math.uint4(2942577577U, 2834440507U, 2671762487U, 2892026051U) + 2455987759U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint shuffle(uint4 left, uint4 right, math.ShuffleComponent x)
		{
			return math.select_shuffle_component(left, right, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2 shuffle(uint4 left, uint4 right, math.ShuffleComponent x, math.ShuffleComponent y)
		{
			return math.uint2(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3 shuffle(uint4 left, uint4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z)
		{
			return math.uint3(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 shuffle(uint4 left, uint4 right, math.ShuffleComponent x, math.ShuffleComponent y, math.ShuffleComponent z, math.ShuffleComponent w)
		{
			return math.uint4(math.select_shuffle_component(left, right, x), math.select_shuffle_component(left, right, y), math.select_shuffle_component(left, right, z), math.select_shuffle_component(left, right, w));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint select_shuffle_component(uint4 a, uint4 b, math.ShuffleComponent component)
		{
			switch (component)
			{
			case math.ShuffleComponent.LeftX:
				return a.x;
			case math.ShuffleComponent.LeftY:
				return a.y;
			case math.ShuffleComponent.LeftZ:
				return a.z;
			case math.ShuffleComponent.LeftW:
				return a.w;
			case math.ShuffleComponent.RightX:
				return b.x;
			case math.ShuffleComponent.RightY:
				return b.y;
			case math.ShuffleComponent.RightZ:
				return b.z;
			case math.ShuffleComponent.RightW:
				return b.w;
			default:
				throw new ArgumentException("Invalid shuffle component: " + component.ToString());
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(uint4 c0, uint4 c1)
		{
			return new uint4x2(c0, c1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(uint m00, uint m01, uint m10, uint m11, uint m20, uint m21, uint m30, uint m31)
		{
			return new uint4x2(m00, m01, m10, m11, m20, m21, m30, m31);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(uint v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(bool v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(bool4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(int v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(int4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(float v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(float4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(double v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x2 uint4x2(double4x2 v)
		{
			return new uint4x2(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint2x4 transpose(uint4x2 v)
		{
			return math.uint2x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint4x2 v)
		{
			return math.csum(v.c0 * math.uint4(4198118021U, 2908068253U, 3705492289U, 2497566569U) + v.c1 * math.uint4(2716413241U, 1166264321U, 2503385333U, 2944493077U)) + 2599999021U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(uint4x2 v)
		{
			return v.c0 * math.uint4(3814721321U, 1595355149U, 1728931849U, 2062756937U) + v.c1 * math.uint4(2920485769U, 1562056283U, 2265541847U, 1283419601U) + 1210229737U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(uint4 c0, uint4 c1, uint4 c2)
		{
			return new uint4x3(c0, c1, c2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(uint m00, uint m01, uint m02, uint m10, uint m11, uint m12, uint m20, uint m21, uint m22, uint m30, uint m31, uint m32)
		{
			return new uint4x3(m00, m01, m02, m10, m11, m12, m20, m21, m22, m30, m31, m32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(uint v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(bool v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(bool4x3 v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(int v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(int4x3 v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(float v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(float4x3 v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(double v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x3 uint4x3(double4x3 v)
		{
			return new uint4x3(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint3x4 transpose(uint4x3 v)
		{
			return math.uint3x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint4x3 v)
		{
			return math.csum(v.c0 * math.uint4(3881277847U, 4017968839U, 1727237899U, 1648514723U) + v.c1 * math.uint4(1385344481U, 3538260197U, 4066109527U, 2613148903U) + v.c2 * math.uint4(3367528529U, 1678332449U, 2918459647U, 2744611081U)) + 1952372791U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(uint4x3 v)
		{
			return v.c0 * math.uint4(2631698677U, 4200781601U, 2119021007U, 1760485621U) + v.c1 * math.uint4(3157985881U, 2171534173U, 2723054263U, 1168253063U) + v.c2 * math.uint4(4228926523U, 1610574617U, 1584185147U, 3041325733U) + 3150930919U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(uint4 c0, uint4 c1, uint4 c2, uint4 c3)
		{
			return new uint4x4(c0, c1, c2, c3);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(uint m00, uint m01, uint m02, uint m03, uint m10, uint m11, uint m12, uint m13, uint m20, uint m21, uint m22, uint m23, uint m30, uint m31, uint m32, uint m33)
		{
			return new uint4x4(m00, m01, m02, m03, m10, m11, m12, m13, m20, m21, m22, m23, m30, m31, m32, m33);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(uint v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(bool v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(bool4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(int v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(int4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(float v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(float4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(double v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 uint4x4(double4x4 v)
		{
			return new uint4x4(v);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4x4 transpose(uint4x4 v)
		{
			return math.uint4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint hash(uint4x4 v)
		{
			return math.csum(v.c0 * math.uint4(2627668003U, 1520214331U, 2949502447U, 2827819133U) + v.c1 * math.uint4(3480140317U, 2642994593U, 3940484981U, 1954192763U) + v.c2 * math.uint4(1091696537U, 3052428017U, 4253034763U, 2338696631U) + v.c3 * math.uint4(3757372771U, 1885959949U, 3508684087U, 3919501043U)) + 1209161033U;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint4 hashwide(uint4x4 v)
		{
			return v.c0 * math.uint4(4007793211U, 3819806693U, 3458005183U, 2078515003U) + v.c1 * math.uint4(4206465343U, 3025146473U, 3763046909U, 3678265601U) + v.c2 * math.uint4(2070747979U, 1480171127U, 1588341193U, 4234155257U) + v.c3 * math.uint4(1811310911U, 2635799963U, 4165137857U, 2759770933U) + 2759319383U;
		}

		public const double E_DBL = 2.718281828459045;

		public const double LOG2E_DBL = 1.4426950408889634;

		public const double LOG10E_DBL = 0.4342944819032518;

		public const double LN2_DBL = 0.6931471805599453;

		public const double LN10_DBL = 2.302585092994046;

		public const double PI_DBL = 3.141592653589793;

		public const double SQRT2_DBL = 1.4142135623730951;

		public const double EPSILON_DBL = 2.220446049250313E-16;

		public const double INFINITY_DBL = double.PositiveInfinity;

		public const double NAN_DBL = double.NaN;

		public const float FLT_MIN_NORMAL = 1.1754944E-38f;

		public const double DBL_MIN_NORMAL = 2.2250738585072014E-308;

		public const float E = 2.7182817f;

		public const float LOG2E = 1.442695f;

		public const float LOG10E = 0.4342945f;

		public const float LN2 = 0.6931472f;

		public const float LN10 = 2.3025851f;

		public const float PI = 3.1415927f;

		public const float SQRT2 = 1.4142135f;

		public const float EPSILON = 1.1920929E-07f;

		public const float INFINITY = float.PositiveInfinity;

		public const float NAN = float.NaN;

		public enum RotationOrder : byte
		{
			XYZ,
			XZY,
			YXZ,
			YZX,
			ZXY,
			ZYX,
			Default = 4
		}

		public enum ShuffleComponent : byte
		{
			LeftX,
			LeftY,
			LeftZ,
			LeftW,
			RightX,
			RightY,
			RightZ,
			RightW
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct IntFloatUnion
		{
			[FieldOffset(0)]
			public int intValue;

			[FieldOffset(0)]
			public float floatValue;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct LongDoubleUnion
		{
			[FieldOffset(0)]
			public long longValue;

			[FieldOffset(0)]
			public double doubleValue;
		}
	}
}
