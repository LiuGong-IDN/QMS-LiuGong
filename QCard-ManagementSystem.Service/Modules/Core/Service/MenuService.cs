using Dapper;
using Hangfire.Common;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using QCard.ManagementSystem.Domain;
using QCard.ManagementSystem.Domain.Modules.Core.ViewModel;
using QCard.ManagementSystem.Infrastructure.Session;

namespace QCard.ManagementSystem.Service.Modules.Core.Service
{
    public class MenuService : DbService
    {
        public MenuService(IDbHelper db) : base(db)
        {
        }

        public DataSourceResult GetMenuDataSource()
        {
            DataSourceResult result = new DataSourceResult();
            var menus = Db.MenuRepository.GetMenuList();

            result.Data = menus;
            return result;
        }

        public BasicResponse Save(Domain.Modules.Core.Model.Menu menu)
        {
            string methodName = "Save";
            BasicResponse message = new BasicResponse();

            try
            {
                var MenuModel = new Domain.Modules.Core.Model.Menu()
                {
                    PARENT_ID = menu.PARENT_ID,
                    TITLE = menu.TITLE,
                    URL = menu.URL,
                    DESCRIPTION = menu.DESCRIPTION,
                    ICON_CLASS = menu.ICON_CLASS,
                    ORDER_INDEX = 0,
                    STATUS = true,
                    FUNCTION_ID = menu.FUNCTION_ID,
                    CREATED_BY = SessionManager.Current,
                    CREATED_DATE = DateTime.Now,
                };

                var result = Db.MenuRepository.Save(MenuModel, new string[]
                {
                    "PARENT_ID",
                    "TITLE",
                    "URL",
                    "DESCRIPTION",
                    "ICON_CLASS",
                    "ORDER_INDEX",
                    "STATUS",
                    "FUNCTION_ID",
                    "CREATED_BY",
                    "CREATED_DATE"
                });

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;
                }
                else
                {
                    throw new Exception("Failed Create Data");
                }

                new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
            }
            catch (Exception ex)
            {
                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
                message.Success = false;
                message.Message = ex.Message;
            }

            return message;
        }

        public BasicResponse Update(Domain.Modules.Core.Model.Menu menu)
        {
            string methodName = "Update";
            BasicResponse message = new BasicResponse();

            try
            {
                var MenuModel = new Domain.Modules.Core.Model.Menu()
                {
                    ID = menu.ID,
                    PARENT_ID = menu.PARENT_ID,
                    ORDER_INDEX = menu.ORDER_INDEX,
                    TITLE = menu.TITLE,
                    URL = menu.URL,
                    STATUS = menu.STATUS,
                    DESCRIPTION = menu.DESCRIPTION,
                    ICON_CLASS = menu.ICON_CLASS,
                    FUNCTION_ID = menu.FUNCTION_ID,
                    UPDATED_BY = SessionManager.Current,
                    UPDATED_DATE = DateTime.Now,
                };

                var result = Db.MenuRepository.Update(MenuModel, new string[]
                {
                    "ID",
                    "PARENT_ID",
                    "ORDER_INDEX",
                    "TITLE",
                    "URL",
                    "STATUS",
                    "DESCRIPTION",
                    "ICON_CLASS",
                    "FUNCTION_ID",
                    "UPDATED_BY",
                    "UPDATED_DATE"
                });

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;

                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
                else
                {
                    throw new Exception("Failed to Update Data");
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Message = ex.Message;

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return message;
        }

        public BasicResponse Delete(Domain.Modules.Core.Model.Menu menu)
        {
            string methodName = "Delete";
            BasicResponse message = new BasicResponse();

            try
            {
                var result = Db.MenuRepository.Delete(menu.ID);

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;

                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
                else
                {
                    throw new Exception("Failed to Delete Data");
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Message = ex.Message;

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return message;
        }

        public BasicResponse MoveMenu(Domain.Modules.Core.Model.Menu menu)
        {
            string methodName = "MoveMenu";
            BasicResponse message = new BasicResponse();

            try
            {
                var MenuModel = new Domain.Modules.Core.Model.Menu()
                {
                    ID = menu.ID,
                    PARENT_ID = menu.PARENT_ID,
                    ORDER_INDEX = menu.ORDER_INDEX,
                    STATUS = menu.STATUS,
                    TITLE = menu.TITLE,
                    URL = menu.URL,
                    DESCRIPTION = menu.DESCRIPTION,
                    ICON_CLASS = menu.ICON_CLASS,
                    FUNCTION_ID = menu.FUNCTION_ID,
                    UPDATED_BY = SessionManager.Current,
                    UPDATED_DATE = DateTime.Now,
                };

                var result = Db.MenuRepository.Update(MenuModel, new string[]
                {
                    "ID",
                    "PARENT_ID",
                    "ORDER_INDEX",
                    "STATUS",
                    "TITLE",
                    "URL",
                    "DESCRIPTION",
                    "ICON_CLASS",
                    "FUNCTION_ID",
                    "UPDATED_BY",
                    "UPDATED_DATE"
                });

                if (result > 0)
                {
                    message.Success = true;
                    message.Message = string.Empty;

                    new ActivityLogService(Db).CreateSuccessLog(methodName, this.GetType().Name);
                }
                else
                {
                    throw new Exception("Failed to Update Data");
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.Message = ex.Message;

                new ActivityLogService(Db).CreateErrorLog(methodName, this.GetType().Name, ex.Message, ex.StackTrace);
            }
            return message;
        }
    }
}
