using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace WindowsFormsApplication1
{
    public partial class ScreenShot : Form
    {
        private WebBrowser wb;
        private Bitmap jpg;

        public ScreenShot()
        {
            InitializeComponent();
        }

        private void ScreenShot_Load(object sender, EventArgs e)
        {
            ActiveControl = UserName;
        }

        private void EnableStuff()
        {
            button1.Enabled = true;
            printToolStripMenuItem.Enabled = true;
            alertToolStripMenuItem.Enabled = true;
        }

        private void DisableStuff()
        {
            button1.Enabled = false;
            printToolStripMenuItem.Enabled = false;
            alertToolStripMenuItem.Enabled = false;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wb = sender as WebBrowser;
            urlTb.Text = wb.Url.AbsoluteUri;
            EnableStuff();
        }

        private void Go_Click(object sender, EventArgs e)
        {
            string address = "";
            if (!urlTb.Text.ToString().StartsWith("http://") && !urlTb.Text.ToString().StartsWith("https://"))
            {
                address = "http://" + urlTb.Text.ToString();
            }
            else
            {
                address = urlTb.Text.ToString();
            }

            DisableStuff();
            wb.Url = new Uri(address);
            wb.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
        }

        private void urlTb_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.ToString() == "Return")
            {
                DisableStuff();
                wb.Url = new Uri(urlTb.Text.ToString());
                wb.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
                wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(this.webBrowser_DocumentCompleted);
            }
        }
        
        public System.Drawing.Bitmap CaptureWebPage(string URL)
        {
            WebBrowser web = new WebBrowser();
            web.ScrollBarsEnabled = false;
            web.ScriptErrorsSuppressed = true;
            web.Navigate(URL);

            while (web.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();
            Thread.Sleep(1500);

            web.Width = wb.Document.Body.ScrollRectangle.Width + 50;
            web.Height = wb.Document.Body.ScrollRectangle.Height + 50;

            Bitmap bmp = new Bitmap(web.Width, web.Height);
            web.DrawToBitmap(bmp, new Rectangle(0, 0, web.Width, web.Height));

            return bmp;
        }

        private void ScreenShot_Resize(object sender, EventArgs e)
        {
            if (webBrowser != null)
            {
                webBrowser.Width = this.Width - 19;
                webBrowser.Height = this.Height - (webBrowser.Location.Y * 2) + 18;
            }

            if (urlTb != null)
            {
                urlTb.Width = this.Width - urlTb.Location.X - button1.Width - 26;
                button1.SetBounds(urlTb.Width + urlTb.Location.X + 5, button1.Location.Y, button1.Width, button1.Height);
            }
        }


        private void takeScreenShotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wb != null)
            {
                jpg = CaptureWebPage(wb.Url.AbsoluteUri.ToString());

                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        jpg.Save(saveFileDialog1.FileName, ImageFormat.Jpeg);
                        jpg = null;
                    }                    
                }                
            } 
        }

        private void alertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < wb.Document.Forms.Count; i++)
            {
                foreach (HtmlElement g in wb.Document.Forms[i].Children)
                {
                    HtmlElement forms = g;
                    for (int t = 0; t < forms.GetElementsByTagName("input").Count; t++)
                    {
                        wb.Document.Window.Alert(forms.GetElementsByTagName("input")[t].GetAttribute("type").ToString() + " | " + forms.GetElementsByTagName("input")[t].GetAttribute("name").ToString());//forms.GetElementsByTagName("input")[t].GetAttribute("value").ToString()
                    }
                    if (forms.TagName == "INPUT")
                    {
                        //wb.Document.Window.Alert(forms.GetAttribute("value") +" | "+ forms.InnerText);
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wb.ShowPrintDialog();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void forwardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }

        private void login_Click(object sender, EventArgs e)
        {
            loginContainer.Visible = false;
            //webBrowser.Navigate(url.AbsoluteUri + "?user[]=JAYAF&pass[]=test");
            byte[] d = ASCIIEncoding.ASCII.GetBytes("jay=true&user=" + UserName.Text + "&pass=" + PassWord.Text);
            webBrowser.Navigate(url.AbsoluteUri, "", d, "Content-Type: application/x-www-form-urlencoded" + "\n" + "\r");
        }
    }
}
