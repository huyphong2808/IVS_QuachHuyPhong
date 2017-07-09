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
    public interface IItem
    {
        List<ItemDisplayModel> GetAll();
        int Search(SearchItemModel searchCondition, out List<ItemDisplayModel> lstModel, out int total, int _page);
        int Insert(ItemModel Model, out List<string> lstMsg);
        int Update(ItemModel Model, out List<string> lstMsg);
        int Delete(List<int> lstID, out List<string> lstMsg);
        int GetDetail(int id, out ItemModel Model);
        int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox);
        int GetMeasure(bool hasEmpty, out List<GetMeasureModel> lstCombobox);
    }
    public class ItemBLL : IItem
    {
        public ItemDAO _itemDAO;
        public ItemBLL()
        {
            _itemDAO = new ItemDAO();
        }
        public List<ItemDisplayModel> GetAll()
        {
            return _itemDAO.GetAll();
        }
        public int Search(SearchItemModel searchCondition, out List<ItemDisplayModel> lstModel, out int total, int _page)
        {
            return _itemDAO.Search(searchCondition, out lstModel, out total, _page);
        }
        public int Insert(ItemModel Model, out List<string> lstMsg)
        {
            return _itemDAO.Insert(Model, out lstMsg);
        }
        public int Update(ItemModel Model, out List<string> lstMsg)
        {
            return _itemDAO.Update(Model, out lstMsg);
        }
        public int Delete(List<int> lstID, out List<string> lstMsg)
        {
            return _itemDAO.Delete(lstID, out lstMsg);
        }
        public int GetDetail(int id, out ItemModel Model)
        {
            return _itemDAO.GetDetail(id, out Model);
        }
        public int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox)
        {
            return _itemDAO.GetCategory(hasEmpty, out lstCombobox);
        }
        public int GetMeasure(bool hasEmpty, out List<GetMeasureModel> lstCombobox)
        {
            return _itemDAO.GetMeasure(hasEmpty, out lstCombobox);
        }
    }
}
