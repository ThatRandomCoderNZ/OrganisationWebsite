using OrganisationWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrganisationWebsite.DatabaseRepository;
using System.Diagnostics;

namespace OrganisationWebsite.Controllers
{
    public class HomeController : Controller
    {
        DBConnection dbConnection = new DBConnection();

        bool UserIDExists() {
            foreach (string key in Request.Cookies.AllKeys) {
                Debug.WriteLine(key);
                if (key == "UserID")
                {
                    Debug.WriteLine("Value of Cookie: " + Request.Cookies["UserID"].Value);
                    return true;
                }
            }
            Debug.WriteLine("Returning False");
            return false;
        }

        public ActionResult Index()
        {
            string userID = "";
            if (UserIDExists() && Request.Cookies["UserID"].Value != "")
            {
                userID = Request.Cookies["UserID"].Value;
            }
            else {
                Debug.WriteLine("Returning to logon");
                return RedirectToAction("LoginScreen", "Login");
                //Return to logon screen.
            }

            string main_information_query = @"SELECT T.task_id, T.name, T.duration, T.goal_id, T.notes, T.status, G.colour
                                  FROM student93061.tasks T
                                  LEFT JOIN student93061.goals G ON T.goal_id = G.goal_id
                                  WHERE T.user_id = '" + userID + @"' AND T.status < 100;";
            string[] column_names = new string[] {"task_id", "name", "duration", "notes", "status", "colour", "goal_id"};
            Dictionary<string, List<string>> task_data = dbConnection.RetrieveFromDatabase(column_names, main_information_query);
            Dictionary<string, TaskBlockData> taskBlocks = new Dictionary<string, TaskBlockData>();

            //Create all of the TASKBLOCK DATA
            for (int i = 0; i < task_data["task_id"].Count; i++)
            {
                TaskBlockData newBlock = new TaskBlockData();
                newBlock.TaskID = int.Parse(task_data["task_id"][i]);
                newBlock.TaskName = task_data["name"][i];
                newBlock.TaskStatus = float.Parse(task_data["status"][i]);
                newBlock.TaskNotes = task_data["notes"][i];
                newBlock.TaskDuration = int.Parse(task_data["duration"][i]);
                newBlock.GoalColour = task_data["colour"][i];
                Debug.WriteLine(newBlock.TaskGoalID = task_data["goal_id"][i]);
                newBlock.TaskGoalID = task_data["goal_id"][i];
                //Retrieve all history notations related to this specific task.
                string[] historyColumnNames = new string[] {"information", "date"};
                string historyQuery = @"SELECT date, information 
                                         FROM student93061.history
                                         WHERE task_id = '" + task_data["task_id"][i]  + "'; ";
                Dictionary<string, List<string>> historyData = dbConnection.RetrieveFromDatabase(historyColumnNames, historyQuery);
                List<HistoryData> allHistoryData = new List<HistoryData>();
                string historyString = "";
                for (int j = 0; j < historyData["information"].Count; j++)
                {
                    HistoryData newHistoryData = new HistoryData
                    {
                        Information = historyData["information"][j],
                        Date = DateTime.Parse(historyData["date"][j])
                    };
                    allHistoryData.Add(newHistoryData);
                    historyString += String.Format("{0}. {1} - {2}" + System.Environment.NewLine, (j + 1), historyData["date"][j], historyData["information"][j]);                                                   
                }
                newBlock.TaskHistoryString = historyString;
                newBlock.TaskHistory = allHistoryData;
                taskBlocks.Add(task_data["task_id"][i], newBlock);
            }
            //Get Information from the Database.

            //Process returned data and construct a List<TaskBlockData>

            //Repeat similar process with TaskGoalData
            string[] goal_column_names = new string[] { "goal_id", "name", "colour"};
            string goal_query = @"SELECT goal_id, name, colour FROM student93061.goals;";
            Dictionary<string, List<string>> goalData = dbConnection.RetrieveFromDatabase(goal_column_names, goal_query);
            Dictionary<string, TaskGoalData> allGoalData = new Dictionary<string, TaskGoalData>();
            for(int i = 0; i < goalData["goal_id"].Count; i++)
            {
                TaskGoalData newGoalData = new TaskGoalData { GoalID = goalData["goal_id"][i],
                                                              GoalName = goalData["name"][i],
                                                              GoalColour = goalData["colour"][i]};
                allGoalData.Add(goalData["goal_id"][i], newGoalData);
            }
            //Create Model and submit to view.
            DetailsFormData formData = new DetailsFormData { TaskDuration=" ", TaskNotes=" ", HistoryDetails = " ", TaskName = " "};
            TaskViewModel taskViewModel = new TaskViewModel { TaskBlocks = taskBlocks, FormDetails = formData, GoalBlocks = allGoalData };
            return View(taskViewModel);
        }

        [HttpPost]
        public ActionResult Index(TaskViewModel taskModel) {
            //Control has returned here because a form has been updated.
            if (ModelState.IsValid)
            {
                if(taskModel.FormDetails.DeleteTask == "true")
                {
                    string delete_task = @"DELETE FROM student93061.tasks WHERE tasks.task_id = '" + taskModel.FormDetails.TaskID + "';";
                    dbConnection.InsertIntoDatabase(delete_task);

                    return RedirectToAction("Index");
                }
				//Here taskModel.FormDetails should be properly bound and ready to unpack and utilise.
				//Information includes: {name, notes, history_details, and new task duration}

				//First make new History ROW
				if (taskModel.FormDetails.HistoryDetails != null && taskModel.FormDetails.HistoryDetails != "") {
					string statement = @"INSERT INTO `student93061`.`history` (`information`, `task_id`) 
								     VALUES ('" + taskModel.FormDetails.HistoryDetails + "', '" + taskModel.FormDetails.TaskID + "');";
					dbConnection.InsertIntoDatabase(statement);
				}
                //Second update the existing task with the new information. Validation should also take place here.
                Debug.WriteLine(taskModel.FormDetails.TaskID);
                Debug.WriteLine(taskModel.FormDetails.TaskName);
                Debug.WriteLine(taskModel.FormDetails.TaskNotes);
                Debug.WriteLine(taskModel.FormDetails.TaskDuration);


                string update_task = @"UPDATE `student93061`.`tasks` 
									   SET `name`=@task_name, `notes`=@task_notes, `status` =@task_status 
									   WHERE `task_id`=@task_id;";
                List<Tuple<string, string>> update_parameters = new List<Tuple<string, string>>();
                update_parameters.Add(new Tuple<string, string>("@task_name", taskModel.FormDetails.TaskName));
                update_parameters.Add(new Tuple<string, string>("@task_notes", taskModel.FormDetails.TaskNotes));
                update_parameters.Add(new Tuple<string, string>("@task_status", taskModel.FormDetails.TaskDuration));
                update_parameters.Add(new Tuple<string, string>("@task_id", taskModel.FormDetails.TaskID));
                dbConnection.InsertIntoDatabase(update_task, update_parameters);
            }
            //TODO: Yet to add default history logs in the event that the user doesn't enter any history notes.
            //ALSOTODO: Remember to add Time-related tasks and both time specific and non-specific views.
            //All Server update/inserting should be handled here, before control is passed back to the loading controller action
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Check to see if a variable meant for a query is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string checkQueryValueForNull(string value)
        {
            if (value == null || value == "")
            {
                return "null";
            }
            else
            {
                return  "'" + value + "'";
            }
        }

        public ActionResult IndexAddTask(TaskViewModel viewModel)
        {
            string userID = "";
            if (UserIDExists() && Request.Cookies["UserID"].Value != "")
            {
                userID = Request.Cookies["UserID"].Value;
            }
            else {
                Debug.WriteLine("Returning to logon");
                return RedirectToAction("LoginScreen", "Login");
                //Return to logon screen.
            }

            Debug.WriteLine(viewModel.NewTaskFormDetails.Name);
            Debug.WriteLine(viewModel.NewTaskFormDetails.Notes);
            Debug.WriteLine(viewModel.NewTaskFormDetails.Duration);
            Debug.WriteLine(viewModel.NewTaskFormDetails.GoalID);

            viewModel.NewTaskFormDetails.StartTime = checkQueryValueForNull(viewModel.NewTaskFormDetails.StartTime);
            viewModel.NewTaskFormDetails.EndTime = checkQueryValueForNull(viewModel.NewTaskFormDetails.EndTime);
            viewModel.NewTaskFormDetails.GoalID = checkQueryValueForNull(viewModel.NewTaskFormDetails.GoalID);

            string makeNewTask = String.Format(@"INSERT INTO `student93061`.`tasks` (`name`, `notes`, `status`, `duration`, `goal_id`, `user_id`) 
                                   VALUES ('{0}', '{1}', '0','{2}', {3}, '{4}');", 
                                   viewModel.NewTaskFormDetails.Name,
                                   viewModel.NewTaskFormDetails.Notes,
                                   viewModel.NewTaskFormDetails.Duration,
                                   viewModel.NewTaskFormDetails.GoalID,
                                   userID);
            //In the above query, non-nullable values are already in quotation marks. if a value is null it should equal string null, with no quotation marks.
            Debug.WriteLine("Try to make new task");
            Debug.WriteLine(makeNewTask);

            dbConnection.InsertIntoDatabase(makeNewTask);
            return RedirectToAction("Index");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            Dictionary<string, int> testDictionary = new Dictionary<string, int>();


            return View();
        }


    }
}