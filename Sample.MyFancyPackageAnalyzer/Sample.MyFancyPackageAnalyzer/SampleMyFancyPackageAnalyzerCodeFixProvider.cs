using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sample.MyFancyPackageAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SampleMyFancyPackageAnalyzer)), Shared]
    public class SampleMyFancyPackageAnalyzerCodeFixProvider : CodeFixProvider
    {
        /// <inheritdoc />
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(SampleMyFancyPackageAnalyzer.DiagnosticId);

        /// <inheritdoc />
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc />
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var diagnostic = context.Diagnostics.First();
            string newName = diagnostic.Properties[SampleMyFancyPackageAnalyzer.NewNameProperty];
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            ArgumentSyntax declaration = root.FindNode(diagnosticSpan) as ArgumentSyntax;

            if (declaration.Expression is LiteralExpressionSyntax literal)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.EndpointKeyNamingCodeFixTitle,
                        createChangedDocument: c => FixEndpointKeyName(context.Document, literal, newName, c),
                        equivalenceKey: nameof(Resources.EndpointKeyNamingCodeFixTitle)),
                    diagnostic);
            }
        }

        private async Task<Document> FixEndpointKeyName(
            Document document,
            LiteralExpressionSyntax typeDecl,
            string newName,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);
            root = root.ReplaceNode(
                typeDecl,
                SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(newName)));

            return document.WithSyntaxRoot(root);
        }
    }
}
