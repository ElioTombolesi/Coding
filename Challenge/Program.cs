
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

class Program
{
    // Configuracion para poder avisar por mail si un sitio esta caido 

    private static readonly string smtpServer = "smtp.gmail.com";
    private static readonly int smtpPort = 587;
    private static readonly string Email = "pruebadatosccode@gmail.com";
    private static readonly string Password = "usme pwxo nqnk hjtm";
    private static readonly string Receptor = "pruebadatosccode@gmail.com";


    // Lista de url a chequear
    private static readonly List<string> urls = new List<string>
    {
        "https://google.com",
        "https://youtube.com",
        "https://facebook.com",
        "https://whatsapp.com",
        "https://instagram.com",
        "https://infobae.com",
        "https://mercadolibre.com.ar",
        "https://twitter.com",
        "https://casildaplus.com",
        "https://netflix.com",
        "https://netflasfddxaix.com"
    };


    static async Task Main(string[] args)
    {

        //corro el programa
        while (true)
        {
            Console.WriteLine($"Monitoreo iniciado a las: {DateTime.Now:dd-MM-yyyy HH:mm:ss}\n");
            await Monitoreo();
            Thread.Sleep(60000); // Se hace cada 60s
        }
    }

    //Codigo del monitoreo
    private static async Task Monitoreo()
    {
        using HttpClient client = new HttpClient();

        foreach (var url in urls)
        {
            try
            {

                var stopwatch = Stopwatch.StartNew();

                var response = await client.GetAsync(url);

                stopwatch.Stop();



                if (response.IsSuccessStatusCode)
                    if (stopwatch.ElapsedMilliseconds < 3000)
                    {
                        Console.WriteLine($"{url}  || Está activo. Tiempo de respuesta: {stopwatch.ElapsedMilliseconds} ms.");
                    }
                    else
                    {
                        Console.WriteLine($"{url}  || Está activo, pero esta demorando en cargar. Tiempo de respuesta: {stopwatch.ElapsedMilliseconds} ms.");
                        SendEmail($"El sitio {url} está demorando en cargar", $"Código de estado: {response.StatusCode}");
                    }


                else
                {
                    Console.WriteLine($"{url}  || Está inactivo. Código de respuesta: {response.StatusCode}");
                    SendEmail($"El sitio {url} está caído", $"Código de estado: {response.StatusCode}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{url} || no se pudo acceder. Error: {ex.Message}");
                SendEmail($"Error crítico al acceder al sitio {url}", $"Error: {ex.Message}");

            }



        }
        Console.WriteLine($"--- Fin del reporte {DateTime.Now:dd-MM-yyyy HH:mm:ss} ---\n");
        Console.WriteLine("------------------------\n");
    }


    // Funcion para enviar el mail de alerta
    private static void SendEmail(string asunto, string correo)
    {
        try
        {
            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(Email, Password),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(Email),
                Subject = asunto,
                Body = correo,
                IsBodyHtml = false
            };
            mail.To.Add(Receptor);

            smtpClient.Send(mail);

            Console.WriteLine("Mail de alerta enviado.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar el correo: {ex.Message}");
        }
    }


}

