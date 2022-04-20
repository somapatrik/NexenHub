using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using NexenHub.Class;
using Newtonsoft.Json;

namespace NexenHub.Models
{
    public class MemberSearchList
    {
        public class MemberSearchListItem
        {            
            public string ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }

        public string jsonMembers 
        {
            // get => JsonConvert.SerializeObject(MemberList.Select(x => @"{ 'ID':'" + x.ID + "','Name':'" + x.Name + "','Phone':'" + x.Phone + "'}"));
             get => JsonConvert.SerializeObject(MemberList);
        }

        public List<MemberSearchListItem> MemberList = new List<MemberSearchListItem>();

        private GlobalDatabase dbglob = new GlobalDatabase();

        public MemberSearchList()
        {  
            LoadFromDb();
        }

        private void LoadFromDb()
        {
            foreach (DataRow row in dbglob.GetMemberSearch().Rows)
            {
                MemberList.Add(new MemberSearchListItem()
                {
                    ID = row["MEMBER_ID"].ToString(),
                    Name = row["MEMBER_NAME"].ToString(),
                    Phone = row["PHONE"].ToString(),
                    Email = row["EMAIL"].ToString()
                }) ;
            }
        }

    }
}
