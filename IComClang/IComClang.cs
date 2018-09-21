using CppSharp.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	[ComVisible(true)]
	public interface IDecl
	{
		DeclKind Kind { get; }
		IntPtr Native { get; set; }
		void SetRange(ISourceLocation begin, ISourceLocation end);
		void SetName(string name);
		void SetStmt(string name, IStmt stmt);
	}

	[ComVisible(true)]
	public interface IStmt
	{
		StmtClass Class { get; }
		IntPtr Native { get; set; }
		void SetRange(ISourceLocation begin, ISourceLocation end);
		void SetName(string name);
	}

	[ComVisible(true)]
	public interface ISourceLocation
	{
		string FileName { get; set; }
		int LineNo { get; set; }
		int ColumnNo { get; set; }
	}

	[ComVisible(true)]
	public interface ISourceRange
	{
		ISourceLocation Begin { get; set; }
		ISourceLocation End { get; set; }
	}

	[ComVisible(true)]
	public enum DeclKind
	{
		AccessSpec,
		Block,
		Captured,
		ClassScopeFunctionSpecialization,
		Empty,
		Export,
		ExternCContext,
		FileScopeAsm,
		Friend,
		FriendTemplate,
		Import,
		LinkageSpec,
		Label,
		Namespace,
		NamespaceAlias,
		ObjCCompatibleAlias,
		ObjCCategory,
		ObjCCategoryImpl,
		ObjCImplementation,
		firstObjCImpl = ObjCCategoryImpl, lastObjCImpl = ObjCImplementation,
		ObjCInterface,
		ObjCProtocol,
		firstObjCContainer = ObjCCategory, lastObjCContainer = ObjCProtocol,
		ObjCMethod,
		ObjCProperty,
		BuiltinTemplate,
		ClassTemplate,
		FunctionTemplate,
		TypeAliasTemplate,
		VarTemplate,
		firstRedeclarableTemplate = ClassTemplate, lastRedeclarableTemplate = VarTemplate,
		TemplateTemplateParm,
		firstTemplate = BuiltinTemplate, lastTemplate = TemplateTemplateParm,
		Enum,
		Record,
		CXXRecord,
		ClassTemplateSpecialization,
		ClassTemplatePartialSpecialization,
		firstClassTemplateSpecialization = ClassTemplateSpecialization, lastClassTemplateSpecialization = ClassTemplatePartialSpecialization,
		firstCXXRecord = CXXRecord, lastCXXRecord = ClassTemplatePartialSpecialization,
		firstRecord = Record, lastRecord = ClassTemplatePartialSpecialization,
		firstTag = Enum, lastTag = ClassTemplatePartialSpecialization,
		TemplateTypeParm,
		ObjCTypeParam,
		TypeAlias,
		Typedef,
		firstTypedefName = ObjCTypeParam, lastTypedefName = Typedef,
		UnresolvedUsingTypename,
		firstType = Enum, lastType = UnresolvedUsingTypename,
		Using,
		UsingDirective,
		UsingPack,
		UsingShadow,
		ConstructorUsingShadow,
		firstUsingShadow = UsingShadow, lastUsingShadow = ConstructorUsingShadow,
		Binding,
		Field,
		ObjCAtDefsField,
		ObjCIvar,
		firstField = Field, lastField = ObjCIvar,
		Function,
		CXXDeductionGuide,
		CXXMethod,
		CXXConstructor,
		CXXConversion,
		CXXDestructor,
		firstCXXMethod = CXXMethod, lastCXXMethod = CXXDestructor,
		firstFunction = Function, lastFunction = CXXDestructor,
		MSProperty,
		NonTypeTemplateParm,
		Var,
		Decomposition,
		ImplicitParam,
		OMPCapturedExpr,
		ParmVar,
		VarTemplateSpecialization,
		VarTemplatePartialSpecialization,
		firstVarTemplateSpecialization = VarTemplateSpecialization, lastVarTemplateSpecialization = VarTemplatePartialSpecialization,
		firstVar = Var, lastVar = VarTemplatePartialSpecialization,
		firstDeclarator = Field, lastDeclarator = VarTemplatePartialSpecialization,
		EnumConstant,
		IndirectField,
		OMPDeclareReduction,
		UnresolvedUsingValue,
		firstValue = Binding, lastValue = UnresolvedUsingValue,
		firstNamed = Label, lastNamed = UnresolvedUsingValue,
		OMPThreadPrivate,
		ObjCPropertyImpl,
		PragmaComment,
		PragmaDetectMismatch,
		StaticAssert,
		TranslationUnit,
		firstDecl = AccessSpec, lastDecl = TranslationUnit
	};

	[ComVisible(true)]
	public enum StmtClass
	{
		NoStmt = 0,
		GCCAsmStmt,
		MSAsmStmt,
		firstAsmStmtConstant = GCCAsmStmt, lastAsmStmtConstant = MSAsmStmt,
		AttributedStmt,
		BreakStmt,
		CXXCatchStmt,
		CXXForRangeStmt,
		CXXTryStmt,
		CapturedStmt,
		CompoundStmt,
		ContinueStmt,
		CoreturnStmt,
		CoroutineBodyStmt,
		DeclStmt,
		DoStmt,
		BinaryConditionalOperator,
		ConditionalOperator,
		firstAbstractConditionalOperatorConstant = BinaryConditionalOperator, lastAbstractConditionalOperatorConstant = ConditionalOperator,
		AddrLabelExpr,
		ArrayInitIndexExpr,
		ArrayInitLoopExpr,
		ArraySubscriptExpr,
		ArrayTypeTraitExpr,
		AsTypeExpr,
		AtomicExpr,
		BinaryOperator,
		CompoundAssignOperator,
		firstBinaryOperatorConstant = BinaryOperator, lastBinaryOperatorConstant = CompoundAssignOperator,
		BlockExpr,
		CXXBindTemporaryExpr,
		CXXBoolLiteralExpr,
		CXXConstructExpr,
		CXXTemporaryObjectExpr,
		firstCXXConstructExprConstant = CXXConstructExpr, lastCXXConstructExprConstant = CXXTemporaryObjectExpr,
		CXXDefaultArgExpr,
		CXXDefaultInitExpr,
		CXXDeleteExpr,
		CXXDependentScopeMemberExpr,
		CXXFoldExpr,
		CXXInheritedCtorInitExpr,
		CXXNewExpr,
		CXXNoexceptExpr,
		CXXNullPtrLiteralExpr,
		CXXPseudoDestructorExpr,
		CXXScalarValueInitExpr,
		CXXStdInitializerListExpr,
		CXXThisExpr,
		CXXThrowExpr,
		CXXTypeidExpr,
		CXXUnresolvedConstructExpr,
		CXXUuidofExpr,
		CallExpr,
		CUDAKernelCallExpr,
		CXXMemberCallExpr,
		CXXOperatorCallExpr,
		UserDefinedLiteral,
		firstCallExprConstant = CallExpr, lastCallExprConstant = UserDefinedLiteral,
		CStyleCastExpr,
		CXXFunctionalCastExpr,
		CXXConstCastExpr,
		CXXDynamicCastExpr,
		CXXReinterpretCastExpr,
		CXXStaticCastExpr,
		firstCXXNamedCastExprConstant = CXXConstCastExpr, lastCXXNamedCastExprConstant = CXXStaticCastExpr,
		ObjCBridgedCastExpr,
		firstExplicitCastExprConstant = CStyleCastExpr, lastExplicitCastExprConstant = ObjCBridgedCastExpr,
		ImplicitCastExpr,
		firstCastExprConstant = CStyleCastExpr, lastCastExprConstant = ImplicitCastExpr,
		CharacterLiteral,
		ChooseExpr,
		CompoundLiteralExpr,
		ConvertVectorExpr,
		CoawaitExpr,
		CoyieldExpr,
		firstCoroutineSuspendExprConstant = CoawaitExpr, lastCoroutineSuspendExprConstant = CoyieldExpr,
		DeclRefExpr,
		DependentCoawaitExpr,
		DependentScopeDeclRefExpr,
		DesignatedInitExpr,
		DesignatedInitUpdateExpr,
		ExprWithCleanups,
		ExpressionTraitExpr,
		ExtVectorElementExpr,
		FloatingLiteral,
		FunctionParmPackExpr,
		GNUNullExpr,
		GenericSelectionExpr,
		ImaginaryLiteral,
		ImplicitValueInitExpr,
		InitListExpr,
		IntegerLiteral,
		LambdaExpr,
		MSPropertyRefExpr,
		MSPropertySubscriptExpr,
		MaterializeTemporaryExpr,
		MemberExpr,
		NoInitExpr,
		OMPArraySectionExpr,
		ObjCArrayLiteral,
		ObjCAvailabilityCheckExpr,
		ObjCBoolLiteralExpr,
		ObjCBoxedExpr,
		ObjCDictionaryLiteral,
		ObjCEncodeExpr,
		ObjCIndirectCopyRestoreExpr,
		ObjCIsaExpr,
		ObjCIvarRefExpr,
		ObjCMessageExpr,
		ObjCPropertyRefExpr,
		ObjCProtocolExpr,
		ObjCSelectorExpr,
		ObjCStringLiteral,
		ObjCSubscriptRefExpr,
		OffsetOfExpr,
		OpaqueValueExpr,
		UnresolvedLookupExpr,
		UnresolvedMemberExpr,
		firstOverloadExprConstant = UnresolvedLookupExpr, lastOverloadExprConstant = UnresolvedMemberExpr,
		PackExpansionExpr,
		ParenExpr,
		ParenListExpr,
		PredefinedExpr,
		PseudoObjectExpr,
		ShuffleVectorExpr,
		SizeOfPackExpr,
		StmtExpr,
		StringLiteral,
		SubstNonTypeTemplateParmExpr,
		SubstNonTypeTemplateParmPackExpr,
		TypeTraitExpr,
		TypoExpr,
		UnaryExprOrTypeTraitExpr,
		UnaryOperator,
		VAArgExpr,
		firstExprConstant = BinaryConditionalOperator, lastExprConstant = VAArgExpr,
		ForStmt,
		GotoStmt,
		IfStmt,
		IndirectGotoStmt,
		LabelStmt,
		MSDependentExistsStmt,
		NullStmt,
		OMPAtomicDirective,
		OMPBarrierDirective,
		OMPCancelDirective,
		OMPCancellationPointDirective,
		OMPCriticalDirective,
		OMPFlushDirective,
		OMPDistributeDirective,
		OMPDistributeParallelForDirective,
		OMPDistributeParallelForSimdDirective,
		OMPDistributeSimdDirective,
		OMPForDirective,
		OMPForSimdDirective,
		OMPParallelForDirective,
		OMPParallelForSimdDirective,
		OMPSimdDirective,
		OMPTargetParallelForSimdDirective,
		OMPTargetSimdDirective,
		OMPTargetTeamsDistributeDirective,
		OMPTargetTeamsDistributeParallelForDirective,
		OMPTargetTeamsDistributeParallelForSimdDirective,
		OMPTargetTeamsDistributeSimdDirective,
		OMPTaskLoopDirective,
		OMPTaskLoopSimdDirective,
		OMPTeamsDistributeDirective,
		OMPTeamsDistributeParallelForDirective,
		OMPTeamsDistributeParallelForSimdDirective,
		OMPTeamsDistributeSimdDirective,
		firstOMPLoopDirectiveConstant = OMPDistributeDirective, lastOMPLoopDirectiveConstant = OMPTeamsDistributeSimdDirective,
		OMPMasterDirective,
		OMPOrderedDirective,
		OMPParallelDirective,
		OMPParallelSectionsDirective,
		OMPSectionDirective,
		OMPSectionsDirective,
		OMPSingleDirective,
		OMPTargetDataDirective,
		OMPTargetDirective,
		OMPTargetEnterDataDirective,
		OMPTargetExitDataDirective,
		OMPTargetParallelDirective,
		OMPTargetParallelForDirective,
		OMPTargetTeamsDirective,
		OMPTargetUpdateDirective,
		OMPTaskDirective,
		OMPTaskgroupDirective,
		OMPTaskwaitDirective,
		OMPTaskyieldDirective,
		OMPTeamsDirective,
		firstOMPExecutableDirectiveConstant = OMPAtomicDirective, lastOMPExecutableDirectiveConstant = OMPTeamsDirective,
		ObjCAtCatchStmt,
		ObjCAtFinallyStmt,
		ObjCAtSynchronizedStmt,
		ObjCAtThrowStmt,
		ObjCAtTryStmt,
		ObjCAutoreleasePoolStmt,
		ObjCForCollectionStmt,
		ReturnStmt,
		SEHExceptStmt,
		SEHFinallyStmt,
		SEHLeaveStmt,
		SEHTryStmt,
		CaseStmt,
		DefaultStmt,
		firstSwitchCaseConstant = CaseStmt, lastSwitchCaseConstant = DefaultStmt,
		SwitchStmt,
		WhileStmt,
		firstStmtConstant = GCCAsmStmt, lastStmtConstant = WhileStmt
	}

	[ComVisible(true)]
	public interface IComClang
	{
		void Start();
		void Exit();
		IDecl CreateDecl(DeclKind kind, IDecl parent);
		IStmt CreateStmt(StmtClass kind, IStmt parent);
		ISourceLocation CreateSourceLocation(string fileName, int lineNo, int columnNo);
		ISourceRange CreateSourceRange(ISourceLocation begin, ISourceLocation end);
		IBinaryOperator CreateBinaryOperator(string str, IExpression lhs, IExpression rhs, string opcodeStr);
		ICXXConstructExpr CreateCXXConstructExpr(string str, IDeclaration decl);
		ICallExpr CreateCallExpr(string str, IDeclaration decl);
		IExpression CreateExpression(string str, StatementClass Class, IDeclaration decl);
		IInitListExpr CreateInitListExpr(string str, IDeclaration decl);
		IStatement CreateStatement(string str, StatementClass Class, IDeclaration decl);
		ISubStmtExpr CreateSubStmtExpr(string str, IDeclaration decl);
		IVTableLayout CreateVTableLayout();
		IClassLayout CreateClassLayout();
		IAccessSpecifierDecl CreateAccessSpecifierDecl();
		IBaseClassSpecifier CreateBaseClassSpecifier();
		IClassTemplateSpecialization CreateClassTemplateSpecialization();
		IClassTemplatePartialSpecialization CreateClassTemplatePartialSpecialization();
		IClassTemplate CreateClassTemplate();
		ITemplateArgument CreateTemplateArgument();
		ITemplateTemplateParameter CreateTemplateTemplateParameter();
		ITypeTemplateParameter CreateTypeTemplateParameter();
		INonTypeTemplateParameter CreateNonTypeTemplateParameter();
		ITypeAliasTemplate CreateTypeAliasTemplate();
		IFunctionTemplate CreateFunctionTemplate();
		IFunctionTemplateSpecialization CreateFunctionTemplateSpecialization();
		IVarTemplate CreateVarTemplate();
		IVarTemplateSpecialization CreateVarTemplateSpecialization();
		IVarTemplatePartialSpecialization CreateVarTemplatePartialSpecialization();
		IMethod CreateMethod();
		IField CreateField();
		IAttributedType CreateAttributedType();
		IBuiltinType CreateBuiltinType();
		ITagType CreateTagType();
		IPointerType CreatePointerType();
		ITypedefType CreateTypedefType();
		IDecayedType CreateDecayedType();
		IArrayType CreateArrayType();
		IFunctionType CreateFunctionType();
		IMemberPointerType CreateMemberPointerType();
		ITemplateSpecializationType CreateTemplateSpecializationType();
		IDependentTemplateSpecializationType CreateDependentTemplateSpecializationType();
		ITemplateParameterType CreateTemplateParameterType();
		ITemplateParameterSubstitutionType CreateTemplateParameterSubstitutionType();
		IInjectedClassNameType CreateInjectedClassNameType();
		IDependentNameType CreateDependentNameType();
		IUnaryTransformType CreateUnaryTransformType();
		IVectorType CreateVectorType();
		IPackExpansionType CreatePackExpansionType();
		IEnumeration CreateEnumeration();
		IEnumeration_Item CreateEnumeration_Item();
		IParameter CreateParameter();
		IFunction CreateFunction();
		IVariable CreateVariable();
		IFriend CreateFriend();
		IMacroExpansion CreateMacroExpansion();
		IMacroDefinition CreateMacroDefinition();
		INativeLibrary CreateNativeLibrary();
		IRawComment CreateRawComment();
		IBlockCommandComment_Argument CreateBlockCommandComment_Argument();
		IFullComment CreateFullComment();
		IBlockCommandComment CreateBlockCommandComment();
		IParamCommandComment CreateParamCommandComment();
		ITParamCommandComment CreateTParamCommandComment();
		IVerbatimBlockComment CreateVerbatimBlockComment();
		IVerbatimLineComment CreateVerbatimLineComment();
		IParagraphComment CreateParagraphComment();
		IHTMLStartTagComment CreateHTMLStartTagComment();
		IHTMLStartTagComment_Attribute CreateHTMLStartTagComment_Attribute();
		IHTMLEndTagComment CreateHTMLEndTagComment();
		ITextComment CreateTextComment();
		IInlineCommandComment CreateInlineCommandComment();
		IInlineCommandComment_Argument CreateInlineCommandComment_Argument();
		IVerbatimBlockLineComment CreateVerbatimBlockLineComment();
		IParserDiagnostic CreateParserDiagnostic();
		IParserResult CreateParserResult();
		ITypeQualifiers CreateTypeQualifiers();
		IQualifiedType CreateQualifiedType();
		IVTableComponent CreateVTableComponent();
		ILayoutField CreateLayoutField();
		ILayoutBase CreateLayoutBase();
		IVFTableInfo CreateVFTableInfo();
		IParserTargetInfo CreateParserTargetInfo();
	}

	[ComVisible(true)]
	public interface IDoxPPCallbacks
	{

	}
}
