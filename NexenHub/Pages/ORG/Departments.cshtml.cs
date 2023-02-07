using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections.Generic;
using System.Data;

namespace NexenHub.Pages.ORG
{
    public class DepartmentsModel : PageModel
    {

        public List<Department> Departments;

        GlobalDatabase dbglob = new GlobalDatabase();

        public void OnGet()
        {
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            Departments = new List<Department>();

            DataTable dt = dbglob.GetDepartments("50007344");
            foreach (DataRow row in dt.Rows)
            {
                Departments.Add(new Department()
                {
                    ID = row["DEPT_ID"].ToString(),
                    Name = row["DEPT_NAME"].ToString(),
                    UpDepartment_ID = row["UP_DEPT_ID"].ToString(),
                    Level = int.Parse(row["LEVEL"].ToString())
                });
                
            }
        }
    }
}
