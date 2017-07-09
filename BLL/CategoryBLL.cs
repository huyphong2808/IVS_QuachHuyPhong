using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Model;
using DAL.DAO;
using Components;

namespace BLL
{
    public interface ICategoryBLL
    {
        int GetByID(long ID, out CategoryModel model);
        int GetDetail(long ID, out CategoryViewModel model);
        int Insert(CategoryModel model, out List<string> lstMsg);
        int Update(CategoryModel model, out List<string> lstMsg);
        int Delete(List<int> lstID, out List<string> lstMsg);
        int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox);
        int Search(SearchCategoryModel searchCondition, out List<CategoryViewModel> lstModel, out int total, int _page);
    }
    public class CategoryBLL : ICategoryBLL
    {
        public CategoryDAO _categoryDAO;
        public CategoryBLL()
        {
            _categoryDAO = new CategoryDAO();
        }

        public int Delete(List<int> lstID, out List<string> lstMsg)
        {
            return _categoryDAO.Delete(lstID, out lstMsg);
        }

        public int GetByID(long ID, out CategoryModel model)
        {
            return _categoryDAO.GetByID(ID, out model);
        }
        public int GetDetail(long ID, out CategoryViewModel model)
        {
            return _categoryDAO.GetDetail(ID, out model);
        }

        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Insert(model, out lstMsg);
        }

        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            return _categoryDAO.Update(model, out lstMsg);
        }
        public int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox)
        {
            return _categoryDAO.GetCategory(hasEmpty, out lstCombobox);
        }
        public int Search(SearchCategoryModel searchCondition, out List<CategoryViewModel> lstModel, out int total, int _page)
        {
            return _categoryDAO.Search(searchCondition, out lstModel, out total, _page);
        }
    }
}
