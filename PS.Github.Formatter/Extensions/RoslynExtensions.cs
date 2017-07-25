using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace PS.Github.Formatter.Extensions
{
    public static class RoslynExtensions
    {
        #region Static members

        public static string FormatSyntax(this string source, string propertyImage, string methodImage, string enumImage)
        {
            if (string.IsNullOrWhiteSpace(source)) return string.Empty;
            var tree = CSharpSyntaxTree.ParseText(source);
            var visitor = new SyntaxVisitor(propertyImage, methodImage, enumImage);
            visitor.Visit(tree.GetRoot());
            return visitor.Builder.ToString();
        }

        public static XElement GetDocumentation(this CSharpSyntaxNode node)
        {
            var trivia = node.GetLeadingTrivia().FirstOrDefault(t => t.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia);
            if (Equals(trivia, default(SyntaxTrivia))) return null;

            var xml = trivia.GetStructure().ToString().Replace("///", string.Empty).Trim(' ');
            try
            {
                return XDocument.Parse($"<root>{xml}</root>").Root;
            }
            catch (Exception e)
            {
                //Nothing
            }
            return null;
        }

        #endregion
    }
}