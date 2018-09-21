using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CppSharp.AST.Extensions;

namespace CppSharp.AST
{
	/// <summary>
	/// Represents a C++ type.
	/// </summary>
	[DebuggerDisplay("{ToString()} [{GetType().Name}]")]
	public abstract class Type : ICloneable, IType
	{
		public static Func<Type, string> TypePrinterDelegate;

		public bool IsDependent { get; set; }
		public TypeKind Kind { get; set; }

		protected Type()
		{
		}

		protected Type(Type type)
		{
			IsDependent = type.IsDependent;
		}

		public abstract T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals
			= new TypeQualifiers());

		public string ToNativeString()
		{
			var cppTypePrinter = new CppTypePrinter { PrintScopeKind = TypePrintScopeKind.Qualified };
			return Visit(cppTypePrinter);
		}

		public override string ToString()
		{
			return TypePrinterDelegate(this);
		}

		public abstract object Clone();
	}

	/// <summary>
	/// Represents C++ type qualifiers.
	/// </summary>
	public struct TypeQualifiers : ITypeQualifiers
	{
		public bool IsConst { get; set; }
		public bool IsVolatile { get; set; }
		public bool IsRestrict { get; set; }
	}

	/// <summary>
	/// Represents a qualified C++ type.
	/// </summary>
	public struct QualifiedType : IQualifiedType
	{
		public QualifiedType(Type type)
			: this()
		{
			Type = type;
		}

		public QualifiedType(Type type, TypeQualifiers qualifiers)
			: this()
		{
			Type = type;
			Qualifiers = qualifiers;
		}

		public Type Type { get; set; }
		public TypeQualifiers Qualifiers { get; set; }
		IType IQualifiedType.Type { get => Type; set => Type = (Type)value; }
		ITypeQualifiers IQualifiedType.Qualifiers { get => Qualifiers; set => Qualifiers = (TypeQualifiers)value; }

		public T Visit<T>(ITypeVisitor<T> visitor)
		{
			return Type.Visit(visitor, Qualifiers);
		}

		public override string ToString()
		{
			return Type.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is QualifiedType)) return false;

			var type = (QualifiedType)obj;
			return Type.Equals(type.Type) && Qualifiers.Equals(type.Qualifiers);
		}

		public static bool operator ==(QualifiedType left, QualifiedType right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(QualifiedType left, QualifiedType right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a C++ tag type.
	/// </summary>
	public class TagType : Type, ITagType
	{
		public TagType()
		{
		}

		public TagType(Declaration decl)
		{
			Declaration = decl;
		}

		public TagType(TagType type)
			: base(type)
		{
			Declaration = type.Declaration;
		}

		public Declaration Declaration;

		IDeclaration ITagType.Declaration { get => Declaration; set => Declaration = (Declaration)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitTagType(this, quals);
		}

		public override object Clone()
		{
			return new TagType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as TagType;
			if (type == null) return false;

			return Declaration.Equals(type.Declaration);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents an C/C++ array type.
	/// </summary>
	public class ArrayType : Type, IArrayType
	{
		// Type of the array elements.
		public QualifiedType QualifiedType { get; set; }

		// Size type of array.
		public ArraySize SizeType { get; set; }

		// In case of a constant size array.
		public long Size { get; set; }

		// Size of the element type of the array.
		public long ElementSize { get; set; }

		public ArrayType()
		{
		}

		public ArrayType(ArrayType type)
			: base(type)
		{
			QualifiedType = new QualifiedType((Type)type.QualifiedType.Type.Clone(),
				type.QualifiedType.Qualifiers);
			SizeType = type.SizeType;
			Size = type.Size;
		}

		public Type Type {
			get { return QualifiedType.Type; }
		}

		IQualifiedType IArrayType.QualifiedType { get => QualifiedType; set => QualifiedType = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitArrayType(this, quals);
		}

		public override object Clone()
		{
			return new ArrayType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as ArrayType;
			if (type == null) return false;
			var equals = QualifiedType.Equals(type.QualifiedType) && SizeType.Equals(type.SizeType);

			if (SizeType == ArraySize.Constant)
				equals &= Size.Equals(type.Size);

			return equals;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents an C/C++ function type.
	/// </summary>
	public class FunctionType : Type, IFunctionType
	{
		// Return type of the function.
		public QualifiedType ReturnType;

		// Argument types.
		public List<Parameter> Parameters { get; } = new List<Parameter>();

		public CallingConvention CallingConvention { get; set; }

		public ExceptionSpecType ExceptionSpecType { get; set; }

		IQualifiedType IFunctionType.ReturnType { get => ReturnType; set => ReturnType = (QualifiedType)value; }

		public int ParameterCount => Parameters.Count;

		public FunctionType()
		{
		}

		public FunctionType(FunctionType type)
			: base(type)
		{
			ReturnType = new QualifiedType((Type)type.ReturnType.Type.Clone(), type.ReturnType.Qualifiers);
			Parameters.AddRange(type.Parameters.Select(p => new Parameter(p)));
			CallingConvention = type.CallingConvention;
			IsDependent = type.IsDependent;
		}

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitFunctionType(this, quals);
		}

		public override object Clone()
		{
			return new FunctionType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as FunctionType;
			if (type == null) return false;

			return ReturnType.Equals(type.ReturnType) && Parameters.SequenceEqual(type.Parameters);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public IParameter GetParameter(int index)
		{
			return Parameters[index];
		}

		public void AddParameter(IParameter parameter)
		{
			Parameters.Add((Parameter)parameter);
		}
	}

	/// <summary>
	/// Represents a C++ pointer/reference type.
	/// </summary>
	public class PointerType : Type, IPointerType
	{
		public PointerType(QualifiedType pointee = new QualifiedType())
		{
			Modifier = TypeModifier.Pointer;
			QualifiedPointee = pointee;
		}

		public PointerType()
		{
		}

		public PointerType(PointerType type)
			: base(type)
		{
			QualifiedPointee = new QualifiedType((Type)type.QualifiedPointee.Type.Clone(),
				type.QualifiedPointee.Qualifiers);
			Modifier = type.Modifier;
		}

		public bool IsReference {
			get {
				return Modifier == TypeModifier.LVReference ||
					Modifier == TypeModifier.RVReference;
			}
		}

		public QualifiedType QualifiedPointee;
		public Type Pointee { get { return QualifiedPointee.Type; } }

		public TypeModifier Modifier { get; set; }

		IQualifiedType IPointerType.QualifiedPointee { get => QualifiedPointee; set => QualifiedPointee = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitPointerType(this, quals);
		}

		public override object Clone()
		{
			return new PointerType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as PointerType;
			if (type == null) return false;

			return QualifiedPointee.Equals(type.QualifiedPointee)
				&& Modifier == type.Modifier;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a C++ member function pointer type.
	/// </summary>
	public class MemberPointerType : Type, IMemberPointerType
	{
		public QualifiedType QualifiedPointee;

		public MemberPointerType()
		{
		}

		public MemberPointerType(MemberPointerType type)
			: base(type)
		{
			QualifiedPointee = new QualifiedType((Type)type.QualifiedPointee.Type.Clone(),
				type.QualifiedPointee.Qualifiers);
		}

		public Type Pointee {
			get { return QualifiedPointee.Type; }
		}

		IQualifiedType IMemberPointerType.Pointee { get => QualifiedPointee; set => QualifiedPointee = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitMemberPointerType(this, quals);
		}

		public override object Clone()
		{
			return new MemberPointerType(this);
		}

		public override bool Equals(object obj)
		{
			var pointer = obj as MemberPointerType;
			if (pointer == null) return false;

			return QualifiedPointee.Equals(pointer.QualifiedPointee);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a C/C++ typedef type.
	/// </summary>
	public class TypedefType : Type, ITypedefType
	{
		public TypedefNameDecl Declaration { get; set; }

		ITypedefNameDecl ITypedefType.Declaration { get => Declaration; set => Declaration = (TypedefNameDecl)value; }

		public TypedefType()
		{
		}

		public TypedefType(TypedefNameDecl decl)
		{
			Declaration = decl;
		}

		public TypedefType(TypedefType type)
			: base(type)
		{
			Declaration = type.Declaration;
		}

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitTypedefType(this, quals);
		}

		public override object Clone()
		{
			return new TypedefType(this);
		}

		public override bool Equals(object obj)
		{
			var typedef = obj as TypedefType;
			return Declaration.Type.Equals(typedef == null ? obj : typedef.Declaration.Type);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// An attributed type is a type to which a type attribute has been
	/// applied.
	///
	/// For example, in the following attributed type:
	///     int32_t __attribute__((vector_size(16)))
	///
	/// The modified type is the TypedefType for int32_t
	/// The equivalent type is VectorType(16, int32_t)
	/// </summary>
	public class AttributedType : Type, IAttributedType
	{
		public QualifiedType Modified;

		public QualifiedType Equivalent;

		IQualifiedType IAttributedType.Modified { get => Modified; set => Modified = (QualifiedType)value; }
		IQualifiedType IAttributedType.Equivalent { get => Equivalent; set => Equivalent = (QualifiedType)value; }

		public AttributedType()
		{
		}

		public AttributedType(AttributedType type)
			: base(type)
		{
			Modified = new QualifiedType((Type)type.Modified.Type.Clone(), type.Modified.Qualifiers);
			Equivalent = new QualifiedType((Type)type.Equivalent.Type.Clone(), type.Equivalent.Qualifiers);
		}

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitAttributedType(this, quals);
		}

		public override object Clone()
		{
			return new AttributedType
			{
				IsDependent = IsDependent,
				Modified = new QualifiedType((Type)Modified.Type.Clone(), Modified.Qualifiers),
				Equivalent = new QualifiedType((Type)Equivalent.Type.Clone(), Equivalent.Qualifiers)
			};
		}

		public override bool Equals(object obj)
		{
			var attributed = obj as AttributedType;
			if (attributed == null) return false;

			return Modified.Equals(attributed.Modified)
				&& Equivalent.Equals(attributed.Equivalent);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a pointer type decayed from an array or function type.
	/// </summary>
	public class DecayedType : Type, IDecayedType
	{
		public QualifiedType Decayed;
		public QualifiedType Original;
		public QualifiedType Pointee;

		IQualifiedType IDecayedType.Decayed { get => Decayed; set => Decayed = (QualifiedType)value; }
		IQualifiedType IDecayedType.Original { get => Original; set => Original = (QualifiedType)value; }
		IQualifiedType IDecayedType.Pointee { get => Pointee; set => Pointee = (QualifiedType)value; }

		public DecayedType()
		{
		}

		public DecayedType(DecayedType type)
			: base(type)
		{
			Decayed = new QualifiedType((Type)type.Decayed.Type.Clone(), type.Decayed.Qualifiers);
			Original = new QualifiedType((Type)type.Original.Type.Clone(), type.Original.Qualifiers);
			Pointee = new QualifiedType((Type)type.Pointee.Type.Clone(), type.Pointee.Qualifiers);
		}

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitDecayedType(this, quals);
		}

		public override object Clone()
		{
			return new DecayedType(this);
		}

		public override bool Equals(object obj)
		{
			var decay = obj as DecayedType;
			if (decay == null) return false;

			return Original.Equals(decay.Original);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a template argument within a class template specialization.
	/// </summary>
	public struct TemplateArgument : ITemplateArgument
	{
		public ArgumentKind Kind { get; set; }
		public QualifiedType Type;
		public Declaration Declaration;
		public long Integral { get; set; }
		IQualifiedType ITemplateArgument.Type { get => Type; set => Type = (QualifiedType)value; }
		IDeclaration ITemplateArgument.Declaration { get => Declaration; set => Declaration = (Declaration)value; }

		public override bool Equals(object obj)
		{
			if (!(obj is TemplateArgument)) return false;
			var arg = (TemplateArgument)obj;

			if (Kind != arg.Kind) return false;

			switch (Kind)
			{
			case ArgumentKind.Type:
				return Type.Equals(arg.Type);
			case ArgumentKind.Declaration:
				return Declaration.Equals(arg.Declaration);
			case ArgumentKind.Integral:
				return Integral.Equals(arg.Integral);
			case ArgumentKind.Expression:
				return true;
			default:
				throw new Exception("Unknown TemplateArgument Kind");
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			switch (Kind)
			{
			case ArgumentKind.Type:
				return Type.ToString();
			case ArgumentKind.Declaration:
				return Declaration.ToString();
			case ArgumentKind.Integral:
				return Integral.ToString();
			case ArgumentKind.Expression:
				return string.Empty;
			default:
				throw new Exception("Unknown TemplateArgument Kind");
			}
		}
	}

	/// <summary>
	/// Represents a C++ template specialization type.
	/// </summary>
	public class TemplateSpecializationType : Type, ITemplateSpecializationType
	{
		public TemplateSpecializationType()
		{
			Arguments = new List<TemplateArgument>();
		}

		public TemplateSpecializationType(TemplateSpecializationType type)
			: base(type)
		{
			Arguments = type.Arguments.Select(
				t => new TemplateArgument
				{
					Declaration = t.Declaration,
					Integral = t.Integral,
					Kind = t.Kind,
					Type = new QualifiedType((Type)t.Type.Type.Clone(), t.Type.Qualifiers)
				}).ToList();
			Template = type.Template;
			if (type.Desugared.Type != null)
				Desugared = new QualifiedType((Type)type.Desugared.Type.Clone(),
					type.Desugared.Qualifiers);
		}

		public List<TemplateArgument> Arguments;

		public Template Template;

		public QualifiedType Desugared;

		public int ArgumentsCount => Arguments.Count;

		ITemplate ITemplateSpecializationType.Template { get => Template; set => Template = (Template)value; }

		IQualifiedType ITemplateSpecializationType.Desugared { get => Desugared; set => Desugared = (QualifiedType)value; }

		public ClassTemplateSpecialization GetClassTemplateSpecialization()
		{
			return GetDeclaration() as ClassTemplateSpecialization;
		}

		private Declaration GetDeclaration()
		{
			var finalType = Desugared.Type.GetFinalPointee() ?? Desugared.Type;

			var tagType = finalType as TagType;
			if (tagType != null)
				return tagType.Declaration;

			var injectedClassNameType = finalType as InjectedClassNameType;
			if (injectedClassNameType == null)
				return null;

			var injectedSpecializationType = (TemplateSpecializationType)
				injectedClassNameType.InjectedSpecializationType.Type;
			return injectedSpecializationType.GetDeclaration();
		}

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitTemplateSpecializationType(this, quals);
		}

		public override object Clone()
		{
			return new TemplateSpecializationType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as TemplateSpecializationType;
			if (type == null) return false;

			return Arguments.SequenceEqual(type.Arguments) &&
				((Template != null && Template.Name == type.Template.Name) ||
				(Desugared.Type != null && Desugared == type.Desugared));
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public ITemplateArgument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(ITemplateArgument value)
		{
			Arguments.Add((TemplateArgument)value);
		}
	}

	/// <summary>
	/// Represents a C++ dependent template specialization type.
	/// </summary>
	public class DependentTemplateSpecializationType : Type, IDependentTemplateSpecializationType
	{
		public DependentTemplateSpecializationType()
		{
			Arguments = new List<TemplateArgument>();
		}

		public DependentTemplateSpecializationType(DependentTemplateSpecializationType type)
			: base(type)
		{
			Arguments = type.Arguments.Select(
				t => new TemplateArgument
				{
					Declaration = t.Declaration,
					Integral = t.Integral,
					Kind = t.Kind,
					Type = new QualifiedType((Type)t.Type.Type.Clone(), t.Type.Qualifiers)
				}).ToList();
			Desugared = new QualifiedType((Type)type.Desugared.Type.Clone(), type.Desugared.Qualifiers);
		}

		public List<TemplateArgument> Arguments;

		public QualifiedType Desugared;

		public int ArgumentsCount => Arguments.Count;

		IQualifiedType IDependentTemplateSpecializationType.Desugared { get => Desugared; set => Desugared = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitDependentTemplateSpecializationType(this, quals);
		}

		public override object Clone()
		{
			return new DependentTemplateSpecializationType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as TemplateSpecializationType;
			if (type == null) return false;

			return Arguments.SequenceEqual(type.Arguments) &&
				Desugared == type.Desugared;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public ITemplateArgument GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(ITemplateArgument value)
		{
			Arguments.Add((TemplateArgument)value);
		}
	}

	/// <summary>
	/// Represents a C++ template parameter type.
	/// </summary>
	public class TemplateParameterType : Type, ITemplateParameterType
	{
		public TypeTemplateParameter Parameter;
		public uint Depth { get; set; }
		public uint Index { get; set; }
		public bool IsParameterPack { get; set; }
		ITypeTemplateParameter ITemplateParameterType.Parameter { get => Parameter; set => Parameter = (TypeTemplateParameter)value; }

		public TemplateParameterType()
		{
		}

		public TemplateParameterType(TemplateParameterType type)
			: base(type)
		{
			if (type.Parameter != null)
				Parameter = new TypeTemplateParameter
				{
					Constraint = type.Parameter.Constraint,
					Name = type.Parameter.Name
				};
			Depth = type.Depth;
			Index = type.Index;
			IsParameterPack = type.IsParameterPack;
		}

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitTemplateParameterType(this, quals);
		}

		public override object Clone()
		{
			return new TemplateParameterType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as TemplateParameterType;
			if (type == null) return false;

			return Parameter == type.Parameter
				&& Depth == type.Depth
				&& Index == type.Index
				&& IsParameterPack == type.IsParameterPack;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents the result of substituting a type for a template type parameter.
	/// </summary>
	public class TemplateParameterSubstitutionType : Type, ITemplateParameterSubstitutionType
	{
		public QualifiedType Replacement;

		public TemplateParameterType ReplacedParameter { get; set; }

		IQualifiedType ITemplateParameterSubstitutionType.Replacement { get => Replacement; set => Replacement = (QualifiedType)value; }

		ITemplateParameterType ITemplateParameterSubstitutionType.ReplacedParameter { get => ReplacedParameter; set => ReplacedParameter = (TemplateParameterType)value; }

		public TemplateParameterSubstitutionType()
		{
		}

		public TemplateParameterSubstitutionType(TemplateParameterSubstitutionType type)
			: base(type)
		{
			Replacement = new QualifiedType((Type)type.Replacement.Type.Clone(), type.Replacement.Qualifiers);
			ReplacedParameter = (TemplateParameterType)type.ReplacedParameter.Clone();
		}

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitTemplateParameterSubstitutionType(this, quals);
		}

		public override object Clone()
		{
			return new TemplateParameterSubstitutionType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as TemplateParameterSubstitutionType;
			if (type == null) return false;

			return Replacement.Equals(type.Replacement);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// The injected class name of a C++ class template or class template partial
	/// specialization.
	/// </summary>
	public class InjectedClassNameType : Type, IInjectedClassNameType
	{
		public TemplateSpecializationType TemplateSpecialization;
		public Class Class;

		public InjectedClassNameType()
		{
		}

		public InjectedClassNameType(InjectedClassNameType type)
			: base(type)
		{
			if (type.TemplateSpecialization != null)
				TemplateSpecialization = (TemplateSpecializationType)type.TemplateSpecialization.Clone();
			InjectedSpecializationType = new QualifiedType(
				(Type)type.InjectedSpecializationType.Type.Clone(),
				type.InjectedSpecializationType.Qualifiers);
			Class = type.Class;
		}

		public QualifiedType InjectedSpecializationType { get; set; }
		IQualifiedType IInjectedClassNameType.InjectedSpecializationType { get => InjectedSpecializationType; set => InjectedSpecializationType = (QualifiedType)value; }
		IClass IInjectedClassNameType.Class { get => Class; set => Class = (Class)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitInjectedClassNameType(this, quals);
		}

		public override object Clone()
		{
			return new InjectedClassNameType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as InjectedClassNameType;
			if (type == null) return false;
			if (TemplateSpecialization == null || type.TemplateSpecialization == null)
				return TemplateSpecialization == type.TemplateSpecialization;

			return TemplateSpecialization.Equals(type.TemplateSpecialization)
				&& Class.Equals(type.Class);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	/// <summary>
	/// Represents a qualified type name for which the type name is dependent.
	/// </summary>
	public class DependentNameType : Type, IDependentNameType
	{
		public DependentNameType()
		{
		}

		public DependentNameType(DependentNameType type)
			: base(type)
		{
		}

		public QualifiedType Qualifier { get; set; }

		public string Identifier { get; set; } = "";

		IQualifiedType IDependentNameType.Qualifier { get => Qualifier; set => Qualifier = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitDependentNameType(this, quals);
		}

		public override object Clone()
		{
			return new DependentNameType(this);
		}
	}

	/// <summary>
	/// Represents a CIL type.
	/// </summary>
	public class CILType : Type
	{
		public CILType(System.Type type)
		{
			Type = type;
		}

		public CILType()
		{
		}

		public CILType(CILType type)
			: base(type)
		{
			Type = type.Type;
		}

		public System.Type Type;

		public override T Visit<T>(ITypeVisitor<T> visitor,
								   TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitCILType(this, quals);
		}

		public override object Clone()
		{
			return new CILType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as CILType;
			if (type == null) return false;

			return Type == type.Type;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public class PackExpansionType : Type, IPackExpansionType
	{
		public PackExpansionType()
		{
		}

		public PackExpansionType(PackExpansionType type)
			: base(type)
		{
		}

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitPackExpansionType(this, quals);
		}

		public override object Clone()
		{
			return new PackExpansionType(this);
		}
	}

	public class UnaryTransformType : Type, IUnaryTransformType
	{
		public UnaryTransformType()
		{
		}

		public UnaryTransformType(UnaryTransformType type)
			: base(type)
		{
			Desugared = type.Desugared;
			BaseType = type.BaseType;
		}

		public QualifiedType Desugared { get; set; }
		public QualifiedType BaseType { get; set; }
		IQualifiedType IUnaryTransformType.Desugared { get => Desugared; set => Desugared = (QualifiedType)value; }
		IQualifiedType IUnaryTransformType.BaseType { get => BaseType; set => BaseType = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitUnaryTransformType(this, quals);
		}

		public override object Clone()
		{
			return new UnaryTransformType(this);
		}
	}

	public class VectorType : Type, IVectorType
	{
		public VectorType()
		{
		}

		public VectorType(VectorType type)
			: base(type)
		{
		}

		public QualifiedType ElementType { get; set; }
		public uint NumElements { get; set; }
		IQualifiedType IVectorType.ElementType { get => ElementType; set => ElementType = (QualifiedType)value; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitVectorType(this, quals);
		}

		public override object Clone()
		{
			return new VectorType(this);
		}
	}

	public class UnsupportedType : Type
	{
		public UnsupportedType()
		{
		}

		public UnsupportedType(string description)
		{
			Description = description;
		}

		public UnsupportedType(UnsupportedType type)
			: base(type)
		{
		}

		public string Description;

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitUnsupportedType(this, quals);
		}

		public override object Clone()
		{
			return new UnsupportedType(this);
		}
	}

	#region Primitives

	/// <summary>
	/// Represents an instance of a C++ built-in type.
	/// </summary>
	public class BuiltinType : Type, IBuiltinType
	{
		public BuiltinType()
		{
		}

		public BuiltinType(PrimitiveType type)
		{
			Type = type;
		}

		public BuiltinType(BuiltinType type)
			: base(type)
		{
			Type = type.Type;
		}

		public bool IsUnsigned {
			get {
				switch (Type)
				{
				case PrimitiveType.Bool:
				case PrimitiveType.UChar:
				case PrimitiveType.UShort:
				case PrimitiveType.UInt:
				case PrimitiveType.ULong:
				case PrimitiveType.ULongLong:
					return true;
				}

				return false;
			}
		}

		// Primitive type of built-in type.
		public PrimitiveType Type { get; set; }

		public override T Visit<T>(ITypeVisitor<T> visitor, TypeQualifiers quals = new TypeQualifiers())
		{
			return visitor.VisitBuiltinType(this, quals);
		}

		public override object Clone()
		{
			return new BuiltinType(this);
		}

		public override bool Equals(object obj)
		{
			var type = obj as BuiltinType;
			if (type == null) return false;

			return Type == type.Type;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	#endregion

	public interface ITypeVisitor<out T>
	{
		T VisitTagType(TagType tag, TypeQualifiers quals);
		T VisitArrayType(ArrayType array, TypeQualifiers quals);
		T VisitFunctionType(FunctionType function, TypeQualifiers quals);
		T VisitPointerType(PointerType pointer, TypeQualifiers quals);
		T VisitMemberPointerType(MemberPointerType member, TypeQualifiers quals);
		T VisitBuiltinType(BuiltinType builtin, TypeQualifiers quals);
		T VisitTypedefType(TypedefType typedef, TypeQualifiers quals);
		T VisitAttributedType(AttributedType attributed, TypeQualifiers quals);
		T VisitDecayedType(DecayedType decayed, TypeQualifiers quals);
		T VisitTemplateSpecializationType(TemplateSpecializationType template,
										  TypeQualifiers quals);
		T VisitDependentTemplateSpecializationType(
			DependentTemplateSpecializationType template, TypeQualifiers quals);
		T VisitPrimitiveType(PrimitiveType type, TypeQualifiers quals);
		T VisitDeclaration(Declaration decl, TypeQualifiers quals);
		T VisitTemplateParameterType(TemplateParameterType param,
			TypeQualifiers quals);
		T VisitTemplateParameterSubstitutionType(
			TemplateParameterSubstitutionType param, TypeQualifiers quals);
		T VisitInjectedClassNameType(InjectedClassNameType injected,
			TypeQualifiers quals);
		T VisitDependentNameType(DependentNameType dependent,
			TypeQualifiers quals);
		T VisitPackExpansionType(PackExpansionType packExpansionType, TypeQualifiers quals);
		T VisitUnaryTransformType(UnaryTransformType unaryTransformType, TypeQualifiers quals);
		T VisitVectorType(VectorType vectorType, TypeQualifiers quals);
		T VisitCILType(CILType type, TypeQualifiers quals);
		T VisitUnsupportedType(UnsupportedType type, TypeQualifiers quals);
	}
}
