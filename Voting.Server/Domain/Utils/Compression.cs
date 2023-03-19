using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Voting.Server.Domain.Utils;
internal class Compression
{
    public string Compress(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        using MemoryStream ms = new MemoryStream();
        using (BrotliStream brotli = new BrotliStream(ms, CompressionLevel.Optimal))
        {
            brotli.Write(bytes, 0, bytes.Length);
        }

        byte[] compressed = ms.ToArray();
        return Convert.ToBase64String(compressed);
    }

    public string Decompress(string base64)
    {
        byte[] compressed = Convert.FromBase64String(base64);
        try
        {
            using MemoryStream ms = new MemoryStream(compressed);
            using BrotliStream brotli = new BrotliStream(ms, CompressionMode.Decompress);
            using StreamReader reader = new StreamReader(brotli, Encoding.UTF8);
            using (JsonDocument document = JsonDocument.Parse(reader.ReadToEnd()))
            {
                return document.RootElement.GetRawText();
            }
        }
        catch
        {
            return "";
        }
    }
}
