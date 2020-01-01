using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using tartIsh.Models;

namespace tartIsh.Controllers
{
    public class HomeController : Controller
    {
        private readonly tartIshEntities _tartIsh = new tartIshEntities();

        [Route("")]
        public ActionResult Index()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();

            ViewBag.CategoryList = getCategoryList();

            return View();
        }

        [Route("Giris-Yap")]
        public ActionResult LogIn()
        {
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        [Route("Giris-Yap")]
        public ActionResult LogIn(string username, string pass, bool? rememberme)
        {
            if (isUserExist(username, Md5Sifrele(pass)))
            {
                userLogIn(username, Md5Sifrele(pass), rememberme);

                TempData["Giris"] = true;

                return RedirectToAction("Index");
            }
            else if (!isUserExist(username, Md5Sifrele(pass)))
            {
                TempData["Giris"] = false;
                TempData["LogInMessage"] = "Kullanıcı Adı veya Şifre yanlış.";
            }
            else if (isUserBanned(getId(username)))
            {
                TempData["Giris"] = false;
                TempData["LogInMessage"] = "Hesabınız " + getUserLastBanTime(getId(username)) + "tarihine kadar erişime kapatılmıştır." +
                    " Bu kararın yanlış olduğunu düşünüyorsanız lütfen destek ekibiyle görüşünüz.";
            }
            return RedirectToAction("Index");
        }

        [Route("Cikis-Yap")]
        public ActionResult LogOff()
        {
            userLogOut();

            return RedirectToAction("Index");
        }

        [Route("Kayit-Ol")]
        public ActionResult SignUp()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Kayit-Ol")]
        public ActionResult SignUp(string firstname, string lastname, string email, int gender,string username,
            string password,System.DateTime birthday)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                return RedirectToAction("Index");
            }

            if (isUserNameExist(username))
            {
                TempData["Kayit"] = false;
                TempData["SignUpMessage"] = "Bu kullanıcı adı zaten bulunmakta.";
            }
            else if (isUserEmailExist(email))
            {
                TempData["Kayit"] = false;
                TempData["SignUpMessage"] = "Email zaten bulunmakta.";
            }
            else
            {
                TempData["Kayit"] = true;
                userSignUp(firstname, lastname, email, gender, username, password, birthday);
                TempData["SignUpMessage"] = "Başarıyla kayıt oldunuz.";
            }

            return RedirectToAction("Index");
        }

        [Route("Profilim")]
        public ActionResult UserOwnPage()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetProfileInfos(Session["username"].ToString());
                
                string user = Session["userName"].ToString();
                ViewBag.UserSubsCribe = getUserSubscribeCount(user);
                
                return View();
            }

            return RedirectToAction("Index");
        }

        [Route("Profil-Sayfasi")]
        public ActionResult UserPage(string user)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetProfileInfos(Session["username"].ToString());

                if (user == Session["userName"].ToString())
                    return RedirectToAction("UserOwnPage");
                if (string.IsNullOrEmpty(user))
                    return RedirectToAction("Index");
            }

            ViewBag.IsUserSubs = isUserAlreadySubscribed(user);
            ViewBag.UserSubscribe = getUserSubscribeCount(user);

            GetProfileInfos(user);
          
            return View();
        }

        [Route("Hesap-Ayarlari")]
        public ActionResult AccountSettings()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                TartIshUser t = getUser(getCurrentUserId());

                ViewBag.CategoryList = getCategoryList();
                ViewBag.HobbiesThatTheUserDoNotHave = HobbiesThatTheUserDoNotHave();

                return View(t);
            }

            return RedirectToAction("Index");
        }

        [Route("Gezin")]
        public ActionResult Explore()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();
            }
            ViewBag.CategoryList = getCategoryList();
            return View();
        }

        [Route("Kategoriler")]
        public ActionResult Categories()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();
            }
            ViewBag.CategoryList = getCategoryList();
            return View();
        }

        [Route("Siralama")]
        public ActionResult Placement()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();
            }

            ViewBag.mostWinners = mostWinnerList();
            ViewBag.CategoryList = getCategoryList();
            ViewBag.mostSubbers = mostSubsList();

            return View();

        }

        [HttpPost]
        [Route("hobi-ekle")]
        public ActionResult AddHobbyToUser(int hobiEkle)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                if(hobiEkle!= 0)
                {
                    addHobby(hobiEkle);
                }

                return RedirectToAction("AccountSettings");
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        [Route("hobi-sil")]
        public ActionResult DeleteHobbyFromUser(int hobiSil)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                if(hobiSil != 0)
                {
                    deleteHobby(hobiSil);
                }

                return RedirectToAction("AccountSettings");
            }

            return RedirectToAction("Index");

        }

        [Route("Iletisim")]
        public ActionResult Contact()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();
            }
            ViewBag.CategoryList = getCategoryList();
            return View();
        }

        [HttpPost]
        [Route("Iletisim")]
        public ActionResult Contact(string name, string email, string msg)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();
            }

            ViewBag.CategoryList = getCategoryList();

            sendEmail(email, msg, name);

            return View();
        }
        [Route("Mesajlarim")]
        public ActionResult UserMessages()
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                List<UserMessage> lastMessages = getUserLastMessagesList();

                return View(lastMessages);
            }

            return RedirectToAction("Index");
        }

        [Route("mesajlar")]
        public ActionResult Messages(string user)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                List<UserMessage> userMessageList = UserSentAndReceivedMessageList(user);

                ViewBag.TheOtherUser = user;
                ViewBag.userMessageList = userMessageList;

                List<UserMessage> lastMessages = getUserLastMessagesList();

                return View(lastMessages);
            }

            return RedirectToAction("Index");
        }

        [Route("Gonderilen-Mesajlar")]
        public ActionResult SentMessages(string receiver, string msg)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();
            if (isUserLoggedIn())
            {
                GetUserInfos();

                if(!string.IsNullOrEmpty(msg))
                    SendMessage(receiver, msg);

                List<UserMessage> userMessageList = UserSentAndReceivedMessageList(receiver);

                return View(userMessageList);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Takip-et")]
        public ActionResult Subscribe(string username)
        {
            if (!isUserLoggedIn()) return RedirectToAction("Index");

            GetUserInfos();

            UserSubscribe(username);

            return RedirectToAction("UserPage", new { user= username});
        }

        [Route("Tartisma")]
        public ActionResult TartIshRoom(int id)
        {
            ViewBag.isUserLoggedIn = isUserLoggedIn();

            //if (isRoomOver(id) == true) return RedirectToAction("Index"); //Will be active when live chat system made
            if(!isThereARoomIdLike(id)) 
                return RedirectToAction("Index");

            return View(GetTartIshRoom(id));
        }


        //****************** GETTER METHODS **************************
        private int getId(string userName) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.UserName == userName).Id; }
        private int getCurrentUserId() {
            string userName = Session["userName"].ToString();
            return _tartIsh.TartIshUsers.FirstOrDefault(x => x.UserName == userName).Id; 
        }
        private TartIshUser getUser(int id) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.Id == id); }
        private TartIshUser getUser(string uName) { return _tartIsh.TartIshUsers.FirstOrDefault(x => x.UserName == uName); }
        private void GetUserInfos()
        {
            int id = getCurrentUserId();

            ViewBag.Id = id;

            ViewBag.Name = getRealName(id) + " " + getRealSurname(id);

            ViewBag.RealName = getRealName(id);

            ViewBag.Surname = getRealSurname(id);

            ViewBag.UserName = getUserName(id);

            ViewBag.Email = getEmail(id);

            ViewBag.Sex = getSex(id) == true ? "Erkek" : "Kadın";

            ViewBag.UserType = getUserType(id);

           
        }
        private void GetProfileInfos(string username)
        {
            username = username.Trim();
            TartIshUser t = getUser(username);
            int id = t.Id;

            ViewBag.profileImage = t.ProfileImage;
            ViewBag.UserName = getUserName(id);
            ViewBag.userType = getUserType(id);
            ViewBag.Masteries = getUserMasterieList(id);
            ViewBag.Rosettes = getUserRosetteList(id);
            ViewBag.Hobies = getUserHobieList(id);
        }
        private string getRealName(int id) { return getUser(id).FirstName; }
        private string getRealSurname(int id) { return getUser(id).LastName; }
        private string getUserName(int id) { return getUser(id).UserName; }
        private string getEmail(int id) { return getUser(id).Email; }
        private System.DateTime getBirthDate(int id) { return getUser(id).BirthDate; }
        private bool getSex(int id) { return getUser(id).Sex; }
        private string getUserPassword(int id) { return getUser(id).UserPassword; }
        private DateTime? getUserRegisterDate(int id) { return getUser(id).RegisterDate; }
        private int getUserTypeId(int id) { return getUser(id).UserTypeId; }
        private String getUserType(int id) { return getUser(id).UserType.TypeName; }
        private int getTotalActiveteMinutes(int id) { return getUser(id).TotalActiveteMinutes; }
        private bool getIsPassive(int id) { return getUser(id).IsPassive; }
        private bool getIsDelete(int id) { return getUser(id).IsDelete; }
        private bool getIsBanned(int id) { return getUser(id).IsBanned; }
        private Nullable<System.DateTime> getLastLogIn(int id) { return getUser(id).LastLogIn; }
        private List<TartIshUser> GetTartIshUsers() { return _tartIsh.TartIshUsers.ToList(); }
        private Nullable<System.DateTime> getRegisterDate(int id) { return getUser(id).RegisterDate; }
        private Nullable<System.DateTime> getPassiveDate(int id) { return getUser(id).PassiveDate; }
        private Nullable<System.DateTime> getDeleteDate(int id) { return getUser(id).DeleteDate; }
        private List<ChatMessage> getUserChatMessageList(int id)
        {

            List<ChatMessage> userChatMessages = new List<ChatMessage>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.ChatMessages)
            {
                if (u.Id == message.SenderId)
                    userChatMessages.Add(message);
            }

            return userChatMessages;
        }
        private List<DeletedMessage> getUserDeletedChatMessageList(int id)
        {

            List<DeletedMessage> userChatMessages = new List<DeletedMessage>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.DeletedMessages)
            {
                if (u.Id == message.SenderId)
                    userChatMessages.Add(message);
            }

            return userChatMessages;
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
        private List<Category> getCategoryList()
        {
            List<Category> categories = _tartIsh.Categories.ToList();

            return categories;
        }
        private List<Hobby> getHobbyList()
        {
            List<Hobby> hobby = _tartIsh.Hobbies.ToList();

            return hobby;
        }
        private List<UserMessage> getUserLastMessagesList()
        {

            int userId = getCurrentUserId();

            List<UserMessage> uM = getUserMessageList(userId);

            List<int> userIds = new List<int>();

            List<UserMessage> lastMessages = new List<UserMessage>();

            foreach (var item in uM)
            {
                if (item.SenderId == userId && !userIds.Contains(item.ReceiverId))
                    userIds.Add(item.ReceiverId);
                if (item.ReceiverId == userId && !userIds.Contains(item.SenderId))
                    userIds.Add(item.SenderId);
            }

            foreach (var item in userIds)
            {
                int id = item;

                List<UserMessage> allUserMessages = getUserMessageList(getCurrentUserId());

                UserMessage m = allUserMessages.LastOrDefault(x => x.ReceiverId == item || x.SenderId == item);

                lastMessages.Add(m);
            }

            bool swapped;
            do
            {
                swapped = false;
                for (int i = 0; i <= lastMessages.Count() - 2; i++)
                {
                    if (lastMessages[i].SentDate < lastMessages[i + 1].SentDate)
                    {
                        UserMessage temp = lastMessages[i];
                        lastMessages[i] = lastMessages[i + 1];
                        lastMessages[i + 1] = temp;
                        swapped = true;
                    }
                }
                if (!swapped)
                {
                    break;
                }
                swapped = false;
                for (int i = lastMessages.Count() - 2; i >= 0; i--)
                {
                    if (lastMessages[i].SentDate < lastMessages[i + 1].SentDate)
                    {
                        UserMessage temp = lastMessages[i];
                        lastMessages[i] = lastMessages[i + 1];
                        lastMessages[i + 1] = temp;
                        swapped = true;
                    }
                }
            } while (swapped);

            return lastMessages;
        }
        private List<Hobby> HobbiesThatTheUserDoNotHave()
        {
            List<UserHoby> userHobies = getUserHobieList(getCurrentUserId());

            List<Hobby> hobby = _tartIsh.Hobbies.ToList(); ;

            for (int i = 0; i < hobby.Count(); i++)
            {
                for (int j = 0; j < userHobies.Count(); j++)
                {
                    if (hobby[i].Id == userHobies[j].HobbyId)
                        hobby.Remove(hobby[i]);
                }
            }

            return hobby;
        }
        private List<NotificationUser> getUserNotificationList(int id)
        {
            List<NotificationUser> list = new List<NotificationUser>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.NotificationUsers)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private List<PersonComplaint> getUserComplaintList(int id)
        {
            List<PersonComplaint> list = new List<PersonComplaint>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.PersonComplaints)
            {
                if (u.Id == message.ComplaintedId)
                    list.Add(message);
            }

            return list;
        }
        private List<PremiumUser> getUserPremiumInfoList(int id)
        {
            List<PremiumUser> list = new List<PremiumUser>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.PremiumUsers)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private List<Rating> getUserRatingList(int id)
        {
            List<Rating> list = new List<Rating>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.Ratings)
            {
                if (u.Id == message.ReceiverId)
                    list.Add(message);
            }

            return list;
        }
        private List<RoomVote> getUserRoomVoteList(int id)
        {
            List<RoomVote> list = new List<RoomVote>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.RoomVotes)
            {
                if (u.Id == message.SenderId)
                    list.Add(message);
            }

            return list;
        }
        private List<Suggestion> getUserSuggestionList(int id)
        {
            List<Suggestion> list = new List<Suggestion>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.Suggestions)
            {
                if (u.Id == message.SenderId)
                    list.Add(message);
            }

            return list;
        }
        private List<TartIshRoom> getUserCreatedTartishRoomList(int id)
        {
            List<TartIshRoom> list = new List<TartIshRoom>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.TartIshRooms)
            {
                if (u.Id == message.RoomCreatorId)
                    list.Add(message);
            }

            return list;
        }
        private List<TartIshRoom> getUserAttendedTartishRoomList(int id)
        {
            List<TartIshRoom> list = new List<TartIshRoom>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.TartIshRooms)
            {
                if (u.Id == message.RivalId)
                    list.Add(message);
            }

            return list;
        }
        private List<TartIshRoom> getUserWatchedTartishRoomList(int id)
        {
            List<TartIshRoom> list = new List<TartIshRoom>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserWatchedRooms)
            {
                if (u.Id == message.UserId)
                    list.Add(message.TartIshRoom);
            }

            return list;
        }
        private List<UserBan> getUserBanList(int id)
        {
            List<UserBan> list = new List<UserBan>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserBans)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private System.DateTime getUserLastBanTime(int id)
        {
            UserBan ban = _tartIsh.UserBans.Last(x => x.UserId == id);
            return ban.LastDate;
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
        private List<UserMessage> getUserMessageList(int id)
        {
            List<UserMessage> list = new List<UserMessage>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserMessages)
            {
                if (u.Id == message.ReceiverId || u.Id == message.SenderId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserMessage> getUserRecievedMessageList(int id)
        {
            List<UserMessage> list = new List<UserMessage>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserMessages)
            {
                if (u.Id == message.ReceiverId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserMessage> getUserSentMessageList(int id)
        {
            List<UserMessage> list = new List<UserMessage>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserMessages)
            {
                if (u.Id == message.SenderId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserRosette> getUserRosetteList(int id)
        {
            List<UserRosette> list = new List<UserRosette>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserRosettes)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserSelectedPoll> getUserSelectedPollList(int id)
        {
            List<UserSelectedPoll> list = new List<UserSelectedPoll>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserSelectedPolls)
            {
                if (u.Id == message.UserId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserSubscriber> getUserSubscriberList(int id)
        {
            List<UserSubscriber> list = new List<UserSubscriber>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserSubscribers)
            {
                if (u.Id == message.SubscribedId)
                    list.Add(message);
            }

            return list;
        }
        private List<UserSubscriber> getUserSubscribed(int id)
        {
            List<UserSubscriber> list = new List<UserSubscriber>();

            TartIshUser u = getUser(id);

            foreach (var message in _tartIsh.UserSubscribers)
            {
                if (u.Id == message.SubscriberId)
                    list.Add(message);
            }

            return list;
        }
        private List<TartIshStatu> getAllTartIshStatus()
        {
            List<TartIshStatu> tS = _tartIsh.TartIshStatus.ToList();

            return tS;
        }
        private List<UserStatu> GetAllUserStatus()
        {
            List<UserStatu> uS = _tartIsh.UserStatus.ToList();

            return uS;
        }
        private UserStatu GetUserStatus(int id)
        {
            TartIshUser tU = _tartIsh.TartIshUsers.FirstOrDefault(x => x.Id == id);

            UserStatu uS = _tartIsh.UserStatus.FirstOrDefault(x => x.Id == tU.Id);

            return uS;
        }
        private List<Chat> GetAllChats()
        {
            List<Chat> c = _tartIsh.Chats.ToList();

            return c;
        }
        private List<ChatMessage> GetAllChatMessages()
        {
            List<ChatMessage> cM = _tartIsh.ChatMessages.ToList();

            return cM;
        } 
        private List<DeletedMessage> GetAllDeletedMessages()
        {
            List<DeletedMessage> dM = _tartIsh.DeletedMessages.ToList();

            return dM;
        }
        private List<Document> GetAllDocuments()
        {
            List<Document> uS = _tartIsh.Documents.ToList();

            return uS;
        }
        private List<Document> GetUserDocuments(int userId)
        {
            List<Document> uS = _tartIsh.Documents.Where(x => x.SenderId == userId).ToList();

            return uS;
        }
        private Chat GetTartIshChat(int tartIshRoomId)
        {
            TartIshRoom t = _tartIsh.TartIshRooms.FirstOrDefault(x => x.Id == tartIshRoomId);

            Chat c = t.Chat;

            return c;
        }
        private List<ChatMessage> GetUserChatMessages(int userId)
        {
            List<ChatMessage> cM = _tartIsh.ChatMessages.Where(x => x.SenderId == userId).ToList();

            return cM;
        }
        private List<DeletedMessage> GetUserDeletedMessages(int userId)
        {
            List<DeletedMessage> dM = _tartIsh.DeletedMessages.Where(x => x.SenderId == userId).ToList();

            return dM;
        }
        private List<NotificationUser> GetMyNotification()
        {
            int id = getCurrentUserId();

            List <NotificationUser> y = _tartIsh.NotificationUsers.Where(x => x.UserId == id).ToList();

            return y;
        }
        private bool isUserNameExist(string s)
        {
            s = s.Trim();
            
            var user = _tartIsh.TartIshUsers.SingleOrDefault(x => x.UserName == s);

            bool userD = user == null ? false : true;

            return userD;
        }
        private bool isUserEmailExist(string eMail)
        {
            eMail = eMail.Trim();

            var user = _tartIsh.TartIshUsers.FirstOrDefault(x => x.Email == eMail);

            bool userM = user == null ? false : true;

            return userM;
        }
        private bool isUserExist(string userName, string password)
        {
            userName = userName.Trim();
            password = password.Trim();

            return (isUserNameExist(userName) && !string.IsNullOrEmpty(getId(userName).ToString()) && getUser(userName).UserPassword == password) ? true : false;
        }
        private bool isUserExist(int id)
        {
            var u = _tartIsh.TartIshUsers.FirstOrDefault(x => x.Id == id);

            return u != null ? true : false;
        }
        private bool isUserBanned(int id)
        {
            if (isUserExist(id))
            {
                TartIshUser tU = getUser(id);

                List<UserBan> uBL = _tartIsh.UserBans?.Where(x => x.UserId == tU.Id).ToList();

                UserBan u = new UserBan();

                if (uBL.Count != 0)
                    u = uBL[0];

                foreach (var x in uBL)
                {
                    if (getUserLastBanTime(tU.Id) > DateTime.Now) { return true; }
                }
            }
            return false;
        }
        private bool isUserLoggedIn()
        {
            return (!string.IsNullOrEmpty(Session["UserName"]?.ToString())) ? true : false;
        }
        private bool isUserAlreadySubscribed(string userName)
        {
            int id = getCurrentUserId();
            int subscribedId = getId(userName);

            UserSubscriber uS = _tartIsh.UserSubscribers.FirstOrDefault(x => x.SubscriberId == id && x.SubscribedId == subscribedId);

            return uS == null ? false : true;
        }
        private int getUserSubscribeCount(string username)
        {
            int id = getId(username);

            int count = _tartIsh.UserSubscribers.Where(x => x.SubscribedId == id).ToList().Count();

            return count;
        }
        private TartIshRoom GetTartIshRoom(int id)
        {
            TartIshRoom tR = _tartIsh.TartIshRooms.FirstOrDefault(x => x.Id == id);

            return tR;
        }
        private bool isThereARoomIdLike(int id)
        {
            TartIshRoom tR = _tartIsh.TartIshRooms.FirstOrDefault(x => x.Id == id);

            return tR == null ? false : true;
        }
        private bool isRoomOver(int id)
        {
            if (isThereARoomIdLike(id))
            {
                TartIshRoom tR = _tartIsh.TartIshRooms.FirstOrDefault(x => x.Id == id);

                return (tR.IsFinished == false || tR.IsFinished == null) ? true : false;
            }
            return true;
        }
        private Array mostWinnerList()
        {
            List<TartIshUser> allUsers = GetTartIshUsers();
            List<TartIshRoom> allRooms = _tartIsh.TartIshRooms.ToList();

            string[,] userWins = new string[allUsers.Count, 5];

            for (int i = 0; i < allUsers.Count(); i++)
            {
                userWins[i, 0] = allUsers[i].Id.ToString();
                userWins[i, 1] = allUsers[i].UserName;
                userWins[i, 4] = allUsers[i].ProfileImage;
                int wins = 0;

                for (int j = 0; j < allRooms.Count(); j++)
                {
                    bool state = allRooms[j].IsPassive == null || false ? false : true;
                    if (allUsers[i].Id == allRooms[j].WinnerId && state)
                    {
                        wins++;
                    }
                }
                userWins[i, 2] = wins.ToString();
                userWins[i, 3] = (DateTime.Now.Year - allUsers[i].RegisterDate.Value.Year).ToString();
            }

            List<int> tutulanSayilar = new List<int>();
            string[,] mostWinners = new string[8, 5];

            //sorting
            for (int i = 0; i < mostWinners.GetLength(0); i++)
            {
                int enb = 0;
                int userid = Int32.Parse(userWins[0, 0]);
                for (int j = 0; j < userWins.GetLength(0) - 1; j++)
                {
                    if (j == 0 && !tutulanSayilar.Contains(Int32.Parse(userWins[0, 0])))
                    {
                        userid = Int32.Parse(userWins[0, 0]);
                    }
                    if (i != 0 && Int32.Parse(userWins[j, 2]) < Int32.Parse(userWins[j + 1, 2]) && Int32.Parse(userWins[j + 1, 2]) > enb && !tutulanSayilar.Contains(Int32.Parse(userWins[j + 1, 0])))
                    {
                        enb = Int32.Parse(userWins[j + 1, 2]);
                        userid = Int32.Parse(userWins[j + 1, 0]);
                    }
                }
                tutulanSayilar.Add(userid);
            }

            //inserting

            for (int i = 0; i < mostWinners.GetLength(0); i++)
            {
                for (int j = 0; j < userWins.GetLength(0); j++)
                {
                    if (tutulanSayilar[i] == int.Parse(userWins[j, 0]))
                    {
                        mostWinners[i, 0] = userWins[j, 0];
                        mostWinners[i, 1] = userWins[j, 1];
                        mostWinners[i, 2] = userWins[j, 2];
                        mostWinners[i, 3] = userWins[j, 3];
                        mostWinners[i, 4] = userWins[j, 4];
                    }
                }
            }

            return mostWinners;
        }
        private Array mostSubsList()
        {
            List<TartIshUser> allUsers = GetTartIshUsers();
            List<UserSubscriber> allSubscribers = _tartIsh.UserSubscribers.ToList();

            string[,] userSubs = new string[allUsers.Count, 5];

            for (int i = 0; i < allUsers.Count(); i++)
            {
                userSubs[i, 0] = allUsers[i].Id.ToString();
                userSubs[i, 1] = allUsers[i].UserName;
                userSubs[i, 4] = allUsers[i].ProfileImage;
                int subsCount = 0;

                for (int j = 0; j < allSubscribers.Count(); j++)
                {
                    if (allUsers[i].Id == allSubscribers[j].SubscribedId)
                    {
                        subsCount++;
                    }
                }
                userSubs[i, 2] = subsCount.ToString();
                userSubs[i, 3] = (DateTime.Now.Year - allUsers[i].RegisterDate.Value.Year).ToString();
            }

            List<int> tutulanSayilar = new List<int>();
            string[,] mostSubber = new string[8, 5];

            //sorting
            for (int i = 0; i < mostSubber.GetLength(0); i++)
            {
                int enb = 0;
                int userid = Int32.Parse(userSubs[0, 0]);
                for (int j = 0; j < userSubs.GetLength(0) - 1; j++)
                {
                    if (j == 0 && Int32.Parse(userSubs[0, 0]) > 0 && !tutulanSayilar.Contains(Int32.Parse(userSubs[0, 0])))
                    {
                        userid = Int32.Parse(userSubs[0, 0]);
                    }
                    if (Int32.Parse(userSubs[j, 2]) < Int32.Parse(userSubs[j + 1, 2]) && Int32.Parse(userSubs[j + 1, 2]) > enb
                        && !tutulanSayilar.Contains(Int32.Parse(userSubs[j + 1, 0])))
                    {
                        enb = Int32.Parse(userSubs[j + 1, 2]);
                        userid = Int32.Parse(userSubs[j + 1, 0]);
                    }
                }
                tutulanSayilar.Add(userid);
            }

            //inserting

            for (int i = 0; i < mostSubber.GetLength(0); i++)
            {
                for (int j = 0; j < userSubs.GetLength(0); j++)
                {
                    if (tutulanSayilar[i] == int.Parse(userSubs[j, 0]))
                    {
                        mostSubber[i, 0] = userSubs[j, 0];
                        mostSubber[i, 1] = userSubs[j, 1];
                        mostSubber[i, 2] = userSubs[j, 2];
                        mostSubber[i, 3] = userSubs[j, 3];
                        mostSubber[i, 4] = userSubs[j, 4];
                    }
                }
            }

            return mostSubber;
        }



        // SETTER METHODS

        private void userLogIn(string userName, string pass, bool? rememberme)
        {
            bool rememberMe = rememberme == null || rememberme == false ? false : true;

            if (isUserExist(userName, pass) && rememberMe)
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
            }
            else if (isUserExist(userName, pass))
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
            }
        }
        private void sendEmail(string email, string content, string name)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true; 
            mail.To.Add("tartishmail@gmail.com"); 

            mail.From = new MailAddress(email, name, System.Text.Encoding.UTF8);
            mail.Subject = "TartIsh İletişim Maili ";

            mail.Body = "E-Posta:" + email + "isim:" + name + "Içerik:" + content;
            mail.IsBodyHtml = true;
            SmtpClient smp = new SmtpClient();

            smp.Credentials = new NetworkCredential("xxxxx", "yyyyy"); // mail and pass
            smp.Port = 587;
            smp.Host = "smtp.gmail.com";
            smp.EnableSsl = true;
            smp.Send(mail);
        }
        private void userLogOut() { Session.Abandon(); }
        private void SendMessage(string receiver, string msg)
        {

            TartIshUser sender = getUser(getCurrentUserId());
            TartIshUser receviver = getUser(receiver);

            UserMessage uM = new UserMessage
            {
                SenderId = sender.Id,
                ReceiverId = receviver.Id,
                MessageText = msg,
                SentDate = DateTime.Now,
                IsDelete = false
            };

            _tartIsh.UserMessages.Add(uM);
            _tartIsh.SaveChanges();
        }
        private void UserSubscribe(string userName)
        {
            if (isUserNameExist(userName))
            {

                int id = getCurrentUserId();
                int subscribedId = getId(userName);

                if (!isUserAlreadySubscribed(userName))
                {
                    UserSubscriber uS = new UserSubscriber
                    {
                        SubscriberId = id,
                        SubscribedId = subscribedId,
                        SubscriptionDate = DateTime.Now
                    };

                    _tartIsh.UserSubscribers.Add(uS);
                }
                else
                {
                    UserSubscriber uS = _tartIsh.UserSubscribers.FirstOrDefault(x => x.SubscriberId == id && x.SubscribedId == subscribedId);

                    _tartIsh.UserSubscribers.Remove(uS);
                }
                _tartIsh.SaveChanges();
            }
        }
        private List<UserMessage> UserSentAndReceivedMessageList(string receiver)
        {

            int currentUserId = getCurrentUserId();
            int theOtherUserId = getId(receiver);

            List<UserMessage> uM = _tartIsh.UserMessages.Where(x => x.SenderId == currentUserId && x.ReceiverId == theOtherUserId ||
            x.ReceiverId == currentUserId && x.SenderId == theOtherUserId).ToList();
            
            return uM;
        }
        private void userSignUp(string firstname, string lastname, string email, int gender, 
            string username, string password, DateTime birthday) { 
       
            TartIshUser u = new TartIshUser
        {
            FirstName = firstname,
            LastName = lastname,
            UserName = username,
            Email = email,
            BirthDate = birthday,
            Sex = gender == 1 ? true : false, 
            UserPassword = Md5Sifrele(password), 
            TotalActiveteMinutes = 0,
            UserTypeId = 3,
            IsPassive = false,
            IsDelete = false,
            IsBanned = false,
            RegisterDate = DateTime.Now,
            ProfileImage = "/images/profile-photos/kayitli_kullanici.png"
            };

         _tartIsh.TartIshUsers.Add(u);

         _tartIsh.SaveChanges();
        }
        private void addHobby(int hobbyId)
        {
            UserHoby uH = new UserHoby
            {
                UserId = getCurrentUserId(),

                HobbyId = hobbyId
            };

            _tartIsh.UserHobies.Add(uH);

            _tartIsh.SaveChanges();
        }
        private void deleteHobby(int hobbyId)
        {
            int userId = getCurrentUserId();

            UserHoby uH = _tartIsh.UserHobies.FirstOrDefault(x => x.HobbyId == hobbyId && x.UserId == userId);

            _tartIsh.UserHobies.Remove(uH);

            _tartIsh.SaveChanges();
        }
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

    }
}