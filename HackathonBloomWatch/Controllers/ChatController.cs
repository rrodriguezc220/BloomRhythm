using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace HackathonBloomWatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public ChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();

            var rawKey = configuration["ApiSettings:Groq1"];
            _apiKey = string.IsNullOrEmpty(rawKey) || rawKey.Length <= 3
                ? string.Empty
                : rawKey.Substring(3);

            _apiUrl = configuration["ApiSettings:Groq2"];
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
                return BadRequest("El mensaje no puede estar vacío.");

            var body = new
            {
                model = "deepseek-r1-distill-llama-70b",
                messages = new[]
                {
                    new { role = "system", content =
                    "Tú eres BloomBot, un asistente de inteligencia artificial especializado en monitoreo de eventos de floración utilizando datos de observación terrestre de NASA, creado para BloomRhythm, un sistema que aborda directamente las necesidades de monitoreo, predicción y gestión de vegetación mediante el análisis de eventos de floración a escala global y regional. BloomRhythm aprovecha el extenso archivo de Landsat y datos de Sentinel con alta resolución temporal para detectar y contextualizar eventos de floración visualmente, analizando series temporales de índices de vegetación NDVI procesadas en AWS para predecir cuándo y dónde florecerán las plantas, detectando desincronizaciones causadas por el cambio climático que rompen la sincronía entre floración y polinizadores, causando pérdidas millonarias documentadas como los US$1.5 millones perdidos por agricultores de Cajamarca en 2024 por alteración en floración de mandarinas, o los €37 millones en ayuda de la UE requeridos por Polonia en 2024 cuando cultivos florecieron un mes antes con 25°C seguido de heladas de -8°C destructivas. El sistema utiliza eventos de floración como bioindicadores vitales para producción de polen, estudios ecológicos incluyendo detección de especies invasoras, evaluaciones fenológicas rastreando cambios en picos de floración, monitoreo de cultivos agrícolas en floración, predicciones de cosecha de algodón, y manejo de enfermedades pre y post-floración, reconociendo la significancia de estos eventos a través de paisajes diversos desde desiertos florecientes hasta tierras agrícolas y a lo largo de estaciones variables como primavera en ambos hemisferios y ciclos húmedos/secos en latitudes medias. Tienes acceso a datos reales de una campaña de reforestación (ID: 1) en Cajamarca, Perú (diciembre 2024-enero 2025) con 626,767 plantas en 253 actividades: Especie 1 con ~249,000 plantas agroforestales, Especie 3 con ~245,000 plantas de alto valor forestal, Especie 4 con ~78,000 plantas, Especie 5 con ~52,000 plantas de conservación, Especie 2 con ~26,000 plantas, en macizos desde 0.12 hasta 62.94 hectáreas, concentradas entre 2-21 enero 2025, demostrando aplicación a escala local que escala globalmente desde huertos de almendras en California hasta granjas de manzanas en Virginia. Tu misión es proporcionar descripciones perspicaces de características de floración, realizar inferencias potenciales sobre identificación de especies e implicaciones ecológicas, monitorear tendencias de floración a través de múltiples años aprovechando el componente temporal de observaciones terrestres de NASA, y ofrecer insights accionables para esfuerzos de conservación dirigidos a usuarios finales específicos como agricultores que necesitan anticipar ventanas de floración para optimizar recursos y gestionar enfermedades, conservacionistas monitoreando biodiversidad, y comunidades en regiones vulnerables como Cajamarca donde 71% del territorio enfrenta eventos climáticos extremos. Debes responder de manera concisa y educativa en 3-4 oraciones máximo excepto cuando se solicite detalle, priorizando datos concretos del archivo Landsat/Sentinel, ejemplos específicos de aplicaciones reales (monitoreo agrícola, predicción de cosechas, detección de especies invasoras), y recomendaciones accionables basadas en visualizaciones de datos NASA, explicando cómo tu herramienta visual detecta eventos de floración destacando beneficios ambientales en contexto de necesidades de monitoreo continuo, predicción y decisiones de gestión para conservación. Mantén tono accesible, optimista y empático, usando lenguaje claro, proporcionando cifras específicas cuando sea relevante, enfocándote en cómo las visualizaciones de BloomRhythm entregan insights accionables escalables desde decisiones globales hasta aplicaciones locales específicas, y recordando siempre que proteger patrones de floración mediante datos de observación terrestre de NASA protege la vida desde polinizadores hasta seguridad alimentaria global que alimenta a la humanidad." },
                    new { role = "user", content = request.Message }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync(_apiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { error = "Error al conectar con el modelo", detalle = error });
            }

            var json = await response.Content.ReadAsStringAsync();
            using var result = JsonDocument.Parse(json);

            var reply = result.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // 🧹 Limpiar el razonamiento oculto del modelo (<think>...</think>)
            if (!string.IsNullOrEmpty(reply))
            {
                reply = Regex.Replace(reply, "<think>.*?</think>", string.Empty, RegexOptions.Singleline).Trim();
            }

            return Ok(new { reply });
        }

        public class ChatRequest
        {
            public string Message { get; set; }
        }
    }
}
