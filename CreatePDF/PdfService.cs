using Shared.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Model.Domain;

namespace CreatePDF
{
    public class PdfService
    {
        private readonly TransacaoRepo _transacaoRepo;
        public PdfService(TransacaoRepo transacaoRepo)
        {
            _transacaoRepo = transacaoRepo;
        }
        public byte[] GerarComprovante(TransacaoDTO transacao)
        {
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Comprovante de Transação")
                        .SemiBold().FontSize(20).FontColor(Colors.Purple.Medium);

                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text($"Data Emissão Comprovante: {DateTime.Now}");
                            col.Item().Text($"Valor: {transacao.Valor}");
                            col.Item().Text(@$"De: {_transacaoRepo.GetUsuarioNome(transacao.UsuarioId)} :
                                    {_transacaoRepo.GetUsuarioCpf(transacao.UsuarioId)}");
                            col.Item().Text($"Para: {transacao.UsuarioRecebedorCnpj}");
                        });
                    page.Footer()
                        .AlignCenter()
                        .Text("Banco Financeiro © 2025");
                });
            });
            return pdf.GeneratePdf();
        }
    }
}
