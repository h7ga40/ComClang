using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComClang
{
	public class AsmStmt : Stmt
	{
		internal AsmStmt(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class GCCAsmStmtStmt : AsmStmt
	{
		internal GCCAsmStmtStmt(IStmt parent)
		: base(StmtClass.GCCAsmStmt, parent)
		{
		}
	}

	public class MSAsmStmtStmt : AsmStmt
	{
		internal MSAsmStmtStmt(IStmt parent)
		: base(StmtClass.MSAsmStmt, parent)
		{
		}
	}

	public class AttributedStmtStmt : Stmt
	{
		internal AttributedStmtStmt(IStmt parent)
		: base(StmtClass.AttributedStmt, parent)
		{
		}
	}

	public class BreakStmtStmt : Stmt
	{
		internal BreakStmtStmt(IStmt parent)
		: base(StmtClass.BreakStmt, parent)
		{
		}
	}

	public class CXXCatchStmtStmt : Stmt
	{
		internal CXXCatchStmtStmt(IStmt parent)
		: base(StmtClass.CXXCatchStmt, parent)
		{
		}
	}

	public class CXXForRangeStmtStmt : Stmt
	{
		internal CXXForRangeStmtStmt(IStmt parent)
		: base(StmtClass.CXXForRangeStmt, parent)
		{
		}
	}

	public class CXXTryStmtStmt : Stmt
	{
		internal CXXTryStmtStmt(IStmt parent)
		: base(StmtClass.CXXTryStmt, parent)
		{
		}
	}

	public class CapturedStmtStmt : Stmt
	{
		internal CapturedStmtStmt(IStmt parent)
		: base(StmtClass.CapturedStmt, parent)
		{
		}
	}

	public class CompoundStmtStmt : Stmt
	{
		internal CompoundStmtStmt(IStmt parent)
		: base(StmtClass.CompoundStmt, parent)
		{
		}
	}

	public class ContinueStmtStmt : Stmt
	{
		internal ContinueStmtStmt(IStmt parent)
		: base(StmtClass.ContinueStmt, parent)
		{
		}
	}

	public class CoreturnStmtStmt : Stmt
	{
		internal CoreturnStmtStmt(IStmt parent)
		: base(StmtClass.CoreturnStmt, parent)
		{
		}
	}

	public class CoroutineBodyStmtStmt : Stmt
	{
		internal CoroutineBodyStmtStmt(IStmt parent)
		: base(StmtClass.CoroutineBodyStmt, parent)
		{
		}
	}

	public class DeclStmtStmt : Stmt
	{
		internal DeclStmtStmt(IStmt parent)
		: base(StmtClass.DeclStmt, parent)
		{
		}
	}

	public class DoStmtStmt : Stmt
	{
		internal DoStmtStmt(IStmt parent)
		: base(StmtClass.DoStmt, parent)
		{
		}
	}

	public class Expr : Stmt
	{
		internal Expr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class AbstractConditionalOperator : Expr
	{
		internal AbstractConditionalOperator(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class BinaryConditionalOperatorStmt : AbstractConditionalOperator
	{
		internal BinaryConditionalOperatorStmt(IStmt parent)
		: base(StmtClass.BinaryConditionalOperator, parent)
		{
		}
	}

	public class ConditionalOperatorStmt : AbstractConditionalOperator
	{
		internal ConditionalOperatorStmt(IStmt parent)
		: base(StmtClass.ConditionalOperator, parent)
		{
		}
	}

	public class AddrLabelExprStmt : Expr
	{
		internal AddrLabelExprStmt(IStmt parent)
		: base(StmtClass.AddrLabelExpr, parent)
		{
		}
	}

	public class ArrayInitIndexExprStmt : Expr
	{
		internal ArrayInitIndexExprStmt(IStmt parent)
		: base(StmtClass.ArrayInitIndexExpr, parent)
		{
		}
	}

	public class ArrayInitLoopExprStmt : Expr
	{
		internal ArrayInitLoopExprStmt(IStmt parent)
		: base(StmtClass.ArrayInitLoopExpr, parent)
		{
		}
	}

	public class ArraySubscriptExprStmt : Expr
	{
		internal ArraySubscriptExprStmt(IStmt parent)
		: base(StmtClass.ArraySubscriptExpr, parent)
		{
		}
	}

	public class ArrayTypeTraitExprStmt : Expr
	{
		internal ArrayTypeTraitExprStmt(IStmt parent)
		: base(StmtClass.ArrayTypeTraitExpr, parent)
		{
		}
	}

	public class AsTypeExprStmt : Expr
	{
		internal AsTypeExprStmt(IStmt parent)
		: base(StmtClass.AsTypeExpr, parent)
		{
		}
	}

	public class AtomicExprStmt : Expr
	{
		internal AtomicExprStmt(IStmt parent)
		: base(StmtClass.AtomicExpr, parent)
		{
		}
	}

	public class BinaryOperatorStmt : Expr
	{
		internal BinaryOperatorStmt(IStmt parent)
		: base(StmtClass.BinaryOperator, parent)
		{
		}
	}

	public class BinaryOperator : Expr
	{
		internal BinaryOperator(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}


	public class CompoundAssignOperatorStmt : BinaryOperator
	{
		internal CompoundAssignOperatorStmt(IStmt parent)
		: base(StmtClass.CompoundAssignOperator, parent)
		{
		}
	}

	public class BlockExprStmt : Expr
	{
		internal BlockExprStmt(IStmt parent)
		: base(StmtClass.BlockExpr, parent)
		{
		}
	}

	public class CXXBindTemporaryExprStmt : Expr
	{
		internal CXXBindTemporaryExprStmt(IStmt parent)
		: base(StmtClass.CXXBindTemporaryExpr, parent)
		{
		}
	}

	public class CXXBoolLiteralExprStmt : Expr
	{
		internal CXXBoolLiteralExprStmt(IStmt parent)
		: base(StmtClass.CXXBoolLiteralExpr, parent)
		{
		}
	}

	public class CXXConstructExprStmt : Expr
	{
		internal CXXConstructExprStmt(IStmt parent)
		: base(StmtClass.CXXConstructExpr, parent)
		{
		}
	}

	public class CXXConstructExpr : Expr
	{
		internal CXXConstructExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class CXXTemporaryObjectExprStmt : CXXConstructExpr
	{
		internal CXXTemporaryObjectExprStmt(IStmt parent)
		: base(StmtClass.CXXTemporaryObjectExpr, parent)
		{
		}
	}

	public class CXXDefaultArgExprStmt : Expr
	{
		internal CXXDefaultArgExprStmt(IStmt parent)
		: base(StmtClass.CXXDefaultArgExpr, parent)
		{
		}
	}

	public class CXXDefaultInitExprStmt : Expr
	{
		internal CXXDefaultInitExprStmt(IStmt parent)
		: base(StmtClass.CXXDefaultInitExpr, parent)
		{
		}
	}

	public class CXXDeleteExprStmt : Expr
	{
		internal CXXDeleteExprStmt(IStmt parent)
		: base(StmtClass.CXXDeleteExpr, parent)
		{
		}
	}

	public class CXXDependentScopeMemberExprStmt : Expr
	{
		internal CXXDependentScopeMemberExprStmt(IStmt parent)
		: base(StmtClass.CXXDependentScopeMemberExpr, parent)
		{
		}
	}

	public class CXXFoldExprStmt : Expr
	{
		internal CXXFoldExprStmt(IStmt parent)
		: base(StmtClass.CXXFoldExpr, parent)
		{
		}
	}

	public class CXXInheritedCtorInitExprStmt : Expr
	{
		internal CXXInheritedCtorInitExprStmt(IStmt parent)
		: base(StmtClass.CXXInheritedCtorInitExpr, parent)
		{
		}
	}

	public class CXXNewExprStmt : Expr
	{
		internal CXXNewExprStmt(IStmt parent)
		: base(StmtClass.CXXNewExpr, parent)
		{
		}
	}

	public class CXXNoexceptExprStmt : Expr
	{
		internal CXXNoexceptExprStmt(IStmt parent)
		: base(StmtClass.CXXNoexceptExpr, parent)
		{
		}
	}

	public class CXXNullPtrLiteralExprStmt : Expr
	{
		internal CXXNullPtrLiteralExprStmt(IStmt parent)
		: base(StmtClass.CXXNullPtrLiteralExpr, parent)
		{
		}
	}

	public class CXXPseudoDestructorExprStmt : Expr
	{
		internal CXXPseudoDestructorExprStmt(IStmt parent)
		: base(StmtClass.CXXPseudoDestructorExpr, parent)
		{
		}
	}

	public class CXXScalarValueInitExprStmt : Expr
	{
		internal CXXScalarValueInitExprStmt(IStmt parent)
		: base(StmtClass.CXXScalarValueInitExpr, parent)
		{
		}
	}

	public class CXXStdInitializerListExprStmt : Expr
	{
		internal CXXStdInitializerListExprStmt(IStmt parent)
		: base(StmtClass.CXXStdInitializerListExpr, parent)
		{
		}
	}

	public class CXXThisExprStmt : Expr
	{
		internal CXXThisExprStmt(IStmt parent)
		: base(StmtClass.CXXThisExpr, parent)
		{
		}
	}

	public class CXXThrowExprStmt : Expr
	{
		internal CXXThrowExprStmt(IStmt parent)
		: base(StmtClass.CXXThrowExpr, parent)
		{
		}
	}

	public class CXXTypeidExprStmt : Expr
	{
		internal CXXTypeidExprStmt(IStmt parent)
		: base(StmtClass.CXXTypeidExpr, parent)
		{
		}
	}

	public class CXXUnresolvedConstructExprStmt : Expr
	{
		internal CXXUnresolvedConstructExprStmt(IStmt parent)
		: base(StmtClass.CXXUnresolvedConstructExpr, parent)
		{
		}
	}

	public class CXXUuidofExprStmt : Expr
	{
		internal CXXUuidofExprStmt(IStmt parent)
		: base(StmtClass.CXXUuidofExpr, parent)
		{
		}
	}

	public class CallExprStmt : Expr
	{
		internal CallExprStmt(IStmt parent)
		: base(StmtClass.CallExpr, parent)
		{
		}
	}

	public class CallExpr : Expr
	{
		internal CallExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class CUDAKernelCallExprStmt : CallExpr
	{
		internal CUDAKernelCallExprStmt(IStmt parent)
		: base(StmtClass.CUDAKernelCallExpr, parent)
		{
		}
	}

	public class CXXMemberCallExprStmt : CallExpr
	{
		internal CXXMemberCallExprStmt(IStmt parent)
		: base(StmtClass.CXXMemberCallExpr, parent)
		{
		}
	}

	public class CXXOperatorCallExprStmt : CallExpr
	{
		internal CXXOperatorCallExprStmt(IStmt parent)
		: base(StmtClass.CXXOperatorCallExpr, parent)
		{
		}
	}

	public class UserDefinedLiteralStmt : CallExpr
	{

		internal UserDefinedLiteralStmt(IStmt parent)
			: base(StmtClass.UserDefinedLiteral, parent)
		{
		}
	}

	public class CastExpr : Expr
	{
		internal CastExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class ExplicitCastExpr : CastExpr
	{
		internal ExplicitCastExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class CStyleCastExprStmt : ExplicitCastExpr
	{
		internal CStyleCastExprStmt(IStmt parent)
		: base(StmtClass.CStyleCastExpr, parent)
		{
		}
	}

	public class CXXFunctionalCastExprStmt : ExplicitCastExpr
	{
		internal CXXFunctionalCastExprStmt(IStmt parent)
		: base(StmtClass.CXXFunctionalCastExpr, parent)
		{
		}
	}

	public class CXXNamedCastExpr : ExplicitCastExpr
	{
		internal CXXNamedCastExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class CXXConstCastExprStmt : CXXNamedCastExpr
	{
		internal CXXConstCastExprStmt(IStmt parent)
		: base(StmtClass.CXXConstCastExpr, parent)
		{
		}
	}

	public class CXXDynamicCastExprStmt : CXXNamedCastExpr
	{
		internal CXXDynamicCastExprStmt(IStmt parent)
		: base(StmtClass.CXXDynamicCastExpr, parent)
		{
		}
	}

	public class CXXReinterpretCastExprStmt : CXXNamedCastExpr
	{
		internal CXXReinterpretCastExprStmt(IStmt parent)
		: base(StmtClass.CXXReinterpretCastExpr, parent)
		{
		}
	}

	public class CXXStaticCastExprStmt : CXXNamedCastExpr
	{
		internal CXXStaticCastExprStmt(IStmt parent)
		: base(StmtClass.CXXStaticCastExpr, parent)
		{
		}
	}

	public class ObjCBridgedCastExprStmt : ExplicitCastExpr
	{
		internal ObjCBridgedCastExprStmt(IStmt parent)
		: base(StmtClass.ObjCBridgedCastExpr, parent)
		{
		}
	}

	public class ImplicitCastExprStmt : CastExpr
	{
		internal ImplicitCastExprStmt(IStmt parent)
		: base(StmtClass.ImplicitCastExpr, parent)
		{
		}
	}

	public class CharacterLiteralStmt : Expr
	{
		internal CharacterLiteralStmt(IStmt parent)
		: base(StmtClass.CharacterLiteral, parent)
		{
		}
	}

	public class ChooseExprStmt : Expr
	{
		internal ChooseExprStmt(IStmt parent)
		: base(StmtClass.ChooseExpr, parent)
		{
		}
	}

	public class CompoundLiteralExprStmt : Expr
	{
		internal CompoundLiteralExprStmt(IStmt parent)
		: base(StmtClass.CompoundLiteralExpr, parent)
		{
		}
	}

	public class ConvertVectorExprStmt : Expr
	{
		internal ConvertVectorExprStmt(IStmt parent)
		: base(StmtClass.ConvertVectorExpr, parent)
		{
		}
	}

	public class CoroutineSuspendExpr : Expr
	{
		internal CoroutineSuspendExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class CoawaitExprStmt : CoroutineSuspendExpr
	{
		internal CoawaitExprStmt(IStmt parent)
		: base(StmtClass.CoawaitExpr, parent)
		{
		}
	}

	public class CoyieldExprStmt : CoroutineSuspendExpr
	{
		internal CoyieldExprStmt(IStmt parent)
		: base(StmtClass.CoyieldExpr, parent)
		{
		}
	}

	public class DeclRefExprStmt : Expr
	{
		internal DeclRefExprStmt(IStmt parent)
		: base(StmtClass.DeclRefExpr, parent)
		{
		}
	}

	public class DependentCoawaitExprStmt : Expr
	{
		internal DependentCoawaitExprStmt(IStmt parent)
		: base(StmtClass.DependentCoawaitExpr, parent)
		{
		}
	}

	public class DependentScopeDeclRefExprStmt : Expr
	{
		internal DependentScopeDeclRefExprStmt(IStmt parent)
		: base(StmtClass.DependentScopeDeclRefExpr, parent)
		{
		}
	}

	public class DesignatedInitExprStmt : Expr
	{
		internal DesignatedInitExprStmt(IStmt parent)
		: base(StmtClass.DesignatedInitExpr, parent)
		{
		}
	}

	public class DesignatedInitUpdateExprStmt : Expr
	{
		internal DesignatedInitUpdateExprStmt(IStmt parent)
		: base(StmtClass.DesignatedInitUpdateExpr, parent)
		{
		}
	}

	public class ExprWithCleanupsStmt : Expr
	{
		internal ExprWithCleanupsStmt(IStmt parent)
		: base(StmtClass.ExprWithCleanups, parent)
		{
		}
	}

	public class ExpressionTraitExprStmt : Expr
	{
		internal ExpressionTraitExprStmt(IStmt parent)
		: base(StmtClass.ExpressionTraitExpr, parent)
		{
		}
	}

	public class ExtVectorElementExprStmt : Expr
	{
		internal ExtVectorElementExprStmt(IStmt parent)
		: base(StmtClass.ExtVectorElementExpr, parent)
		{
		}
	}

	public class FloatingLiteralStmt : Expr
	{
		internal FloatingLiteralStmt(IStmt parent)
		: base(StmtClass.FloatingLiteral, parent)
		{
		}
	}

	public class FunctionParmPackExprStmt : Expr
	{
		internal FunctionParmPackExprStmt(IStmt parent)
		: base(StmtClass.FunctionParmPackExpr, parent)
		{
		}
	}

	public class GNUNullExprStmt : Expr
	{
		internal GNUNullExprStmt(IStmt parent)
		: base(StmtClass.GNUNullExpr, parent)
		{
		}
	}

	public class GenericSelectionExprStmt : Expr
	{
		internal GenericSelectionExprStmt(IStmt parent)
		: base(StmtClass.GenericSelectionExpr, parent)
		{
		}
	}

	public class ImaginaryLiteralStmt : Expr
	{
		internal ImaginaryLiteralStmt(IStmt parent)
		: base(StmtClass.ImaginaryLiteral, parent)
		{
		}
	}

	public class ImplicitValueInitExprStmt : Expr
	{
		internal ImplicitValueInitExprStmt(IStmt parent)
		: base(StmtClass.ImplicitValueInitExpr, parent)
		{
		}
	}

	public class InitListExprStmt : Expr
	{
		internal InitListExprStmt(IStmt parent)
		: base(StmtClass.InitListExpr, parent)
		{
		}
	}

	public class IntegerLiteralStmt : Expr
	{
		internal IntegerLiteralStmt(IStmt parent)
		: base(StmtClass.IntegerLiteral, parent)
		{
		}
	}

	public class LambdaExprStmt : Expr
	{
		internal LambdaExprStmt(IStmt parent)
		: base(StmtClass.LambdaExpr, parent)
		{
		}
	}

	public class MSPropertyRefExprStmt : Expr
	{
		internal MSPropertyRefExprStmt(IStmt parent)
		: base(StmtClass.MSPropertyRefExpr, parent)
		{
		}
	}

	public class MSPropertySubscriptExprStmt : Expr
	{
		internal MSPropertySubscriptExprStmt(IStmt parent)
		: base(StmtClass.MSPropertySubscriptExpr, parent)
		{
		}
	}

	public class MaterializeTemporaryExprStmt : Expr
	{
		internal MaterializeTemporaryExprStmt(IStmt parent)
		: base(StmtClass.MaterializeTemporaryExpr, parent)
		{
		}
	}

	public class MemberExprStmt : Expr
	{
		internal MemberExprStmt(IStmt parent)
		: base(StmtClass.MemberExpr, parent)
		{
		}
	}

	public class NoInitExprStmt : Expr
	{
		internal NoInitExprStmt(IStmt parent)
		: base(StmtClass.NoInitExpr, parent)
		{
		}
	}

	public class OMPArraySectionExprStmt : Expr
	{
		internal OMPArraySectionExprStmt(IStmt parent)
		: base(StmtClass.OMPArraySectionExpr, parent)
		{
		}
	}

	public class ObjCArrayLiteralStmt : Expr
	{
		internal ObjCArrayLiteralStmt(IStmt parent)
		: base(StmtClass.ObjCArrayLiteral, parent)
		{
		}
	}

	public class ObjCAvailabilityCheckExprStmt : Expr
	{
		internal ObjCAvailabilityCheckExprStmt(IStmt parent)
		: base(StmtClass.ObjCAvailabilityCheckExpr, parent)
		{
		}
	}

	public class ObjCBoolLiteralExprStmt : Expr
	{
		internal ObjCBoolLiteralExprStmt(IStmt parent)
		: base(StmtClass.ObjCBoolLiteralExpr, parent)
		{
		}
	}

	public class ObjCBoxedExprStmt : Expr
	{
		internal ObjCBoxedExprStmt(IStmt parent)
		: base(StmtClass.ObjCBoxedExpr, parent)
		{
		}
	}

	public class ObjCDictionaryLiteralStmt : Expr
	{
		internal ObjCDictionaryLiteralStmt(IStmt parent)
		: base(StmtClass.ObjCDictionaryLiteral, parent)
		{
		}
	}

	public class ObjCEncodeExprStmt : Expr
	{
		internal ObjCEncodeExprStmt(IStmt parent)
		: base(StmtClass.ObjCEncodeExpr, parent)
		{
		}
	}

	public class ObjCIndirectCopyRestoreExprStmt : Expr
	{
		internal ObjCIndirectCopyRestoreExprStmt(IStmt parent)
		: base(StmtClass.ObjCIndirectCopyRestoreExpr, parent)
		{
		}
	}

	public class ObjCIsaExprStmt : Expr
	{
		internal ObjCIsaExprStmt(IStmt parent)
		: base(StmtClass.ObjCIsaExpr, parent)
		{
		}
	}

	public class ObjCIvarRefExprStmt : Expr
	{
		internal ObjCIvarRefExprStmt(IStmt parent)
		: base(StmtClass.ObjCIvarRefExpr, parent)
		{
		}
	}

	public class ObjCMessageExprStmt : Expr
	{
		internal ObjCMessageExprStmt(IStmt parent)
		: base(StmtClass.ObjCMessageExpr, parent)
		{
		}
	}

	public class ObjCPropertyRefExprStmt : Expr
	{
		internal ObjCPropertyRefExprStmt(IStmt parent)
		: base(StmtClass.ObjCPropertyRefExpr, parent)
		{
		}
	}

	public class ObjCProtocolExprStmt : Expr
	{
		internal ObjCProtocolExprStmt(IStmt parent)
		: base(StmtClass.ObjCProtocolExpr, parent)
		{
		}
	}

	public class ObjCSelectorExprStmt : Expr
	{
		internal ObjCSelectorExprStmt(IStmt parent)
		: base(StmtClass.ObjCSelectorExpr, parent)
		{
		}
	}

	public class ObjCStringLiteralStmt : Expr
	{
		internal ObjCStringLiteralStmt(IStmt parent)
		: base(StmtClass.ObjCStringLiteral, parent)
		{
		}
	}

	public class ObjCSubscriptRefExprStmt : Expr
	{
		internal ObjCSubscriptRefExprStmt(IStmt parent)
		: base(StmtClass.ObjCSubscriptRefExpr, parent)
		{
		}
	}

	public class OffsetOfExprStmt : Expr
	{
		internal OffsetOfExprStmt(IStmt parent)
		: base(StmtClass.OffsetOfExpr, parent)
		{
		}
	}

	public class OpaqueValueExprStmt : Expr
	{
		internal OpaqueValueExprStmt(IStmt parent)
		: base(StmtClass.OpaqueValueExpr, parent)
		{
		}
	}

	public class OverloadExpr : Expr
	{
		internal OverloadExpr(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class UnresolvedLookupExprStmt : OverloadExpr
	{
		internal UnresolvedLookupExprStmt(IStmt parent)
		: base(StmtClass.UnresolvedLookupExpr, parent)
		{
		}
	}

	public class UnresolvedMemberExprStmt : OverloadExpr
	{
		internal UnresolvedMemberExprStmt(IStmt parent)
		: base(StmtClass.UnresolvedMemberExpr, parent)
		{
		}
	}

	public class PackExpansionExprStmt : Expr
	{
		internal PackExpansionExprStmt(IStmt parent)
		: base(StmtClass.PackExpansionExpr, parent)
		{
		}
	}

	public class ParenExprStmt : Expr
	{
		internal ParenExprStmt(IStmt parent)
		: base(StmtClass.ParenExpr, parent)
		{
		}
	}

	public class ParenListExprStmt : Expr
	{
		internal ParenListExprStmt(IStmt parent)
		: base(StmtClass.ParenListExpr, parent)
		{
		}
	}

	public class PredefinedExprStmt : Expr
	{
		internal PredefinedExprStmt(IStmt parent)
		: base(StmtClass.PredefinedExpr, parent)
		{
		}
	}

	public class PseudoObjectExprStmt : Expr
	{
		internal PseudoObjectExprStmt(IStmt parent)
		: base(StmtClass.PseudoObjectExpr, parent)
		{
		}
	}

	public class ShuffleVectorExprStmt : Expr
	{
		internal ShuffleVectorExprStmt(IStmt parent)
		: base(StmtClass.ShuffleVectorExpr, parent)
		{
		}
	}

	public class SizeOfPackExprStmt : Expr
	{
		internal SizeOfPackExprStmt(IStmt parent)
		: base(StmtClass.SizeOfPackExpr, parent)
		{
		}
	}

	public class StmtExprStmt : Expr
	{
		internal StmtExprStmt(IStmt parent)
		: base(StmtClass.StmtExpr, parent)
		{
		}
	}

	public class StringLiteralStmt : Expr
	{
		internal StringLiteralStmt(IStmt parent)
		: base(StmtClass.StringLiteral, parent)
		{
		}
	}

	public class SubstNonTypeTemplateParmExprStmt : Expr
	{
		internal SubstNonTypeTemplateParmExprStmt(IStmt parent)
		: base(StmtClass.SubstNonTypeTemplateParmExpr, parent)
		{
		}
	}

	public class SubstNonTypeTemplateParmPackExprStmt : Expr
	{
		internal SubstNonTypeTemplateParmPackExprStmt(IStmt parent)
		: base(StmtClass.SubstNonTypeTemplateParmPackExpr, parent)
		{
		}
	}

	public class TypeTraitExprStmt : Expr
	{
		internal TypeTraitExprStmt(IStmt parent)
		: base(StmtClass.TypeTraitExpr, parent)
		{
		}
	}

	public class TypoExprStmt : Expr
	{
		internal TypoExprStmt(IStmt parent)
		: base(StmtClass.TypoExpr, parent)
		{
		}
	}

	public class UnaryExprOrTypeTraitExprStmt : Expr
	{
		internal UnaryExprOrTypeTraitExprStmt(IStmt parent)
		: base(StmtClass.UnaryExprOrTypeTraitExpr, parent)
		{
		}
	}

	public class UnaryOperatorStmt : Expr
	{
		internal UnaryOperatorStmt(IStmt parent)
		: base(StmtClass.UnaryOperator, parent)
		{
		}
	}

	public class VAArgExprStmt : Expr
	{
		internal VAArgExprStmt(IStmt parent)
		: base(StmtClass.VAArgExpr, parent)
		{
		}
	}

	public class ForStmtStmt : Stmt
	{
		internal ForStmtStmt(IStmt parent)
		: base(StmtClass.ForStmt, parent)
		{
		}
	}

	public class GotoStmtStmt : Stmt
	{
		internal GotoStmtStmt(IStmt parent)
		: base(StmtClass.GotoStmt, parent)
		{
		}
	}

	public class IfStmtStmt : Stmt
	{
		internal IfStmtStmt(IStmt parent)
		: base(StmtClass.IfStmt, parent)
		{
		}
	}

	public class IndirectGotoStmtStmt : Stmt
	{
		internal IndirectGotoStmtStmt(IStmt parent)
		: base(StmtClass.IndirectGotoStmt, parent)
		{
		}
	}

	public class LabelStmtStmt : Stmt
	{
		internal LabelStmtStmt(IStmt parent)
		: base(StmtClass.LabelStmt, parent)
		{
		}
	}

	public class MSDependentExistsStmtStmt : Stmt
	{
		internal MSDependentExistsStmtStmt(IStmt parent)
		: base(StmtClass.MSDependentExistsStmt, parent)
		{
		}
	}

	public class NullStmtStmt : Stmt
	{
		internal NullStmtStmt(IStmt parent)
		: base(StmtClass.NullStmt, parent)
		{
		}
	}

	public class OMPExecutableDirective : Stmt
	{
		internal OMPExecutableDirective(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class OMPAtomicDirectiveStmt : OMPExecutableDirective
	{
		internal OMPAtomicDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPAtomicDirective, parent)
		{
		}
	}

	public class OMPBarrierDirectiveStmt : OMPExecutableDirective
	{
		internal OMPBarrierDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPBarrierDirective, parent)
		{
		}
	}

	public class OMPCancelDirectiveStmt : OMPExecutableDirective
	{
		internal OMPCancelDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPCancelDirective, parent)
		{
		}
	}

	public class OMPCancellationPointDirectiveStmt : OMPExecutableDirective
	{
		internal OMPCancellationPointDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPCancellationPointDirective, parent)
		{
		}
	}

	public class OMPCriticalDirectiveStmt : OMPExecutableDirective
	{
		internal OMPCriticalDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPCriticalDirective, parent)
		{
		}
	}

	public class OMPFlushDirectiveStmt : OMPExecutableDirective
	{
		internal OMPFlushDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPFlushDirective, parent)
		{
		}
	}

	public class OMPLoopDirective : Stmt
	{
		internal OMPLoopDirective(StmtClass @class, IStmt parent)
		: base(@class, parent)
		{
		}
	}

	public class OMPDistributeDirectiveStmt : OMPLoopDirective
	{
		internal OMPDistributeDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPDistributeDirective, parent)
		{
		}
	}

	public class OMPDistributeParallelForDirectiveStmt : OMPLoopDirective
	{
		internal OMPDistributeParallelForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPDistributeParallelForDirective, parent)
		{
		}
	}

	public class OMPDistributeParallelForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPDistributeParallelForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPDistributeParallelForSimdDirective, parent)
		{
		}
	}

	public class OMPDistributeSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPDistributeSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPDistributeSimdDirective, parent)
		{
		}
	}

	public class OMPForDirectiveStmt : OMPLoopDirective
	{
		internal OMPForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPForDirective, parent)
		{
		}
	}

	public class OMPForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPForSimdDirective, parent)
		{
		}
	}

	public class OMPParallelForDirectiveStmt : OMPLoopDirective
	{
		internal OMPParallelForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPParallelForDirective, parent)
		{
		}
	}

	public class OMPParallelForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPParallelForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPParallelForSimdDirective, parent)
		{
		}
	}

	public class OMPSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPSimdDirective, parent)
		{
		}
	}

	public class OMPTargetParallelForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetParallelForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetParallelForSimdDirective, parent)
		{
		}
	}

	public class OMPTargetSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetSimdDirective, parent)
		{
		}
	}

	public class OMPTargetTeamsDistributeDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetTeamsDistributeDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetTeamsDistributeDirective, parent)
		{
		}
	}

	public class OMPTargetTeamsDistributeParallelForDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetTeamsDistributeParallelForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetTeamsDistributeParallelForDirective, parent)
		{
		}
	}

	public class OMPTargetTeamsDistributeParallelForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetTeamsDistributeParallelForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetTeamsDistributeParallelForSimdDirective, parent)
		{
		}
	}

	public class OMPTargetTeamsDistributeSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTargetTeamsDistributeSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetTeamsDistributeSimdDirective, parent)
		{
		}
	}

	public class OMPTaskLoopDirectiveStmt : OMPLoopDirective
	{
		internal OMPTaskLoopDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskLoopDirective, parent)
		{
		}
	}

	public class OMPTaskLoopSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTaskLoopSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskLoopSimdDirective, parent)
		{
		}
	}

	public class OMPTeamsDistributeDirectiveStmt : OMPLoopDirective
	{
		internal OMPTeamsDistributeDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTeamsDistributeDirective, parent)
		{
		}
	}

	public class OMPTeamsDistributeParallelForDirectiveStmt : OMPLoopDirective
	{
		internal OMPTeamsDistributeParallelForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTeamsDistributeParallelForDirective, parent)
		{
		}
	}

	public class OMPTeamsDistributeParallelForSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTeamsDistributeParallelForSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTeamsDistributeParallelForSimdDirective, parent)
		{
		}
	}

	public class OMPTeamsDistributeSimdDirectiveStmt : OMPLoopDirective
	{
		internal OMPTeamsDistributeSimdDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTeamsDistributeSimdDirective, parent)
		{
		}
	}

	public class OMPMasterDirectiveStmt : OMPExecutableDirective
	{
		internal OMPMasterDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPMasterDirective, parent)
		{
		}
	}

	public class OMPOrderedDirectiveStmt : OMPExecutableDirective
	{
		internal OMPOrderedDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPOrderedDirective, parent)
		{
		}
	}

	public class OMPParallelDirectiveStmt : OMPExecutableDirective
	{
		internal OMPParallelDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPParallelDirective, parent)
		{
		}
	}

	public class OMPParallelSectionsDirectiveStmt : OMPExecutableDirective
	{
		internal OMPParallelSectionsDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPParallelSectionsDirective, parent)
		{
		}
	}

	public class OMPSectionDirectiveStmt : OMPExecutableDirective
	{
		internal OMPSectionDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPSectionDirective, parent)
		{
		}
	}

	public class OMPSectionsDirectiveStmt : OMPExecutableDirective
	{
		internal OMPSectionsDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPSectionsDirective, parent)
		{
		}
	}

	public class OMPSingleDirectiveStmt : OMPExecutableDirective
	{
		internal OMPSingleDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPSingleDirective, parent)
		{
		}
	}

	public class OMPTargetDataDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetDataDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetDataDirective, parent)
		{
		}
	}

	public class OMPTargetDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetDirective, parent)
		{
		}
	}

	public class OMPTargetEnterDataDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetEnterDataDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetEnterDataDirective, parent)
		{
		}
	}

	public class OMPTargetExitDataDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetExitDataDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetExitDataDirective, parent)
		{
		}
	}

	public class OMPTargetParallelDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetParallelDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetParallelDirective, parent)
		{
		}
	}

	public class OMPTargetParallelForDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetParallelForDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetParallelForDirective, parent)
		{
		}
	}

	public class OMPTargetTeamsDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetTeamsDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetTeamsDirective, parent)
		{
		}
	}

	public class OMPTargetUpdateDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTargetUpdateDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTargetUpdateDirective, parent)
		{
		}
	}

	public class OMPTaskDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTaskDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskDirective, parent)
		{
		}
	}

	public class OMPTaskgroupDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTaskgroupDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskgroupDirective, parent)
		{
		}
	}

	public class OMPTaskwaitDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTaskwaitDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskwaitDirective, parent)
		{
		}
	}

	public class OMPTaskyieldDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTaskyieldDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTaskyieldDirective, parent)
		{
		}
	}

	public class OMPTeamsDirectiveStmt : OMPExecutableDirective
	{
		internal OMPTeamsDirectiveStmt(IStmt parent)
		: base(StmtClass.OMPTeamsDirective, parent)
		{
		}
	}

	public class ObjCAtCatchStmtStmt : Stmt
	{
		internal ObjCAtCatchStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAtCatchStmt, parent)
		{
		}
	}

	public class ObjCAtFinallyStmtStmt : Stmt
	{
		internal ObjCAtFinallyStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAtFinallyStmt, parent)
		{
		}
	}

	public class ObjCAtSynchronizedStmtStmt : Stmt
	{
		internal ObjCAtSynchronizedStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAtSynchronizedStmt, parent)
		{
		}
	}

	public class ObjCAtThrowStmtStmt : Stmt
	{
		internal ObjCAtThrowStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAtThrowStmt, parent)
		{
		}
	}

	public class ObjCAtTryStmtStmt : Stmt
	{
		internal ObjCAtTryStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAtTryStmt, parent)
		{
		}
	}

	public class ObjCAutoreleasePoolStmtStmt : Stmt
	{
		internal ObjCAutoreleasePoolStmtStmt(IStmt parent)
		: base(StmtClass.ObjCAutoreleasePoolStmt, parent)
		{
		}
	}

	public class ObjCForCollectionStmtStmt : Stmt
	{
		internal ObjCForCollectionStmtStmt(IStmt parent)
		: base(StmtClass.ObjCForCollectionStmt, parent)
		{
		}
	}

	public class ReturnStmtStmt : Stmt
	{
		internal ReturnStmtStmt(IStmt parent)
		: base(StmtClass.ReturnStmt, parent)
		{
		}
	}

	public class SEHExceptStmtStmt : Stmt
	{
		internal SEHExceptStmtStmt(IStmt parent)
		: base(StmtClass.SEHExceptStmt, parent)
		{
		}
	}

	public class SEHFinallyStmtStmt : Stmt
	{
		internal SEHFinallyStmtStmt(IStmt parent)
		: base(StmtClass.SEHFinallyStmt, parent)
		{
		}
	}

	public class SEHLeaveStmtStmt : Stmt
	{
		internal SEHLeaveStmtStmt(IStmt parent)
		: base(StmtClass.SEHLeaveStmt, parent)
		{
		}
	}

	public class SEHTryStmtStmt : Stmt
	{
		internal SEHTryStmtStmt(IStmt parent)
		: base(StmtClass.SEHTryStmt, parent)
		{
		}
	}

	public class SwitchCase : Stmt
	{
		internal SwitchCase(StmtClass @class, IStmt parent)
			: base(@class, parent)
		{

		}
	}

	public class CaseStmtStmt : SwitchCase
	{
		internal CaseStmtStmt(IStmt parent)
			: base(StmtClass.CaseStmt, parent)
		{
		}
	}

	public class DefaultStmtStmt : SwitchCase
	{
		internal DefaultStmtStmt(IStmt parent)
		: base(StmtClass.DefaultStmt, parent)
		{
		}
	}

	public class SwitchStmtStmt : Stmt
	{
		internal SwitchStmtStmt(IStmt parent)
		: base(StmtClass.SwitchStmt, parent)
		{
		}
	}

	public class WhileStmtStmt : Stmt
	{
		internal WhileStmtStmt(IStmt parent)
		: base(StmtClass.WhileStmt, parent)
		{
		}
	}
}
