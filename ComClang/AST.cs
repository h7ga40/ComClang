using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	#region Types

	[ComVisible(true)]
	public enum TypeKind
	{
		Tag,
		Array,
		Function,
		Pointer,
		MemberPointer,
		Typedef,
		Attributed,
		Decayed,
		TemplateSpecialization,
		DependentTemplateSpecialization,
		TemplateParameter,
		TemplateParameterSubstitution,
		InjectedClassName,
		DependentName,
		PackExpansion,
		Builtin,
		UnaryTransform,
		Vector
	}

	[ComVisible(true)]
	public class Type
	{
		public Type(TypeKind kind)
		{
		}

		public TypeKind kind { get; set; }
		public bool isDependent { get; set; }
	}

	[ComVisible(true)]
	public class TypeQualifiers
	{
		public bool isConst { get; set; }
		public bool isVolatile { get; set; }
		public bool isRestrict { get; set; }
	}

	[ComVisible(true)]
	public class QualifiedType
	{
		public QualifiedType() { }
		public Type type { get; set; }
		public TypeQualifiers qualifiers { get; set; }
	}

	[ComVisible(true)]
	public class TagType : Type
	{
		public TagType()
			: base(TypeKind.Tag)
		{
		}

		public Declaration declaration { get; set; }
	}

	[ComVisible(true)]
	public class ArrayType : Type
	{
		[ComVisible(true)]
		public enum ArraySize
		{
			Constant,
			Variable,
			Dependent,
			Incomplete
		}

		public ArrayType() : base(TypeKind.Array) { }

		public QualifiedType qualifiedType { get; set; }
		public ArraySize sizeType { get; set; }
		public long size { get; set; }
		public long elementSize { get; set; }
	}

	[ComVisible(true)]
	public enum CallingConvention
	{
		Default,
		C,
		StdCall,
		ThisCall,
		FastCall,
		Unknown
	}

	[ComVisible(true)]
	public enum ExceptionSpecType
	{
		None,
		DynamicNone,
		Dynamic,
		MSAny,
		BasicNoexcept,
		ComputedNoexcept,
		Unevaluated,
		Uninstantiated,
		Unparsed
	}

	[ComVisible(true)]
	public class FunctionType : Type
	{
		public FunctionType() : base(TypeKind.Function) { }
		public QualifiedType returnType { get; set; }
		public CallingConvention callingConvention { get; set; }
		public ExceptionSpecType exceptionSpecType { get; set; }
		public List<Parameter> Parameters { get; } = new List<Parameter>();
		public void AddParameters(Parameter parameter) { Parameters.Add(parameter); }
	}

	[ComVisible(true)]
	public class PointerType : Type
	{
		[ComVisible(true)]
		public enum TypeModifier
		{
			Value,
			Pointer,
			LVReference,
			RVReference
		}

		public PointerType() : base(TypeKind.Pointer) { }
		public QualifiedType qualifiedPointee { get; set; }
		public TypeModifier modifier { get; set; }
	}

	[ComVisible(true)]
	public class MemberPointerType : Type
	{
		public MemberPointerType() : base(TypeKind.MemberPointer) { }
		public QualifiedType pointee { get; set; }
	}

	[ComVisible(true)]
	public class TypedefType : Type
	{
		public TypedefType() : base(TypeKind.Typedef) { }
		public TypedefNameDecl declaration { get; set; }
	}

	[ComVisible(true)]
	public class AttributedType : Type
	{
		public AttributedType() : base(TypeKind.Attributed) { }
		public QualifiedType modified { get; set; }
		public QualifiedType equivalent { get; set; }
	}

	[ComVisible(true)]
	public class DecayedType : Type
	{
		public DecayedType() : base(TypeKind.Decayed) { }
		public QualifiedType decayed { get; set; }
		public QualifiedType original { get; set; }
		public QualifiedType pointee { get; set; }
	}

	[ComVisible(true)]
	public class TemplateArgument
	{
		public TemplateArgument() { }

		[ComVisible(true)]
		public enum ArgumentKind
		{
			Type,
			Declaration,
			NullPtr,
			Integral,
			Template,
			TemplateExpansion,
			Expression,
			Pack
		}

		public ArgumentKind kind { get; set; }
		public QualifiedType type { get; set; }
		public Declaration declaration { get; set; }
		public long integral { get; set; }
	}

	[ComVisible(true)]
	public class TemplateSpecializationType : Type
	{
		public TemplateSpecializationType() : base(TypeKind.TemplateSpecialization) { }

		public List<TemplateArgument> Arguments { get; } = new List<TemplateArgument>();
		public void AddArguments(TemplateArgument value) { Arguments.Add(value); }

		public Template _template { get; set; }
		public QualifiedType desugared { get; set; }
	}

	[ComVisible(true)]
	public class DependentTemplateSpecializationType : Type
	{
		public DependentTemplateSpecializationType() : base(TypeKind.TemplateParameterSubstitution) { }

		public List<TemplateArgument> Arguments { get; } = new List<TemplateArgument>();
		public void AddArguments(TemplateArgument value) { Arguments.Add(value); }

		public QualifiedType desugared { get; set; }
	}

	[ComVisible(true)]
	public class TemplateParameterType : Type
	{
		public TemplateParameterType() : base(TypeKind.TemplateParameter) { }
		public TypeTemplateParameter parameter { get; set; }
		public uint depth { get; set; }
		public uint index { get; set; }
		bool isParameterPack { get; set; }
	}

	[ComVisible(true)]
	public class TemplateParameterSubstitutionType : Type
	{
		public TemplateParameterSubstitutionType() : base(TypeKind.TemplateParameterSubstitution) { }
		public QualifiedType replacement { get; set; }
		public TemplateParameterType replacedParameter { get; set; }
	}

	[ComVisible(true)]
	public class InjectedClassNameType : Type
	{
		public InjectedClassNameType() : base(TypeKind.InjectedClassName) { }
		public QualifiedType injectedSpecializationType { get; set; }
		public Class _class { get; set; }
	}

	[ComVisible(true)]
	public class DependentNameType : Type
	{
		public DependentNameType() : base(TypeKind.DependentName) { }
		public QualifiedType qualifier { get; set; }
		public string identifier { get; set; }
	}

	[ComVisible(true)]
	public class PackExpansionType : Type
	{
		public PackExpansionType() : base(TypeKind.PackExpansion) { }
	}

	[ComVisible(true)]
	public class UnaryTransformType : Type
	{
		public UnaryTransformType() : base(TypeKind.UnaryTransform) { }
		public QualifiedType desugared { get; set; }
		public QualifiedType baseType { get; set; }
	}

	[ComVisible(true)]
	public class VectorType : Type
	{
		public VectorType() : base(TypeKind.Vector) { }
		public QualifiedType elementType { get; set; }
		public uint numElements { get; set; }
	}

	[ComVisible(true)]
	public enum PrimitiveType
	{
		Null,
		Void,
		Bool,
		WideChar,
		Char,
		SChar,
		UChar,
		Char16,
		Char32,
		Short,
		UShort,
		Int,
		UInt,
		Long,
		ULong,
		LongLong,
		ULongLong,
		Int128,
		UInt128,
		Half,
		Float,
		Double,
		LongDouble,
		Float128,
		IntPtr
	}

	[ComVisible(true)]
	public class BuiltinType : Type
	{
		public BuiltinType() : base(TypeKind.Builtin) { }
		PrimitiveType type;
	}

	#endregion

	#region ABI

	[ComVisible(true)]
	public enum CppAbi
	{
		Itanium,
		Microsoft,
		ARM,
		iOS,
		iOS64
	}

	[ComVisible(true)]
	public enum VTableComponentKind
	{
		VCallOffset,
		VBaseOffset,
		OffsetToTop,
		RTTI,
		FunctionPointer,
		CompleteDtorPointer,
		DeletingDtorPointer,
		UnusedFunctionPointer,
	}

	[ComVisible(true)]
	public class VTableComponent
	{
		public VTableComponent() { }
		public VTableComponentKind kind { get; set; }
		public uint offset { get; set; }
		public Declaration declaration { get; set; }
	}

	[ComVisible(true)]
	public class VTableLayout
	{
		public VTableLayout() { }
		public List<VTableComponent> Components { get; } = new List<VTableComponent>();
		public void AddComponents(VTableComponent value) { Components.Add(value); }

	}

	[ComVisible(true)]
	public class VFTableInfo
	{
		public VFTableInfo() { }
		public ulong VBTableIndex { get; set; }
		public uint VFPtrOffset { get; set; }
		public uint VFPtrFullOffset { get; set; }
		public VTableLayout layout { get; set; }
	}

	[ComVisible(true)]
	public class LayoutField
	{
		public LayoutField() { }
		public uint offset { get; set; }
		public string name { get; set; }
		public QualifiedType qualifiedType { get; set; }
		public IntPtr fieldPtr { get; set; }
	}

	[ComVisible(true)]
	public class LayoutBase
	{
		public LayoutBase() { }
		public uint offset { get; set; }
		public Class _class { get; set; }
	}

	[ComVisible(true)]
	public class ClassLayout
	{
		public ClassLayout() { }
		public CppAbi ABI { get; set; }
		public List<VFTableInfo> VFTables { get; } = new List<VFTableInfo>();
		public void AddVFTables(VFTableInfo value) { VFTables.Add(value); }

		public VTableLayout layout { get; set; }
		public bool hasOwnVFPtr { get; set; }
		public long VBPtrOffset { get; set; }
		public int alignment { get; set; }
		public int size { get; set; }
		public int dataSize { get; set; }
		public List<LayoutField> Fields { get; } = new List<LayoutField>();
		public void AddFields(LayoutField value) { Fields.Add(value); }
		public List<LayoutBase> Bases { get; } = new List<LayoutBase>();
		public void AddBases(LayoutBase value) { Bases.Add(value); }
	}

	#endregion

	#region Declarations

	[ComVisible(true)]
	public enum DeclarationKind
	{
		DeclarationContext,
		Typedef,
		TypeAlias,
		Parameter,
		Function,
		Method,
		Enumeration,
		EnumerationItem,
		Variable,
		Field,
		AccessSpecifier,
		Class,
		Template,
		TypeAliasTemplate,
		ClassTemplate,
		ClassTemplateSpecialization,
		ClassTemplatePartialSpecialization,
		FunctionTemplate,
		Namespace,
		PreprocessedEntity,
		MacroDefinition,
		MacroExpansion,
		TranslationUnit,
		Friend,
		TemplateTemplateParm,
		TemplateTypeParm,
		NonTypeTemplateParm,
		VarTemplate,
		VarTemplateSpecialization,
		VarTemplatePartialSpecialization
	}

	[ComVisible(true)]
	public enum AccessSpecifier
	{
		Private,
		Protected,
		Public
	}

	[ComVisible(true)]
	public class Declaration
	{
		public Declaration(DeclarationKind kind) { }

		public DeclarationKind kind { get; set; }
		public int maxFieldAlignment { get; set; }
		public AccessSpecifier access { get; set; }
		public DeclarationContext _namespace { get; set; }
		public ISourceLocation location { get; set; }
		public int lineNumberStart { get; set; }
		public int lineNumberEnd { get; set; }
		public string name { get; set; }
		public string USR { get; set; }
		public string debugText { get; set; }
		public bool isIncomplete { get; set; }
		public bool isDependent { get; set; }
		public bool isImplicit { get; set; }
		public bool isInvalid { get; set; }
		public Declaration completeDeclaration { get; set; }
		public uint definitionOrder { get; set; }
		public List<PreprocessedEntity> PreprocessedEntities { get; } = new List<PreprocessedEntity>();
		public void AddPreprocessedEntities(PreprocessedEntity value) { PreprocessedEntities.Add(value); }
		public List<Declaration> Redeclarations { get; } = new List<Declaration>();
		public void AddRedeclarations(Declaration value) { Redeclarations.Add(value); }
		public IntPtr originalPtr { get; set; }
		public RawComment comment { get; set; }
	}

	[ComVisible(true)]
	public class DeclarationContext : Declaration
	{
		public DeclarationContext(DeclarationKind kind) : base(kind) { }
		public Declaration FindAnonymous(string USR) { return null; }

		public Namespace FindNamespace(string Name) { return null; }
		public Namespace FindNamespace(string[] names) { return null; }
		public Namespace FindCreateNamespace(string Name) { return null; }

		public Class CreateClass(string Name, bool IsComplete) { return null; }
		public Class FindClass(IntPtr OriginalPtr, string Name, bool IsComplete) { return null; }
		public Class FindClass(IntPtr OriginalPtr, string Name, bool IsComplete, bool Create) { return null; }

		public T FindTemplate<T>(string USR) where T : Template
		{
			var foundTemplate = Templates.FirstOrDefault((t) =>
			{
				return t.USR == USR;
			});

			if (foundTemplate != null)
				return (T)(foundTemplate);

			return null;
		}

		Enumeration FindEnum(IntPtr OriginalPtr) { return null; }
		Enumeration FindEnum(string Name, bool Create = false) { return null; }
		Enumeration FindEnumWithItem(string Name) { return null; }

		Function FindFunction(string USR) { return null; }

		TypedefDecl FindTypedef(string Name, bool Create = false) { return null; }

		TypeAlias FindTypeAlias(string Name, bool Create = false) { return null; }

		Variable FindVariable(string USR) { return null; }

		Friend FindFriend(string USR) { return null; }

		public List<Namespace> Namespaces { get; } = new List<Namespace>();
		public void AddNamespaces(Namespace value) { Namespaces.Add(value); }

		public List<Enumeration> Enums { get; } = new List<Enumeration>();
		public void AddEnums(Enumeration value) { Enums.Add(value); }

		public List<Function> Functions { get; } = new List<Function>();
		public void AddFunctions(Function value) { Functions.Add(value); }

		public List<Class> Classes { get; } = new List<Class>();
		public void AddClasses(Class value) { Classes.Add(value); }

		public List<Template> Templates { get; } = new List<Template>();
		public void AddTemplates(Template value) { Templates.Add(value); }

		public List<TypedefDecl> Typedefs { get; } = new List<TypedefDecl>();
		public void AddTypedefs(TypedefDecl value) { Typedefs.Add(value); }

		public List<TypeAlias> TypeAliases { get; } = new List<TypeAlias>();
		public void AddTypeAliases(TypeAlias value) { TypeAliases.Add(value); }

		public List<Variable> Variables { get; } = new List<Variable>();
		public void AddVariables(Variable value) { Variables.Add(value); }

		public List<Friend> Friends { get; } = new List<Friend>();
		public void AddFriends(Friend value) { Friends.Add(value); }


		public Dictionary<string, Declaration> anonymous { get; } = new Dictionary<string, Declaration>();
		public void AddAnonymous(string p1, Declaration p2) { anonymous.Add(p1, p2); }
		public bool isAnonymous { get; set; }
	}

	[ComVisible(true)]
	public class TypedefNameDecl : Declaration
	{
		public TypedefNameDecl(DeclarationKind kind) : base(kind) { }
		public QualifiedType qualifiedType { get; set; }
	}

	[ComVisible(true)]
	public class TypedefDecl : TypedefNameDecl
	{
		public TypedefDecl() : base(DeclarationKind.Typedef) { }
	}

	[ComVisible(true)]
	public class TypeAlias : TypedefNameDecl
	{
		public TypeAlias() : base(DeclarationKind.TypeAlias) { }
		public TypeAliasTemplate describedAliasTemplate { get; set; }
	}

	[ComVisible(true)]
	public class Friend : Declaration
	{
		public Friend() : base(DeclarationKind.Friend) { }
		public Declaration declaration { get; set; }
	}

	[ComVisible(true)]
	public enum StatementClass
	{
		Any,
		BinaryOperator,
		CallExprClass,
		DeclRefExprClass,
		CXXConstructExprClass,
		CXXOperatorCallExpr,
		ImplicitCastExpr,
		ExplicitCastExpr,
		InitListExprClass,
		SubStmtExpr,
	}

	[ComVisible(true)]
	public class Statement
	{
		public Statement(string str, StatementClass Class = StatementClass.Any, Declaration decl = null)
		{
		}

		public StatementClass _class { get; set; }
		public Declaration decl { get; set; }
		public string @string { get; set; }
	}

	[ComVisible(true)]
	public class Expression : Statement
	{
		public Expression(string str, StatementClass Class = StatementClass.Any, Declaration decl = null)
			: base(str, Class, decl)
		{

		}
	}

	[ComVisible(true)]
	public class BinaryOperator : Expression
	{
		public BinaryOperator(string str, Expression lhs, Expression rhs, string opcodeStr)
			: base(str, StatementClass.BinaryOperator)
		{
		}

		public Expression LHS { get; set; }
		public Expression RHS { get; set; }
		public string opcodeStr { get; set; }
	}

	[ComVisible(true)]
	public class CallExpr : Expression
	{
		public CallExpr(string str, Declaration decl)
			: base(str, StatementClass.CallExprClass, decl)
		{
		}

		public List<Expression> Arguments { get; } = new List<Expression>();
		public void AddArguments(Expression value) { Arguments.Add(value); }
	}

	[ComVisible(true)]
	public class CXXConstructExpr : Expression
	{
		public CXXConstructExpr(string str, Declaration decl = null)
		: base(str, StatementClass.CXXConstructExprClass, decl)
		{
		}

		public List<Expression> Arguments { get; } = new List<Expression>();
		public void AddArguments(Expression value) { Arguments.Add(value); }
	}

	[ComVisible(true)]
	public class InitListExpr : Expression
	{
		public InitListExpr(string str, Declaration decl = null)
			: base(str, StatementClass.InitListExprClass, decl)
		{
		}

		public List<Expression> Inits { get; } = new List<Expression>();
		public void AddInits(Expression value) { Inits.Add(value); }

	}

	[ComVisible(true)]
	public class SubStmtExpr : Expression
	{
		public SubStmtExpr(string str, Declaration decl = null)
			: base(str, StatementClass.SubStmtExpr, decl)
		{
		}

		public List<Statement> Statements { get; } = new List<Statement>();
		public void AddStatements(Statement value) { Statements.Add(value); }
	}

	[ComVisible(true)]
	public class Parameter : Declaration
	{
		public Parameter()
			: base(DeclarationKind.Parameter)
		{
		}

		public QualifiedType qualifiedType { get; set; }
		public bool isIndirect { get; set; }
		public bool hasDefaultValue { get; set; }
		public uint index { get; set; }
		public Expression defaultArgument { get; set; }
	}

	[ComVisible(true)]
	public enum CXXMethodKind
	{
		Normal,
		Constructor,
		Destructor,
		Conversion,
		Operator,
		UsingDirective
	}

	[ComVisible(true)]
	public enum CXXOperatorKind
	{
		None,
		New,
		Delete,
		Array_New,
		Array_Delete,
		Plus,
		Minus,
		Star,
		Slash,
		Percent,
		Caret,
		Amp,
		Pipe,
		Tilde,
		Exclaim,
		Equal,
		Less,
		Greater,
		PlusEqual,
		MinusEqual,
		StarEqual,
		SlashEqual,
		PercentEqual,
		CaretEqual,
		AmpEqual,
		PipeEqual,
		LessLess,
		GreaterGreater,
		LessLessEqual,
		GreaterGreaterEqual,
		EqualEqual,
		ExclaimEqual,
		LessEqual,
		GreaterEqual,
		AmpAmp,
		PipePipe,
		PlusPlus,
		MinusMinus,
		Comma,
		ArrowStar,
		Arrow,
		Call,
		Subscript,
		Conditional,
		Coawait
	}

	[ComVisible(true)]
	public enum FriendKind
	{
		None,
		Declared,
		Undeclared
	}

	[ComVisible(true)]
	public class Function : DeclarationContext
	{
		public Function()
			: base(DeclarationKind.Function)
		{
		}

		public QualifiedType returnType { get; set; }
		public bool isReturnIndirect { get; set; }
		public bool hasThisReturn { get; set; }

		public bool isConstExpr { get; set; }
		public bool isVariadic { get; set; }
		public bool isInline { get; set; }
		public bool isPure { get; set; }
		public bool isDeleted { get; set; }
		public bool isDefaulted { get; set; }
		public FriendKind friendKind { get; set; }
		public CXXOperatorKind operatorKind { get; set; }
		public string mangled { get; set; }
		public string signature { get; set; }
		public string body { get; set; }
		public CallingConvention callingConvention { get; set; }
		public List<Parameter> Parameters { get; } = new List<Parameter>();
		public void AddParameters(Parameter value) { Parameters.Add(value); }

		public FunctionTemplateSpecialization specializationInfo { get; set; }
		public Function instantiatedFrom { get; set; }
		public QualifiedType qualifiedType { get; set; }
	}

	[ComVisible(true)]
	public enum RefQualifierKind
	{
		None,
		LValue,
		RValue
	}

	[ComVisible(true)]
	public class Method : Function
	{
		public Method() { }

		public bool isVirtual { get; set; }
		public bool isStatic { get; set; }
		public bool isConst { get; set; }
		public bool isExplicit { get; set; }

		public CXXMethodKind methodKind { get; set; }

		public bool isDefaultConstructor { get; set; }
		public bool isCopyConstructor { get; set; }
		public bool isMoveConstructor { get; set; }

		public QualifiedType conversionType { get; set; }
		public RefQualifierKind refQualifier { get; set; }
		public List<Method> OverriddenMethods { get; } = new List<Method>();
		public void AddOverriddenMethods(Method value) { OverriddenMethods.Add(value); }
	}

	[ComVisible(true)]
	public class Enumeration : DeclarationContext
	{
		public Enumeration() : base(DeclarationKind.Enumeration) { }

		[ComVisible(true)]
		public class Item : Declaration
		{
			public Item() : base(DeclarationKind.EnumerationItem) { }
			public string expression;
			public ulong value;
		}

		[Flags]
		[ComVisible(true)]
		public enum EnumModifiers
		{
			Anonymous = 1 << 0,
			Scoped = 1 << 1,
			Flags = 1 << 2,
		}

		public EnumModifiers modifiers { get; set; }
		public Type type;
		public BuiltinType builtinType { get; set; }
		public List<Item> Items { get; } = new List<Item>();
		public void AddItems(Item value) { Items.Add(value); }
		public Item FindItemByName(string Name) { return null; }
	}

	[ComVisible(true)]
	public class Variable : Declaration
	{
		public Variable() : base(DeclarationKind.Variable) { }
		public string mangled { get; set; }
		public QualifiedType qualifiedType { get; set; }
		public Expression init { get; set; }
	}

	[ComVisible(true)]
	public class BaseClassSpecifier
	{
		public BaseClassSpecifier() { }
		public AccessSpecifier access { get; set; }
		public bool isVirtual { get; set; }
		public Type type { get; set; }
		public int offset { get; set; }
	}

	[ComVisible(true)]
	public class Field : Declaration
	{
		public Field() : base(DeclarationKind.Field) { }

		public QualifiedType qualifiedType { get; set; }
		public Class _class { get; set; }
		public bool isBitField { get; set; }
		public uint bitWidth { get; set; }
	}

	[ComVisible(true)]
	public class AccessSpecifierDecl : Declaration
	{
		public AccessSpecifierDecl() : base(DeclarationKind.AccessSpecifier) { }
	}

	[ComVisible(true)]
	public class Class : DeclarationContext
	{
		public Class() : base(DeclarationKind.Class) { }

		public List<BaseClassSpecifier> Bases { get; } = new List<BaseClassSpecifier>();
		public void AddBases(BaseClassSpecifier value) { Bases.Add(value); }

		public List<Field> Fields { get; } = new List<Field>();
		public void AddFields(Field value) { Fields.Add(value); }

		public List<Method> Methods { get; } = new List<Method>();
		public void AddMethods(Method value) { Methods.Add(value); }

		public List<AccessSpecifierDecl> Specifiers { get; } = new List<AccessSpecifierDecl>();
		public void AddSpecifiers(AccessSpecifierDecl value) { Specifiers.Add(value); }


		public bool isPOD { get; set; }
		public bool isAbstract { get; set; }
		public bool isUnion { get; set; }
		public bool isDynamic { get; set; }
		public bool isPolymorphic { get; set; }
		public bool hasNonTrivialDefaultConstructor { get; set; }
		public bool hasNonTrivialCopyConstructor { get; set; }
		public bool hasNonTrivialDestructor { get; set; }
		public bool isExternCContext { get; set; }
		public bool isInjected { get; set; }

		public ClassLayout layout { get; set; }
	}

	[ComVisible(true)]
	public class Template : Declaration
	{
		public Template(DeclarationKind kind) : base(DeclarationKind.Template)
		{
		}

		public Declaration TemplatedDecl { get; set; }
		public List<Declaration> Parameters { get; } = new List<Declaration>();
		public void AddParameters(Declaration value) { Parameters.Add(value); }
	}

	[ComVisible(true)]
	public class TypeAliasTemplate : Template
	{
		public TypeAliasTemplate() : base(DeclarationKind.TypeAliasTemplate) { }
	}

	[ComVisible(true)]
	public class TemplateParameter : Declaration
	{
		public TemplateParameter(DeclarationKind kind) : base(kind) { }
		public uint depth { get; set; }
		public uint index { get; set; }
		public bool isParameterPack { get; set; }
	}

	[ComVisible(true)]
	public class TemplateTemplateParameter : Template
	{
		public TemplateTemplateParameter() : base(DeclarationKind.TemplateTemplateParm) { }

		public bool isParameterPack { get; set; }
		public bool isPackExpansion { get; set; }
		public bool isExpandedParameterPack { get; set; }
	}

	[ComVisible(true)]
	public class TypeTemplateParameter : TemplateParameter
	{
		public TypeTemplateParameter() : base(DeclarationKind.TemplateTypeParm) { }

		public QualifiedType defaultArgument { get; set; }
	}

	[ComVisible(true)]
	public class NonTypeTemplateParameter : TemplateParameter
	{
		public NonTypeTemplateParameter() : base(DeclarationKind.NonTypeTemplateParm) { }

		public Expression defaultArgument { get; set; }
		public uint position { get; set; }
		public bool isPackExpansion { get; set; }
		public bool isExpandedParameterPack { get; set; }
	}

	[ComVisible(true)]
	public class ClassTemplate : Template
	{
		public ClassTemplate() : base(DeclarationKind.ClassTemplate) { }
		public List<ClassTemplateSpecialization> Specializations { get; } = new List<ClassTemplateSpecialization>();
		public void AddSpecializations(ClassTemplateSpecialization value) { Specializations.Add(value); }

		public ClassTemplateSpecialization FindSpecialization(string usr) { return null; }
		public ClassTemplatePartialSpecialization FindPartialSpecialization(string usr) { return null; }
	}

	[ComVisible(true)]
	public enum TemplateSpecializationKind
	{
		Undeclared,
		ImplicitInstantiation,
		ExplicitSpecialization,
		ExplicitInstantiationDeclaration,
		ExplicitInstantiationDefinition
	}

	[ComVisible(true)]
	public class ClassTemplateSpecialization : Class
	{
		public ClassTemplateSpecialization() { }
		public ClassTemplate templatedDecl { get; set; }
		public List<TemplateArgument> Arguments { get; } = new List<TemplateArgument>();
		public void AddArguments(TemplateArgument value) { Arguments.Add(value); }

		public TemplateSpecializationKind specializationKind { get; set; }
	}

	[ComVisible(true)]
	public class ClassTemplatePartialSpecialization : ClassTemplateSpecialization
	{
		public ClassTemplatePartialSpecialization() { }
	}

	[ComVisible(true)]
	public class FunctionTemplate : Template
	{
		public FunctionTemplate() : base(DeclarationKind.FunctionTemplate) { }
		public List<FunctionTemplateSpecialization> Specializations { get; } = new List<FunctionTemplateSpecialization>();
		public void AddSpecializations(FunctionTemplateSpecialization value) { Specializations.Add(value); }

		public FunctionTemplateSpecialization FindSpecialization(string usr) { return null; }
	}

	[ComVisible(true)]
	public class FunctionTemplateSpecialization
	{
		public FunctionTemplateSpecialization() { }
		public FunctionTemplate _template { get; set; }
		public List<TemplateArgument> Arguments { get; } = new List<TemplateArgument>();
		public void AddArguments(TemplateArgument value) { Arguments.Add(value); }

		public Function specializedFunction { get; set; }
		public TemplateSpecializationKind specializationKind { get; set; }
	}

	[ComVisible(true)]
	public class VarTemplate : Template
	{
		public VarTemplate() : base(DeclarationKind.VarTemplate) { }
		public List<VarTemplateSpecialization> Specializations { get; } = new List<VarTemplateSpecialization>();
		public void AddSpecializations(VarTemplateSpecialization value) { Specializations.Add(value); }

		public VarTemplateSpecialization FindSpecialization(string usr)
		{
			return null;
		}

		public VarTemplatePartialSpecialization FindPartialSpecialization(string usr)
		{
			return null;
		}
	}

	[ComVisible(true)]
	public class VarTemplateSpecialization : Variable
	{
		public VarTemplateSpecialization() { }
		public VarTemplate templatedDecl { get; set; }
		public List<TemplateArgument> Arguments { get; } = new List<TemplateArgument>();
		public void AddArguments(TemplateArgument value) { Arguments.Add(value); }

		public TemplateSpecializationKind specializationKind { get; set; }
	}

	[ComVisible(true)]
	public class VarTemplatePartialSpecialization : VarTemplateSpecialization
	{
		public VarTemplatePartialSpecialization() { }
	}

	[ComVisible(true)]
	public class Namespace : DeclarationContext
	{
		public Namespace() : base(DeclarationKind.Namespace) { }

		public bool isInline { get; set; }
	}

	[ComVisible(true)]
	public enum MacroLocation
	{
		Unknown,
		ClassHead,
		ClassBody,
		FunctionHead,
		FunctionParameters,
		FunctionBody,
	}

	[ComVisible(true)]
	public class PreprocessedEntity
	{
		public PreprocessedEntity() { }
		public MacroLocation macroLocation { get; set; }
		public IntPtr originalPtr { get; set; }
		public DeclarationKind kind { get; set; }
	}

	[ComVisible(true)]
	public class MacroDefinition : PreprocessedEntity
	{
		public MacroDefinition() { }
		public string name { get; set; }
		public string expression { get; set; }
		public int lineNumberStart { get; set; }
		public int lineNumberEnd { get; set; }
	}

	[ComVisible(true)]
	public class MacroExpansion : PreprocessedEntity
	{
		public MacroExpansion() { }
		public string name { get; set; }
		public string text { get; set; }
		public MacroDefinition definition { get; set; }
	}

	[ComVisible(true)]
	public class TranslationUnit : Namespace
	{
		public TranslationUnit() { }
		public string fileName { get; set; }
		public bool isSystemHeader { get; set; }
		public List<MacroDefinition> Macros { get; } = new List<MacroDefinition>();
		public void AddMacros(MacroDefinition value) { Macros.Add(value); }
	}

	[ComVisible(true)]
	public enum ArchType
	{
		UnknownArch,
		x86,
		x86_64
	}

	[ComVisible(true)]
	public class NativeLibrary
	{
		public NativeLibrary() { }

		public string fileName { get; set; }
		public ArchType archType { get; set; }
		public List<string> Symbols { get; } = new List<string>();
		public void AddSymbol(string value) { Symbols.Add(value); }
		public List<string> Dependencies { get; } = new List<string>();
		public void AddDependencie(string value) { Dependencies.Add(value); }
	}

	[ComVisible(true)]
	public class ASTContext
	{
		public ASTContext() { }
		public TranslationUnit FindOrCreateModule(string File) { return null; }
		public List<TranslationUnit> TranslationUnits { get; } = new List<TranslationUnit>();
		public void AddTranslationUnits(TranslationUnit value) { TranslationUnits.Add(value); }
	}

	#endregion

	#region Comments

	[ComVisible(true)]
	public enum CommentKind
	{
		FullComment,
		BlockContentComment,
		BlockCommandComment,
		ParamCommandComment,
		TParamCommandComment,
		VerbatimBlockComment,
		VerbatimLineComment,
		ParagraphComment,
		HTMLTagComment,
		HTMLStartTagComment,
		HTMLEndTagComment,
		TextComment,
		InlineContentComment,
		InlineCommandComment,
		VerbatimBlockLineComment
	}

	public abstract class Comment
	{
		public Comment(CommentKind kind) { }
		public CommentKind kind { get; set; }
	}

	[ComVisible(true)]
	public class BlockContentComment : Comment
	{
		public BlockContentComment() : base(CommentKind.BlockContentComment) { }
		public BlockContentComment(CommentKind Kind) : base(Kind) { }
	}

	[ComVisible(true)]
	public class FullComment : Comment
	{
		public FullComment() : base(CommentKind.FullComment) { }

		public List<BlockContentComment> Blocks { get; } = new List<BlockContentComment>();
		public void AddBlocks(BlockContentComment value) { Blocks.Add(value); }
	}

	[ComVisible(true)]
	public class InlineContentComment : Comment
	{
		public InlineContentComment() : base(CommentKind.InlineContentComment) { }
		public InlineContentComment(CommentKind Kind) : base(Kind) { }
		public bool hasTrailingNewline { get; set; }
	}

	[ComVisible(true)]
	public class ParagraphComment : BlockContentComment
	{
		public ParagraphComment() : base(CommentKind.ParagraphComment) { }

		public bool isWhitespace { get; set; }
		public List<InlineContentComment> Content { get; } = new List<InlineContentComment>();
		public void AddContent(InlineContentComment value) { Content.Add(value); }
	}

	[ComVisible(true)]
	public class BlockCommandComment : BlockContentComment
	{
		[ComVisible(true)]
		public class Argument
		{
			public Argument() { }

			public string text { get; set; }
		}
		public BlockCommandComment() : base(CommentKind.BlockCommandComment) { }
		public BlockCommandComment(CommentKind Kind) : base(Kind) { }

		public uint commandId { get; set; }
		public ParagraphComment paragraphComment { get; set; }
		public List<Argument> Arguments { get; } = new List<Argument>();
		public void AddArguments(Argument value) { Arguments.Add(value); }
	}

	[ComVisible(true)]
	public class ParamCommandComment : BlockCommandComment
	{
		[ComVisible(true)]
		public enum PassDirection
		{
			In,
			Out,
			InOut
		}
		public ParamCommandComment() : base(CommentKind.ParamCommandComment) { }
		public PassDirection direction { get; set; }
		public uint paramIndex { get; set; }
	}

	[ComVisible(true)]
	public class TParamCommandComment : BlockCommandComment
	{
		public TParamCommandComment() : base(CommentKind.TParamCommandComment) { }
		public List<uint> Position { get; } = new List<uint>();
		public void AddPosition(uint value) { Position.Add(value); }

	}

	[ComVisible(true)]
	public class VerbatimBlockLineComment : Comment
	{
		public VerbatimBlockLineComment() : base(CommentKind.VerbatimBlockComment) { }
		public string text { get; set; }
	}

	[ComVisible(true)]
	public class VerbatimBlockComment : BlockCommandComment
	{
		public VerbatimBlockComment() : base(CommentKind.VerbatimBlockLineComment) { }

		public List<VerbatimBlockLineComment> Lines { get; } = new List<VerbatimBlockLineComment>();
		public void AddLines(VerbatimBlockLineComment value) { Lines.Add(value); }
	}

	[ComVisible(true)]
	public class VerbatimLineComment : BlockCommandComment
	{
		public VerbatimLineComment() : base(CommentKind.VerbatimLineComment) { }
		public string text { get; set; }
	}

	[ComVisible(true)]
	public class InlineCommandComment : InlineContentComment
	{
		[ComVisible(true)]
		public enum RenderKind
		{
			RenderNormal,
			RenderBold,
			RenderMonospaced,
			RenderEmphasized
		}
		[ComVisible(true)]
		public class Argument
		{
			public Argument() { }
			public string text { get; set; }
		}
		public InlineCommandComment() : base(CommentKind.InlineCommandComment) { }
		public uint commandId { get; set; }
		public RenderKind commentRenderKind { get; set; }
		public List<Argument> Arguments { get; } = new List<Argument>();
		public void AddArguments(Argument value) { Arguments.Add(value); }
	}

	[ComVisible(true)]
	public class HTMLTagComment : InlineContentComment
	{
		public HTMLTagComment() : base(CommentKind.HTMLTagComment) { }
		public HTMLTagComment(CommentKind Kind) : base(Kind) { }
	}

	[ComVisible(true)]
	public class HTMLStartTagComment : HTMLTagComment
	{
		[ComVisible(true)]
		public class Attribute
		{
			public Attribute() { }

			public string name { get; set; }
			public string value { get; set; }
		}
		public HTMLStartTagComment() : base(CommentKind.HTMLStartTagComment) { }
		public string tagName { get; set; }
		public List<Attribute> Attributes { get; } = new List<Attribute>();
		public void AddAttributes(Attribute value) { Attributes.Add(value); }
	}

	[ComVisible(true)]
	public class HTMLEndTagComment : HTMLTagComment
	{
		public HTMLEndTagComment() : base(CommentKind.HTMLEndTagComment) { }
		public string tagName { get; set; }
	}

	[ComVisible(true)]
	public class TextComment : InlineContentComment
	{
		public TextComment() : base(CommentKind.TextComment) { }
		public string text { get; set; }
	}

	[ComVisible(true)]
	public enum RawCommentKind
	{
		Invalid,
		OrdinaryBCPL,
		OrdinaryC,
		BCPLSlash,
		BCPLExcl,
		JavaDoc,
		Qt,
		Merged
	}

	[ComVisible(true)]
	public class RawComment
	{
		public RawComment() { }

		public RawCommentKind kind { get; set; }
		public string text { get; set; }
		public string briefText { get; set; }
		public FullComment fullCommentBlock { get; set; }
	}

	#region Commands

	#endregion

	#endregion
}
