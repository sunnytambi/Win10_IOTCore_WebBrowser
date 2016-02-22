using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Win10_IOTCore_WebBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Navigate(string url)
        {
            wv.Source = new Uri(url);
        }

        private void GoBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigate(UrlBar.Text);
        }

        private void UrlBar_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Navigate(UrlBar.Text);
            }
        }

        private void wv_ContentLoading(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            LoadingTxt.Visibility = Visibility.Visible;
            Progress.Visibility = Visibility.Visible;
        }

        private void wv_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            LoadingTxt.Visibility = Visibility.Collapsed;
            Progress.Visibility = Visibility.Collapsed;
            if (args.Uri != null)
            {
                UrlBar.Text = args.Uri.ToString();
            }
        }
    }
}
