using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Class;
using NexenHub.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NexenHub.Pages.ORG
{
    [Authorize(Roles = "User")]
    public class DepartmentsModel : PageModel
    {
        GlobalDatabase dbglob = new GlobalDatabase();


        private string TopDepartmentID = "50007344";
        public List<Department> allDepts { get; set; }
        public StringBuilder generatedCode { set; get; }

        public List<Member> members { get; set; }

        public void OnGet()
        {
            LoadDepartments();
            CreateSubDepartments();

            generatedCode = new StringBuilder();
            allDepts.ForEach(d=>GenerateHtml(d));
        }

        private void GenerateHtml(Department department)
        {
            generatedCode.AppendLine("<ul class=\"my-1\" style=\"list-style:none;\">");
            generatedCode.AppendLine("<li>");

            if (department.SubDepartments.Count() > 0)
            {
                generatedCode.AppendLine($"<a onclick=\"selectByDepartment('{department.ID}')\" class=\"upDepartment text-muted\" data-bs-toggle=\"collapse\" href=\"#collapse_{department.ID}\" role=\"button\" aria-expanded=\"false\" aria-controls=\"collapse_{department.ID}\">{department.Name}</a>");
                generatedCode.AppendLine($"<div class=\"collapse\" id=\"collapse_{department.ID}\">");
                department.SubDepartments.ForEach(sub => GenerateHtml(sub));
                generatedCode.AppendLine("</div>");
            }
            else
            {
                generatedCode.AppendLine($"<a onclick=\"selectByDepartment('{department.ID}')\" class=\"endDepartment text-muted\">{department.Name}</a>");
            }

            generatedCode.AppendLine("</li>");
            generatedCode.AppendLine("</ul>");
        }

        

        private void CreateSubDepartments()
        {
            // First remove top department from which all are created to avoid only one top which is EU Plant anyway
            allDepts.Remove(allDepts.First(d=>d.ID == TopDepartmentID));

            // List of top level departmens which will be moved deeper inside list
            // After that they will not be needed on top
            List<string> toRemove = new List<string>();

            foreach (Department dept in allDepts)
            {
                // Get up department
                string UpDepartment = dept.UpDepartment_ID;
                // Find updepartment and move it deeper to the list
                Department upFind = allDepts.FirstOrDefault(u => u.ID == UpDepartment);
                if (upFind != null)
                {
                    // Put sub department into up department object
                    upFind.SubDepartments.Add(dept);
                    // Sub department can be removed from top level in the main list
                    toRemove.Add(dept.ID);
                }
            }

            // Remove top level items
            allDepts.RemoveAll(x => toRemove.Contains(x.ID));
        }

        private void LoadDepartments()
        {
            // Complete list
            allDepts = new List<Department>();
            DataTable dt = dbglob.GetDepartments(TopDepartmentID);
            foreach (DataRow row in dt.Rows)
            {
                allDepts.Add(new Department()
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
