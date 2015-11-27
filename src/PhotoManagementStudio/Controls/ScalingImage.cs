using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Catel.Logging;
using ErrorReporting;

namespace PhotoManagementStudio.Controls
{
    public class ScalingImage : Control
    {
        private Uri _uri128;
        private ImageSource _img128;
        private Uri _uri256;
        private ImageSource _img256;
        private Uri _uri512;
        private ImageSource _img512;
        private Uri _uri1024;
        private ImageSource _img1024;

        private static readonly string DocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public string ImageId
        {
            get { return GetValue(ImageIdProperty) as string; }
            set { SetValue(ImageIdProperty, value); }
        }

        public static readonly DependencyProperty ImageIdProperty = DependencyProperty.Register("ImageId", typeof(string), typeof(ScalingImage), new PropertyMetadata(OnAttributesChanged));

        public string CacheFolder
        {
            get { return GetValue(CacheFolderProperty) as string; }
            set { SetValue(CacheFolderProperty, value); }
        }

        public static string GetCacheFolder(DependencyObject d)
        {
            return d.GetValue(CacheFolderProperty) as string;
        }

        public static void SetCacheFolder(DependencyObject d, string value)
        {
            d.SetValue(CacheFolderProperty, value);
        }

        public static readonly DependencyProperty CacheFolderProperty = DependencyProperty.RegisterAttached("CacheFolder", typeof(string), typeof(ScalingImage), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnAttributesChanged));

        public string ThumbServer
        {
            get { return GetValue(ThumbServerProperty) as string; }
            set { SetValue(ThumbServerProperty, value); }
        }

        public static string GetThumbServer(DependencyObject d)
        {
            return d.GetValue(ThumbServerProperty) as string;
        }

        public static void SetThumbServer(DependencyObject d, string value)
        {
            d.SetValue(ThumbServerProperty, value);
        }

        public static readonly DependencyProperty ThumbServerProperty = DependencyProperty.RegisterAttached("ThumbServer", typeof(string), typeof(ScalingImage), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnAttributesChanged));

        private static void OnAttributesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var img = sender as ScalingImage;
            if (img == null)
            {
                return;
            }

            img.OnAttributesChanged(false);
        }

        private void OnAttributesChanged(bool delayRecursion)
        {
            if (String.IsNullOrWhiteSpace(ImageId))
            {
                return;
            }

            var path = Path.Combine(DocumentFolder, GetCacheFolder(this) ?? "");

            var imgPath = Path.Combine(path, ImageId + "_128.jpg");
            _uri128 = File.Exists(imgPath) ? new Uri(imgPath, UriKind.Absolute) : null;
            _img128 = null;

            imgPath = Path.Combine(path, ImageId + "_256.jpg");
            _uri256 = File.Exists(imgPath) ? new Uri(imgPath, UriKind.Absolute) : null;
            _img256 = null;

            imgPath = Path.Combine(path, ImageId + "_512.jpg");
            _uri512 = File.Exists(imgPath) ? new Uri(imgPath, UriKind.Absolute) : null;
            _img512 = null;

            imgPath = Path.Combine(path, ImageId + "_1024.jpg");
            _uri1024 = File.Exists(imgPath) ? new Uri(imgPath, UriKind.Absolute) : null;
            _img1024 = null;

            if (_uri128 == null && _uri256 == null && _uri512 == null && _uri1024 == null)
            {
                _uri128 = new Uri("pack://application:,,,/PlaceholderImages/Placeholder_128.jpg");
                _uri256 = new Uri("pack://application:,,,/PlaceholderImages/Placeholder_256.jpg");
                _uri512 = new Uri("pack://application:,,,/PlaceholderImages/Placeholder_512.jpg");
                _uri1024 = new Uri("pack://application:,,,/PlaceholderImages/Placeholder_1024.jpg");

                if (delayRecursion)
                {
                    new Timer((state) => Dispatcher.InvokeAsync(GetThumbFromServer), null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
                }
                else
                {
                    GetThumbFromServer();
                }
            }

            InvalidateVisual();
        }

        private async void GetThumbFromServer()
        {
            if (GetThumbServer(this) == null)
            {
                return;
            }

            var url = GetThumbServer(this) + ImageId + "?size=";
            var outputPath = Path.Combine(DocumentFolder, GetCacheFolder(this) ?? "", ImageId);

            var client = new HttpClient();
            var task128 = ProcessUrlAsync(url + "128", outputPath + "_128.jpg", client);
            var task256 = ProcessUrlAsync(url + "256", outputPath + "_256.jpg", client);
            var task512 = ProcessUrlAsync(url + "512", outputPath + "_512.jpg", client);
            var task1024 = ProcessUrlAsync(url + "1024", outputPath + "_1024.jpg", client);

            try
            {
                await Task.WhenAll(task128, task256, task512, task1024);
            }
            catch (HttpRequestException ex)
            {
				ErrorReporter.SendException(ex);
                // TODO: Swallow this or raise message for UI indicator - probably means that the server is down.
            }

            OnAttributesChanged(true);
        }

        private async static Task ProcessUrlAsync(string url, string savePath, HttpClient client)
        {
            Debug.WriteLine("Requesting " + url);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Connection", new[] { "Keep-Alive" });
            var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var stream = new MemoryStream())
                {
                    await response.Content.CopyToAsync(stream);
                    if (stream.Length > 0)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                        {
                            var bytes = new byte[stream.Length];
                            await stream.ReadAsync(bytes, 0, (int)stream.Length);
                            await fs.WriteAsync(bytes, 0, bytes.Length);
                        }
                        stream.Close();
                    }
                }
            }
        }

        private ImageSource GetCurrentSource(Size size)
        {
            var maxDimension = Math.Max(size.Width, size.Height);

            if (maxDimension <= 128 && _uri128 != null)
            {
                return _img128 ?? (_img128 = new BitmapImage(_uri128));
            }
            if (maxDimension <= 256 && _uri256 != null)
            {
                return _img256 ?? (_img256 = new BitmapImage(_uri256));
            }
            if (maxDimension <= 512 && _uri512 != null)
            {
                return _img512 ?? (_img512 = new BitmapImage(_uri512));
            }
            if (_uri1024 != null)
            {
                return _img1024 ?? (_img1024 = new BitmapImage(_uri1024));
            }
            if (_uri512 != null)
            {
                return _img512 ?? (_img512 = new BitmapImage(_uri512));
            }
            if (_uri256 != null)
            {
                return _img256 ?? (_img256 = new BitmapImage(_uri256));
            }
            if (_uri128 != null)
            {
                return _img128 ?? (_img128 = new BitmapImage(_uri128));
            }
            return null;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var src = GetCurrentSource(RenderSize);
            if (src == null)
            {
                base.OnRender(drawingContext);
                return;
            }

            drawingContext.DrawImage(src, new Rect(new Point(0, 0), RenderSize));
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var w = arrangeBounds.Width;
            var h = arrangeBounds.Height;
            var src = GetCurrentSource(arrangeBounds);
            if (src == null)
            {
                return arrangeBounds;
            }

            var ratio = src.Width / src.Height;

            var size = (w / ratio > h)
                ? new Size(h * ratio, h)
                : new Size(w, w / ratio);
            var size2 = base.ArrangeOverride(size);
            return size2;
        }
    }
}
