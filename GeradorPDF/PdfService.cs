using Model.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shared.Models;

namespace GeradorPDF
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
                        .Text("Banco Financeiro")
                        .SemiBold().FontSize(48).FontColor(Colors.Purple.Medium);

                    page.Content()
                        .PaddingVertical(20)
                        .Column(col =>
                        {
                            col.Spacing(10);

                            col.Item().Text("Comprovante de Transação").FontSize(24);

                            col.Item().Text($"Data Emissão Comprovante: {DateTime.Now}");
                            col.Item().Text($"Valor: R${transacao.Valor}");
                            col.Item().Text(@$"De: {_transacaoRepo.GetUsuarioNome(transacao.UsuarioId)} :{_transacaoRepo.GetUsuarioCpf(transacao.UsuarioId)}");
                            col.Item().Text($"Para: {_transacaoRepo.GetUsuarioNome(transacao.UsuarioRecebedorId)} :{_transacaoRepo.GetUsuarioCpf(transacao.UsuarioRecebedorId)}");
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
