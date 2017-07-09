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
    public class MeasureDAO
    {
        public IDbConnection db;
        public MeasureDAO()
        {
            db = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString);
        }
        public List<MeasureModel> GetAll()
        {
            string sql = "SELECT `id`, `code`, `name`, `description` FROM `product_measure` WHERE TRUE";
            return db.Query<MeasureModel>(sql).ToList();
        }
        public int Search(MeasureModel searchCondition, out List<MeasureModel> lstModel, out int total, int _page)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstModel = new List<MeasureModel>();
            _page = _page * 15 - 15;
            total = new int();
            try
            {
                string sql = "SELECT `id`, `code`, `name`, `description` FROM `product_measure` WHERE TRUE ";
                string _sql = "SELECT COUNT(`id`) FROM product_measure AS i WHERE TRUE ";
                if (!string.IsNullOrEmpty(searchCondition.code))
                {
                    sql += "AND `code` LIKE @code ";
                    _sql += "AND `code` LIKE @code ";
                }
                if (!string.IsNullOrEmpty(searchCondition.name))
                {
                    sql += "AND `name` LIKE @name ";
                    _sql += "AND `name` LIKE @name ";
                }
                sql += " LIMIT @page,15 ";
                lstModel = db.Query<MeasureModel>(sql, new { code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%', page = _page }).ToList();
                total = db.ExecuteScalar<int>(_sql, new { code = '%' + searchCondition.code + '%', name = '%' + searchCondition.name + '%' });
                return returnCode;
            }
            catch (Exception ex)
            {
                return returnCode = (int)Common.ReturnCode.UnSuccess;
            }
        }

        public int Insert(MeasureModel Model, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(Model, (int)Common.ActionType.Add, out lstMsg))
                {
                    return (int)Common.ReturnCode.UnSuccess;
                }
                string sql = "INSERT INTO `product_measure` (`code`, `name`, `description`) VALUES (@code, @name, @description)";
                db.Execute(sql, new { code = Model.code.Trim(), name = Model.name.Trim(), description = Model.description.Trim() });
            }
            catch (Exception ex)
            {
                return (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }

        public int Update(MeasureModel Model, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                if (isError(Model, (int)Common.ActionType.Update, out lstMsg))
                {
                    returnCode = (int)Common.ReturnCode.UnSuccess;
                }
                string sql = "UPDATE `product_measure` SET `code` = @code, `name` = @name, `description` = @description WHERE `id` = @id";
                db.Execute(sql, new { code = Model.code, name = Model.name, description = Model.description, id = Model.id });
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public bool isError(MeasureModel Model, int action, out List<string> lstMessage)
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
                var result = db.Query<MeasureModel>("SELECT * FROM `product_measure` WHERE `code` = @code", new { code = Model.code.Trim() }).ToList();
                if (result.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            if ((int)Common.ActionType.Update == action)
            {
                var result = db.Query<MeasureModel>("SELECT * FROM `product_measure` WHERE `code` = @code AND `id` <> @id", new { code = Model.code.Trim(), id = Model.id }).ToList();
                if (result.Count != 0)
                {
                    isError = true;
                    lstMessage.Add("[Code] is duplicate!");
                }
            }
            return isError;
        }
        public int Delete(List<int> lstID, out List<string> lstMsg)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            lstMsg = new List<string>();
            try
            {
                db.Open();
                using (var _transacsion = db.BeginTransaction())
                {
                    for (int i = 0; i < lstID.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(lstID[i].ToString()))
                        {
                            // Check inventory_measure_id
                            var result_inventory = db.Query<ItemModel>("SELECT `id` FROM `product_item` WHERE `inventory_measure_id` = @id", new { id = lstID[i] }).ToList();

                            //Check manufacture_size_measure_id
                            var result_manufacture_size = db.Query<ItemModel>("SELECT `id` FROM `product_item` WHERE `manufacture_size_measure_id` = @id", new { id = lstID[i] }).ToList();

                            //Check manufacture_weight_measure_id
                            var result_manufacture_weight = db.Query<ItemModel>("SELECT `id` FROM `product_item` WHERE `manufacture_weight_measure_id` = @id", new { id = lstID[i] }).ToList();

                            if (result_inventory.Count != 0)
                            {
                                foreach (var item in result_inventory)
                                {
                                    db.Execute("UPDATE `product_item` SET `inventory_measure_id` = 0 WHERE `id` = @id", new { id = item.id });
                                }
                            }
                            if (result_manufacture_size.Count != 0)
                            {
                                foreach (var item in result_manufacture_size)
                                {
                                    db.Execute("UPDATE `product_item` SET `manufacture_size_measure_id` = 0 WHERE `id` = @id", new { id = item.id });
                                }
                            }
                            if (result_manufacture_weight.Count != 0)
                            {
                                foreach (var item in result_manufacture_weight)
                                {
                                    db.Execute("UPDATE `product_item` SET `manufacture_weight_measure_id` = 0 WHERE `id` = @id", new { id = item.id });
                                }
                            }
                            db.Execute("DELETE FROM `product_measure` WHERE `id` = @id", new { id = lstID[i] });
                        }
                        else
                        {
                            lstMsg.Add("Measure has ID " + lstID[i].ToString() + " has been delete ");
                        }
                    }
                    _transacsion.Commit();
                }
            }
            catch (Exception ex)
            {
                returnCode = (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
        public int GetDetail(int id, out MeasureModel Model)
        {
            int returnCode = (int)Common.ReturnCode.Succeed;
            Model = new MeasureModel();
            try
            {
                if (id != 0)
                {
                    string sql = "SELECT `id`, `code`, `name`, `description` FROM `product_measure` WHERE `id` = @id";
                    Model = db.Query<MeasureModel>(sql, new { id = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                return (int)Common.ReturnCode.UnSuccess;
            }
            return returnCode;
        }
    }
}
