using Microsoft.AspNetCore.Mvc;
using FacturacionElectronica.Models;
using FacturacionElectronica.Services;
using System.Threading.Tasks;

namespace FacturacionElectronica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicingController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoicingController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendInvoice([FromBody] Invoice invoice)
        {
            if (invoice == null || invoice.Customer == null || string.IsNullOrEmpty(invoice.Customer.Email))
                return BadRequest("Datos de factura o cliente incompletos.");

            bool result = await _invoiceService.SendInvoiceEmail(invoice);

            if (result)
                return Ok("Factura enviada correctamente por correo electrónico.");
            else
                return StatusCode(500, "Error al enviar la factura por correo.");
        }
    }
}