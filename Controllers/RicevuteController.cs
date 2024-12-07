using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Scadenzario.Models.InputModels.Ricevute;
using Scadenzario.Models.InputModels.Scadenze;
using Scadenzario.Models.Services.Applications.Scadenze;
using Scadenzario.Models.Utility;
using Scadenzario.Models.ViewModels.Ricevute;

namespace Scadenzario.Controllers;

public class RicevuteController : Controller
{
    private readonly IRicevuteService _ricevute;
    private readonly IWebHostEnvironment _environment;
    private readonly IScadenzeService _service;
    public static List<RicevutaCreateInputModel>? Ricevute { get; private set;}
    public RicevuteController(IScadenzeService service,IRicevuteService ricevute,IWebHostEnvironment environment)
    {
        _ricevute = ricevute;
        _environment = environment;
        _service = service;
    }
    public async Task<IActionResult> FileUpload()
     {
            var id = Convert.ToInt32(TempData["IDScadenza"]);
            ScadenzaEditInputModel inputModel = new();
            inputModel = await _service.GetScadenzaForEditingAsync(id);
            var files = Request.Form.Files;
            var i = 0;
            string physicalWebRootPath = _environment.ContentRootPath;
            var path = String.Empty;
            if(OperatingSystem.IsWindows())
                path = physicalWebRootPath + "\\Upload";
            else if (OperatingSystem.IsLinux()|| OperatingSystem.IsMacOS())
                path = physicalWebRootPath + "/Upload";
            foreach (var file in files)
            {
                RicevutaCreateInputModel ricevuta = new RicevutaCreateInputModel();
                var fileName = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName;
                if (fileName != null)
                {
                    var filename = fileName
                        .Trim('"');
                    ricevuta.FileName=filename;
                    var fileType = file.ContentType;
                    var fileLenght = file.Length;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    filename = System.IO.Path.Combine(path, filename);
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        await file.CopyToAsync(fs);
                        await fs.FlushAsync();
                    }
                    i += 1;
                    ricevuta.FileType=fileType;
                    ricevuta.Path=filename;
                    ricevuta.IDScadenza=inputModel.IdScadenza;
                    ricevuta.Beneficiario=inputModel.Denominazione;
                    byte[] filedata = new byte[fileLenght];
                    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            filedata = reader.ReadBytes((int)stream.Length);
                        }
                    } 
                    ricevuta.FileContent=filedata;
                }

                AddRicevuta(ricevuta);
            }

            await SalvaRicevute();
            string message = "Upload ed inserimento effettuati correttamente!";
            JsonResult result = new JsonResult(message);
            return result;
     }
     public static void AddRicevuta(RicevutaCreateInputModel ricevuta)
     {
            if(Ricevute==null)
                Ricevute = new();
            Ricevute.Add(ricevuta);
     }
     public async Task<IActionResult> Download(int Id)
     {
         var viewModel = await _ricevute.GetRicevutaAsync(Id);
         string filename = viewModel.Path;
         if (filename == null)
             throw new Exception("File name not found");

         var path = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot", filename);

         var memory = new MemoryStream();
         using (var stream = new FileStream(path, FileMode.Open))
         {
             await stream.CopyToAsync(memory);
         }
         memory.Position = 0;
         return File(memory, Utility.GetContentType(path), Path.GetFileName(path));
     }
     public async Task<IActionResult> DeleteAllegato(int id)
     {
         RicevutaViewModel ricevutaViewModel = await _ricevute.GetRicevutaAsync(id);
         await _ricevute.DeleteRicevutaAsync(id);
         string filename = ricevutaViewModel.Path;
         if (filename == null)
             throw new ArgumentException("File name not found");
         var path = Path.Combine(
             Directory.GetCurrentDirectory(),
             "wwwroot", filename);
         System.IO.File.Delete(path);
         TempData["Message"] = "Cancellazione effettuata correttamente";
         return RedirectToAction(nameof(System.Index),"Scadenze");
     }

     public async Task<bool> SalvaRicevute()
     {
         //Gestione Ricevute
         if(Ricevute!=null)
             await _ricevute.CreateRicevutaAsync(Ricevute);
         Ricevute=null;
         return true;
     }
}