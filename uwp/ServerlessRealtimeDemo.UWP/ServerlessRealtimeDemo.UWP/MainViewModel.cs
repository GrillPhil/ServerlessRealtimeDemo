using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Notifications;

namespace ServerlessRealtimeDemo.UWP
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SignalRService _signalRService = new SignalRService();

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand SendCommand { get; private set; }

        public MainViewModel()
        {
            Init();
            SendCommand = new RelayCommand(Send);
        }

        private async void Init()
        {
            IsLoading = true;
            await _signalRService.Init();
            _signalRService.MessageReceived += _signalRService_MessageReceived;
            IsLoading = false;
        }

        private void _signalRService_MessageReceived(object sender, string e)
        {
            SendToast(e);
        }

        private async void Send()
        {
            await _signalRService.Send(_message);
        }

        private void SendToast(string message)
        {
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = message
                        }
                    }
                }
            };
            var toastContent = new ToastContent()
            {
                Visual = visual
            };
            var toast = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
