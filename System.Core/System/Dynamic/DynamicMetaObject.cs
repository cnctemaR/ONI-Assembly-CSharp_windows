using System;
using System.Collections.Generic;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	public class DynamicMetaObject
	{
		public DynamicMetaObject(Expression expression, BindingRestrictions restrictions)
		{
			ContractUtils.RequiresNotNull(expression, "expression");
			ContractUtils.RequiresNotNull(restrictions, "restrictions");
			this.Expression = expression;
			this.Restrictions = restrictions;
		}

		public DynamicMetaObject(Expression expression, BindingRestrictions restrictions, object value)
			: this(expression, restrictions)
		{
			this.Value = value;
			this.HasValue = true;
		}

		public Expression Expression { get; }

		public BindingRestrictions Restrictions { get; }

		public object Value { get; }

		public bool HasValue { get; }

		public Type RuntimeType
		{
			get
			{
				if (!this.HasValue)
				{
					return null;
				}
				Type type = this.Expression.Type;
				if (type.IsValueType)
				{
					return type;
				}
				object value = this.Value;
				if (value == null)
				{
					return null;
				}
				return value.GetType();
			}
		}

		public Type LimitType
		{
			get
			{
				return this.RuntimeType ?? this.Expression.Type;
			}
		}

		public virtual DynamicMetaObject BindConvert(ConvertBinder binder)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackConvert(this);
		}

		public virtual DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackGetMember(this);
		}

		public virtual DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackSetMember(this, value);
		}

		public virtual DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackDeleteMember(this);
		}

		public virtual DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackGetIndex(this, indexes);
		}

		public virtual DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackSetIndex(this, indexes, value);
		}

		public virtual DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackDeleteIndex(this, indexes);
		}

		public virtual DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackInvokeMember(this, args);
		}

		public virtual DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackInvoke(this, args);
		}

		public virtual DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackCreateInstance(this, args);
		}

		public virtual DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackUnaryOperation(this);
		}

		public virtual DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
		{
			ContractUtils.RequiresNotNull(binder, "binder");
			return binder.FallbackBinaryOperation(this, arg);
		}

		public virtual IEnumerable<string> GetDynamicMemberNames()
		{
			return Array.Empty<string>();
		}

		internal static Expression[] GetExpressions(DynamicMetaObject[] objects)
		{
			ContractUtils.RequiresNotNull(objects, "objects");
			Expression[] array = new Expression[objects.Length];
			for (int i = 0; i < objects.Length; i++)
			{
				DynamicMetaObject dynamicMetaObject = objects[i];
				ContractUtils.RequiresNotNull(dynamicMetaObject, "objects");
				Expression expression = dynamicMetaObject.Expression;
				array[i] = expression;
			}
			return array;
		}

		public static DynamicMetaObject Create(object value, Expression expression)
		{
			ContractUtils.RequiresNotNull(expression, "expression");
			IDynamicMetaObjectProvider dynamicMetaObjectProvider = value as IDynamicMetaObjectProvider;
			if (dynamicMetaObjectProvider == null)
			{
				return new DynamicMetaObject(expression, BindingRestrictions.Empty, value);
			}
			DynamicMetaObject metaObject = dynamicMetaObjectProvider.GetMetaObject(expression);
			if (metaObject == null || !metaObject.HasValue || metaObject.Value == null || metaObject.Expression != expression)
			{
				throw Error.InvalidMetaObjectCreated(dynamicMetaObjectProvider.GetType());
			}
			return metaObject;
		}

		public static readonly DynamicMetaObject[] EmptyMetaObjects = Array.Empty<DynamicMetaObject>();
	}
}
