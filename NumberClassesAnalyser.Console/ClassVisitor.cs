// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ClassVisitor : CSharpSyntaxRewriter
{
    public List<string> Classes { get; } = new List<string>();

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);

        string name = node.Identifier.ValueText;
        Classes.Add(name);
        return node;
    }
}