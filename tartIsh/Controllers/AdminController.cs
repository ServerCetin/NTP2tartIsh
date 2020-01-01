using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using tartIsh.Models;

namespace tartIsh.Controllers
{

    public class AdminController : Controller
    {
        private readonly tartIshEntities _tartIsh = new tartIshEntities();

        [Route("Panel")]
        public ActionResult Index()
        {
            if (!isUserLoggedIn()) 
                return RedirectToAction("LogIn");
            PanelIndexInfos();
            GetUserInfos();
            return View();
        }

        [Route("Panel/LogIn")]
        public ActionResult LogIn() {
            return isUserLoggedIn() ? RedirectToAction("Index") : (ActionResult)View();
        }

        [HttpPost]
        [Route("Panel/LogIn")]
        public ActionResult LogIn(string username, string password, bool? rememberme)
        {

            if (isUserExist(username, Md5Sifrele(password))) {

                userLogIn(username, Md5Sifrele(password), rememberme);

                return RedirectToAction("Index");
            }

            ViewBag.LogInInfo = "Başarısız giriş. Lütfen bilgilerinizi kontrol ediniz.";
            return View();
        }

        [Route("Panel/LogOut")]
        public ActionResult LogOut()
        {
            if (isUserLoggedIn())
            {
                userLogOut();

                return RedirectToAction("LogIn");
            }
            return RedirectToAction("LogIn");
        }

        [Route("Panel/Kategoriler")]
        public ActionResult Categories()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.UsersCategoryMastery = GetUsersCategoryMastery();

            return View(GetCategories());
        }

        [Route("Panel/Kategori-Duzenle")]
        public ActionResult EditCategory(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(GetCategory(id));
        }

        [HttpPost]
        [Route("Panel/Kategori-Duzenle")]
        public ActionResult EditCategory(string CategoryDescription, int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            EditThisCategory(CategoryDescription, id);

            return RedirectToAction("Categories");
        }

        [Route("Panel/Rozetler")]
        public ActionResult Rosettes()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.GetUsersRosette = GetUsersRosette();

            return View(GetRosettes());
        }

        [Route("Panel/Rozet-Duzenle")]
        public ActionResult EditRosettes(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(GetRosette(id));
        }

        [HttpPost]
        [Route("Panel/Rozet-Duzenle")]
        public ActionResult EditRosettes(string RosetteImg, int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            EditThisRosette(RosetteImg, id);

            return RedirectToAction("Rosettes");
        }

        [Route("Panel/Hobiler")]
        public ActionResult Hobbies()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.GetUsersHobby = GetUsersHobby();

            return View(GetHobbies());
        }

        [Route("Panel/Hobi-Duzenle")]
        public ActionResult EditHobbies(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(GetHobby(id));
        }

        [HttpPost]
        [Route("Panel/Hobi-Duzenle")]
        public ActionResult EditHobbies(string HobbyName, string HobbyDescription, string HobbyImg, int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            EditThisHobby(HobbyName, HobbyDescription, HobbyImg, id);

            return RedirectToAction("Hobbies");
        }

        [Route("Panel/Sikayetler")]
        public ActionResult Complaints()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.GetAllPersonComplaints = GetPersonUserComplaintList();

            ViewBag.GetAllRoomComplaints = GetPersonRoomComplaintList();

            return View();
        }

        [Route("Panel/Oneriler")]
        public ActionResult Suggestions()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.GetAllUserSuggestions = GetUserSuggestiontList();

            return View();
        }

        [Route("Panel/Gorev-Listesi")]
        public ActionResult TaskList()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(GetAdminTasks());
        }

        [Route("Panel/Gorev-Ekle")]
        public ActionResult AddTask()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.Admins = getAdmins();

            return View();
        }

        [HttpPost]
        [Route("Panel/Gorev-Ekle")]
        public ActionResult AddTask(string Task, int EmployeeId, DateTime LastDate)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            AddAdminTask(Task, EmployeeId, LastDate);

            return RedirectToAction("TaskList");
        }

        [Route("Panel/Gorev-Duzenle")]
        public ActionResult EditTask(int taskId)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.Admins = getAdmins();

            AdminTask adminTask = getAdminTask(taskId);

            return View(adminTask);
        }

        [HttpPost]
        [Route("Panel/Gorev-Duzenle")]
        public ActionResult EditTask(int Id, string Task, int EmployeeId, DateTime LastDate)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            EditAdminTask(Id, Task, EmployeeId, LastDate);

            return RedirectToAction("TaskList");
        }

        [Route("Panel/Gorev-Sil")]
        public ActionResult DeleteTask(int taskId)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            DeleteAdminTask(taskId);

            return RedirectToAction("TaskList");
        }

        [Route("Panel/Kullanici-Listesi")]
        public ActionResult UserList()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(getAllUsers());
        }


        [Route("Panel/Usta-kullanicilar")]
        public ActionResult MasterUsers()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(getAllMasterUsers());
        }

        [Route("Panel/TartIsh-Odalari")]
        public ActionResult TartIshRooms()
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(GetTartIshRooms());
        }

        [Route("Panel/Kullanici-Yasakla")]
        public ActionResult BanUser(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            ViewBag.User = getUser(id);

            return View(GetBanTypes());
        }

        [HttpPost]
        [Route("Panel/Kullanici-Yasakla")]
        public ActionResult BanUser(int id,int reason)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            BanAUser(id, reason);

            return RedirectToAction("UserList");
        }


        [Route("Panel/Ustalik-Kaldir")]
        public ActionResult RemoveRMasterie(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            GetUserInfos();

            return View(getUser(id));
        }

        [HttpPost]
        [Route("Panel/Ustalik-Kaldir")]
        public ActionResult RemoveRMasterie(int id, int masterie)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            if (masterie!=0) { RemoveMasterieFromUser(id, masterie); }

            return RedirectToAction("UserList");
        }

        [Route("Panel/Odayı-Kaldir")]
        public ActionResult RemoveRoom(int id)
        {
            if (!isUserLoggedIn()) return RedirectToAction("LogIn");

            RemoveTheRoom(id);

            return RedirectToAction("TartIshRooms");
        }

       

        //Getter methods

        private int getId(string userName) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.UserName == userName).Id; }
        private TartIshUser getUser(int id) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.Id == id); }
        private TartIshUser getUser(string uName) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.UserName == uName); }
        private List<TartIshUser> getAdmins() {

            List<TartIshUser> admins = _tartIsh.TartIshUsers.Where(x => x.UserType.TypeName == "Admin").ToList();

            return admins;
        }
        private void GetUserInfos()
        {
            string username = Session["UserName"].ToString();

            int id = getId(username);

            ViewBag.Name = getRealName(id) + " " + getRealSurname(id);

            ViewBag.RealName = getRealName(id);

            ViewBag.Surname = getRealSurname(id);

            ViewBag.UserName = getUserName(id);

            ViewBag.Email = getEmail(id);

            ViewBag.Sex = getSex(id) == true ? "Erkek" : "Kadın";

            ViewBag.UserType = getUserType(id);
        }
        private List<TartIshUser> getUserList()
        {
            List<TartIshUser> list = new List<TartIshUser>();

            foreach (var user in _tartIsh.TartIshUsers)
            {
                list.Add(user);
            }

            return list;
        }
        private List<TartIshUser> getAllUsers()
        {
            List<TartIshUser> z = _tartIsh.TartIshUsers.ToList();
            return z;
        }

        private List<TartIshUser> getAllMasterUsers()
        {
            List<TartIshUser> z = _tartIsh.TartIshUsers.Where(x=>x.Masteries.Count()!=0).ToList();
            return z;
        }
        private string getRealName(int id) { return getUser(id).FirstName; }
        private string getRealSurname(int id) { return getUser(id).LastName; }
        private string getUserName(int id) { return getUser(id).UserName; }
        private string getEmail(int id) { return getUser(id).Email; }
        private System.DateTime getBirthDate(int id) { return getUser(id).BirthDate; }
        private bool getSex(int id) { return getUser(id).Sex; }
        private string getUserPassword(int id) { return getUser(id).UserPassword; }
        private int getUserTypeId(int id) { return getUser(id).UserTypeId; }
        private String getUserType(int id) { return getUser(id).UserType.TypeName; }
        private List<BanType> GetBanTypes()
        {
            List<BanType> bT = _tartIsh.BanTypes.ToList();
            return bT;
        }
        private List<Mastery> getUserMasterieList(int id)
        {
            List<Mastery> userMasteries = new List<Mastery>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.Masteries)
            {
                if (u.Id == message.UserId)
                    userMasteries.Add(message);
            }

            return userMasteries;
        }
        private List<TartIshRoom> GetTartIshRooms()
        {
            List<TartIshRoom> list = _tartIsh.TartIshRooms.ToList();

            return list;
        }
        private List<TartIshRoom> GetActiveTartIshRooms()
        {
            List<TartIshRoom> list = _tartIsh.TartIshRooms.Where(x => x.IsFinished != true).ToList();

            return list;
        }
        private List<UserHoby> getUserHobieList(int id)
        {
            List<UserHoby> list = new List<UserHoby>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserHobies)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private List<AdminTask> GetAdminTasks()
        {
            List<AdminTask> list = _tartIsh.AdminTasks.ToList();

            return list;
        }
        private AdminTask getAdminTask(int id)
        {
            AdminTask a = _tartIsh.AdminTasks.FirstOrDefault(x => x.Id == id);

            return a;
        }
        private Category GetCategory(int id) { return _tartIsh.Categories.FirstOrDefault(x => x.Id == id); }
        private List<Category> GetCategories() { return _tartIsh.Categories?.ToList(); }
        private List<Mastery> getAllMasteries() { return _tartIsh.Masteries.ToList(); }
        private List<UserRosette> GetAllUserRosettes() { return _tartIsh.UserRosettes?.ToList(); }
        private List<UserHoby> GetAllUserHobies() { return _tartIsh.UserHobies?.ToList(); }
        private List<PersonComplaint> GetAllPersonComplaints() { return _tartIsh.PersonComplaints?.ToList(); }
        private List<Suggestion> GetAllSuggestions() { return _tartIsh.Suggestions?.ToList(); }
        private List<RoomComplaint> GetAllRoomComplaints() { return _tartIsh.RoomComplaints?.ToList(); }
        private List<Rosette> GetRosettes() { return _tartIsh.Rosettes?.ToList(); }
        private Rosette GetRosette(int id) { return _tartIsh.Rosettes?.SingleOrDefault(x => x.Id == id); }
        private Hobby GetHobby(int id) { return _tartIsh.Hobbies?.SingleOrDefault(x => x.Id == id); }
        private List<Hobby> GetHobbies() { return _tartIsh.Hobbies?.ToList(); }
        private Array GetUsersCategoryMastery()
        {
            List<Category> categories = GetCategories();
            List<Mastery> masteries = getAllMasteries();
            int counter = 0;

            int[,] userCategoryMasteries = new int[categories.Count, 2];

            for (int x = 0; x < userCategoryMasteries.GetLength(0); x++)
            {

                userCategoryMasteries[x, 0] = categories[x].Id;

                if (masteries != null && masteries.Count != 0) {

                    for (int i = 0; i < masteries.Count; i++)
                    {
                        if (masteries[i].CategoryId == categories[x].Id)
                            counter++;
                    }
                    userCategoryMasteries[x, 1] = counter;

                    counter = 0;
                }
            }

            return userCategoryMasteries;
        }
        private Array GetUsersRosette()
        {

            List<UserRosette> userRosette = GetAllUserRosettes();

            List<Rosette> rosette = GetRosettes();

            int[,] rosetteUsers = new int[rosette.Count, 2];

            for (int x = 0; x < rosetteUsers.GetLength(0); x++)
            {
                int rosetteId = rosette[x].Id;

                rosetteUsers[x, 0] = rosetteId;

                rosetteUsers[x, 1] = _tartIsh.UserRosettes.Where(z => z.RosetteId == rosetteId).Count();
            }

            return rosetteUsers;
        }
        private Array GetUsersHobby()
        {
            List<UserHoby> userHobby = GetAllUserHobies();

            List<Hobby> hobby = GetHobbies();

            int counter = 0;

            int[,] hobbyUsers = new int[hobby.Count, 2];

            for (int x = 0; x < hobbyUsers.GetLength(0); x++)
            {
                if (userHobby != null && userHobby.Count != 0)
                {
                    hobbyUsers[x, 0] = hobby[x].Id;

                    for (int i = 0; i < userHobby.Count; i++)
                    {
                        if (userHobby[i].HobbyId == hobby[x].Id)
                            counter++;
                    }
                    hobbyUsers[x, 1] = counter;
                    counter = 0;
                }
            }

            return hobbyUsers;
        }
        private Array GetPersonUserComplaintList()
        {
            List<PersonComplaint> personComplaints = GetAllPersonComplaints();

            string[,] pComplaints = new string[personComplaints.Count, 4];

            for (int x = 0; x < pComplaints.GetLength(0); x++)
            {
                int complaintId = personComplaints[x].Id;
                int complainterId = personComplaints[x].SenderId;
                int complaintedId = personComplaints[x].ComplaintedId;
                string reason = personComplaints[x].Reason;

                pComplaints[x, 0] = complaintId.ToString();

                TartIshUser complainter = getUser(complainterId);
                TartIshUser complainted = getUser(complaintedId);

                pComplaints[x, 1] = complainter.UserName;

                pComplaints[x, 2] = complainted.UserName;

                pComplaints[x, 3] = reason;
            }

            return pComplaints;

        }
        private Array GetPersonRoomComplaintList()
        {
            List<RoomComplaint> roomComplaints = GetAllRoomComplaints();

            string[,] pComplaints = new string[roomComplaints.Count, 4];

            for (int x = 0; x < pComplaints.GetLength(0); x++)
            {
                int complaintId = roomComplaints[x].Id;
                int complainterId = roomComplaints[x].SenderId;
                int complaintedId = roomComplaints[x].TartIshRoomId;
                string reason = roomComplaints[x].Reason;

                pComplaints[x, 0] = complaintId.ToString();

                TartIshUser complainter = getUser(complainterId);
                TartIshUser complainted = getUser(complaintedId);

                pComplaints[x, 1] = complainter.UserName;

                pComplaints[x, 2] = complainted.UserName;

                pComplaints[x, 3] = reason;
            }

            return pComplaints;

        }
        private Array GetUserSuggestiontList()
        {
            List<Suggestion> personSuggestions = GetAllSuggestions();

            string[,] pSuggestions = new string[personSuggestions.Count, 4];

            for (int x = 0; x < pSuggestions.GetLength(0); x++)
            {
                int suggestiobId = personSuggestions[x].Id;
                int senderId = personSuggestions[x].SenderId;
                string title = personSuggestions[x].Title;
                string reason = personSuggestions[x].SuggestionText;

                pSuggestions[x, 0] = suggestiobId.ToString();

                TartIshUser sender = getUser(senderId);

                pSuggestions[x, 1] = sender.UserName;

                pSuggestions[x, 2] = title;

                pSuggestions[x, 3] = reason;
            }

            return pSuggestions;

        }
        private bool isUserNameExist(string s)
        {
            var user = _tartIsh.TartIshUsers.SingleOrDefault(x => x.UserName == s);

            return user == null ? false : true;
        }
        private bool isUserExist(string userName, string password)
        {
            return (isUserNameExist(userName) && !string.IsNullOrEmpty(getId(userName).ToString()) && getUser(userName).UserPassword == password) ? true : false;
        }
        private bool userLogIn(string userName, string pass, bool? rememberme)
        {
            bool rememberMe = rememberme == null || rememberme == false ? false : true;

            if (isUserNameExist(userName)) {
                if (isUserExist(userName, pass) && getUser(userName).UserTypeId == 1 && rememberMe)
                {
                    TartIshUser t = getUser(userName);

                    Session.Add("Id", t.Id);
                    Session.Add("userName", userName);
                    Session.Add("fullName", t.FirstName + " " + t.LastName);
                    Session.Add("firstName", t.FirstName);
                    Session.Add("lastName", t.LastName);
                    Session.Add("profileImage", t.ProfileImage);
                    Session.Add("userType", getUserType(t.Id));
                    Session.Add("userTypeId", t.Id);
                    Session.Add("sexId", getSex(t.Id));
                    string c = getSex(t.Id) == true ? "Erkek" : "Kadın";
                    Session.Add("sex", c);
                    Session.Add("masteries", getUserMasterieList(t.Id));
                    Session.Add("hobies", getUserHobieList(t.Id));
                    Session.Add("rosettes", getUserMasterieList(t.Id));

                    Session.Timeout = 105192; //for 2 year

                    return true;
                }
                else if (isUserExist(userName, pass) && getUser(userName).UserTypeId == 1)
                {
                    TartIshUser t = getUser(userName);

                    Session.Add("Id", t.Id);
                    Session.Add("userName", userName);
                    Session.Add("fullName", t.FirstName + " " + t.LastName);
                    Session.Add("firstName", t.FirstName);
                    Session.Add("lastName", t.LastName);
                    Session.Add("profileImage", t.ProfileImage);
                    Session.Add("userType", getUserType(t.Id));
                    Session.Add("userTypeId", t.Id);
                    Session.Add("sexId", getSex(t.Id));
                    string c = getSex(t.Id) == true ? "Erkek" : "Kadın";
                    Session.Add("sex", c);
                    Session.Add("masteries", getUserMasterieList(t.Id));
                    Session.Add("hobies", getUserHobieList(t.Id));
                    Session.Add("rosettes", getUserMasterieList(t.Id));

                    Session.Timeout = 120; 

                    return true;
                }
            }
            return false;
        }
        private bool isUserLoggedIn()
        {
            string username = Session["UserName"]?.ToString();

            return (!string.IsNullOrEmpty(username) && getUserType(getId(username)) == "Admin") ? true : false;
        }


        // SETTER METHODS

        private void userLogOut() { Session.Abandon(); }
        private static string Md5Sifrele(string password)
        {

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] dizi = Encoding.UTF8.GetBytes(password);
            dizi = md5.ComputeHash(dizi);

            SHA1 sha = new SHA1CryptoServiceProvider();

            string SifrelenecekVeri = dizi.ToString();

            StringBuilder sb = new StringBuilder();

            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }

            return sb.ToString();
        }
        private void PanelIndexInfos() {

            int userCount = getUserList().Count();
            int tartIshRoomCount = GetActiveTartIshRooms().Count();
            int categoriesCount = GetCategories().Count();
            int complaintsCount = GetAllPersonComplaints().Count() + GetAllRoomComplaints().Count();

            ViewBag.KayıtlıKullanici = userCount;
            ViewBag.TartIshOdaları = tartIshRoomCount;
            ViewBag.Kategoriler = categoriesCount;
            ViewBag.Sikayetler = complaintsCount;
            ViewBag.TaskList = GetAdminTasks();

        }
        private void EditThisHobby(string HobbyName, string HobbyDescription, string HobbyImg, int id)
        {
            Hobby h = _tartIsh.Hobbies?.FirstOrDefault(x => x.Id == id);

            if (h != null)
            {
                h.HobbyName = HobbyName;

                h.HobbyDescription = HobbyDescription;

                h.HobbyImg = HobbyImg;

                _tartIsh.SaveChanges();
            }
        }
        private void EditThisCategory(string CategoryDescription, int id)
        {
            Category c = _tartIsh.Categories?.FirstOrDefault(x => x.Id == id);

            if (c != null)
            {
                c.CategoryDescription = CategoryDescription;

                _tartIsh.SaveChanges();
            }
        }
        private void EditThisRosette(string RosetteImg, int id)
        {
            Rosette r = _tartIsh.Rosettes?.FirstOrDefault(x => x.Id == id);

            if (r != null)
            {
                r.RosetteImg = RosetteImg;

                _tartIsh.SaveChanges();
            }
        }
        private void AddAdminTask(string Task, int EmployeeID, DateTime LastDate)
        {
            AdminTask a = new AdminTask()
            {
                Task = Task,
                LastDate = LastDate,
            };
            if (EmployeeID != 0)
                a.EmployeeId = EmployeeID;

            _tartIsh.AdminTasks.Add(a);

            _tartIsh.SaveChanges();
        }
        private void EditAdminTask(int Id, string Task, int EmployeeId, DateTime LastDate)
        {
            AdminTask a = _tartIsh.AdminTasks.FirstOrDefault(x => x.Id == Id);

            a.Task = Task;
            if(EmployeeId != 0)
                a.EmployeeId =  EmployeeId;
            a.LastDate = LastDate;

            _tartIsh.SaveChanges();
        }
        private void DeleteAdminTask(int id)
        {
            AdminTask a = _tartIsh.AdminTasks.FirstOrDefault(x => x.Id == id);

            _tartIsh.AdminTasks.Remove(a);

            _tartIsh.SaveChanges();
        }

        private void BanAUser(int id, int banTypeId)
        {
            TartIshUser t = getUser(id);
            BanType b = _tartIsh.BanTypes.FirstOrDefault(x => x.Id == banTypeId);

            UserBan uB = new UserBan
            {
                UserId = t.Id,
                BanTypeId = banTypeId,
                LastDate = DateTime.Now.AddMinutes(b.BanDuration)
            };

            _tartIsh.UserBans.Add(uB);

            _tartIsh.SaveChanges();
        }

        private void RemoveMasterieFromUser(int id, int masterieId)
        {
            TartIshUser t = getUser(id);
            Mastery m = _tartIsh.Masteries.FirstOrDefault(x => x.Id == masterieId);

            _tartIsh.Masteries.Remove(m);
            _tartIsh.SaveChanges();
        }
        private void RemoveTheRoom(int id)
        {
            TartIshRoom t = _tartIsh.TartIshRooms.FirstOrDefault(x => x.Id == id);
            _tartIsh.TartIshRooms.Remove(t);
            _tartIsh.SaveChanges();
        }

    }
}
