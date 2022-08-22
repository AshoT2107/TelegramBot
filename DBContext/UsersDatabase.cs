using Models;

namespace DBContext
{
    class UsersDatabase
    {
        public List<UserEntity> Users = new List<UserEntity>();

        public UserEntity AddUser(long chatId,string name)
        {
            if(Users.Any(user=>user.ChatId ==chatId))
            {
                return Users.FirstOrDefault(user=>user.ChatId==chatId)!;
            }
            var user = new UserEntity(chatId,name);
            Users.Add(user);
            return user;
        }

        public UserEntity? GetUser(long chatId)
        {
           return Users.FirstOrDefault(user=>user.ChatId==chatId);
        }
        public string GetUsersText()
        {
            string usersText = "";
            for(int i=0;i<Users.Count;i++)
            {
                usersText += $"{i+1}. {Users[i].ToText()}\n";
            } 
            return usersText;
        }


    }
}