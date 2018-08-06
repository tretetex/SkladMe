using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using SkladMe.Infrastructure;

namespace SkladMe.Controls
{
    class CookieMonster : ICookieVisitor
    {
        readonly List<Cookie> cookies = new List<Cookie>();
        readonly Action<IEnumerable<Cookie>> useAllCookies;

        public CookieMonster(Action<IEnumerable<Cookie>> useAllCookies)
        {
            this.useAllCookies = useAllCookies;
        }

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            cookies.Add(cookie);

            if (count == total - 1)
                useAllCookies(cookies);

            return true;
        }

        public void Dispose()
        {
        }
    }

    public class CustomMenuHandler : CefSharp.IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            model.Clear();
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {

            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }

    public partial class Browser : UserControl
    {
        public Browser()
        {
            InitializeComponent();
            browser.MenuHandler = new CustomMenuHandler();
            browser.FrameLoadEnd += BrowserOnFrameLoadEnd;
            
        }

        private void BrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            var manager = Cef.GetGlobalCookieManager();

            var visitor = new CookieMonster(allCookies =>
            {
                if (allCookies.Any(c => c.Name == "__cfduid"))
                {
                    Cookies.AllCookies.Clear();

                    foreach (var visitorCookie in allCookies)
                    {
                        var cookie = new System.Net.Cookie
                        {
                            Name = visitorCookie.Name,
                            Domain = visitorCookie.Domain,
                            Value = visitorCookie.Value
                        };

                        Cookies.AllCookies.Add(cookie);
                    }

                }
            });

            manager.VisitAllCookies(visitor);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            browser.Load(browser.Address);
        }
    }
}