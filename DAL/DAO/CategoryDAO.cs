using Dapper;
using Models.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Components;
using System;
using PagedList;

namespace DAL.DAO
{
    public class CategoryDAO
    {
        public IDbConnection _db;
        public CategoryDAO()
        {
            _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }
        //Insert data 
        public int Insert(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                var strQuery = "INSERT INTO `product_category` (`parent_id`, `code`, `name`, `description`) VALUES(@parent_id, @code, @name, @description); ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }
            return result;
        }

        public int Update(CategoryModel model, out List<string> lstMsg)
        {
            int result = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();

            try
            {
                if (isError(model, (int)Common.ActionType.Update, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string strQuery = "UPDATE `product_category` SET `parent_id` = @parent_id, `code` = @code, `name` = @name, `description` = @description, `updated_datetime` = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF") + "' WHERE `id` = @id ";
                _db.Execute(strQuery, model);
            }
            catch (Exception ex)
            {
                lstMsg.Add("Exception Occurred.");
                result = (int)Common.ReturnCode.UnSuccess;
            }

            return result;
        }
        public int GetByID(long ID, out CategoryModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT *";
                    strQuery += " FROM product_category";
                    strQuery += " WHERE `id` = @id";
                    Model = _db.Query<CategoryModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int GetDetail(long ID, out CategoryViewModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new CategoryViewModel();
            try
            {
                if (ID != 0)
                {
                    string strQuery = "SELECT cate.`id`, cate_parent.`name` parent_name, cate.`code`, cate.`name`, cate.`description`";
                    strQuery += " FROM product_category cate LEFT JOIN(SELECT `id`, `name` FROM `product_category`) cate_parent";
                    strQuery += " ON cate.`parent_id` = cate_parent.id WHERE cate.`id` = @id";
                    Model = _db.Query<CategoryViewModel>(strQuery, new { id = ID }).SingleOrDefault();
                }
            }
            catch (Exception)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }

            return returnCode;
        }

        public int Delete(List<int> lstID, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                _db.Open();
                using (var _transacsion = _db.BeginTransaction())
                {
                    for (int i = 0; i < lstID.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstID[i].ToString()))
                        {
                            //Check Category children
                            var result_children = _db.Query<CategoryModel>("SELECT `id` FROM `product_category` WHERE `parent_id` = @id", new { id = lstID[i] }).ToList();
                            //Check Item
                            var result_item = _db.Query<ItemModel>("SELECT `id` FROM `product_item` WHERE `category_id` = @id", new { id = lstID[i] }).ToList();
                            if(result_children.Count != 0)
                            {
                                foreach(var item in result_children)
                                {
                                    _db.Execute("UPDATE `product_category` SET `parent_id` = 0 WHERE `id` = @id", new { id = item.id });
                                }
                            }
                            if (result_item.Count != 0)
                            {
                                foreach (var item in result_children)
                                {
                                    _db.Execute("UPDATE `product_item` SET `category_id` = 0 WHERE `id` = @id", new { id = item.id });
                                }
                            }
                            _db.Execute("DELETE FROM `product_category` WHERE `id` = @id", new { id = lstID[i] });
                        }
                        else
                        {
                            lstMsg.Add("Category has ID " + lstID[i].ToString() + " has been delete ");
                        }
                    }
                    _transacsion.Commit();
                }
            }
            catch(Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }

        private bool isError(CategoryModel Model, int ActionType, out List<string> lstMessage)
        {
            bool isError = false;
            lstMessage = new List<string>();
            if (Model.code.Contains(" "))
            {
                isError = true;
                lstMessage.Add("[Code] must not contains space character!");
            }
            if ((int)Common.ActionType.Add == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code LIMIT 1 ";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == ActionType)
            {
                string strQuery = "SELECT `id` FROM `product_category` WHERE `code` = @code AND `id` <> @id LIMIT 1";
                var hasItem = _db.Query<CategoryParent>(strQuery, new { code = Model.code, id = Model.id }).ToList();
                if (hasItem.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }

        public int Search(SearchCategoryModel searchCondition, out List<CategoryViewModel> lstModel, out int total, int _page)
        {
            _page = _page * 15 - 15;
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstModel = new List<CategoryViewModel>();
            total = new int();
            try
            {
                string sql = "SELECT cate.`id`, cate_parent.`name` parent_name, cate.`code`, cate.`name`, cate.`description` ";
                sql += "FROM `product_category` cate  ";
                sql += "LEFT JOIN (SELECT `id`, `name` FROM `product_category`) cate_parent ";
                sql += "ON cate.`parent_id` = cate_parent.`id` WHERE TRUE ";

                string _sql = "SELECT COUNT(cate.`id`) ";
                _sql += "FROM `product_category` cate  ";
                _sql += "LEFT JOIN (SELECT `id`, `name` FROM `product_category`) cate_parent ";
                _sql += "ON cate.`parent_id` = cate_parent.`id` WHERE TRUE ";
                
                if(!string.IsNullOrEmpty(searchCondition.parent_id.ToString()))
                {
                    sql += "AND (cate.`id` = @parent_id OR cate.`id` IN (SELECT `id` FROM tuankhai_freshernet.product_category WHERE `parent_id` = @parent_id)) ";
                    _sql += "AND (cate.`id` = @parent_id OR cate.`id` IN (SELECT `id` FROM tuankhai_freshernet.product_category WHERE `parent_id` = @parent_id)) ";
                }
                if(!string.IsNullOrEmpty(searchCondition.code))
                {
                    sql += "AND cate.`code` LIKE @code ";
                    _sql += "AND cate.`code` LIKE @code ";
                }
                if(!string.IsNullOrEmpty(searchCondition.name))
                {
                    sql += "AND cate.`name` LIKE @name ";
                    _sql += "AND cate.`name` LIKE @name ";
                }
                sql += " ORDER BY cate.`id` ASC LIMIT @page,15";
                lstModel = _db.Query<CategoryViewModel>(sql, new { parent_id = searchCondition.parent_id, code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%', page = _page }).ToList();
                total = _db.ExecuteScalar<int>(_sql, new { parent_id = searchCondition.parent_id, code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%', page = _page });
            }
            catch(Exception ex)
            {
                return returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public List<ComboboxParent> GetParent()
        {
            List<ComboboxParent> model = new List<ComboboxParent>();
            try
            {
                string strQuery = "SELECT DISTINCT cate_parent.id AS id, cate_parent.name AS name";
                strQuery += "  FROM product_category cate INNER JOIN (SELECT id, name FROM product_category) cate_parent";
                strQuery += "  ON cate.parent_id = cate_parent.id";
                model = _db.Query<ComboboxParent>(strQuery).ToList();


                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstCombobox = new List<GetCatetoryModel>();
            try
            {

                string sql = "SELECT `id`, `name` FROM `product_category`";
                lstCombobox = _db.Query<GetCatetoryModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
    }
}
