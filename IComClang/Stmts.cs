using System;

using System.Collections.Generic;

using System.Linq;

using System.Runtime.InteropServices;

using System.Text;

using System.Threading.Tasks;

namespace ComClang
{

		public IStmt CreateStmt(StmtClass @class, IStmt parent)
		{

			Stmt ret;

			switch (@class)
			{

case StmtClass.GCCAsmStmtClass:
 ret = new GCCAsmStmtStmt(parent);
 break;

case StmtClass.MSAsmStmtClass:
 ret = new MSAsmStmtStmt(parent);
 break;

case StmtClass.AttributedStmtClass:
 ret = new AttributedStmtStmt(parent);
 break;

case StmtClass.BreakStmtClass:
 ret = new BreakStmtStmt(parent);
 break;

case StmtClass.CXXCatchStmtClass:
 ret = new CXXCatchStmtStmt(parent);
 break;

case StmtClass.CXXForRangeStmtClass:
 ret = new CXXForRangeStmtStmt(parent);
 break;

case StmtClass.CXXTryStmtClass:
 ret = new CXXTryStmtStmt(parent);
 break;

case StmtClass.CapturedStmtClass:
 ret = new CapturedStmtStmt(parent);
 break;

case StmtClass.CompoundStmtClass:
 ret = new CompoundStmtStmt(parent);
 break;

case StmtClass.ContinueStmtClass:
 ret = new ContinueStmtStmt(parent);
 break;

case StmtClass.CoreturnStmtClass:
 ret = new CoreturnStmtStmt(parent);
 break;

case StmtClass.CoroutineBodyStmtClass:
 ret = new CoroutineBodyStmtStmt(parent);
 break;

case StmtClass.DeclStmtClass:
 ret = new DeclStmtStmt(parent);
 break;

case StmtClass.DoStmtClass:
 ret = new DoStmtStmt(parent);
 break;

case StmtClass.BinaryConditionalOperatorClass:
 ret = new BinaryConditionalOperatorStmt(parent);
 break;

case StmtClass.ConditionalOperatorClass:
 ret = new ConditionalOperatorStmt(parent);
 break;

case StmtClass.AddrLabelExprClass:
 ret = new AddrLabelExprStmt(parent);
 break;

case StmtClass.ArrayInitIndexExprClass:
 ret = new ArrayInitIndexExprStmt(parent);
 break;

case StmtClass.ArrayInitLoopExprClass:
 ret = new ArrayInitLoopExprStmt(parent);
 break;

case StmtClass.ArraySubscriptExprClass:
 ret = new ArraySubscriptExprStmt(parent);
 break;

case StmtClass.ArrayTypeTraitExprClass:
 ret = new ArrayTypeTraitExprStmt(parent);
 break;

case StmtClass.AsTypeExprClass:
 ret = new AsTypeExprStmt(parent);
 break;

case StmtClass.AtomicExprClass:
 ret = new AtomicExprStmt(parent);
 break;

case StmtClass.BinaryOperatorClass:
 ret = new BinaryOperatorStmt(parent);
 break;

case StmtClass.CompoundAssignOperatorClass:
 ret = new CompoundAssignOperatorStmt(parent);
 break;

case StmtClass.BlockExprClass:
 ret = new BlockExprStmt(parent);
 break;

case StmtClass.CXXBindTemporaryExprClass:
 ret = new CXXBindTemporaryExprStmt(parent);
 break;

case StmtClass.CXXBoolLiteralExprClass:
 ret = new CXXBoolLiteralExprStmt(parent);
 break;

case StmtClass.CXXConstructExprClass:
 ret = new CXXConstructExprStmt(parent);
 break;

case StmtClass.CXXTemporaryObjectExprClass:
 ret = new CXXTemporaryObjectExprStmt(parent);
 break;

case StmtClass.CXXDefaultArgExprClass:
 ret = new CXXDefaultArgExprStmt(parent);
 break;

case StmtClass.CXXDefaultInitExprClass:
 ret = new CXXDefaultInitExprStmt(parent);
 break;

case StmtClass.CXXDeleteExprClass:
 ret = new CXXDeleteExprStmt(parent);
 break;

case StmtClass.CXXDependentScopeMemberExprClass:
 ret = new CXXDependentScopeMemberExprStmt(parent);
 break;

case StmtClass.CXXFoldExprClass:
 ret = new CXXFoldExprStmt(parent);
 break;

case StmtClass.CXXInheritedCtorInitExprClass:
 ret = new CXXInheritedCtorInitExprStmt(parent);
 break;

case StmtClass.CXXNewExprClass:
 ret = new CXXNewExprStmt(parent);
 break;

case StmtClass.CXXNoexceptExprClass:
 ret = new CXXNoexceptExprStmt(parent);
 break;

case StmtClass.CXXNullPtrLiteralExprClass:
 ret = new CXXNullPtrLiteralExprStmt(parent);
 break;

case StmtClass.CXXPseudoDestructorExprClass:
 ret = new CXXPseudoDestructorExprStmt(parent);
 break;

case StmtClass.CXXScalarValueInitExprClass:
 ret = new CXXScalarValueInitExprStmt(parent);
 break;

case StmtClass.CXXStdInitializerListExprClass:
 ret = new CXXStdInitializerListExprStmt(parent);
 break;

case StmtClass.CXXThisExprClass:
 ret = new CXXThisExprStmt(parent);
 break;

case StmtClass.CXXThrowExprClass:
 ret = new CXXThrowExprStmt(parent);
 break;

case StmtClass.CXXTypeidExprClass:
 ret = new CXXTypeidExprStmt(parent);
 break;

case StmtClass.CXXUnresolvedConstructExprClass:
 ret = new CXXUnresolvedConstructExprStmt(parent);
 break;

case StmtClass.CXXUuidofExprClass:
 ret = new CXXUuidofExprStmt(parent);
 break;

case StmtClass.CallExprClass:
 ret = new CallExprStmt(parent);
 break;

case StmtClass.CUDAKernelCallExprClass:
 ret = new CUDAKernelCallExprStmt(parent);
 break;

case StmtClass.CXXMemberCallExprClass:
 ret = new CXXMemberCallExprStmt(parent);
 break;

case StmtClass.CXXOperatorCallExprClass:
 ret = new CXXOperatorCallExprStmt(parent);
 break;

case StmtClass.UserDefinedLiteralClass:
 ret = new UserDefinedLiteralStmt(parent);
 break;

case StmtClass.CStyleCastExprClass:
 ret = new CStyleCastExprStmt(parent);
 break;

case StmtClass.CXXFunctionalCastExprClass:
 ret = new CXXFunctionalCastExprStmt(parent);
 break;

case StmtClass.CXXConstCastExprClass:
 ret = new CXXConstCastExprStmt(parent);
 break;

case StmtClass.CXXDynamicCastExprClass:
 ret = new CXXDynamicCastExprStmt(parent);
 break;

case StmtClass.CXXReinterpretCastExprClass:
 ret = new CXXReinterpretCastExprStmt(parent);
 break;

case StmtClass.CXXStaticCastExprClass:
 ret = new CXXStaticCastExprStmt(parent);
 break;

case StmtClass.ObjCBridgedCastExprClass:
 ret = new ObjCBridgedCastExprStmt(parent);
 break;

case StmtClass.ImplicitCastExprClass:
 ret = new ImplicitCastExprStmt(parent);
 break;

case StmtClass.CharacterLiteralClass:
 ret = new CharacterLiteralStmt(parent);
 break;

case StmtClass.ChooseExprClass:
 ret = new ChooseExprStmt(parent);
 break;

case StmtClass.CompoundLiteralExprClass:
 ret = new CompoundLiteralExprStmt(parent);
 break;

case StmtClass.ConvertVectorExprClass:
 ret = new ConvertVectorExprStmt(parent);
 break;

case StmtClass.CoawaitExprClass:
 ret = new CoawaitExprStmt(parent);
 break;

case StmtClass.CoyieldExprClass:
 ret = new CoyieldExprStmt(parent);
 break;

case StmtClass.DeclRefExprClass:
 ret = new DeclRefExprStmt(parent);
 break;

case StmtClass.DependentCoawaitExprClass:
 ret = new DependentCoawaitExprStmt(parent);
 break;

case StmtClass.DependentScopeDeclRefExprClass:
 ret = new DependentScopeDeclRefExprStmt(parent);
 break;

case StmtClass.DesignatedInitExprClass:
 ret = new DesignatedInitExprStmt(parent);
 break;

case StmtClass.DesignatedInitUpdateExprClass:
 ret = new DesignatedInitUpdateExprStmt(parent);
 break;

case StmtClass.ExprWithCleanupsClass:
 ret = new ExprWithCleanupsStmt(parent);
 break;

case StmtClass.ExpressionTraitExprClass:
 ret = new ExpressionTraitExprStmt(parent);
 break;

case StmtClass.ExtVectorElementExprClass:
 ret = new ExtVectorElementExprStmt(parent);
 break;

case StmtClass.FloatingLiteralClass:
 ret = new FloatingLiteralStmt(parent);
 break;

case StmtClass.FunctionParmPackExprClass:
 ret = new FunctionParmPackExprStmt(parent);
 break;

case StmtClass.GNUNullExprClass:
 ret = new GNUNullExprStmt(parent);
 break;

case StmtClass.GenericSelectionExprClass:
 ret = new GenericSelectionExprStmt(parent);
 break;

case StmtClass.ImaginaryLiteralClass:
 ret = new ImaginaryLiteralStmt(parent);
 break;

case StmtClass.ImplicitValueInitExprClass:
 ret = new ImplicitValueInitExprStmt(parent);
 break;

case StmtClass.InitListExprClass:
 ret = new InitListExprStmt(parent);
 break;

case StmtClass.IntegerLiteralClass:
 ret = new IntegerLiteralStmt(parent);
 break;

case StmtClass.LambdaExprClass:
 ret = new LambdaExprStmt(parent);
 break;

case StmtClass.MSPropertyRefExprClass:
 ret = new MSPropertyRefExprStmt(parent);
 break;

case StmtClass.MSPropertySubscriptExprClass:
 ret = new MSPropertySubscriptExprStmt(parent);
 break;

case StmtClass.MaterializeTemporaryExprClass:
 ret = new MaterializeTemporaryExprStmt(parent);
 break;

case StmtClass.MemberExprClass:
 ret = new MemberExprStmt(parent);
 break;

case StmtClass.NoInitExprClass:
 ret = new NoInitExprStmt(parent);
 break;

case StmtClass.OMPArraySectionExprClass:
 ret = new OMPArraySectionExprStmt(parent);
 break;

case StmtClass.ObjCArrayLiteralClass:
 ret = new ObjCArrayLiteralStmt(parent);
 break;

case StmtClass.ObjCAvailabilityCheckExprClass:
 ret = new ObjCAvailabilityCheckExprStmt(parent);
 break;

case StmtClass.ObjCBoolLiteralExprClass:
 ret = new ObjCBoolLiteralExprStmt(parent);
 break;

case StmtClass.ObjCBoxedExprClass:
 ret = new ObjCBoxedExprStmt(parent);
 break;

case StmtClass.ObjCDictionaryLiteralClass:
 ret = new ObjCDictionaryLiteralStmt(parent);
 break;

case StmtClass.ObjCEncodeExprClass:
 ret = new ObjCEncodeExprStmt(parent);
 break;

case StmtClass.ObjCIndirectCopyRestoreExprClass:
 ret = new ObjCIndirectCopyRestoreExprStmt(parent);
 break;

case StmtClass.ObjCIsaExprClass:
 ret = new ObjCIsaExprStmt(parent);
 break;

case StmtClass.ObjCIvarRefExprClass:
 ret = new ObjCIvarRefExprStmt(parent);
 break;

case StmtClass.ObjCMessageExprClass:
 ret = new ObjCMessageExprStmt(parent);
 break;

case StmtClass.ObjCPropertyRefExprClass:
 ret = new ObjCPropertyRefExprStmt(parent);
 break;

case StmtClass.ObjCProtocolExprClass:
 ret = new ObjCProtocolExprStmt(parent);
 break;

case StmtClass.ObjCSelectorExprClass:
 ret = new ObjCSelectorExprStmt(parent);
 break;

case StmtClass.ObjCStringLiteralClass:
 ret = new ObjCStringLiteralStmt(parent);
 break;

case StmtClass.ObjCSubscriptRefExprClass:
 ret = new ObjCSubscriptRefExprStmt(parent);
 break;

case StmtClass.OffsetOfExprClass:
 ret = new OffsetOfExprStmt(parent);
 break;

case StmtClass.OpaqueValueExprClass:
 ret = new OpaqueValueExprStmt(parent);
 break;

case StmtClass.UnresolvedLookupExprClass:
 ret = new UnresolvedLookupExprStmt(parent);
 break;

case StmtClass.UnresolvedMemberExprClass:
 ret = new UnresolvedMemberExprStmt(parent);
 break;

case StmtClass.PackExpansionExprClass:
 ret = new PackExpansionExprStmt(parent);
 break;

case StmtClass.ParenExprClass:
 ret = new ParenExprStmt(parent);
 break;

case StmtClass.ParenListExprClass:
 ret = new ParenListExprStmt(parent);
 break;

case StmtClass.PredefinedExprClass:
 ret = new PredefinedExprStmt(parent);
 break;

case StmtClass.PseudoObjectExprClass:
 ret = new PseudoObjectExprStmt(parent);
 break;

case StmtClass.ShuffleVectorExprClass:
 ret = new ShuffleVectorExprStmt(parent);
 break;

case StmtClass.SizeOfPackExprClass:
 ret = new SizeOfPackExprStmt(parent);
 break;

case StmtClass.StmtExprClass:
 ret = new StmtExprStmt(parent);
 break;

case StmtClass.StringLiteralClass:
 ret = new StringLiteralStmt(parent);
 break;

case StmtClass.SubstNonTypeTemplateParmExprClass:
 ret = new SubstNonTypeTemplateParmExprStmt(parent);
 break;

case StmtClass.SubstNonTypeTemplateParmPackExprClass:
 ret = new SubstNonTypeTemplateParmPackExprStmt(parent);
 break;

case StmtClass.TypeTraitExprClass:
 ret = new TypeTraitExprStmt(parent);
 break;

case StmtClass.TypoExprClass:
 ret = new TypoExprStmt(parent);
 break;

case StmtClass.UnaryExprOrTypeTraitExprClass:
 ret = new UnaryExprOrTypeTraitExprStmt(parent);
 break;

case StmtClass.UnaryOperatorClass:
 ret = new UnaryOperatorStmt(parent);
 break;

case StmtClass.VAArgExprClass:
 ret = new VAArgExprStmt(parent);
 break;

case StmtClass.ForStmtClass:
 ret = new ForStmtStmt(parent);
 break;

case StmtClass.GotoStmtClass:
 ret = new GotoStmtStmt(parent);
 break;

case StmtClass.IfStmtClass:
 ret = new IfStmtStmt(parent);
 break;

case StmtClass.IndirectGotoStmtClass:
 ret = new IndirectGotoStmtStmt(parent);
 break;

case StmtClass.LabelStmtClass:
 ret = new LabelStmtStmt(parent);
 break;

case StmtClass.MSDependentExistsStmtClass:
 ret = new MSDependentExistsStmtStmt(parent);
 break;

case StmtClass.NullStmtClass:
 ret = new NullStmtStmt(parent);
 break;

case StmtClass.OMPAtomicDirectiveClass:
 ret = new OMPAtomicDirectiveStmt(parent);
 break;

case StmtClass.OMPBarrierDirectiveClass:
 ret = new OMPBarrierDirectiveStmt(parent);
 break;

case StmtClass.OMPCancelDirectiveClass:
 ret = new OMPCancelDirectiveStmt(parent);
 break;

case StmtClass.OMPCancellationPointDirectiveClass:
 ret = new OMPCancellationPointDirectiveStmt(parent);
 break;

case StmtClass.OMPCriticalDirectiveClass:
 ret = new OMPCriticalDirectiveStmt(parent);
 break;

case StmtClass.OMPFlushDirectiveClass:
 ret = new OMPFlushDirectiveStmt(parent);
 break;

case StmtClass.OMPDistributeDirectiveClass:
 ret = new OMPDistributeDirectiveStmt(parent);
 break;

case StmtClass.OMPDistributeParallelForDirectiveClass:
 ret = new OMPDistributeParallelForDirectiveStmt(parent);
 break;

case StmtClass.OMPDistributeParallelForSimdDirectiveClass:
 ret = new OMPDistributeParallelForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPDistributeSimdDirectiveClass:
 ret = new OMPDistributeSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPForDirectiveClass:
 ret = new OMPForDirectiveStmt(parent);
 break;

case StmtClass.OMPForSimdDirectiveClass:
 ret = new OMPForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPParallelForDirectiveClass:
 ret = new OMPParallelForDirectiveStmt(parent);
 break;

case StmtClass.OMPParallelForSimdDirectiveClass:
 ret = new OMPParallelForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPSimdDirectiveClass:
 ret = new OMPSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetParallelForSimdDirectiveClass:
 ret = new OMPTargetParallelForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetSimdDirectiveClass:
 ret = new OMPTargetSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetTeamsDistributeDirectiveClass:
 ret = new OMPTargetTeamsDistributeDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetTeamsDistributeParallelForDirectiveClass:
 ret = new OMPTargetTeamsDistributeParallelForDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetTeamsDistributeParallelForSimdDirectiveClass:
 ret = new OMPTargetTeamsDistributeParallelForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetTeamsDistributeSimdDirectiveClass:
 ret = new OMPTargetTeamsDistributeSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskLoopDirectiveClass:
 ret = new OMPTaskLoopDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskLoopSimdDirectiveClass:
 ret = new OMPTaskLoopSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTeamsDistributeDirectiveClass:
 ret = new OMPTeamsDistributeDirectiveStmt(parent);
 break;

case StmtClass.OMPTeamsDistributeParallelForDirectiveClass:
 ret = new OMPTeamsDistributeParallelForDirectiveStmt(parent);
 break;

case StmtClass.OMPTeamsDistributeParallelForSimdDirectiveClass:
 ret = new OMPTeamsDistributeParallelForSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPTeamsDistributeSimdDirectiveClass:
 ret = new OMPTeamsDistributeSimdDirectiveStmt(parent);
 break;

case StmtClass.OMPMasterDirectiveClass:
 ret = new OMPMasterDirectiveStmt(parent);
 break;

case StmtClass.OMPOrderedDirectiveClass:
 ret = new OMPOrderedDirectiveStmt(parent);
 break;

case StmtClass.OMPParallelDirectiveClass:
 ret = new OMPParallelDirectiveStmt(parent);
 break;

case StmtClass.OMPParallelSectionsDirectiveClass:
 ret = new OMPParallelSectionsDirectiveStmt(parent);
 break;

case StmtClass.OMPSectionDirectiveClass:
 ret = new OMPSectionDirectiveStmt(parent);
 break;

case StmtClass.OMPSectionsDirectiveClass:
 ret = new OMPSectionsDirectiveStmt(parent);
 break;

case StmtClass.OMPSingleDirectiveClass:
 ret = new OMPSingleDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetDataDirectiveClass:
 ret = new OMPTargetDataDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetDirectiveClass:
 ret = new OMPTargetDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetEnterDataDirectiveClass:
 ret = new OMPTargetEnterDataDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetExitDataDirectiveClass:
 ret = new OMPTargetExitDataDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetParallelDirectiveClass:
 ret = new OMPTargetParallelDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetParallelForDirectiveClass:
 ret = new OMPTargetParallelForDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetTeamsDirectiveClass:
 ret = new OMPTargetTeamsDirectiveStmt(parent);
 break;

case StmtClass.OMPTargetUpdateDirectiveClass:
 ret = new OMPTargetUpdateDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskDirectiveClass:
 ret = new OMPTaskDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskgroupDirectiveClass:
 ret = new OMPTaskgroupDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskwaitDirectiveClass:
 ret = new OMPTaskwaitDirectiveStmt(parent);
 break;

case StmtClass.OMPTaskyieldDirectiveClass:
 ret = new OMPTaskyieldDirectiveStmt(parent);
 break;

case StmtClass.OMPTeamsDirectiveClass:
 ret = new OMPTeamsDirectiveStmt(parent);
 break;

case StmtClass.ObjCAtCatchStmtClass:
 ret = new ObjCAtCatchStmtStmt(parent);
 break;

case StmtClass.ObjCAtFinallyStmtClass:
 ret = new ObjCAtFinallyStmtStmt(parent);
 break;

case StmtClass.ObjCAtSynchronizedStmtClass:
 ret = new ObjCAtSynchronizedStmtStmt(parent);
 break;

case StmtClass.ObjCAtThrowStmtClass:
 ret = new ObjCAtThrowStmtStmt(parent);
 break;

case StmtClass.ObjCAtTryStmtClass:
 ret = new ObjCAtTryStmtStmt(parent);
 break;

case StmtClass.ObjCAutoreleasePoolStmtClass:
 ret = new ObjCAutoreleasePoolStmtStmt(parent);
 break;

case StmtClass.ObjCForCollectionStmtClass:
 ret = new ObjCForCollectionStmtStmt(parent);
 break;

case StmtClass.ReturnStmtClass:
 ret = new ReturnStmtStmt(parent);
 break;

case StmtClass.SEHExceptStmtClass:
 ret = new SEHExceptStmtStmt(parent);
 break;

case StmtClass.SEHFinallyStmtClass:
 ret = new SEHFinallyStmtStmt(parent);
 break;

case StmtClass.SEHLeaveStmtClass:
 ret = new SEHLeaveStmtStmt(parent);
 break;

case StmtClass.SEHTryStmtClass:
 ret = new SEHTryStmtStmt(parent);
 break;

case StmtClass.CaseStmtClass:
 ret = new CaseStmtStmt(parent);
 break;

case StmtClass.DefaultStmtClass:
 ret = new DefaultStmtStmt(parent);
 break;

case StmtClass.SwitchStmtClass:
 ret = new SwitchStmtStmt(parent);
 break;

case StmtClass.WhileStmtClass:
 ret = new WhileStmtStmt(parent);
 break;

			default: throw new NotImplementedException();

			}

			if (parent == null)
				Stmts.Add(ret);

			return ret;

		}

public class GCCAsmStmtStmt : AsmStmt {
 internal GCCAsmStmtStmt(IStmt parent) :
 base(StmtClass.GCCAsmStmt, parent) {
 }
 }

public class MSAsmStmtStmt : AsmStmt {
 internal MSAsmStmtStmt(IStmt parent) :
 base(StmtClass.MSAsmStmt, parent) {
 }
 }

public class AttributedStmtStmt : Stmt {
 internal AttributedStmtStmt(IStmt parent) :
 base(StmtClass.AttributedStmt, parent) {
 }
 }

public class BreakStmtStmt : Stmt {
 internal BreakStmtStmt(IStmt parent) :
 base(StmtClass.BreakStmt, parent) {
 }
 }

public class CXXCatchStmtStmt : Stmt {
 internal CXXCatchStmtStmt(IStmt parent) :
 base(StmtClass.CXXCatchStmt, parent) {
 }
 }

public class CXXForRangeStmtStmt : Stmt {
 internal CXXForRangeStmtStmt(IStmt parent) :
 base(StmtClass.CXXForRangeStmt, parent) {
 }
 }

public class CXXTryStmtStmt : Stmt {
 internal CXXTryStmtStmt(IStmt parent) :
 base(StmtClass.CXXTryStmt, parent) {
 }
 }

public class CapturedStmtStmt : Stmt {
 internal CapturedStmtStmt(IStmt parent) :
 base(StmtClass.CapturedStmt, parent) {
 }
 }

public class CompoundStmtStmt : Stmt {
 internal CompoundStmtStmt(IStmt parent) :
 base(StmtClass.CompoundStmt, parent) {
 }
 }

public class ContinueStmtStmt : Stmt {
 internal ContinueStmtStmt(IStmt parent) :
 base(StmtClass.ContinueStmt, parent) {
 }
 }

public class CoreturnStmtStmt : Stmt {
 internal CoreturnStmtStmt(IStmt parent) :
 base(StmtClass.CoreturnStmt, parent) {
 }
 }

public class CoroutineBodyStmtStmt : Stmt {
 internal CoroutineBodyStmtStmt(IStmt parent) :
 base(StmtClass.CoroutineBodyStmt, parent) {
 }
 }

public class DeclStmtStmt : Stmt {
 internal DeclStmtStmt(IStmt parent) :
 base(StmtClass.DeclStmt, parent) {
 }
 }

public class DoStmtStmt : Stmt {
 internal DoStmtStmt(IStmt parent) :
 base(StmtClass.DoStmt, parent) {
 }
 }

public class BinaryConditionalOperatorStmt : AbstractConditionalOperator {
 internal BinaryConditionalOperatorStmt(IStmt parent) :
 base(StmtClass.BinaryConditionalOperator, parent) {
 }
 }

public class ConditionalOperatorStmt : AbstractConditionalOperator {
 internal ConditionalOperatorStmt(IStmt parent) :
 base(StmtClass.ConditionalOperator, parent) {
 }
 }

public class AddrLabelExprStmt : Expr {
 internal AddrLabelExprStmt(IStmt parent) :
 base(StmtClass.AddrLabelExpr, parent) {
 }
 }

public class ArrayInitIndexExprStmt : Expr {
 internal ArrayInitIndexExprStmt(IStmt parent) :
 base(StmtClass.ArrayInitIndexExpr, parent) {
 }
 }

public class ArrayInitLoopExprStmt : Expr {
 internal ArrayInitLoopExprStmt(IStmt parent) :
 base(StmtClass.ArrayInitLoopExpr, parent) {
 }
 }

public class ArraySubscriptExprStmt : Expr {
 internal ArraySubscriptExprStmt(IStmt parent) :
 base(StmtClass.ArraySubscriptExpr, parent) {
 }
 }

public class ArrayTypeTraitExprStmt : Expr {
 internal ArrayTypeTraitExprStmt(IStmt parent) :
 base(StmtClass.ArrayTypeTraitExpr, parent) {
 }
 }

public class AsTypeExprStmt : Expr {
 internal AsTypeExprStmt(IStmt parent) :
 base(StmtClass.AsTypeExpr, parent) {
 }
 }

public class AtomicExprStmt : Expr {
 internal AtomicExprStmt(IStmt parent) :
 base(StmtClass.AtomicExpr, parent) {
 }
 }

public class BinaryOperatorStmt : Expr {
 internal BinaryOperatorStmt(IStmt parent) :
 base(StmtClass.BinaryOperator, parent) {
 }
 }

public class CompoundAssignOperatorStmt : BinaryOperator {
 internal CompoundAssignOperatorStmt(IStmt parent) :
 base(StmtClass.CompoundAssignOperator, parent) {
 }
 }

public class BlockExprStmt : Expr {
 internal BlockExprStmt(IStmt parent) :
 base(StmtClass.BlockExpr, parent) {
 }
 }

public class CXXBindTemporaryExprStmt : Expr {
 internal CXXBindTemporaryExprStmt(IStmt parent) :
 base(StmtClass.CXXBindTemporaryExpr, parent) {
 }
 }

public class CXXBoolLiteralExprStmt : Expr {
 internal CXXBoolLiteralExprStmt(IStmt parent) :
 base(StmtClass.CXXBoolLiteralExpr, parent) {
 }
 }

public class CXXConstructExprStmt : Expr {
 internal CXXConstructExprStmt(IStmt parent) :
 base(StmtClass.CXXConstructExpr, parent) {
 }
 }

public class CXXTemporaryObjectExprStmt : CXXConstructExpr {
 internal CXXTemporaryObjectExprStmt(IStmt parent) :
 base(StmtClass.CXXTemporaryObjectExpr, parent) {
 }
 }

public class CXXDefaultArgExprStmt : Expr {
 internal CXXDefaultArgExprStmt(IStmt parent) :
 base(StmtClass.CXXDefaultArgExpr, parent) {
 }
 }

public class CXXDefaultInitExprStmt : Expr {
 internal CXXDefaultInitExprStmt(IStmt parent) :
 base(StmtClass.CXXDefaultInitExpr, parent) {
 }
 }

public class CXXDeleteExprStmt : Expr {
 internal CXXDeleteExprStmt(IStmt parent) :
 base(StmtClass.CXXDeleteExpr, parent) {
 }
 }

public class CXXDependentScopeMemberExprStmt : Expr {
 internal CXXDependentScopeMemberExprStmt(IStmt parent) :
 base(StmtClass.CXXDependentScopeMemberExpr, parent) {
 }
 }

public class CXXFoldExprStmt : Expr {
 internal CXXFoldExprStmt(IStmt parent) :
 base(StmtClass.CXXFoldExpr, parent) {
 }
 }

public class CXXInheritedCtorInitExprStmt : Expr {
 internal CXXInheritedCtorInitExprStmt(IStmt parent) :
 base(StmtClass.CXXInheritedCtorInitExpr, parent) {
 }
 }

public class CXXNewExprStmt : Expr {
 internal CXXNewExprStmt(IStmt parent) :
 base(StmtClass.CXXNewExpr, parent) {
 }
 }

public class CXXNoexceptExprStmt : Expr {
 internal CXXNoexceptExprStmt(IStmt parent) :
 base(StmtClass.CXXNoexceptExpr, parent) {
 }
 }

public class CXXNullPtrLiteralExprStmt : Expr {
 internal CXXNullPtrLiteralExprStmt(IStmt parent) :
 base(StmtClass.CXXNullPtrLiteralExpr, parent) {
 }
 }

public class CXXPseudoDestructorExprStmt : Expr {
 internal CXXPseudoDestructorExprStmt(IStmt parent) :
 base(StmtClass.CXXPseudoDestructorExpr, parent) {
 }
 }

public class CXXScalarValueInitExprStmt : Expr {
 internal CXXScalarValueInitExprStmt(IStmt parent) :
 base(StmtClass.CXXScalarValueInitExpr, parent) {
 }
 }

public class CXXStdInitializerListExprStmt : Expr {
 internal CXXStdInitializerListExprStmt(IStmt parent) :
 base(StmtClass.CXXStdInitializerListExpr, parent) {
 }
 }

public class CXXThisExprStmt : Expr {
 internal CXXThisExprStmt(IStmt parent) :
 base(StmtClass.CXXThisExpr, parent) {
 }
 }

public class CXXThrowExprStmt : Expr {
 internal CXXThrowExprStmt(IStmt parent) :
 base(StmtClass.CXXThrowExpr, parent) {
 }
 }

public class CXXTypeidExprStmt : Expr {
 internal CXXTypeidExprStmt(IStmt parent) :
 base(StmtClass.CXXTypeidExpr, parent) {
 }
 }

public class CXXUnresolvedConstructExprStmt : Expr {
 internal CXXUnresolvedConstructExprStmt(IStmt parent) :
 base(StmtClass.CXXUnresolvedConstructExpr, parent) {
 }
 }

public class CXXUuidofExprStmt : Expr {
 internal CXXUuidofExprStmt(IStmt parent) :
 base(StmtClass.CXXUuidofExpr, parent) {
 }
 }

public class CallExprStmt : Expr {
 internal CallExprStmt(IStmt parent) :
 base(StmtClass.CallExpr, parent) {
 }
 }

public class CUDAKernelCallExprStmt : CallExpr {
 internal CUDAKernelCallExprStmt(IStmt parent) :
 base(StmtClass.CUDAKernelCallExpr, parent) {
 }
 }

public class CXXMemberCallExprStmt : CallExpr {
 internal CXXMemberCallExprStmt(IStmt parent) :
 base(StmtClass.CXXMemberCallExpr, parent) {
 }
 }

public class CXXOperatorCallExprStmt : CallExpr {
 internal CXXOperatorCallExprStmt(IStmt parent) :
 base(StmtClass.CXXOperatorCallExpr, parent) {
 }
 }

public class UserDefinedLiteralStmt : CallExpr {
 internal UserDefinedLiteralStmt(IStmt parent) :
 base(StmtClass.UserDefinedLiteral, parent) {
 }
 }

public class CStyleCastExprStmt : ExplicitCastExpr {
 internal CStyleCastExprStmt(IStmt parent) :
 base(StmtClass.CStyleCastExpr, parent) {
 }
 }

public class CXXFunctionalCastExprStmt : ExplicitCastExpr {
 internal CXXFunctionalCastExprStmt(IStmt parent) :
 base(StmtClass.CXXFunctionalCastExpr, parent) {
 }
 }

public class CXXConstCastExprStmt : CXXNamedCastExpr {
 internal CXXConstCastExprStmt(IStmt parent) :
 base(StmtClass.CXXConstCastExpr, parent) {
 }
 }

public class CXXDynamicCastExprStmt : CXXNamedCastExpr {
 internal CXXDynamicCastExprStmt(IStmt parent) :
 base(StmtClass.CXXDynamicCastExpr, parent) {
 }
 }

public class CXXReinterpretCastExprStmt : CXXNamedCastExpr {
 internal CXXReinterpretCastExprStmt(IStmt parent) :
 base(StmtClass.CXXReinterpretCastExpr, parent) {
 }
 }

public class CXXStaticCastExprStmt : CXXNamedCastExpr {
 internal CXXStaticCastExprStmt(IStmt parent) :
 base(StmtClass.CXXStaticCastExpr, parent) {
 }
 }

public class ObjCBridgedCastExprStmt : ExplicitCastExpr {
 internal ObjCBridgedCastExprStmt(IStmt parent) :
 base(StmtClass.ObjCBridgedCastExpr, parent) {
 }
 }

public class ImplicitCastExprStmt : CastExpr {
 internal ImplicitCastExprStmt(IStmt parent) :
 base(StmtClass.ImplicitCastExpr, parent) {
 }
 }

public class CharacterLiteralStmt : Expr {
 internal CharacterLiteralStmt(IStmt parent) :
 base(StmtClass.CharacterLiteral, parent) {
 }
 }

public class ChooseExprStmt : Expr {
 internal ChooseExprStmt(IStmt parent) :
 base(StmtClass.ChooseExpr, parent) {
 }
 }

public class CompoundLiteralExprStmt : Expr {
 internal CompoundLiteralExprStmt(IStmt parent) :
 base(StmtClass.CompoundLiteralExpr, parent) {
 }
 }

public class ConvertVectorExprStmt : Expr {
 internal ConvertVectorExprStmt(IStmt parent) :
 base(StmtClass.ConvertVectorExpr, parent) {
 }
 }

public class CoawaitExprStmt : CoroutineSuspendExpr {
 internal CoawaitExprStmt(IStmt parent) :
 base(StmtClass.CoawaitExpr, parent) {
 }
 }

public class CoyieldExprStmt : CoroutineSuspendExpr {
 internal CoyieldExprStmt(IStmt parent) :
 base(StmtClass.CoyieldExpr, parent) {
 }
 }

public class DeclRefExprStmt : Expr {
 internal DeclRefExprStmt(IStmt parent) :
 base(StmtClass.DeclRefExpr, parent) {
 }
 }

public class DependentCoawaitExprStmt : Expr {
 internal DependentCoawaitExprStmt(IStmt parent) :
 base(StmtClass.DependentCoawaitExpr, parent) {
 }
 }

public class DependentScopeDeclRefExprStmt : Expr {
 internal DependentScopeDeclRefExprStmt(IStmt parent) :
 base(StmtClass.DependentScopeDeclRefExpr, parent) {
 }
 }

public class DesignatedInitExprStmt : Expr {
 internal DesignatedInitExprStmt(IStmt parent) :
 base(StmtClass.DesignatedInitExpr, parent) {
 }
 }

public class DesignatedInitUpdateExprStmt : Expr {
 internal DesignatedInitUpdateExprStmt(IStmt parent) :
 base(StmtClass.DesignatedInitUpdateExpr, parent) {
 }
 }

public class ExprWithCleanupsStmt : Expr {
 internal ExprWithCleanupsStmt(IStmt parent) :
 base(StmtClass.ExprWithCleanups, parent) {
 }
 }

public class ExpressionTraitExprStmt : Expr {
 internal ExpressionTraitExprStmt(IStmt parent) :
 base(StmtClass.ExpressionTraitExpr, parent) {
 }
 }

public class ExtVectorElementExprStmt : Expr {
 internal ExtVectorElementExprStmt(IStmt parent) :
 base(StmtClass.ExtVectorElementExpr, parent) {
 }
 }

public class FloatingLiteralStmt : Expr {
 internal FloatingLiteralStmt(IStmt parent) :
 base(StmtClass.FloatingLiteral, parent) {
 }
 }

public class FunctionParmPackExprStmt : Expr {
 internal FunctionParmPackExprStmt(IStmt parent) :
 base(StmtClass.FunctionParmPackExpr, parent) {
 }
 }

public class GNUNullExprStmt : Expr {
 internal GNUNullExprStmt(IStmt parent) :
 base(StmtClass.GNUNullExpr, parent) {
 }
 }

public class GenericSelectionExprStmt : Expr {
 internal GenericSelectionExprStmt(IStmt parent) :
 base(StmtClass.GenericSelectionExpr, parent) {
 }
 }

public class ImaginaryLiteralStmt : Expr {
 internal ImaginaryLiteralStmt(IStmt parent) :
 base(StmtClass.ImaginaryLiteral, parent) {
 }
 }

public class ImplicitValueInitExprStmt : Expr {
 internal ImplicitValueInitExprStmt(IStmt parent) :
 base(StmtClass.ImplicitValueInitExpr, parent) {
 }
 }

public class InitListExprStmt : Expr {
 internal InitListExprStmt(IStmt parent) :
 base(StmtClass.InitListExpr, parent) {
 }
 }

public class IntegerLiteralStmt : Expr {
 internal IntegerLiteralStmt(IStmt parent) :
 base(StmtClass.IntegerLiteral, parent) {
 }
 }

public class LambdaExprStmt : Expr {
 internal LambdaExprStmt(IStmt parent) :
 base(StmtClass.LambdaExpr, parent) {
 }
 }

public class MSPropertyRefExprStmt : Expr {
 internal MSPropertyRefExprStmt(IStmt parent) :
 base(StmtClass.MSPropertyRefExpr, parent) {
 }
 }

public class MSPropertySubscriptExprStmt : Expr {
 internal MSPropertySubscriptExprStmt(IStmt parent) :
 base(StmtClass.MSPropertySubscriptExpr, parent) {
 }
 }

public class MaterializeTemporaryExprStmt : Expr {
 internal MaterializeTemporaryExprStmt(IStmt parent) :
 base(StmtClass.MaterializeTemporaryExpr, parent) {
 }
 }

public class MemberExprStmt : Expr {
 internal MemberExprStmt(IStmt parent) :
 base(StmtClass.MemberExpr, parent) {
 }
 }

public class NoInitExprStmt : Expr {
 internal NoInitExprStmt(IStmt parent) :
 base(StmtClass.NoInitExpr, parent) {
 }
 }

public class OMPArraySectionExprStmt : Expr {
 internal OMPArraySectionExprStmt(IStmt parent) :
 base(StmtClass.OMPArraySectionExpr, parent) {
 }
 }

public class ObjCArrayLiteralStmt : Expr {
 internal ObjCArrayLiteralStmt(IStmt parent) :
 base(StmtClass.ObjCArrayLiteral, parent) {
 }
 }

public class ObjCAvailabilityCheckExprStmt : Expr {
 internal ObjCAvailabilityCheckExprStmt(IStmt parent) :
 base(StmtClass.ObjCAvailabilityCheckExpr, parent) {
 }
 }

public class ObjCBoolLiteralExprStmt : Expr {
 internal ObjCBoolLiteralExprStmt(IStmt parent) :
 base(StmtClass.ObjCBoolLiteralExpr, parent) {
 }
 }

public class ObjCBoxedExprStmt : Expr {
 internal ObjCBoxedExprStmt(IStmt parent) :
 base(StmtClass.ObjCBoxedExpr, parent) {
 }
 }

public class ObjCDictionaryLiteralStmt : Expr {
 internal ObjCDictionaryLiteralStmt(IStmt parent) :
 base(StmtClass.ObjCDictionaryLiteral, parent) {
 }
 }

public class ObjCEncodeExprStmt : Expr {
 internal ObjCEncodeExprStmt(IStmt parent) :
 base(StmtClass.ObjCEncodeExpr, parent) {
 }
 }

public class ObjCIndirectCopyRestoreExprStmt : Expr {
 internal ObjCIndirectCopyRestoreExprStmt(IStmt parent) :
 base(StmtClass.ObjCIndirectCopyRestoreExpr, parent) {
 }
 }

public class ObjCIsaExprStmt : Expr {
 internal ObjCIsaExprStmt(IStmt parent) :
 base(StmtClass.ObjCIsaExpr, parent) {
 }
 }

public class ObjCIvarRefExprStmt : Expr {
 internal ObjCIvarRefExprStmt(IStmt parent) :
 base(StmtClass.ObjCIvarRefExpr, parent) {
 }
 }

public class ObjCMessageExprStmt : Expr {
 internal ObjCMessageExprStmt(IStmt parent) :
 base(StmtClass.ObjCMessageExpr, parent) {
 }
 }

public class ObjCPropertyRefExprStmt : Expr {
 internal ObjCPropertyRefExprStmt(IStmt parent) :
 base(StmtClass.ObjCPropertyRefExpr, parent) {
 }
 }

public class ObjCProtocolExprStmt : Expr {
 internal ObjCProtocolExprStmt(IStmt parent) :
 base(StmtClass.ObjCProtocolExpr, parent) {
 }
 }

public class ObjCSelectorExprStmt : Expr {
 internal ObjCSelectorExprStmt(IStmt parent) :
 base(StmtClass.ObjCSelectorExpr, parent) {
 }
 }

public class ObjCStringLiteralStmt : Expr {
 internal ObjCStringLiteralStmt(IStmt parent) :
 base(StmtClass.ObjCStringLiteral, parent) {
 }
 }

public class ObjCSubscriptRefExprStmt : Expr {
 internal ObjCSubscriptRefExprStmt(IStmt parent) :
 base(StmtClass.ObjCSubscriptRefExpr, parent) {
 }
 }

public class OffsetOfExprStmt : Expr {
 internal OffsetOfExprStmt(IStmt parent) :
 base(StmtClass.OffsetOfExpr, parent) {
 }
 }

public class OpaqueValueExprStmt : Expr {
 internal OpaqueValueExprStmt(IStmt parent) :
 base(StmtClass.OpaqueValueExpr, parent) {
 }
 }

public class UnresolvedLookupExprStmt : OverloadExpr {
 internal UnresolvedLookupExprStmt(IStmt parent) :
 base(StmtClass.UnresolvedLookupExpr, parent) {
 }
 }

public class UnresolvedMemberExprStmt : OverloadExpr {
 internal UnresolvedMemberExprStmt(IStmt parent) :
 base(StmtClass.UnresolvedMemberExpr, parent) {
 }
 }

public class PackExpansionExprStmt : Expr {
 internal PackExpansionExprStmt(IStmt parent) :
 base(StmtClass.PackExpansionExpr, parent) {
 }
 }

public class ParenExprStmt : Expr {
 internal ParenExprStmt(IStmt parent) :
 base(StmtClass.ParenExpr, parent) {
 }
 }

public class ParenListExprStmt : Expr {
 internal ParenListExprStmt(IStmt parent) :
 base(StmtClass.ParenListExpr, parent) {
 }
 }

public class PredefinedExprStmt : Expr {
 internal PredefinedExprStmt(IStmt parent) :
 base(StmtClass.PredefinedExpr, parent) {
 }
 }

public class PseudoObjectExprStmt : Expr {
 internal PseudoObjectExprStmt(IStmt parent) :
 base(StmtClass.PseudoObjectExpr, parent) {
 }
 }

public class ShuffleVectorExprStmt : Expr {
 internal ShuffleVectorExprStmt(IStmt parent) :
 base(StmtClass.ShuffleVectorExpr, parent) {
 }
 }

public class SizeOfPackExprStmt : Expr {
 internal SizeOfPackExprStmt(IStmt parent) :
 base(StmtClass.SizeOfPackExpr, parent) {
 }
 }

public class StmtExprStmt : Expr {
 internal StmtExprStmt(IStmt parent) :
 base(StmtClass.StmtExpr, parent) {
 }
 }

public class StringLiteralStmt : Expr {
 internal StringLiteralStmt(IStmt parent) :
 base(StmtClass.StringLiteral, parent) {
 }
 }

public class SubstNonTypeTemplateParmExprStmt : Expr {
 internal SubstNonTypeTemplateParmExprStmt(IStmt parent) :
 base(StmtClass.SubstNonTypeTemplateParmExpr, parent) {
 }
 }

public class SubstNonTypeTemplateParmPackExprStmt : Expr {
 internal SubstNonTypeTemplateParmPackExprStmt(IStmt parent) :
 base(StmtClass.SubstNonTypeTemplateParmPackExpr, parent) {
 }
 }

public class TypeTraitExprStmt : Expr {
 internal TypeTraitExprStmt(IStmt parent) :
 base(StmtClass.TypeTraitExpr, parent) {
 }
 }

public class TypoExprStmt : Expr {
 internal TypoExprStmt(IStmt parent) :
 base(StmtClass.TypoExpr, parent) {
 }
 }

public class UnaryExprOrTypeTraitExprStmt : Expr {
 internal UnaryExprOrTypeTraitExprStmt(IStmt parent) :
 base(StmtClass.UnaryExprOrTypeTraitExpr, parent) {
 }
 }

public class UnaryOperatorStmt : Expr {
 internal UnaryOperatorStmt(IStmt parent) :
 base(StmtClass.UnaryOperator, parent) {
 }
 }

public class VAArgExprStmt : Expr {
 internal VAArgExprStmt(IStmt parent) :
 base(StmtClass.VAArgExpr, parent) {
 }
 }

public class ForStmtStmt : Stmt {
 internal ForStmtStmt(IStmt parent) :
 base(StmtClass.ForStmt, parent) {
 }
 }

public class GotoStmtStmt : Stmt {
 internal GotoStmtStmt(IStmt parent) :
 base(StmtClass.GotoStmt, parent) {
 }
 }

public class IfStmtStmt : Stmt {
 internal IfStmtStmt(IStmt parent) :
 base(StmtClass.IfStmt, parent) {
 }
 }

public class IndirectGotoStmtStmt : Stmt {
 internal IndirectGotoStmtStmt(IStmt parent) :
 base(StmtClass.IndirectGotoStmt, parent) {
 }
 }

public class LabelStmtStmt : Stmt {
 internal LabelStmtStmt(IStmt parent) :
 base(StmtClass.LabelStmt, parent) {
 }
 }

public class MSDependentExistsStmtStmt : Stmt {
 internal MSDependentExistsStmtStmt(IStmt parent) :
 base(StmtClass.MSDependentExistsStmt, parent) {
 }
 }

public class NullStmtStmt : Stmt {
 internal NullStmtStmt(IStmt parent) :
 base(StmtClass.NullStmt, parent) {
 }
 }

public class OMPAtomicDirectiveStmt : OMPExecutableDirective {
 internal OMPAtomicDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPAtomicDirective, parent) {
 }
 }

public class OMPBarrierDirectiveStmt : OMPExecutableDirective {
 internal OMPBarrierDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPBarrierDirective, parent) {
 }
 }

public class OMPCancelDirectiveStmt : OMPExecutableDirective {
 internal OMPCancelDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPCancelDirective, parent) {
 }
 }

public class OMPCancellationPointDirectiveStmt : OMPExecutableDirective {
 internal OMPCancellationPointDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPCancellationPointDirective, parent) {
 }
 }

public class OMPCriticalDirectiveStmt : OMPExecutableDirective {
 internal OMPCriticalDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPCriticalDirective, parent) {
 }
 }

public class OMPFlushDirectiveStmt : OMPExecutableDirective {
 internal OMPFlushDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPFlushDirective, parent) {
 }
 }

public class OMPDistributeDirectiveStmt : OMPLoopDirective {
 internal OMPDistributeDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPDistributeDirective, parent) {
 }
 }

public class OMPDistributeParallelForDirectiveStmt : OMPLoopDirective {
 internal OMPDistributeParallelForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPDistributeParallelForDirective, parent) {
 }
 }

public class OMPDistributeParallelForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPDistributeParallelForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPDistributeParallelForSimdDirective, parent) {
 }
 }

public class OMPDistributeSimdDirectiveStmt : OMPLoopDirective {
 internal OMPDistributeSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPDistributeSimdDirective, parent) {
 }
 }

public class OMPForDirectiveStmt : OMPLoopDirective {
 internal OMPForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPForDirective, parent) {
 }
 }

public class OMPForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPForSimdDirective, parent) {
 }
 }

public class OMPParallelForDirectiveStmt : OMPLoopDirective {
 internal OMPParallelForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPParallelForDirective, parent) {
 }
 }

public class OMPParallelForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPParallelForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPParallelForSimdDirective, parent) {
 }
 }

public class OMPSimdDirectiveStmt : OMPLoopDirective {
 internal OMPSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPSimdDirective, parent) {
 }
 }

public class OMPTargetParallelForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTargetParallelForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetParallelForSimdDirective, parent) {
 }
 }

public class OMPTargetSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTargetSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetSimdDirective, parent) {
 }
 }

public class OMPTargetTeamsDistributeDirectiveStmt : OMPLoopDirective {
 internal OMPTargetTeamsDistributeDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetTeamsDistributeDirective, parent) {
 }
 }

public class OMPTargetTeamsDistributeParallelForDirectiveStmt : OMPLoopDirective {
 internal OMPTargetTeamsDistributeParallelForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetTeamsDistributeParallelForDirective, parent) {
 }
 }

public class OMPTargetTeamsDistributeParallelForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTargetTeamsDistributeParallelForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetTeamsDistributeParallelForSimdDirective, parent) {
 }
 }

public class OMPTargetTeamsDistributeSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTargetTeamsDistributeSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetTeamsDistributeSimdDirective, parent) {
 }
 }

public class OMPTaskLoopDirectiveStmt : OMPLoopDirective {
 internal OMPTaskLoopDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskLoopDirective, parent) {
 }
 }

public class OMPTaskLoopSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTaskLoopSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskLoopSimdDirective, parent) {
 }
 }

public class OMPTeamsDistributeDirectiveStmt : OMPLoopDirective {
 internal OMPTeamsDistributeDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTeamsDistributeDirective, parent) {
 }
 }

public class OMPTeamsDistributeParallelForDirectiveStmt : OMPLoopDirective {
 internal OMPTeamsDistributeParallelForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTeamsDistributeParallelForDirective, parent) {
 }
 }

public class OMPTeamsDistributeParallelForSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTeamsDistributeParallelForSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTeamsDistributeParallelForSimdDirective, parent) {
 }
 }

public class OMPTeamsDistributeSimdDirectiveStmt : OMPLoopDirective {
 internal OMPTeamsDistributeSimdDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTeamsDistributeSimdDirective, parent) {
 }
 }

public class OMPMasterDirectiveStmt : OMPExecutableDirective {
 internal OMPMasterDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPMasterDirective, parent) {
 }
 }

public class OMPOrderedDirectiveStmt : OMPExecutableDirective {
 internal OMPOrderedDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPOrderedDirective, parent) {
 }
 }

public class OMPParallelDirectiveStmt : OMPExecutableDirective {
 internal OMPParallelDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPParallelDirective, parent) {
 }
 }

public class OMPParallelSectionsDirectiveStmt : OMPExecutableDirective {
 internal OMPParallelSectionsDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPParallelSectionsDirective, parent) {
 }
 }

public class OMPSectionDirectiveStmt : OMPExecutableDirective {
 internal OMPSectionDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPSectionDirective, parent) {
 }
 }

public class OMPSectionsDirectiveStmt : OMPExecutableDirective {
 internal OMPSectionsDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPSectionsDirective, parent) {
 }
 }

public class OMPSingleDirectiveStmt : OMPExecutableDirective {
 internal OMPSingleDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPSingleDirective, parent) {
 }
 }

public class OMPTargetDataDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetDataDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetDataDirective, parent) {
 }
 }

public class OMPTargetDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetDirective, parent) {
 }
 }

public class OMPTargetEnterDataDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetEnterDataDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetEnterDataDirective, parent) {
 }
 }

public class OMPTargetExitDataDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetExitDataDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetExitDataDirective, parent) {
 }
 }

public class OMPTargetParallelDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetParallelDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetParallelDirective, parent) {
 }
 }

public class OMPTargetParallelForDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetParallelForDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetParallelForDirective, parent) {
 }
 }

public class OMPTargetTeamsDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetTeamsDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetTeamsDirective, parent) {
 }
 }

public class OMPTargetUpdateDirectiveStmt : OMPExecutableDirective {
 internal OMPTargetUpdateDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTargetUpdateDirective, parent) {
 }
 }

public class OMPTaskDirectiveStmt : OMPExecutableDirective {
 internal OMPTaskDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskDirective, parent) {
 }
 }

public class OMPTaskgroupDirectiveStmt : OMPExecutableDirective {
 internal OMPTaskgroupDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskgroupDirective, parent) {
 }
 }

public class OMPTaskwaitDirectiveStmt : OMPExecutableDirective {
 internal OMPTaskwaitDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskwaitDirective, parent) {
 }
 }

public class OMPTaskyieldDirectiveStmt : OMPExecutableDirective {
 internal OMPTaskyieldDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTaskyieldDirective, parent) {
 }
 }

public class OMPTeamsDirectiveStmt : OMPExecutableDirective {
 internal OMPTeamsDirectiveStmt(IStmt parent) :
 base(StmtClass.OMPTeamsDirective, parent) {
 }
 }

public class ObjCAtCatchStmtStmt : Stmt {
 internal ObjCAtCatchStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAtCatchStmt, parent) {
 }
 }

public class ObjCAtFinallyStmtStmt : Stmt {
 internal ObjCAtFinallyStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAtFinallyStmt, parent) {
 }
 }

public class ObjCAtSynchronizedStmtStmt : Stmt {
 internal ObjCAtSynchronizedStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAtSynchronizedStmt, parent) {
 }
 }

public class ObjCAtThrowStmtStmt : Stmt {
 internal ObjCAtThrowStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAtThrowStmt, parent) {
 }
 }

public class ObjCAtTryStmtStmt : Stmt {
 internal ObjCAtTryStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAtTryStmt, parent) {
 }
 }

public class ObjCAutoreleasePoolStmtStmt : Stmt {
 internal ObjCAutoreleasePoolStmtStmt(IStmt parent) :
 base(StmtClass.ObjCAutoreleasePoolStmt, parent) {
 }
 }

public class ObjCForCollectionStmtStmt : Stmt {
 internal ObjCForCollectionStmtStmt(IStmt parent) :
 base(StmtClass.ObjCForCollectionStmt, parent) {
 }
 }

public class ReturnStmtStmt : Stmt {
 internal ReturnStmtStmt(IStmt parent) :
 base(StmtClass.ReturnStmt, parent) {
 }
 }

public class SEHExceptStmtStmt : Stmt {
 internal SEHExceptStmtStmt(IStmt parent) :
 base(StmtClass.SEHExceptStmt, parent) {
 }
 }

public class SEHFinallyStmtStmt : Stmt {
 internal SEHFinallyStmtStmt(IStmt parent) :
 base(StmtClass.SEHFinallyStmt, parent) {
 }
 }

public class SEHLeaveStmtStmt : Stmt {
 internal SEHLeaveStmtStmt(IStmt parent) :
 base(StmtClass.SEHLeaveStmt, parent) {
 }
 }

public class SEHTryStmtStmt : Stmt {
 internal SEHTryStmtStmt(IStmt parent) :
 base(StmtClass.SEHTryStmt, parent) {
 }
 }

public class CaseStmtStmt : SwitchCase {
 internal CaseStmtStmt(IStmt parent) :
 base(StmtClass.CaseStmt, parent) {
 }
 }

public class DefaultStmtStmt : SwitchCase {
 internal DefaultStmtStmt(IStmt parent) :
 base(StmtClass.DefaultStmt, parent) {
 }
 }

public class SwitchStmtStmt : Stmt {
 internal SwitchStmtStmt(IStmt parent) :
 base(StmtClass.SwitchStmt, parent) {
 }
 }

public class WhileStmtStmt : Stmt {
 internal WhileStmtStmt(IStmt parent) :
 base(StmtClass.WhileStmt, parent) {
 }
 }

}

