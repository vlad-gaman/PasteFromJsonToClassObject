using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Windows;

namespace PasteFromJsonToClassObject
{
    [Command(PackageIds.SpecialPasteJsonCommand)]
    internal sealed class SpecialPasteJsonCommand : BaseCommand<SpecialPasteJsonCommand>
    {
        private static IServiceProvider _serviceProvider = new ServiceCollection()
                    .AddScoped<ClassesCache>()
                    .AddScoped<CodeGenerator>()
                    .AddScoped<ClassBuilder>()
                    .AddScoped<PropertyBuilder>()
                    .BuildServiceProvider();
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            var doc = await VS.Documents.GetActiveDocumentViewAsync();
            var selection = doc?.TextView.Selection.SelectedSpans.FirstOrDefault();
            if (selection.HasValue)
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var codeGenService = scope.ServiceProvider.GetRequiredService<CodeGenerator>();

                var clipBoardText = Clipboard.GetText();

                try
                {
                    var generatedCode = codeGenService.GenerateCode(clipBoardText).ToString();
                    doc.TextBuffer.Replace(selection.Value, generatedCode);
                }
                catch (Exception ex)
                {
                    await VS.MessageBox.ShowErrorAsync("Text is not a valid json", ex.Message);
                }

                await VS.Commands.ExecuteAsync(Microsoft.VisualStudio.VSConstants.VSStd2KCmdID.FORMATDOCUMENT);
            }
        }
    }
}
