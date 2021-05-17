using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.Models
{
    public class DetailsFormData
    {
        public string TaskName { get; set; }

        public string TaskNotes  { get; set; }

        public string TaskDuration { get; set; }

        public string HistoryDetails { get; set; }

		public string TaskID { get; set; }

        public string DeleteTask { get; set; }

    }
}