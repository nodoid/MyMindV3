using PCLStorage;
using System.IO;
using System.Threading.Tasks;

namespace MvvmFramework.Helpers
{
    class FileIO
    {
        IFileSystem Current { get; }
        IFolder CurrentFolder { get { return Current.LocalStorage; } }
        IFile File { get; }

        byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public async Task SaveFile(string filename, Stream data)
        {
            await Current.LocalStorage.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting).ContinueWith(async (t) =>
            {
                if (t.IsCompleted)
                {
                    var bytes = ReadFully(data);
                    using (var stream = await File.OpenAsync(FileAccess.ReadAndWrite))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            });
        }

        public async Task<Stream> LoadFile(string filename)
        {
            var file = await CurrentFolder.GetFileAsync(filename);
            //load stream to buffer  
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                var length = stream.Length;
                var streamBuffer = new byte[length];
                stream.Read(streamBuffer, 0, (int)length);
                return new MemoryStream(streamBuffer);
            }
        }
    }
}
