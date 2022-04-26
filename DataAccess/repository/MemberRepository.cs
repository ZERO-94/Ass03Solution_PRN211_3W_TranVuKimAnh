using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public class MemberRepository : IMemberRepository
    {

        private MemberDAO daoInstance;

        public MemberRepository()
        {
            daoInstance = new MemberDAO();
        }

        public bool ChangePassword(int id, string oldPassword, string newPassword) => daoInstance.ChangePassword(id, oldPassword, newPassword);

        public Member CheckLogin(string email, string password) => daoInstance.CheckLogin(email, password);

        public bool CreateMember(Member member) => daoInstance.CreateMember(member);

        public bool DeleteMember(int id) => daoInstance.DeleteMember(id);

        public List<Member> GetAllMembers() => daoInstance.GetAllMembers();

        public Member GetMemberById(int id) => daoInstance.GetMemberById(id);

        public Member GetMemberByEmail(string email) => daoInstance.GetMemberByEmail(email);

        public bool UpdateMember(int id, Member member) => daoInstance.UpdateMember(id, member);
    }
}
