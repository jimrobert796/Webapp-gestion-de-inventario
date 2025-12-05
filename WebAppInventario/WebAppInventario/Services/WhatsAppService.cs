using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace WebAppInventario.Services
{
    public class WhatsAppService
    {
        private readonly TwilioConfig _config;

        public WhatsAppService(IOptions<TwilioConfig> config)
        {
            _config = config.Value;
            TwilioClient.Init(_config.AccountSid, _config.AuthToken);
        }

        // --- MÉTODO MODIFICADO ---
        // Ya no recibe 'numeroDestino'
        public MessageResource EnviarAlertaStockBajo(string productoNombre, int stockActual)
        {
            string mensaje = $"⚠️ ALERTA DE STOCK BAJO ⚠️\n\n" +
                            $"📦 Producto: {productoNombre}\n" +
                            $"📊 Stock actual: {stockActual} unidades\n" +
                            $"Por favor, reponer inventario.";

            var message = MessageResource.Create(
                body: mensaje,
                from: new Twilio.Types.PhoneNumber(_config.WhatsAppSandboxNumber),
                // Lee el número de admin desde la configuración (secrets.json)
                to: new Twilio.Types.PhoneNumber($"whatsapp:{_config.AdminWhatsAppNumber}")
            );

            return message;
        }
    }
}