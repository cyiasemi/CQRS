using System;
using System.Linq;
using System.Text;

namespace Showcase.Services.FileImageSaver
{
    internal static class FileExtensions
    {
        public static string ToValidImageName(this string name)
        {
            var knownImageTypes = new string[] { "jpg", "gif", "jpeg", "png", "bmp", "tiff", "svg" };
            var nameArray = name.Split('.');
            var format = string.Empty;

            if (nameArray.Length > 0 && knownImageTypes.Contains(nameArray.Last().ToLower()))
                format = nameArray.Last();

            if (name.Length > 40)
            {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(format))
                    sb.Append(Guid.NewGuid()).Append(".").Append(format).Replace('-', '_');
                else
                    sb.Append(Guid.NewGuid()).Replace('-', '_');
                name = sb.ToString();
            }
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }
    }
}
