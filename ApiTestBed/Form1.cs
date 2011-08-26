﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApiTestBed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void GetImage(object sender, EventArgs e)
        {
            Image img = null;

            if (sender == btnGetCharPortrait)
                img = EveApi.ImageApi.GetCharacterPortrait((int)nudCharID.Value, EveApi.ImageApi.ImageRequestSize.Size256x256);
            if (sender == btnGetCorpLogo)
                img = EveApi.ImageApi.GetCorporationLogo((int)nudCorpID.Value, EveApi.ImageApi.ImageRequestSize.Size256x256);

            if (img != null)
            {
                picImage.Image = img;
            }
        }

        private void btnAccontStatus_Click(object sender, EventArgs e)
        {
            EveApi.Account.AccountStatus status = new EveApi.Account.AccountStatus();

            status.ApiUserId = txtUserID.Text;
            status.ApiKey = txtApiKey.Text;

            status.GrabDataFromApi(null);

            StringBuilder statusString = new StringBuilder();

            statusString.AppendLine(string.Format("Paid Until {0} at {1}", status.PaidUntil.ToShortDateString(), 
                                                status.PaidUntil.ToShortTimeString()));
            statusString.AppendLine(string.Format("Created on {0} at {1}", status.CreationDate.ToShortDateString(),
                                                status.CreationDate.ToShortTimeString()));
            statusString.AppendLine(string.Format("Logged on {0} times to CCP", status.NumberOfLogons));
            statusString.AppendLine(string.Format("Played EVE for {0} minutes", status.PlayTimeMinutes));

            MessageBox.Show(statusString.ToString());
        }

        private void btnCharList_Click(object sender, EventArgs e)
        {
            EveApi.Account.CharacterList charList = new EveApi.Account.CharacterList();

            charList.ApiUserId = txtUserID.Text;
            charList.ApiKey = txtApiKey.Text;

            charList.GrabDataFromApi(null);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Characters:");

            foreach (EveApi.Account.CharacterList.CharacterInfo charInfo in charList.Characters)
            {
                sb.AppendLine(string.Format("{0} ({1}) with {2} ({3})", charInfo.Name, charInfo.ID, charInfo.CorporationName, charInfo.CorporationID));
            }

            MessageBox.Show(sb.ToString());
        }

        private void btnServerStatus_Click(object sender, EventArgs e)
        {
            EveApi.ServerStatus status = new EveApi.ServerStatus();

            status.GrabDataFromApi(null);

            MessageBox.Show(string.Format("Server is {0} with {1} players", status.ServerOnline ? "Online" : "Offline", status.NumberOfPlayers));
        }

        private void btnSkillTree_Click(object sender, EventArgs e)
        {
            EveApi.Eve.SkillTree tree = new EveApi.Eve.SkillTree();

            tree.GrabDataFromApi(null);

            MessageBox.Show("Done");
        }

        private void btnAssets_Click(object sender, EventArgs e)
        {
            EveApi.Character.AssetList tree = new EveApi.Character.AssetList();

            tree.ApiUserId = txtUserID.Text;
            tree.ApiKey = txtApiKey.Text;
            tree.CharacterID = int.Parse(txtCharID.Text);

            tree.GrabDataFromApi(null);

            MessageBox.Show("Done");
        }

        private void btnCharAccounts_Click(object sender, EventArgs e)
        {
            EveApi.Character.AccountBalance accounts = new EveApi.Character.AccountBalance();

            accounts.ApiUserId = txtUserID.Text;
            accounts.ApiKey = txtApiKey.Text;
            accounts.CharacterID = int.Parse(txtCharID.Text);

            accounts.GrabDataFromApi(null);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Accounts:");

            foreach (EveApi.Character.AccountBalance.AccountInfo info in accounts.Accounts)
            {
                sb.AppendLine(string.Format("#{0} (ID: {1}) with {2:0,0.00} ISK", info.AccountKey, info.AccountID, info.AccountBalance));
            }

            MessageBox.Show(sb.ToString());
        }

        private void btnCertTree_Click(object sender, EventArgs e)
        {
            EveApi.Eve.CertificateTree tree = new EveApi.Eve.CertificateTree();

            tree.GrabDataFromApi(null);

            MessageBox.Show("Done");
        }

        private void btnCharSheet_Click(object sender, EventArgs e)
        {
            EveApi.Character.CharacterSheet sheet = new EveApi.Character.CharacterSheet();

            sheet.ApiUserId = txtUserID.Text;
            sheet.ApiKey = txtApiKey.Text;
            sheet.CharacterID = int.Parse(txtCharID.Text);

            sheet.GrabDataFromApi(null);

            MessageBox.Show("Done");
        }
    }
}
