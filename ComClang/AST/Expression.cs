using CppSharp.AST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CppSharp.AST
{
	public abstract class Expression : Statement, IExpression
	{
		public string DebugText;
		public abstract TV Visit<TV>(IExpressionVisitor<TV> visitor);
	}

	public class BuiltinTypeExpression : Expression
	{
		public long Value { get; set; }

		public BuiltinType Type { get; set; }

		public bool IsHexadecimal {
			get {
				if (DebugText == null)
				{
					return false;
				}
				return DebugText.Contains("0x") || DebugText.Contains("0X");
			}
		}

		public override string ToString()
		{
			var printAsHex = IsHexadecimal && Type.IsUnsigned;
			var format = printAsHex ? "x" : string.Empty;
			var value = Value.ToString(format);
			return printAsHex ? "0x" + value : value;
		}

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			return new BuiltinTypeExpression
			{
				Value = this.Value,
				Type = this.Type,
				DebugText = this.DebugText,
				Class = this.Class,
				Declaration = this.Declaration,
				String = this.String
			};
		}
	}

	public class BinaryOperator : Expression, IBinaryOperator
	{
		public BinaryOperator(Expression lhs, Expression rhs, string opcodeStr)
		{
			Class = StatementClass.BinaryOperator;
			LHS = lhs;
			RHS = rhs;
			OpcodeStr = opcodeStr;
		}

		public Expression LHS { get; set; }
		public Expression RHS { get; set; }
		public string OpcodeStr { get; set; } = "";
		IExpression IBinaryOperator.LHS { get => LHS; set => LHS = (Expression)value; }
		IExpression IBinaryOperator.RHS { get => RHS; set => RHS = (Expression)value; }
		string IBinaryOperator.OpcodeStr { get => OpcodeStr; set => OpcodeStr = value; }

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			return new BinaryOperator((Expression)LHS.Clone(), (Expression)RHS.Clone(), OpcodeStr)
			{
				DebugText = this.DebugText,
				Declaration = this.Declaration,
				String = this.String
			};
		}
	}

	public class CallExpr : Expression, ICallExpr
	{
		public CallExpr()
		{
			Class = StatementClass.Call;
			Arguments = new List<Expression>();
		}

		public List<Expression> Arguments { get; private set; }

		public int ArgumentsCount => Arguments.Count;

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			var clone = new CallExpr
			{
				DebugText = this.DebugText,
				Declaration = this.Declaration,
				String = this.String
			};
			clone.Arguments.AddRange(Arguments.Select(a => (Expression)a.Clone()));
			return clone;
		}

		public IExpression GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(IExpression value)
		{
			Arguments.Add((Expression)value);
		}
	}

	public class CXXConstructExpr : Expression, ICXXConstructExpr
	{
		public CXXConstructExpr()
		{
			Class = StatementClass.ConstructorReference;
			Arguments = new List<Expression>();
		}

		public List<Expression> Arguments { get; private set; }

		public int ArgumentsCount => Arguments.Count;

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			var clone = new CXXConstructExpr
			{
				DebugText = this.DebugText,
				Declaration = this.Declaration,
				String = this.String
			};
			clone.Arguments.AddRange(Arguments.Select(a => (Expression)a.Clone()));
			return clone;
		}

		public IExpression GetArguments(int index)
		{
			return Arguments[index];
		}

		public void AddArguments(IExpression value)
		{
			Arguments.Add((Expression)value);
		}
	}

	public class InitListExpr : Expression, IInitListExpr
	{
		public InitListExpr()
		{
			Class = StatementClass.InitList;
			Inits = new List<Expression>();
		}

		public List<Expression> Inits { get; private set; }

		public int InitCount => Inits.Count;

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			var clone = new InitListExpr
			{
				DebugText = this.DebugText,
				Declaration = this.Declaration,
				String = this.String
			};
			clone.Inits.AddRange(Inits.Select(a => (Expression)a.Clone()));
			return clone;
		}

		public IExpression GetInit(int index)
		{
			return Inits[index];
		}

		public void AddInit(IExpression value)
		{
			Inits.Add((Expression)value);
		}
	}

	public class SubStmtExpr : Expression, ISubStmtExpr
	{
		public SubStmtExpr()
		{
			Class = StatementClass.SubStmt;
			Statements = new List<Statement>();
		}

		public List<Statement> Statements { get; private set; }

		public int StatementCount => Statements.Count;

		public override T Visit<T>(IExpressionVisitor<T> visitor)
		{
			return visitor.VisitExpression(this);
		}

		public override Statement Clone()
		{
			var clone = new SubStmtExpr
			{
				DebugText = this.DebugText,
				Declaration = this.Declaration,
				String = this.String
			};
			clone.Statements.AddRange(Statements.Select(a => a.Clone()));
			return clone;
		}

		public IStatement GetStatement(int index)
		{
			return Statements[index];
		}

		public void AddStatement(IStatement value)
		{
			Statements.Add((Statement)value);
		}
	}

	public interface IExpressionVisitor<out T>
	{
		T VisitExpression(Expression exp);
	}
}
