using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientUserPageActivity : TipitakaDB_w
    {
        const string _UserPageActivity_ = "UserPageActivity";
        public ClientUserPageActivity() : base(_UserPageActivity_)
        { }
        public void AddUserPageActivity(UserPageActivity userPageActivity)
        {
            InsertTableRec(userPageActivity).Wait();
        }
        public void AddUserPageActivity(string email, string DocID, string activity, int StartPage, int EndPage)
        {
            UserPageActivity userPageActivity = new UserPageActivity()
            {
                PartitionKey = "UserPageActivity",
                RowKey = email + "-" + DocID,
                DocID = DocID,
                Activity = activity,
                PageRange = string.Format("{0}-{1}", StartPage, EndPage),
                Pages = EndPage - StartPage + 1
            };
            if (userPageActivity != null)
            {
                InsertTableRec(userPageActivity).Wait();
            }
        }
        public List<UserPageActivity> QueryUserPageActivity(string suttaType, string userID)
        {
            List<UserPageActivity> listUserPageActivity = new List<UserPageActivity>();
            string rowKey = String.Format("RowKey ge '{0}-000' and RowKey le '{0}-999'", suttaType);
            string userfilter = String.Format("AssignedTo eq '{0}'", userID);
            string filter = String.Format("({0}) and ({1})", rowKey, userfilter);
            QueryTableRec(filter).Wait();
            listUserPageActivity = (List<UserPageActivity>) objResult;
            return listUserPageActivity;
        }
        public UserPageActivity? GetUserPageActivity(string rowKey)
        {
            UserPageActivity? result = null;
            if (rowKey != null && rowKey.Length > 0)
            {
                RetrieveTableRec(rowKey).Wait();
                if (StatusCode == 200)
                {
                    result = (UserPageActivity)objResult;
                }
            }
            else StatusCode = -1;
            return result;
        }
        public void UpdateUserPageActivity(UserPageActivity userPageActivity)
        {
            UpdateTableRec(userPageActivity).Wait();
        }
    }
}
