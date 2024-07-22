using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZipDetector.Utils
{
    internal static class ZipDetectorUtil
    {
        private static readonly byte[] ZipHead = [0x50, 0x4B, 0x03, 0x04];

        public static async Task<string> DetectAsync(string path, string outPath)
        {
            if (!Path.Exists(path))
            {
                return "target file not exists!";
            }

            var data = await File.ReadAllBytesAsync(path);

            for (int i = 0; i < data.Length - 4; ++i)
            {
                if (data.AsSpan(i, 4).SequenceEqual(ZipHead))
                {
                    var zipdata = data.Skip(i).ToArray();
                    await File.WriteAllBytesAsync(outPath, zipdata);

                    return "completed.";
                }
            }

            return "cannot detect any zip head!";
        }
    }
}
