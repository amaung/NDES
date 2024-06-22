using Tipitaka_DBTables;

namespace Tipitaka_DB
{
    public class ClientUserProfile : TipitakaDB_w
    {
        public const string _UserProfile_ = "UserProfile";

        //*******************************************************************
        //*** User Profile
        //*******************************************************************
        public ClientUserProfile() : base(_UserProfile_) { }

        //*******************************************************************
        //*** Get the user profile
        //*******************************************************************
#nullable enable
        public UserProfile? GetUserProfile(string rowKey)
        {
            RetrieveTableRec(rowKey).Wait();
            if (StatusCode == 200) return (UserProfile)objResult;
            return null;
        }
#nullable disable
        //*******************************************************************
        //*** Add new user profile
        //*******************************************************************
        public UserProfile AddUserProfile(string email, string nameE, string nameM = "", string user_Class = "U", int loginCount = 1)
        {
            //string pid = GetPID();
            //string encrypt = Encrypt(pwd, pid);
            //string password = Encrypt(encrypt, pid, false);
            string userClass = user_Class;
            // special accounts
            if (email == "heinsithuwin95@gmail.com") userClass = "P";
            if (email == "monksarana@gmail.com") userClass = "A";
            if (email == "dhammayaungchi2011@gmail.com") userClass = "S";
            UserProfile userProfile = new UserProfile()
            {
                PartitionKey = "UserProfile",
                RowKey = email,
                Name_E = nameE,
                Name_M = nameM,
                LoginCount = loginCount,
                //Password = encrypt,
                //PID = pid,
                UserClass = userClass,
                //Status = "Active",
                JoinedDate = DateTime.Now.ToUniversalTime(),
                LastDate = null
                //Remarks = remarks,
            };
            userProfile.LastLogin = userProfile.JoinedDate;
            InsertTableRec(userProfile).Wait();
            //userProfile.Password= pwd;
            return userProfile;
        }
        //*******************************************************************
        //*** Update new user profile
        //*******************************************************************
        public void UpdateUserProfile(UserProfile userProfile)
        {
            UserProfile? serverUserProfile = GetUserProfile(userProfile.RowKey);
            if (serverUserProfile == null) return;
            serverUserProfile.Name_E = userProfile.Name_E;
            serverUserProfile.Name_M = userProfile.Name_M;
            serverUserProfile.LoginCount = userProfile.LoginCount;
            serverUserProfile.UserClass = userProfile.UserClass;
            serverUserProfile.LastDate = userProfile.LastDate;
            UpdateTableRec(serverUserProfile).Wait();
            return;
        }
        //*******************************************************************
        //*** Delete new user profile
        //*******************************************************************
        public void DeleteUserProfile(string userID)
        {
            RetrieveTableRec(userID).Wait();
            if (StatusCode == 200)
            {
                if (objResult == null) return;
                UserProfile userProfile = (UserProfile)objResult;
                userProfile.PartitionKey = _UserProfile_;
                userProfile.RowKey = userID;
                DeleteTableRec(userProfile).Wait();
            }
        }
        //*******************************************************************
        //*** Get users meeting given qaulifications
        //*******************************************************************
        public List<UserProfile> GetUsers(string userID, string userClass, string userStatus)
        {
            string qualifier = String.Empty;
            if (userID.Length > 0) qualifier += String.Format(" and (RowKey eq '{0}')", userID);
            if (userClass.Length > 0) qualifier += String.Format(" and (UserClass eq '{0}')", userClass);
            if (userStatus.Length > 0) qualifier += String.Format(" and (Status eq '{0}')", userStatus);
            QueryTableRec(qualifier).Wait();
            List<UserProfile> listUserProfile = (List<UserProfile>)objResult;
            return listUserProfile;
        }
        //*******************************************************************
        //*** Get users meeting given qaulifications
        //*******************************************************************
        public List<UserProfile> GetAllUsers(bool inclDev = true, bool active = true)
        {
            string qualifier = String.Empty;
            if (!inclDev) qualifier += " and (UserClass ne 'S')";
            if (active) qualifier += " and (LastDate eq '')";
            //qualifier += " and (Status eq 'Active')";
            QueryTableRec(qualifier).Wait();
            List<UserProfile> listUserProfile = (List<UserProfile>)objResult;
            return listUserProfile;
        }
        //*******************************************************************
        //*** Get all admins
        //*******************************************************************
        public List<UserProfile> GetAllAdmins(bool inclDev = false)
        {
            string qualifier = String.Empty;
            if (!inclDev) qualifier += " and (UserClass ne 'D') and (UserClass eq 'A')";
            QueryTableRec(qualifier).Wait();
            List<UserProfile> listUserProfile = (List<UserProfile>)objResult;
            return listUserProfile;
        }        //*******************************************************************
        //*** Get password encryption code
        //*******************************************************************
        public string GetPID()
        {
            string pid;
            Random r = new Random(DateTime.Now.Second);
            pid = r.Next(10000, 99999).ToString();
            pid += r.Next(10000, 99999).ToString();
            return pid;
        }
        //*******************************************************************
        //*** Encrypt password
        //*******************************************************************
        public string Encrypt(string p, string pid, bool encrypt = true)
        {
            string pwd = string.Empty, s = String.Empty;
            if (p.Length == 0) return pwd;
            int offset = 50;
            int n = p.Length / pid.Length;
            while (n-- >= 0) s += pid;
            List<char> newPwd = new List<char>();
            n = 0;
            foreach (char c in p)
            {
                int m = (int)c;
                if (encrypt) m += (int)s[n] + offset; else m -= (int)s[n] + offset;
                newPwd.Add(Convert.ToChar(m));
                n++;
            }
            pwd = new string(newPwd.ToArray());
            return pwd;
        }
        //*******************************************************************
        //*** GetUSerClass
        //*******************************************************************
        public string GetUserClass(string userClassCode)
        {
            string s = String.Empty;
            switch (userClassCode)
            {
                case "U": s = "Data Entry User"; break;
                case "R": s = "Proof Reader"; break;
                case "A": s = "Administrator"; break;
                case "D": s = "Developer"; break;
            }
            return s;
        }
    }
}
