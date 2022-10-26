using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections;
using IvanAkcheurov.NTextCat;
using IvanAkcheurov.NTextCat.Lib;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace SubtitleTransferer
{
    // This program relies on the files following The Scene naming conventions for TV shows, mainly the "SxxExx" and "Part.X" (for miniseries)
    // effectiveness varies if TV shows do not follow them
    // more info about the standard:  https://scenerules.org/ (Archive: https://archive.ph/W5dAO )
    public partial class SMR : Form
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        //string resourceName = "Wiki82.profile.xml";
        Stream stream;
        public bool preferClosedCaption;
        public bool bDebugEnabled;
        public bool bCopyFiles;
        // We declare a max number of parenthesis that are allowed in a subtitle file
        // before it is declared to be Closed Captions (for the Hearing impaired)
        public const int ParenthesisLimitForCC = 10; // 10 is the Parenthesis limit used by OpenSubtitle Uploader program

        RankedLanguageIdentifierFactory factory = new RankedLanguageIdentifierFactory();
        //BasicProfileFactoryBase<RankedLanguageIdentifier>
        RankedLanguageIdentifier identifier;



        string strStartFolder = null;
        public ArrayList SubtitleFiles = new ArrayList();
        public SMR()
        {
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Wiki82.profile.xml"));

            stream = assembly.GetManifestResourceStream(resourceName);
            identifier = factory.Load(stream);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initialize();
        }

        private void txt_startFolder_Click(object sender, EventArgs e)
        {

        }

        private void initialize()
        {
            strStartFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            txt_startFolder.Text = strStartFolder;
            preferClosedCaption = chkBoxDefaultCC.Checked;
            bCopyFiles = chkBox_CopyAction.Checked;
            bDebugEnabled = chkBoxDebug.Checked;

            // Keyrum done fallið í lokin
            initialization_done();
        }
        private void initialization_done()
        {
            btn_startScan.Enabled = true;
            ChangeMainLabel("Ready to start Scanning for subtitles");
            //lbl_status.Text = "Ready to start Scanning for subtitles";
        }

        private void select_startFolder(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = strStartFolder;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                strStartFolder = dialog.FileName;
                txt_startFolder.Text = strStartFolder;
            }
            UpdateProgressBar(0);
        }

        private void LogMessage(string msg)
        {
            if (this.txtBox_summary.InvokeRequired)
            {
                this.txtBox_summary.BeginInvoke((MethodInvoker)delegate () { LogMessage(msg); });
            }
            else
            {
                // Regular LogMessage
                txtBox_summary.AppendText(msg + Environment.NewLine);
            }
        }
        private void LogMessage(string msg, bool debuglog)
        {
            if (this.txtBox_summary.InvokeRequired)
            {
                this.txtBox_summary.BeginInvoke((MethodInvoker)delegate () { LogMessage(msg, debuglog); });
            }
            else
            {
                // Message is only relevant if debug is enabled
                if (bDebugEnabled && debuglog)
                {
                    txtBox_summary.AppendText(msg + Environment.NewLine);
                }
            }
        }

        /*
        private void ChangeMainLabel(string msg)
        {

            lbl_status.Text = msg;

            // for long text changes the program started doing tasks that locked the gui before it finished updating the text on the label
            // calling DoEvents makes the application process every change that has been queued
            Application.DoEvents(); 
        }
        */
        private void ChangeMainLabel(string msg, Color? col = null)
        {
            if (this.lbl_status.InvokeRequired)
            {
                this.lbl_status.BeginInvoke((MethodInvoker)delegate () { ChangeMainLabel(msg); });
            }
            else
            {
                lbl_status.Text = msg;
                lbl_status.ForeColor = col ?? Color.Black;

                // for long text changes the program started doing tasks that locked the gui before it finished updating the text on the label
                // calling DoEvents makes the application process every change that has been queued
                Application.DoEvents();
            }
        }
        private void resetSubtitleTransferer()
        {
            if (this.listBox_subtitleFiles.InvokeRequired)
            {
                this.listBox_subtitleFiles.BeginInvoke((MethodInvoker)delegate () { resetSubtitleTransferer(); ; });
            }
            else
            {
                txt_startFolder.Text = "";
                SubtitleFiles.Clear();
                UpdateProgressBar(0);
                listBox_subtitleFiles.Items.Clear();
                btn_SelectAllFiles.Text = "Select All";
            }
        }

        /// <summary>
        /// We do not want the user to mess with the form while the async operation is running so we disable all interactive elements
        /// </summary>
        /// <param name="state"></param>
        private void ToggleAllCheckboxStates(bool state)
        {
            if (this.btn_startScan.InvokeRequired)
            {
                this.btn_startScan.BeginInvoke((MethodInvoker)delegate () { ToggleAllCheckboxStates(state); });
            }
            else
            {
                chkBox_CopyAction.Enabled = state;
                chkBoxDefaultCC.Enabled = state;
                chkBoxDebug.Enabled = state;
            }
        }

        private void ToggleAllButtonsState(bool state)
        {
            if (this.btn_startScan.InvokeRequired)
            {
                this.btn_startScan.BeginInvoke((MethodInvoker)delegate () { ToggleAllButtonsState(state); });
            }
            else
            {
                btn_startScan.Enabled = false;
                btn_SelectAllFiles.Enabled = false;
                btn_moveAndRenameSelectedSubtitles.Enabled = false;
            }
        }

        /// <summary>
        /// This function starts the AsyncStartScan function in a new thread so the main gui won't freeze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_startScan_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                AsyncStartScan();
            }).Start();
        }





        private void AsyncStartScan()
        {
            // TODO: breyta leitarscriptunni, ekki gera hana recursive heldur leita bara eitt dir upp til að finna movie möppur og svo upp aftur til að finna sub möppu
            // accounta fyrir ef maður velur beint movie möppuna, 

            // Set cursor as hourglass
            ChangeCursor("Wait");
            // Disable Checkboxes
            ToggleAllCheckboxStates(false);
            UpdateProgressBar(0); // Reset the progressbar

            // Hreinsum út listann af subtitle skrám ef einhver var til fyrir
            //SubtitleFiles.Clear();
            //listBox_subtitleFiles.Items.Clear();
            resetSubtitleTransferer();
            ChangeMainLabel("Scanning for files..");
            LogMessage("Scanning for files in '" + new DirectoryInfo(strStartFolder).Name + "'");

            // Start scanning the directory for subtitles
            ScanDirectoryForSubtitles(strStartFolder);

            if (SubtitleFiles.Count > 0)
            {
                PopulateListBox();
                ChangeButtonStatus(btn_SelectAllFiles, true);
                //ChangeButtonStatus("btn_SelectAllFiles", true);
                //btn_SelectAllFiles.Enabled = true;
                //lbl_status.Text = "Waiting ...";
                ChangeMainLabel("Waiting...");
                //txtBox_summary.AppendText("found " + SubtitleFiles.Count + " english subtitle files" + Environment.NewLine);
                LogMessage("found " + SubtitleFiles.Count + " english subtitle files");
            } else {
                //lbl_status.Text = "Nothing found";
                ChangeMainLabel("Nothing found");
                //txtBox_summary.AppendText("No subtitles found" + Environment.NewLine);
                LogMessage("No subtitles to work with");

            }

            // Set cursor as default arrow
            ChangeCursor("Normal");
            // Enable Checkboxes
            ToggleAllCheckboxStates(true);
        }

        private void PopulateListBox()
        {
            if (this.listBox_subtitleFiles.InvokeRequired)
            {
                this.listBox_subtitleFiles.BeginInvoke((MethodInvoker)delegate () { PopulateListBox(); });
            }
            else
            {
                listBox_subtitleFiles.Items.Clear();
                
                foreach (Subtitle file in SubtitleFiles)
                //foreach (string filename in SubtitleFiles)
                {
                    if (file != null)
                    {
                        listBox_subtitleFiles.Items.Add(file.SubtitleFilepath);
                    } else
                    {
                        LogMessage("There was an empty subtitle file in our array", true);
                    }
                }

            }
        }

        private void txt_startFolder_TextChanged(object sender, EventArgs e)
        {
            // Svo við keyrum ekki á einhverri bull slóð þá disableum við takkana
            if (txt_startFolder.Text == "")
            {
                // startFolder path is empty
                return;
            }

            DisableActionButtons();
            
            // Náum í slóðina
            strStartFolder = txt_startFolder.Text;

            // Athugum hvort hún sé til
            if (Directory.Exists(strStartFolder))
            {
                // Virkjum aftur Start Scan takkana
                btn_startScan.Enabled = true;
                txt_startFolder.ForeColor = Color.Black;
                lbl_status.ForeColor = Color.Black;
                //lbl_status.Text = "Ready to start Scanning for subtitles";
                ChangeMainLabel("Ready to start Scanning for subtitles");
            }
            else
            {
                txt_startFolder.ForeColor = Color.Red;
                lbl_status.ForeColor = Color.Red;
                //lbl_status.Text = "Invalid directory";
                ChangeMainLabel("Ready to start Scanning for subtitles");
            }
        }

        private void DisableActionButtons()
        {
            btn_startScan.Enabled = false;
            btn_SelectAllFiles.Enabled = false;
            btn_moveAndRenameSelectedSubtitles.Enabled = false;
        }

        private int CountMovieFiles(string sDirPath)
        {
            var RootMovies = Directory.GetFiles(sDirPath, "*.*").Where(s => s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".avi") || s.EndsWith(".m4v"));
            return RootMovies.Count();
        }
        private int CountSubtitles(string sDirPath)
        {
            return new DirectoryInfo(sDirPath).GetFiles("*.srt").Count();
        }
        private int CountDirectories(string sDirPath)
        {
            return new DirectoryInfo(sDirPath).EnumerateDirectories().ToArray().Count();
        }

        private DirectoryInfo[] GetAllDirectories(string sDirPath)
        {
            return new DirectoryInfo(sDirPath).EnumerateDirectories().ToArray();
        }

        private Subtitle[] GetAllSubtitles(string sDirPath, bool SubsForTVShow = false)
        {
            FileInfo[] raw = new DirectoryInfo(sDirPath).GetFiles("*.srt");
            Subtitle[] subs = new Subtitle[raw.Count()];
            int i = 0;
            foreach (FileInfo f in raw)
            {
                subs[i] = new Subtitle(f.FullName, SubsForTVShow);
                i++;
            }
            return subs;
            //return new DirectoryInfo(sDirPath).GetFiles("*.srt");
        }

        /// <summary>
        /// Checks if a directory contains a TV series or a miniseries
        /// </summary>
        /// <param name="sDirPath">Path to the directory to check</param>
        /// <returns></returns>
        private bool DirectoryContainsSeries(string sDirPath)
        {
            // This checks if the directory the path leads to contains a TV series or not
            Regex tvshowreg = new Regex(@".*s\d\de\d\d.*"); // matches *SddEdd*
            Regex miniseriesreg = new Regex(@".*part.\d.*"); // matches *part.d*

            //FileInfo[] tvshows = new DirectoryInfo(sDirPath).GetFiles("*S??E??*");
            string[] tvshows = Directory.GetFiles(sDirPath, "*.*")  // Get all files in folder
                .Select(s => s.ToLowerInvariant()) // Convert the names to lowercase
                .Where(s => tvshowreg.IsMatch(s)) // match them with our regular expression
                .ToArray(); // convert to array

            //FileInfo[] miniseries = new DirectoryInfo(sDirPath).GetFiles("*Part??*");
            string[] miniseries = Directory.GetFiles(sDirPath, "*.*")
                .Select(s => s.ToLowerInvariant()) // Convert the names to lowercase
                .Where(s => miniseriesreg.IsMatch(s)) // match them with our regular expression
                .ToArray(); // convert to array;

            // We have to consider series that do not follow Scene naming rules
            var others = Directory.GetFiles(sDirPath, "*.*").Where(s => s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".avi") || s.EndsWith(".m4v"));

            if(tvshows.Count() > 0 || miniseries.Count() > 0 || others.Count() > 5)
            {
                // If there is at least one file matching either SxxExx or Part.x , or there are more than 5 video files then we return true, otherwise false
                return true;
            } else
            {
                return false;
            }

            //return (tvshows.Count() > 0 || miniseries.Count() > 0) ? true : false; // If there is at least one file matching either SxxExx or Part.x we return true, otherwise false
        }

        private bool CheckforMovieSubtitleCombo(string sDir)
        {
            DirectoryInfo di = new DirectoryInfo(sDir);
            var Movies = Directory.GetFiles(sDir, "*.*").Where(s => s.EndsWith(".mkv") || s.EndsWith(".mp4") || s.EndsWith(".avi") || s.EndsWith(".m4v"));
            var Subtitles = new DirectoryInfo(sDir).GetFiles("*.srt");


            //if (Movies.Count() > 0 && Subtitles.Count() > 0)
            if (Movies.Count() == Subtitles.Count())  // If the subtitles and the video files match we consider this folder already processed
            {
                // We got at least 1 movie file and 1 subtitle file, we need to check if there is a matching movie and subtitle files which signify there is already a renamed subtitle file present
                foreach (string m in Movies)
                {
                    FileInfo mfi = new FileInfo(m);
                    foreach (FileInfo s in Subtitles)
                    {
                        string bareMovieName = mfi.Name.Substring(0, mfi.Name.LastIndexOf('.'));
                        string bareSubtitleName = s.Name.Substring(0, s.Name.LastIndexOf('.'));
                        if (bareMovieName.ToLowerInvariant() == bareSubtitleName.ToLowerInvariant())
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        private int DetectFolderType(string sDir)
        {
            // We need to know what kind of folder we're dealing with
            // - is this a directory containing movies (like a download directory)
            // - is this the directory of movie
            // - is this the directory of a TV series

            DirectoryInfo di = new DirectoryInfo(sDir);
            int movieCount = CountMovieFiles(sDir);
            int subtitleCount = CountSubtitles(sDir);
            int subdirCount = CountDirectories(sDir);

            // we compare the directories in the folder with the movie count. If there are more folders than movies this is most likely a directory root of multiple movies folders
            if (subdirCount > movieCount)
            {
                // This is likely a folder containing multiple movies
                LogMessage("Directory '" + di.Name + "' is likely a root folder");
                return Foldertype.Root;
            }
            else if (subdirCount <= movieCount)
            {
                // This is most likely a movie folder or a TV folder, we need to check which it is
                if (DirectoryContainsSeries(sDir))
                {
                    // This is likely a folder containing a series
                    LogMessage("Directory '" + di.Name + "' likely contains a TV series", true);
                    return Foldertype.TVShow;
                }
                else
                {
                    // This is likely a movie folder
                    LogMessage("Directory '" + di.Name + "' likely contains a movie", true);
                    return Foldertype.Movie;
                }
            }
            else
            {
                // I'm not sure if this will ever trigger but still
                LogMessage("Directory '" + di.Name + "' has unknown folder type", true);
                return Foldertype.Unknown;
            }
        }

        private Subtitle SelectSubtitleFromArray(Subtitle[] subtitles)
        {
            if (subtitles.Count() == 1)
            {
                // There is only one subtitle in this folder, we presume it is english and return it
                return (Subtitle)subtitles[0];
            }
            else
            {
                LogMessage("There are multiple subtitles, we have to look for the english ones", true);
                ArrayList engSub = new ArrayList();
                // We got more subtitles than 1, we go over all of them to find the english ones
                foreach (Subtitle fi in subtitles)
                {
                    if (IsSubtitleEnglish(fi))
                    {
                        engSub.Add(fi);
                    }
                }

                // Check how many english subtitles we got
                if (engSub.Count == 0)
                {
                    // We didn't find any english subtitles
                    LogMessage("We didn't find any english subtitles in directory");
                    return null;
                }
                else if (engSub.Count == 1)
                {
                    // We got 1 english subtitle
                    return (Subtitle)engSub[0];
                }
                else
                {
                    // Copy our arraylist into an array of subtitles
                    Subtitle[] engSubs = new Subtitle[engSub.Count];
                    int i = 0;
                    foreach (Subtitle fi in engSub)
                    {
                        engSubs[i++] = fi;
                    }

                    // We got multiple english subtitles so we call our function that chooses an english sub
                    //SubtitleFiles.Add(SelectEnglishSubtitle(engSub));
                    return SelectEnglishSubtitle(engSubs);
                }
            }
        }

        private void ChangeCursor(string CursorType)
        {

            switch (CursorType)
            {
                case "Wait":
                    //Cursor.Current = Cursors.WaitCursor;
                    this.UseWaitCursor = true;
                    break;
                case "Normal":
                    //Cursor.Current = Cursors.Default;
                    this.UseWaitCursor = false;
                    break;
            }
        }
        //private void UpdateProgressBar(int val)
        private void UpdateProgressBar(double val)
        {

            if (this.progressBar1.InvokeRequired)
            {
                this.progressBar1.BeginInvoke((MethodInvoker)delegate () { UpdateProgressBar(val); });
            }
            else
            {
                int i = Convert.ToInt32(val);
                progressBar1.Value = i;
                Application.DoEvents(); // Wait to finish drawing the updated value before returning
            }
        }

        private void ProcessMovieFolder(DirectoryInfo folderInfo, DirectoryInfo[] subfolders, bool updateProgressBar = true)
        {

            if (subfolders.Count() == 0)
            {
                // There is no folder here, nothing to do here
                LogMessage("We found no subtitle folder in '" + folderInfo.Name + "'");
            }
            else if (subfolders.Count() == 1)
            {
                if (updateProgressBar) { UpdateProgressBar(20); }
                // There is only one subfolder here, we presume this is the subtitle folder
                Subtitle[] subtitles = GetAllSubtitles(subfolders[0].FullName);
                Subtitle oursub = SelectSubtitleFromArray(subtitles);
                if (updateProgressBar) { UpdateProgressBar(45); }
                if (oursub != null)
                {
                    SubtitleFiles.Add(oursub);
                    LogMessage("We selected '" + oursub.SubtitleFileInfo.Name + "' ", true);

                    if (updateProgressBar) { UpdateProgressBar(70); } // We only go to 70 since we still have to transfer the subtitle
                }
            }
            else
            {
                // There are actually more subfolders here
                // TODO: loop through folders and find the one containing subtitles
                LogMessage("We found more than 1 subfolder in '" + folderInfo.Name + "', not sure what to do");
            }
        }
        private void ProcessTVFolder(DirectoryInfo folderInfo, DirectoryInfo[] subfolders, bool updateProgressBar = true)
        {
            if (subfolders.Count() == 0)
            {
                // There is no folder here, nothing to do here
                LogMessage("There is no subfolder in '" + folderInfo.Name + "' containing subtitles, nothing to do");
                return;
            }
            else if (subfolders.Count() == 1)
            {
                // we found 1 subfolder which we assume is the subs folder
                LogMessage("Processing TV subtitles for " + folderInfo.Name);

                // TV Shows have subtitles each in their own folders
                DirectoryInfo[] tvshowSubtitleFolders = subfolders[0].EnumerateDirectories().ToArray();

                // Calculate our Progressbar increments
                int shows = tvshowSubtitleFolders.Count();
                double ProgressbarIncrementValue = 80.0 / shows;
                int ProgressBarIncrementRun = 0;

                foreach (DirectoryInfo di in tvshowSubtitleFolders)
                {
                    // We only update the progressbar if the updateProgressBar is true
                    if (updateProgressBar)
                    {
                        UpdateProgressBar(++ProgressBarIncrementRun * ProgressbarIncrementValue);
                    }

                    // We get all subtitles that are present and mark them as for a subtitle
                    Subtitle[] tvshowSubtitles = GetAllSubtitles(di.FullName, true);

                    // We choose the preferred subtitle from the lot
                    Subtitle oursub = SelectSubtitleFromArray(tvshowSubtitles);
                    SubtitleFiles.Add(oursub);
                }
                UpdateProgressBar(80); // We only go to 80% since we still have to transfer the subtitle
                return;
            }
            else
            {
                // There are many folders here.. we need to do something
                // TODO: loop through folders and find the one containing subtitles
                LogMessage("We found more than 1 subfolder in '" + folderInfo.Name + "', not sure what to do");
            }
        }

        private void ScanDirectoryForSubtitles(string sDir)
        {
            // Check if the folder already has a movie+subtitle combo in which case there is no need to continue
            if (CheckforMovieSubtitleCombo(sDir))
            {
                LogMessage("This folder has Video+Subtitle files that share the same name, folder has most likely already been processed");
                return;
            }

            // First we detect the folder type
            int type = DetectFolderType(sDir);

            // Get folder info for our sDir
            DirectoryInfo folderInfo = new DirectoryInfo(sDir);
            // Get subfolders
            DirectoryInfo[] subfolders = folderInfo.EnumerateDirectories().ToArray();

            // We do different things depending on what kind of folder it is
            switch (type)
            {
                case Foldertype.Root:
                    //TODO: Fix progressbar to support this type of a folder process

                    // Calculate our Progressbar increments
                    double realIncrementValue = (double)(70.0 / subfolders.Count());
                    //int ProgressbarIncrementValue = (int)Math.Ceiling(70.0 / subfolders.Count());
                    int ProgressBarIncrementRun = 0;

                    foreach (DirectoryInfo subfolder in subfolders)
                    {
                        // We skip "system" folders if they are present, these can be "System Volume Information" folders and others that most likely don't contain media and so it's pointless to process them
                        if (subfolder.Attributes.HasFlag(System.IO.FileAttributes.System))
                        { continue; }

                        // update the probressbar
                        double progress = ++ProgressBarIncrementRun * realIncrementValue;
                        UpdateProgressBar(progress);

                        if (CheckforMovieSubtitleCombo(subfolder.FullName))
                        {
                            string message = "We are skipping " + subfolder.Name + ", folder has most likely already been processed";
                            if(bDebugEnabled) { // We show more information
                                message = "We are skipping " + subfolder.Name + ". Folder contains Video+Subtitle files that share the same names, folder has likely already been processed";
                            }
                            LogMessage(message);
                            continue;
                        }

                        LogMessage("We are processing subfolder '" + subfolder.Name + "'");
                        int rootsubfoldertype = DetectFolderType(subfolder.FullName);
                        DirectoryInfo rootsubfolderInfo = new DirectoryInfo(subfolder.FullName);
                        DirectoryInfo[] rootsubfolders = rootsubfolderInfo.EnumerateDirectories().ToArray();

                        //check the folder type of our subfolder
                        switch (rootsubfoldertype)
                        {
                            case Foldertype.Movie:
                                ProcessMovieFolder(rootsubfolderInfo, rootsubfolders, false);
                                break;
                            case Foldertype.TVShow:
                                ProcessTVFolder(rootsubfolderInfo, rootsubfolders, false);
                                break;
                            default:
                                // We only go 1 level up so we do nothing
                                LogMessage("subfolder is of type'" + rootsubfoldertype + "' and we are only processing Movies and TV shows");
                                break;
                        }
                    }
                    UpdateProgressBar(70); // We only go to 70% since we still have to transfer the subtitles
                    break;
                case Foldertype.Movie: // The current folder is a movie folder, we look for subtitles
                    // Call our function to process a Movie folder
                    ProcessMovieFolder(folderInfo, subfolders);
                    break;
                case Foldertype.TVShow:
                    // Call our function to process a TV folder
                    ProcessTVFolder(folderInfo, subfolders);
                    break;
                case Foldertype.Unknown:
                    // The detected foldertype is unknown, we don't know what to do
                    LogMessage("We couldn't accurately predict the folder type for '" + folderInfo.Name + "', no action taken");
                    break;
            } // end of switch (type)
        }

        private Subtitle SelectEnglishSubtitle(ArrayList al)
        {
            Subtitle[] sl = new Subtitle[al.Count];
            int i = 0;
            foreach (Subtitle s in al)
            {
                sl[i] = s;
                i++;
            }
            return SelectEnglishSubtitle(sl);
        }
        private Subtitle SelectEnglishSubtitle(Subtitle[] al)
        {
            // First we find all subtitles that are Closed Caption
            ArrayList ccSubs = new ArrayList();
            ArrayList normalSubs = new ArrayList();
            foreach (Subtitle s in al)
            {
                if (s.SubtitleIsForClosedCaption)
                {
                    ccSubs.Add(s);
                } else
                {
                    normalSubs.Add(s);
                }
            }

            if (preferClosedCaption && ccSubs.Count == 1)
            {
                // We prefer closed caption and we only got 1 cc subtitle so we return that
                Subtitle oursub = (Subtitle)ccSubs[0];
                return oursub;
            }
            else if (preferClosedCaption && ccSubs.Count > 1)
            {
                // We prefer closed caption and we have multiple cc subtitles so we return the largest one
                FileInfo[] ccFI = new FileInfo[ccSubs.Count];
                int ccsize = 0;
                foreach (Subtitle cc in ccSubs)
                {
                    ccFI[ccsize] = cc.SubtitleFileInfo;
                    ccsize++;
                }
                // TODO: Hérna er failure point..  ég er að búa til subtitle aftur
                //Subtitle oursub = ccSubs. new Subtitle(ccFI.OrderByDescending(x => x.Length).First());
                FileInfo bfi = ccFI.OrderByDescending(x => x.Length).FirstOrDefault();
                return SelectSubtitleFromArray(ccSubs, bfi);
            }
            else if (!preferClosedCaption && normalSubs.Count == 0)
            {
                // We do not prefer closed caption but we have no normal subtitles
                // So we must choose one of the closed captions
                if (ccSubs.Count > 0)
                {
                    // We have 1+ closed caption subtitles
                    FileInfo[] normalFI = new FileInfo[ccSubs.Count];
                    int size = 0;
                    foreach (Subtitle cc in ccSubs)
                    {
                        normalFI[size] = cc.SubtitleFileInfo;
                        size++;
                    }
                    
                    FileInfo bfi = normalFI.OrderByDescending(x => x.Length).FirstOrDefault();
                    return SelectSubtitleFromArray(ccSubs, bfi);

                } else
                {
                    // There are no cc subtitles
                    LogMessage("We have no english subtitles to select");
                    return null;
                }
            }
            else if ((!preferClosedCaption && normalSubs.Count == 1) || (preferClosedCaption && ccSubs.Count == 0 && normalSubs.Count == 1))
            {
                // We do not prefer closed caption and we only have 1 non-cc subtitle so we return that
                // or we prefer closed caption but there are no cc subtitles so we return the only normal one we have
                Subtitle oursub = (Subtitle)normalSubs[0];
                return oursub;
            }
            else if ((!preferClosedCaption && normalSubs.Count > 1) || (preferClosedCaption && ccSubs.Count == 0 && normalSubs.Count > 1))
            {
                // We do not prefer closed caption and we have multiple non-cc subtitles so we return the largest one
                // or we prefer closed caption but there are no cc subtitles and we have more that one normal subtitles
                FileInfo[] normalFI = new FileInfo[normalSubs.Count];
                int size = 0;
                foreach (Subtitle normal in normalSubs)
                {
                    normalFI[size] = normal.SubtitleFileInfo;
                    size++;
                }

                FileInfo bfi = normalFI.OrderByDescending(x => x.Length).FirstOrDefault();
                return SelectSubtitleFromArray(normalSubs, bfi);
            }
            else
            {
                // Something is weird, this should not happen
                LogMessage("Something is weird,  we could not find a preferred subtitle");
                return null;
            }
        }

        private void btnSelectAllFiles_Click(object sender, EventArgs e)
        {
            bool action = btn_SelectAllFiles.Text == "Select All" ? true : false;
            btn_moveAndRenameSelectedSubtitles.Enabled = action;
            if (action == true)
            {
                btn_SelectAllFiles.Text = "Clear Selection";
            } else
            {
                btn_SelectAllFiles.Text = "Select All";
            }

            for (int i = 0; i < listBox_subtitleFiles.Items.Count; i++)
            {
                listBox_subtitleFiles.SetItemChecked(i, action);
            }
        }

        private Subtitle SelectSubtitleFromArray(ArrayList SubArray, FileInfo bfi)
        {
            // if our bfi is null we return null
            if(bfi == null) { LogMessage("bfi was null, this should not happen", true); return null; }

            // Loop through all the subtitles in the arraylist and search for a match
            foreach (Subtitle s in SubArray)
            {
                if (s.SubtitleFileInfo.FullName == bfi.FullName)
                {
                    //LogMessage("subtitle " + s.SubtitleFileInfo.FullName + " is equal to " + bfi.FullName, true);
                    return s;
                }
            }
            LogMessage("We found no matching subtitle, this should not happen", true);
            return null;
        }

        private void ExecuteTransfer(bool rename = false)
        {
            // Disable Checkboxes
            ToggleAllCheckboxStates(false);
            // Disable Buttons
            ToggleAllButtonsState(false);

            int SuccessfulTransfers = 0;
            int UnsuccessfulTransfers = 0;

            string ActionName;
            if (bCopyFiles)
            {
                ActionName = "Copy and Rename";
            }
            else
            {
                ActionName = "Move and Rename";
            }

            // Put a message in the summary and change the mainlabel
            LogMessage("executing '" + ActionName + "' action on " + listBox_subtitleFiles.CheckedItems.Count + " files");
            ChangeMainLabel("executing " + ActionName + " action");

            //TODO: Þar sem við erum bara með pathana í Listboxinu, þá þurfum við að matcha slóðina við subtitleinn í arrayinu okkar
            ArrayList subtitlesToTransfer = new ArrayList();

            // förum í gegnum skrárnar sem eru merktar
            foreach (string filepath in listBox_subtitleFiles.CheckedItems)
            {
                // Now we loop through the subtitle array and pick out the chosen subtitles
                foreach (Subtitle s in SubtitleFiles)
                {
                    if (s.SubtitleFilepath == filepath)
                    {
                        subtitlesToTransfer.Add(s);
                    }
                }
            }

            // Calculate progressbar leftovers
            int progressbarLeftover = 100 - progressBar1.Value;
            double progressbarIncrement = (double)(progressbarLeftover / subtitlesToTransfer.Count);
            int progressbarIncrementRun = 0;

            // Loop through all the subtitles we are transfering
            foreach (Subtitle s in subtitlesToTransfer)
            {
                // Update our progressbar
                double progress = ++progressbarIncrementRun * progressbarIncrement;
                UpdateProgressBar(progress);

                if (File.Exists(s.SubtitleFilepath) && !File.Exists(s.SubtitleTransferDestination))
                {
                    // Subtitle File exists and the destination file does not
                    LogMessage("conditions OK for " + s.SubtitleFilename + ", will be transferred", true);

                    try
                    {
                        // We attempt to transfer the subtitle
                        if (bCopyFiles)
                        {
                            // We copy the subtitle
                            File.Copy(s.SubtitleFilepath, s.SubtitleTransferDestination);
                            SuccessfulTransfers++;
                        }
                        else
                        {
                            // We move the subtitle
                            File.Move(s.SubtitleFilepath, s.SubtitleTransferDestination);
                            SuccessfulTransfers++;
                        }
                        LogMessage(ActionName + " complete: " + s.SubtitleFilepath + " to " + s.SubtitleTransferDestination, true);
                    }
                    catch (Exception err)
                    {
                        UnsuccessfulTransfers++;
                        LogMessage("ERROR happened while copying " + s.SubtitleFilename + ": " + err.Message);
                    }
                }
                else
                {
                    UnsuccessfulTransfers++;
                    // Either the subtitle file no longer exists or the destination file already exists... either is bad
                    LogMessage("conditions are NOT ok for " + s.SubtitleFilename + ", file not transferred", true);
                }
            }

            // Done processing all the subtitles, check if there were any errors
            if (UnsuccessfulTransfers == 0)
            {
                LogMessage("'" + ActionName + "' has been completed, " + SuccessfulTransfers + " subtitles successfully transferred");
                ChangeMainLabel("All done, select new folder to scan");
                LogMessage("All done, select new folder to scan");
            } else
            {
                // There was an error transfering 1 or more files, show an error message
                LogMessage("'" + ActionName + "' has been completed, " + SuccessfulTransfers + " subtitles successfully transferred, error transferring " + UnsuccessfulTransfers);
                ChangeMainLabel("All done, 1 or more errors occured", Color.Red);
                LogMessage("All done, select new folder to scan");
            }

            // Reset the program
            resetSubtitleTransferer();

            // Set the progressbar to 0
            UpdateProgressBar(100);

            // Enable Checkboxes
            ToggleAllCheckboxStates(true);
            // Disable Buttons
            //ToggleAllButtonsState(false);

        }

        private void resetSubtitleTransferer2()
        {
            // Reset the form
            txt_startFolder.Text = "";
            SubtitleFiles.Clear();
            listBox_subtitleFiles.Items.Clear();
            btn_SelectAllFiles.Text = "Select All";
        }
 
        private void btn_moveAndRenameSelectedSubtitles_Click(object sender, EventArgs e)
        {
            ExecuteTransfer();
        }

        private bool CheckIfMovieFileExists(string path)
        {
            // File extensions of movie files
            string[] movieExtensions = { ".avi", ".mp4", ".mkv", ".wmv" };

            // Get Files in directory
            DirectoryInfo folderInfo = new DirectoryInfo(path);
            FileInfo[] fileinfos = folderInfo.GetFiles();
            foreach(FileInfo f in fileinfos)
            {
                string ext = Path.GetExtension(f.FullName).ToLower();
                if(movieExtensions.Contains(ext))
                {
                    // We found a movie file in the directory
                    return true;
                } 
            }

            // No movie files in th directory
            return false;
        }

        private string parentFolder(string filepath)
        {
            // Return Parent path
            return System.IO.Directory.GetParent(filepath).FullName;
        }
        private string gparentFolder(string filepath)
        {
            // Return Grandparent path
            return parentFolder(parentFolder(filepath));
            // return Directory.GetParent(Parent).FullName;
        }

        private void listBox_subtitleFiles_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Þessi event skýtur þegar átt er við Check statusinn á einhverju í listanum
            // svo við athugum hvort við erum að virkja eitthvað eða afvirkja
            if (e.NewValue == CheckState.Checked)
            {
                btn_moveAndRenameSelectedSubtitles.Enabled = true;
                // We have to check if every item in the list is checked, if so we change the name of the button
                if(listBox_subtitleFiles.Items.Count == listBox_subtitleFiles.CheckedItems.Count + 1) //We add +1 because we are already checking one item
                {
                    btn_SelectAllFiles.Text = "Clear";
                } else
                {
                    btn_SelectAllFiles.Text = "Select All";
                }
            }
            else
            {
                // Athugum hvort það sé eitthvað annað valið
                if (listBox_subtitleFiles.CheckedItems.Count > 1) // Það verður alltaf allavega 1 í checkedItems þar sem við erum að breyta stöðunni á iteminu
                {
                    // Allavega eitt annað er valið, virkjum Execute takkann
                    btn_moveAndRenameSelectedSubtitles.Enabled = true;
                    btn_SelectAllFiles.Text = "Clear";
                }
                else
                {
                    btn_moveAndRenameSelectedSubtitles.Enabled = false;
                    btn_SelectAllFiles.Text = "Select All";
                }
            }
        }

        private bool IsSubtitleEnglish(FileInfo file)
        {
            // Cheat, convert the FileInfo to Subtitle and just call the real function with it 
            Subtitle sf = new Subtitle(file);
            return IsSubtitleEnglish(sf);
        }
        private bool IsSubtitleEnglish(Subtitle file)
        {
            try { 
                string lang;
                string langCode;

                // Read the srt file
                string srtContent = System.IO.File.ReadAllText(file.SubtitleFilepath).ToLowerInvariant();

                // Run NTextCat on the content to identify the language
                var languages = identifier.Identify(srtContent);

                var mostCertainLanguage = languages.FirstOrDefault();
                if (mostCertainLanguage != null)
                {
                    lang = mostCertainLanguage.Item1.EnglishName;
                    langCode = mostCertainLanguage.Item1.Iso639_3;
                } else {
                    // There is no certain language detected so we assume it's not in english
                    LogMessage("There was no mostCertainLanguage detected for file '" + file.SubtitleFolder + "/" + file.SubtitleFilename + "'",true); // Debug Message
                    return false;
                }

                // Now we have the identified language so we need to check what it is, this is simple we just check if we got 'simple'
                if (langCode == "simple" || langCode == "en")
                {
                    // NTextCat detected english so we return true
                    LogMessage("The file '" + file.SubtitleFolder + "/" + file.SubtitleFilename + "' is english",true);
                    return true;
                }
                else
                {
                    // subtitle is not english
                    //LogMessage("The file '" + file.SubtitleFolder + "/" + file.SubtitleFilename + "' is not English, detected language is '" + langCode + "'",true); // We comment this out to reduce noise in the summary log
                    return false;

                    // If we need to further analyze what language the subtitle is in we can use CultureInfo to convert the 2 letter Lang code into the name of the language/local
                    /*
                    string engname;
                    CultureInfo ci;
                    ci = CultureInfo.GetCultureInfoByIetfLanguageTag(langCode);
                    engname = ci.EnglishName;
                    */
                }
            } catch(Exception e)
            {
                // oh no,  there was an error identifying the language
                LogMessage("Exception happened while identifying language of file '" + file.SubtitleFilename + "', error is " + e.Message,true);
                return false;
            }
        }

        /* Moved inside the Subtitle class
        public bool isSubtitleClosedCaptions(FileInfo file)
        {
            // Cheat, convert the FileInfo to Subtitle and just call the real function with it 
            Subtitle sf = new Subtitle(file);
            return isSubtitleClosedCaptions(sf);
        }
        private bool isSubtitleClosedCaptions(Subtitle sub)
        {
            string subtitleContent = System.IO.File.ReadAllText(sub.SubtitleFilepath);
            int parenthesisCount = Regex.Matches(subtitleContent, @"\(.*\)").Count;
            int bracketCount = Regex.Matches(subtitleContent, @"\[.*\]").Count;
            int braceCount = Regex.Matches(subtitleContent, @"\{.*\}").Count;

            if (parenthesisCount > ParenthesisLimitForCC)
            {
                // There are more parenthesis in the file than the limit so we declare it to be CC subtitle
                LogMessage(sub.SubtitleFilename + " IS closed captions");
                return true;
            }
            else
            {
                // There are less parenthesis in the file so it's probably just a normal subtitle file
                LogMessage(sub.SubtitleFilename + " is NOT closed captions");
                return false;
            }
        }
        */

        //public void ChangeButtonStatus(string ButtonName, bool State)
        public void ChangeButtonStatus(Button btn, bool State )
        {
            if (this.lbl_status.InvokeRequired)
            {
                //this.lbl_status.BeginInvoke((MethodInvoker)delegate () { ChangeButtonStatus(ButtonName, State); });
                this.lbl_status.BeginInvoke((MethodInvoker)delegate () { ChangeButtonStatus(btn, State); });
            }
            else
            {
                //Button btn = Controls.OfType<Button>().FirstOrDefault(b => b.Name == ButtonName);
                btn.Enabled = State;
            }
        }

        /// <summary>
        /// This is a test button, used when testing and debugging. This button is not visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            CheckforMovieSubtitleCombo(txt_startFolder.Text);


            return;
            //DirectoryContainsSeries(dir);
            DirectoryInfo folderInfo = new DirectoryInfo(txt_startFolder.Text);
            FileInfo[] fileinfos = folderInfo.GetFiles("*.srt");

            foreach (FileInfo fi in fileinfos)
            {
                IsSubtitleEnglish(fi);
                //bool isCC = isSubtitleClosedCaptions(fi);
                continue;

                string filename = fi.Name;
                string lang;
                string langCode;

                // Read the srt file
                string srtContent = System.IO.File.ReadAllText(fi.FullName);

                //var factory = new RankedLanguageIdentifierFactory();
                //var identifier = factory.Load("Wiki82.profile.xml"); // can be an absolute or relative path. Beware of 260 chars limitation of the path length in Windows. Linux allows 4096 chars.
                var languages = identifier.Identify(srtContent);

                var mostCertainLanguage = languages.FirstOrDefault();
                if (mostCertainLanguage != null)
                {
                    lang = mostCertainLanguage.Item1.EnglishName;
                    langCode = mostCertainLanguage.Item1.Iso639_3;
                    //Console.WriteLine("The language of the text is '{0}' (ISO639-3 code)", mostCertainLanguage.Item1.Iso639_3);
                }
                else
                {
                    lang = "Unknown";
                    langCode = "N/A";
                    //Console.WriteLine("The language couldn’t be identified with an acceptable degree of certainty");
                }
                string results = filename + " is most likely '" + lang + "' (" + langCode + ")";

                CultureInfo ci;
                string engname;
                if (langCode == "simple")
                {
                    engname = "english";
                }
                else
                {
                    ci = CultureInfo.GetCultureInfoByIetfLanguageTag(langCode);
                    engname = ci.EnglishName;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            preferClosedCaption = chkBoxDefaultCC.Checked;
            SubtitleFiles.Clear(); // Clear the listbox,  if this changed then we will have to scan the subtitles again
            listBox_subtitleFiles.Items.Clear();
            btn_SelectAllFiles.Enabled = false;
            LogMessage("SDH/Closed Captions switch toggled " + preferClosedCaption);
        }

        private void chkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            bDebugEnabled = chkBoxDebug.Checked;
            LogMessage("debug logging switch toggled " + bDebugEnabled);
        }
        private void ClearSummary(object sender, EventArgs e)
        {
            if (this.txtBox_summary.InvokeRequired)
            {
                this.txtBox_summary.BeginInvoke((MethodInvoker)delegate () { ClearSummary(sender, e); ; });
            }
            else
            {
                txtBox_summary.Text = "";
                LogMessage("Cleared Summary");
            }
            
        }

        private void chkBox_CopyAction_CheckedChanged(object sender, EventArgs e)
        {
            bCopyFiles = chkBox_CopyAction.Checked;
            LogMessage("Copy/Move subtitle switch toggled " + bCopyFiles);
        }
    }
    class Foldertype
    {
        public const int Unknown = 0;
        public const int Root = 1;
        public const int Movie = 2;
        public const int TVShow = 3;
    }

    class Subtitle
    {
        private const int TVShowDirectoryTransfer = 2;

        public string Name;
        public string Value;

        public string SubtitleFilepath;
        public string SubtitleFilename;
        public string SubtitleFolder;
        public bool SubtitleIsForClosedCaption;
        public FileInfo SubtitleFileInfo;
        public bool SubtitleSourceIsTVSeries;
        public string SubtitleTransferDestination;
        public Subtitle(string Filepath, bool isTV = false)
        {
            SubtitleFilepath = Filepath;
            SubtitleFileInfo = new FileInfo(Filepath);
            SubtitleFilename = SubtitleFileInfo.Name;
            SubtitleFolder = SubtitleFileInfo.DirectoryName;
            SubtitleIsForClosedCaption = isSubtitleClosedCaptions(SubtitleFileInfo);
            SubtitleSourceIsTVSeries = isTV;
            SubtitleTransferDestination = DestinationTarget();
            Name = SubtitleFilename;
            Value = SubtitleFilename;
    }

        public Subtitle(FileInfo subtitle, bool isTV = false)
        {
            SubtitleFileInfo = subtitle;
            SubtitleFilepath = subtitle.FullName;
            SubtitleFilename = subtitle.Name;
            SubtitleFolder = subtitle.DirectoryName;
            SubtitleIsForClosedCaption = isSubtitleClosedCaptions(SubtitleFileInfo);
            SubtitleSourceIsTVSeries = isTV;
            SubtitleTransferDestination = DestinationTarget();
            Name = SubtitleFilename;
            Value = SubtitleFilename;
        }

        private string DestinationTarget()
        {
            string path;
            string newfilename;
            string newfilepath;
            if (this.SubtitleSourceIsTVSeries)
            {
                path = Path.GetFullPath(Path.Combine(SubtitleFileInfo.DirectoryName, "..", ".."));
                newfilename = SubtitleFileInfo.Directory.Name;
                newfilepath = Path.GetFullPath(Path.Combine(path, newfilename)) + ".srt";
            }
            else
            {
                path = Path.GetFullPath(Path.Combine(SubtitleFileInfo.DirectoryName, ".."));
                
                // Find the largest 
                newfilename = new DirectoryInfo(path).EnumerateFiles()
                       .OrderByDescending(f => f.Length)
                       .FirstOrDefault().Name;
                newfilename = newfilename.Substring(0, newfilename.LastIndexOf('.')) + ".srt";

                // We combine everything into a new destination path
                newfilepath = Path.Combine(path, newfilename);  // We combine our path with our newfilename
                //newfilepath = newfilepath.Substring(0, newfilepath.LastIndexOf('.'))+".srt"; // We cut off the fileending
            }
            return newfilepath;
        }
        
        private bool isSubtitleClosedCaptions(FileInfo fi)
        {
            string subtitleContent = System.IO.File.ReadAllText(fi.FullName);

            // We look for tell tale signals of Closed Captions subtitles which are usually () or [] around descriptions
            // of sounds that are happening or use eight note symbols (music notes) to describe music that's playing
            int parenthesisCount = Regex.Matches(subtitleContent, @"\(.*\)").Count;
            int bracketCount = Regex.Matches(subtitleContent, @"\[.*\]").Count;

            // Brace/{} symbol count and eight note symbol/♪ counts are skipped to safe time while processing subtitles
            // but it's left commented out for the future in case the parenthesis and brackets don't cut it
            //int braceCount = Regex.Matches(subtitleContent, @"\{.*\}").Count;
            //int musicCount = Regex.Matches(subtitleContent, @"\♪.*\♪").Count;

            if ( (parenthesisCount > 10) || (bracketCount > 10))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
