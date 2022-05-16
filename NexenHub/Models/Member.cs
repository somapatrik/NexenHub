using NexenHub.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NexenHub.Models
{
    public class Member
    {
        private string _ID;
        public string ID { 
            get { return _ID; } 
            set 
            { 
                _ID = value;
                LoadFromDb();
            } 
        }
        public string Name { get; set; }
        public string Position { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string HeadDepartmentID { get; set; }
        public string HeadDepartmentName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Special { get; set; }

        public bool IsPresent { get; set; }

        private GlobalDatabase dbglob = new GlobalDatabase();

        public Member() { }
        public Member(string MEMBER_ID)
        {
            _ID = MEMBER_ID;
            LoadFromDb();
        }

        private void LoadFromDb()
        {
            if (!string.IsNullOrEmpty(_ID))
            {
                DataTable dt = dbglob.GetMember(_ID);
                if (dt.Rows.Count > 0)
                {
                    Name = dt.Rows[0]["MEMBER_NAME"].ToString();
                    Position = dt.Rows[0]["POSITION"].ToString();
                    DepartmentID = dt.Rows[0]["DEPT_ID"].ToString();
                    DepartmentName = dt.Rows[0]["DEPT_NAME"].ToString();
                    HeadDepartmentID = dt.Rows[0]["HEAD_DEPT_ID"].ToString();
                    HeadDepartmentName = dt.Rows[0]["HEAD_DEPT_NAME"].ToString();
                    Phone = dt.Rows[0]["PHONE"].ToString();
                    Email = dt.Rows[0]["EMAIL"].ToString();

                    // For the best
                    Special = _ID == "40175001" ? true : false;

                    // Aktion
                    IsPresent = dbglob.IsMemberPresent(_ID)?.Rows[0]["STATUS"].ToString() == "1" ? true : false;
                }
            }
        }
    }
}
