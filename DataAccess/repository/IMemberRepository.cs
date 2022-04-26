using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public interface IMemberRepository
    {
        public Member GetMemberById(int id);

        public List<Member> GetAllMembers();

        public bool CreateMember(Member member);

        public bool UpdateMember(int id, Member member);

        public bool DeleteMember(int id);

        public bool ChangePassword(int id, string oldPassword, string newPassword);

        public Member CheckLogin(string email, string password);

        public Member GetMemberByEmail(string email);
    }
}
