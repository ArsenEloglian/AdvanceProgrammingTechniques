﻿using EAGetMail;
using gra.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace gra
{
    public partial class GetMail : Form
    {
        void InitializeComponentHere()
        {
            Icon = Program.żabaIcon;
            btnSaveAttachmentsToDesktop.BackgroundImage = Resources.saveAttachments;
            btnSaveAttachmentsToDesktop.BackgroundImageLayout= ImageLayout.Stretch;
            btnInbox.BackgroundImage = Resources.inbox;
            btnSent.BackgroundImage = Resources.sent;
            btnSpam.BackgroundImage = Resources.spam;
            btnTrash.BackgroundImage = Resources.trash;
            btnResize.BackgroundImage = Resources.down_arrow;
            btnKosz.BackgroundImage = Resources.kosz;
            btnBulb.BackgroundImage = Resources.bulb;
            btnRead.BackgroundImage = Resources.read;
            btnUnread.BackgroundImage = Resources.unread;
            btnReplyFrom.BackgroundImage = Resources.reply;
            btnReplyTo.BackgroundImage = Resources.reply;
            btnShouldShowTagInfo.BackgroundImage = Resources.noTagInfo;
            btnGromadzone.BackgroundImage = Resources.gromadź;
            btnUsuńTeczkę.BackgroundImage = Resources.usuńTeczkę;
            btnPrzeńeś.BackgroundImage = Resources.pżeńeś;
            btnInbox.BackgroundImageLayout = ImageLayout.Stretch;
            btnSent.BackgroundImageLayout = ImageLayout.Stretch;
            btnSpam.BackgroundImageLayout = ImageLayout.Stretch;
            btnTrash.BackgroundImageLayout = ImageLayout.Stretch;
            btnResize.BackgroundImageLayout = ImageLayout.Stretch;
            btnKosz.BackgroundImageLayout = ImageLayout.Stretch;
            btnBulb.BackgroundImageLayout = ImageLayout.Stretch;
            btnRead.BackgroundImageLayout = ImageLayout.Stretch;
            btnUnread.BackgroundImageLayout = ImageLayout.Stretch;
            btnReplyFrom.BackgroundImageLayout = ImageLayout.Stretch;
            btnReplyTo.BackgroundImageLayout = ImageLayout.Stretch;
            btnShouldShowTagInfo.BackgroundImageLayout = ImageLayout.Stretch;
            btnGromadzone.BackgroundImageLayout= ImageLayout.Stretch;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            Cursor = new Cursor(Resources.osoitin.Handle);
            btnBulb.Cursor = Cursor;
            btnInbox.Cursor = Cursor;
            btnResize.Cursor = Cursor;
            btnRead.Cursor = Cursor;
            btnKosz.Cursor = Cursor;
            btnSaveAttachmentsToDesktop.Cursor = Cursor;
            btnUnread.Cursor = Cursor;
            btnTrash.Cursor = Cursor;
            btnSpam.Cursor = Cursor;
            btnSent.Cursor = Cursor;
            btnReplyFrom.Cursor = Cursor;
            btnReplyTo.Cursor = Cursor;
            btnShouldShowTagInfo.Cursor = Cursor;
            btnGromadzone.Cursor = Cursor;
            btnUsuńTeczkę.Cursor = Cursor;
            btnPrzeńeś.Cursor = Cursor;
            cmbAttachments.Cursor= Cursor;
            cmbFolders.Cursor= Cursor;
            cmbTO.Cursor = Cursor;
            listBox1.Cursor = Cursor;
            tabControl1.Cursor = Cursor;
            tableLayoutPanel1.Cursor = Cursor;
            tableLayoutPanel2.Cursor = Cursor;
            tableLayoutPanel3.Cursor = Cursor;
            tableLayoutPanel4.Cursor = Cursor;
            tabPage1.Cursor = Cursor;
            tabPage2.Cursor = Cursor;
            tabPage3.Cursor = Cursor;
            txtDateTime.Cursor = Cursor;
            txtFrom.Cursor = Cursor;
            txtOd.Cursor = Cursor;
            txtSubject.Cursor=Cursor;
            tabControl1.TabPages.Remove(tabPage2);//błąd załączńków bo go nie powinno być
        }
        bool shouldShowTagInfo = false;
        private void btnShouldShowTagInfo_Click(object sender, EventArgs e)
        {
            shouldShowTagInfo = !shouldShowTagInfo;
            decideUponShowingTagInfo();
        }
        void decideUponShowingTagInfo() {
            if (!shouldShowTagInfo)
            {
                btnShouldShowTagInfo.Image = Resources.noTagInfo;
            }
            else {
                btnShouldShowTagInfo.Image = Resources.tagInfo;
            }
        }
        public GetMail(){
            InitializeComponent();
            InitializeComponentHere();
            if ((emailLoginsKey = Registry.CurrentUser.OpenSubKey(Program.żabkaEmailLoginsKeyName, true)) == null) emailLoginsKey = Registry.CurrentUser.CreateSubKey(Program.żabkaEmailLoginsKeyName);
        }
        RegistryKey emailLoginsKey;
        class LoginInfo {
            public string email,contracena,portSMTP,serverSMTP,portIMAP,serverIMAP;
        }
        private void GetMail_Load(object sender, EventArgs e)
        {
            webBrowser1.ScriptErrorsSuppressed = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private class Imap4FolderItem
        {
            public Imap4Folder imap4Folder;
            public Imap4FolderItem(Imap4Folder _imap4Folder)
            {
                imap4Folder= _imap4Folder;
            }
            public override string ToString()
            {
                return imap4Folder.Name;
            }
        }
        LoginInfo loginInfo;
        MailServer mailServer;
        MailClient mailClient;
        void getFoldersDisplayed() {
            try
            {
                Imap4Folder[] allFolders = mailClient.GetFolders();
                cmbFolders.Items.Clear();
                foreach (Imap4Folder imap4f in allFolders) cmbFolders.Items.Add(new Imap4FolderItem(imap4f));
                foreach(Imap4FolderItem imap4FolderItem in cmbFolders.Items) if(imap4FolderItem.imap4Folder.Name == "INBOX") cmbFolders.SelectedItem = imap4FolderItem;
            }
            catch (Exception ex)
            {
                Text = "rozłączono";
            }
        }
        public void WantedInbox(string emailName) {
            Show();
            Text = emailName;
            RegistryKey currentLoginKey = emailLoginsKey.OpenSubKey(emailName, true);
            
            loginInfo = new LoginInfo() { email = emailName, portSMTP = currentLoginKey.GetValue("portSMTP").ToString(), portIMAP = currentLoginKey.GetValue("portIMAP").ToString(), serverSMTP = currentLoginKey.GetValue("serverSMTP").ToString(), serverIMAP = currentLoginKey.GetValue("serverIMAP").ToString(), contracena = Program.DecryptStringFromBytes((byte[])currentLoginKey.GetValue("contracena", RegistryValueKind.Binary), Encoding.ASCII.GetBytes("1234567890123456"), Encoding.ASCII.GetBytes("1234567890123456")) };
            mailServer = new MailServer(loginInfo.serverIMAP, loginInfo.email, loginInfo.contracena, ServerProtocol.Imap4) { SSLConnection = true, Port = Int32.Parse(loginInfo.portIMAP) };
            mailClient = new MailClient("TryIt");
            try
            {
                mailClient.Connect(mailServer);
            }
            catch (Exception ex)
            {
                Text = "rozłączono";
            }
            getFoldersDisplayed();
        }
        private void GetMail_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private class MailItem
        {
            public Mail mail;
            public MailInfo mailInfo;
            public MailItem(Mail _mail, MailInfo _mailInfo)
            {
                mail = _mail;
                mailInfo = _mailInfo;
            }
            public override string ToString()
            {
                return mail.Subject;
            }
        }
        private void cmbFolders_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            tableLayoutPanel1.RowStyles[1].Height = 100;
            tableLayoutPanel1.RowStyles[3].Height = 0;
            tableLayoutPanel1.RowStyles[4].Height = 0;
            btnResize.BackgroundImage = Resources.down_arrow;
            try
            {
                mailClient.SelectFolder((cmbFolders.SelectedItem as Imap4FolderItem).imap4Folder);
                MailInfo[] infos = mailClient.GetMailInfos();
                listBox1.Items.Clear();
                foreach (MailInfo mailInfo in infos)
                {
                    Mail mail = new Mail("TryIt");
                    mail.Load(mailClient.GetMailHeader(mailInfo));
                    bool addedAlready = false;
                    for (int i = 0; i < listBox1.Items.Count && !addedAlready; i++) if (mail.SentDate > (listBox1.Items[i] as MailItem).mail.SentDate) {
                            listBox1.Items.Insert(i, new MailItem(mail, mailInfo));
                            addedAlready = true;
                        }
                    if (!addedAlready) listBox1.Items.Add(new MailItem(mail, mailInfo));
                }
            }
            catch (Exception ex)
            {
                Text = "rozłączono";
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            Mail mail = (listBox1.SelectedItem as MailItem).mail;
            txtDateTime.Text = mail.SentDate.ToString("dddd, dd MMMM yyyy HH:mm");
            txtOd.Text = mail.From.Address;
        }
        private class AttachmentItem
        {
            public Attachment attachment;
            public AttachmentItem(Attachment _attachment)
            {
                attachment = _attachment;
            }
            public override string ToString()
            {
                return attachment.Name;
            }
        }
        string currentEmailBody;
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItems.Count != 1) return;
            try {
                Mail currentEmail = mailClient.GetMail((listBox1.SelectedItem as MailItem).mailInfo);
                txtSubject.Text = currentEmail.Subject;
                tabControl1.TabPages.Remove(tabPage2);
                //--attachments
                Attachment[] attachments = currentEmail.Attachments;
                cmbAttachments.Items.Clear();
                foreach (Attachment attachment in attachments) cmbAttachments.Items.Add(new AttachmentItem(attachment));
                if (cmbAttachments.Items.Count > 0) {
                    cmbAttachments.SelectedItem = cmbAttachments.Items[0];
                    tabControl1.TabPages.Add(tabPage2);
                }
                //--combo TO
                MailAddress[] ma = currentEmail.To;
                cmbTO.Items.Clear();
                foreach (MailAddress mailAddress in ma) cmbTO.Items.Add(mailAddress.Address);
                cmbTO.SelectedItem = cmbTO.Items[0];
                //--EMAIL view
                foreach (Attachment attachment in attachments) currentEmail.HtmlBody = currentEmail.HtmlBody.Replace("cid:" + attachment.ContentID, "data:" + attachment.ContentType + ";base64," + Convert.ToBase64String(attachment.Content));
                webBrowser1.DocumentText = currentEmailBody =currentEmail.HtmlBody;
                tableLayoutPanel1.RowStyles[1].Height = 20;
                tableLayoutPanel1.RowStyles[3].Height = 70;
                tableLayoutPanel1.RowStyles[4].Height = 80;
                btnResize.BackgroundImage = Resources.up_arrow;
            }
            catch (Exception ex) {
                Text = "błąd połączeńa";
            }
        }
        [DllImport("USER32.dll")]
        static extern short GetKeyState(int nVirtKey);
        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete) btnKosz_Click(null, null);
            if (e.KeyCode == Keys.Return) listBox1_MouseDoubleClick(null, null);
            bool ctrl = Convert.ToBoolean(GetKeyState(0x11) & 0x8000);
            if (e.KeyCode == Keys.A&&ctrl) {
                listBox1.SelectedItems.Clear();
                for(int i=0;i< listBox1.Items.Count;i++) listBox1.SelectedItems.Add(listBox1.Items[i]);
            }
            if ((int)e.KeyCode == 226 && ctrl)
            {
                listBox1.SelectedItems.Clear();
            }
        }
        private void btnSaveAttachmentsToDesktop_Click(object sender, EventArgs e)
        {
            foreach (AttachmentItem attachmentItem in cmbAttachments.Items) attachmentItem.attachment.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + attachmentItem.attachment.Name, true);
        }
        private void btnInbox_Click(object sender, EventArgs e)
        {
            foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "INBOX") cmbFolders.SelectedItem = imap4FolderItem;
        }
        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) return;
            e.DrawBackground();
            if ((listBox1.Items[e.Index] as MailItem).mailInfo.Read == false) e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), new Font("Segoe Script", 12, FontStyle.Bold), Brushes.Black, e.Bounds);
            else e.Graphics.DrawString(listBox1.Items[e.Index].ToString(), new Font("Segoe Script", 10, FontStyle.Regular), Brushes.Black, e.Bounds);
            e.DrawFocusRectangle();
        }
        private void btnSent_Click(object sender, EventArgs e)
        {
            foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "Sent") cmbFolders.SelectedItem = imap4FolderItem;
        }
        private void btnSpam_Click(object sender, EventArgs e)
        {
            foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "Junk"|| imap4FolderItem.imap4Folder.Name == "Spam") cmbFolders.SelectedItem = imap4FolderItem;
        }
        private void btnTrash_Click(object sender, EventArgs e)
        {
            foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "Trash") cmbFolders.SelectedItem = imap4FolderItem;
        }
        private void btnResize_Click(object sender, EventArgs e)
        {
            if (tableLayoutPanel1.RowStyles[1].Height == 100)
            {
                tableLayoutPanel1.RowStyles[1].Height = 20;
                tableLayoutPanel1.RowStyles[3].Height = 70;
                tableLayoutPanel1.RowStyles[4].Height = 80;
                btnResize.BackgroundImage = Resources.up_arrow;
            }
            else {
                tableLayoutPanel1.RowStyles[1].Height = 100;
                tableLayoutPanel1.RowStyles[3].Height = 0;
                tableLayoutPanel1.RowStyles[4].Height = 0;
                btnResize.BackgroundImage = Resources.down_arrow;
            }
        }
        private void btnKosz_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;
            try {
                if ((cmbFolders.SelectedItem as Imap4FolderItem).imap4Folder.Name == "Trash")
                {
                    foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Delete(mailItem.mailInfo);
                    mailClient.Expunge();//było zaznaczone, teraz usunięte
                    List<MailItem> mailItems = new List<MailItem>();
                    foreach (MailItem mailItem in listBox1.SelectedItems) mailItems.Add(mailItem);
                    listBox1.SelectedItems.Clear();
                    foreach (MailItem mailItem in mailItems) listBox1.Items.Remove(mailItem);
                }
                else {
                    Imap4Folder trash = null;
                    foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "Trash") trash = imap4FolderItem.imap4Folder;
                    foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Move(mailItem.mailInfo, trash);
                    foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Delete(mailItem.mailInfo);
                    mailClient.Expunge();
                    List<MailItem> mailItems = new List<MailItem>();
                    foreach (MailItem mailItem in listBox1.SelectedItems) mailItems.Add(mailItem);
                    listBox1.SelectedItems.Clear();
                    foreach (MailItem mailItem in mailItems) listBox1.Items.Remove(mailItem);
                }
            }
            catch (Exception ex) {
                Text = "rozłączono";
            }
        }
        private void btnBulb_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;
            try
            {
                Imap4Folder inbox=null;
                foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "INBOX") inbox = imap4FolderItem.imap4Folder;
                foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Move(mailItem.mailInfo, inbox);
                foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Delete(mailItem.mailInfo);
                mailClient.Expunge();
                List<MailItem> mailItems = new List<MailItem>();
                foreach (MailItem mailItem in listBox1.SelectedItems) mailItems.Add(mailItem);
                listBox1.SelectedItems.Clear();
                foreach (MailItem mailItem in mailItems) listBox1.Items.Remove(mailItem);
            }
            catch (Exception ex)
            {
                Text = "rozłączono";
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;
            foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.MarkAsRead(mailItem.mailInfo,true);
            listBox1.SelectedItems.Clear();
            listBox1.Refresh();
        }

        private void btnUnread_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;
            foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.MarkAsRead(mailItem.mailInfo, false);
            listBox1.SelectedItems.Clear();
            listBox1.Refresh();
        }
        void czasWyświetlićZałączńk() {
            Attachment attachment = (cmbAttachments.SelectedItem as AttachmentItem).attachment;
            if (attachment.ContentType.ToLower().StartsWith("image")) webBrowser1.DocumentText = "<html><body><img src='data:" + attachment.ContentType + ";base64," + Convert.ToBase64String(attachment.Content) + "'></img></body></html>";
        }
        void czasWyświetlićEmail() {
            webBrowser1.DocumentText = currentEmailBody;
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.TabPages[tabControl1.SelectedIndex].Name == "tabPage2") czasWyświetlićZałączńk();
            else if (tabControl1.TabPages[tabControl1.SelectedIndex].Name == "tabPage1") czasWyświetlićEmail();
        }

        private void cmbAttachments_SelectedIndexChanged(object sender, EventArgs e)
        {
            czasWyświetlićZałączńk();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().ToUpper() == "ABOUT:BLANK") return;
            Process.Start(e.Url.ToString());
            e.Cancel = true;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) 
        {
            string tagUpper = "";

            foreach (HtmlElement tag in (sender as WebBrowser).Document.All)
            {
                tagUpper = tag.TagName.ToUpper();

                if ((tagUpper == "AREA") || (tagUpper == "A"))
                {
                    tag.MouseUp += new HtmlElementEventHandler(tag_MouseUp);
                }
                else {
                    tag.MouseUp += new HtmlElementEventHandler(otherTag_MouseUp);
                }
            }
        }
        void otherTag_MouseUp(object sender, HtmlElementEventArgs e) {
            if (e.MouseButtonsPressed == MouseButtons.Left && shouldShowTagInfo)
            {
                MessageBox.Show((sender as HtmlElement).TagName);
            }
        }
        void tag_MouseUp(object sender, HtmlElementEventArgs e)
        {
            if (e.MouseButtonsPressed == MouseButtons.Left) {
                Regex pattern = new Regex(Resources.linkString);
                Match match = pattern.Match((sender as HtmlElement).OuterHtml);
                string link = match.Groups["link"].Value;
                link = link.Substring(1, link.Length - 2);
                Process.Start(link);
            }
        }
        private void webBrowser1_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnReply_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItems.Count != 1) return;
            if (Program.sendMail == null) Program.sendMail = new SendMail();
            Program.sendMail.replyThisOne((listBox1.SelectedItem as MailItem).mail.From.Address);
            Program.sendMail.Show();
        }

        private void btnReplyTo_Click(object sender, EventArgs e)
        {
            if (cmbTO.Text==null||cmbTO.Text==null) return;
            if (Program.sendMail == null) Program.sendMail = new SendMail();
            Program.sendMail.replyThisOne(cmbTO.Text);
            Program.sendMail.Show();
        }

        private void btnGromadzone_Click(object sender, EventArgs e)
        {
            foreach (Imap4FolderItem imap4FolderItem in cmbFolders.Items) if (imap4FolderItem.imap4Folder.Name == "Gromadzone") cmbFolders.SelectedItem = imap4FolderItem;
        }

        private void btnUsuńTeczkę_Click(object sender, EventArgs e)
        {
            mailClient.DeleteFolder(new Imap4Folder(cmbFolders.Text));
            getFoldersDisplayed();
        }
        Imap4FolderItem moveMailsFrom;
        private void cmbFolders_SelectedIndexChanged_2(object sender, EventArgs e)
        {
            cmbFolders.SelectedIndexChanged -= cmbFolders_SelectedIndexChanged_2;
            btnInbox.Enabled = true;
            btnSpam.Enabled = true;
            btnSent.Enabled = true;
            btnTrash.Enabled = true;
            btnResize.Enabled = true;
            btnKosz.Enabled = true;
            btnBulb.Enabled = true;
            btnRead.Enabled = true;
            btnUnread.Enabled = true;
            btnReplyFrom.Enabled = true;
            btnShouldShowTagInfo.Enabled = true;
            btnGromadzone.Enabled = true;
            btnUsuńTeczkę.Enabled = true;
            btnPrzeńeś.Enabled = true;
            listBox1.Enabled = true;
            tabControl1.Enabled = true;
            if (cmbFolders.SelectedItem.ToString() == "Spam"|| cmbFolders.SelectedItem.ToString() == "Segregator" || cmbFolders.SelectedItem.ToString() == "Społeczności") cmbFolders.SelectedItem = moveMailsFrom;
            cmbFolders.SelectedIndexChanged += cmbFolders_SelectedIndexChanged_1;
            if (moveMailsFrom.ToString()!=cmbFolders.SelectedItem.ToString()) {
                foreach (MailItem mailItem in listBox1.SelectedItems) mailClient.Move(mailItem.mailInfo, new Imap4Folder(cmbFolders.SelectedItem.ToString()));
                cmbFolders_SelectedIndexChanged_1(null,null);
            }
            return;
        }

        private void btnPrzeńeś_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count == 0) return;
            btnInbox.Enabled = false;
            btnSpam.Enabled = false;
            btnSent.Enabled = false;
            btnTrash.Enabled = false;
            btnResize.Enabled = false;
            btnKosz.Enabled = false;
            btnBulb.Enabled = false;
            btnRead.Enabled = false;
            btnUnread.Enabled = false;
            btnReplyFrom.Enabled = false;
            btnShouldShowTagInfo.Enabled = false;
            btnGromadzone.Enabled = false;
            btnUsuńTeczkę.Enabled = false;
            btnPrzeńeś.Enabled = false;
            listBox1.Enabled = false;
            tabControl1.Enabled = false;
            moveMailsFrom = cmbFolders.SelectedItem as Imap4FolderItem;
            cmbFolders.SelectedIndexChanged -= cmbFolders_SelectedIndexChanged_1;
            cmbFolders.SelectedIndexChanged += cmbFolders_SelectedIndexChanged_2;
            return;
        }
    }
}
