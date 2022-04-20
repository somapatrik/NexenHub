using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NexenHub.Models;

namespace NexenHub.Pages
{
    public class MemberModel : PageModel
    {
        public Member member { get; set; }

        public bool isMemberValid { get; set; }

        public void OnGet(string argMember)
        {
            member = new Member();
            isMemberValid = false;

            if (!string.IsNullOrEmpty(argMember))
            {
                isMemberValid = true;
                member.ID = argMember;
            }
        }
    }
}
