using System;
using System.Collections.Generic;
using System.Linq;
using CppSharp.AST.Extensions;

namespace CppSharp.AST
{
	// A C++ access specifier declaration.
	public class AccessSpecifierDecl : Declaration, IAccessSpecifierDecl
	{
		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			throw new NotImplementedException();
		}
	}

	// Represents a base class of a C++ class.
	public class BaseClassSpecifier : IBaseClassSpecifier
	{
		public BaseClassSpecifier()
		{
		}

		public BaseClassSpecifier(BaseClassSpecifier other)
		{
			Access = other.Access;
			IsVirtual = other.IsVirtual;
			Type = other.Type;
			Offset = other.Offset;
		}

		public AccessSpecifier Access { get; set; }
		public bool IsVirtual { get; set; }
		public Type Type { get; set; }
		public int Offset { get; set; }

		public Class Class {
			get {
				Class @class;
				Type.TryGetClass(out @class);
				return @class;
			}
		}

		public bool IsClass {
			get {
				return Type.IsClass();
			}
		}

		IType IBaseClassSpecifier.Type { get => Type; set => Type = (Type)value; }
	}

	public enum ClassType
	{
		ValueType,
		RefType,
		Interface
	}

	// Represents a C++ record Decl.
	public class Class : DeclarationContext, IClass
	{
		public List<BaseClassSpecifier> Bases;
		public List<Field> Fields;
		public List<Property> Properties;
		public List<Method> Methods;
		public List<AccessSpecifierDecl> Specifiers;

		// True if the record is a POD (Plain Old Data) type.
		public bool IsPOD { get; set; }

		// Semantic type of the class.
		public ClassType Type;

		// ABI-specific class layout.
		public ClassLayout Layout;

		// True if class provides pure virtual methods.
		public bool IsAbstract { get; set; }

		// True if the type is to be treated as a union.
		public bool IsUnion { get; set; }

		// True if the class is final / sealed.
		public bool IsFinal { get; set; }

		private bool? isOpaque = null;

		public bool IsInjected { get; set; }

		// True if the type is to be treated as opaque.
		public bool IsOpaque {
			get {
				return isOpaque == null ? IsIncomplete && CompleteDeclaration == null : isOpaque.Value;
			}
			set {
				isOpaque = value;
			}
		}

		// True if the class is dynamic.
		public bool IsDynamic { get; set; }

		// True if the class is polymorphic.
		public bool IsPolymorphic { get; set; }

		// True if the class has a non trivial default constructor.
		public bool HasNonTrivialDefaultConstructor { get; set; }

		// True if the class has a non trivial copy constructor.
		public bool HasNonTrivialCopyConstructor { get; set; }

		// True if the class has a non trivial destructor.
		public bool HasNonTrivialDestructor { get; set; }

		// True if the class represents a static class.
		public bool IsStatic { get; set; }

		public Class()
		{
			Bases = new List<BaseClassSpecifier>();
			Fields = new List<Field>();
			Properties = new List<Property>();
			Methods = new List<Method>();
			Specifiers = new List<AccessSpecifierDecl>();
			IsAbstract = false;
			IsUnion = false;
			IsFinal = false;
			IsPOD = false;
			Type = ClassType.RefType;
			Layout = new ClassLayout();
			templateParameters = new List<Declaration>();
			specializations = new List<ClassTemplateSpecialization>();
		}

		public bool HasBase {
			get { return Bases.Count > 0; }
		}

		public bool HasBaseClass {
			get { return BaseClass != null; }
		}

		public Class BaseClass {
			get {
				foreach (var @base in Bases)
				{
					if (@base.IsClass && @base.Class.IsGenerated)
						return @base.Class;
				}

				return null;
			}
		}

		public bool HasNonIgnoredBase {
			get {
				return HasBaseClass && !IsValueType
					   && Bases[0].Class != null
					   && !Bases[0].Class.IsValueType
					   && Bases[0].Class.IsGenerated;
			}
		}

		public bool NeedsBase {
			get { return HasNonIgnoredBase && IsGenerated; }
		}

		// When we have an interface, this is the class mapped to that interface.
		public Class OriginalClass { get; set; }

		public bool IsValueType {
			get { return Type == ClassType.ValueType || IsUnion; }
		}

		public bool IsRefType {
			get { return Type == ClassType.RefType && !IsUnion; }
		}

		public bool IsInterface {
			get { return Type == ClassType.Interface; }
		}

		public bool IsAbstractImpl {
			get { return Methods.Any(m => m.SynthKind == FunctionSynthKind.AbstractImplCall); }
		}

		public IEnumerable<Method> Constructors {
			get {
				return Methods.Where(
					method => method.IsConstructor || method.IsCopyConstructor);
			}
		}

		public IEnumerable<Method> Destructors {
			get {
				return Methods.Where(method => method.IsDestructor);
			}
		}

		public IEnumerable<Method> Operators {
			get {
				return Methods.Where(method => method.IsOperator);
			}
		}

		/// <summary>
		/// If this class is a template, this list contains all of its template parameters.
		/// <para>
		/// <see cref="ClassTemplate"/> cannot be relied upon to contain all of them because
		/// ClassTemplateDecl in Clang is not a complete declaration, it only serves to forward template classes.
		/// </para>
		/// </summary>
		public List<Declaration> TemplateParameters {
			get {
				if (!IsDependent)
					throw new InvalidOperationException(
						"Only dependent classes have template parameters.");
				return templateParameters;
			}
		}

		/// <summary>
		/// If this class is a template, this list contains all of its specializations.
		/// <see cref="ClassTemplate"/> cannot be relied upon to contain all of them because
		/// ClassTemplateDecl in Clang is not a complete declaration, it only serves to forward template classes.
		/// </summary>
		public List<ClassTemplateSpecialization> Specializations {
			get {
				if (!IsDependent)
					throw new InvalidOperationException(
						"Only dependent classes have specializations.");
				return specializations;
			}
		}

		public bool IsTemplate {
			get { return IsDependent && TemplateParameters.Count > 0; }
		}

		public int BaseCount => Bases.Count;

		public int FieldCount => Fields.Count;

		public int MethodCount => Methods.Count;

		public int SpecifierCount => Specifiers.Count;

		IClassLayout IClass.Layout { get => Layout; set => Layout = (ClassLayout)value; }

		public override IEnumerable<Function> FindOperator(CXXOperatorKind kind)
		{
			return Methods.Where(m => m.OperatorKind == kind);
		}

		public override IEnumerable<Function> GetOverloads(Function function)
		{
			if (function.IsOperator)
				return Methods.Where(fn => fn.OperatorKind == function.OperatorKind);

			var overloads = Methods.Where(m => m.Name == function.Name)
				.Union(Declarations.Where(d => d is Function && d.Name == function.Name))
				.Cast<Function>();

			overloads = overloads.Union(base.GetOverloads(function));

			return overloads;
		}

		public Method FindMethod(string name)
		{
			return Methods
				.Concat(Templates.OfType<FunctionTemplate>()
					.Select(t => t.TemplatedFunction)
					.OfType<Method>())
				.FirstOrDefault(m => m.Name == name);
		}

		public Method FindMethodByUSR(string usr)
		{
			return Methods
				.Concat(Templates.OfType<FunctionTemplate>()
					.Select(t => t.TemplatedFunction)
					.OfType<Method>())
				.FirstOrDefault(m => m.USR == usr);
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitClassDecl(this);
		}

		public IBaseClassSpecifier GetBase(int index)
		{
			return Bases[index];
		}

		public void AddBase(IBaseClassSpecifier value)
		{
			Bases.Add((BaseClassSpecifier)value);
		}

		public IField GetField(int index)
		{
			return Fields[index];
		}

		public void AddField(IField value)
		{
			Fields.Add((Field)value);
		}

		public IMethod GetMethod(int index)
		{
			return Methods[index];
		}

		public void AddMethod(IMethod value)
		{
			Methods.Add((Method)value);
		}

		public IAccessSpecifierDecl GetSpecifier(int index)
		{
			return Specifiers[index];
		}

		public void AddSpecifier(IAccessSpecifierDecl value)
		{
			Specifiers.Add((AccessSpecifierDecl)value);
		}

		private List<Declaration> templateParameters;
		private List<ClassTemplateSpecialization> specializations;
	}
}
