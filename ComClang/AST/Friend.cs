namespace CppSharp.AST
{
	public class Friend : Declaration, IFriend
	{
		public Declaration Declaration { get; set; }
		IDeclaration IFriend.Declaration { get => Declaration; set => Declaration = (Declaration)value; }

		public Friend()
		{
		}

		public Friend(Declaration declaration)
		{
			Declaration = declaration;
		}

		public override T Visit<T>(IDeclVisitor<T> visitor)
		{
			return visitor.VisitFriend(this);
		}
	}
}
