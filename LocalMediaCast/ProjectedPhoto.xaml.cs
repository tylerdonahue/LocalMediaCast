using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.BulkAccess;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace LocalMediaCast
{
    public sealed partial class ProjectedPhoto : Page
    {
        public ProjectedPhoto()
        {
            this.InitializeComponent();
        }

        private async Task<BitmapImage> LoadImage(FileInformation file)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                bitmapImage.SetSource(fileStream);
            }
            return bitmapImage;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            FileInformation file = e.Parameter as FileInformation;
            if (file != null)
            {
                photo.Source = await LoadImage(file);
            }

            Uri uri = e.Parameter as Uri;
            if (uri != null)
            {
                photo.Source = new BitmapImage(uri);
            }
        }
    }
}
