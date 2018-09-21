using System;
using System.Collections.Generic;
using System.Linq;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a C/C++ enumeration declaration.
	/// </summary>
	public class Enumeration : DeclarationContext, IEnumeration
	{
		/// <summary>
		/// Represents a C/C++ enumeration item.
		/// </summary>
		public class Item : Declaration, IEnumeration_Item
		{
			public ulong Value { get; set; }
			public string Expression { get; set; } = "";
			public bool ExplicitValue = true;

			public bool IsHexadecimal {
				get {
					if (Expression == null)
					{
						return false;
					}
					return Expression.Contains("0x") || Expression.Contains("0X");
				}
			}

			public override T Visit<T>(IDeclVisitor<T> visitor)
			{
				return visitor.VisitEnumItemDecl(this);
			}
		}

		public Enumeration()
		{
			Items = new List<Item>();
			ItemsByName = new Dictionary<string, Item>();
			BuiltinType = new BuiltinType(PrimitiveType.Int);
		}

		public Enumeration AddItem(Item item)
		{
			Items.Add(item);
			ItemsByName[item.Name] = item;
			return this;
		}

		public string GetItemValueAsString(Item item)
		{
			var printAsHex = item.IsHexadecimal && BuiltinType.IsUnsigned;
			var format = printAsHex ? "x" : string.Empty;
			var value = BuiltinType.IsUnsigned ? item.Value.ToString(format) :
				((long)item.Value).ToString(format);
			return printAsHex ? "0x" + value : value;
		}

		public Enumeration SetFlags()
		{
			Modifiers |= EnumModifiers.Flags;
			return this;
		}

		public bool IsFlags {
			get { return Modifiers.HasFlag(EnumModifiers.Flags); }
		}

		public Type Type { get; set; }
		IType IEnumeration.Type { get => Type; set => Type = (Type)value; }
		public BuiltinType BuiltinType { get; set; }
		IBuiltinType IEnumeration.BuiltinType { get => BuiltinType; set => BuiltinType = (BuiltinType)value; }
		public EnumModifiers Modifiers { get; set; }

		public List<Item> Items { get; }
		public int ItemCount => Items.Count;

		public Dictionary<string, Item> ItemsByName;

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitEnumDecl(this);
		}

		public IEnumeration_Item GetItem(int index)
		{
			return Items[index];
		}

		public void AddItem(IEnumeration_Item value)
		{
			Items.Add((Item)value);
		}

		public IEnumeration_Item FindItemByName(string Name)
		{
			return Items.FirstOrDefault(item => item.Name == Name);
		}
	}
}