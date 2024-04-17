using Firebase.Auth;
using Firebase.Storage;
using FireBase.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FireBase.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            // Leemos el archivo subido
            Stream archivoASubir = archivo.OpenReadStream();

            // Configuramos la conexión hacia Firebase
            string email = "soymariohdez@gmail.com";
            string clave = "catolica";
            string ruta = "practica09-daw.appspot.com";
            string api_key = "AIzaSyDoGt86tnnpLevO8D25gh1ig3_1KI_HQW0";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var autenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);
            var cancellation = new CancellationTokenSource();
            var tokenUser = autenticarFireBase.FirebaseToken;

            // Subir archivo a Firebase
            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                                        new FirebaseStorageOptions
                                                        {
                                                            AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                                            ThrowOnCancel = true
                                                        }
                                                        ).Child("Archivos").Child(archivo.FileName).PutAsync(archivoASubir, cancellation.Token);

            // Esperar a que se complete la tarea de cargar el archivo
            var urlArchivoCargado = await tareaCargarArchivo;

            // Redirigir a la página "VerImagen"
            return RedirectToAction("VerImagen", new { urlImagen = urlArchivoCargado });
        }


       

        [HttpGet]
        public ActionResult VerImagen(string urlImagen)
        {
            // Puedes hacer cualquier procesamiento adicional si es necesario
            // Por ejemplo, validar el URL de la imagen

            // Puedes pasar el URL de la imagen a la vista como parte del modelo
            return View((object)urlImagen); // Puedes pasar el URL de la imagen como parte del modelo
        }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
