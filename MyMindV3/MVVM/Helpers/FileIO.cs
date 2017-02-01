using PCLStorage;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

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
            try
            {
                var fs = FileSystem.Current;
                await fs.LocalStorage.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting).ContinueWith(async (t) =>
                {
                    if (t.IsCompleted)
                    {
                        var bytes = ReadFully(data);
                        var file = t.Result;
                        using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}--{1}", ex.Message, ex.InnerException);
            }
        }

        public async Task<Stream> LoadFile(string filename)
        {
            var file = await FileSystem.Current.LocalStorage.GetFileAsync(filename);
            //load stream to buffer  
            using (var stream = await file.OpenAsync(FileAccess.ReadAndWrite))
            {
                var length = stream.Length;
                var streamBuffer = new byte[length];
                stream.Read(streamBuffer, 0, (int)length);
                return new MemoryStream(streamBuffer);
            }
        }

        public bool FileExists(string filename)
        {
            var res = FileSystem.Current.LocalStorage.CheckExistsAsync(filename).Result;
            return res == ExistenceCheckResult.FileExists;
        }
    }
}
