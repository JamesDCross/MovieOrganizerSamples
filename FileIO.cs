using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MO
{
    class FileIO
    {

        //save method
        public static async Task SaveObjectToXml<T>(T objectToSave, string filename)
        {
            // stores an object in XML format in file 
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting).AsTask().ConfigureAwait(false);
            Stream stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false);

            using (stream)
            {
                serializer.Serialize(stream, objectToSave);
            }
        }

        //load method
        public static async Task<T> ReadObjectFromXmlFileAsync<T>(string filename)
        {
            // this reads XML content from a file ("filename") and returns an object  from the XML
            T objectFromXml = default(T);
            var serializer = new XmlSerializer(typeof(T));
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.GetFileAsync(filename).AsTask().ConfigureAwait(false);
            Stream stream = await file.OpenStreamForReadAsync().ConfigureAwait(false);
            objectFromXml = (T)serializer.Deserialize(stream);
            stream.Dispose();
            return objectFromXml;
        }


        //downloads pictures to cache folder
        public static async void getPic(string url, string title)
        {
            //.Replace existing //changed because getting bug,thanks microsoft
            StorageFolder picsFolder = ApplicationData.Current.LocalCacheFolder;
            StorageFile file = await picsFolder.CreateFileAsync(title = title.Replace("/", "").Replace(':', ' ') + ".jpg", CreationCollisionOption.GenerateUniqueName);
            HttpClient client = new HttpClient();
            byte[] responseBytes = await client.GetByteArrayAsync(url);
            var stream = await file.OpenAsync(FileAccessMode.ReadWrite);
            using (var outputStream = stream.GetOutputStreamAt(0))
            {
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBytes(responseBytes);
                await writer.StoreAsync();
                await outputStream.FlushAsync();
            }
        }

    }
}
