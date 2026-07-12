using System;
using System.Reflection;
using System.Reflection.Emit;
using Unity.Collections;

namespace Unity.Properties
{
	public class ReflectedMemberProperty<TContainer, TValue> : Property<TContainer, TValue>
	{
		public override string Name { get; }

		public override bool IsReadOnly { get; }

		public ReflectedMemberProperty(FieldInfo info, string name)
			: this(new FieldMember(info), name)
		{
		}

		public ReflectedMemberProperty(PropertyInfo info, string name)
			: this(new PropertyMember(info), name)
		{
		}

		internal ReflectedMemberProperty(IMemberInfo info, string name)
		{
			this.Name = name;
			this.m_Info = info;
			this.m_IsStructContainerType = TypeTraits<TContainer>.IsValueType;
			base.AddAttributes(info.GetCustomAttributes());
			bool flag = this.m_Info.IsReadOnly || base.HasAttribute<ReadOnlyAttribute>();
			this.IsReadOnly = flag;
			IMemberInfo memberInfo = this.m_Info;
			FieldMember fieldMember;
			bool flag2;
			if (memberInfo is FieldMember)
			{
				fieldMember = (FieldMember)memberInfo;
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			if (flag3)
			{
				FieldInfo fieldInfo = fieldMember.m_FieldInfo;
				DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, fieldInfo.FieldType, new Type[] { this.m_IsStructContainerType ? fieldInfo.ReflectedType.MakeByRefType() : fieldInfo.ReflectedType }, true);
				ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldfld, fieldInfo);
				ilgenerator.Emit(OpCodes.Ret);
				bool isStructContainerType = this.m_IsStructContainerType;
				if (isStructContainerType)
				{
					this.m_GetStructValueAction = (ReflectedMemberProperty<TContainer, TValue>.GetStructValueAction)dynamicMethod.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.GetStructValueAction));
				}
				else
				{
					this.m_GetClassValueAction = (ReflectedMemberProperty<TContainer, TValue>.GetClassValueAction)dynamicMethod.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.GetClassValueAction));
				}
				bool flag4 = !flag;
				if (flag4)
				{
					dynamicMethod = new DynamicMethod(string.Empty, typeof(void), new Type[]
					{
						this.m_IsStructContainerType ? fieldInfo.ReflectedType.MakeByRefType() : fieldInfo.ReflectedType,
						fieldInfo.FieldType
					}, true);
					ilgenerator = dynamicMethod.GetILGenerator();
					ilgenerator.Emit(OpCodes.Ldarg_0);
					ilgenerator.Emit(OpCodes.Ldarg_1);
					ilgenerator.Emit(OpCodes.Stfld, fieldInfo);
					ilgenerator.Emit(OpCodes.Ret);
					bool isStructContainerType2 = this.m_IsStructContainerType;
					if (isStructContainerType2)
					{
						this.m_SetStructValueAction = (ReflectedMemberProperty<TContainer, TValue>.SetStructValueAction)dynamicMethod.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.SetStructValueAction));
					}
					else
					{
						this.m_SetClassValueAction = (ReflectedMemberProperty<TContainer, TValue>.SetClassValueAction)dynamicMethod.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.SetClassValueAction));
					}
				}
			}
			else
			{
				memberInfo = this.m_Info;
				PropertyMember propertyMember;
				bool flag5;
				if (memberInfo is PropertyMember)
				{
					propertyMember = (PropertyMember)memberInfo;
					flag5 = true;
				}
				else
				{
					flag5 = false;
				}
				bool flag6 = flag5;
				if (flag6)
				{
					bool isStructContainerType3 = this.m_IsStructContainerType;
					if (isStructContainerType3)
					{
						MethodInfo getMethod = propertyMember.m_PropertyInfo.GetGetMethod(true);
						this.m_GetStructValueAction = (ReflectedMemberProperty<TContainer, TValue>.GetStructValueAction)Delegate.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.GetStructValueAction), getMethod);
						bool flag7 = !flag;
						if (flag7)
						{
							MethodInfo setMethod = propertyMember.m_PropertyInfo.GetSetMethod(true);
							this.m_SetStructValueAction = (ReflectedMemberProperty<TContainer, TValue>.SetStructValueAction)Delegate.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.SetStructValueAction), setMethod);
						}
					}
					else
					{
						MethodInfo getMethod2 = propertyMember.m_PropertyInfo.GetGetMethod(true);
						this.m_GetClassValueAction = (ReflectedMemberProperty<TContainer, TValue>.GetClassValueAction)Delegate.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.GetClassValueAction), getMethod2);
						bool flag8 = !flag;
						if (flag8)
						{
							MethodInfo setMethod2 = propertyMember.m_PropertyInfo.GetSetMethod(true);
							this.m_SetClassValueAction = (ReflectedMemberProperty<TContainer, TValue>.SetClassValueAction)Delegate.CreateDelegate(typeof(ReflectedMemberProperty<TContainer, TValue>.SetClassValueAction), setMethod2);
						}
					}
				}
			}
		}

		public override TValue GetValue(ref TContainer container)
		{
			bool isStructContainerType = this.m_IsStructContainerType;
			TValue tvalue;
			if (isStructContainerType)
			{
				tvalue = ((this.m_GetStructValueAction == null) ? ((TValue)((object)this.m_Info.GetValue(container))) : this.m_GetStructValueAction(ref container));
			}
			else
			{
				tvalue = ((this.m_GetClassValueAction == null) ? ((TValue)((object)this.m_Info.GetValue(container))) : this.m_GetClassValueAction(container));
			}
			return tvalue;
		}

		public override void SetValue(ref TContainer container, TValue value)
		{
			bool isReadOnly = this.IsReadOnly;
			if (isReadOnly)
			{
				throw new InvalidOperationException("Property is ReadOnly.");
			}
			bool isStructContainerType = this.m_IsStructContainerType;
			if (isStructContainerType)
			{
				bool flag = this.m_SetStructValueAction == null;
				if (flag)
				{
					object obj = container;
					this.m_Info.SetValue(obj, value);
					container = (TContainer)((object)obj);
				}
				else
				{
					this.m_SetStructValueAction(ref container, value);
				}
			}
			else
			{
				bool flag2 = this.m_SetClassValueAction == null;
				if (flag2)
				{
					this.m_Info.SetValue(container, value);
				}
				else
				{
					this.m_SetClassValueAction(container, value);
				}
			}
		}

		private readonly IMemberInfo m_Info;

		private readonly bool m_IsStructContainerType;

		private ReflectedMemberProperty<TContainer, TValue>.GetStructValueAction m_GetStructValueAction;

		private ReflectedMemberProperty<TContainer, TValue>.SetStructValueAction m_SetStructValueAction;

		private ReflectedMemberProperty<TContainer, TValue>.GetClassValueAction m_GetClassValueAction;

		private ReflectedMemberProperty<TContainer, TValue>.SetClassValueAction m_SetClassValueAction;

		private delegate TValue GetStructValueAction(ref TContainer container);

		private delegate void SetStructValueAction(ref TContainer container, TValue value);

		private delegate TValue GetClassValueAction(TContainer container);

		private delegate void SetClassValueAction(TContainer container, TValue value);
	}
}
