using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CppSharp;
using CppSharp.AST;

namespace ComClang
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class ComClang : IComClang
	{
		public List<Decl> Decls { get; } = new List<Decl>();
		public List<Stmt> Stmts { get; } = new List<Stmt>();

		public void Start()
		{
		}

		public void Exit()
		{
		}

		public IDecl CreateDecl(DeclKind kind, IDecl parent)
		{
			Decl ret;

			switch (kind)
			{
			case DeclKind.AccessSpec: ret = new AccessSpecDecl(parent); break;
			case DeclKind.Block: ret = new BlockDecl(parent); break;
			case DeclKind.Captured: ret = new CapturedDecl(parent); break;
			case DeclKind.ClassScopeFunctionSpecialization: ret = new ClassScopeFunctionSpecializationDecl(parent); break;
			case DeclKind.Empty: ret = new EmptyDecl(parent); break;
			case DeclKind.Export: ret = new ExportDecl(parent); break;
			case DeclKind.ExternCContext: ret = new ExternCContextDecl(parent); break;
			case DeclKind.FileScopeAsm: ret = new FileScopeAsmDecl(parent); break;
			case DeclKind.Friend: ret = new FriendDecl(parent); break;
			case DeclKind.FriendTemplate: ret = new FriendTemplateDecl(parent); break;
			case DeclKind.Import: ret = new ImportDecl(parent); break;
			case DeclKind.LinkageSpec: ret = new LinkageSpecDecl(parent); break;
			case DeclKind.Label: ret = new LabelDecl(parent); break;
			case DeclKind.Namespace: ret = new NamespaceDecl(parent); break;
			case DeclKind.NamespaceAlias: ret = new NamespaceAliasDecl(parent); break;
			case DeclKind.ObjCCompatibleAlias: ret = new ObjCCompatibleAliasDecl(parent); break;
			case DeclKind.ObjCCategory: ret = new ObjCCategoryDecl(parent); break;
			case DeclKind.ObjCCategoryImpl: ret = new ObjCCategoryImplDecl(parent); break;
			case DeclKind.ObjCImplementation: ret = new ObjCImplementationDecl(parent); break;
			case DeclKind.ObjCInterface: ret = new ObjCInterfaceDecl(parent); break;
			case DeclKind.ObjCProtocol: ret = new ObjCProtocolDecl(parent); break;
			case DeclKind.ObjCMethod: ret = new ObjCMethodDecl(parent); break;
			case DeclKind.ObjCProperty: ret = new ObjCPropertyDecl(parent); break;
			case DeclKind.BuiltinTemplate: ret = new BuiltinTemplateDecl(parent); break;
			case DeclKind.ClassTemplate: ret = new ClassTemplateDecl(parent); break;
			case DeclKind.FunctionTemplate: ret = new FunctionTemplateDecl(parent); break;
			case DeclKind.TypeAliasTemplate: ret = new TypeAliasTemplateDecl(parent); break;
			case DeclKind.VarTemplate: ret = new VarTemplateDecl(parent); break;
			case DeclKind.TemplateTemplateParm: ret = new TemplateTemplateParmDecl(parent); break;
			case DeclKind.Enum: ret = new EnumDecl(parent); break;
			case DeclKind.Record: ret = new RecordDecl(parent); break;
			case DeclKind.CXXRecord: ret = new CXXRecordDecl(parent); break;
			case DeclKind.ClassTemplateSpecialization: ret = new ClassTemplateSpecializationDecl(parent); break;
			case DeclKind.ClassTemplatePartialSpecialization: ret = new ClassTemplatePartialSpecializationDecl(parent); break;
			case DeclKind.TemplateTypeParm: ret = new TemplateTypeParmDecl(parent); break;
			case DeclKind.ObjCTypeParam: ret = new ObjCTypeParamDecl(parent); break;
			case DeclKind.TypeAlias: ret = new TypeAliasDecl(parent); break;
			case DeclKind.Typedef: ret = new TypedefDecl(parent); break;
			case DeclKind.UnresolvedUsingTypename: ret = new UnresolvedUsingTypenameDecl(parent); break;
			case DeclKind.Using: ret = new UsingDecl(parent); break;
			case DeclKind.UsingDirective: ret = new UsingDirectiveDecl(parent); break;
			case DeclKind.UsingPack: ret = new UsingPackDecl(parent); break;
			case DeclKind.UsingShadow: ret = new UsingShadowDecl(parent); break;
			case DeclKind.ConstructorUsingShadow: ret = new ConstructorUsingShadowDecl(parent); break;
			case DeclKind.Binding: ret = new BindingDecl(parent); break;
			case DeclKind.Field: ret = new FieldDecl(parent); break;
			case DeclKind.ObjCAtDefsField: ret = new ObjCAtDefsFieldDecl(parent); break;
			case DeclKind.ObjCIvar: ret = new ObjCIvarDecl(parent); break;
			case DeclKind.Function: ret = new FunctionDecl(parent); break;
			case DeclKind.CXXDeductionGuide: ret = new CXXDeductionGuideDecl(parent); break;
			case DeclKind.CXXMethod: ret = new CXXMethodDecl(parent); break;
			case DeclKind.CXXConstructor: ret = new CXXConstructorDecl(parent); break;
			case DeclKind.CXXConversion: ret = new CXXConversionDecl(parent); break;
			case DeclKind.CXXDestructor: ret = new CXXDestructorDecl(parent); break;
			case DeclKind.MSProperty: ret = new MSPropertyDecl(parent); break;
			case DeclKind.NonTypeTemplateParm: ret = new NonTypeTemplateParmDecl(parent); break;
			case DeclKind.Var: ret = new VarDecl(parent); break;
			case DeclKind.Decomposition: ret = new DecompositionDecl(parent); break;
			case DeclKind.ImplicitParam: ret = new ImplicitParamDecl(parent); break;
			case DeclKind.OMPCapturedExpr: ret = new OMPCapturedExprDecl(parent); break;
			case DeclKind.ParmVar: ret = new ParmVarDecl(parent); break;
			case DeclKind.VarTemplateSpecialization: ret = new VarTemplateSpecializationDecl(parent); break;
			case DeclKind.VarTemplatePartialSpecialization: ret = new VarTemplatePartialSpecializationDecl(parent); break;
			case DeclKind.EnumConstant: ret = new EnumConstantDecl(parent); break;
			case DeclKind.IndirectField: ret = new IndirectFieldDecl(parent); break;
			case DeclKind.OMPDeclareReduction: ret = new OMPDeclareReductionDecl(parent); break;
			case DeclKind.UnresolvedUsingValue: ret = new UnresolvedUsingValueDecl(parent); break;
			case DeclKind.OMPThreadPrivate: ret = new OMPThreadPrivateDecl(parent); break;
			case DeclKind.ObjCPropertyImpl: ret = new ObjCPropertyImplDecl(parent); break;
			case DeclKind.PragmaComment: ret = new PragmaCommentDecl(parent); break;
			case DeclKind.PragmaDetectMismatch: ret = new PragmaDetectMismatchDecl(parent); break;
			case DeclKind.StaticAssert: ret = new StaticAssertDecl(parent); break;
			case DeclKind.TranslationUnit: ret = new TranslationUnitDecl(parent); break;
			default: throw new NotImplementedException();
			}

			if (parent == null)
				Decls.Add(ret);

			return ret;
		}

		public IStmt CreateStmt(StmtClass @class, IStmt parent)
		{
			Stmt ret;

			switch (@class)
			{
			case StmtClass.GCCAsmStmt: ret = new GCCAsmStmtStmt(parent); break;
			case StmtClass.MSAsmStmt: ret = new MSAsmStmtStmt(parent); break;
			case StmtClass.AttributedStmt: ret = new AttributedStmtStmt(parent); break;
			case StmtClass.BreakStmt: ret = new BreakStmtStmt(parent); break;
			case StmtClass.CXXCatchStmt: ret = new CXXCatchStmtStmt(parent); break;
			case StmtClass.CXXForRangeStmt: ret = new CXXForRangeStmtStmt(parent); break;
			case StmtClass.CXXTryStmt: ret = new CXXTryStmtStmt(parent); break;
			case StmtClass.CapturedStmt: ret = new CapturedStmtStmt(parent); break;
			case StmtClass.CompoundStmt: ret = new CompoundStmtStmt(parent); break;
			case StmtClass.ContinueStmt: ret = new ContinueStmtStmt(parent); break;
			case StmtClass.CoreturnStmt: ret = new CoreturnStmtStmt(parent); break;
			case StmtClass.CoroutineBodyStmt: ret = new CoroutineBodyStmtStmt(parent); break;
			case StmtClass.DeclStmt: ret = new DeclStmtStmt(parent); break;
			case StmtClass.DoStmt: ret = new DoStmtStmt(parent); break;
			case StmtClass.BinaryConditionalOperator: ret = new BinaryConditionalOperatorStmt(parent); break;
			case StmtClass.ConditionalOperator: ret = new ConditionalOperatorStmt(parent); break;
			case StmtClass.AddrLabelExpr: ret = new AddrLabelExprStmt(parent); break;
			case StmtClass.ArrayInitIndexExpr: ret = new ArrayInitIndexExprStmt(parent); break;
			case StmtClass.ArrayInitLoopExpr: ret = new ArrayInitLoopExprStmt(parent); break;
			case StmtClass.ArraySubscriptExpr: ret = new ArraySubscriptExprStmt(parent); break;
			case StmtClass.ArrayTypeTraitExpr: ret = new ArrayTypeTraitExprStmt(parent); break;
			case StmtClass.AsTypeExpr: ret = new AsTypeExprStmt(parent); break;
			case StmtClass.AtomicExpr: ret = new AtomicExprStmt(parent); break;
			case StmtClass.BinaryOperator: ret = new BinaryOperatorStmt(parent); break;
			case StmtClass.CompoundAssignOperator: ret = new CompoundAssignOperatorStmt(parent); break;
			case StmtClass.BlockExpr: ret = new BlockExprStmt(parent); break;
			case StmtClass.CXXBindTemporaryExpr: ret = new CXXBindTemporaryExprStmt(parent); break;
			case StmtClass.CXXBoolLiteralExpr: ret = new CXXBoolLiteralExprStmt(parent); break;
			case StmtClass.CXXConstructExpr: ret = new CXXConstructExprStmt(parent); break;
			case StmtClass.CXXTemporaryObjectExpr: ret = new CXXTemporaryObjectExprStmt(parent); break;
			case StmtClass.CXXDefaultArgExpr: ret = new CXXDefaultArgExprStmt(parent); break;
			case StmtClass.CXXDefaultInitExpr: ret = new CXXDefaultInitExprStmt(parent); break;
			case StmtClass.CXXDeleteExpr: ret = new CXXDeleteExprStmt(parent); break;
			case StmtClass.CXXDependentScopeMemberExpr: ret = new CXXDependentScopeMemberExprStmt(parent); break;
			case StmtClass.CXXFoldExpr: ret = new CXXFoldExprStmt(parent); break;
			case StmtClass.CXXInheritedCtorInitExpr: ret = new CXXInheritedCtorInitExprStmt(parent); break;
			case StmtClass.CXXNewExpr: ret = new CXXNewExprStmt(parent); break;
			case StmtClass.CXXNoexceptExpr: ret = new CXXNoexceptExprStmt(parent); break;
			case StmtClass.CXXNullPtrLiteralExpr: ret = new CXXNullPtrLiteralExprStmt(parent); break;
			case StmtClass.CXXPseudoDestructorExpr: ret = new CXXPseudoDestructorExprStmt(parent); break;
			case StmtClass.CXXScalarValueInitExpr: ret = new CXXScalarValueInitExprStmt(parent); break;
			case StmtClass.CXXStdInitializerListExpr: ret = new CXXStdInitializerListExprStmt(parent); break;
			case StmtClass.CXXThisExpr: ret = new CXXThisExprStmt(parent); break;
			case StmtClass.CXXThrowExpr: ret = new CXXThrowExprStmt(parent); break;
			case StmtClass.CXXTypeidExpr: ret = new CXXTypeidExprStmt(parent); break;
			case StmtClass.CXXUnresolvedConstructExpr: ret = new CXXUnresolvedConstructExprStmt(parent); break;
			case StmtClass.CXXUuidofExpr: ret = new CXXUuidofExprStmt(parent); break;
			case StmtClass.CallExpr: ret = new CallExprStmt(parent); break;
			case StmtClass.CUDAKernelCallExpr: ret = new CUDAKernelCallExprStmt(parent); break;
			case StmtClass.CXXMemberCallExpr: ret = new CXXMemberCallExprStmt(parent); break;
			case StmtClass.CXXOperatorCallExpr: ret = new CXXOperatorCallExprStmt(parent); break;
			case StmtClass.UserDefinedLiteral: ret = new UserDefinedLiteralStmt(parent); break;
			case StmtClass.CStyleCastExpr: ret = new CStyleCastExprStmt(parent); break;
			case StmtClass.CXXFunctionalCastExpr: ret = new CXXFunctionalCastExprStmt(parent); break;
			case StmtClass.CXXConstCastExpr: ret = new CXXConstCastExprStmt(parent); break;
			case StmtClass.CXXDynamicCastExpr: ret = new CXXDynamicCastExprStmt(parent); break;
			case StmtClass.CXXReinterpretCastExpr: ret = new CXXReinterpretCastExprStmt(parent); break;
			case StmtClass.CXXStaticCastExpr: ret = new CXXStaticCastExprStmt(parent); break;
			case StmtClass.ObjCBridgedCastExpr: ret = new ObjCBridgedCastExprStmt(parent); break;
			case StmtClass.ImplicitCastExpr: ret = new ImplicitCastExprStmt(parent); break;
			case StmtClass.CharacterLiteral: ret = new CharacterLiteralStmt(parent); break;
			case StmtClass.ChooseExpr: ret = new ChooseExprStmt(parent); break;
			case StmtClass.CompoundLiteralExpr: ret = new CompoundLiteralExprStmt(parent); break;
			case StmtClass.ConvertVectorExpr: ret = new ConvertVectorExprStmt(parent); break;
			case StmtClass.CoawaitExpr: ret = new CoawaitExprStmt(parent); break;
			case StmtClass.CoyieldExpr: ret = new CoyieldExprStmt(parent); break;
			case StmtClass.DeclRefExpr: ret = new DeclRefExprStmt(parent); break;
			case StmtClass.DependentCoawaitExpr: ret = new DependentCoawaitExprStmt(parent); break;
			case StmtClass.DependentScopeDeclRefExpr: ret = new DependentScopeDeclRefExprStmt(parent); break;
			case StmtClass.DesignatedInitExpr: ret = new DesignatedInitExprStmt(parent); break;
			case StmtClass.DesignatedInitUpdateExpr: ret = new DesignatedInitUpdateExprStmt(parent); break;
			case StmtClass.ExprWithCleanups: ret = new ExprWithCleanupsStmt(parent); break;
			case StmtClass.ExpressionTraitExpr: ret = new ExpressionTraitExprStmt(parent); break;
			case StmtClass.ExtVectorElementExpr: ret = new ExtVectorElementExprStmt(parent); break;
			case StmtClass.FloatingLiteral: ret = new FloatingLiteralStmt(parent); break;
			case StmtClass.FunctionParmPackExpr: ret = new FunctionParmPackExprStmt(parent); break;
			case StmtClass.GNUNullExpr: ret = new GNUNullExprStmt(parent); break;
			case StmtClass.GenericSelectionExpr: ret = new GenericSelectionExprStmt(parent); break;
			case StmtClass.ImaginaryLiteral: ret = new ImaginaryLiteralStmt(parent); break;
			case StmtClass.ImplicitValueInitExpr: ret = new ImplicitValueInitExprStmt(parent); break;
			case StmtClass.InitListExpr: ret = new InitListExprStmt(parent); break;
			case StmtClass.IntegerLiteral: ret = new IntegerLiteralStmt(parent); break;
			case StmtClass.LambdaExpr: ret = new LambdaExprStmt(parent); break;
			case StmtClass.MSPropertyRefExpr: ret = new MSPropertyRefExprStmt(parent); break;
			case StmtClass.MSPropertySubscriptExpr: ret = new MSPropertySubscriptExprStmt(parent); break;
			case StmtClass.MaterializeTemporaryExpr: ret = new MaterializeTemporaryExprStmt(parent); break;
			case StmtClass.MemberExpr: ret = new MemberExprStmt(parent); break;
			case StmtClass.NoInitExpr: ret = new NoInitExprStmt(parent); break;
			case StmtClass.OMPArraySectionExpr: ret = new OMPArraySectionExprStmt(parent); break;
			case StmtClass.ObjCArrayLiteral: ret = new ObjCArrayLiteralStmt(parent); break;
			case StmtClass.ObjCAvailabilityCheckExpr: ret = new ObjCAvailabilityCheckExprStmt(parent); break;
			case StmtClass.ObjCBoolLiteralExpr: ret = new ObjCBoolLiteralExprStmt(parent); break;
			case StmtClass.ObjCBoxedExpr: ret = new ObjCBoxedExprStmt(parent); break;
			case StmtClass.ObjCDictionaryLiteral: ret = new ObjCDictionaryLiteralStmt(parent); break;
			case StmtClass.ObjCEncodeExpr: ret = new ObjCEncodeExprStmt(parent); break;
			case StmtClass.ObjCIndirectCopyRestoreExpr: ret = new ObjCIndirectCopyRestoreExprStmt(parent); break;
			case StmtClass.ObjCIsaExpr: ret = new ObjCIsaExprStmt(parent); break;
			case StmtClass.ObjCIvarRefExpr: ret = new ObjCIvarRefExprStmt(parent); break;
			case StmtClass.ObjCMessageExpr: ret = new ObjCMessageExprStmt(parent); break;
			case StmtClass.ObjCPropertyRefExpr: ret = new ObjCPropertyRefExprStmt(parent); break;
			case StmtClass.ObjCProtocolExpr: ret = new ObjCProtocolExprStmt(parent); break;
			case StmtClass.ObjCSelectorExpr: ret = new ObjCSelectorExprStmt(parent); break;
			case StmtClass.ObjCStringLiteral: ret = new ObjCStringLiteralStmt(parent); break;
			case StmtClass.ObjCSubscriptRefExpr: ret = new ObjCSubscriptRefExprStmt(parent); break;
			case StmtClass.OffsetOfExpr: ret = new OffsetOfExprStmt(parent); break;
			case StmtClass.OpaqueValueExpr: ret = new OpaqueValueExprStmt(parent); break;
			case StmtClass.UnresolvedLookupExpr: ret = new UnresolvedLookupExprStmt(parent); break;
			case StmtClass.UnresolvedMemberExpr: ret = new UnresolvedMemberExprStmt(parent); break;
			case StmtClass.PackExpansionExpr: ret = new PackExpansionExprStmt(parent); break;
			case StmtClass.ParenExpr: ret = new ParenExprStmt(parent); break;
			case StmtClass.ParenListExpr: ret = new ParenListExprStmt(parent); break;
			case StmtClass.PredefinedExpr: ret = new PredefinedExprStmt(parent); break;
			case StmtClass.PseudoObjectExpr: ret = new PseudoObjectExprStmt(parent); break;
			case StmtClass.ShuffleVectorExpr: ret = new ShuffleVectorExprStmt(parent); break;
			case StmtClass.SizeOfPackExpr: ret = new SizeOfPackExprStmt(parent); break;
			case StmtClass.StmtExpr: ret = new StmtExprStmt(parent); break;
			case StmtClass.StringLiteral: ret = new StringLiteralStmt(parent); break;
			case StmtClass.SubstNonTypeTemplateParmExpr: ret = new SubstNonTypeTemplateParmExprStmt(parent); break;
			case StmtClass.SubstNonTypeTemplateParmPackExpr: ret = new SubstNonTypeTemplateParmPackExprStmt(parent); break;
			case StmtClass.TypeTraitExpr: ret = new TypeTraitExprStmt(parent); break;
			case StmtClass.TypoExpr: ret = new TypoExprStmt(parent); break;
			case StmtClass.UnaryExprOrTypeTraitExpr: ret = new UnaryExprOrTypeTraitExprStmt(parent); break;
			case StmtClass.UnaryOperator: ret = new UnaryOperatorStmt(parent); break;
			case StmtClass.VAArgExpr: ret = new VAArgExprStmt(parent); break;
			case StmtClass.ForStmt: ret = new ForStmtStmt(parent); break;
			case StmtClass.GotoStmt: ret = new GotoStmtStmt(parent); break;
			case StmtClass.IfStmt: ret = new IfStmtStmt(parent); break;
			case StmtClass.IndirectGotoStmt: ret = new IndirectGotoStmtStmt(parent); break;
			case StmtClass.LabelStmt: ret = new LabelStmtStmt(parent); break;
			case StmtClass.MSDependentExistsStmt: ret = new MSDependentExistsStmtStmt(parent); break;
			case StmtClass.NullStmt: ret = new NullStmtStmt(parent); break;
			case StmtClass.OMPAtomicDirective: ret = new OMPAtomicDirectiveStmt(parent); break;
			case StmtClass.OMPBarrierDirective: ret = new OMPBarrierDirectiveStmt(parent); break;
			case StmtClass.OMPCancelDirective: ret = new OMPCancelDirectiveStmt(parent); break;
			case StmtClass.OMPCancellationPointDirective: ret = new OMPCancellationPointDirectiveStmt(parent); break;
			case StmtClass.OMPCriticalDirective: ret = new OMPCriticalDirectiveStmt(parent); break;
			case StmtClass.OMPFlushDirective: ret = new OMPFlushDirectiveStmt(parent); break;
			case StmtClass.OMPDistributeDirective: ret = new OMPDistributeDirectiveStmt(parent); break;
			case StmtClass.OMPDistributeParallelForDirective: ret = new OMPDistributeParallelForDirectiveStmt(parent); break;
			case StmtClass.OMPDistributeParallelForSimdDirective: ret = new OMPDistributeParallelForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPDistributeSimdDirective: ret = new OMPDistributeSimdDirectiveStmt(parent); break;
			case StmtClass.OMPForDirective: ret = new OMPForDirectiveStmt(parent); break;
			case StmtClass.OMPForSimdDirective: ret = new OMPForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPParallelForDirective: ret = new OMPParallelForDirectiveStmt(parent); break;
			case StmtClass.OMPParallelForSimdDirective: ret = new OMPParallelForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPSimdDirective: ret = new OMPSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTargetParallelForSimdDirective: ret = new OMPTargetParallelForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTargetSimdDirective: ret = new OMPTargetSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTargetTeamsDistributeDirective: ret = new OMPTargetTeamsDistributeDirectiveStmt(parent); break;
			case StmtClass.OMPTargetTeamsDistributeParallelForDirective: ret = new OMPTargetTeamsDistributeParallelForDirectiveStmt(parent); break;
			case StmtClass.OMPTargetTeamsDistributeParallelForSimdDirective: ret = new OMPTargetTeamsDistributeParallelForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTargetTeamsDistributeSimdDirective: ret = new OMPTargetTeamsDistributeSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTaskLoopDirective: ret = new OMPTaskLoopDirectiveStmt(parent); break;
			case StmtClass.OMPTaskLoopSimdDirective: ret = new OMPTaskLoopSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTeamsDistributeDirective: ret = new OMPTeamsDistributeDirectiveStmt(parent); break;
			case StmtClass.OMPTeamsDistributeParallelForDirective: ret = new OMPTeamsDistributeParallelForDirectiveStmt(parent); break;
			case StmtClass.OMPTeamsDistributeParallelForSimdDirective: ret = new OMPTeamsDistributeParallelForSimdDirectiveStmt(parent); break;
			case StmtClass.OMPTeamsDistributeSimdDirective: ret = new OMPTeamsDistributeSimdDirectiveStmt(parent); break;
			case StmtClass.OMPMasterDirective: ret = new OMPMasterDirectiveStmt(parent); break;
			case StmtClass.OMPOrderedDirective: ret = new OMPOrderedDirectiveStmt(parent); break;
			case StmtClass.OMPParallelDirective: ret = new OMPParallelDirectiveStmt(parent); break;
			case StmtClass.OMPParallelSectionsDirective: ret = new OMPParallelSectionsDirectiveStmt(parent); break;
			case StmtClass.OMPSectionDirective: ret = new OMPSectionDirectiveStmt(parent); break;
			case StmtClass.OMPSectionsDirective: ret = new OMPSectionsDirectiveStmt(parent); break;
			case StmtClass.OMPSingleDirective: ret = new OMPSingleDirectiveStmt(parent); break;
			case StmtClass.OMPTargetDataDirective: ret = new OMPTargetDataDirectiveStmt(parent); break;
			case StmtClass.OMPTargetDirective: ret = new OMPTargetDirectiveStmt(parent); break;
			case StmtClass.OMPTargetEnterDataDirective: ret = new OMPTargetEnterDataDirectiveStmt(parent); break;
			case StmtClass.OMPTargetExitDataDirective: ret = new OMPTargetExitDataDirectiveStmt(parent); break;
			case StmtClass.OMPTargetParallelDirective: ret = new OMPTargetParallelDirectiveStmt(parent); break;
			case StmtClass.OMPTargetParallelForDirective: ret = new OMPTargetParallelForDirectiveStmt(parent); break;
			case StmtClass.OMPTargetTeamsDirective: ret = new OMPTargetTeamsDirectiveStmt(parent); break;
			case StmtClass.OMPTargetUpdateDirective: ret = new OMPTargetUpdateDirectiveStmt(parent); break;
			case StmtClass.OMPTaskDirective: ret = new OMPTaskDirectiveStmt(parent); break;
			case StmtClass.OMPTaskgroupDirective: ret = new OMPTaskgroupDirectiveStmt(parent); break;
			case StmtClass.OMPTaskwaitDirective: ret = new OMPTaskwaitDirectiveStmt(parent); break;
			case StmtClass.OMPTaskyieldDirective: ret = new OMPTaskyieldDirectiveStmt(parent); break;
			case StmtClass.OMPTeamsDirective: ret = new OMPTeamsDirectiveStmt(parent); break;
			case StmtClass.ObjCAtCatchStmt: ret = new ObjCAtCatchStmtStmt(parent); break;
			case StmtClass.ObjCAtFinallyStmt: ret = new ObjCAtFinallyStmtStmt(parent); break;
			case StmtClass.ObjCAtSynchronizedStmt: ret = new ObjCAtSynchronizedStmtStmt(parent); break;
			case StmtClass.ObjCAtThrowStmt: ret = new ObjCAtThrowStmtStmt(parent); break;
			case StmtClass.ObjCAtTryStmt: ret = new ObjCAtTryStmtStmt(parent); break;
			case StmtClass.ObjCAutoreleasePoolStmt: ret = new ObjCAutoreleasePoolStmtStmt(parent); break;
			case StmtClass.ObjCForCollectionStmt: ret = new ObjCForCollectionStmtStmt(parent); break;
			case StmtClass.ReturnStmt: ret = new ReturnStmtStmt(parent); break;
			case StmtClass.SEHExceptStmt: ret = new SEHExceptStmtStmt(parent); break;
			case StmtClass.SEHFinallyStmt: ret = new SEHFinallyStmtStmt(parent); break;
			case StmtClass.SEHLeaveStmt: ret = new SEHLeaveStmtStmt(parent); break;
			case StmtClass.SEHTryStmt: ret = new SEHTryStmtStmt(parent); break;
			case StmtClass.CaseStmt: ret = new CaseStmtStmt(parent); break;
			case StmtClass.DefaultStmt: ret = new DefaultStmtStmt(parent); break;
			case StmtClass.SwitchStmt: ret = new SwitchStmtStmt(parent); break;
			case StmtClass.WhileStmt: ret = new WhileStmtStmt(parent); break;
			default: throw new NotImplementedException();
			}

			if (parent == null)
				Stmts.Add(ret);

			return ret;
		}

		public ISourceLocation CreateSourceLocation(string fileName, int lineNo, int columnNo)
		{
			return new SourceLocation(fileName, lineNo, columnNo);
		}

		public ISourceRange CreateSourceRange(ISourceLocation begin, ISourceLocation end)
		{
			return new SourceRange(begin, end);
		}

		public IBinaryOperator CreateBinaryOperator(string str, IExpression lhs, IExpression rhs, string opcodeStr)
		{
			var ret = new CppSharp.AST.BinaryOperator((CppSharp.AST.Expression)lhs, (CppSharp.AST.Expression)rhs, opcodeStr);
			ret.String = str;
			return ret;
		}

		public ICXXConstructExpr CreateCXXConstructExpr(string str, IDeclaration decl)
		{
			var ret = new CppSharp.AST.CXXConstructExpr();
			ret.String = str;
			return ret;
		}

		public ICallExpr CreateCallExpr(string str, IDeclaration decl)
		{
			var ret = new CppSharp.AST.CallExpr();
			ret.String = str;
			return ret;
		}

		public IExpression CreateExpression(string str, StatementClass Class, IDeclaration decl)
		{
			IExpression ret = null;

			switch (Class)
			{
			/*case StatementClass.BinaryOperator:
				ret = new CppSharp.AST.BinaryOperator();
				break;*/
			case StatementClass.DeclarationReference:
				ret = new CppSharp.AST.BuiltinTypeExpression();
				ret.Class = StatementClass.DeclarationReference;
				break;
			case StatementClass.Call:
				ret = new CppSharp.AST.CallExpr();
				break;
			case StatementClass.ConstructorReference:
				ret = new CppSharp.AST.CXXConstructExpr();
				break;
			case StatementClass.CXXOperatorCall:
				ret = new CppSharp.AST.BuiltinTypeExpression();
				ret.Class = StatementClass.CXXOperatorCall;
				break;
			case StatementClass.ImplicitCast:
				ret = new CppSharp.AST.CallExpr();
				break;
			case StatementClass.ExplicitCast:
				ret = new CppSharp.AST.CallExpr();
				break;
			case StatementClass.InitList:
				ret = new CppSharp.AST.InitListExpr();
				break;
			case StatementClass.SubStmt:
				ret = new CppSharp.AST.SubStmtExpr();
				break;
			default:
				throw new NotImplementedException();
			}
			ret.String = str;
			ret.Declaration = decl;

			return ret;
		}

		public IInitListExpr CreateInitListExpr(string str, IDeclaration decl)
		{
			var ret = (IInitListExpr)new CppSharp.AST.InitListExpr();
			ret.String = str;
			ret.Declaration = decl;
			return ret;
		}

		public IStatement CreateStatement(string str, StatementClass Class, IDeclaration decl)
		{
			var ret = new Statement();
			ret.String = str;
			return ret;
		}

		public ISubStmtExpr CreateSubStmtExpr(string str, IDeclaration decl)
		{
			return new SubStmtExpr();
		}

		public IVTableLayout CreateVTableLayout()
		{
			return new VTableLayout();
		}

		public IClassLayout CreateClassLayout()
		{
			return new ClassLayout();
		}

		public IAccessSpecifierDecl CreateAccessSpecifierDecl()
		{
			return new AccessSpecifierDecl();
		}

		public IBaseClassSpecifier CreateBaseClassSpecifier()
		{
			return new BaseClassSpecifier();
		}

		public IClassTemplateSpecialization CreateClassTemplateSpecialization()
		{
			return new ClassTemplateSpecialization();
		}

		public IClassTemplatePartialSpecialization CreateClassTemplatePartialSpecialization()
		{
			return new ClassTemplatePartialSpecialization();
		}

		public IClassTemplate CreateClassTemplate()
		{
			return new ClassTemplate();
		}

		public ITemplateArgument CreateTemplateArgument()
		{
			return new TemplateArgument();
		}

		public ITemplateTemplateParameter CreateTemplateTemplateParameter()
		{
			return new TemplateTemplateParameter();
		}

		public ITypeTemplateParameter CreateTypeTemplateParameter()
		{
			return new TypeTemplateParameter();
		}

		public INonTypeTemplateParameter CreateNonTypeTemplateParameter()
		{
			return new NonTypeTemplateParameter();
		}

		public ITypeAliasTemplate CreateTypeAliasTemplate()
		{
			return new TypeAliasTemplate();
		}

		public IFunctionTemplate CreateFunctionTemplate()
		{
			return new FunctionTemplate();
		}

		public IFunctionTemplateSpecialization CreateFunctionTemplateSpecialization()
		{
			return new FunctionTemplateSpecialization();
		}

		public IVarTemplate CreateVarTemplate()
		{
			return new VarTemplate();
		}

		public IVarTemplateSpecialization CreateVarTemplateSpecialization()
		{
			return new VarTemplateSpecialization();
		}

		public IVarTemplatePartialSpecialization CreateVarTemplatePartialSpecialization()
		{
			return new VarTemplatePartialSpecialization();
		}

		public IMethod CreateMethod()
		{
			return new Method();
		}

		public IField CreateField()
		{
			return new Field();
		}

		public IAttributedType CreateAttributedType()
		{
			return new AttributedType();
		}

		public IBuiltinType CreateBuiltinType()
		{
			return new BuiltinType();
		}

		public ITagType CreateTagType()
		{
			return new TagType();
		}

		public IPointerType CreatePointerType()
		{
			return new PointerType();
		}

		public ITypedefType CreateTypedefType()
		{
			return new TypedefType();
		}

		public IDecayedType CreateDecayedType()
		{
			return new DecayedType();
		}

		public IArrayType CreateArrayType()
		{
			return new ArrayType();
		}

		public IFunctionType CreateFunctionType()
		{
			return new FunctionType();
		}

		public IMemberPointerType CreateMemberPointerType()
		{
			return new MemberPointerType();
		}

		public ITemplateSpecializationType CreateTemplateSpecializationType()
		{
			return new TemplateSpecializationType();
		}

		public IDependentTemplateSpecializationType CreateDependentTemplateSpecializationType()
		{
			return new DependentTemplateSpecializationType();
		}

		public ITemplateParameterType CreateTemplateParameterType()
		{
			return new TemplateParameterType();
		}

		public ITemplateParameterSubstitutionType CreateTemplateParameterSubstitutionType()
		{
			return new TemplateParameterSubstitutionType();
		}

		public IInjectedClassNameType CreateInjectedClassNameType()
		{
			return new InjectedClassNameType();
		}

		public IDependentNameType CreateDependentNameType()
		{
			return new DependentNameType();
		}

		public IUnaryTransformType CreateUnaryTransformType()
		{
			return new UnaryTransformType();
		}

		public IVectorType CreateVectorType()
		{
			return new VectorType();
		}

		public IPackExpansionType CreatePackExpansionType()
		{
			return new PackExpansionType();
		}

		public IEnumeration CreateEnumeration()
		{
			return new Enumeration();
		}

		public IEnumeration_Item CreateEnumeration_Item()
		{
			return new Enumeration.Item();
		}

		public IParameter CreateParameter()
		{
			return new Parameter();
		}

		public IFunction CreateFunction()
		{
			return new Function();
		}

		public IVariable CreateVariable()
		{
			return new Variable();
		}

		public IFriend CreateFriend()
		{
			return new Friend();
		}

		public IMacroExpansion CreateMacroExpansion()
		{
			return new MacroExpansion();
		}

		public IMacroDefinition CreateMacroDefinition()
		{
			return new MacroDefinition();
		}

		public INativeLibrary CreateNativeLibrary()
		{
			return new NativeLibrary();
		}

		public IRawComment CreateRawComment()
		{
			return new RawComment();
		}

		public IBlockCommandComment_Argument CreateBlockCommandComment_Argument()
		{
			return new BlockCommandComment.Argument();
		}

		public IFullComment CreateFullComment()
		{
			return new FullComment();
		}

		public IBlockCommandComment CreateBlockCommandComment()
		{
			return new BlockCommandComment();
		}

		public IParamCommandComment CreateParamCommandComment()
		{
			return new ParamCommandComment();
		}

		public ITParamCommandComment CreateTParamCommandComment()
		{
			return new TParamCommandComment();
		}

		public IVerbatimBlockComment CreateVerbatimBlockComment()
		{
			return new VerbatimBlockComment();
		}

		public IVerbatimLineComment CreateVerbatimLineComment()
		{
			return new VerbatimLineComment();
		}

		public IParagraphComment CreateParagraphComment()
		{
			return new ParagraphComment();
		}

		public IHTMLStartTagComment CreateHTMLStartTagComment()
		{
			return new HTMLStartTagComment();
		}

		public IHTMLStartTagComment_Attribute CreateHTMLStartTagComment_Attribute()
		{
			return new HTMLStartTagComment.Attribute();
		}

		public IHTMLEndTagComment CreateHTMLEndTagComment()
		{
			return new HTMLEndTagComment();
		}

		public ITextComment CreateTextComment()
		{
			return new TextComment();
		}

		public IInlineCommandComment CreateInlineCommandComment()
		{
			return new InlineCommandComment();
		}

		public IInlineCommandComment_Argument CreateInlineCommandComment_Argument()
		{
			return new InlineCommandComment.Argument();
		}

		public IVerbatimBlockLineComment CreateVerbatimBlockLineComment()
		{
			return new VerbatimBlockLineComment();
		}

		public IParserDiagnostic CreateParserDiagnostic()
		{
			return new ParserDiagnostic();
		}

		public IParserResult CreateParserResult()
		{
			var result = new ParserResult();
			result.AddedDiagnostics += Result_AddedDiagnostics;
			return result;
		}

		private void Result_AddedDiagnostics(object sender, ParserDiagnostic e)
		{
			Console.WriteLine(e.Message);
		}

		public ITypeQualifiers CreateTypeQualifiers()
		{
			return new TypeQualifiers();
		}

		public IQualifiedType CreateQualifiedType()
		{
			return new QualifiedType();
		}

		public IVTableComponent CreateVTableComponent()
		{
			return new VTableComponent();
		}

		public ILayoutField CreateLayoutField()
		{
			return new LayoutField();
		}

		public ILayoutBase CreateLayoutBase()
		{
			return new LayoutBase();
		}

		public IVFTableInfo CreateVFTableInfo()
		{
			return new VFTableInfo();
		}

		public IParserTargetInfo CreateParserTargetInfo()
		{
			return new ParserTargetInfo();
		}
	}

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Decl : IDecl
	{
		public Decl(DeclKind kind, IDecl parent)
		{
			Kind = kind;
			Parent = parent as Decl;
			if (Parent != null)
				Parent.Decls.Add(this);
		}

		public DeclKind Kind { get; }
		public IntPtr Native { get; set; }
		public Decl Parent { get; }
		public List<Decl> Decls { get; } = new List<Decl>();
		public ISourceLocation Begin { get; private set; }
		public ISourceLocation End { get; private set; }

		public void SetRange(ISourceLocation begin, ISourceLocation end)
		{
			Begin = begin;
			End = end;
			Console.WriteLine($"{Kind}, {begin} - {end}");
		}

		public virtual void SetName(string name)
		{
		}

		public void SetStmt(string name, IStmt stmt)
		{
			var prop = GetType().GetProperty(name, typeof(IStmt));
			if (prop == null)
				return;
			prop.SetValue(this, stmt);
		}
	}

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class Stmt : IStmt
	{
		public Stmt(StmtClass kind, IStmt parent)
		{
			Class = kind;
			Parent = parent as Stmt;
			if (Parent != null)
				Parent.Stmts.Add(this);
		}

		public StmtClass Class { get; }
		public IntPtr Native { get; set; }
		public Stmt Parent { get; }
		public List<Stmt> Stmts { get; } = new List<Stmt>();
		public ISourceLocation Begin { get; private set; }
		public ISourceLocation End { get; private set; }

		public void SetRange(ISourceLocation begin, ISourceLocation end)
		{
			Begin = begin;
			End = end;
			Console.WriteLine($"{Class}, {begin} - {end}");
		}

		public virtual void SetName(string name)
		{
		}
	}

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class SourceLocation : ISourceLocation
	{
		public string FileName { get; set; } = "";
		public int LineNo { get; set; }
		public int ColumnNo { get; set; }

		public SourceLocation(string fileName, int lineNo, int columnNo)
		{
			FileName = fileName;
			LineNo = lineNo;
			ColumnNo = columnNo;
		}

		public override string ToString()
		{
			return $"{FileName}:{LineNo}:{ColumnNo}";
		}
	}

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public class SourceRange : ISourceRange
	{
		public SourceRange(ISourceLocation begin, ISourceLocation end)
		{
			this.Begin = begin;
			this.End = end;
		}

		public ISourceLocation Begin { get; set; }
		public ISourceLocation End { get; set; }
	}
}
