using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Harmony
{
	public class Traverse
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		static Traverse()
		{
			bool flag = Traverse.Cache == null;
			if (flag)
			{
				Traverse.Cache = new AccessCache();
			}
		}

		public static Traverse Create(Type type)
		{
			return new Traverse(type);
		}

		public static Traverse Create<T>()
		{
			return Traverse.Create(typeof(T));
		}

		public static Traverse Create(object root)
		{
			return new Traverse(root);
		}

		public static Traverse CreateWithType(string name)
		{
			return new Traverse(AccessTools.TypeByName(name));
		}

		private Traverse()
		{
		}

		public Traverse(Type type)
		{
			this._type = type;
		}

		public Traverse(object root)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null);
		}

		private Traverse(object root, MemberInfo info, object[] index)
		{
			this._root = root;
			this._type = ((root != null) ? root.GetType() : null);
			this._info = info;
			this._params = index;
		}

		private Traverse(object root, MethodInfo method, object[] parameter)
		{
			this._root = root;
			this._type = method.ReturnType;
			this._method = method;
			this._params = parameter;
		}

		public object GetValue()
		{
			bool flag = this._info is FieldInfo;
			object obj;
			if (flag)
			{
				obj = ((FieldInfo)this._info).GetValue(this._root);
			}
			else
			{
				bool flag2 = this._info is PropertyInfo;
				if (flag2)
				{
					obj = ((PropertyInfo)this._info).GetValue(this._root, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
				}
				else
				{
					bool flag3 = this._method != null;
					if (flag3)
					{
						obj = this._method.Invoke(this._root, this._params);
					}
					else
					{
						bool flag4 = this._root == null && this._type != null;
						if (flag4)
						{
							obj = this._type;
						}
						else
						{
							obj = this._root;
						}
					}
				}
			}
			return obj;
		}

		public T GetValue<T>()
		{
			object value = this.GetValue();
			bool flag = value == null;
			T t;
			if (flag)
			{
				t = default(T);
			}
			else
			{
				t = (T)((object)value);
			}
			return t;
		}

		public object GetValue(params object[] arguments)
		{
			bool flag = this._method == null;
			if (flag)
			{
				throw new Exception("cannot get method value without method");
			}
			return this._method.Invoke(this._root, arguments);
		}

		public T GetValue<T>(params object[] arguments)
		{
			bool flag = this._method == null;
			if (flag)
			{
				throw new Exception("cannot get method value without method");
			}
			return (T)((object)this._method.Invoke(this._root, arguments));
		}

		public Traverse SetValue(object value)
		{
			bool flag = this._info is FieldInfo;
			if (flag)
			{
				((FieldInfo)this._info).SetValue(this._root, value, AccessTools.all, null, CultureInfo.CurrentCulture);
			}
			bool flag2 = this._info is PropertyInfo;
			if (flag2)
			{
				((PropertyInfo)this._info).SetValue(this._root, value, AccessTools.all, null, this._params, CultureInfo.CurrentCulture);
			}
			bool flag3 = this._method != null;
			if (flag3)
			{
				throw new Exception("cannot set value of method " + this._method.FullDescription());
			}
			return this;
		}

		public Type GetValueType()
		{
			bool flag = this._info is FieldInfo;
			Type type;
			if (flag)
			{
				type = ((FieldInfo)this._info).FieldType;
			}
			else
			{
				bool flag2 = this._info is PropertyInfo;
				if (flag2)
				{
					type = ((PropertyInfo)this._info).PropertyType;
				}
				else
				{
					type = null;
				}
			}
			return type;
		}

		private Traverse Resolve()
		{
			bool flag = this._root == null && this._type != null;
			Traverse traverse;
			if (flag)
			{
				traverse = this;
			}
			else
			{
				traverse = new Traverse(this.GetValue());
			}
			return traverse;
		}

		public Traverse Type(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			bool flag2 = this._type == null;
			Traverse traverse;
			if (flag2)
			{
				traverse = new Traverse();
			}
			else
			{
				Type type = AccessTools.Inner(this._type, name);
				bool flag3 = type == null;
				if (flag3)
				{
					traverse = new Traverse();
				}
				else
				{
					traverse = new Traverse(type);
				}
			}
			return traverse;
		}

		public Traverse Field(string name)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			Traverse traverse = this.Resolve();
			bool flag2 = traverse._type == null;
			Traverse traverse2;
			if (flag2)
			{
				traverse2 = new Traverse();
			}
			else
			{
				FieldInfo fieldInfo = Traverse.Cache.GetFieldInfo(traverse._type, name);
				bool flag3 = fieldInfo == null;
				if (flag3)
				{
					traverse2 = new Traverse();
				}
				else
				{
					bool flag4 = !fieldInfo.IsStatic && traverse._root == null;
					if (flag4)
					{
						traverse2 = new Traverse();
					}
					else
					{
						traverse2 = new Traverse(traverse._root, fieldInfo, null);
					}
				}
			}
			return traverse2;
		}

		public Traverse<T> Field<T>(string name)
		{
			return new Traverse<T>(this.Field(name));
		}

		public List<string> Fields()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetFieldNames(traverse._type);
		}

		public Traverse Property(string name, object[] index = null)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			Traverse traverse = this.Resolve();
			bool flag2 = traverse._root == null || traverse._type == null;
			Traverse traverse2;
			if (flag2)
			{
				traverse2 = new Traverse();
			}
			else
			{
				PropertyInfo propertyInfo = Traverse.Cache.GetPropertyInfo(traverse._type, name);
				bool flag3 = propertyInfo == null;
				if (flag3)
				{
					traverse2 = new Traverse();
				}
				else
				{
					traverse2 = new Traverse(traverse._root, propertyInfo, index);
				}
			}
			return traverse2;
		}

		public Traverse<T> Property<T>(string name, object[] index = null)
		{
			return new Traverse<T>(this.Property(name, index));
		}

		public List<string> Properties()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetPropertyNames(traverse._type);
		}

		public Traverse Method(string name, params object[] arguments)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			Traverse traverse = this.Resolve();
			bool flag2 = traverse._type == null;
			Traverse traverse2;
			if (flag2)
			{
				traverse2 = new Traverse();
			}
			else
			{
				Type[] types = AccessTools.GetTypes(arguments);
				MethodBase methodInfo = Traverse.Cache.GetMethodInfo(traverse._type, name, types);
				bool flag3 = methodInfo == null;
				if (flag3)
				{
					traverse2 = new Traverse();
				}
				else
				{
					traverse2 = new Traverse(traverse._root, (MethodInfo)methodInfo, arguments);
				}
			}
			return traverse2;
		}

		public Traverse Method(string name, Type[] paramTypes, object[] arguments = null)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name cannot be null");
			}
			Traverse traverse = this.Resolve();
			bool flag2 = traverse._type == null;
			Traverse traverse2;
			if (flag2)
			{
				traverse2 = new Traverse();
			}
			else
			{
				MethodBase methodInfo = Traverse.Cache.GetMethodInfo(traverse._type, name, paramTypes);
				bool flag3 = methodInfo == null;
				if (flag3)
				{
					traverse2 = new Traverse();
				}
				else
				{
					traverse2 = new Traverse(traverse._root, (MethodInfo)methodInfo, arguments);
				}
			}
			return traverse2;
		}

		public List<string> Methods()
		{
			Traverse traverse = this.Resolve();
			return AccessTools.GetMethodNames(traverse._type);
		}

		public bool FieldExists()
		{
			return this._info != null;
		}

		public bool MethodExists()
		{
			return this._method != null;
		}

		public bool TypeExists()
		{
			return this._type != null;
		}

		public static void IterateFields(object source, Action<Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f));
			});
		}

		public static void IterateFields(object source, object target, Action<Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		public static void IterateFields(object source, object target, Action<string, Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetFieldNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Field(f), targetTrv.Field(f));
			});
		}

		public static void IterateProperties(object source, Action<Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null));
			});
		}

		public static void IterateProperties(object source, object target, Action<Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		public static void IterateProperties(object source, object target, Action<string, Traverse, Traverse> action)
		{
			Traverse sourceTrv = Traverse.Create(source);
			Traverse targetTrv = Traverse.Create(target);
			AccessTools.GetPropertyNames(source).ForEach(delegate(string f)
			{
				action(f, sourceTrv.Property(f, null), targetTrv.Property(f, null));
			});
		}

		public override string ToString()
		{
			object obj = this._method ?? this.GetValue();
			return (obj != null) ? obj.ToString() : null;
		}

		private static AccessCache Cache;

		private Type _type;

		private object _root;

		private MemberInfo _info;

		private MethodBase _method;

		private object[] _params;

		public static Action<Traverse, Traverse> CopyFields = delegate(Traverse from, Traverse to)
		{
			to.SetValue(from.GetValue());
		};
	}
}
