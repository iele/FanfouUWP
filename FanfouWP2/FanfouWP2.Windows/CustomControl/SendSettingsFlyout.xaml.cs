using FanfouWP2.FanfouAPI;
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

namespace FanfouWP2.CustomControl
{
    public sealed partial class SendSettingsFlyout : SettingsFlyout
    {
        public delegate void StatusUpdateSuccessHandler(object sender, EventArgs e);
        public delegate void StatusUpdateFailedHandler(object sender, FailedEventArgs e);

        public event StatusUpdateSuccessHandler StatusUpdateSuccess;
        public event StatusUpdateFailedHandler StatusUpdateFailed;

        public enum SendMode { Normal, Reply, Repose, ReplyUser };
        private SendMode mode;
        private Item data;

        public SendSettingsFlyout()
        {
            this.InitializeComponent();

            FanfouAPI.FanfouAPI.Instance.StatusUpdateSuccess += Instance_StatusUpdateSuccess;
            FanfouAPI.FanfouAPI.Instance.StatusUpdateFailed += Instance_StatusUpdateFailed;
        }

        public void ChangeMode(SendMode mode, Item data = null)
        {
            this.mode = mode;
            this.data = data;

            if (mode == SendMode.Reply)
            {
                this.Title = "回复消息";
                this.send.Text = "@" + (data as Status).user.screen_name + " ";
            }
            else if (mode == SendMode.Repose)
            {
                this.Title = "转发消息";
                var text = "转：@" + (data as Status).user.screen_name + " " + (data as Status).text;
                if (text.Length <= 140)
                    this.send.Text = text;
                else
                    this.send.Text = text.Substring(0, 140);
            }
            else if (mode == SendMode.ReplyUser)
            {
                this.Title = "回复 " + (data as User).screen_name;

                if (!this.send.Text.StartsWith("@" + (data as User).screen_name))
                    this.Title = "@" + (data as User).screen_name + " ";
                else {                 
                }
            }
            else
            {
                this.Title = "发送新消息";
            }
        }

        void Instance_StatusUpdateFailed(object sender, FanfouAPI.FailedEventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateFailed != null)
            {
                StatusUpdateFailed(this, e);
            }
        }

        void Instance_StatusUpdateSuccess(object sender, EventArgs e)
        {
            loading.Visibility = Visibility.Collapsed;
            if (StatusUpdateSuccess != null)
            {
                StatusUpdateSuccess(this, e);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.send.Text.Length > 0 && this.send.Text.Length <= 140)
            {
                loading.Visibility = Visibility.Visible;
                if (mode == SendMode.Reply)
                {
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_status_id: (data as Status).id);
                }
                else if (mode == SendMode.Repose)
                {
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, repost_status_id: (data as Status).id);
                }
                else if (mode == SendMode.ReplyUser)
                {
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text, in_reply_to_user_id: (data as User).id);
                }
                else
                {
                    FanfouAPI.FanfouAPI.Instance.StatusUpdate(this.send.Text);
                }
            }
        }
    }
}
