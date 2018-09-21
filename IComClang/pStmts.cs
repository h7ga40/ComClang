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
#define ABSTRACT_STMT(STMT)
#define STMT(CLASS, PARENT)                              \
			case StmtClass.CLASS ## Class: ret = new CLASS ## Stmt(parent); break;
#include "clang/AST/StmtNodes.inc"
			default: throw new NotImplementedException();
			}

			if (parent == null)
				Stmts.Add(ret);

			return ret;
		}

#define ABSTRACT_STMT(STMT)
#define STMT(CLASS, PARENT)                              \
	public class CLASS ## Stmt : PARENT                  \
	{                                                    \
		internal CLASS ## Stmt(IStmt parent)             \
			: base(StmtClass.CLASS, parent)              \
		{                                                \
		}                                                \
	}                                                    \

#include "clang/AST/StmtNodes.inc"
}
