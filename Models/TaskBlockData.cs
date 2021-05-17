using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.Models
{
    public class TaskBlockData
    {
        public string TaskName { get; set; }

        public int TaskDuration { get; set; }

        public int TaskID { get; set; }

        public string GoalColour { get; set; }

        public string TaskGoalID { get; set; }

        public string TaskNotes { get; set; }

        public float TaskStatus { get; set; }

        public List<HistoryData> TaskHistory { get; set; }

        public string TaskHistoryString { get; set; }
    }
}