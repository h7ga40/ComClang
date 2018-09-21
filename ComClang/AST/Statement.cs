using CppSharp.AST;
using System;

namespace CppSharp.AST
{
	public /*abstract*/ class Statement : IStatement
	{
		public StatementClass Class { get; set; }
		public Declaration Declaration { get; set; }
		public string String { get; set; } = "";
		IDeclaration IStatement.Declaration { get => (IDeclaration)((object)Declaration); set => Declaration = (Declaration)value; }

		public /*abstract*/virtual Statement Clone()
		{
			return new Statement()
			{
				Class = this.Class,
				Declaration = this.Declaration,
				String = this.String
			};
		}
	}
}
