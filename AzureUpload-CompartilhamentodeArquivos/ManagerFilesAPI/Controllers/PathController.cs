using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManagerFilesAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ManagerFilesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PathController : ControllerBase
    {
        private readonly IConfiguration _configurationn;

        public PathController(IConfiguration configuration) =>
            this._configurationn = configuration;

        [HttpGet]
        public List<PathCloudInfo> Paths([FromQuery]string path = null)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;

            if (!string.IsNullOrEmpty(path))
                fullPath = Path.Combine(fullPath, path);

            string[] directories = Directory.GetDirectories(fullPath);

            return directories.Select(dir => new PathCloudInfo
            {
                PathName = Path.Combine(fullPath, dir)
            }).ToList();
        }

        [HttpPost]
        public bool CreatePath([FromQuery] string pathName)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;
            fullPath = Path.Combine(fullPath, pathName);

            if (Directory.Exists(fullPath))
                return false;

            Directory.CreateDirectory(fullPath);

            return true;
        }

        [HttpDelete]
        public bool DeletePath([FromQuery] string pathName)
        {
            string fullPath = _configurationn.GetSection("Unidade").Value;
            fullPath = Path.Combine(fullPath, pathName);

            if (!Directory.Exists(fullPath))
                return false;

            Directory.Delete(fullPath);

            return true;
        }
    }
}
