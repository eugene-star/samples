using SIAMTest.Formats;
using SIAMTest.Loaders;
using SIAMTest.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIAMTest
{
    public partial class MainForm : Form
    {
        int _filesLoaded = 0;
        TaskScheduler _uiScheduler;

        public MainForm()
        {
            InitializeComponent();
        }

        private void LoadFileBtn_Click(object sender, EventArgs e)
        {
            Uri uri;
            ProcessState state = new ProcessState();

            try
            {
                uri = new Uri(FtpURLText.Text);
            }
            catch (UriFormatException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return;
            }

            // выясняем, из какого источника будем загружать
            if (uri.Scheme == Uri.UriSchemeFtp)
                state.Loader = new FTPLoader(uri);
            else if (uri.Scheme == Uri.UriSchemeHttp)
            {
                state.Loader = new WebServiceLoader(uri);
                state.Parser = new XMLParser();
            }
            else if (uri.IsUnc)
                state.Loader = new UNCLoader(uri);
            else
            {
                MessageBox.Show("Неизвестный источник.", "Ошибка");
                return;
            }

            // выясняем тип файла
            if (uri.Scheme != Uri.UriSchemeHttp)
            {
                string[] filename = uri.Segments[uri.Segments.Length - 1].Split('.');
                switch (filename[filename.Length - 1].ToLower())
                {
                    case "csv":
                        state.Parser = new CSVParser();
                        break;
                        
                    case "xml":
                        state.Parser = new XMLParser();
                        break;

                    default:
                        MessageBox.Show("Неизвестный формат.", "Ошибка");
                        return;
                }
            }

            Task.Factory.StartNew((p) =>
                {
                    ProcessFile((ProcessState)p);
                }, state).ContinueWith((cw) => 
                { 
                    UpdateProgress(cw); 
                });
        }

        /// <summary>
        /// Загрузка и обработка файла.
        /// </summary>
        /// <param name="param"></param>
        private void ProcessFile(ProcessState param)
        {
            IEnumerable<Order> orders;

            try
            {
                using (Stream stream = param.Loader.LoadFile())
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    orders = param.Parser.GetObjects(stream);
                }

                foreach (var order in orders)
                    DbHelper.SaveOrder(order);
            }
            catch (Exception ex)
            {
                param.Error = ex;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private void UpdateProgress(Task cw)
        {
            Task.Factory.StartNew(() =>
            {
                ProcessState state = cw.AsyncState as ProcessState;
                if (state.Error != null)
                    MessageBox.Show(state.Error.Message, "Ошибка");
                else
                    FilesLoadedLabel.Text = (++_filesLoaded).ToString();
            }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
        } 
    }

    public class ProcessState
    {
        public FileLoader Loader { get; set; }
        public StreamParser Parser { get; set; }
        public Exception Error { get; set; }
    }
}








