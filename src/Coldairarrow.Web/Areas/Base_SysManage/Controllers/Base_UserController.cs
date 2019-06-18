using Coldairarrow.Business.Base_SysManage;
using Coldairarrow.Entity.Base_SysManage;
using Coldairarrow.Util;
using System.Linq;
using System.Web.Mvc;

namespace Coldairarrow.Web.Areas.Base_SysManage.Controllers
{
    public class Base_UserController : BaseMvcController
    {
        #region DI

        public Base_UserController(IBase_UserBusiness sysUserBus, IPermissionManage permissionManage)
        {
            _sysUserBus = sysUserBus;
            _permissionManage = permissionManage;
        }
        IBase_UserBusiness _sysUserBus { get; }
        IPermissionManage _permissionManage { get; set; }

        #endregion

        #region 视图功能

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form(string id)
        {
            var theData = id.IsNullOrEmpty() ? new Base_User() : _sysUserBus.GetTheData(id);

            return View(theData);
        }

        public ActionResult ChangePwdForm()
        {
            return View();
        }

        public ActionResult PermissionForm(string userId)
        {
            ViewData["userId"] = userId;

            return View();
        }

        #endregion

        #region 获取数据

        public ActionResult GetDataList(Pagination pagination, string keyword)
        {
            var dataList = _sysUserBus.GetDataList(pagination, false, null, keyword);

            return DataTable_Bootstrap(dataList, pagination);
        }

        public ActionResult GetDataList_NoPagin(string values, string q)
        {
            var resList = _sysUserBus.BuildSelectResult(values, q, "RealName", "Id");

            return JsonContent(resList.ToJson());
        }

        #endregion

        #region 提交数据

        public ActionResult SaveData(Base_User theData, string Pwd, string RoleIdList)
        {
            if (!Pwd.IsNullOrEmpty())
                theData.Password = Pwd.ToMD5String();

            if (theData.Id.IsNullOrEmpty())
            {
                theData.Id = IdHelper.GetId();

                _sysUserBus.AddData(theData);
            }
            else
            {
                _sysUserBus.UpdateData(theData);
            }

            //角色设置
            if (!RoleIdList.IsNullOrEmpty())
            {
                _sysUserBus.SetUserRole(theData.Id, RoleIdList.ToList<string>());
            }

            return Success();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="theData">删除的数据</param>
        public ActionResult DeleteData(string ids)
        {
            _sysUserBus.DeleteData(ids.ToList<string>());

            return Success("删除成功！");
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="oldPwd">老密码</param>
        /// <param name="newPwd">新密码</param>
        public ActionResult ChangePwd(string oldPwd, string newPwd)
        {
            var res = _sysUserBus.ChangePwd(oldPwd, newPwd);

            return Content(res.ToJson());
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="permissions">权限</param>
        /// <returns></returns>
        public ActionResult SavePermission(string userId, string permissions)
        {
            _permissionManage.SetUserPermission(userId, permissions.ToList<string>());

            return Success();
        }

        #endregion
    }
}