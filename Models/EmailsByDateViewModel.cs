using System;
using System.Collections.Generic;

namespace EmailCampaignApp.Models
{
    public class EmailsByDateViewModel
    {
        public string Date { get; set; }
        public List<SentEmail> Emails { get; set; }
        
    }
}