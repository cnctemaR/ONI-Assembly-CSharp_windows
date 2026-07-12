using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.ReflectionModel
{
	public struct LazyMemberInfo
	{
		public LazyMemberInfo(MemberInfo member)
		{
			Requires.NotNull<MemberInfo>(member, "member");
			LazyMemberInfo.EnsureSupportedMemberType(member.MemberType, "member");
			this._accessorsCreator = null;
			this._memberType = member.MemberType;
			MemberTypes memberType = this._memberType;
			if (memberType == MemberTypes.Event)
			{
				EventInfo eventInfo = (EventInfo)member;
				this._accessors = new MemberInfo[]
				{
					eventInfo.GetRaiseMethod(true),
					eventInfo.GetAddMethod(true),
					eventInfo.GetRemoveMethod(true)
				};
				return;
			}
			if (memberType == MemberTypes.Property)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				Assumes.NotNull<PropertyInfo>(propertyInfo);
				this._accessors = new MemberInfo[]
				{
					propertyInfo.GetGetMethod(true),
					propertyInfo.GetSetMethod(true)
				};
				return;
			}
			this._accessors = new MemberInfo[] { member };
		}

		public LazyMemberInfo(MemberTypes memberType, params MemberInfo[] accessors)
		{
			LazyMemberInfo.EnsureSupportedMemberType(memberType, "memberType");
			Requires.NotNull<MemberInfo[]>(accessors, "accessors");
			string text;
			if (!LazyMemberInfo.AreAccessorsValid(memberType, accessors, out text))
			{
				throw new ArgumentException(text, "accessors");
			}
			this._memberType = memberType;
			this._accessors = accessors;
			this._accessorsCreator = null;
		}

		public LazyMemberInfo(MemberTypes memberType, Func<MemberInfo[]> accessorsCreator)
		{
			LazyMemberInfo.EnsureSupportedMemberType(memberType, "memberType");
			Requires.NotNull<Func<MemberInfo[]>>(accessorsCreator, "accessorsCreator");
			this._memberType = memberType;
			this._accessors = null;
			this._accessorsCreator = accessorsCreator;
		}

		public MemberTypes MemberType
		{
			get
			{
				return this._memberType;
			}
		}

		public MemberInfo[] GetAccessors()
		{
			if (this._accessors == null && this._accessorsCreator != null)
			{
				MemberInfo[] array = this._accessorsCreator();
				string text;
				if (!LazyMemberInfo.AreAccessorsValid(this.MemberType, array, out text))
				{
					throw new InvalidOperationException(text);
				}
				this._accessors = array;
			}
			return this._accessors;
		}

		public override int GetHashCode()
		{
			if (this._accessorsCreator != null)
			{
				return this.MemberType.GetHashCode() ^ this._accessorsCreator.GetHashCode();
			}
			Assumes.NotNull<MemberInfo[]>(this._accessors);
			Assumes.NotNull<MemberInfo>(this._accessors[0]);
			return this.MemberType.GetHashCode() ^ this._accessors[0].GetHashCode();
		}

		public override bool Equals(object obj)
		{
			LazyMemberInfo lazyMemberInfo = (LazyMemberInfo)obj;
			if (this._memberType != lazyMemberInfo._memberType)
			{
				return false;
			}
			if (this._accessorsCreator != null || lazyMemberInfo._accessorsCreator != null)
			{
				return object.Equals(this._accessorsCreator, lazyMemberInfo._accessorsCreator);
			}
			Assumes.NotNull<MemberInfo[]>(this._accessors);
			Assumes.NotNull<MemberInfo[]>(lazyMemberInfo._accessors);
			return this._accessors.SequenceEqual<MemberInfo>(lazyMemberInfo._accessors);
		}

		public static bool operator ==(LazyMemberInfo left, LazyMemberInfo right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(LazyMemberInfo left, LazyMemberInfo right)
		{
			return !left.Equals(right);
		}

		private static void EnsureSupportedMemberType(MemberTypes memberType, string argument)
		{
			MemberTypes memberTypes = MemberTypes.All;
			Requires.IsInMembertypeSet(memberType, argument, memberTypes);
		}

		private static bool AreAccessorsValid(MemberTypes memberType, MemberInfo[] accessors, out string errorMessage)
		{
			errorMessage = string.Empty;
			if (accessors == null)
			{
				errorMessage = Strings.LazyMemberInfo_AccessorsNull;
				return false;
			}
			if (accessors.All<MemberInfo>((MemberInfo accessor) => accessor == null))
			{
				errorMessage = Strings.LazyMemberInfo_NoAccessors;
				return false;
			}
			if (memberType != MemberTypes.Event)
			{
				if (memberType == MemberTypes.Property)
				{
					if (accessors.Length != 2)
					{
						errorMessage = Strings.LazyMemberInfo_InvalidPropertyAccessors_Cardinality;
						return false;
					}
					if (accessors.Where<MemberInfo>((MemberInfo accessor) => accessor != null && accessor.MemberType != MemberTypes.Method).Any<MemberInfo>())
					{
						errorMessage = Strings.LazyMemberinfo_InvalidPropertyAccessors_AccessorType;
						return false;
					}
				}
				else if (accessors.Length != 1 || (accessors.Length == 1 && accessors[0].MemberType != memberType))
				{
					errorMessage = string.Format(CultureInfo.CurrentCulture, Strings.LazyMemberInfo_InvalidAccessorOnSimpleMember, memberType);
					return false;
				}
			}
			else
			{
				if (accessors.Length != 3)
				{
					errorMessage = Strings.LazyMemberInfo_InvalidEventAccessors_Cardinality;
					return false;
				}
				if (accessors.Where<MemberInfo>((MemberInfo accessor) => accessor != null && accessor.MemberType != MemberTypes.Method).Any<MemberInfo>())
				{
					errorMessage = Strings.LazyMemberinfo_InvalidEventAccessors_AccessorType;
					return false;
				}
			}
			return true;
		}

		private readonly MemberTypes _memberType;

		private MemberInfo[] _accessors;

		private readonly Func<MemberInfo[]> _accessorsCreator;
	}
}
