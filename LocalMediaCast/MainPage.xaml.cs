using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace LocalMediaCast
{
    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value != null)
            {
                var thumbnailStream = (IRandomAccessStream)value;
                var image = new BitmapImage();
                image.SetSource(thumbnailStream);

                return image;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility OnTrue { get; set; }
        public Visibility OnFalse { get; set; }

        public BooleanToVisibilityConverter()
        {
            OnFalse = Visibility.Collapsed;
            OnTrue = Visibility.Visible;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = (bool)value;

            return v ? OnTrue : OnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility == false)
                return DependencyProperty.UnsetValue;

            if ((Visibility)value == OnTrue)
                return true;
            else
                return false;
        }
    }

    public sealed partial class MainPage : Page
    {

        private Window projectionWindow;

        public MainPage()
        {
            this.InitializeComponent();
            var queryOptions = new QueryOptions
            {
                FolderDepth = FolderDepth.Deep,
                IndexerOption = IndexerOption.UseIndexerWhenAvailable
            };

            queryOptions.FileTypeFilter.Add(".jpg");
            queryOptions.FileTypeFilter.Add(".png");

            queryOptions.SortOrder.Clear();
            var sortEntry = new SortEntry
            {
                PropertyName = "System.FileName",
                AscendingOrder = true
            };
            queryOptions.SortOrder.Add(sortEntry);

            var fileQuery = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions);
            const uint size = 190;
            var fileInformationFactory = new FileInformationFactory(fileQuery, ThumbnailMode.PicturesView, size, ThumbnailOptions.UseCurrentScale, true);
            photosViewSource.Source = fileInformationFactory.GetVirtualizedFilesVector();
        }

        public void ClearSelection()
        {
            PhotosGrid.SelectedItem = null;
        }

        private async void PhotosGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            FileInformation photo = (FileInformation)e.ClickedItem;

            if (projectionWindow == null)
            {
                CoreApplicationView newView = CoreApplication.CreateNewView();
                int projectionView = 0;
                await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Frame frame = new Frame();
                    frame.Navigate(typeof(ProjectedPhoto), photo, new SuppressNavigationTransitionInfo());
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                    projectionWindow = Window.Current;
                    projectionView = ApplicationView.GetForCurrentView().Id;
                });
                await ProjectionManager.StartProjectingAsync(projectionView, ApplicationView.GetForCurrentView().Id);
            }
            else
            {
                await projectionWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    (projectionWindow.Content as Frame).Navigate(typeof(ProjectedPhoto), photo, new SuppressNavigationTransitionInfo());
                });
            }
        }
    }
}

