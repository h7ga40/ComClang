using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CppSharp.AST
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
	public interface IType
	{
		TypeKind Kind { get; set; }
		bool IsDependent { get; set; }
	}

	[ComVisible(true)]
	public interface ITypeQualifiers
	{
		bool IsConst { get; set; }
		bool IsVolatile { get; set; }
		bool IsRestrict { get; set; }
	}

	[ComVisible(true)]
	public interface IQualifiedType
	{
		IType Type { get; set; }
		ITypeQualifiers Qualifiers { get; set; }
	}

	[ComVisible(true)]
	public interface ITagType : IType
	{
		IDeclaration Declaration { get; set; }
	}

	[ComVisible(true)]
	public enum ArraySize
	{
		Constant,
		Variable,
		Dependent,
		Incomplete
	}

	[ComVisible(true)]
	public interface IArrayType : IType
	{
		IQualifiedType QualifiedType { get; set; }
		ArraySize SizeType { get; set; }
		long Size { get; set; }
		long ElementSize { get; set; }
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
	public interface IFunctionType : IType
	{
		IQualifiedType ReturnType { get; set; }
		CallingConvention CallingConvention { get; set; }
		ExceptionSpecType ExceptionSpecType { get; set; }
		int ParameterCount { get; }
		IParameter GetParameter(int index);
		void AddParameter(IParameter parameter);
	}

	[ComVisible(true)]
	/// <summary>
	/// Represents the modifiers on a C++ type reference.
	/// </summary>
	public enum TypeModifier
	{
		Value,
		Pointer,
		// L-value references
		LVReference,
		// R-value references
		RVReference
	}

	[ComVisible(true)]
	public interface IPointerType : IType
	{
		IQualifiedType QualifiedPointee { get; set; }
		TypeModifier Modifier { get; set; }
	}

	[ComVisible(true)]
	public interface IMemberPointerType : IType
	{
		IQualifiedType Pointee { get; set; }
	}

	[ComVisible(true)]
	public interface ITypedefType : IType
	{
		ITypedefNameDecl Declaration { get; set; }
	}

	[ComVisible(true)]
	public interface IAttributedType : IType
	{
		IQualifiedType Modified { get; set; }
		IQualifiedType Equivalent { get; set; }
	}

	[ComVisible(true)]
	public interface IDecayedType : IType
	{
		IQualifiedType Decayed { get; set; }
		IQualifiedType Original { get; set; }
		IQualifiedType Pointee { get; set; }
	}

	[ComVisible(true)]
	/// The kind of template argument we're storing.
	public enum ArgumentKind
	{
		/// The template argument is a type.
		Type,

		/// The template argument is a declaration that was provided for a
		/// pointer. reference, or pointer to member non-type template
		/// parameter.
		Declaration,

		/// The template argument is a null pointer or null pointer to member
		/// that was provided for a non-type template parameter.
		NullPtr,

		/// The template argument is an integral value that was provided for
		/// an integral non-type template parameter.
		Integral,

		/// The template argument is a template name that was provided for a
		/// template template parameter.
		Template,

		/// The template argument is a pack expansion of a template name that
		/// was provided for a template template parameter.
		TemplateExpansion,

		/// The template argument is a value- or type-dependent expression.
		Expression,

		/// The template argument is actually a parameter pack.
		Pack
	}

	[ComVisible(true)]
	public interface ITemplateArgument
	{
		ArgumentKind Kind { get; set; }
		IQualifiedType Type { get; set; }
		IDeclaration Declaration { get; set; }
		long Integral { get; set; }
	}

	[ComVisible(true)]
	public interface ITemplateSpecializationType : IType
	{
		int ArgumentsCount { get; }
		ITemplateArgument GetArguments(int index);
		void AddArguments(ITemplateArgument value);

		ITemplate Template { get; set; }
		IQualifiedType Desugared { get; set; }
	}

	[ComVisible(true)]
	public interface IDependentTemplateSpecializationType : IType
	{
		int ArgumentsCount { get; }
		ITemplateArgument GetArguments(int index);
		void AddArguments(ITemplateArgument value);
		IQualifiedType Desugared { get; set; }
	}

	[ComVisible(true)]
	public interface ITemplateParameterType : IType
	{
		ITypeTemplateParameter Parameter { get; set; }
		uint Depth { get; set; }
		uint Index { get; set; }
		bool IsParameterPack { get; set; }
	}

	[ComVisible(true)]
	public interface ITemplateParameterSubstitutionType : IType
	{
		IQualifiedType Replacement { get; set; }
		ITemplateParameterType ReplacedParameter { get; set; }
	}

	[ComVisible(true)]
	public interface IInjectedClassNameType : IType
	{
		IQualifiedType InjectedSpecializationType { get; set; }
		IClass Class { get; set; }
	}

	[ComVisible(true)]
	public interface IDependentNameType : IType
	{
		IQualifiedType Qualifier { get; set; }
		string Identifier { get; set; }
	}

	[ComVisible(true)]
	public interface IPackExpansionType : IType
	{
	}

	[ComVisible(true)]
	public interface IUnaryTransformType : IType
	{
		IQualifiedType Desugared { get; set; }
		IQualifiedType BaseType { get; set; }
	}

	[ComVisible(true)]
	public interface IVectorType : IType
	{
		IQualifiedType ElementType { get; set; }
		uint NumElements { get; set; }
	}

	/// <summary>
	/// Represents the C++ built-in types.
	/// </summary>
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
		IntPtr,
		UIntPtr,
		String,
		Decimal
	}

	[ComVisible(true)]
	public interface IBuiltinType : IType
	{
		PrimitiveType Type { get; set; }
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

	/// <summary>
	/// Virtual table component kind.
	/// </summary>
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
	public interface IVTableComponent
	{
		VTableComponentKind Kind { get; set; }
		ulong Offset { get; set; }
		IDeclaration Declaration { get; set; }
	}

	[ComVisible(true)]
	public interface IVTableLayout
	{
		int ComponentCount { get; }
		IVTableComponent GetComponent(int index);
		void AddComponent(IVTableComponent value);
	}

	[ComVisible(true)]
	public interface IVFTableInfo
	{
		ulong VBTableIndex { get; set; }
		long VFPtrOffset { get; set; }
		long VFPtrFullOffset { get; set; }
		IVTableLayout Layout { get; set; }
	}

	[ComVisible(true)]
	public interface ILayoutField
	{
		uint Offset { get; set; }
		string Name { get; set; }
		IQualifiedType QualifiedType { get; set; }
		IntPtr FieldPtr { get; set; }
	}

	[ComVisible(true)]
	public interface ILayoutBase
	{
		uint Offset { get; set; }
		IClass Class { get; set; }
	}

	[ComVisible(true)]
	public interface IClassLayout
	{
		CppAbi ABI { get; set; }
		int VFTableCount { get; }
		IVFTableInfo GetVFTable(int index);
		void AddVFTable(IVFTableInfo value);

		IVTableLayout Layout { get; set; }
		bool HasOwnVFPtr { get; set; }
		long VBPtrOffset { get; set; }
		int Alignment { get; set; }
		int Size { get; set; }
		int DataSize { get; set; }
		int FieldCount { get; }
		ILayoutField GetField(int index);
		void AddField(ILayoutField value);
		int BaseCount { get; }
		ILayoutBase GetBase(int index);
		void AddBase(ILayoutBase value);
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

	// A C++ access specifier.
	[ComVisible(true)]
	public enum AccessSpecifier
	{
		Private,
		Protected,
		Public,
		Internal
	}


	[ComVisible(true)]
	public interface IDeclaration
	{
		DeclarationKind DeclKind { get; set; }
		int MaxFieldAlignment { get; set; }
		AccessSpecifier Access { get; set; }
		IDeclarationContext Namespace { get; set; }
		ComClang.ISourceLocation Location { get; set; }
		int LineNumberStart { get; set; }
		int LineNumberEnd { get; set; }
		string Name { get; set; }
		string USR { get; set; }
		string DebugText { get; set; }
		bool IsIncomplete { get; set; }
		bool IsDependent { get; set; }
		bool IsImplicit { get; set; }
		bool IsInvalid { get; set; }
		IDeclaration CompleteDeclaration { get; set; }
		uint DefinitionOrder { get; set; }
		int PreprocessedEntitieCount { get; }
		IPreprocessedEntity GetPreprocessedEntitie(int index);
		void AddPreprocessedEntitie(IPreprocessedEntity value);
		int RedeclarationCount { get; }
		IDeclaration GetRedeclaration(int index);
		void AddRedeclaration(IDeclaration value);
		IntPtr OriginalPtr { get; set; }
		IRawComment Comment { get; set; }
	}

	[ComVisible(true)]
	public interface IDeclarationContext : IDeclaration
	{
		IDeclaration FindAnonymous(string USR);

		INamespace FindNamespace(string Name);
		INamespace FindNamespace(string[] names);
		INamespace FindCreateNamespace(string Name);

		IClass CreateClass(string Name, bool IsComplete);
		IClass FindClass(IntPtr OriginalPtr, string Name, bool IsComplete);
		IClass FindClass(IntPtr OriginalPtr, string Name, bool IsComplete, bool Create);

		ITemplate FindTemplate(string USR);

		IEnumeration FindEnum(IntPtr OriginalPtr);
		IEnumeration FindEnum(string Name, bool Create = false);
		IEnumeration FindEnumWithItem(string Name);

		IFunction FindFunction(string USR);

		ITypedefDecl FindTypedef(string Name, bool Create = false);

		ITypeAlias FindTypeAlias(string Name, bool Create = false);

		IVariable FindVariable(string USR);

		IFriend FindFriend(string USR);

		int NamespaceCount { get; }
		INamespace GetNamespace(int index);
		void AddNamespace(INamespace value);

		int EnumCount { get; }
		IEnumeration GetEnum(int index);
		void AddEnum(IEnumeration value);

		int FunctionCount { get; }
		IFunction GetFunction(int index);
		void AddFunction(IFunction value);

		int ClasseCount { get; }
		IClass GetClasse(int index);
		void AddClasse(IClass value);

		int TemplateCount { get; }
		ITemplate GetTemplate(int index);
		void AddTemplate(ITemplate value);

		int TypedefCount { get; }
		ITypedefDecl GetTypedef(int index);
		void AddTypedef(ITypedefDecl value);

		int TypeAliaseCount { get; }
		ITypeAlias GetTypeAliase(int index);
		void AddTypeAliase(ITypeAlias value);

		int VariableCount { get; }
		IVariable GetVariable(int index);
		void AddVariable(IVariable value);

		int FriendCount { get; }
		IFriend GetFriend(int index);
		void AddFriend(IFriend value);

		int AnonymousCount { get; }
		IDeclaration GetAnonymous(string USR);
		void AddAnonymous(string USR, IDeclaration declaration);
		bool IsAnonymous { get; set; }
	}

	[ComVisible(true)]
	public interface ITypedefNameDecl : IDeclaration
	{
		IQualifiedType QualifiedType { get; set; }
	}

	[ComVisible(true)]
	public interface ITypedefDecl : ITypedefNameDecl
	{
	}

	[ComVisible(true)]
	public interface ITypeAlias : ITypedefNameDecl
	{
		ITypeAliasTemplate DescribedAliasTemplate { get; set; }
	}

	[ComVisible(true)]
	public interface IFriend : IDeclaration
	{
		IDeclaration Declaration { get; set; }
	}

	[ComVisible(true)]
	public enum StatementClass
	{
		Any,
		BinaryOperator,
		DeclarationReference,
		Call,
		ConstructorReference,
		CXXOperatorCall,
		ImplicitCast,
		ExplicitCast,
		InitList,
		SubStmt,
	}

	[ComVisible(true)]
	public interface IStatement
	{
		StatementClass Class { get; set; }
		IDeclaration Declaration { get; set; }
		string String { get; set; }
	}

	[ComVisible(true)]
	public interface IExpression : IStatement
	{
	}

	[ComVisible(true)]
	public interface IBinaryOperator : IExpression
	{
		IExpression LHS { get; set; }
		IExpression RHS { get; set; }
		string OpcodeStr { get; set; }
	}

	[ComVisible(true)]
	public interface ICallExpr : IExpression
	{
		int ArgumentsCount { get; }
		IExpression GetArguments(int index);
		void AddArguments(IExpression value);
	}

	[ComVisible(true)]
	public interface ICXXConstructExpr : IExpression
	{
		int ArgumentsCount { get; }
		IExpression GetArguments(int index);
		void AddArguments(IExpression value);
	}

	[ComVisible(true)]
	public interface IInitListExpr : IExpression
	{
		int InitCount { get; }
		IExpression GetInit(int index);
		void AddInit(IExpression value);

	}

	[ComVisible(true)]
	public interface ISubStmtExpr : IExpression
	{
		int StatementCount { get; }
		IStatement GetStatement(int index);
		void AddStatement(IStatement value);
	}

	[ComVisible(true)]
	public interface IParameter : IDeclaration
	{
		IQualifiedType QualifiedType { get; set; }
		bool IsIndirect { get; set; }
		bool HasDefaultValue { get; set; }
		uint Index { get; set; }
		IExpression DefaultArgument { get; set; }
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
		Coawait,
		Conversion,
		ExplicitConversion
	}

	[ComVisible(true)]
	public enum FriendKind
	{
		None,
		Declared,
		Undeclared
	}

	[ComVisible(true)]
	public interface IFunction : IDeclarationContext
	{
		IQualifiedType ReturnType { get; set; }
		bool IsReturnIndirect { get; set; }
		bool HasThisReturn { get; set; }

		bool IsConstExpr { get; set; }
		bool IsVariadic { get; set; }
		bool IsInline { get; set; }
		bool IsPure { get; set; }
		bool IsDeleted { get; set; }
		bool IsDefaulted { get; set; }
		FriendKind FriendKind { get; set; }
		CXXOperatorKind OperatorKind { get; set; }
		string Mangled { get; set; }
		string Signature { get; set; }
		string Body { get; set; }
		CallingConvention CallingConvention { get; set; }
		int ParameterCount { get; }
		IParameter GetParameter(int index);
		void AddParameter(IParameter value);
		IFunctionTemplateSpecialization SpecializationInfo { get; set; }
		IFunction InstantiatedFrom { get; set; }
		IQualifiedType QualifiedType { get; set; }
	}

	[ComVisible(true)]
	public enum RefQualifier
	{
		None,
		LValue,
		RValue
	}

	[ComVisible(true)]
	public interface IMethod : IFunction
	{
		bool IsVirtual { get; set; }
		bool IsStatic { get; set; }
		bool IsConst { get; set; }
		bool IsExplicit { get; set; }

		CXXMethodKind MethodKind { get; set; }

		bool IsDefaultConstructor { get; set; }
		bool IsCopyConstructor { get; set; }
		bool IsMoveConstructor { get; set; }

		IQualifiedType ConversionType { get; set; }
		RefQualifier RefQualifier { get; set; }
		int OverriddenMethodCount { get; }
		IMethod GetOverriddenMethod(int index);
		void AddOverriddenMethod(IMethod value);
	}

	[ComVisible(true)]
	public interface IEnumeration_Item : IDeclaration
	{
		string Expression { get; set; }
		ulong Value { get; set; }
	}

	[Flags]
	[ComVisible(true)]
	public enum EnumModifiers
	{
		Anonymous = 1 << 0,
		Scoped = 1 << 1,
		Flags = 1 << 2,
	}

	[ComVisible(true)]
	public interface IEnumeration : IDeclarationContext
	{
		EnumModifiers Modifiers { get; set; }
		IType Type { get; set; }
		IBuiltinType BuiltinType { get; set; }
		int ItemCount { get; }
		IEnumeration_Item GetItem(int index);
		void AddItem(IEnumeration_Item value);
		IEnumeration_Item FindItemByName(string Name);
	}

	[ComVisible(true)]
	public interface IVariable : IDeclaration
	{
		string Mangled { get; set; }
		IQualifiedType QualifiedType { get; set; }
		IExpression Init { get; set; }
	}

	[ComVisible(true)]
	public interface IBaseClassSpecifier
	{
		AccessSpecifier Access { get; set; }
		bool IsVirtual { get; set; }
		IType Type { get; set; }
		int Offset { get; set; }
	}

	[ComVisible(true)]
	public interface IField : IDeclaration
	{
		IQualifiedType QualifiedType { get; set; }
		IClass Class { get; set; }
		bool IsBitField { get; set; }
		uint BitWidth { get; set; }
	}

	[ComVisible(true)]
	public interface IAccessSpecifierDecl : IDeclaration
	{
	}

	[ComVisible(true)]
	public interface IClass : IDeclarationContext
	{
		int BaseCount { get; }
		IBaseClassSpecifier GetBase(int index);
		void AddBase(IBaseClassSpecifier value);

		int FieldCount { get; }
		IField GetField(int index);
		void AddField(IField value);

		int MethodCount { get; }
		IMethod GetMethod(int index);
		void AddMethod(IMethod value);

		int SpecifierCount { get; }
		IAccessSpecifierDecl GetSpecifier(int index);
		void AddSpecifier(IAccessSpecifierDecl value);


		bool IsPOD { get; set; }
		bool IsAbstract { get; set; }
		bool IsUnion { get; set; }
		bool IsDynamic { get; set; }
		bool IsPolymorphic { get; set; }
		bool HasNonTrivialDefaultConstructor { get; set; }
		bool HasNonTrivialCopyConstructor { get; set; }
		bool HasNonTrivialDestructor { get; set; }
		bool IsExternCContext { get; set; }
		bool IsInjected { get; set; }

		IClassLayout Layout { get; set; }
	}

	[ComVisible(true)]
	public interface ITemplate : IDeclaration
	{
		IDeclaration TemplatedDecl { get; set; }
		int ParameterCount { get; }
		IDeclaration GetParameter(int index);
		void AddParameter(IDeclaration value);
	}

	[ComVisible(true)]
	public interface ITypeAliasTemplate : ITemplate
	{
	}

	[ComVisible(true)]
	public interface ITemplateParameter : IDeclaration
	{
		uint Depth { get; set; }
		uint Index { get; set; }
		bool IsParameterPack { get; set; }
	}

	[ComVisible(true)]
	public interface ITemplateTemplateParameter : ITemplate
	{
		bool IsParameterPack { get; set; }
		bool IsPackExpansion { get; set; }
		bool IsExpandedParameterPack { get; set; }
	}

	[ComVisible(true)]
	public interface ITypeTemplateParameter : ITemplateParameter
	{
		IQualifiedType DefaultArgument { get; set; }
	}

	[ComVisible(true)]
	public interface INonTypeTemplateParameter : ITemplateParameter
	{
		IExpression DefaultArgument { get; set; }
		uint Position { get; set; }
		bool IsPackExpansion { get; set; }
		bool IsExpandedParameterPack { get; set; }
	}

	[ComVisible(true)]
	public interface IClassTemplate : ITemplate
	{
		int SpecializationCount { get; }
		IClassTemplateSpecialization GetSpecialization(int index);
		void AddSpecialization(IClassTemplateSpecialization value);

		IClassTemplateSpecialization FindSpecialization(string usr);
		IClassTemplatePartialSpecialization FindPartialSpecialization(string usr);
	}

	/// <summary>
	/// Describes the kind of template specialization that a particular
	/// template specialization declaration represents.
	/// </summary>
	[ComVisible(true)]
	public enum TemplateSpecializationKind
	{
		/// This template specialization was formed from a template-id but has
		/// not yet been declared, defined, or instantiated.
		Undeclared,

		/// This template specialization was implicitly instantiated from a
		/// template.
		ImplicitInstantiation,

		/// This template specialization was declared or defined by an explicit
		/// specialization or partial specialization.
		ExplicitSpecialization,

		/// This template specialization was instantiated from a template due
		/// to an explicit instantiation declaration request.
		ExplicitInstantiationDeclaration,

		/// This template specialization was instantiated from a template due
		/// to an explicit instantiation definition request.
		ExplicitInstantiationDefinition
	}

	[ComVisible(true)]
	public interface IClassTemplateSpecialization : IClass
	{
		IClassTemplate TemplatedDecl { get; set; }
		int ArgumentsCount { get; }
		ITemplateArgument GetArguments(int index);
		void AddArguments(ITemplateArgument value);

		TemplateSpecializationKind SpecializationKind { get; set; }
	}

	[ComVisible(true)]
	public interface IClassTemplatePartialSpecialization : IClassTemplateSpecialization
	{
	}

	[ComVisible(true)]
	public interface IFunctionTemplate : ITemplate
	{
		int SpecializationCount { get; }
		IFunctionTemplateSpecialization GetSpecialization(int index);
		void AddSpecialization(IFunctionTemplateSpecialization value);

		IFunctionTemplateSpecialization FindSpecialization(string usr);
	}

	[ComVisible(true)]
	public interface IFunctionTemplateSpecialization
	{
		IFunctionTemplate Template { get; set; }
		int ArgumentsCount { get; }
		ITemplateArgument GetArguments(int index);
		void AddArguments(ITemplateArgument value);

		IFunction SpecializedFunction { get; set; }
		TemplateSpecializationKind SpecializationKind { get; set; }
	}

	[ComVisible(true)]
	public interface IVarTemplate : ITemplate
	{
		int SpecializationCount { get; }
		IVarTemplateSpecialization GetSpecialization(int index);
		void AddSpecialization(IVarTemplateSpecialization value);
		IVarTemplateSpecialization FindSpecialization(string usr);
		IVarTemplatePartialSpecialization FindPartialSpecialization(string usr);
	}

	[ComVisible(true)]
	public interface IVarTemplateSpecialization : IVariable
	{
		IVarTemplate TemplatedDecl { get; set; }
		int ArgumentsCount { get; }
		ITemplateArgument GetArguments(int index);
		void AddArguments(ITemplateArgument value);
		TemplateSpecializationKind SpecializationKind { get; set; }
	}

	[ComVisible(true)]
	public interface IVarTemplatePartialSpecialization : IVarTemplateSpecialization
	{
	}

	[ComVisible(true)]
	public interface INamespace : IDeclarationContext
	{
		bool IsInline { get; set; }
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
	public interface IPreprocessedEntity
	{
		MacroLocation MacroLocation { get; set; }
		IntPtr OriginalPtr { get; set; }
		DeclarationKind Kind { get; set; }
	}

	[ComVisible(true)]
	public interface IMacroDefinition : IPreprocessedEntity
	{
		string Name { get; set; }
		string Expression { get; set; }
		int LineNumberStart { get; set; }
		int LineNumberEnd { get; set; }
	}

	[ComVisible(true)]
	public interface IMacroExpansion : IPreprocessedEntity
	{
		string Name { get; set; }
		string Text { get; set; }
		IMacroDefinition Definition { get; set; }
	}

	[ComVisible(true)]
	public interface ITranslationUnit : INamespace
	{
		string FileName { get; set; }
		bool IsSystemHeader { get; set; }
		int MacroCount { get; }
		IMacroDefinition GetMacro(int index);
		void AddMacro(IMacroDefinition value);
	}

	[ComVisible(true)]
	public enum ArchType
	{
		UnknownArch,
		// X86: i[3-9]86
		x86,
		// X86-64: amd64, x86_64
		x86_64 
	}

	[ComVisible(true)]
	public interface INativeLibrary
	{
		string FileName { get; set; }
		ArchType ArchType { get; set; }
		int SymbolsCount { get; }
		string GetSymbols(int index);
		void AddSymbols(string value);
		int DependenciesCount { get; }
		string GetDependencies(int index);
		void AddDependencies(string value);
	}

	[ComVisible(true)]
	public interface IASTContext
	{
		ITranslationUnit FindOrCreateModule(string File);
		int TranslationUnitCount { get; }
		ITranslationUnit GetTranslationUnit(int index);
		void AddTranslationUnit(ITranslationUnit value);
	}

	#endregion

	#region Comments

	[ComVisible(true)]
	public enum DocumentationCommentKind
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

	[ComVisible(true)]
	public interface IComment
	{
		DocumentationCommentKind Kind { get; set; }
	}

	[ComVisible(true)]
	public interface IBlockContentComment : IComment
	{
	}

	[ComVisible(true)]
	public interface IFullComment : IComment
	{
		int BlockCount { get; }
		IBlockContentComment GetBlock(int index);
		void AddBlock(IBlockContentComment value);
	}

	[ComVisible(true)]
	public interface IInlineContentComment : IComment
	{
		bool HasTrailingNewline { get; set; }
	}

	[ComVisible(true)]
	public interface IParagraphComment : IBlockContentComment
	{
		bool IsWhitespace { get; set; }
		int ContentCount { get; }
		IInlineContentComment GetContent(int index);
		void AddContent(IInlineContentComment value);
	}

	[ComVisible(true)]
	public interface IBlockCommandComment_Argument
	{
		string Text { get; set; }
	}

	[ComVisible(true)]
	public interface IBlockCommandComment : IBlockContentComment
	{
		uint CommandId { get; set; }
		IParagraphComment ParagraphComment { get; set; }
		int ArgumentsCount { get; }
		IBlockCommandComment_Argument GetArguments(int index);
		void AddArguments(IBlockCommandComment_Argument value);
	}

	[ComVisible(true)]
	public enum ParamCommandComment_PassDirection
	{
		In,
		Out,
		InOut
	}

	[ComVisible(true)]
	public interface IParamCommandComment : IBlockCommandComment
	{
		ParamCommandComment_PassDirection Direction { get; set; }
		uint ParamIndex { get; set; }
	}

	[ComVisible(true)]
	public interface ITParamCommandComment : IBlockCommandComment
	{
		int PositionCount { get; }
		uint GetPosition(int index);
		void AddPosition(uint value);
	}

	[ComVisible(true)]
	public interface IVerbatimBlockLineComment : IComment
	{
		string Text { get; set; }
	}

	[ComVisible(true)]
	public interface IVerbatimBlockComment : IBlockCommandComment
	{
		int LineCount { get; }
		IVerbatimBlockLineComment GetLine(int index);
		void AddLine(IVerbatimBlockLineComment value);
	}

	[ComVisible(true)]
	public interface IVerbatimLineComment : IBlockCommandComment
	{
		string Text { get; set; }
	}

	[ComVisible(true)]
	public enum InlineCommandComment_RenderKind
	{
		RenderNormal,
		RenderBold,
		RenderMonospaced,
		RenderEmphasized
	}

	[ComVisible(true)]
	public interface IInlineCommandComment_Argument
	{
		string Text { get; set; }
	}

	[ComVisible(true)]
	public interface IInlineCommandComment : IInlineContentComment
	{
		uint CommandId { get; set; }
		InlineCommandComment_RenderKind CommentRenderKind { get; set; }
		int ArgumentsCount { get; }
		IInlineCommandComment_Argument GetArguments(int index);
		void AddArguments(IInlineCommandComment_Argument value);
	}

	[ComVisible(true)]
	public interface IHTMLTagComment : IInlineContentComment
	{
	}

	[ComVisible(true)]
	public interface IHTMLStartTagComment_Attribute
	{
		string Name { get; set; }
		string Value { get; set; }
	}

	[ComVisible(true)]
	public interface IHTMLStartTagComment : IHTMLTagComment
	{
		string TagName { get; set; }
		int AttributeCount { get; }
		IHTMLStartTagComment_Attribute GetAttribute(int index);
		void AddAttribute(IHTMLStartTagComment_Attribute value);
	}

	[ComVisible(true)]
	public interface IHTMLEndTagComment : IHTMLTagComment
	{
		string TagName { get; set; }
	}

	[ComVisible(true)]
	public interface ITextComment : IInlineContentComment
	{
		string Text { get; set; }
	}

	[ComVisible(true)]
	public enum CommentKind
	{
		Invalid,
		BCPL,
		C,
		BCPLSlash,
		BCPLExcl,
		JavaDoc,
		Qt,
		Merged
	}

	[ComVisible(true)]
	public interface IRawComment
	{
		CommentKind Kind { get; set; }
		string Text { get; set; }
		string BriefText { get; set; }
		IFullComment FullCommentBlock { get; set; }
	}

	#region Commands

	#endregion

	#endregion
}
