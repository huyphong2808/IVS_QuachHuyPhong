using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Model;
using DAL.DAO;

namespace BLL
{
    public interface IMeasure
    {
        List<MeasureModel> GetAll();
        int Search(MeasureModel searchCondition, out List<MeasureModel> lstModel, out int total, int _page);
        int Insert(MeasureModel Model, out List<string> lstMsg);
        int Update(MeasureModel Model, out List<string> lstMsg);
        int Delete(List<int> lstID, out List<string> lstMsg);
        int GetDetail(int id, out MeasureModel Model);
    }
    public class MeasureBLL : IMeasure
    {
        public MeasureDAO _measureDAO;
        public MeasureBLL()
        {
            _measureDAO = new MeasureDAO();
        }
        public List<MeasureModel> GetAll()
        {
            return _measureDAO.GetAll();
        }
        public int Search(MeasureModel searchCondition, out List<MeasureModel> lstModel, out int total, int _page)
        {
            return _measureDAO.Search(searchCondition, out lstModel, out total, _page);
        }
        public int Insert(MeasureModel Model, out List<string> lstMsg)
        {
            return _measureDAO.Insert(Model, out lstMsg);
        }
        public int Update(MeasureModel Model, out List<string> lstMsg)
        {
            return _measureDAO.Update(Model, out lstMsg);
        }
        public int Delete(List<int> lstID, out List<string> lstMsg)
        {
            return _measureDAO.Delete(lstID, out lstMsg);
        }
        public int GetDetail(int id, out MeasureModel Model)
        {
            return _measureDAO.GetDetail(id, out Model);
        }
    }
}
