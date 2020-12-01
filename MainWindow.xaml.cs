using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Net.Cache;

namespace ProxyTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ObservableCollection<ListOfSites> listOfSites = new ObservableCollection<ListOfSites>();
        private ObservableCollection<ListOfProxies> listOfProxies = new ObservableCollection<ListOfProxies>();
        private ObservableCollection<ListOfProxyTests> listOfProxyTests = new ObservableCollection<ListOfProxyTests>();
        private ObservableCollection<ListOfProxyTests> listOfProxyTestsForDisplay = new ObservableCollection<ListOfProxyTests>();
        private string proxyCsvFilePath = "";
        private bool isImportOrAdd = false;
        private bool isTested = false;
        private bool isFilterUsed = false;
        private int tabId = 0;
        private string siteUrl = "";
        private CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();

            listOfSites = ReadSitesCSV();
            lv_sites.ItemsSource = listOfSites;

            cb_sites.ItemsSource = listOfSites;
            cb_sites.DisplayMemberPath = "sitename";
            cb_sites.SelectedIndex = 0;

            lv_proxytests.ItemsSource = listOfProxyTests;


        }
        private async void testAll_click(object sender, RoutedEventArgs e)
        {
            if (this.siteUrl.Equals(""))
            {
                MessageBox.Show("Site Url is empty!", "Trek Proxy Tester");
                return;
            }
            cts = new CancellationTokenSource();
           
            this.isTested = false;
            this.listOfProxyTests = ReadListProxyTests();
            this.listOfProxyTestsForDisplay = this.listOfProxyTests;
            lv_proxytests.ItemsSource = this.listOfProxyTests;

            int numberPerOnce = 20;
            int amountProxy = this.listOfProxyTestsForDisplay.Count;
            int remainder = this.listOfProxyTestsForDisplay.Count % numberPerOnce;
            int i = 0;
            bool f = false;
            int timeout = Int32.Parse(tb_ms.Text);
            if (timeout == 0)
                timeout = 1000;

            List<ListOfProxyTests> listOfProxyTestss = new List<ListOfProxyTests>();
            try
            {

                foreach (ListOfProxyTests lopt in this.listOfProxyTestsForDisplay)
                {
                    //listOfProxyTestss.Add(lopt);
                    Task dd = lopt.TestProxyAsync(this.siteUrl, timeout, cts.Token);
                    //if (i++ > 2)
                    //    break;
                    //listOfProxyTestss.Add(lopt);
                    //obj[i][i] = lopt;
                    //if (((i + 1) % numberPerOnce == 0) || f)
                    // {
                    //   Task d = proxyTester(listOfProxyTestss);
                    //}

                    //if ((amountProxy - remainder) == (1 + i++)) f = true;

                }
            }
            catch (OperationCanceledException e1)
            {
                Console.WriteLine(e1.ToString());
                MessageBox.Show("Canceled");

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }

            this.isTested = true;
        }
        async Task ddd(CancellationToken ct)
        {

            var tasks = new ConcurrentBag<Task>();
            //Task t;

            int i = 0;
            await Task.Run(() =>
            {
                ct.ThrowIfCancellationRequested();
                bool f = false;

                Task t = Task.Run(() =>
                {
                    while (i++ < 5)
                    {
                        Thread.Sleep(1000);
                    }
                    //if(f) ct.ThrowIfCancellationRequested();
                    if (f) return;
                    else MessageBox.Show("Hi");
                }, ct);

                while (true)
                {

                    if (ct.IsCancellationRequested)
                    {
                        f = true;
                        //t.Dispose();
                        ct.ThrowIfCancellationRequested();
                    }
                }

            }, ct);

            // tasks.Add(t);
        }
        private async Task proxyTester(List<ListOfProxyTests> listOfProxyTestss)
        {

            int timeout = Int32.Parse(tb_ms.Text);
            if (timeout == 0)
                timeout = 1000;
            Task[] task = new Task[listOfProxyTestss.Count];
            await Task.Run(() =>
            {
                try
                {
                    int i = 0;
                    foreach (ListOfProxyTests lopt in listOfProxyTestss)
                    {
                        task[i++] = lopt.TestProxyAsync(this.siteUrl, timeout, cts.Token);

                    }
                    listOfProxyTestss.Clear();

                }
                catch (OperationCanceledException e1)
                {
                    Console.WriteLine(e1.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            });
        }

        private void deleteFailed_click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ListOfProxyTests> temp = new ObservableCollection<ListOfProxyTests>();
            foreach (ListOfProxyTests lopt in this.listOfProxyTests)
            {
                if (!lopt.speed.Equals("X"))
                {
                    //this.listOfProxyTests.Remove(lopt);
                    temp.Add(lopt);
                }
            }
            this.listOfProxyTests = temp;
            lv_proxytests.ItemsSource = this.listOfProxyTests;
        }

        private void unactiveFailed_click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<ListOfProxyTests> temp = new ObservableCollection<ListOfProxyTests>();
            ObservableCollection<ListOfProxyTests> temp1 = new ObservableCollection<ListOfProxyTests>();
            ObservableCollection<ListOfProxies> templist = new ObservableCollection<ListOfProxies>();
            var csv = new StringBuilder();

            foreach (ListOfProxyTests lopt in this.listOfProxyTests)
            {
                if (!lopt.speed.Equals("X"))
                {
                    //this.listOfProxyTests.Remove(lopt);
                    temp.Add(lopt);
                }
                else
                {
                    temp1.Add(lopt);
                }
            }

            foreach (ListOfProxies lop in this.listOfProxies)
            {
                foreach (ListOfProxyTests lopt in temp1)
                {
                    if (lop.ip.Equals(lopt.proxy.Split(':')[0]) && lop.port.Equals(lopt.proxy.Split(':')[1]))
                    {
                        lop.status = false;
                        lop.btnContent = "Unactive";
                        lop.color = "Red";
                        break;
                    }
                }
                string status = "0";
                if (lop.status) status = "1";
                var newLine = string.Format("{0},{1},{2},{3},{4}", lop.ip, lop.port, lop.username, lop.password, status);
                csv.AppendLine(newLine);
                templist.Add(lop);
            }
            this.listOfProxies = templist;
            lv_proxies.ItemsSource = this.listOfProxies;
            this.listOfProxyTests = temp;
            lv_proxytests.ItemsSource = this.listOfProxyTests;

            if (this.isImportOrAdd)
            {
                File.WriteAllText(this.proxyCsvFilePath, csv.ToString());
            }
        }

        private void reload_click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                Console.WriteLine("Cancel clicked!");
            }
            this.isTested = false;
            this.listOfProxyTests = ReadListProxyTests();
            this.listOfProxyTestsForDisplay = this.listOfProxyTests;
            lv_proxytests.ItemsSource = this.listOfProxyTests;
        }

        private void site_delete(object sender, RoutedEventArgs e)
        {

            cb_sites.SelectedIndex = 0;
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var selected = lv_sites.SelectedItems.Cast<Object>().ToArray();
                foreach (var item in selected)
                {
                    ListOfSites site = (item as ListOfSites);
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TrekProxyTester"))
                    {
                        string[] siteKeys = key.GetValueNames();
                        foreach (string sitekey in siteKeys)
                            if (key.GetValue(sitekey).Equals(site.sitedomain))
                            {
                                key.DeleteValue(sitekey);
                            }
                    }
                }
                foreach (var item in selected) this.listOfSites.Remove(item as ListOfSites);
            }
        }

        private void proxytests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var temp = (e.AddedItems[0] as ListOfSites);
                this.siteUrl = temp.sitedomain;
                return;
            }
            this.siteUrl = "";
        }

        private void importProxy_clicked(object sender, RoutedEventArgs e)
        {
            isImportOrAdd = true;
            string filename = "";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                this.proxyCsvFilePath = filename;
                this.listOfProxies = ReadProxyCSV(filename);
                lv_proxies.ItemsSource = this.listOfProxies;
            }

        }

        public ObservableCollection<ListOfSites> ReadSitesCSV()
        {

            ObservableCollection<ListOfSites> templist = new ObservableCollection<ListOfSites>();

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TrekProxyTester"))
            {
                string[] siteKeys = key.GetValueNames();
                int amountOfSite = siteKeys.Length;
                if (amountOfSite > 0)
                {
                    foreach (string sitekey in siteKeys)
                    {
                        string stieUrl = key.GetValue(sitekey).ToString();
                        templist.Add(new ListOfSites { sitedomain = stieUrl, sitename = sitekey });
                    }
                }
                else
                {
                    var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "DefaultSiteUrls.txt");
                    if (!File.Exists(path))
                    {
                        using (System.IO.FileStream fs = System.IO.File.Create(path)) { }
                    }
                    else
                    {
                        string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(path, ".txt"));

                        foreach (string line in lines)
                        {
                            string[] data = line.Split(',');
                            templist.Add(new ListOfSites { sitedomain = data[0], sitename = data[1] });
                            key.SetValue(data[1], data[0]);
                        }
                    }
                }
                key.Close();
            }
            return templist;

        }

        public ObservableCollection<ListOfProxies> ReadProxyCSV(string fileName)
        {
            string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".txt"));
            ObservableCollection<ListOfProxies> templist = new ObservableCollection<ListOfProxies>();
            int i = 0;
            try
            {
                foreach (string line in lines)
                {
                    string[] data = line.Split(':');
                    bool status = false;
                    string username = "", password = "";
                    if (data.Length > 2)
                    {
                        username = data[2];
                        password = data[3];
                    }
                    if (data.Length != 5 || data[4].Equals("1"))
                    {
                        status = true;
                    }
                    templist.Add(new ListOfProxies { id = i, ip = data[0], port = data[1], username = username, password = password, status = status });
                    i++;
                }
            }
            catch (Exception)
            {
            }

            return templist;
        }

        private void addSite_clicked(object sender, RoutedEventArgs e)
        {
            string Url = tb_domain.Text;
            string siteName = tb_siteName.Text;
            if (Url.Trim().Equals("") || siteName.Trim().Equals(""))
            {
                return;
            }
            tb_domain.Text = "";
            tb_siteName.Text = "";


            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\TrekProxyTester"))
            {
                key.SetValue(siteName, Url);
            }

            this.listOfSites.Add(new ListOfSites { sitedomain = Url, sitename = siteName });
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var csv = new StringBuilder();
            var path = this.proxyCsvFilePath;
            if (isImportOrAdd)
            {
                File.Delete(path);
            }

            foreach (ListOfProxies lop in this.listOfProxies)
            {
                string status = "1";
                if (!lop.status)
                    status = "0";
                if (lop.id == Int64.Parse(btn.Tag.ToString()))
                {
                    if (lop.status)
                    {
                        lop.status = false;
                        btn.Content = "Unactive";
                        btn.Foreground = Brushes.Red;
                        status = "0";
                    }
                    else
                    {
                        lop.status = true;
                        btn.Content = "Active";
                        btn.Foreground = Brushes.Blue;
                    }
                }
                if (isImportOrAdd)
                {
                    var newLine = string.Format("{0}:{1}:{2}:{3}:{4}", lop.ip, lop.port, lop.username, lop.password, status);
                    csv.AppendLine(newLine);
                }

            }
            if (isImportOrAdd)
            {
                File.AppendAllText(path, csv.ToString());
            }
        }

        private void SaveProxyClick(object sender, RoutedEventArgs e)
        {
            string[] lines = tb_newproxy.Text.Split('\n');
            var csv = new StringBuilder();
            bool f = false;
            foreach (string line in lines)
            {
                if (line.Equals("") || line.Equals("\r"))
                    break;
                string[] data = line.Split(':');
                if (data.Length < 2)
                {
                    f = true;
                    break;
                }

                string ip = "", port = "", username = "", password = "", status = "1";
                int i = 0;
                foreach (string temp in data)
                {
                    string temp1 = temp.Replace("\r", "");
                    switch (i)
                    {
                        case 0: ip = temp1; break;
                        case 1: port = temp1; break;
                        case 2: username = temp1; break;
                        case 3: password = temp1; break;
                        default: break;
                    }
                    i++;
                }
                var newLine = string.Format("{0},{1},{2},{3},{4}", ip, port, username, password, status);
                csv.AppendLine(newLine);
            }
            if (f)
            {
                MessageBox.Show("Proxy data wrong!\n Please check it", "Wrong");
            }
            else
            {
                var filename = "";
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    filename = dlg.FileName;
                    File.WriteAllText(filename, csv.ToString());
                    this.proxyCsvFilePath = filename;
                }

            }
        }

        private void AddProxyClick(object sender, RoutedEventArgs e)
        {
            isImportOrAdd = false;
            ObservableCollection<ListOfProxies> templist = new ObservableCollection<ListOfProxies>();
            bool f = false;
            if (tb_newproxy.Text.Trim().Equals(""))
            {
                f = true;
            }
            string[] lines = tb_newproxy.Text.Split('\n');
            int j = 0;
            foreach (string line in lines)
            {
                if (line.Equals("") || line.Equals("\r"))
                    continue;
                string[] data = line.Split(':');
                if (data.Length < 2)
                {
                    f = true;
                    break;
                }

                string ip = "", port = "", username = "", password = "";
                bool status = true;
                int i = 0;
                foreach (string temp in data)
                {
                    string temp1 = temp.Replace("\r", "");
                    switch (i)
                    {
                        case 0: ip = temp1; break;
                        case 1: port = temp1; break;
                        case 2: username = temp1; break;
                        case 3: password = temp1; break;
                        default: break;
                    }
                    i++;
                }
                templist.Add(new ListOfProxies { id = j, ip = ip, port = port, username = username, password = password, status = status });
                j++;
            }

            if (f)
            {
                MessageBox.Show("Proxy data wrong!\n Please check it", "Wrong");
            }
            else
            {
                this.listOfProxies = templist;
                lv_proxies.ItemsSource = this.listOfProxies;
            }
        }

        private ObservableCollection<ListOfProxyTests> ReadListProxyTests()
        {
            ObservableCollection<ListOfProxyTests> listOfProxyTests = new ObservableCollection<ListOfProxyTests>();
            int id = 0;
            foreach (ListOfProxies lop in this.listOfProxies)
            {
                string proxy = lop.ip + ":" + lop.port;
                if (lop.status)
                {
                    listOfProxyTests.Add(new ListOfProxyTests { id = id, proxy = proxy, status = "Ready", username = lop.username, password = lop.password, speed = "0", isChecked = false });
                    id++;
                }
            }

            return listOfProxyTests;
        }

        private void tabclick(object sender, SelectionChangedEventArgs e)
        {
            if (tabproxy != null && tabproxy.IsSelected)
            {
                tabId = 1;
                if (tabId != 1)
                {
                    //ObservableCollection<ListOfProxies> tt = new ObservableCollection<ListOfProxies>();
                    //tt.Add(new ListOfProxies { id = 0, ip = "123123", port = "345", username = "username", password = "password", status = false });
                    ////lv_proxies.ItemsSource = this.listOfProxies;
                    //this.listOfProxies = tt;    

                    //lv_proxies.ItemsSource = tt;
                }
            }
            // do your staff
            if (tabsite != null && tabsite.IsSelected)
            {
                tabId = 2;
            }
            // do your staff
            if (tabproxytest != null && tabproxytest.IsSelected)
            {
                if (tabId != 3)
                {
                    this.listOfProxyTests = ReadListProxyTests();
                    this.listOfProxyTestsForDisplay = this.listOfProxyTests;
                    lv_proxytests.ItemsSource = this.listOfProxyTests;
                    cb_sites.SelectedIndex = 0;
                    tb_ms.Text = "5000";
                    tabId = 3;
                    this.isTested = false;
                }
            }
        }

        private void export_click(object sender, RoutedEventArgs e)
        {
            if (!this.isTested) return;
            var csv = new StringBuilder();
            bool f = false;
            ObservableCollection<ListOfProxyTests> datas = new ObservableCollection<ListOfProxyTests>();
            if (this.isFilterUsed)
            {
                datas = this.listOfProxyTestsForDisplay;
            }
            else
            {
                datas = this.listOfProxyTests;
            }
            foreach (ListOfProxyTests lopt in datas)
            {
                if (!lopt.isChecked)
                {
                    f = true;
                    break;
                }
                if (lopt.status.ToLower().Equals("good"))
                {
                    var newLine = string.Format("{0}:{1}:{2}", lopt.proxy, lopt.username, lopt.password);
                    csv.AppendLine(newLine);
                }
            }
            if (f)
            {
                MessageBox.Show("Not Test Proxy!", "Message");
            }
            else
            {
                var filename = "";
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Display OpenFileDialog by calling ShowDialog method 
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    filename = dlg.FileName;
                    File.WriteAllText(filename, csv.ToString());
                }

            }
        }

        private void copy_click(object sender, RoutedEventArgs e)
        {
            if (!this.isTested) return;
            var csv = new StringBuilder();
            bool f = false;
            ObservableCollection<ListOfProxyTests> datas = new ObservableCollection<ListOfProxyTests>();
            if (this.isFilterUsed)
            {
                datas = this.listOfProxyTestsForDisplay;
            }
            else
            {
                datas = this.listOfProxyTests;
            }
            foreach (ListOfProxyTests lopt in datas)
            {
                if (!lopt.isChecked)
                {
                    f = true;
                    break;
                }
                if (lopt.status.ToLower().Equals("good"))
                {
                    var newLine = string.Format("{0}:{1}:{2}", lopt.proxy, lopt.username, lopt.password);
                    csv.AppendLine(newLine);
                }
            }
            if (f)
            {
                MessageBox.Show("Not Test Proxy!", "Message");
            }
            else
            {
                Clipboard.SetText(csv.ToString());
                MessageBox.Show("Good Proxies Copied!", "Message");
            }
        }

        private void tb_ms_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tabId == 3)
            {
                TextBox tb = sender as TextBox;
                int filterValue = 5000;
                try
                {
                    filterValue = Int32.Parse(tb.Text);
                }
                catch (Exception e1)
                {
                    Console.WriteLine(e1.ToString());
                    tb.Text = "5000";
                    filterValue = 5000;
                }
                if (isTested)
                {
                    listOfProxyTestsForDisplay = getFilterResult(this.listOfProxyTests, filterValue);
                    lv_proxytests.ItemsSource = listOfProxyTestsForDisplay;
                    this.isFilterUsed = true;
                }
            }
        }

        private ObservableCollection<ListOfProxyTests> getFilterResult(ObservableCollection<ListOfProxyTests> datas, int filterValue)
        {
            ObservableCollection<ListOfProxyTests> temp = new ObservableCollection<ListOfProxyTests>();
            foreach (ListOfProxyTests lopt in datas)
            {
                if (lopt.speed.Equals("X")) continue;
                if (Int64.Parse(lopt.speed) <= filterValue)
                {
                    temp.Add(lopt);
                }
            }
            return temp;
        }
        private void windowSize_change(object sender, SizeChangedEventArgs e)
        {
            gridHeight.Height = (sender as Window).ActualHeight - 200;
            rowTab1Height.Height = new GridLength((sender as Window).ActualHeight - 300);
            rowTab2Height.Height = new GridLength((sender as Window).ActualHeight - 350);
            rowTab3Height.Height = new GridLength((sender as Window).ActualHeight - 350);
            //columnWidth.Width = (sender as Window).ActualWidth;
            //this.gridHeight = ((sender as Window).ActualHeight - ss).ToString();
        }

        private void proxyListViewSize_change(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar
            var col1 = 0.25;
            var col2 = 0.15;
            var col3 = 0.20;
            var col4 = 0.20;
            var col5 = 0.15;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
            gView.Columns[4].Width = workingWidth * col5;
        }

        private void siteListViewSize_change(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar
            var col1 = 0.15;
            var col2 = 0.30;
            var col3 = 0.50;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
        }

        private void testListViewSize_change(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar
            var col1 = 0.20;
            var col2 = 0.20;
            var col3 = 0.20;
            var col4 = 0.15;
            var col5 = 0.20;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
            gView.Columns[4].Width = workingWidth * col5;
        }
    }















    public class ListOfSites
    {
        public int index { get; set; }
        public string sitename { get; set; }
        public string sitedomain { get; set; }
    }

    public class ListOfProxies
    {
        private ICommand _btnStatusClick;
        private string statusContent = "";
        private string _btnContent;
        public bool status = true;
        public int id { get; set; }
        public string ip { get; set; }
        public string port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string color
        {
            get
            {
                if (status)
                {
                    return ("Blue");
                }
                else
                {
                    return ("Red");
                }
            }
            set { }
        }
        public string btnContent
        {
            get
            {
                if (this.status)
                {
                    this.statusContent = "Active";
                }
                else
                {
                    this.statusContent = "Unactive";

                }
                return this.statusContent;
            }
            set
            {
                _btnContent = value;
                //NotifyPropertyChanged("btnContent");
                //if (PropertyChanged != null)
                //    PropertyChanged(this, new PropertyChangedEventArgs(btnContent));
            }
        }
        public ICommand btnStatusClick
        {
            get
            {
                return _btnStatusClick ?? (_btnStatusClick = new CommandHandler(() => MyAction(), () => CanExecute));
            }
        }

        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public void MyAction()
        {
            if (!this.status)
            {
                this.statusContent = "Active";
                this.status = true;
            }
            else
            {
                this.statusContent = "Unactive";
                this.status = false;
            }
            this.btnContent = this.statusContent;
        }

    }

    public class CommandHandler : ICommand
    {
        private Action _action;
        private Func<bool> _canExecute;

        /// <summary>
        /// Creates instance of the command handler
        /// </summary>
        /// <param name="action">Action to be executed by the command</param>
        /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
        public CommandHandler(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Wires CanExecuteChanged event 
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Forcess checking if execute is allowed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute.Invoke();
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class ListOfProxyTests : INotifyPropertyChanged
    {
        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;
        private string _status;
        private string _progresshidden = "Collapsed";
        private string _speedhidden = "Collapsed";
        private string _speed = "";
        private string _color = "Gray";
        public int id { get; set; }
        public string proxy { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isChecked { get; set; }
        public bool isIp { get; set; }
        private const int GOOD = 0;
        private const int BANNED = 1;
        private const int FAILED = 2;
        public string status
        {
            get { return _status; }
            set
            {
                _status = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public string color
        {
            get { return _color; }
            set
            {
                _color = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string speedhidden
        {
            get { return _speedhidden; }
            set
            {
                _speedhidden = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        public string progresshidden
        {
            get { return _progresshidden; }
            set
            {
                _progresshidden = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }


        System.Diagnostics.Stopwatch timer1 = new Stopwatch();
        protected string siteUrl = "";
        protected int timeout = 0;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public async Task TestProxyAsync(string siteUrl, int timeout, CancellationToken cta)
        {
            this.siteUrl = siteUrl;
            this.timeout = timeout;
            this.status = "Testing...";
            this.progresshidden = "Visible";
            this.speedhidden = "Collapsed";
            this.color = "Gray";
            this.isChecked = false;

            await Task.Run(() =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                ServicePointManager.CheckCertificateRevocationList = false;
                ServicePointManager.DefaultConnectionLimit = 500;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DnsRefreshTimeout = -1;
                ServicePointManager.EnableDnsRoundRobin = false;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)4080;
                ServicePointManager.ServerCertificateValidationCallback += (sender2, certificate, chain, sslPolicyErrors) => true;
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
                HttpWebRequest.DefaultCachePolicy = policy;
                HttpWebRequest request;

                WebProxy proxy;
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(siteUrl);
                    request.Abort();
                    request = (HttpWebRequest)WebRequest.Create(siteUrl);
                    proxy = new WebProxy(this.proxy, false);
                }
                catch (Exception e)
                {
                    resultView(FAILED);
                    return;
                }

                request.Proxy = proxy;
                request.Method = WebRequestMethods.Http.Head;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;

                request.Headers.Add("origin", siteUrl.Split('/')[0]);
                request.Headers.Add("Accept-Language", "en-GB,en;q=0.9,en-US;q=0.8");
                request.Headers.Add("Cache-Control", "no-cache");
                request.Headers.Add("Pragma", "no-cache");


                request.UseDefaultCredentials = true;
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36";
               
                HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = noCachePolicy;
                request.Proxy.Credentials = new NetworkCredential(this.username, this.password);
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.Timeout = timeout;
                //////request.KeepAlive = false;
                request.AllowReadStreamBuffering = false;
                request.AllowWriteStreamBuffering = false;

                //if (IsIPAddress(this.proxy.Split(':')[0]))
                //{
                //    this.isIp = true;
                //}
                //else
                //{
                //    this.isIp = false;
                //}
                //if (timer1.ElapsedMilliseconds < 1)
                //{
                //}
                timer1.Restart();
                //Console.WriteLine("-----Time out so Reqeust Start------    " + timer1.ElapsedMilliseconds.ToString());
                //request.BeginGetResponse(result => FinishWebRequest(request, timer1), null);
                IAsyncResult result = request.BeginGetResponse(new AsyncCallback(FinishWebRequest), request);
                //ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), request, timeout, true);
                //allDone.WaitOne();
                
                Thread.Sleep(timeout);
                request.Abort();
                //Stream.Flush();
                if (!this.isChecked)
                {
                    timer1.Stop();

                    Console.WriteLine("-----Time out so Reqeust Cancelled------    " + timer1.ElapsedMilliseconds.ToString());
                    resultView(FAILED);
                    return;
                }
            }, cta);
        }

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        private void FinishWebRequest(IAsyncResult result)
        {
            //Stopwatch t = new Stopwatch();
            //timer1.Restart();
            //Console.WriteLine("----- Success------    " + timer1.ElapsedMilliseconds.ToString());

            HttpWebResponse response;
            try
            {
                //response = (result.AsyncState as HttpWebRequest).GetResponse() as HttpWebResponse;
                response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;

                response.Close();
                
                Console.WriteLine("-----Reqeust Success------    " + timer1.ElapsedMilliseconds.ToString());
                timer1.Stop();
                if (this.isChecked) return;
                Console.WriteLine("------Response Success------");
                resultView(GOOD, timer1.ElapsedMilliseconds);
                return;
            }
            catch (WebException ex)
            {
                //response = null;
                timer1.Stop();
                Console.WriteLine("------Response Exception------");
                Console.WriteLine("------Response Exception WHY  ------");
                Console.WriteLine(ex.ToString());
                if (this.isChecked) return;

                try
                {
                    string errorStr = "";
                    if (ex.Response == null)
                    {
                        Console.WriteLine("------Response Null------");
                        resultView(FAILED);
                        //if (this.isIp)
                        //{
                        //    resultView(FAILED);
                        //}
                        //else
                        //{
                        //    resultView(GOOD, timer1.ElapsedMilliseconds);
                        //}

                        return;
                    }

                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        Console.WriteLine("------Response Exception 2------");
                        errorStr += reader.ReadToEnd();
                        reader.Close();
                    }
                    Console.WriteLine(errorStr);
                    if (errorStr.ToLower().Contains("access denied") && !errorStr.ToUpper().Contains("ERR_ACCESS_DENIED"))
                    {
                        Console.WriteLine("------Response Banned------");
                        resultView(BANNED, timer1.ElapsedMilliseconds);
                    }
                    else
                    {
                        if (((HttpWebResponse)ex.Response).Server.ToString().Equals("cloudflare"))
                        {
                            Console.WriteLine("------Response is cloudflare ------");
                            HttpWebResponse res = ((HttpWebResponse)ex.Response);
                            if ((int)res.StatusCode == 403)
                            {
                                resultView(BANNED, timer1.ElapsedMilliseconds);
                            }
                            else if ((int)res.StatusCode == 503)
                            {
                                resultView(GOOD, timer1.ElapsedMilliseconds);
                            }
                            else
                            {
                                resultView(FAILED);
                            }
                            return;
                        }

                        HttpWebResponse res1 = ((HttpWebResponse)ex.Response);
                        if ((int)res1.StatusCode == 403)
                        {
                            resultView(BANNED, timer1.ElapsedMilliseconds);
                        }
                        else if ((int)res1.StatusCode == 503)
                        {
                            resultView(GOOD, timer1.ElapsedMilliseconds);
                        }
                        else
                        {
                            resultView(FAILED);
                        }
                        return;
                    }
                }
                catch (Exception ex1)
                {
                    timer1.Stop();
                    if (this.isChecked) return;
                    Console.WriteLine(ex1.ToString());
                    resultView(FAILED);
                }
            }
        }

        /*
         *  status  Good   = 0
         *          Banned = 1
         *          Failed = 2
         *  time    status = 0,1 && time
         *          other null
         * */
        private void resultView(int status, long time = 0)
        {
            switch (status)
            {
                case 0:
                    this.status = "Good";
                    this.color = "Blue";
                    this.speed = time.ToString();
                    break;
                case 1:
                    this.status = "Banned";
                    this.color = "Red";
                    this.speed = time.ToString();
                    break;
                case 2:
                    this.status = "Failed";
                    this.color = "Red";
                    this.speed = "X";
                    break;
                default:
                    this.status = "Failed";
                    this.color = "Red";
                    this.speed = "X";
                    break;
            }
            this.progresshidden = "Collapsed";
            this.speedhidden = "Visible";
            this.isChecked = true;
        }


        private bool IsIPAddress(string input)
        {
            var hostNameType = Uri.CheckHostName(input);

            return hostNameType == UriHostNameType.IPv4 || hostNameType == UriHostNameType.IPv6;
        }
    }


}