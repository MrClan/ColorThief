using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace ColorThiefDotNet.Forms
{
    public class ColorThiefImplementation : IColorThief
    {
        private readonly ColorThief ct = new ColorThief();

        public async Task<QuantizedColor> GetColor(ImageSource sourceImage, int quality = ColorThief.DefaultQuality, bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return await ct.GetColor(await GetImageFromImageSource(sourceImage), quality, ignoreWhite);
        }

        public async Task<List<QuantizedColor>> GetPalette(ImageSource sourceImage, int colorCount = ColorThief.DefaultColorCount, int quality = ColorThief.DefaultQuality,
            bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return await ct.GetPalette(await GetImageFromImageSource(sourceImage), colorCount, quality, ignoreWhite);
        }

        private async Task<BitmapDecoder> GetImageFromImageSource(ImageSource imageSource)
        {
            IImageSourceHandler handler;

            if(imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if(imageSource is StreamImageSource)
            {
                handler = new StreamImageSourceHandler();
            }
            else if(imageSource is UriImageSource)
            {
                handler = new UriImageSourceHandler();
            }
            else
            {
                throw new NotImplementedException();
            }

            var originalBitmap = await handler.LoadImageAsync(imageSource) as BitmapImage;

            RandomAccessStreamReference rasr = RandomAccessStreamReference.CreateFromUri(originalBitmap.UriSource);

            using(IRandomAccessStreamWithContentType streamWithContent = await rasr.OpenReadAsync())
            {
                return await BitmapDecoder.CreateAsync(streamWithContent);
            }
        }
    }
}