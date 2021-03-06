﻿using System;
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
        private readonly string _enumImage;

        private readonly List<EnumMemberDeclarationSyntax> _enumMembers;
        private readonly string _methodImage;
        private readonly List<MethodDeclarationSyntax> _methods;
        private readonly List<ConstructorDeclarationSyntax> _constructors;

        private readonly List<PropertyDeclarationSyntax> _properties;
        private readonly string _propertyImage;

        #region Constructors

        public SyntaxVisitor(string propertyImage, string methodImage, string enumImage)
        {
            if (propertyImage == null) throw new ArgumentNullException(nameof(propertyImage));
            if (methodImage == null) throw new ArgumentNullException(nameof(methodImage));
            if (enumImage == null) throw new ArgumentNullException(nameof(enumImage));

            _propertyImage = propertyImage;
            _methodImage = methodImage;
            _enumImage = enumImage;

            Builder = new StringBuilder();
            _properties = new List<PropertyDeclarationSyntax>();
            _methods = new List<MethodDeclarationSyntax>();
            _constructors = new List<ConstructorDeclarationSyntax>();
            _enumMembers = new List<EnumMemberDeclarationSyntax>();
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

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            _constructors.Add(node);
            return base.VisitConstructorDeclaration(node);
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
            _enumMembers.Clear();

            Builder.AppendLine($"### {node.Identifier.Text} reference");

            var documentation = node.GetDocumentation();
            var summary = documentation.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary)) Builder.AppendLine(summary);

            var result = base.VisitEnumDeclaration(node);
            if (_enumMembers.Any())
            {
                Builder.AppendLine("<table>");
                Builder.AppendLine("  <tr>");
                Builder.AppendLine("    <th width=\"45\"></th>");
                Builder.AppendLine("    <th>Name</th>");
                Builder.AppendLine("    <th>Description</th>");
                Builder.AppendLine("  </tr>");

                foreach (var enumMember in _enumMembers)
                {
                    var enumDocumentation = enumMember.GetDocumentation();
                    var enumSummary = enumDocumentation.GetSummary();

                    Builder.AppendLine("  <tr>");
                    Builder.AppendLine($"   <td><img src=\"{_enumImage}\"/></td>");
                    Builder.AppendLine($"   <td>{enumMember.Identifier.Text}</td>");
                    Builder.AppendLine($"   <td>{enumSummary ?? string.Empty}</td>");
                    Builder.AppendLine("  </tr>");
                }

                Builder.AppendLine("</table>");
            }
            return result;
        }

        public override SyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            _enumMembers.Add(node);
            return base.VisitEnumMemberDeclaration(node);
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

            if (_properties.Any() || _methods.Any() || _constructors.Any())
            {
                Builder.AppendLine("<table>");
                Builder.AppendLine("  <tr>");
                Builder.AppendLine("    <th width=\"45\"></th>");
                Builder.AppendLine("    <th>Name</th>");
                Builder.AppendLine("    <th>Description</th>");
                Builder.AppendLine("  </tr>");

                foreach (var constructor in _constructors)
                {
                    var methodDocumentation = constructor.GetDocumentation();
                    var methodSummary = methodDocumentation.GetSummary();

                    Builder.AppendLine("  <tr>");
                    Builder.AppendLine($"    <td><img src=\"{_methodImage}\"/></td>");
                    Builder.AppendLine($"    <td>Constructor</td>");
                    Builder.AppendLine($"    <td>{methodSummary ?? string.Empty}");
                    Builder.AppendLine("      <dl>");

                    foreach (var parameter in constructor.ParameterList.Parameters)
                    {
                        var parameterName = parameter.Identifier.Text;
                        var parameterDocumentation = methodDocumentation.GetParameter(parameterName);
                        var parameterType = parameter.Type.ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                        Builder.AppendLine($"        <dt>{parameterName} : {parameterType}</dt>");
                        Builder.AppendLine($"        <dd>{parameterDocumentation}</dd>");
                    }

                    Builder.AppendLine("      </dl>");

                    Builder.AppendLine("    </td>");
                    Builder.AppendLine("  </tr>");
                }

                foreach (var property in _properties)
                {
                    var propertyDocumentation = property.GetDocumentation();
                    var propertySummary = propertyDocumentation.GetSummary();
                    var propertyType = property.Type.ToString().Replace("<", "&lt;").Replace(">", "&gt;");

                    Builder.AppendLine("  <tr>");
                    Builder.AppendLine($"   <td><img src=\"{_propertyImage}\"/></td>");
                    Builder.AppendLine($"   <td>{property.Identifier.Text}</td>");
                    Builder.AppendLine($"   <td>{propertySummary ?? string.Empty}<br/>");
                    Builder.AppendLine($"        <b><i>Type : {propertyType}</i></b>");
                    Builder.AppendLine("   </td>");
                    Builder.AppendLine("  </tr>");
                }

                foreach (var method in _methods)
                {
                    var methodDocumentation = method.GetDocumentation();
                    var methodSummary = methodDocumentation.GetSummary();

                    Builder.AppendLine("  <tr>");
                    Builder.AppendLine($"    <td><img src=\"{_methodImage}\"/></td>");
                    Builder.AppendLine($"    <td>{method.Identifier.Text}</td>");
                    Builder.AppendLine($"    <td>{methodSummary ?? string.Empty}");
                    Builder.AppendLine("      <dl>");

                    foreach (var parameter in method.ParameterList.Parameters)
                    {
                        var parameterName = parameter.Identifier.Text;
                        var parameterDocumentation = methodDocumentation.GetParameter(parameterName);
                        var parameterType = parameter.Type.ToString().Replace("<", "&lt;").Replace(">", "&gt;");
                        Builder.AppendLine($"        <dt>{parameterName} : {parameterType}</dt>");
                        Builder.AppendLine($"        <dd>{parameterDocumentation}</dd>");
                    }

                    if (method.ReturnType.ToString() != "void")
                    {
                        var returnsDocumentation = methodDocumentation.GetReturns();
                        var returnsType = method.ReturnType.ToString().Replace("<", "&lt;").Replace(">", "&gt;");

                        Builder.AppendLine($"        <dt>Returns : {returnsType}</dt>");
                        Builder.AppendLine($"        <dd>{returnsDocumentation}</dd>");
                    }

                    Builder.AppendLine("      </dl>");

                    Builder.AppendLine("    </td>");
                    Builder.AppendLine("  </tr>");
                }

                Builder.AppendLine("</table>");
            }

            return result;
        }

        #endregion
    }
}