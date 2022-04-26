using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public interface ICategoryRepository
    {
        public List<Category> GetAllCategories();

        public Category GetCategoryById(int id);
    }
}
