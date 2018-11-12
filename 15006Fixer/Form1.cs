using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace _15006Fixer
{
    public partial class Form1 : Form
    {
        string hostname = "public.universe.robertsspaceindustries.com";
        string fixerNote = "#Following line added by SC 15006 Fixer:";
        string hostsPath = "C:/Windows/System32/drivers/etc/hosts";
        Dictionary<string, string> servers = new Dictionary<string, string> { { "US 1", "50.17.105.39" }, { "US 2", "18.215.191.91" }, { "US 3", "100.24.145.206" }, { "US 4", "52.204.2.76" }, { "Asia 1", "13.237.27.15" }, { "Asia 2", "52.62.175.64" } };

        public Form1()
        {
            InitializeComponent();
            serverSelect.DataSource = new List<string>(servers.Keys);
            serverSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            RefreshState();
        }
        void RefreshState()
        {
            if (File.Exists(hostsPath))
            {
                string[] lines = File.ReadAllLines(hostsPath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i] == fixerNote)
                    {
                        string[] entry = lines[i + 1].Split(' ');
                        foreach (var v in servers)
                        {
                            if (entry[0] == v.Value)
                            {
                                statusLabel.Text = "Fix applied, rerouted to: " + v.Key;
                                return;
                            }
                        }
                        statusLabel.Text = "Fix applied, rerouted to: " + "[failed to determine]";
                        return;
                    }
                }
                statusLabel.Text = "No fix applied";
            }
            else statusLabel.Text = "Hosts file not found";
        }
        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (!File.Exists(hostsPath))
            {
                MessageBox.Show("It seems that your PC does not have a hosts file. This program will add it, but it is likely that this fix won't work.");
                File.Create(hostsPath);
            }

            List<string> lines = new List<string>(File.ReadAllLines(hostsPath));
            bool foundExisting = false;
            string entry = servers[serverSelect.Text] + ' ' + hostname + "\r\n";
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == fixerNote)
                {
                    lines[i + 1] = entry;
                    foundExisting = true;
                }
            }
            if (!foundExisting)
            {
                lines.Add(fixerNote);
                lines.Add(entry);
            }
            File.WriteAllLines(hostsPath, lines, Encoding.ASCII);
            MessageBox.Show("Applied. Be sure to revert any changes if the fix does not work! Restarting your computer might help if the fix is not effective straight away.");
            RefreshState();
        }

        private void buttonRevert_Click(object sender, EventArgs e)
        {
            if (!File.Exists(hostsPath))
            {
                MessageBox.Show("Hosts file does not exist. There's nothing to revert.");
            }
            else
            {

                List<String> lines = new List<string>(File.ReadAllLines(hostsPath));
                int entryIndex = -1;
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i] == fixerNote)
                    {
                        entryIndex = i;
                    }
                }
                if (entryIndex != -1)
                {
                    lines.RemoveAt(entryIndex+1);
                    lines.RemoveAt(entryIndex);
                    File.WriteAllLines(hostsPath, lines, Encoding.ASCII);
                    MessageBox.Show("All changes reverted successfully.");
                }
                else
                {
                    MessageBox.Show("Hosts file does not contain any changes. There's nothing to revert!");
                }
            }
            RefreshState();
        }
    }
}
