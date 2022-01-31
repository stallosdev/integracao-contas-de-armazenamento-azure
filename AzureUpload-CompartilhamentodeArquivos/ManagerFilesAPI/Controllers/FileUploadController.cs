using ManagerFilesAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerFilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration _configurationn;

        public FileUploadController(IConfiguration configuration) =>
            _configurationn = configuration;

        [HttpGet]
        public List<FileInfoWeb> ListarArquivos(string path = null)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;

            if (!string.IsNullOrEmpty(path))
                fullPath = Path.Combine(fullPath, path);

            if (!Directory.Exists(fullPath))
                throw new InvalidOperationException("Diretorio não existe");

            DirectoryInfo diretorio = new DirectoryInfo(fullPath);

            FileInfo[] Arquivos = diretorio.GetFiles("*.*");

            return Arquivos.Select(t => new FileInfoWeb
            {
                Path = fullPath,
                Extension = t.Extension,
                FileName = t.Name,
                SizeFile = t.Length
            }).ToList();
        }

        [HttpPost]
        public async Task<bool> UploadAsync(List<IFormFile> formFile, string path = null)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;

            if (!string.IsNullOrEmpty(path))
                fullPath = Path.Combine(fullPath, path);

            foreach (var item in formFile)
            {
                if (item.Length > 0)
                {
                    string filename = Path.Combine(fullPath, item.FileName);
                    using (var stream = System.IO.File.Create(filename))
                    {
                        await item.CopyToAsync(stream);
                    }
                }
            }

            return true;
        }

        [HttpDelete]
        public bool Delete(string filename, string path = null)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;

            if (!string.IsNullOrEmpty(path))
                fullPath = Path.Combine(fullPath, path);

            System.IO.File.Delete(Path.Combine(fullPath, filename));

            return true;
        }
    }
}
