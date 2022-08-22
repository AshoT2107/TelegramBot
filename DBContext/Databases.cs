namespace DBContext
{
    class Databases
    {
        public QuestionsDatabase questionsDb;
        public UsersDatabase usersDb;

        public Databases()
        {
            questionsDb = new QuestionsDatabase();
            usersDb = new UsersDatabase();
        }
    }
}