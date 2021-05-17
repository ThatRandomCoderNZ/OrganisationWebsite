using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.Models
{
    public class NewTaskDetails
    {
        public string Name { get; set; }

        public string Notes { get; set; }

        public string GoalID { get; set; }

        public string UserID { get; set; }

        public string Duration { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}