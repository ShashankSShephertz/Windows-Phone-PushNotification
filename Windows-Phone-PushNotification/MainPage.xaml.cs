using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Notification;
using System.Text;
using com.shephertz.app42.paas.sdk.windows;
using com.shephertz.app42.paas.sdk.windows.push;

namespace Windows_Phone_PushNotification
{
    public partial class MainPage : PhoneApplicationPage, App42Callback
    {

        ServiceAPI sp = new ServiceAPI("Your APIKey", "Your SecretKey");
        PushNotificationService pushObj = null;
		String userId = "Your User";
        public MainPage()
        {
             HttpNotificationChannel channel;

             pushObj = sp.BuildPushNotificationService();

             String channelName = "App42PushNotification";

            InitializeComponent();

            channel = HttpNotificationChannel.Find(channelName);

            if (channel == null)
            {
                channel = new HttpNotificationChannel(channelName);

                channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                
                channel.Open();

                channel.BindToShellToast();

            }
            else
            {
                channel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                channel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                channel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                StoreURIWithApp42(channel.ChannelUri.ToString());

            }
        }

       void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            StoreURIWithApp42(e.ChannelUri.ToString());
        }

        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("error occurred.",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }

        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("App42 Notification {0}:\n", DateTime.Now.ToShortTimeString());

             foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));

        }

        void StoreURIWithApp42(String ChannelUri) 
        {
            pushObj.StoreDeviceToken(userId, ChannelUri, this);
        }

        void App42Callback.OnException(App42Exception exception)
        {
            // here Exception is handled
            Console.WriteLine(exception.ToString());
        }

        void App42Callback.OnSuccess(object response)
        {
            // here success is shown
            Console.WriteLine(response.ToString());
       
        }
       
    }
}
