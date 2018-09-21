using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a C++ virtual table component.
	/// </summary>
	[DebuggerDisplay("{Kind}, {Offset}, {Declaration}")]
	public struct VTableComponent : IVTableComponent
	{
		public VTableComponentKind Kind { get; set; }
		public ulong Offset { get; set; }
		public Declaration Declaration;

		/// Method declaration (if Kind == FunctionPointer).
		public Method Method {
			get {
				Debug.Assert(Kind == VTableComponentKind.FunctionPointer);
				return Declaration as Method;
			}
		}

		IDeclaration IVTableComponent.Declaration { get => Declaration; set => Declaration = (Declaration)value; }
	}

	/// <summary>
	/// Represents a C++ virtual table layout.
	/// </summary>
	public class VTableLayout : IVTableLayout
	{
		public List<VTableComponent> Components { get; set; }

		public int ComponentCount => Components.Count;

		public VTableLayout()
		{
			Components = new List<VTableComponent>();
		}

		public IVTableComponent GetComponent(int index)
		{
			return Components[index];
		}

		public void AddComponent(IVTableComponent value)
		{
			Components.Add((VTableComponent)value);
		}
	}

	/// <summary>
	/// Contains information about virtual function pointers.
	/// </summary>
	public struct VFTableInfo : IVFTableInfo
	{
		/// If nonzero, holds the vbtable index of the virtual base with the vfptr.
		public ulong VBTableIndex { get; set; }

		/// This is the offset of the vfptr from the start of the last vbase,
		/// or the complete type if there are no virtual bases.
		public long VFPtrOffset { get; set; }

		/// This is the full offset of the vfptr from the start of the complete type.
		public long VFPtrFullOffset { get; set; }
		IVTableLayout IVFTableInfo.Layout { get => Layout; set => Layout = (VTableLayout)value; }

		/// Layout of the table at this pointer.
		public VTableLayout Layout;
	}

	// Represents ABI-specific layout details for a class.
	public class ClassLayout : IClassLayout
	{
		public CppAbi ABI { get; set; }

		/// Virtual function tables in Microsoft mode.
		public List<VFTableInfo> VFTables { get; set; }

		/// Virtual table layout in Itanium mode.
		public VTableLayout Layout { get; set; }

		public ClassLayout()
		{
			VFTables = new List<VFTableInfo>();
			Fields = new List<LayoutField>();
			Bases = new List<LayoutBase>();
		}

		public List<LayoutField> Fields { get; private set; }
		public List<LayoutBase> Bases { get; private set; }

		public ClassLayout(ClassLayout classLayout)
			: this()
		{
			ABI = classLayout.ABI;
			HasOwnVFPtr = classLayout.HasOwnVFPtr;
			VBPtrOffset = classLayout.VBPtrOffset;
			PrimaryBase = classLayout.PrimaryBase;
			HasVirtualBases = classLayout.HasVirtualBases;
			Alignment = classLayout.Alignment;
			Size = classLayout.Size;
			DataSize = classLayout.DataSize;
			VFTables.AddRange(classLayout.VFTables);
			if (classLayout.Layout != null)
			{
				Layout = new VTableLayout();
				Layout.Components.AddRange(classLayout.Layout.Components);
			}
		}

		/// <summary>
		/// Does this class provide its own virtual-function table
		/// pointer, rather than inheriting one from a primary base
		/// class? If so, it is at offset zero.
		/// </summary>
		public bool HasOwnVFPtr { get; set; }

		/// <summary>
		/// Get the offset for virtual base table pointer.
		/// This is only meaningful with the Microsoft ABI.
		/// </summary>
		public long VBPtrOffset { get; set; }

		/// <summary>
		/// Primary base for this record.
		/// </summary>
		public Class PrimaryBase;

		public bool HasVirtualBases { get; set; }

		public int Alignment { get; set; }
		public int Size { get; set; }
		public int DataSize { get; set; }

		public IList<LayoutField> VTablePointers {
			get {
				if (vTablePointers == null)
				{
					vTablePointers = new List<LayoutField>(Fields.Where(f => f.IsVTablePtr));
				}
				return vTablePointers;
			}
		}

		public int VFTableCount => VFTables.Count;

		IVTableLayout IClassLayout.Layout { get => Layout; set => Layout = (VTableLayout)value; }

		public int FieldCount => Fields.Count;

		public int BaseCount => Bases.Count;

		private List<LayoutField> vTablePointers;

		public IVFTableInfo GetVFTable(int index)
		{
			return VFTables[index];
		}

		public void AddVFTable(IVFTableInfo value)
		{
			VFTables.Add((VFTableInfo)value);
		}

		public ILayoutField GetField(int index)
		{
			return Fields[index];
		}

		public void AddField(ILayoutField value)
		{
			Fields.Add((LayoutField)value);
		}

		public ILayoutBase GetBase(int index)
		{
			return Bases[index];
		}

		public void AddBase(ILayoutBase value)
		{
			Bases.Add((LayoutBase)value);
		}
	}
}
