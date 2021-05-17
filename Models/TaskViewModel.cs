using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrganisationWebsite.Models
{
    public class TaskViewModel
    {
        public Dictionary<string, TaskBlockData> TaskBlocks { get; set; }

        public Dictionary<string, TaskGoalData> GoalBlocks { get; set; }

        public DetailsFormData FormDetails { get; set; }

        public NewTaskDetails NewTaskFormDetails { get; set; }

    }
}