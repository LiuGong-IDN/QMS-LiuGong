using System;
using System.Drawing;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using SkiaSharp;
using Svg.Skia;

namespace QCard.ManagementSystem.Web.Helpers
{
    public static class SvgHelper
    {
        public static string SvgToBase64Png(string svgPath, int width = 400, int height = 250)
        {
            if (!File.Exists(svgPath))
                return "";

            var svg = new SKSvg();
            using (var stream = File.OpenRead(svgPath))
            {
                svg.Load(stream);
            }

            var bitmap = new SKBitmap(width, height);
            var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.Transparent);
            float scaleX = width / svg.Picture.CullRect.Width;
            float scaleY = height / svg.Picture.CullRect.Height;
            canvas.Scale(scaleX, scaleY);
            canvas.DrawPicture(svg.Picture);
            canvas.Flush();

            using (var img = SKImage.FromBitmap(bitmap))
            using (var ms = new MemoryStream())
            {
                img.Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
