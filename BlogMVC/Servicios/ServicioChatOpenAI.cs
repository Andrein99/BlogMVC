using BlogMVC.Configuraciones;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace BlogMVC.Servicios
{
    public class ServicioChatOpenAI : IServicioChat
    {
        private readonly IOptions<ConfiguracionesIA> options;
        private readonly OpenAIClient openAIClient;

        private string systemPromptGenerarCuerpo = """
            Eres un ingeniero de software experto en ASP.NET Core.
            Escribes artículos con un tono jovial y amigable.
            Te esfuerzas para que los productos de software sean fáciles de entender y usar, dando ejemplos prácticos.
            """;

        private string ObtenerPromptGeneraCuerpo(string titulo) => $"""
            Genera el cuerpo de un artículo de blog basado en el siguiente título: "{titulo}".
            El artículo debe tener al menos 500 palabras y estar estructurado con introducción, desarrollo y conclusión.
            Usa un lenguaje claro y accesible, incluyendo ejemplos prácticos relacionados con ASP.NET Core.
            Puedes añadir tipos útiles y mejores prácticas para los desarrolladores.
            El formato de respuesta es HTML. Por tanto, debes colocar negritas donde consideres títulos, subtítulos, entre otras cosas que ayuden a resaltar el formato, y den riqueza a la explicación del tema.

            La respuesta no debe ser un documento HTML completo, sino sólo el artículo en formato HTML con sus párrafos bien separados. Por tanto, no deben incluir DOCTYPE, ni head, ni body. Sólo el artículo.
            No incluyas el título del artículo en la respuesta.
            """;

        public ServicioChatOpenAI(IOptions<ConfiguracionesIA> options, OpenAIClient openAIClient)
        {
            this.options = options;
            this.openAIClient = openAIClient;
        }

        public async Task<string> GenerarCuerpo(string titulo)
        {
            var modeloTexto = options.Value.ModeloTexto;
            var clienteChat = openAIClient.GetChatClient(modeloTexto);

            var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);
            var promptUsuario = ObtenerPromptGeneraCuerpo(titulo);
            var mensajeDeUsuario = new UserChatMessage(promptUsuario);

            ChatMessage[] mensajes = { mensajeDeSistema, mensajeDeUsuario };
            var respuesta = await clienteChat.CompleteChatAsync(mensajes);
            var cuerpo = respuesta.Value.Content[0].Text;
            return cuerpo;
        }

        public async IAsyncEnumerable<string> GenerarCuerpoStream(string titulo)
        {
            var modeloTexto = options.Value.ModeloTexto;
            var clienteChat = openAIClient.GetChatClient(modeloTexto);

            var mensajeDeSistema = new SystemChatMessage(systemPromptGenerarCuerpo);
            var promptUsuario = ObtenerPromptGeneraCuerpo(titulo);
            var mensajeDeUsuario = new UserChatMessage(promptUsuario);
            ChatMessage[] mensajes = { mensajeDeSistema, mensajeDeUsuario };
            
            await foreach (var completionUpdate in clienteChat.CompleteChatStreamingAsync(mensajes))
            {
                foreach (var contenido in completionUpdate.ContentUpdate)
                {
                    yield return contenido.Text;
                }
            }
        }
    }
}
