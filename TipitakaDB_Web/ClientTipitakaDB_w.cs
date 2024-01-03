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
        //ClientSuttaPageAssignment clientSuttaAssignment;
        //ClientMessageLog clientMessageLog;
        //ClientFileStorage clientFileStorage;
        ClientSuttaPageData clientSuttaPageData;
        ClientUserPageActivity clientUserPageActivity;
        //ClientUpdateHistory clientUpdateHistory;
        ClientKeyValueData clientKeyValueData;
        //public UserProfile? loggedinUser = null;

        //public ClientTipitakaDBLogin GetClientTipitakaDBLogin() { return clientTipitakaDBLogin; }
        public ClientUserProfile GetClientUserProfile() { return clientUserProfile; }
        public ClientActivityLog GetClientActivityLog() { return clientActivityLog; }
        public ClientSuttaInfo GetClientSuttaInfo() { return clientSuttaInfo; }
        //public ClientSuttaPageAssignment GetClientSuttaAssignment() { return clientSuttaAssignment; }
        //public ClientMessageLog GetClientMessageLog() { return clientMessageLog; }
        ////public ClientFileStorage GetClientFileStorage() { return clientFileStorage; }
        public ClientSuttaPageData GetClientSuttaPageData() { return clientSuttaPageData; }
        public ClientUserPageActivity GetClientUserPageActivity() { return clientUserPageActivity; }
        //public ClientUpdateHistory GetClientUpdateHistory() {  return clientUpdateHistory; }
        public ClientKeyValueData GetClientKeyValueData() { return clientKeyValueData; }
        public ClientTipitakaDB_w() : base("TipitakaDB")
        {
            //clientTipitakaDBLogin = new ClientTipitakaDBLogin();
            clientUserProfile = new ClientUserProfile();
            clientActivityLog = new ClientActivityLog();
            clientSuttaInfo = new ClientSuttaInfo();
            //clientSuttaAssignment = new ClientSuttaPageAssignment();
            //clientMessageLog = new ClientMessageLog();
            clientSuttaPageData = new ClientSuttaPageData(this);
            clientUserPageActivity = new ClientUserPageActivity();
            //clientUpdateHistory = new ClientUpdateHistory(this);
            clientKeyValueData = new ClientKeyValueData(this);
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
