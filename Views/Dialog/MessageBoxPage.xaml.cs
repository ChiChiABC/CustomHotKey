using CustomHotKey.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CustomHotKey.Views.Dialog
{
    /// <summary>
    /// DialogPage.xaml 的交互逻辑
    /// </summary>
    public partial class DialogPage : Page
    {
        private string mode = "Input";
        Models.Language.LanguageJSON Lang = Models.Language.Lang;

        DoubleAnimation dialogAnimation = new DoubleAnimation();
        DoubleAnimation backgroundAnimation = new DoubleAnimation();

        public DialogPage()
        {
            InitializeComponent();
            InitializeAnimation();

            DialogMessage.Receive += (object sender, ReceiveEventArgs e) =>
            {


                this.ok.Content = Lang.text_ok;
                this.cancel.Content = Lang.text_cancel;

                this.dialog.Focus();

                switch (e.Args[0].ToString())
                {
                    case "Input":
                        this.mode = "Input";
                        ShowInputTextDialog(e.MessageID, e.Args[1].ToString());
                        break;
                    case "Message":
                        this.mode = "Message";
                        if (e.Args.Length == 3)
                        {
                            ShowMessage(e.MessageID, e.Args[1].ToString(), e.Args[2].ToString());
                        } else
                        {
                            ShowMessage(e.MessageID, e.Args[1].ToString(), e.Args[2].ToString(), 
                                e.Args[3].ToString(), e.Args[4].ToString());
                        }
                        break;
                    default:
                        throw new Exception();
                }

                UpdateControlVisibility();

                BeginAnimation();

            };
        }
        void UpdateControlVisibility()
        {
            if (this.mode == "Input")
            {
                foreach (var item in this.dialog.Children)
                {
                    (item as UIElement).Visibility = Visibility.Visible;
                }
                this.iconRect.Visibility = Visibility.Hidden;
            }

            if (this.mode == "Message")
            {
                foreach (var item in this.dialog.Children)
                {
                    (item as UIElement).Visibility = Visibility.Hidden;
                }
                this.iconRect.Visibility = Visibility.Visible;
                this.msg.Visibility = Visibility.Visible;
                this.okCancel.Visibility = Visibility.Visible;
            }
            this.dialogBG.Visibility = Visibility.Visible;
        }

        void ShowInputTextDialog(int id, string title)
        {
            this.text.Focus();
            this.title.Text = title;

            this.ok.Click += okClick;
            this.cancel.Click += cancelClick;

            void okClick(object sender, RoutedEventArgs e)
            {
                DialogMessage.ReturnMessage(this, id, this.text.Text);
                RecoverAnimation();
                this.text.Text = "";
                this.ok.Click -= okClick;
            }
            void cancelClick(object sender, RoutedEventArgs e)
            {
                DialogMessage.ReturnMessage(this, id);
                RecoverAnimation();
                this.text.Text = "";
                this.cancel.Click -= cancelClick;
            }

        }
        void ShowMessage(int id, string msg, string icon, string okContent = "", string cancelContent = "")
        {
            this.ok.Focus();

            if (okContent != "" && cancelContent != "")
            {
                this.ok.Content = okContent;
                this.cancel.Content = cancelContent;
            }
            else
            {
                this.ok.Content = Lang.text_ok;
                this.cancel.Content = Lang.text_cancel;
            }

            if (this.FindResource(icon) != null)
            {
                this.icon.Geometry = (Geometry)this.FindResource(icon);
            }
            else this.icon.Geometry = (Geometry)this.FindResource("information");

            this.msg.Text = msg;
            this.ok.Click += okClick;
            this.cancel.Click += cancelClick;

            void okClick(object sender, RoutedEventArgs e)
            {
                RecoverAnimation();
                DialogMessage.ReturnMessage(this, id, true);
                this.ok.Click -= okClick;
            }
            void cancelClick(object sender, RoutedEventArgs e)
            {
                RecoverAnimation();
                DialogMessage.ReturnMessage(this, id, false);
                this.cancel.Click -= cancelClick;
            }

        }

        void InitializeAnimation()
        {

            dialogAnimation.From = (-60 - this.dialog.ActualHeight);
            dialogAnimation.To = 0;
            dialogAnimation.EasingFunction = new CircleEase()
            {
                EasingMode = EasingMode.EaseOut,
            };
            dialogAnimation.Duration = TimeSpan.FromMilliseconds(500);

            backgroundAnimation.From = 0;
            backgroundAnimation.To = 1;
            backgroundAnimation.Duration = TimeSpan.FromMilliseconds(200);

        }

        private void BeginAnimation()
        {
            this.background.IsHitTestVisible = true;
            this.dialogTT.BeginAnimation(TranslateTransform.YProperty, dialogAnimation);
            this.background.BeginAnimation(Grid.OpacityProperty, backgroundAnimation);
            SystemSounds.Question.Play();
        }

        private void RecoverAnimation()
        {
            this.background.IsHitTestVisible = false;
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = -60 - this.dialog.ActualHeight;
            animation.Duration = TimeSpan.FromMilliseconds(500);
            animation.EasingFunction = new CircleEase() 
            { 
                EasingMode = EasingMode.EaseIn,
            };

            DoubleAnimation animation1 = new DoubleAnimation();
            animation1.From = 1; 
            animation1.To = 0;
            animation1.Duration = TimeSpan.FromMilliseconds(200);

            this.dialogTT.BeginAnimation(TranslateTransform.YProperty, animation);
            this.background.BeginAnimation(Grid.OpacityProperty, animation1);

        }
    }
}
