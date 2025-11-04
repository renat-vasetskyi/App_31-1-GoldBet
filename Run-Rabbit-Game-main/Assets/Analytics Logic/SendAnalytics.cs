using System.Collections.Generic;
using Gpm.WebView;

namespace Analytics_Logic
{
    public class SendAnalytics
    {
        public void Send(string to)
        {
            GpmWebView.ShowUrl(
                to,
                new GpmWebViewRequest.Configuration()
                {
                    style = GpmWebViewStyle.FULLSCREEN,
                    orientation = GpmOrientation.UNSPECIFIED,
                    isClearCookie = false,
                    isClearCache = false,
                    backgroundColor = "#000000",
                    isNavigationBarVisible = true,
                    navigationBarColor = "#4B96E6",
                    title = string.Empty,
                    isBackButtonVisible = true,
                    isForwardButtonVisible = true,
                    isCloseButtonVisible = false,
                    supportMultipleWindows = true,
                    contentMode = GpmWebViewContentMode.MOBILE
                },
                OnCallback,
                new List<string>()
                {
                    "USER_ CUSTOM_SCHEME"
                });
        }

        private void OnCallback(GpmWebViewCallback.CallbackType type, string data, GpmWebViewError error)
        {

        }
    }
}