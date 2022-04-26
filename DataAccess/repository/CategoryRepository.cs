using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.repository
{
    public class CategoryRepository : ICategoryRepository
    {

        private CategoryDAO daoInstance = new CategoryDAO();

        public List<Category> GetAllCategories() => daoInstance.GetAllCategories();

        public Category GetCategoryById(int id) => daoInstance.GetCategoryById(id); 
    }
}
