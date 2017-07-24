using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PS.Github.Formatter.Extensions;

namespace PS.Github.Formatter
{
    class SyntaxVisitor : CSharpSyntaxRewriter
    {
        private readonly List<MethodDeclarationSyntax> _methods;

        private readonly List<PropertyDeclarationSyntax> _properties;

        #region Constructors

        public SyntaxVisitor()
        {
            Builder = new StringBuilder();
            _properties = new List<PropertyDeclarationSyntax>();
            _methods = new List<MethodDeclarationSyntax>();
        }

        #endregion

        #region Properties

        public StringBuilder Builder { get; }

        #endregion

        #region Override members

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            return HandleClassStructInterfaceDeclaration(node, syntax => base.VisitInterfaceDeclaration(node));
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            _properties.Add(node);
            return base.VisitPropertyDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            _methods.Add(node);
            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return HandleClassStructInterfaceDeclaration(node, syntax => base.VisitClassDeclaration(node));
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            return HandleClassStructInterfaceDeclaration(node, syntax => base.VisitStructDeclaration(node));
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            Builder.AppendLine($"### {node.Identifier.Text} reference");
            var documentation = node.GetDocumentation();
            var summary = documentation.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary)) Builder.AppendLine(summary);

            return base.VisitEnumDeclaration(node);
        }

        #endregion

        #region Members

        private SyntaxNode HandleClassStructInterfaceDeclaration<T>(T node, Func<T, SyntaxNode> baseMethod) where T : TypeDeclarationSyntax
        {
            _properties.Clear();
            _methods.Clear();

            Builder.AppendLine($"### {node.Identifier.Text} reference");

            var documentation = node.GetDocumentation();
            var summary = documentation.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary)) Builder.AppendLine(summary);

            var result = baseMethod(node);

            if (_properties.Any() || _methods.Any())
            {
                Builder.AppendLine("<table>");
                Builder.AppendLine("  <colwidth=\"50\">");
                Builder.AppendLine("  <tr>");
                Builder.AppendLine("    <th></th>");
                Builder.AppendLine("    <th>Name</th>");
                Builder.AppendLine("    <th>Description</th>");
                Builder.AppendLine("  </tr>");

                foreach (var property in _properties)
                {
                    var propertyDocumentation = property.GetDocumentation();
                    var propertySummary = propertyDocumentation.GetSummary();

                    Builder.AppendLine("  <tr>");
                    Builder.AppendLine("    <td><img src=\"https://raw.githubusercontent.com/BlackGad/PS.Build/master/.Assets/property.jpeg\"/></td>");
                    Builder.AppendLine($"   <td>{property.Identifier.Text}</td>");
                    Builder.AppendLine($"   <td>{propertySummary ?? string.Empty}</td>");
                    Builder.AppendLine("  </tr>");
                }

                Builder.AppendLine("</table>");
            }

            return result;
        }

        #endregion
    }
}