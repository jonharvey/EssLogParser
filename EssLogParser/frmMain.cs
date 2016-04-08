using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EssLogParser
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private String _filename = null;
        private DataTable _table = null;
        private BindingSource _binding = null;
        private System.Windows.Forms.DataGridView.HitTestInfo _hit = null;
        private bool _sessionFiltered = false;
        private bool _timestampFiltered = false;
        private bool _msgLevelFiltered = false;
        private bool _msgNumFiltered = false;
        private bool _activityFiltered = false;
        private bool _userFiltered = false;
        private bool _ipFiltered = false;
        private bool _gridLoaded = false;

        #region Form Events
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdEssLog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                _filename = ofdEssLog.FileName;
                txtFilename.Text = _filename;
            }

        }
        private void dgvSessionInfo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _hit = dgvSessionInfo.HitTest(e.X, e.Y);
                ContextMenu m = new ContextMenu();
                if (_hit.ColumnIndex < 0)
                    m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                else
                {
                    switch (_table.Columns[_hit.ColumnIndex].ColumnName)
                    {
                        case "SessionID":
                            m.MenuItems.Add(new MenuItem("Filter only this session ID", new EventHandler(this.FilterSessions)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "Timestamp":
                            m.MenuItems.Add(new MenuItem("Filter only this timestamp", new EventHandler(this.FilterTimestamp)));
                            m.MenuItems.Add(new MenuItem("Filter before date", new EventHandler(this.FilterTimestampBefore)));
                            m.MenuItems.Add(new MenuItem("Filter after date", new EventHandler(this.FilterTimestampAfter)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "MsgLevel":
                            m.MenuItems.Add(new MenuItem("Filter only this message level", new EventHandler(this.FilterMsgLevel)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "MsgNum":
                            m.MenuItems.Add(new MenuItem("Filter only this message number", new EventHandler(this.FilterMsgNum)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "Activity":
                            m.MenuItems.Add(new MenuItem("Filter only this activity", new EventHandler(this.FilterActivity)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "User":
                            m.MenuItems.Add(new MenuItem("Filter only this user", new EventHandler(this.FilterUser)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        case "IP":
                            m.MenuItems.Add(new MenuItem("Filter only this IP", new EventHandler(this.FilterIP)));
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                        default:
                            m.MenuItems.Add(new MenuItem("Clear filters", new EventHandler(this.ClearFilters)));
                            break;
                    }
                }
                m.Show(dgvSessionInfo, new Point(e.X, e.Y));
            }
        }
        private void txtFilename_TextChanged(object sender, EventArgs e)
        {
            if (txtFilename.Text.Length > 0)
                btnParse.Enabled = true;
            else
                btnParse.Enabled = false;
        }
        private void btnParse_Click(object sender, EventArgs e)
        {
            ParseFromFile();
        }
        private void btnShowFilter_Click(object sender, EventArgs e)
        {
            if (_binding.Filter != null)
                MessageBox.Show(_binding.Filter.Replace(" AND ", "\r\nAND "));
        }
        private void dgvSessionInfo_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        private void dgvSessionInfo_DragDrop(object sender, DragEventArgs e)
        {
            string[] selected = ((String[])e.Data.GetData(DataFormats.FileDrop));
            if (selected.Length > 1)
            {
                MessageBox.Show("This program currently only supports opening a single file at a time. Please try again.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this._filename = selected[0];
            ParseFromFile();
            return;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.V))
            {
                ParseFromClipboard();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Utility Functions
        private void AttachUserInfo()
        {
            Regex regex = new Regex(@".*\[(.*)\] from \[(.*)\].*");
            Match match = null;
            List<Object> x = (from r in _table.AsEnumerable() select r["SessionID"]).Distinct().ToList();
            foreach (Double sess in x) 
            {
                DataRow[] results = _table.Select(String.Format("(SessionID = {0}) AND (Activity LIKE 'Logging in user%')",sess));
                if (results.Length > 0)
                {
                    match = regex.Match(results[0]["Activity"].ToString());
                    if(match.Success)
                    {
                        DataRow[] toBeUpdated = _table.Select(String.Format("SessionID = {0}",sess));
                        for (int i = 0; i < toBeUpdated.Length; i++)
                        {
                            toBeUpdated[i]["User"] = match.Groups[1].Value;
                            toBeUpdated[i]["IP"] = match.Groups[2].Value;
                        }
                    }
                }
            }
        }
        private void InitializeDataTable()
        {
            _table = new DataTable();
            _table.Columns.Add(new DataColumn("RecordID", typeof(Double)));
            _table.Columns.Add(new DataColumn("SessionID", typeof(Double)));
            _table.Columns.Add(new DataColumn("Timestamp", typeof(DateTime)));
            _table.Columns.Add(new DataColumn("MsgLevel", typeof(String)));
            _table.Columns.Add(new DataColumn("MsgNum", typeof(Double)));
            _table.Columns.Add(new DataColumn("Activity", typeof(String)));
            _table.Columns.Add(new DataColumn("User", typeof(String)));
            _table.Columns.Add(new DataColumn("IP", typeof(String)));
            _table.Columns["RecordID"].AutoIncrement = true;
            return;
        }
        private void ParseFromClipboard()
        {
            String clipText = null;
            StringReader sr = null;
            if (_gridLoaded)
            {
                DialogResult dr = MessageBox.Show("The grid currently contains data.  Reloading will remove this data.  Proceed with reload?", "Reload Grid", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            try
            {
                clipText = Clipboard.GetText();
                sr = new StringReader(clipText);
                ParseData((TextReader)sr);
            }
            catch (ExternalException extEx)
            {
                MessageBox.Show("Error reading from clipboard.  The clipboard is being used by another process.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("ERROR DETAILS: ", extEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to read from clipboard. No additional details are available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine("ERROR DETAILS: ", ex.Message);
            }

        }
        private void ParseFromFile()
        {
            if (_gridLoaded)
            {
                DialogResult dr = MessageBox.Show("The grid currently contains data.  Reloading will remove this data.  Proceed with reload?", "Reload Grid", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr != System.Windows.Forms.DialogResult.Yes)
                    return;
            }
            if (File.Exists(_filename))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(_filename);
                    ParseData((TextReader)sr);
                }
                catch (IOException ioex) // file doesn't exist
                {
                    MessageBox.Show(ioex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex) // general exception
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Invalid Essbase log file path.");
        }
        private void ParseData(TextReader sr)
        {
            try
            {
                InitializeDataTable();

                Regex regex = new Regex(@"\[(Sun|Mon|Tue|Wed|Thu|Fri|Sat) (.*) (.*) (.*) (.*)\].*/.*/.*/.*/(.*)/(.*)\((.*)\)");
                Match match = null;
                String line = null;
                Double sessionID = 0;
                DateTime stamp = DateTime.Now;
                String msgLevel = null;
                Double msgNum = 0;
                DataRow dr = null;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0) //ignore blank lines
                    {
                        match = regex.Match(line);
                        if (line[0] == '[' && match.Groups.Count > 1) //process timestamp line
                        {
                            Debug.WriteLine(line);
                            Debug.WriteLine(String.Format("Match count:{0}",match.Groups.Count));
                            Debug.WriteLine(String.Format("SessionID={0}|MsgNum={1}",Double.Parse(match.Groups[6].Value),Double.Parse(match.Groups[8].Value)));
                            Debug.WriteLine("");
                            sessionID = Double.Parse(match.Groups[6].Value);
                            stamp = DateTime.ParseExact(String.Format("{0} {1} {2} {3}", match.Groups[2].Value, match.Groups[3].Value, match.Groups[5], match.Groups[4].Value), "MMM dd yyyy HH:mm:ss", null);
                            msgLevel = match.Groups[7].Value;
                            msgNum = Double.Parse(match.Groups[8].Value);
                        }
                        else //process activity line, write row to table
                        {
                            dr = _table.NewRow();
                            dr["SessionID"] = sessionID;
                            dr["Timestamp"] = stamp;
                            dr["MsgLevel"] = msgLevel;
                            dr["MsgNum"] = msgNum;
                            dr["Activity"] = line;
                            _table.Rows.Add(dr);
                        }
                    }
                }
                AttachUserInfo();
                _binding = new BindingSource();
                _binding.DataSource = _table;
                dgvSessionInfo.DataSource = _binding;

                //Set the grid style
                dgvSessionInfo.DefaultCellStyle.SelectionBackColor = Color.Black;
                dgvSessionInfo.DefaultCellStyle.SelectionForeColor = Color.White;
                dgvSessionInfo.Columns["Timestamp"].DefaultCellStyle.Format = "MM/dd/yy HH:mm:ss";
                dgvSessionInfo.Columns["RecordID"].Visible = false;
                dgvSessionInfo.AutoResizeColumns();

                _gridLoaded = true;
            }
            catch (Exception ex) // general exception
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        #endregion

        #region Grid Filtering Functions
        private void ClearFilters(object sender, EventArgs e) 
        {
            int recID = int.Parse(dgvSessionInfo[0,dgvSessionInfo.SelectedCells[0].RowIndex].Value.ToString());
            btnShowFilter.Enabled = false;
            _sessionFiltered = false;
            _timestampFiltered = false;
            _msgLevelFiltered = false;
            _msgNumFiltered = false;
            _activityFiltered = false;
            _userFiltered = false;
            _ipFiltered = false;
            _binding.Filter = null;
            dgvSessionInfo.CurrentCell = dgvSessionInfo[1, recID];
        }
        private void FilterSessions(object sender, EventArgs e)
        {
            if (!_sessionFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("SessionID = {0}", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _sessionFiltered = true;
        }
        private void FilterTimestamp(object sender, EventArgs e)
        {
            if (!_timestampFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("Timestamp = '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _timestampFiltered = true;
        }
        private void FilterMsgLevel(object sender, EventArgs e)
        {
            if (!_msgLevelFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("MsgLevel = '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _msgLevelFiltered = true;
        }
        private void FilterMsgNum(object sender, EventArgs e)
        {
            if (!_msgNumFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("MsgNum = {0}", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _msgNumFiltered = true;
        }
        private void FilterActivity(object sender, EventArgs e)
        {
            if (!_activityFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("Activity = '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _activityFiltered = true;
        }
        private void FilterUser(object sender, EventArgs e)
        {
            if (!_userFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("User = '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _userFiltered = true;
        }
        private void FilterIP(object sender, EventArgs e)
        {
            if(!_ipFiltered)
                _binding.Filter = AppendFilter(_binding.Filter, String.Format("IP = '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
            _ipFiltered = true;
        }
        private void FilterTimestampBefore(object sender, EventArgs e)
        {
            _binding.Filter = AppendFilter(_binding.Filter, String.Format("Timestamp <= '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
        }
        private void FilterTimestampAfter(object sender, EventArgs e)
        {
            _binding.Filter = AppendFilter(_binding.Filter, String.Format("Timestamp >= '{0}'", dgvSessionInfo[_hit.ColumnIndex, _hit.RowIndex].Value));
            btnShowFilter.Enabled = true;
        }
        private String AppendFilter(String old, String newer)
        {
            if (old == null)
                return newer;
            else
            {
                if (old.Contains("Timestamp >=") && newer.Contains("Timestamp >="))
                {
                    return Regex.Replace(old, "Timestamp >= '.*'", newer);
                }
                else if (old.Contains("Timestamp <=") && newer.Contains("Timestamp <="))
                {
                    return Regex.Replace(old, "Timestamp <= '.*'", newer);
                }
                else return String.Format("({0}) AND ({1})", old, newer);
            }
        }
        #endregion

    }

}
