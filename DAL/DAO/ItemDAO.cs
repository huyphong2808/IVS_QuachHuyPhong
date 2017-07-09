using Dapper;
using Models.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System;
using Components;

namespace DAL.DAO
{
    public class ItemDAO
    {
        public IDbConnection db;
        public ItemDAO()
        {
            db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }
        public List<ItemDisplayModel> GetAll()
        {
            string sql = "SELECT `id`, `code`, `name`, `specification`, `description`, `dangerous` FROM `product_item` WHERE TRUE";
            return db.Query<ItemDisplayModel>(sql).ToList();
        }
        public int Search(SearchItemModel searchCondition, out List<ItemDisplayModel> lstModel, out int total, int _page)
        {
            _page = _page * 15 - 15;
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstModel = new List<ItemDisplayModel>();
            total = new int();
            try
            {
                string sql = "SELECT item.`id`, item.`code`, item.`name`, cate.`name` AS category_name, item.`specification`, item.`description`, item.`dangerous` ";
                sql += "FROM `product_item` AS item ";
                sql += "JOIN (SELECT c.`id`, cpp.`name` AS category_parent_name, c.`name` ";
                sql += "FROM `product_category` AS c ";
                sql += "LEFT JOIN (SELECT cp.`name`, cp.`id` FROM `product_category` cp) cpp on cpp.`id` = c.`parent_id` ";
                sql += ") cate ON item.`category_id` = cate.`id` WHERE TRUE ";

                string _sql = "SELECT count(item.`id`) ";
                _sql += "FROM `product_item` AS item ";
                _sql += "JOIN (SELECT c.`id`, cpp.`name` AS category_parent_name, c.`name` ";
                _sql += "FROM `product_category` AS c ";
                _sql += "LEFT JOIN (SELECT cp.`name`, cp.`id` FROM `product_category` cp) cpp on cpp.`id` = c.`parent_id` ";
                _sql += ") cate ON item.`category_id` = cate.`id` WHERE TRUE ";

                if (!string.IsNullOrEmpty(searchCondition.category_id.ToString()))
                {
                    sql += "AND (item.`category_id` = @category_id OR item.`category_id` IN (SELECT cate.`id` FROM `product_category` cate WHERE cate.`parent_id` = @category_id)) ";
                    _sql += "AND (item.`category_id` = @category_id OR item.`category_id` IN (SELECT cate.`id` FROM `product_category` cate WHERE cate.`parent_id` = @category_id)) ";
                }
                if (!string.IsNullOrEmpty(searchCondition.code))
                {
                    sql += "AND item.`code` LIKE @code ";
                    _sql += "AND item.`code` LIKE @code ";
                }
                if (!string.IsNullOrEmpty(searchCondition.name))
                {
                    sql += "AND item.`name` LIKE @name ";
                    _sql += "AND item.`name` LIKE @name ";
                }
                sql += " ORDER BY item.`id`ASC LIMIT @page,15";
                lstModel = db.Query<ItemDisplayModel>(sql, new { category_id = searchCondition.category_id, code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%', page = _page }).ToList();
                total = db.ExecuteScalar<int>(_sql, new { category_id = searchCondition.category_id, code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%', page = _page });
            }
            catch (Exception ex)
            {
                return returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public int Insert(ItemModel Model, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(Model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string sql = "INSERT INTO `product_item`";
                sql += "(`category_id`, `code`, `name`, `specification`, `description`, `dangerous`, `discontinued_datetime`, ";
                sql += "`inventory_measure_id`, `inventory_expired`, `inventory_standard_cost`, `inventory_list_price`, `manufacture_day`, ";
                sql += "`manufacture_make`, `manufacture_tool`, `manufacture_finished_goods`, `manufacture_size`, `manufacture_size_measure_id`, ";
                sql += "`manufacture_weight`, `manufacture_weight_measure_id`, `manufacture_style`, `manufacture_class`, ";
                sql += "`manufacture_color`) VALUES (@category_id, @code, @name, @specification, @description, @dangerous, @discontinued_datetime, ";
                sql += "@inventory_measure_id, @inventory_expired, @inventory_standard_cost, @inventory_list_price, @manufacture_day, ";
                sql += "@manufacture_make, @manufacture_tool, @manufacture_finished_goods, @manufacture_size, @manufacture_size_measure_id, ";
                sql += "@manufacture_weight, @manufacture_weight_measure_id, @manufacture_style, @manufacture_class, @manufacture_color)";
                db.Execute(sql, new
                {
                    category_id = Model.category_id,
                    code = Model.code,
                    name = Model.name,
                    specification = Model.specification,
                    description = Model.description,
                    dangerous = Model.dangerous,
                    discontinued_datetime = Model.discontinued_datetime,
                    inventory_measure_id = Model.inventory_measure_id,
                    inventory_expired = Model.inventory_expired,
                    inventory_standard_cost = Model.inventory_standard_cost,
                    inventory_list_price = Model.inventory_list_price,
                    manufacture_day = Model.manufacture_day,
                    manufacture_make = Model.manufacture_make,
                    manufacture_tool = Model.manufacture_tool,
                    manufacture_finished_goods = Model.manufacture_finished_goods,
                    manufacture_size = Model.manufacture_size,
                    manufacture_size_measure_id = Model.manufacture_size_measure_id,
                    manufacture_weight = Model.manufacture_weight,
                    manufacture_weight_measure_id = Model.manufacture_weight_measure_id,
                    manufacture_style = Model.manufacture_style,
                    manufacture_class = Model.manufacture_class,
                    manufacture_color = Model.manufacture_color
                });
            }
            catch (Exception ex)
            {
                return (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }

        public int Update(ItemModel Model, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(Model, (int)Common.ActionType.Update, out lstMsg))
                {
                    returnCode = (int)Common.ReturnCode.UnSuccess;
                }
                string sql = "UPDATE `product_item` SET ";
                sql += "`category_id` = @category_id, `code` = @code, `name` = @name, `specification` = @specification, `description` = @description, `dangerous` = @dangerous, `discontinued_datetime` = @discontinued_datetime, ";
                sql += "`inventory_measure_id` = @inventory_measure_id, `inventory_expired` = @inventory_expired, `inventory_standard_cost` = inventory_standard_cost, `inventory_list_price` = inventory_list_price, ";
                sql += "`manufacture_make` = @manufacture_make, `manufacture_tool` = @manufacture_tool, `manufacture_finished_goods` = @manufacture_finished_goods, `manufacture_size` = @manufacture_size, `manufacture_size_measure_id` = @manufacture_size_measure_id, ";
                sql += "`manufacture_weight` = @manufacture_weight, `manufacture_weight_measure_id` = @manufacture_weight_measure_id, `manufacture_style` = @manufacture_style, `manufacture_class` = @manufacture_class, `manufacture_color` = @manufacture_color ";
                sql += "WHERE `id` = @id";
                db.Execute(sql, new
                {
                    category_id = Model.category_id,
                    code = Model.code,
                    name = Model.name,
                    specification = Model.specification,
                    description = Model.description,
                    dangerous = Model.dangerous,
                    discontinued_datetime = Model.discontinued_datetime,
                    inventory_measure_id = Model.inventory_measure_id,
                    inventory_expired = Model.inventory_expired,
                    inventory_standard_cost = Model.inventory_standard_cost,
                    inventory_list_price = Model.inventory_list_price,
                    manufacture_day = Model.manufacture_day,
                    manufacture_make = Model.manufacture_make,
                    manufacture_tool = Model.manufacture_tool,
                    manufacture_finished_goods = Model.manufacture_finished_goods,
                    manufacture_size = Model.manufacture_size,
                    manufacture_size_measure_id = Model.manufacture_size_measure_id,
                    manufacture_weight = Model.manufacture_weight,
                    manufacture_weight_measure_id = Model.manufacture_weight_measure_id,
                    manufacture_style = Model.manufacture_style,
                    manufacture_class = Model.manufacture_class,
                    manufacture_color = Model.manufacture_color,
                    id = Model.id
                });
            }
            catch (Exception ex)
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
                db.Open();
                using (var _transaction = db.BeginTransaction())
                {
                    for (int i = 0; i < lstID.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstID[i].ToString()))
                        {
                            string sql = "DELETE FROM `product_item` WHERE `id` = @id";
                            db.Execute(sql, new { id = lstID[i] });
                        }
                        else
                        {
                            lstMsg.Add("Item has ID " + lstID[i].ToString() + " has been delete ");
                        }
                    }
                    _transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }

        public int GetDetail(int id, out ItemModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new ItemModel();
            try
            {
                if (id != 0)
                {
                    string sql = "SELECT i.`id`, i.`category_id`, c.`name` AS `category_name`, i.`code`, i.`name`, i.`specification`, i.`description`, i.`dangerous`, i.`discontinued_datetime`, ";
                    sql += "i.`inventory_measure_id`, m1.`name` AS `inventory_measure_name`, i.`inventory_expired`, i.`inventory_standard_cost`, i.`inventory_list_price`, i.`manufacture_day`, ";
                    sql += "i.`manufacture_make`, i.`manufacture_tool`, i.`manufacture_finished_goods`, i.`manufacture_size`, i.`manufacture_size_measure_id`, m2.`name` AS `manufacture_size_measure_name`, ";
                    sql += "i.`manufacture_weight`, i.`manufacture_weight_measure_id`, m3.`name` AS `manufacture_weight_measure_name`, i.`manufacture_style`, i.`manufacture_class`, ";
                    sql += "`manufacture_color` FROM `product_item` AS i ";
                    sql += "LEFT OUTER JOIN `product_measure` AS m1 ON ( i.`inventory_measure_id` = m1.`id`) ";
                    sql += "LEFT OUTER JOIN `product_measure` AS m2 ON ( i.`manufacture_size_measure_id` = m2.`id`) ";
                    sql += "LEFT OUTER JOIN `product_measure` AS m3 ON ( i.`manufacture_weight_measure_id` = m3.`id`) ";
                    sql += "LEFT OUTER JOIN `product_category` AS c ON (i.`category_id` = c.`id`) ";
                    sql += "WHERE i.`id` = @id ";
                    Model = db.Query<ItemModel>(sql, new { id = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                return (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public bool isError(ItemModel Model, int action, out List<string> lstMessage)
        {
            bool isError = false;
            lstMessage = new List<string>();
            if (Model.code.Contains(" "))
            {
                isError = true;
                lstMessage.Add("[Code] must not contains space character!");
            }
            if ((int)Common.ActionType.Add == action)
            {
                var result = db.Query<ItemModel>("SELECT * FROM `product_item` WHERE `code` = @code", new { code = Model.code.Trim() }).ToList();
                if (result.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == action)
            {
                var result = db.Query<ItemModel>("SELECT * FROM `product_item` WHERE `code` = @code AND `id` <> @id", new { code = Model.code.Trim(), id = Model.id }).ToList();
                if (result.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }
        public int GetCategory(bool hasEmpty, out List<GetCatetoryModel> lstCombobox)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstCombobox = new List<GetCatetoryModel>();
            try
            {

                string sql = "SELECT `id`, `name` FROM `product_category`";
                lstCombobox = db.Query<GetCatetoryModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public int GetMeasure(bool hasEmpty, out List<GetMeasureModel> lstCombobox)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstCombobox = new List<GetMeasureModel>();
            try
            {

                string sql = "SELECT `id`, `name` FROM `product_measure`";
                lstCombobox = db.Query<GetMeasureModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
    }
}
