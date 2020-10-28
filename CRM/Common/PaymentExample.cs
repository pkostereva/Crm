using Newtonsoft.Json.Linq;

namespace CRM.API.Common
{
    public static class PaymentExample
    {
        public static string json = "{\"intent\":\"authorize\",\"payer\":{\"payment_method\":\"paypal\"},\"transactions\":[{\"amount\":{\"total\":\"30.11\",\"currency\":\"USD\",\"details\":{}},\"description\":\"This is the payment transaction description.\",\"custom\":\"EBAY_EMS_90048630024435\",\"invoice_number\":\"48787589673\",\"payment_options\":{\"allowed_payment_method\":\"INSTANT_FUNDING_SOURCE\"},\"soft_descriptor\":\"ECHI5786786\",\"item_list\":{\"items\":[],\"shipping_address\":{\"recipient_name\":\"Hello World\",\"line1\":\"4thFloor\"," +
            "\"line2\":\"unit#34\",\"city\":\"SAn Jose\",\"country_code\":\"US\",\"postal_code\":\"95131\",\"phone\":\"011862212345678\",\"state\":\"CA\"}}}],\"redirect_urls\":{\"return_url\":\"https:\\/\\/example.com\\/return\",\"cancel_url\":\"https:\\/\\/example.com\\/cancel\"}}";

        public static JObject o = new JObject()
        {

        };
    }
}
