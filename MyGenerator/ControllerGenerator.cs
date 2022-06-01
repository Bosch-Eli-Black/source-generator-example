using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.OpenApi.Readers;

namespace MyGenerator
{
    [Generator]
    public class ControllerGenerator : /*OpenApiGeneratorsBase,*/ ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                foreach (var file in context.AdditionalFiles)
                {
                    var reader = new OpenApiStringReader();
                    var openApiDocument = reader.Read("{}", out OpenApiDiagnostic openApiDiagnostic);
                    //ReadOpenApiFile();

                    string inputFilename = Path.GetFileNameWithoutExtension(file.Path);
                    string outputFilename = inputFilename + ".g.cs";

                    context.AddSource(outputFilename, $"class {inputFilename} {{}}");
                }
            }
            catch (Exception exception)
            {
                HandleGeneratorException(context, exception, "generator-error.g.cs");
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private static void HandleGeneratorException(GeneratorExecutionContext context, Exception exception, string fileName)
        {
            var diagnosticError = new DiagnosticDescriptor(
                    id: "GENERATOR",
                    title: "Source generator exception",
                    messageFormat: "Error from source generator: {0}. Stack trace: {1}",
                    category: "Generator",
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true,
                    description: ""
                );

            context.ReportDiagnostic(Diagnostic.Create(diagnosticError, Location.None, exception.Message, exception.StackTrace));

            string errorMessage = GetExceptionAsSourceCode(exception);

            context.AddSource(fileName, errorMessage);
        }

        private static string GetExceptionAsSourceCode(Exception exception)
        {
            try
            {
                return $@"#error Source generator error: {exception.Message}.

Stack trace:
{exception.StackTrace}";
            }
            catch
            {
                return $@"#error Error while generating error message";
            }
        }
    }
}