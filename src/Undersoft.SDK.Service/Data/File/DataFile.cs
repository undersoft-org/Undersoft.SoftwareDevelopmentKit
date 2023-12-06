
using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Undersoft.SDK.Service.Data.File.Container;

namespace Undersoft.SDK.Service.Data.File
{
    public class DataFile : FileContainer, IFormFile
    {
        private IFormFile _formFile;
        private Stream _stream;

        public DataFile(FileContainer container, string filename) : base(container.ContainerName)
        {
            var task = GetOrNullAsync(filename);
            task.Wait();
            _stream = task.Result;
            _formFile = new FormFile(_stream, 0, _stream.Length, filename.Split('.')[0], filename);
        }
        public DataFile(string containerName, string filename) : base(containerName)
        {
            var task = GetOrNullAsync(filename);
            task.Wait();
            _stream = task.Result;
            _formFile = new FormFile(_stream, 0, _stream.Length, filename.Split('.')[0], filename);
        }
        public DataFile(string path) : base(Path.GetDirectoryName(path))
        {
            var filename = Path.GetFileName(path);
            var task = GetOrNullAsync(filename);
            task.Wait();
            _stream = task.Result;
            _formFile = new FormFile(_stream, 0, _stream.Length, filename.Split('.')[0], filename);
        }

        public Stream Stream => _stream;

        public string ContentType => _formFile.ContentType;

        public string ContentDisposition => _formFile.ContentDisposition;

        public IHeaderDictionary Headers => _formFile.Headers;

        public long Length => _formFile.Length;

        public string Name => _formFile.Name;

        public string FileName => _formFile.FileName;

        public void CopyTo(Stream target)
        {
            _formFile.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return _formFile.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            return _formFile.OpenReadStream();
        }
    }
}
