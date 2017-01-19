using System.IO;
namespace MyMindV3
{
    public interface IContent
    {
        string ContentDirectory();

        string PicturesDirectory();

        bool FileExists(string filename);

        void StoreFile(string id, Stream str);

        void StoreImageFile(string id, Stream str, bool isUser);

        void LaunchFile(string filename);

        long FileSize(string filename);

        byte[] LoadedFile(string filename);
    }
}

