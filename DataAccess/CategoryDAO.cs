using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CategoryDAO
    {
        private static CategoryDAO instance;
        private static readonly object instanceLock = new object();
        private AssignmentPRN211DBContext context = new AssignmentPRN211DBContext();

        //Use singleton design pattern
        public CategoryDAO()
        {

        }

        public List<Category> GetAllCategories()
        {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Categories.ToList();
            }
        }

        public Category GetCategoryById(int id) {
            using (context = new AssignmentPRN211DBContext())
            {
                return context.Categories.SingleOrDefault(c => c.CategoryId == id);
            }
        }
    }
}
