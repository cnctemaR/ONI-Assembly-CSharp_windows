using System;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	public abstract class PropertyBag<TContainer> : IPropertyBag<TContainer>, IPropertyBag, IPropertyBagRegister, IConstructor<TContainer>, IConstructor
	{
		static PropertyBag()
		{
			bool flag = !TypeTraits.IsContainer(typeof(TContainer));
			if (flag)
			{
				throw new InvalidOperationException(string.Format("Failed to create a property bag for Type=[{0}]. The type is not a valid container type.", typeof(TContainer)));
			}
		}

		void IPropertyBagRegister.Register()
		{
			PropertyBagStore.AddPropertyBag<TContainer>(this);
		}

		public void Accept(ITypeVisitor visitor)
		{
			bool flag = visitor == null;
			if (flag)
			{
				throw new ArgumentNullException("visitor");
			}
			visitor.Visit<TContainer>();
		}

		void IPropertyBag.Accept(IPropertyBagVisitor visitor, ref object container)
		{
			bool flag = container == null;
			if (flag)
			{
				throw new ArgumentNullException("container");
			}
			object obj = container;
			TContainer tcontainer;
			int num;
			if (obj is TContainer)
			{
				tcontainer = (TContainer)((object)obj);
				num = 1;
			}
			else
			{
				num = 0;
			}
			bool flag2 = num == 0;
			if (flag2)
			{
				throw new ArgumentException(string.Format("The given ContainerType=[{0}] does not match the PropertyBagType=[{1}]", container.GetType(), typeof(TContainer)));
			}
			PropertyBag.AcceptWithSpecializedVisitor<TContainer>(this, visitor, ref tcontainer);
			container = tcontainer;
		}

		void IPropertyBag<TContainer>.Accept(IPropertyBagVisitor visitor, ref TContainer container)
		{
			visitor.Visit<TContainer>(this, ref container);
		}

		PropertyCollection<TContainer> IPropertyBag<TContainer>.GetProperties()
		{
			return this.GetProperties();
		}

		PropertyCollection<TContainer> IPropertyBag<TContainer>.GetProperties(ref TContainer container)
		{
			return this.GetProperties(ref container);
		}

		InstantiationKind IConstructor.InstantiationKind
		{
			get
			{
				return this.InstantiationKind;
			}
		}

		TContainer IConstructor<TContainer>.Instantiate()
		{
			return this.Instantiate();
		}

		public abstract PropertyCollection<TContainer> GetProperties();

		public abstract PropertyCollection<TContainer> GetProperties(ref TContainer container);

		protected virtual InstantiationKind InstantiationKind { get; } = InstantiationKind.Activator;

		protected virtual TContainer Instantiate()
		{
			return default(TContainer);
		}

		public TContainer CreateInstance()
		{
			return TypeUtility.Instantiate<TContainer>();
		}

		public bool TryCreateInstance(out TContainer instance)
		{
			return TypeUtility.TryInstantiate<TContainer>(out instance);
		}
	}
}
