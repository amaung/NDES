using Tipitaka_DBTables;
using NissayaEditor_Web.Data;

namespace Tipitaka_DB
{
    public class ClientTipitakaDB_w : TipitakaDB_w
    {
        //ClientTipitakaDBLogin clientTipitakaDBLogin;
        ClientUserProfile clientUserProfile;
        ClientActivityLog clientActivityLog;
        ClientSuttaInfo clientSuttaInfo;
        ClientTaskAssignmentInfo clientTaskAssignmentInfo;
        ClientTaskActivityLog clientTaskActivityLog;
        ClientSourceBookInfo clientSourceBookInfo;
        ClientSuttaPageData clientSuttaPageData;
        ClientUserPageActivity clientUserPageActivity;
        ClientCorrectionLog clientCorrectionLog;
        ClientKeyValueData clientKeyValueData;
        ClientTimesheet clientTimesheet;
        //public UserProfile? loggedinUser = null;

        //public ClientTipitakaDBLogin GetClientTipitakaDBLogin() { return clientTipitakaDBLogin; }
        public ClientUserProfile GetClientUserProfile() { return clientUserProfile; }
        public ClientActivityLog GetClientActivityLog() { return clientActivityLog; }
        public ClientSuttaInfo GetClientSuttaInfo() { return clientSuttaInfo; }
        public ClientTaskAssignmentInfo GetClientTaskAssignmentInfo() { return clientTaskAssignmentInfo; }
        public ClientTaskActivityLog GetClientTaskActivityLog() { return clientTaskActivityLog; }
        ////public ClientFileStorage GetClientFileStorage() { return clientFileStorage; }
        public ClientSuttaPageData GetClientSuttaPageData() { return clientSuttaPageData; }
        public ClientUserPageActivity GetClientUserPageActivity() { return clientUserPageActivity; }
        //public ClientUpdateHistory GetClientUpdateHistory() {  return clientUpdateHistory; }
        public ClientKeyValueData GetClientKeyValueData() { return clientKeyValueData; }
        public ClientSourceBookInfo GetClientSourceBookInfo() { return clientSourceBookInfo; }
        public ClientCorrectionLog GetClientCorrectionLog() { return clientCorrectionLog; }
        public ClientTimesheet GetClientTimesheet() { return clientTimesheet; }
        public ClientTipitakaDB_w() : base("TipitakaDB")
        {
            //clientTipitakaDBLogin = new ClientTipitakaDBLogin();
            clientUserProfile = new ClientUserProfile();
            clientActivityLog = new ClientActivityLog();
            clientSuttaInfo = new ClientSuttaInfo();
            clientTaskAssignmentInfo = new ClientTaskAssignmentInfo();
            clientTaskActivityLog = new ClientTaskActivityLog();
            clientSuttaPageData = new ClientSuttaPageData(this);
            clientUserPageActivity = new ClientUserPageActivity();
            clientSourceBookInfo = new ClientSourceBookInfo(this);
            clientKeyValueData = new ClientKeyValueData(this);
            clientSourceBookInfo = new ClientSourceBookInfo(this);
            clientCorrectionLog = new ClientCorrectionLog();
            clientTimesheet = new ClientTimesheet();
        }
        public void Login(string userID, string password, string userClass)
        {
            //clientTipitakaDBLogin.Login(userID, password, userClass);
        }
        public void Logout()
        {
            //clientTipitakaDBLogin.Logout();
        }
        public List<UserProfile> GetUsers(string userID, string userClass, string userStatus)
        {
            return clientUserProfile.GetUsers(userID, userClass, userStatus);
            
        }
        public void FileDownload(string dataType, int suttaNo, int suttaPage)
        {
            //clientFileStorage.FileDownload(dataType, suttaNo, suttaPage);
        }
        public void UploadSutta(string dataType, int suttaNo, Dictionary<int, List<NIS>> dictPages)
        {
            //string userID = (clientTipitakaDBLogin.loggedinUser != null) ? clientTipitakaDBLogin.loggedinUser.RowKey : string.Empty;
            //clientSuttaPageData.UploadSutta(userID, dataType, suttaNo, dictPages);
        }
    }
}
